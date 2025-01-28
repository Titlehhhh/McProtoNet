using McProtoNet.NBT;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Configuration.Clientbound;

[PacketInfo("RegistryData", PacketState.Configuration, PacketDirection.Clientbound)]
public abstract partial class RegistryDataPacket : IServerPacket
{
    public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);

    [PacketSubInfo(764,765)]
    public sealed partial class V764_765 : RegistryDataPacket
    {
        public NbtTag Codec { get; set; }

        public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        {
            Codec = reader.ReadNbtTag(readRootTag: false);
            }

        
    }

    [PacketSubInfo(766,769)]
    public sealed partial class V766_769 : RegistryDataPacket
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


        public class RegistryEntry
        {
            public string Key { get; set; }
            public NbtTag? Value { get; set; }
        }
    }

}