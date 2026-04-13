using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Configuration.Clientbound;

[PacketInfo("ResourcePackSend", PacketState.Configuration, PacketDirection.Clientbound)]
[ProtocolSupport(764, 764)]
[PacketId(764, 764, 0x06)]
public sealed partial class ResourcePackSendPacket : IServerPacket
{
    public string Url { get; set; }
    public string Hash { get; set; }
    public bool Forced { get; set; }
    public string PromptMessage { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        writer.WriteString(Url);
        writer.WriteString(Hash);
        writer.WriteBoolean(Forced);
        writer.WriteString(PromptMessage);
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        Url = reader.ReadString();
        Hash = reader.ReadString();
        Forced = reader.ReadBoolean();
        PromptMessage = reader.ReadString();
    }

    void IPacket.Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);
}