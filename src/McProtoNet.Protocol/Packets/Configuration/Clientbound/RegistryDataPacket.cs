using McProtoNet.NBT;
using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Configuration.Clientbound;

[PacketInfo("RegistryData", PacketState.Configuration, PacketDirection.Clientbound)]
[ProtocolSupport(764, MinecraftVersion.LatestProtocol)]
[PacketId(764, 765, 0x05)]
[PacketId(766, MinecraftVersion.LatestProtocol, 0x07)]
public sealed partial class RegistryDataPacket : IServerPacket
{
    public V764_765Fields? V764_765 { get; set; }
    public V766_LastFields? V766_Last { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= 764 and <= 765:
            {
                var fields = V764_765 ?? throw new InvalidOperationException("RegistryDataPacket 764-765 fields missing.");
                writer.WriteAnonymousNbtTag(fields.Codec, protocolVersion);
                return;
            }
            case >= 766 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = V766_Last ?? throw new InvalidOperationException("RegistryDataPacket 766-last fields missing.");
                writer.WriteString(fields.Id);
                writer.WriteVarInt(fields.Entries.Length);
                foreach (var entry in fields.Entries)
                {
                    writer.WriteString(entry.Key);
                    writer.WriteAnonOptionalNbtTag(entry.Value, protocolVersion);
                }
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(RegistryDataPacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= 764 and <= 765:
            {
                V764_765 = new V764_765Fields
                {
                    Codec = reader.ReadAnonymousNbtTag(protocolVersion)
                };
                return;
            }
            case >= 766 and <= MinecraftVersion.LatestProtocol:
            {
                var id = reader.ReadString();
                int count = reader.ReadVarInt();
                var entries = new Entry[count];
                for (int i = 0; i < count; i++)
                {
                    var key = reader.ReadString();
                    var value = reader.ReadAnonOptionalNbtTag(protocolVersion);
                    entries[i] = new Entry { Key = key, Value = value };
                }
                V766_Last = new V766_LastFields
                {
                    Id = id,
                    Entries = entries
                };
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(RegistryDataPacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    public struct V764_765Fields
    {
        public NbtTag Codec { get; set; }
    }

    public struct V766_LastFields
    {
        public string Id { get; set; }
        public Entry[] Entries { get; set; }
    }

    public struct Entry
    {
        public string Key { get; set; }
        public NbtTag? Value { get; set; }
    }
}