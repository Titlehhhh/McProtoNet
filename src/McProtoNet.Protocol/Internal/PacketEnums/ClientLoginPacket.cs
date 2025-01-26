namespace McProtoNet.Protocol;

public static class ClientLoginPacket
{
    public static PacketIdentifier CookieResponse => new (0, nameof(CookieResponse),PacketState.Login,PacketDirection.Serverbound);
    public static PacketIdentifier EncryptionBegin => new (1, nameof(EncryptionBegin),PacketState.Login,PacketDirection.Serverbound);
    public static PacketIdentifier LoginAcknowledged => new (2, nameof(LoginAcknowledged),PacketState.Login,PacketDirection.Serverbound);
    public static PacketIdentifier LoginPluginResponse => new (3, nameof(LoginPluginResponse),PacketState.Login,PacketDirection.Serverbound);
    public static PacketIdentifier LoginStart => new (4, nameof(LoginStart),PacketState.Login,PacketDirection.Serverbound);
}