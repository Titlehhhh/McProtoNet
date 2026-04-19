using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("Transfer", PacketState.Play, PacketDirection.Clientbound)]
[ProtocolSupport(766, MinecraftVersion.LatestProtocol)]
[PacketId(766, 767, 0x73)]
[PacketId(768, MinecraftVersion.LatestProtocol, 0x7A)]
public sealed partial class TransferPacket : IServerPacket
{
    public string Host { get; set; }
    public int Port { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        writer.WriteString(Host);
        writer.WriteVarInt(Port);
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        Host = reader.ReadString();
        Port = reader.ReadVarInt();
    }
}