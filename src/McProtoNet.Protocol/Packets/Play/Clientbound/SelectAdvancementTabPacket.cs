using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("SelectAdvancementTab", PacketState.Play, PacketDirection.Clientbound)]
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[PacketId(MinecraftVersion.StartProtocol, 736, 0x3C)]
[PacketId(751, 754, 0x3C)]
[PacketId(755, 758, 0x40)]
[PacketId(759, 759, 0x3E)]
[PacketId(760, 760, 0x41)]
[PacketId(761, 761, 0x40)]
[PacketId(762, 763, 0x44)]
[PacketId(764, 764, 0x46)]
[PacketId(765, 765, 0x48)]
[PacketId(766, 767, 0x4A)]
[PacketId(768, 769, 0x4F)]
[PacketId(770, MinecraftVersion.LatestProtocol, 0x4E)]
public sealed partial class SelectAdvancementTabPacket : IServerPacket
{
    public string? Id { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        writer.WriteBoolean(Id != null);
        if (Id != null)
            writer.WriteString(Id);
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        bool has = reader.ReadBoolean();
        Id = has ? reader.ReadString() : null;
    }
}