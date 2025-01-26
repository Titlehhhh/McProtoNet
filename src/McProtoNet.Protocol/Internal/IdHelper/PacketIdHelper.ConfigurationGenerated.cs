using System.Collections.Frozen;

namespace McProtoNet.Protocol;

public static partial class PacketIdHelper
{
    private static FrozenDictionary<long, int> ClientboundConfigurationPackets = new Dictionary<long, int>
    {
        { Combine(ServerConfigurationPacket.Tags, 769), 0x0d },
        { Combine(ServerConfigurationPacket.SelectKnownPacks, 769), 0x0e },
        { Combine(ServerConfigurationPacket.CustomReportDetails, 769), 0x0f },
        { Combine(ServerConfigurationPacket.ServerLinks, 769), 0x10 },
    }.ToFrozenDictionary();

    private static FrozenDictionary<long, int> ServerboundConfigurationPackets = new Dictionary<long, int>
    {
        { Combine(ClientConfigurationPacket.Settings, 764), 0x00 },
        { Combine(ClientConfigurationPacket.CustomPayload, 764), 0x01 },
        { Combine(ClientConfigurationPacket.FinishConfiguration, 764), 0x02 },
        { Combine(ClientConfigurationPacket.KeepAlive, 764), 0x03 },
        { Combine(ClientConfigurationPacket.Pong, 764), 0x04 },
        { Combine(ClientConfigurationPacket.ResourcePackReceive, 764), 0x05 },
        { Combine(ClientConfigurationPacket.Settings, 765), 0x00 },
        { Combine(ClientConfigurationPacket.CustomPayload, 765), 0x01 },
        { Combine(ClientConfigurationPacket.FinishConfiguration, 765), 0x02 },
        { Combine(ClientConfigurationPacket.KeepAlive, 765), 0x03 },
        { Combine(ClientConfigurationPacket.Pong, 765), 0x04 },
        { Combine(ClientConfigurationPacket.ResourcePackReceive, 765), 0x05 },
        { Combine(ClientConfigurationPacket.Settings, 766), 0x00 },
        { Combine(ClientConfigurationPacket.CookieResponse, 766), 0x01 },
        { Combine(ClientConfigurationPacket.CustomPayload, 766), 0x02 },
        { Combine(ClientConfigurationPacket.FinishConfiguration, 766), 0x03 },
        { Combine(ClientConfigurationPacket.KeepAlive, 766), 0x04 },
        { Combine(ClientConfigurationPacket.Pong, 766), 0x05 },
        { Combine(ClientConfigurationPacket.ResourcePackReceive, 766), 0x06 },
        { Combine(ClientConfigurationPacket.SelectKnownPacks, 766), 0x07 },
        { Combine(ClientConfigurationPacket.Settings, 767), 0x00 },
        { Combine(ClientConfigurationPacket.CookieResponse, 767), 0x01 },
        { Combine(ClientConfigurationPacket.CustomPayload, 767), 0x02 },
        { Combine(ClientConfigurationPacket.FinishConfiguration, 767), 0x03 },
        { Combine(ClientConfigurationPacket.KeepAlive, 767), 0x04 },
        { Combine(ClientConfigurationPacket.Pong, 767), 0x05 },
        { Combine(ClientConfigurationPacket.ResourcePackReceive, 767), 0x06 },
        { Combine(ClientConfigurationPacket.SelectKnownPacks, 767), 0x07 },
        { Combine(ClientConfigurationPacket.CustomReportDetails, 767), 0x08 },
        { Combine(ClientConfigurationPacket.ServerLinks, 767), 0x09 },
        { Combine(ClientConfigurationPacket.Settings, 768), 0x00 },
        { Combine(ClientConfigurationPacket.CookieResponse, 768), 0x01 },
        { Combine(ClientConfigurationPacket.CustomPayload, 768), 0x02 },
        { Combine(ClientConfigurationPacket.FinishConfiguration, 768), 0x03 },
        { Combine(ClientConfigurationPacket.KeepAlive, 768), 0x04 },
        { Combine(ClientConfigurationPacket.Pong, 768), 0x05 },
        { Combine(ClientConfigurationPacket.ResourcePackReceive, 768), 0x06 },
        { Combine(ClientConfigurationPacket.SelectKnownPacks, 768), 0x07 },
        { Combine(ClientConfigurationPacket.CustomReportDetails, 768), 0x08 },
        { Combine(ClientConfigurationPacket.ServerLinks, 768), 0x09 },
        { Combine(ClientConfigurationPacket.Settings, 769), 0x00 },
        { Combine(ClientConfigurationPacket.CookieResponse, 769), 0x01 },
        { Combine(ClientConfigurationPacket.CustomPayload, 769), 0x02 },
        { Combine(ClientConfigurationPacket.FinishConfiguration, 769), 0x03 },
        { Combine(ClientConfigurationPacket.KeepAlive, 769), 0x04 },
        { Combine(ClientConfigurationPacket.Pong, 769), 0x05 },
        { Combine(ClientConfigurationPacket.ResourcePackReceive, 769), 0x06 },
        { Combine(ClientConfigurationPacket.SelectKnownPacks, 769), 0x07 },
        { Combine(ClientConfigurationPacket.CustomReportDetails, 769), 0x08 },
        { Combine(ClientConfigurationPacket.ServerLinks, 769), 0x09 },
    }.ToFrozenDictionary();
}