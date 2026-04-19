using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;

[PacketInfo("UseEntity", PacketState.Play, PacketDirection.Serverbound)]
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[PacketId(MinecraftVersion.StartProtocol, 736, 0x0E)]
[PacketId(751, 754, 0x0E)]
[PacketId(755, 758, 0x0D)]
[PacketId(759, 759, 0x0F)]
[PacketId(760, 760, 0x10)]
[PacketId(761, 761, 0x0F)]
[PacketId(762, 763, 0x10)]
[PacketId(764, 764, 0x12)]
[PacketId(765, 765, 0x13)]
[PacketId(766, 767, 0x16)]
[PacketId(768, 770, 0x18)]
[PacketId(771, MinecraftVersion.LatestProtocol, 0x19)]
public sealed partial class UseEntityPacket : IClientPacket
{
    public int Target { get; set; }
    public int Mouse { get; set; }
    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }
    public int Hand { get; set; }
    public bool Sneaking { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        writer.WriteVarInt(Target);
        writer.WriteVarInt(Mouse);
        writer.WriteFloat(X);
        writer.WriteFloat(Y);
        writer.WriteFloat(Z);
        writer.WriteVarInt(Hand);
        writer.WriteBoolean(Sneaking);
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        Target = reader.ReadVarInt();
        Mouse = reader.ReadVarInt();
        X = reader.ReadFloat();
        Y = reader.ReadFloat();
        Z = reader.ReadFloat();
        Hand = reader.ReadVarInt();
        Sneaking = reader.ReadBoolean();
    }
}