using McProtoNet.Primitives;

namespace McProtoNet.Transport.Framing;

public static class Extensions
{
    /// <summary>Writes a rented packet and returns its buffer to the pool afterwards.</summary>
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
