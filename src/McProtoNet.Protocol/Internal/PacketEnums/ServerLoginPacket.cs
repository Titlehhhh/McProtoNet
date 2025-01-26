namespace McProtoNet.Protocol;

public static class ServerLoginPacket
{
    public static PacketIdentifier Compress => new(0, nameof(Compress), PacketState.Login, PacketDirection.Clientbound);

    public static PacketIdentifier CookieRequest =>
        new(1, nameof(CookieRequest), PacketState.Login, PacketDirection.Clientbound);

    public static PacketIdentifier Disconnect =>
        new(2, nameof(Disconnect), PacketState.Login, PacketDirection.Clientbound);

    public static PacketIdentifier EncryptionBegin =>
        new(3, nameof(EncryptionBegin), PacketState.Login, PacketDirection.Clientbound);

    public static PacketIdentifier LoginPluginRequest =>
        new(4, nameof(LoginPluginRequest), PacketState.Login, PacketDirection.Clientbound);

    public static PacketIdentifier Success => new(5, nameof(Success), PacketState.Login, PacketDirection.Clientbound);
}