namespace McProtoNet.Protocol;

public static class ClientHandshakingPacket
{
    public static PacketIdentifier LegacyServerListPing => new(0, nameof(LegacyServerListPing), PacketState.Handshaking,
        PacketDirection.Serverbound);

    public static PacketIdentifier SetProtocol =>
        new(1, nameof(SetProtocol), PacketState.Handshaking, PacketDirection.Serverbound);
}