using McProtoNet.Primitives;
namespace McProtoNet.Transport.Framing;

public static class Extensions
{
    public static async ValueTask SendAndDisposeAsync(this PacketStreamWriter sender, OutgoingPacket packet,
        CancellationToken token)
    {
        try
        {
            await sender.SendPacketAsync(packet.Memory, token);
        }
        finally
        {
            packet.Dispose();
        }
    }
}