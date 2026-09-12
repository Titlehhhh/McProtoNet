using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
using System.Text.Json;
using McProtoNet.NBT;

namespace McProtoNet.Protocol.Packets.Configuration.Clientbound;

[ProtocolSupport(764, MinecraftVersion.LatestProtocol)]
[Packet("configuration.toClient.registry_data", PacketPhase.Configuration, PacketDirection.Clientbound)]
[PacketField("Codec", "NbtTag", Group = "V764_765", From = 764, To = 765)]
[PacketField("Registry", "string", Group = "V766_Last", From = 766)]
[PacketField("Entries", "RegistryEntry[]", Group = "V766_Last", From = 766)]
public sealed partial record RegistryDataPacket(RegistryDataPacket.V764_765Layer? V764_765 = null, RegistryDataPacket.V766_LastLayer? V766_Last = null) : IPacket<RegistryDataPacket>, IPacket
{
    public readonly record struct V764_765Layer(NbtTag Codec);
    public readonly record struct V766_LastLayer(string Registry, RegistryEntry[] Entries);
    public static RegistryDataPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<RegistryDataPacket>(protocolVersion);
        if (protocolVersion >= 764 && protocolVersion <= 765)
        {
            var codec = reader.ReadNbtTag(false)!;
            return new RegistryDataPacket(V764_765: new V764_765Layer(codec));
        }

        if (protocolVersion >= 766)
        {
            var registry = reader.ReadString();
            int entriesCount = reader.ReadVarInt();
            var entries = new RegistryEntry[entriesCount];
            for (int i = 0; i < entries.Length; i++)
                entries[i] = reader.ReadType<RegistryEntry>(protocolVersion);
            return new RegistryDataPacket(V766_Last: new V766_LastLayer(registry, entries));
        }

        throw new System.NotSupportedException($"RegistryDataPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<RegistryDataPacket>(protocolVersion);
        if (protocolVersion >= 764 && protocolVersion <= 765)
        {
            var layer = V764_765 ?? throw new WrongLayerException("RegistryDataPacket", protocolVersion, "V764_765");
            NbtTag Codec = layer.Codec;
            writer.WriteNbt(Codec);
            return;
        }

        if (protocolVersion >= 766)
        {
            var layer = V766_Last ?? throw new WrongLayerException("RegistryDataPacket", protocolVersion, "V766_Last");
            string Registry = layer.Registry;
            RegistryEntry[] Entries = layer.Entries;
            writer.WriteString(Registry);
            writer.WriteVarInt(Entries.Length);
            foreach (var entriesItem in Entries)
                writer.WriteType<RegistryEntry>(entriesItem, protocolVersion);
            return;
        }

        throw new System.NotSupportedException($"RegistryDataPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public void WriteJson(Utf8JsonWriter writer)
    {
        writer.WriteStartObject();
        if (V764_765 is { } v764_765)
        {
            writer.WritePropertyName("Codec");
            v764_765.Codec.WriteJson(writer);
        }
        else if (V766_Last is { } v766_Last)
        {
            writer.WritePropertyName("Registry");
            writer.WriteStringValue(v766_Last.Registry);
            writer.WritePropertyName("Entries");
            writer.WriteStartArray();
            foreach (var item0 in v766_Last.Entries)
            {
                item0.WriteJson(writer);
            }

            writer.WriteEndArray();
        }

        writer.WriteEndObject();
    }

    public static PacketIdentity Identity => new("configuration.toClient.registry_data", "RegistryData", PacketPhase.Configuration, PacketDirection.Clientbound, 11);

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
