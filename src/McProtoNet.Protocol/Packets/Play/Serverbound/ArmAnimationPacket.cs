using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;

[PacketInfo("ArmAnimation", PacketState.Play, PacketDirection.Serverbound)]
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[PacketId(MinecraftVersion.StartProtocol, 736, 0x2B)]
[PacketId(751, 758, 0x2C)]
[PacketId(759, 759, 0x2E)]
[PacketId(760, 763, 0x2F)]
[PacketId(764, 764, 0x32)]
[PacketId(765, 765, 0x33)]
[PacketId(766, 767, 0x36)]
[PacketId(768, 768, 0x38)]
[PacketId(769, 769, 0x3A)]
[PacketId(770, 770, 0x3B)]
[PacketId(771, MinecraftVersion.LatestProtocol, 0x3C)]
public sealed partial class ArmAnimationPacket : IClientPacket
{
    public int Name { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
        => writer.WriteVarInt(Name);

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Name = reader.ReadVarInt();
}