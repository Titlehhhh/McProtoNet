using DotNext.Buffers;
using McProtoNet.Abstractions;
using McProtoNet.Client;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol;

public static class WriteExtensions
{
    public static bool TrySend<T>(this MinecraftClient protocol, out PacketSender<T> sender)
        where T : IClientPacket, new()
    {
        if (T.SupportedVersion(protocol.ProtocolVersion))
        {
            sender = new PacketSender<T>(protocol);
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

    public static ValueTask SendPacket<T>(this MinecraftClient client, T packet) where T : IClientPacket
    {
        if (T.SupportedVersion(client.ProtocolVersion))
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

        throw new ProtocolNotSupportException(nameof(T.PacketId), client.ProtocolVersion);
    }
}