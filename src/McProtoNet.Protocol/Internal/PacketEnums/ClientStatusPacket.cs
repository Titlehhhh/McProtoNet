namespace McProtoNet.Protocol;

public static class ClientStatusPacket
{
    public static PacketIdentifier Ping => new(0, nameof(Ping), PacketState.Status, PacketDirection.Serverbound);

    public static PacketIdentifier PingStart =>
        new(1, nameof(PingStart), PacketState.Status, PacketDirection.Serverbound);
}