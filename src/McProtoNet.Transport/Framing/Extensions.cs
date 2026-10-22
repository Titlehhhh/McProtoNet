using McProtoNet.Primitives;

namespace McProtoNet.Transport.Framing;

/// <summary>
/// Provides extension methods for <see cref="PacketStreamWriter"/>.
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Asynchronously writes a packet and releases the buffer it owns.
    /// </summary>
    /// <param name="writer">The writer to write the packet to.</param>
    /// <param name="packet">The packet to write. It is disposed whether the write succeeds or
    /// fails.</param>
    /// <param name="token">The token to monitor for cancellation requests. The default value is
    /// <see cref="CancellationToken.None"/>.</param>
    /// <returns>A task that represents the asynchronous write operation.</returns>
    /// <exception cref="ObjectDisposedException"><paramref name="writer"/> has already been
    /// disposed.</exception>
    /// <exception cref="InvalidOperationException">Another write on <paramref name="writer"/> is
    /// already in progress.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception
    /// is stored into the returned task.</exception>
    public static async ValueTask WriteAndDisposeAsync(this PacketStreamWriter writer, OutgoingPacket packet,
        CancellationToken token = default)
    {
        try
        {
            await writer.WritePacketAsync(packet.Memory, token).ConfigureAwait(false);
        }
        finally
        {
            packet.Dispose();
        }
    }
}
