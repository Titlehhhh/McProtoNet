namespace McProtoNet.Protocol;

public static class ClientHandshakingPacket
{
	public static readonly PacketIdentifier LegacyServerListPing = new (0, nameof(LegacyServerListPing),PacketState.Handshaking,PacketDirection.Serverbound);
	public static readonly PacketIdentifier SetProtocol = new (1, nameof(SetProtocol),PacketState.Handshaking,PacketDirection.Serverbound);
}
