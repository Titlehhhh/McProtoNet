namespace McProtoNet.Protocol;

public static class ServerConfigurationPacket
{
	public static readonly PacketIdentifier AddResourcePack = new (0, nameof(AddResourcePack),PacketState.Configuration,PacketDirection.Clientbound);
	public static readonly PacketIdentifier CookieRequest = new (1, nameof(CookieRequest),PacketState.Configuration,PacketDirection.Clientbound);
	public static readonly PacketIdentifier CustomPayload = new (2, nameof(CustomPayload),PacketState.Configuration,PacketDirection.Clientbound);
	public static readonly PacketIdentifier CustomReportDetails = new (3, nameof(CustomReportDetails),PacketState.Configuration,PacketDirection.Clientbound);
	public static readonly PacketIdentifier Disconnect = new (4, nameof(Disconnect),PacketState.Configuration,PacketDirection.Clientbound);
	public static readonly PacketIdentifier FeatureFlags = new (5, nameof(FeatureFlags),PacketState.Configuration,PacketDirection.Clientbound);
	public static readonly PacketIdentifier FinishConfiguration = new (6, nameof(FinishConfiguration),PacketState.Configuration,PacketDirection.Clientbound);
	public static readonly PacketIdentifier KeepAlive = new (7, nameof(KeepAlive),PacketState.Configuration,PacketDirection.Clientbound);
	public static readonly PacketIdentifier Ping = new (8, nameof(Ping),PacketState.Configuration,PacketDirection.Clientbound);
	public static readonly PacketIdentifier RegistryData = new (9, nameof(RegistryData),PacketState.Configuration,PacketDirection.Clientbound);
	public static readonly PacketIdentifier RemoveResourcePack = new (10, nameof(RemoveResourcePack),PacketState.Configuration,PacketDirection.Clientbound);
	public static readonly PacketIdentifier ResetChat = new (11, nameof(ResetChat),PacketState.Configuration,PacketDirection.Clientbound);
	public static readonly PacketIdentifier ResourcePackSend = new (12, nameof(ResourcePackSend),PacketState.Configuration,PacketDirection.Clientbound);
	public static readonly PacketIdentifier SelectKnownPacks = new (13, nameof(SelectKnownPacks),PacketState.Configuration,PacketDirection.Clientbound);
	public static readonly PacketIdentifier ServerLinks = new (14, nameof(ServerLinks),PacketState.Configuration,PacketDirection.Clientbound);
	public static readonly PacketIdentifier StoreCookie = new (15, nameof(StoreCookie),PacketState.Configuration,PacketDirection.Clientbound);
	public static readonly PacketIdentifier Tags = new (16, nameof(Tags),PacketState.Configuration,PacketDirection.Clientbound);
	public static readonly PacketIdentifier Transfer = new (17, nameof(Transfer),PacketState.Configuration,PacketDirection.Clientbound);
}
