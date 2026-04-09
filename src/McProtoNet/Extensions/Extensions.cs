using McProtoNet.Net;

namespace McProtoNet;

public static class Extensions
{
    public static async ValueTask SendAndDisposeAsync(this MinecraftPacketSender sender, OutputPacket packet,
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