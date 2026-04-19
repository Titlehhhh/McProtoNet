using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;

[PacketInfo("RecipeBook", PacketState.Play, PacketDirection.Serverbound)]
[ProtocolSupport(751, MinecraftVersion.LatestProtocol)]
[PacketId(751, 758, 0x1E)]
[PacketId(759, 759, 0x20)]
[PacketId(760, 763, 0x21)]
[PacketId(764, 764, 0x24)]
[PacketId(765, 765, 0x25)]
[PacketId(766, 767, 0x28)]
[PacketId(768, 768, 0x2A)]
[PacketId(769, 770, 0x2C)]
[PacketId(771, MinecraftVersion.LatestProtocol, 0x2D)]
public sealed partial class RecipeBookPacket : IClientPacket
{
    public int BookId { get; set; }
    public bool BookOpen { get; set; }
    public bool FilterActive { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
        => writer.WriteVarInt(BookId);
    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => BookId = reader.ReadVarInt();
}