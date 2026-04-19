using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("EntityMetadata", PacketState.Play, PacketDirection.Clientbound)]
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[PacketId(MinecraftVersion.StartProtocol, 736, 0x44)]
[PacketId(751, 754, 0x44)]
[PacketId(755, 759, 0x4D)]
[PacketId(760, 760, 0x50)]
[PacketId(761, 761, 0x4E)]
[PacketId(762, 763, 0x52)]
[PacketId(764, 764, 0x54)]
[PacketId(765, 765, 0x56)]
[PacketId(766, 767, 0x58)]
[PacketId(768, 769, 0x5D)]
[PacketId(770, MinecraftVersion.LatestProtocol, 0x5C)]
public sealed partial class EntityMetadataPacket : IServerPacket
{
    public int EntityId { get; set; }
    public List<EntityMetadataEntry> Metadata { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        writer.WriteVarInt(EntityId);
        foreach (var entry in Metadata)
        {
            writer.WriteEntityMetadataEntry(entry, protocolVersion);
        }
        writer.WriteEntityMetadataEntry(new EntityMetadataEntry { Type = 255 }, protocolVersion);
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        EntityId = reader.ReadVarInt();
        var metadata = new List<EntityMetadataEntry>();
        while (true)
        {
            var entry = reader.ReadEntityMetadataEntry(protocolVersion);
            if (entry.Type == 255)
                break;
            metadata.Add(entry);
        }
        Metadata = metadata;
    }
}