using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.tags", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("BlockTags", "Tag[]", Group = "VUntil754", To = 754)]
[PacketField("ItemTags", "Tag[]", Group = "VUntil754", To = 754)]
[PacketField("FluidTags", "Tag[]", Group = "VUntil754", To = 754)]
[PacketField("EntityTags", "Tag[]", Group = "VUntil754", To = 754)]
[PacketField("Tags", "TagCategory[]", Group = "V755_Last", From = 755)]
public sealed partial record TagsPacket(TagsPacket.VUntil754Layer? VUntil754 = null, TagsPacket.V755_LastLayer? V755_Last = null) : IPacket<TagsPacket>, IPacket
{
    public readonly record struct VUntil754Layer(Tag[] BlockTags, Tag[] ItemTags, Tag[] FluidTags, Tag[] EntityTags);
    public readonly record struct V755_LastLayer(TagCategory[] Tags);
    public static TagsPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<TagsPacket>(protocolVersion);
        if (protocolVersion <= 754)
        {
            int blockTagsCount = reader.ReadVarInt();
            var blockTags = new Tag[blockTagsCount];
            for (int i = 0; i < blockTags.Length; i++)
                blockTags[i] = reader.ReadType<Tag>(protocolVersion);
            int itemTagsCount = reader.ReadVarInt();
            var itemTags = new Tag[itemTagsCount];
            for (int i = 0; i < itemTags.Length; i++)
                itemTags[i] = reader.ReadType<Tag>(protocolVersion);
            int fluidTagsCount = reader.ReadVarInt();
            var fluidTags = new Tag[fluidTagsCount];
            for (int i = 0; i < fluidTags.Length; i++)
                fluidTags[i] = reader.ReadType<Tag>(protocolVersion);
            int entityTagsCount = reader.ReadVarInt();
            var entityTags = new Tag[entityTagsCount];
            for (int i = 0; i < entityTags.Length; i++)
                entityTags[i] = reader.ReadType<Tag>(protocolVersion);
            return new TagsPacket(VUntil754: new VUntil754Layer(blockTags, itemTags, fluidTags, entityTags));
        }

        if (protocolVersion >= 755)
        {
            int tagsCount = reader.ReadVarInt();
            var tags = new TagCategory[tagsCount];
            for (int i = 0; i < tags.Length; i++)
                tags[i] = reader.ReadType<TagCategory>(protocolVersion);
            return new TagsPacket(V755_Last: new V755_LastLayer(tags));
        }

        throw new System.NotSupportedException($"TagsPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<TagsPacket>(protocolVersion);
        if (protocolVersion <= 754)
        {
            var layer = VUntil754 ?? throw new WrongLayerException("TagsPacket", protocolVersion, "VUntil754");
            Tag[] BlockTags = layer.BlockTags;
            Tag[] ItemTags = layer.ItemTags;
            Tag[] FluidTags = layer.FluidTags;
            Tag[] EntityTags = layer.EntityTags;
            writer.WriteVarInt(BlockTags.Length);
            foreach (var blockTagsItem in BlockTags)
                writer.WriteType<Tag>(blockTagsItem, protocolVersion);
            writer.WriteVarInt(ItemTags.Length);
            foreach (var itemTagsItem in ItemTags)
                writer.WriteType<Tag>(itemTagsItem, protocolVersion);
            writer.WriteVarInt(FluidTags.Length);
            foreach (var fluidTagsItem in FluidTags)
                writer.WriteType<Tag>(fluidTagsItem, protocolVersion);
            writer.WriteVarInt(EntityTags.Length);
            foreach (var entityTagsItem in EntityTags)
                writer.WriteType<Tag>(entityTagsItem, protocolVersion);
            return;
        }

        if (protocolVersion >= 755)
        {
            var layer = V755_Last ?? throw new WrongLayerException("TagsPacket", protocolVersion, "V755_Last");
            TagCategory[] Tags = layer.Tags;
            writer.WriteVarInt(Tags.Length);
            foreach (var tagsItem in Tags)
                writer.WriteType<TagCategory>(tagsItem, protocolVersion);
            return;
        }

        throw new System.NotSupportedException($"TagsPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public static PacketIdentity Identity => new("play.toClient.tags", "Tags", PacketPhase.Play, PacketDirection.Clientbound, 97);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 735 && protocolVersion <= 736)
        {
            id = 0x5B;
            return true;
        }

        if (protocolVersion >= 751 && protocolVersion <= 754)
        {
            id = 0x5B;
            return true;
        }

        if (protocolVersion >= 755 && protocolVersion <= 756)
        {
            id = 0x66;
            return true;
        }

        if (protocolVersion >= 757 && protocolVersion <= 758)
        {
            id = 0x67;
            return true;
        }

        if (protocolVersion >= 759 && protocolVersion <= 759)
        {
            id = 0x68;
            return true;
        }

        if (protocolVersion >= 760 && protocolVersion <= 760)
        {
            id = 0x6B;
            return true;
        }

        if (protocolVersion >= 761 && protocolVersion <= 761)
        {
            id = 0x6A;
            return true;
        }

        if (protocolVersion >= 762 && protocolVersion <= 763)
        {
            id = 0x6E;
            return true;
        }

        if (protocolVersion >= 764 && protocolVersion <= 764)
        {
            id = 0x70;
            return true;
        }

        if (protocolVersion >= 765 && protocolVersion <= 765)
        {
            id = 0x74;
            return true;
        }

        if (protocolVersion >= 766 && protocolVersion <= 767)
        {
            id = 0x78;
            return true;
        }

        if (protocolVersion >= 768 && protocolVersion <= 772)
        {
            id = 0x7F;
            return true;
        }

        id = 0;
        return false;
    }

    public static int GetPacketId(int protocolVersion)
    {
        if (TryGetPacketId(protocolVersion, out var id))
            return id;
        throw new System.NotSupportedException($"No packet id for protocol {protocolVersion}.");
    }
}
