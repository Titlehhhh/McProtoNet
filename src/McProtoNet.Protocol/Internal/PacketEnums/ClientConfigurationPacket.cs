namespace McProtoNet.Protocol;

public static class ClientConfigurationPacket
{
    public static PacketIdentifier CookieResponse => new (0, nameof(CookieResponse),PacketState.Configuration,PacketDirection.Serverbound);
    public static PacketIdentifier CustomPayload => new (1, nameof(CustomPayload),PacketState.Configuration,PacketDirection.Serverbound);
    public static PacketIdentifier CustomReportDetails => new (2, nameof(CustomReportDetails),PacketState.Configuration,PacketDirection.Serverbound);
    public static PacketIdentifier FinishConfiguration => new (3, nameof(FinishConfiguration),PacketState.Configuration,PacketDirection.Serverbound);
    public static PacketIdentifier KeepAlive => new (4, nameof(KeepAlive),PacketState.Configuration,PacketDirection.Serverbound);
    public static PacketIdentifier Pong => new (5, nameof(Pong),PacketState.Configuration,PacketDirection.Serverbound);
    public static PacketIdentifier ResourcePackReceive => new (6, nameof(ResourcePackReceive),PacketState.Configuration,PacketDirection.Serverbound);
    public static PacketIdentifier SelectKnownPacks => new (7, nameof(SelectKnownPacks),PacketState.Configuration,PacketDirection.Serverbound);
    public static PacketIdentifier ServerLinks => new (8, nameof(ServerLinks),PacketState.Configuration,PacketDirection.Serverbound);
    public static PacketIdentifier Settings => new (9, nameof(Settings),PacketState.Configuration,PacketDirection.Serverbound);
}