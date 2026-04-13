using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;
using McProtoNet.NBT;

namespace McProtoNet.Protocol.Packets.Configuration.Clientbound;

[PacketInfo("AddResourcePack", PacketState.Configuration, PacketDirection.Clientbound)]
[ProtocolSupport(765, MinecraftVersion.LatestProtocol)]
[PacketId(765, 765, 0x07)]
[PacketId(766, MinecraftVersion.LatestProtocol, 0x09)]
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

    void IPacket.Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);
}