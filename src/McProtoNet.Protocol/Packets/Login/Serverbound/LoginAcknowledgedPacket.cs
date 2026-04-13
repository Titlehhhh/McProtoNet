using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Login.Serverbound;

[PacketInfo("LoginAcknowledged", PacketState.Login, PacketDirection.Serverbound)]
[ProtocolSupport(764, MinecraftVersion.LatestProtocol)]
[PacketId(764, MinecraftVersion.LatestProtocol, 0x03)]
public sealed partial class LoginAcknowledgedPacket : IClientPacket
{
    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        // Empty serialization as no fields were defined in the schema.
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        // Empty deserialization as no fields were defined in the schema.
    }

    void IPacket.Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);
}