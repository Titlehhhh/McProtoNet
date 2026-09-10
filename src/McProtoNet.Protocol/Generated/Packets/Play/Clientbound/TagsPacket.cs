using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

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

    public static PacketIdentity Identity => new("play.toClient.tags", "Tags", PacketPhase.Play, PacketDirection.Clientbound, 111);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        return PacketRegistry.TryGetId(Identity, protocolVersion, out id);
    }

    public static int GetPacketId(int protocolVersion)
    {
        return PacketRegistry.GetId(Identity, protocolVersion);
    }
}
