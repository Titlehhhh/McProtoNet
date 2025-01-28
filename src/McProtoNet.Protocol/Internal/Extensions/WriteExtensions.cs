using DotNext.Buffers;
using McProtoNet.Abstractions;
using McProtoNet.Client;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol;

public static class WriteExtensions
{
    public static bool TrySend<T>(this IMinecraftClient client, out PacketSender<T> sender)
        where T : IClientPacket, new()
    {
        if (T.IsSupportedVersionStatic(client.ProtocolVersion))
        {
            sender = new PacketSender<T>(client);
            return true;
        }

        sender = default;
        return false;
    }

    public static async ValueTask SendAndDisposeAsync(this IMinecraftClient client, MemoryOwner<byte> data)
    {
        try
        {
            await client.SendPacket(data.Memory);
        }
        finally
        {
            data.Dispose();
        }
    }

    public static ValueTask SendPacket<T>(this IMinecraftClient client, T packet) where T : IClientPacket
    {
        if (T.IsSupportedVersionStatic(client.ProtocolVersion))
        {
            MinecraftPrimitiveWriter writer = new MinecraftPrimitiveWriter();
            try
            {
                int packetId = PacketIdHelper.GetPacketId(client.ProtocolVersion, packet.GetPacketId());
                writer.WriteVarInt(packetId);
                packet.Serialize(ref writer, client.ProtocolVersion);
                return client.SendAndDisposeAsync(writer.GetWrittenMemory());
            }
            finally
            {
                writer.Dispose();
            }
        }

        throw new ProtocolNotSupportException(packet.GetPacketId().ToString(), client.ProtocolVersion);
    }
}