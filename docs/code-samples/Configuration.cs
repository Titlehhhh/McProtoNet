await foreach (var packet in client.OnAllPackets(PacketState.Configuration))
{
    if (packet is CConfig.FinishConfigurationPacket)
    {
        // Succes configuration
        await client.SendPacket(new SConfig.FinishConfigurationPacket());
        break;
    }
    if (packet is CConfig.KeepAlivePacket.V764_769 keepAlivePacket)
    {
        await client.SendPacket(new SConfig.KeepAlivePacket()
        {
            KeepAliveId = keepAlivePacket.KeepAliveId
        });
    }
    else if (packet is CConfig.PingPacket.V764_769 pingPacket)
    {
        await client.SendPacket(new SConfig.PongPacket()
        {
            Id = pingPacket.Id
        });
    }
    else if (packet is CConfig.CustomPayloadPacket.V764_769 payload)
    {
        await client.SendPacket(new SConfig.CustomPayloadPacket.V764_769()
        {
            Data = payload.Data,
            Channel = payload.Channel
        });
    }
    else if (packet is CConfig.SelectKnownPacksPacket)
    {
        await client.SendPacket(new SConfig.SelectKnownPacksPacket());
    }
}