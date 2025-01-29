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


    public static ValueTask SendPacket(this IMinecraftClient client, IClientPacket packet)
    {
        if (packet.IsSupportedVersion(client.ProtocolVersion))
        {
            MinecraftPrimitiveWriter writer = new MinecraftPrimitiveWriter();
            try
            {
                int packetId = PacketIdHelper.GetPacketId(client.ProtocolVersion, packet.GetPacketId());
                writer.WriteVarInt(packetId);
                packet.Serialize(ref writer, client.ProtocolVersion);
                var memoryOwner = writer.GetWrittenMemory();
                var outputPacket = new OutputPacket(memoryOwner);
                return client.SendPacket(outputPacket);
            }
            finally
            {
                writer.Dispose();
            }
        }

        throw new ProtocolNotSupportException(packet.GetPacketId().ToString(), client.ProtocolVersion);
    }
}