using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;

[PacketInfo("PickItemFromEntity", PacketState.Play, PacketDirection.Serverbound)]
[ProtocolSupport(769, MinecraftVersion.LatestProtocol)]
[PacketId(769, 770, 0x23)]
[PacketId(771, MinecraftVersion.LatestProtocol, 0x24)]
public sealed partial class PickItemFromEntityPacket : IClientPacket
{
    public int EntityId { get; set; }
    public bool IncludeData { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        writer.WriteVarInt(EntityId);
        writer.WriteBoolean(IncludeData);
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        EntityId = reader.ReadVarInt();
        IncludeData = reader.ReadBoolean();
    }
}