using System;
using McProtoNet.Serialization;
using McProtoNet.Protocol;

namespace McProtoNet.Protocol.Packets.Configuration.Clientbound;

[PacketInfo("Tags", PacketState.Configuration, PacketDirection.Clientbound)]
public sealed partial class TagsPacket : IServerPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(764, MinecraftVersion.LatestProtocol)
    };
    public TagTypeEntry[] Tags { get; set; } = Array.Empty<TagTypeEntry>();

    internal void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= 764 and <= MinecraftVersion.LatestProtocol:
                writer.WriteVarInt(Tags.Length);
                for (int i = 0; i < Tags.Length; i++)
                {
                    TagTypeEntry entry = Tags[i];
                    writer.WriteString(entry.TagType);
                    writer.WriteTags(entry.Tags, protocolVersion);
                }
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerConfigurationPacket.Tags), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= 764 and <= MinecraftVersion.LatestProtocol:
                Tags = reader.ReadArray(LengthFormat.VarInt, (ref MinecraftPrimitiveReader r) =>
                {
                    string tagType = r.ReadString();
                    Tags tags = r.ReadTags(protocolVersion);
                    return new TagTypeEntry(tagType, tags);
                });
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerConfigurationPacket.Tags), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    void IPacket.Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(ref writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);

    public sealed class TagTypeEntry
    {
        public string TagType { get; set; }
        public Tags Tags { get; set; }

        public TagTypeEntry(string tagType, Tags tags)
        {
            TagType = tagType;
            Tags = tags;
        }
    }
}