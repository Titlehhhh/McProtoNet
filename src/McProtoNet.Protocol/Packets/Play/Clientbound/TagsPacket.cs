using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;
using McProtoNet.NBT;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("Tags", PacketState.Play, PacketDirection.Clientbound)]
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[PacketId(MinecraftVersion.StartProtocol, 736, 0x5B)]
[PacketId(751, 754, 0x5B)]
[PacketId(755, 756, 0x66)]
[PacketId(757, 758, 0x67)]
[PacketId(759, 759, 0x68)]
[PacketId(760, 760, 0x6B)]
[PacketId(761, 761, 0x6A)]
[PacketId(762, 763, 0x6E)]
[PacketId(764, 764, 0x70)]
[PacketId(765, 765, 0x74)]
[PacketId(766, 767, 0x78)]
[PacketId(768, MinecraftVersion.LatestProtocol, 0x7F)]
public sealed partial class TagsPacket : IServerPacket
{
    public VFirst_754Fields? VFirst_754 { get; set; }
    public V755_LastFields? V755_Last { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 754:
            {
                var fields = VFirst_754 ?? throw new InvalidOperationException("TagsPacket 1-754 fields missing.");
                writer.WriteNbtTag(fields.BlockTags, protocolVersion);
                writer.WriteNbtTag(fields.ItemTags, protocolVersion);
                writer.WriteNbtTag(fields.FluidTags, protocolVersion);
                writer.WriteNbtTag(fields.EntityTags, protocolVersion);
                return;
            }
            case >= 755 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = V755_Last ?? throw new InvalidOperationException("TagsPacket 755-last fields missing.");
                writer.WriteVarInt(fields.Tags.Length);
                foreach (var element in fields.Tags)
                {
                    writer.WriteString(element.TagType);
                    writer.WriteNbtTag(element.Tags, protocolVersion);
                }
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(TagsPacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 754:
            {
                VFirst_754 = new VFirst_754Fields
                {
                    BlockTags = reader.ReadNbtTag(protocolVersion),
                    ItemTags = reader.ReadNbtTag(protocolVersion),
                    FluidTags = reader.ReadNbtTag(protocolVersion),
                    EntityTags = reader.ReadNbtTag(protocolVersion)
                };
                V755_Last = null;
                return;
            }
            case >= 755 and <= MinecraftVersion.LatestProtocol:
            {
                int count = reader.ReadVarInt();
                var array = new TagElement[count];
                for (int i = 0; i < count; i++)
                {
                    var tagType = reader.ReadString();
                    var tags = reader.ReadNbtTag(protocolVersion);
                    array[i] = new TagElement { TagType = tagType, Tags = tags };
                }
                V755_Last = new V755_LastFields { Tags = array };
                VFirst_754 = null;
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(TagsPacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    public struct VFirst_754Fields
    {
        public NbtTag BlockTags { get; set; }
        public NbtTag ItemTags { get; set; }
        public NbtTag FluidTags { get; set; }
        public NbtTag EntityTags { get; set; }
    }

    public struct V755_LastFields
    {
        public TagElement[] Tags { get; set; }
    }

    public struct TagElement
    {
        public string TagType { get; set; }
        public NbtTag Tags { get; set; }
    }
}