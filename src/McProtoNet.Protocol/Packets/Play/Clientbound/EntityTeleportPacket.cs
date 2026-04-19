using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("EntityTeleport", PacketState.Play, PacketDirection.Clientbound)]
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[PacketId(MinecraftVersion.StartProtocol, 736, 0x56)]
[PacketId(751, 754, 0x56)]
[PacketId(755, 756, 0x61)]
[PacketId(757, 758, 0x62)]
[PacketId(759, 759, 0x63)]
[PacketId(760, 760, 0x66)]
[PacketId(761, 761, 0x64)]
[PacketId(762, 763, 0x68)]
[PacketId(764, 764, 0x6B)]
[PacketId(765, 765, 0x6D)]
[PacketId(766, 767, 0x70)]
[PacketId(768, 769, 0x77)]
[PacketId(770, MinecraftVersion.LatestProtocol, 0x76)]
public sealed partial class EntityTeleportPacket : IServerPacket
{
    public int EntityId { get; set; }
    public double X { get; set; }
    public double Y { get; set; }
    public double Z { get; set; }
    public sbyte Yaw { get; set; }
    public sbyte Pitch { get; set; }
    public bool OnGround { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        writer.WriteVarInt(EntityId);
        writer.WriteDouble(X);
        writer.WriteDouble(Y);
        writer.WriteDouble(Z);
        writer.WriteSignedByte(Yaw);
        writer.WriteSignedByte(Pitch);
        writer.WriteBoolean(OnGround);
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        EntityId = reader.ReadVarInt();
        X = reader.ReadDouble();
        Y = reader.ReadDouble();
        Z = reader.ReadDouble();
        Yaw = reader.ReadSignedByte();
        Pitch = reader.ReadSignedByte();
        OnGround = reader.ReadBoolean();
    }
}