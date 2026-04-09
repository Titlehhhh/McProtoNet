using System;
using System.Collections.Generic;
using McProtoNet.NBT;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Configuration.Clientbound;

[PacketInfo("RegistryData", PacketState.Configuration, PacketDirection.Clientbound)]
public sealed partial class RegistryDataPacket : IServerPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(764, 765),
        new(766, MinecraftVersion.LatestProtocol)
    };
    public V764_765Fields? V764_765 { get; set; }
    public V766_LastFields? V766_Last { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= 764 and <= 765:
            {
                var fields = V764_765 ?? throw new InvalidOperationException("RegistryData V764_765 fields missing.");
                var codec = fields.Codec ?? throw new InvalidOperationException("RegistryData V764_765 codec missing.");
                writer.WriteAnonymousNbtTag(codec, protocolVersion);
                return;
            }
            case >= 766 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = V766_Last ?? throw new InvalidOperationException("RegistryData V766_Last fields missing.");
                writer.WriteString(fields.Id);
                writer.WriteVarInt(fields.Entries.Count);
                for (int i = 0; i < fields.Entries.Count; i++)
                {
                    RegistryEntry entry = fields.Entries[i];
                    writer.WriteString(entry.Key);
                    writer.WriteAnonOptionalNbtTag(entry.Value, protocolVersion);
                }
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerConfigurationPacket.RegistryData), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= 764 and <= 765:
                V764_765 = new V764_765Fields
                {
                    Codec = reader.ReadNbtTag(readRootTag: false)
                };
                return;
            case >= 766 and <= MinecraftVersion.LatestProtocol:
            {
                string id = reader.ReadString();
                int count = reader.ReadVarInt();
                var entries = new List<RegistryEntry>(count);

                for (int i = 0; i < count; i++)
                {
                    var entry = new RegistryEntry
                    {
                        Key = reader.ReadString(),
                        Value = reader.ReadOptionalNbtTag(readRootTag: false)
                    };
                    entries.Add(entry);
                }

                V766_Last = new V766_LastFields
                {
                    Id = id,
                    Entries = entries
                };
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerConfigurationPacket.RegistryData), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    void IPacket.Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);

    public struct V764_765Fields
    {
        public NbtTag? Codec { get; set; }
    }

    public struct V766_LastFields
    {
        public string Id { get; set; }
        public List<RegistryEntry> Entries { get; set; }
    }

    public sealed class RegistryEntry
    {
        public string Key { get; set; } = string.Empty;
        public NbtTag? Value { get; set; }
    }
}