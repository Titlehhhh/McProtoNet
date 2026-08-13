// McProtoNet.Next experiment — typed floor. Lives next to the old API, changes nothing in src/**.

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using McProtoNet.Net;
using McProtoNet.Protocol;

namespace McProtoNet.Next;

/// <summary>
///     The typed floor of <see cref="MinecraftClient" />: a stream of decoded packet objects
///     instead of raw id-plus-window pairs. This is the main door for bots; the raw floor
///     (<c>ReadPacketsAsync</c>) stays open for experts.
/// </summary>
public static class TypedReadExtensions
{
    /// <summary>
    ///     Reads incoming packets as decoded packets. Every element is either a generated
    ///     packet class — switch on the concrete type — or an <see cref="UnknownPacket" /> when
    ///     the registry does not map the wire id in the current phase.
    /// </summary>
    /// <param name="client">The client to read from.</param>
    /// <param name="phaseProvider">
    ///     Callback that reports the current protocol phase. It is sampled once per received
    ///     packet, before decoding, so the consumer can move the connection through
    ///     login → configuration → play between packets.
    /// </param>
    /// <param name="protocolVersion">The protocol version used to decode packet bodies.</param>
    /// <param name="cancellationToken">Token to cancel the enumeration.</param>
    /// <returns>
    ///     A stream of decoded clientbound packets. Each yielded packet owns its data and may be
    ///     stored, queued, or awaited on freely — no transport window escapes this method.
    /// </returns>
    /// <remarks>
    ///     <para>
    ///         Decoding happens inside the loop, synchronously, before the next transport read —
    ///         the window rule of <see cref="InputPacket.Data" /> is honored internally and never
    ///         reaches the consumer. An unknown id is a normal condition of the stream, not an
    ///         error; a known packet with a broken body, however, ends the enumeration with a
    ///         <see cref="PacketDecodeException" /> — a protocol error is fatal on the typed floor.
    ///     </para>
    ///     <para>
    ///         The element type is <see cref="IPacket" />: every generated packet implements it,
    ///         and so does <see cref="UnknownPacket" />, so a switch over this stream stays one
    ///         world — mapped and unmapped packets share a type. Packets are classes, so yielding
    ///         them behind the interface is a reference conversion — no boxing.
    ///     </para>
    /// </remarks>
    /// <example>
    ///     <code>
    /// await foreach (var packet in client.ReadTypedAsync(() => bot.Phase, pv, ct))
    ///     switch (packet)
    ///     {
    ///         case Packets.Play.Clientbound.KeepAlivePacket k:
    ///             // answer with the serverbound twin
    ///             break;
    ///         case UnknownPacket u:
    ///             // normal: id {u.Id} has no mapping in phase {u.Phase}
    ///             break;
    ///     }
    ///     </code>
    /// </example>
    /// <exception cref="ArgumentNullException">
    ///     <paramref name="client" /> or <paramref name="phaseProvider" /> is <c>null</c>.
    /// </exception>
    /// <exception cref="PacketDecodeException">
    ///     A known packet's body did not decode on the given protocol version — a protocol
    ///     error is fatal on the typed floor (see remarks).
    /// </exception>
    /// <exception cref="OperationCanceledException">
    ///     Thrown from the enumeration when <paramref name="cancellationToken" /> is canceled.
    /// </exception>
    public static IAsyncEnumerable<IPacket> ReadTypedAsync(this MinecraftClient client,
        Func<PacketPhase> phaseProvider, int protocolVersion, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(client);
        ArgumentNullException.ThrowIfNull(phaseProvider);
        return ReadTypedAsyncCore(client, phaseProvider, protocolVersion, cancellationToken);
    }

    /// <summary>
    ///     Reads incoming packets as decoded packets for a phase that never changes.
    /// </summary>
    /// <param name="client">The client to read from.</param>
    /// <param name="phase">The protocol phase every packet is decoded in.</param>
    /// <param name="protocolVersion">The protocol version used to decode packet bodies.</param>
    /// <param name="cancellationToken">Token to cancel the enumeration.</param>
    /// <returns>A stream of decoded clientbound packets; see the phase-provider overload.</returns>
    /// <remarks>
    ///     For exchanges that live in a single phase — a status query, a one-phase sniffer, a
    ///     test fixture — where the callback overload would only ever return a constant. It also
    ///     skips the per-packet delegate call.
    /// </remarks>
    /// <exception cref="ArgumentNullException"><paramref name="client" /> is <c>null</c>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="phase" /> is not a defined value.</exception>
    /// <exception cref="PacketDecodeException">A known packet's body did not decode.</exception>
    /// <exception cref="OperationCanceledException">The enumeration was canceled.</exception>
    public static IAsyncEnumerable<IPacket> ReadTypedAsync(this MinecraftClient client,
        PacketPhase phase, int protocolVersion, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(client);
        if (phase > PacketPhase.Play)
            throw new ArgumentOutOfRangeException(nameof(phase), phase, "Unknown protocol phase.");
        return ReadTypedAsyncCore(client, phase, protocolVersion, cancellationToken);
    }

    private static async IAsyncEnumerable<IPacket> ReadTypedAsyncCore(MinecraftClient client,
        PacketPhase phase, int protocolVersion,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await foreach (var raw in client.ReadPacketsAsync(cancellationToken).ConfigureAwait(false))
        {
            IPacket packet = Decode(in raw, protocolVersion, phase);
            yield return packet;
        }
    }

    private static async IAsyncEnumerable<IPacket> ReadTypedAsyncCore(MinecraftClient client,
        Func<PacketPhase> phaseProvider, int protocolVersion,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await foreach (var raw in client.ReadPacketsAsync(cancellationToken).ConfigureAwait(false))
        {
            // Decode now, synchronously: raw.Data is a window into the transport buffer and
            // dies on the next MoveNextAsync. What we yield is fully materialized.
            var currentPhase = phaseProvider();
            IPacket packet = Decode(in raw, protocolVersion, currentPhase);
            yield return packet;
        }
    }

    /// <summary>
    ///     One raw packet in, one decoded packet out, through the generated
    ///     <see cref="PacketFlow.TryDecode" /> — no visitor of our own: an unmapped id comes back
    ///     as an <see cref="UnknownPacket" />, a broken body as <c>false</c> plus a reason. The
    ///     typed floor is the door that treats that reason as fatal, so it converts the false
    ///     into the throw its contract promises. A consumer who wants to survive one bad packet
    ///     reads the raw floor and calls <c>TryDecode</c> itself.
    /// </summary>
    private static IPacket Decode(in InputPacket raw, int protocolVersion, PacketPhase phase)
    {
        if (PacketFlow.TryDecode(in raw, protocolVersion, phase, PacketDirection.Clientbound,
                out IPacket? packet, out DecodeError error))
            return packet;

        // PacketDecodeException names a type, and a body that did not decode has no type to
        // name — TryDecode reports the reason instead of the exception, and the packet class
        // never materialized. So the interface stands in for the type and the wire facts
        // travel as the inner exception, which is the only place left for them.
        throw new PacketDecodeException(typeof(IPacket), error,
            new InvalidDataException(
                $"Packet id 0x{raw.Id:X2} in phase {phase} did not decode on protocol {protocolVersion}."));
    }
}
