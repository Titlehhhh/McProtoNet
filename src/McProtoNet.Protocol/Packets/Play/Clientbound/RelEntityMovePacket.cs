using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("RelEntityMove", PacketState.Play, PacketDirection.Clientbound)]
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[PacketId(MinecraftVersion.StartProtocol, 736, 0x28)]
[PacketId(751, 754, 0x27)]
[PacketId(755, 758, 0x29)]
[PacketId(759, 759, 0x26)]
[PacketId(760, 760, 0x28)]
[PacketId(761, 761, 0x27)]
[PacketId(762, 763, 0x2B)]
[PacketId(764, 765, 0x2C)]
[PacketId(766, 767, 0x2E)]
[PacketId(768, 769, 0x2F)]
[PacketId(770, MinecraftVersion.LatestProtocol, 0x2E)]
public sealed partial class RelEntityMovePacket : IServerPacket
{
    public int EntityId { get; set; }
    public short DX { get; set; }
    public short DY { get; set; }
    public short DZ { get; set; }
    public bool OnGround { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        writer.WriteVarInt(EntityId);
        writer.WriteSignedShort(DX);
        writer.WriteSignedShort(DY);
        writer.WriteSignedShort(DZ);
        writer.WriteBoolean(OnGround);
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        EntityId = reader.ReadVarInt();
        DX = reader.ReadSignedShort();
        DY = reader.ReadSignedShort();
        DZ = reader.ReadSignedShort();
        OnGround = reader.ReadBoolean();
    }
}