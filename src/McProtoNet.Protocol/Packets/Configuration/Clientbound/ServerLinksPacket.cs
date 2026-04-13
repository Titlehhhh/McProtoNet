using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;
using McProtoNet.NBT;

namespace McProtoNet.Protocol.Packets.Configuration.Clientbound;

[PacketInfo("ServerLinks", PacketState.Configuration, PacketDirection.Clientbound)]
[ProtocolSupport(767, MinecraftVersion.LatestProtocol)]
[PacketId(767, MinecraftVersion.LatestProtocol, 0x10)]
public sealed partial class ServerLinksPacket : IServerPacket
{
    public LinkEntry[] Links { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        writer.WriteVarInt(Links.Length);
        foreach (var entry in Links)
        {
            writer.WriteBoolean(entry.HasKnownType);
            if (entry.HasKnownType)
            {
                writer.WriteType(entry.KnownType, protocolVersion);
            }
            else
            {
                writer.WriteNbtTag(entry.UnknownType, protocolVersion);
            }
            writer.WriteString(entry.Link);
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        int count = reader.ReadVarInt();
        var array = new LinkEntry[count];
        for (int i = 0; i < count; i++)
        {
            var hasKnownType = reader.ReadBoolean();
            ServerLinkType? knownType = default;
            NbtTag? unknownType = null;
            if (hasKnownType)
            {
                knownType = reader.ReadType<ServerLinkType>(protocolVersion);
            }
            else
            {
                unknownType = reader.ReadAnonymousNbtTag(protocolVersion);
            }
            var link = reader.ReadString();
            array[i] = new LinkEntry
            {
                HasKnownType = hasKnownType,
                KnownType = knownType,
                UnknownType = unknownType,
                Link = link
            };
        }
        Links = array;
    }

    void IPacket.Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);

    public struct LinkEntry
    {
        public bool HasKnownType { get; set; }
        public ServerLinkType? KnownType { get; set; }
        public NbtTag? UnknownType { get; set; }
        public string Link { get; set; }
    }
}