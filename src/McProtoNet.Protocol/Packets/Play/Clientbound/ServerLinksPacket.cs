using McProtoNet.NBT;
using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("ServerLinks", PacketState.Play, PacketDirection.Clientbound)]
[ProtocolSupport(767, MinecraftVersion.LatestProtocol)]
[PacketId(767, 767, 0x7B)]
[PacketId(768, MinecraftVersion.LatestProtocol, 0x82)]
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
                writer.WriteType<ServerLinkType>(entry.KnownType!.Value, protocolVersion);
            else
                writer.WriteAnonymousNbtTag(entry.UnknownType!, protocolVersion);
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
            ServerLinkType? knownType = null;
            NbtTag? unknownType = null;
            if (hasKnownType)
                knownType = reader.ReadType<ServerLinkType>(protocolVersion);
            else
                unknownType = reader.ReadAnonymousNbtTag(protocolVersion);
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

    public struct LinkEntry
    {
        public bool HasKnownType { get; set; }
        public ServerLinkType? KnownType { get; set; }
        public NbtTag? UnknownType { get; set; }
        public string Link { get; set; }
    }
}