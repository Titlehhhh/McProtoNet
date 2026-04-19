using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;

[PacketInfo("TestInstanceBlockAction", PacketState.Play, PacketDirection.Serverbound)]
[ProtocolSupport(770, MinecraftVersion.LatestProtocol)]
[PacketId(770, 770, 0x3C)]
[PacketId(771, MinecraftVersion.LatestProtocol, 0x3E)]
public sealed partial class TestInstanceBlockActionPacket : IClientPacket
{
    public Position Pos { get; set; }
    public int Action { get; set; }
    public string? Test { get; set; }
    public Vec3i Size { get; set; }
    public int Rotation { get; set; }
    public bool IgnoreEntities { get; set; }
    public int Status { get; set; }
    public NbtTag? ErrorMessage { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        writer.WriteType<Position>(Pos, protocolVersion);
        writer.WriteVarInt(Action);
        writer.WriteString(Test);
        writer.WriteType<Vec3i>(Size, protocolVersion);
        writer.WriteVarInt(Rotation);
        writer.WriteBoolean(IgnoreEntities);
        writer.WriteVarInt(Status);
        writer.WriteAnonymousNbtTag(ErrorMessage, protocolVersion);
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        Pos = reader.ReadType<Position>(protocolVersion);
        Action = reader.ReadVarInt();
        Test = reader.ReadString();
        Size = reader.ReadType<Vec3i>(protocolVersion);
        Rotation = reader.ReadVarInt();
        IgnoreEntities = reader.ReadBoolean();
        Status = reader.ReadVarInt();
        ErrorMessage = reader.ReadAnonymousNbtTag(protocolVersion);
    }
}