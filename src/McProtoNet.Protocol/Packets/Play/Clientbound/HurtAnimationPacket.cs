using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("HurtAnimation", PacketState.Play, PacketDirection.Clientbound)]
[ProtocolSupport(762, MinecraftVersion.LatestProtocol)]
[PacketId(762, 763, 0x21)]
[PacketId(764, 765, 0x22)]
[PacketId(766, 767, 0x24)]
[PacketId(768, 769, 0x25)]
[PacketId(770, MinecraftVersion.LatestProtocol, 0x24)]
public sealed partial class HurtAnimationPacket : IServerPacket
{
    public int EntityId { get; set; }
    public float Yaw { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        writer.WriteVarInt(EntityId);
        writer.WriteFloat(Yaw);
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        EntityId = reader.ReadVarInt();
        Yaw = reader.ReadFloat();
    }
}