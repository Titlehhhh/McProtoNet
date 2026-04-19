using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;

[PacketInfo("AdvancementTab", PacketState.Play, PacketDirection.Serverbound)]
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[PacketId(MinecraftVersion.StartProtocol, 736, 0x21)]
[PacketId(751, 758, 0x22)]
[PacketId(759, 759, 0x24)]
[PacketId(760, 763, 0x25)]
[PacketId(764, 764, 0x28)]
[PacketId(765, 765, 0x29)]
[PacketId(766, 767, 0x2C)]
[PacketId(768, 768, 0x2E)]
[PacketId(769, 770, 0x30)]
[PacketId(771, MinecraftVersion.LatestProtocol, 0x31)]
public sealed partial class AdvancementTabPacket : IClientPacket
{
    public int Action { get; set; }
    public string? TabId { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        writer.WriteVarInt(Action);
        if (Action == 0)
            writer.WriteString(TabId!);
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        Action = reader.ReadVarInt();
        if (Action == 0)
            TabId = reader.ReadString();
        else
            TabId = null;
    }
}