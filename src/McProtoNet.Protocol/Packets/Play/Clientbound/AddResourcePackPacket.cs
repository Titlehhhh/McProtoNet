using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;
using McProtoNet.NBT;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("AddResourcePack", PacketState.Play, PacketDirection.Clientbound)]
[ProtocolSupport(765, MinecraftVersion.LatestProtocol)]
[PacketId(765, 765, 0x44)]
[PacketId(766, 767, 0x46)]
[PacketId(768, 769, 0x4B)]
[PacketId(770, MinecraftVersion.LatestProtocol, 0x4A)]
public sealed partial class AddResourcePackPacket : IServerPacket
{
    public Guid Uuid { get; set; }
    public string Url { get; set; }
    public string Hash { get; set; }
    public bool Forced { get; set; }
    public NbtTag? PromptMessage { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        writer.WriteUUID(Uuid);
        writer.WriteString(Url);
        writer.WriteString(Hash);
        writer.WriteBoolean(Forced);
        writer.WriteAnonOptionalNbtTag(PromptMessage, protocolVersion);
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        Uuid = reader.ReadUUID();
        Url = reader.ReadString();
        Hash = reader.ReadString();
        Forced = reader.ReadBoolean();
        PromptMessage = reader.ReadAnonOptionalNbtTag(protocolVersion);
    }
}