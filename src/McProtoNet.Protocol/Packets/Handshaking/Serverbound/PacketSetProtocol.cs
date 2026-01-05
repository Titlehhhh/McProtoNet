using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Handshaking.Serverbound;

[PacketInfo("SetProtocol", PacketState.Handshaking, PacketDirection.Serverbound)]
public sealed class SetProtocolPacket : IClientPacket
{
    public int ProtocolVersion { get; set; }
    public string ServerHost { get; set; } = string.Empty;
    public ushort ServerPort { get; set; }
    public int NextState { get; set; }

    internal void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= MinecraftVersion.LatestProtocol:
                writer.WriteVarInt(ProtocolVersion);
                writer.WriteString(ServerHost);
                writer.WriteUnsignedShort(ServerPort);
                writer.WriteVarInt(NextState);
                return;
            default:
                throw new ProtocolNotSupportException(nameof(ClientHandshakingPacket.SetProtocol), protocolVersion);
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= MinecraftVersion.LatestProtocol:
                ProtocolVersion = reader.ReadVarInt();
                ServerHost = reader.ReadString();
                ServerPort = reader.ReadUnsignedShort();
                NextState = reader.ReadVarInt();
                return;
            default:
                throw new ProtocolNotSupportException(nameof(ClientHandshakingPacket.SetProtocol), protocolVersion);
        }
    }

    void IPacket.Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(ref writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);
}
