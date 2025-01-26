namespace McProtoNet.Protocol;

public static class ServerConfigurationPacket
{
    public static PacketIdentifier AddResourcePack => new(0, nameof(AddResourcePack), PacketState.Configuration,
        PacketDirection.Clientbound);

    public static PacketIdentifier CookieRequest =>
        new(1, nameof(CookieRequest), PacketState.Configuration, PacketDirection.Clientbound);

    public static PacketIdentifier CustomPayload =>
        new(2, nameof(CustomPayload), PacketState.Configuration, PacketDirection.Clientbound);

    public static PacketIdentifier CustomReportDetails => new(3, nameof(CustomReportDetails), PacketState.Configuration,
        PacketDirection.Clientbound);

    public static PacketIdentifier Disconnect =>
        new(4, nameof(Disconnect), PacketState.Configuration, PacketDirection.Clientbound);

    public static PacketIdentifier FeatureFlags =>
        new(5, nameof(FeatureFlags), PacketState.Configuration, PacketDirection.Clientbound);

    public static PacketIdentifier FinishConfiguration => new(6, nameof(FinishConfiguration), PacketState.Configuration,
        PacketDirection.Clientbound);

    public static PacketIdentifier KeepAlive =>
        new(7, nameof(KeepAlive), PacketState.Configuration, PacketDirection.Clientbound);

    public static PacketIdentifier Ping => new(8, nameof(Ping), PacketState.Configuration, PacketDirection.Clientbound);

    public static PacketIdentifier RegistryData =>
        new(9, nameof(RegistryData), PacketState.Configuration, PacketDirection.Clientbound);

    public static PacketIdentifier RemoveResourcePack => new(10, nameof(RemoveResourcePack), PacketState.Configuration,
        PacketDirection.Clientbound);

    public static PacketIdentifier ResetChat =>
        new(11, nameof(ResetChat), PacketState.Configuration, PacketDirection.Clientbound);

    public static PacketIdentifier ResourcePackSend => new(12, nameof(ResourcePackSend), PacketState.Configuration,
        PacketDirection.Clientbound);

    public static PacketIdentifier SelectKnownPacks => new(13, nameof(SelectKnownPacks), PacketState.Configuration,
        PacketDirection.Clientbound);

    public static PacketIdentifier ServerLinks =>
        new(14, nameof(ServerLinks), PacketState.Configuration, PacketDirection.Clientbound);

    public static PacketIdentifier StoreCookie =>
        new(15, nameof(StoreCookie), PacketState.Configuration, PacketDirection.Clientbound);

    public static PacketIdentifier Tags =>
        new(16, nameof(Tags), PacketState.Configuration, PacketDirection.Clientbound);

    public static PacketIdentifier Transfer =>
        new(17, nameof(Transfer), PacketState.Configuration, PacketDirection.Clientbound);
}