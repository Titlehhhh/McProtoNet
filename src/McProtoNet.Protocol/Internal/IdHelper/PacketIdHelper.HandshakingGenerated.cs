using System.Collections.Frozen;

namespace McProtoNet.Protocol;

public static partial class PacketIdHelper
{
    private static readonly FrozenDictionary<long, int> ClientboundHandshakingPackets =
        new Dictionary<long, int>().ToFrozenDictionary();


    private static readonly Dictionary<(PacketIdentifier, int), int> serverboundHandshakingPackets =
        new()
        {
            { (ClientHandshakingPacket.SetProtocol, 340), 0x00 },
            { (ClientHandshakingPacket.LegacyServerListPing, 340), 0xfe },
            { (ClientHandshakingPacket.SetProtocol, 351), 0x00 },
            { (ClientHandshakingPacket.LegacyServerListPing, 351), 0xfe },
            { (ClientHandshakingPacket.SetProtocol, 393), 0x00 },
            { (ClientHandshakingPacket.LegacyServerListPing, 393), 0xfe },
            { (ClientHandshakingPacket.SetProtocol, 401), 0x00 },
            { (ClientHandshakingPacket.LegacyServerListPing, 401), 0xfe },
            { (ClientHandshakingPacket.SetProtocol, 402), 0x00 },
            { (ClientHandshakingPacket.LegacyServerListPing, 402), 0xfe },
            { (ClientHandshakingPacket.SetProtocol, 403), 0x00 },
            { (ClientHandshakingPacket.LegacyServerListPing, 403), 0xfe },
            { (ClientHandshakingPacket.SetProtocol, 404), 0x00 },
            { (ClientHandshakingPacket.LegacyServerListPing, 404), 0xfe },
            { (ClientHandshakingPacket.SetProtocol, 477), 0x00 },
            { (ClientHandshakingPacket.LegacyServerListPing, 477), 0xfe },
            { (ClientHandshakingPacket.SetProtocol, 480), 0x00 },
            { (ClientHandshakingPacket.LegacyServerListPing, 480), 0xfe },
            { (ClientHandshakingPacket.SetProtocol, 490), 0x00 },
            { (ClientHandshakingPacket.LegacyServerListPing, 490), 0xfe },
            { (ClientHandshakingPacket.SetProtocol, 498), 0x00 },
            { (ClientHandshakingPacket.LegacyServerListPing, 498), 0xfe },
            { (ClientHandshakingPacket.SetProtocol, 573), 0x00 },
            { (ClientHandshakingPacket.LegacyServerListPing, 573), 0xfe },
            { (ClientHandshakingPacket.SetProtocol, 575), 0x00 },
            { (ClientHandshakingPacket.LegacyServerListPing, 575), 0xfe },
            { (ClientHandshakingPacket.SetProtocol, 578), 0x00 },
            { (ClientHandshakingPacket.LegacyServerListPing, 578), 0xfe },
            { (ClientHandshakingPacket.SetProtocol, 709), 0x00 },
            { (ClientHandshakingPacket.LegacyServerListPing, 709), 0xfe },
            { (ClientHandshakingPacket.SetProtocol, 710), 0x00 },
            { (ClientHandshakingPacket.LegacyServerListPing, 710), 0xfe },
            { (ClientHandshakingPacket.SetProtocol, 734), 0x00 },
            { (ClientHandshakingPacket.LegacyServerListPing, 734), 0xfe },
            { (ClientHandshakingPacket.SetProtocol, 735), 0x00 },
            { (ClientHandshakingPacket.LegacyServerListPing, 735), 0xfe },
            { (ClientHandshakingPacket.SetProtocol, 736), 0x00 },
            { (ClientHandshakingPacket.LegacyServerListPing, 736), 0xfe },
            { (ClientHandshakingPacket.SetProtocol, 751), 0x00 },
            { (ClientHandshakingPacket.LegacyServerListPing, 751), 0xfe },
            { (ClientHandshakingPacket.SetProtocol, 753), 0x00 },
            { (ClientHandshakingPacket.LegacyServerListPing, 753), 0xfe },
            { (ClientHandshakingPacket.SetProtocol, 754), 0x00 },
            { (ClientHandshakingPacket.LegacyServerListPing, 754), 0xfe },
            { (ClientHandshakingPacket.SetProtocol, 755), 0x00 },
            { (ClientHandshakingPacket.LegacyServerListPing, 755), 0xfe },
            { (ClientHandshakingPacket.SetProtocol, 756), 0x00 },
            { (ClientHandshakingPacket.LegacyServerListPing, 756), 0xfe },
            { (ClientHandshakingPacket.SetProtocol, 757), 0x00 },
            { (ClientHandshakingPacket.LegacyServerListPing, 757), 0xfe },
            { (ClientHandshakingPacket.SetProtocol, 758), 0x00 },
            { (ClientHandshakingPacket.LegacyServerListPing, 758), 0xfe },
            { (ClientHandshakingPacket.SetProtocol, 759), 0x00 },
            { (ClientHandshakingPacket.LegacyServerListPing, 759), 0xfe },
            { (ClientHandshakingPacket.SetProtocol, 760), 0x00 },
            { (ClientHandshakingPacket.LegacyServerListPing, 760), 0xfe },
            { (ClientHandshakingPacket.SetProtocol, 761), 0x00 },
            { (ClientHandshakingPacket.LegacyServerListPing, 761), 0xfe },
            { (ClientHandshakingPacket.SetProtocol, 762), 0x00 },
            { (ClientHandshakingPacket.LegacyServerListPing, 762), 0xfe },
            { (ClientHandshakingPacket.SetProtocol, 763), 0x00 },
            { (ClientHandshakingPacket.LegacyServerListPing, 763), 0xfe },
            { (ClientHandshakingPacket.SetProtocol, 764), 0x00 },
            { (ClientHandshakingPacket.LegacyServerListPing, 764), 0xfe },
            { (ClientHandshakingPacket.SetProtocol, 765), 0x00 },
            { (ClientHandshakingPacket.LegacyServerListPing, 765), 0xfe },
            { (ClientHandshakingPacket.SetProtocol, 766), 0x00 },
            { (ClientHandshakingPacket.LegacyServerListPing, 766), 0xfe },
            { (ClientHandshakingPacket.SetProtocol, 767), 0x00 },
            { (ClientHandshakingPacket.LegacyServerListPing, 767), 0xfe },
            { (ClientHandshakingPacket.SetProtocol, 768), 0x00 },
            { (ClientHandshakingPacket.LegacyServerListPing, 768), 0xfe },
            { (ClientHandshakingPacket.SetProtocol, 769), 0x00 },
            { (ClientHandshakingPacket.LegacyServerListPing, 769), 0xfe },
        };

    private static readonly FrozenDictionary<long, int> ServerboundHandshakingPackets =
        CombineAll(serverboundHandshakingPackets);
}