using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("EntityMoveLook", PacketState.Play, PacketDirection.Clientbound)]
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[PacketId(MinecraftVersion.StartProtocol, 736, 0x29)]
[PacketId(751, 754, 0x28)]
[PacketId(755, 758, 0x2A)]
[PacketId(759, 759, 0x27)]
[PacketId(760, 760, 0x29)]
[PacketId(761, 761, 0x28)]
[PacketId(762, 763, 0x2C)]
[PacketId(764, 765, 0x2D)]
[PacketId(766, 767, 0x2F)]
[PacketId(768, 769, 0x30)]
[PacketId(770, MinecraftVersion.LatestProtocol, 0x2F)]
public sealed partial class EntityMoveLookPacket : IServerPacket
{
    public int EntityId { get; set; }
    public short DX { get; set; }
    public short DY { get; set; }
    public short DZ { get; set; }
    public sbyte Yaw { get; set; }
    public sbyte Pitch { get; set; }
    public bool OnGround { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        writer.WriteVarInt(EntityId);
        writer.WriteSignedShort(DX);
        writer.WriteSignedShort(DY);
        writer.WriteSignedShort(DZ);
        writer.WriteSignedByte(Yaw);
        writer.WriteSignedByte(Pitch);
        writer.WriteBoolean(OnGround);
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        EntityId = reader.ReadVarInt();
        DX = reader.ReadSignedShort();
        DY = reader.ReadSignedShort();
        DZ = reader.ReadSignedShort();
        Yaw = reader.ReadSignedByte();
        Pitch = reader.ReadSignedByte();
        OnGround = reader.ReadBoolean();
    }
}