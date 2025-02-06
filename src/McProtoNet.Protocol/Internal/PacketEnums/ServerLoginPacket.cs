namespace McProtoNet.Protocol;

public static class ServerLoginPacket
{
    public static readonly PacketIdentifier Compress = new(0, nameof(Compress), PacketState.Login,
        PacketDirection.Clientbound);

    public static readonly PacketIdentifier CookieRequest =
        new(1, nameof(CookieRequest), PacketState.Login, PacketDirection.Clientbound);

    public static readonly PacketIdentifier Disconnect = new(2, nameof(Disconnect), PacketState.Login,
        PacketDirection.Clientbound);

    public static readonly PacketIdentifier EncryptionBegin =
        new(3, nameof(EncryptionBegin), PacketState.Login, PacketDirection.Clientbound);

    public static readonly PacketIdentifier LoginPluginRequest =
        new(4, nameof(LoginPluginRequest), PacketState.Login, PacketDirection.Clientbound);

    public static readonly PacketIdentifier Success = new(5, nameof(Success), PacketState.Login,
        PacketDirection.Clientbound);
}