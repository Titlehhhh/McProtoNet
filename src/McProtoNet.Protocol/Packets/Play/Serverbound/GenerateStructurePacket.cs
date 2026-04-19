using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;

[PacketInfo("GenerateStructure", PacketState.Play, PacketDirection.Serverbound)]
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[PacketId(MinecraftVersion.StartProtocol, 736, 0x0F)]
[PacketId(751, 754, 0x0F)]
[PacketId(755, 758, 0x0E)]
[PacketId(759, 759, 0x10)]
[PacketId(760, 760, 0x11)]
[PacketId(761, 761, 0x10)]
[PacketId(762, 763, 0x11)]
[PacketId(764, 764, 0x13)]
[PacketId(765, 765, 0x14)]
[PacketId(766, 767, 0x17)]
[PacketId(768, 770, 0x19)]
[PacketId(771, MinecraftVersion.LatestProtocol, 0x1A)]
public sealed partial class GenerateStructurePacket : IClientPacket
{
    public Position Name { get; set; } = default!;
    public int Levels { get; set; }
    public bool KeepJigsaws { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
        => writer.WriteType(Name, protocolVersion);
    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Name = reader.ReadType<Position>(protocolVersion);
}