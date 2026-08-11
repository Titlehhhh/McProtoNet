// McProtoNet.Next experiment — typed floor. Lives next to the old API, changes nothing in src/**.

using System;
using System.Collections.Generic;
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
    ///     Reads incoming packets as decoded packet objects. Every element is either a generated
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
    ///     A stream of decoded clientbound packets. Each yielded object owns its data and may be
    ///     stored, queued, or awaited on freely — no transport window escapes this method.
    /// </returns>
    /// <remarks>
    ///     <para>
    ///         Decoding happens inside the loop, synchronously, before the next transport read —
    ///         the window rule of <see cref="InputPacket.Data" /> is honored internally and never
    ///         reaches the consumer. An unknown id is a normal condition of the stream, not an
    ///         error; a known packet with a broken body, however, throws from the decoder and
    ///         ends the enumeration — a protocol error is fatal on the typed floor.
    ///     </para>
    ///     <para>
    ///         The element type is <see cref="object" /> because the generated packet classes do
    ///         not implement a non-generic packet interface yet — an internal prototype
    ///         (<see cref="IPacketBase" />) records the shape the generator could emit, and it
    ///         stays out of the public surface until the generator does. Packets are classes, so
    ///         yielding them as <see cref="object" /> is a reference conversion — no boxing.
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
    public static IAsyncEnumerable<object> ReadTypedAsync(this MinecraftClient client,
        Func<PacketPhase> phaseProvider, int protocolVersion, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(client);
        ArgumentNullException.ThrowIfNull(phaseProvider);
        return ReadTypedAsyncCore(client, phaseProvider, protocolVersion, cancellationToken);
    }

    /// <summary>
    ///     Reads incoming packets as decoded packet objects for a phase that never changes.
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
    public static IAsyncEnumerable<object> ReadTypedAsync(this MinecraftClient client,
        PacketPhase phase, int protocolVersion, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(client);
        if (phase > PacketPhase.Play)
            throw new ArgumentOutOfRangeException(nameof(phase), phase, "Unknown protocol phase.");
        return ReadTypedAsyncCore(client, phase, protocolVersion, cancellationToken);
    }

    private static async IAsyncEnumerable<object> ReadTypedAsyncCore(MinecraftClient client,
        PacketPhase phase, int protocolVersion,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await foreach (var raw in client.ReadPacketsAsync(cancellationToken).ConfigureAwait(false))
        {
            var visitor = new DecodeAccumulator(phase);
            PacketFlow.Dispatch(in raw, protocolVersion, phase, PacketDirection.Clientbound, ref visitor);
            yield return visitor.Result!;
        }
    }

    private static async IAsyncEnumerable<object> ReadTypedAsyncCore(MinecraftClient client,
        Func<PacketPhase> phaseProvider, int protocolVersion,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await foreach (var raw in client.ReadPacketsAsync(cancellationToken).ConfigureAwait(false))
        {
            // Decode now, synchronously: raw.Data is a window into the transport buffer and
            // dies on the next MoveNextAsync. What we yield is fully materialized.
            var currentPhase = phaseProvider();
            var visitor = new DecodeAccumulator(currentPhase);
            PacketFlow.Dispatch(in raw, protocolVersion, currentPhase, PacketDirection.Clientbound, ref visitor);
            yield return visitor.Result!; // Dispatch always ends in Visit or Unknown.
        }
    }

    /// <summary>
    ///     Visitor-accumulator: catches the result of one <see cref="PacketFlow.Dispatch" /> call.
    ///     <see cref="Visit{T}" /> stores the decoded packet (a class — the assignment to
    ///     <see cref="object" /> is a reference conversion, no boxing); <see cref="Unknown" />
    ///     stores an <see cref="UnknownPacket" />. A struct passed by <c>ref</c>: no allocation,
    ///     no interface dispatch.
    /// </summary>
    private struct DecodeAccumulator : IPacketVisitor
    {
        private readonly PacketPhase _phase;
        public object? Result;

        public DecodeAccumulator(PacketPhase phase)
        {
            _phase = phase;
            Result = null;
        }

        public void Visit<T>(T packet) where T : class, IPacket<T> => Result = packet;

        // Deliberately takes nothing from the raw window (showcase variant В: no raw data
        // leaves the typed floor) — only the id survives; the window dies on the next read.
        public void Unknown(in InputPacket raw) => Result = new UnknownPacket(raw.Id, _phase);
    }
}

/// <summary>
///     Element of the typed stream for a wire id the registry cannot map in the current phase.
///     Carries only the id and the phase — deliberately no body: the raw bytes live in a
///     transport window that dies on the next read, and the typed floor does not hand out raw
///     windows. Consumers who need the body read the raw floor (<c>ReadPacketsAsync</c>)
///     instead. An unknown id is a normal condition of the stream, not an error.
/// </summary>
public sealed class UnknownPacket
{
    /// <summary>Initializes an unknown-packet marker.</summary>
    /// <param name="id">The wire id that had no mapping.</param>
    /// <param name="phase">The protocol phase in which the packet arrived.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     <paramref name="phase" /> is not a defined value.
    /// </exception>
    public UnknownPacket(int id, PacketPhase phase)
    {
        if (phase > PacketPhase.Play)
            throw new ArgumentOutOfRangeException(nameof(phase), phase, "Unknown protocol phase.");
        Id = id;
        Phase = phase;
    }

    /// <summary>Gets the wire id that had no mapping in the phase's registry.</summary>
    public int Id { get; }

    /// <summary>Gets the protocol phase in which the packet arrived.</summary>
    public PacketPhase Phase { get; }
}
