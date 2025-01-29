using System.Buffers;
using System.Runtime.CompilerServices;
using DotNext.Buffers;
using McProtoNet.Abstractions;
using McProtoNet.Net;

namespace McProtoNet;

public static class Extensions
{
    public static async ValueTask SendAndDisposeAsync(this MinecraftPacketSender sender, OutputPacket packet,
        CancellationToken token)
    {
        try
        {
            await sender.SendPacketAsync(packet, token);
        }
        finally
        {
            packet.Dispose();
        }
    }
}