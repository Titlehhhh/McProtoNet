using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Configuration.Serverbound;

[PacketInfo("SelectKnownPacks", PacketState.Configuration, PacketDirection.Serverbound)]
[ProtocolSupport(766, MinecraftVersion.LatestProtocol)]
[PacketId(766, MinecraftVersion.LatestProtocol, 0x07)]
public sealed partial class SelectKnownPacksPacket : IClientPacket
{
    public PackInfo[] Packs { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        writer.WriteVarInt(Packs.Length);
        foreach (var pack in Packs)
        {
            writer.WriteString(pack.Name);
            writer.WriteString(pack.Id);
            writer.WriteString(pack.Version);
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        int count = reader.ReadVarInt();
        var packs = new PackInfo[count];
        for (int i = 0; i < count; i++)
        {
            packs[i] = new PackInfo
            {
                Name = reader.ReadString(),
                Id = reader.ReadString(),
                Version = reader.ReadString()
            };
        }
        Packs = packs;
    }

    void IPacket.Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);

    public struct PackInfo
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public string Version { get; set; }
    }
}