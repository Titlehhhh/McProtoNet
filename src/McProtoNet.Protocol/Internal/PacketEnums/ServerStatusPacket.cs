namespace McProtoNet.Protocol;

public static class ServerStatusPacket
{
	public static readonly PacketIdentifier Ping = new (0, nameof(Ping),PacketState.Status,PacketDirection.Clientbound);
	public static readonly PacketIdentifier ServerInfo = new (1, nameof(ServerInfo),PacketState.Status,PacketDirection.Clientbound);
}
