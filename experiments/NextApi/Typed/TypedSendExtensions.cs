// McProtoNet.Next experiment — typed floor. Lives next to the old API, changes nothing in src/**.
// API-review follow-up: the typed floor had only a read door; the naming canon demands
// symmetric Read*/Send* pairs, and the canonical bot pseudocode sends with
// client.SendAsync(new ...Packet(...), pv). This is that missing twin, shaped after the
// old world's ClientPacketExtensions.SendAsync.

using System;
using System.Threading;
using System.Threading.Tasks;
using McProtoNet.Protocol;
using McProtoNet.Serialization;

namespace McProtoNet.Next;

/// <summary>
///     The send half of the typed floor: the wire id comes from the packet type, the body is
///     serialized by the packet itself, and the result leaves through the packet door
///     (<see cref="MinecraftClient.SendPacketAsync" />) under the current compression and
///     encryption settings. The twin of <see cref="TypedReadExtensions.ReadTypedAsync" />.
/// </summary>
public static class TypedSendExtensions
{
    /// <summary>
    ///     Sends one typed packet: resolves the wire id of <typeparamref name="T" /> for the
    ///     protocol version, serializes the body, and sends both as a single frame.
    /// </summary>
    /// <typeparam name="T">The packet type to send.</typeparam>
    /// <param name="client">The client to send through.</param>
    /// <param name="packet">The packet to serialize and send.</param>
    /// <param name="protocolVersion">The protocol version that defines the wire id and body layout.</param>
    /// <param name="cancellationToken">A token to cancel the send.</param>
    /// <returns>A task that completes when the frame has been handed to the transport.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="client" /> or <paramref name="packet" /> is <see langword="null" />.</exception>
    /// <exception cref="UnsupportedVersionException"><typeparamref name="T" /> does not exist on <paramref name="protocolVersion" />.</exception>
    /// <exception cref="ObjectDisposedException">The client has been disposed.</exception>
    /// <exception cref="InvalidOperationException">The transport is closed; await <see cref="MinecraftClient.Completion" /> for the root cause.</exception>
    /// <exception cref="OperationCanceledException">The send was canceled through <paramref name="cancellationToken" />.</exception>
    public static ValueTask SendAsync<T>(this MinecraftClient client, T packet, int protocolVersion,
        CancellationToken cancellationToken = default)
        where T : class, IPacket<T>
    {
        ArgumentNullException.ThrowIfNull(client);
        ArgumentNullException.ThrowIfNull(packet);
        if (!T.TryGetPacketId(protocolVersion, out var id))
            throw new UnsupportedVersionException(T.Identity.Key, protocolVersion);
        return SendAsyncCore(client, packet, id, protocolVersion, cancellationToken);
    }

    private static async ValueTask SendAsyncCore<T>(MinecraftClient client, T packet, int id,
        int protocolVersion, CancellationToken cancellationToken)
        where T : class, IPacket<T>
    {
        // One writer per thread instead of one per packet, in the shape Utf8JsonWriter uses:
        // rent outside the try, return in the finally, exactly once, on every path including a
        // throw from Write. The rent clears the thread slot, so a concurrent or reentrant send
        // gets its own writer.
        var writer = MinecraftPrimitiveWriterCache.Rent();
        try
        {
            // The body only — the packet door writes the VarInt id itself. WrittenMemory goes
            // straight to the door, with no pooled copy in between: SendPacketAsync copies the
            // body into its own frame buffer and says so in its contract, and the writer stays
            // rented until that ValueTask completes.
            packet.Write(writer, protocolVersion);
            await client.SendPacketAsync(id, writer.WrittenMemory, cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            MinecraftPrimitiveWriterCache.Return(writer);
        }
    }
}
