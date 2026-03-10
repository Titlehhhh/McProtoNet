using McProtoNet.Protocol;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("Tags", PacketState.Play, PacketDirection.Clientbound)]
public sealed partial class TagsPacket : IServerPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(MinecraftVersion.StartProtocol, 754),
        new(755, MinecraftVersion.LatestProtocol),
    };

    public VFirst_754Fields? VFirst_754 { get; set; }
    public V755_LastFields? V755_Last { get; set; }

    internal void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 754:
            {
                var fields = VFirst_754 ?? throw new InvalidOperationException("Tags VFirst_754 fields missing.");
                writer.WriteTags(fields.BlockTags, protocolVersion);
                writer.WriteTags(fields.ItemTags, protocolVersion);
                writer.WriteTags(fields.FluidTags, protocolVersion);
                writer.WriteTags(fields.EntityTags, protocolVersion);
                return;
            }
            case >= 755 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = V755_Last ?? throw new InvalidOperationException("Tags V755_Last fields missing.");
                writer.WriteVarInt(fields.Tags.Length);
                for (int i = 0; i < fields.Tags.Length; i++)
                {
                    writer.WriteString(fields.Tags[i].TagType);
                    writer.WriteTags(fields.Tags[i].Tags, protocolVersion);
                }
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.Tags), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 754:
                VFirst_754 = new VFirst_754Fields
                {
                    BlockTags = reader.ReadTags(protocolVersion),
                    ItemTags = reader.ReadTags(protocolVersion),
                    FluidTags = reader.ReadTags(protocolVersion),
                    EntityTags = reader.ReadTags(protocolVersion)
                };
                return;
            case >= 755 and <= MinecraftVersion.LatestProtocol:
            {
                int count = reader.ReadVarInt();
                var tags = new TagEntry[count];
                for (int i = 0; i < tags.Length; i++)
                {
                    tags[i] = new TagEntry
                    {
                        TagType = reader.ReadString(),
                        Tags = reader.ReadTags(protocolVersion)
                    };
                }
                V755_Last = new V755_LastFields { Tags = tags };
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.Tags), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    void IPacket.Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(ref writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);

    public struct VFirst_754Fields
    {
        public Tags BlockTags { get; set; }
        public Tags ItemTags { get; set; }
        public Tags FluidTags { get; set; }
        public Tags EntityTags { get; set; }
    }

    public struct V755_LastFields
    {
        public TagEntry[] Tags { get; set; }
    }

    public struct TagEntry
    {
        public string TagType { get; set; }
        public Tags Tags { get; set; }
    }
}
