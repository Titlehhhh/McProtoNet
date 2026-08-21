using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Configuration.Clientbound;
[ProtocolSupport(764, MinecraftVersion.LatestProtocol)]
[Packet("configuration.toClient.tags", PacketPhase.Configuration, PacketDirection.Clientbound)]
[PacketField("Tags", "TagCategory[]")]
public sealed partial record TagsPacket(TagCategory[] Tags) : IPacket<TagsPacket>, IPacket
{
    public static TagsPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<TagsPacket>(protocolVersion);
        int tagsCount = reader.ReadVarInt();
        var tags = new TagCategory[tagsCount];
        for (int i = 0; i < tags.Length; i++)
            tags[i] = reader.ReadType<TagCategory>(protocolVersion);
        return new TagsPacket(tags);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<TagsPacket>(protocolVersion);
        writer.WriteVarInt(Tags.Length);
        foreach (var tagsItem in Tags)
            writer.WriteType<TagCategory>(tagsItem, protocolVersion);
    }

    public static PacketIdentity Identity => new("configuration.toClient.tags", "Tags", PacketPhase.Configuration, PacketDirection.Clientbound, 17);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 764 && protocolVersion <= 764)
        {
            id = 0x08;
            return true;
        }

        if (protocolVersion >= 765 && protocolVersion <= 765)
        {
            id = 0x09;
            return true;
        }

        if (protocolVersion >= 766 && protocolVersion <= 776)
        {
            id = 0x0D;
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
