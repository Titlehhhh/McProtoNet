using McProtoNet.NBT;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Configuration.Clientbound;

public abstract class RegistryDataPacket : IServerPacket
{
    public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);

    public static PacketIdentifier PacketId => ServerConfigurationPacket.RegistryData;

    public PacketIdentifier GetPacketId() => PacketId;

    public sealed class V764_765 : RegistryDataPacket
    {
        public NbtTag Codec { get; set; }

        public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        {
            Codec = reader.ReadNbtTag(readRootTag: false);
        }

        public new static bool SupportedVersion(int protocolVersion)
        {
            return protocolVersion is >= 764 and <= 765;
        }
    }

    public sealed class V766_769 : RegistryDataPacket
    {
        public string Id { get; set; }
        public List<RegistryEntry> Entries { get; set; }

        public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        {
            Id = reader.ReadString();
            int count = reader.ReadVarInt();
            Entries = new List<RegistryEntry>(count);

            for (int i = 0; i < count; i++)
            {
                var entry = new RegistryEntry
                {
                    Key = reader.ReadString(),
                    Value = reader.ReadOptionalNbtTag(readRootTag: false)
                };
                Entries.Add(entry);
            }
        }

        public new static bool SupportedVersion(int protocolVersion)
        {
            return protocolVersion is >= 766 and <= 769;
        }

        public class RegistryEntry
        {
            public string Key { get; set; }
            public NbtTag? Value { get; set; }
        }
    }

    public static bool SupportedVersion(int protocolVersion)
    {
        return V764_765.SupportedVersion(protocolVersion) ||
               V766_769.SupportedVersion(protocolVersion);
    }
}