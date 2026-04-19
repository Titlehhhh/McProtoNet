using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("EntityLook", PacketState.Play, PacketDirection.Clientbound)]
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[PacketId(MinecraftVersion.StartProtocol, 736, 0x2A)]
[PacketId(751, 754, 0x29)]
[PacketId(755, 758, 0x2B)]
[PacketId(759, 759, 0x28)]
[PacketId(760, 760, 0x2A)]
[PacketId(761, 761, 0x29)]
[PacketId(762, 763, 0x2D)]
[PacketId(764, 765, 0x2E)]
[PacketId(766, 767, 0x30)]
[PacketId(768, 769, 0x32)]
[PacketId(770, MinecraftVersion.LatestProtocol, 0x31)]
public sealed partial class EntityLookPacket : IServerPacket
{
    public int EntityId { get; set; }
    public sbyte Yaw { get; set; }
    public sbyte Pitch { get; set; }
    public bool OnGround { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        writer.WriteVarInt(EntityId);
        writer.WriteSignedByte(Yaw);
        writer.WriteSignedByte(Pitch);
        writer.WriteBoolean(OnGround);
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        EntityId = reader.ReadVarInt();
        Yaw = reader.ReadSignedByte();
        Pitch = reader.ReadSignedByte();
        OnGround = reader.ReadBoolean();
    }
}