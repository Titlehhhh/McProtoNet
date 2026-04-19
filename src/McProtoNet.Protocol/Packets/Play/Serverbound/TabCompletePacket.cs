using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;

[PacketInfo("TabComplete", PacketState.Play, PacketDirection.Serverbound)]
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[PacketId(MinecraftVersion.StartProtocol, 736, 0x06)]
[PacketId(751, 758, 0x06)]
[PacketId(759, 759, 0x08)]
[PacketId(760, 760, 0x09)]
[PacketId(761, 761, 0x08)]
[PacketId(762, 763, 0x09)]
[PacketId(764, 765, 0x0A)]
[PacketId(766, 767, 0x0B)]
[PacketId(768, 770, 0x0D)]
[PacketId(771, MinecraftVersion.LatestProtocol, 0x0E)]
public sealed partial class TabCompletePacket : IClientPacket
{
    public int TransactionId { get; set; }
    public string Text { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
        => writer.WriteVarInt(TransactionId);
           writer.WriteString(Text);

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => TransactionId = reader.ReadVarInt();
           Text = reader.ReadString();
}