namespace McProtoNet.Protocol;

public static class ClientStatusPacket
{
	public static readonly PacketIdentifier Ping = new (0, nameof(Ping),PacketState.Status,PacketDirection.Serverbound);
	public static readonly PacketIdentifier PingStart = new (1, nameof(PingStart),PacketState.Status,PacketDirection.Serverbound);
}
