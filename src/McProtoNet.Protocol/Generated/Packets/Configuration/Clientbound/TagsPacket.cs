using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
using System.Text.Json;

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

    public void WriteJson(Utf8JsonWriter writer)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("Tags");
        writer.WriteStartArray();
        foreach (var item0 in Tags)
        {
            item0.WriteJson(writer);
        }

        writer.WriteEndArray();
        writer.WriteEndObject();
    }

    public static PacketIdentity Identity => new("configuration.toClient.tags", "Tags", PacketPhase.Configuration, PacketDirection.Clientbound, 18);

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
