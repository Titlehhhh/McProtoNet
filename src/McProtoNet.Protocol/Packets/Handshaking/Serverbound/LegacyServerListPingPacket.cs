using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Handshaking.Serverbound;

[PacketInfo("LegacyServerListPing", PacketState.Handshaking, PacketDirection.Serverbound)]
public sealed class LegacyServerListPingPacket : IClientPacket
{
    public byte Payload { get; set; }

    internal void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= MinecraftVersion.LatestProtocol:
                writer.WriteUnsignedByte(Payload);
                return;
            default:
                throw new ProtocolNotSupportException(nameof(ClientHandshakingPacket.LegacyServerListPing), protocolVersion);
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= MinecraftVersion.LatestProtocol:
                Payload = reader.ReadUnsignedByte();
                return;
            default:
                throw new ProtocolNotSupportException(nameof(ClientHandshakingPacket.LegacyServerListPing), protocolVersion);
        }
    }

    void IPacket.Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(ref writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);
}
