using System.Collections.Frozen;
using System.Runtime.CompilerServices;

namespace McProtoNet.Protocol;

public static class PacketFactory
{
    private const int MinVersion = 340;
    private const int MaxVersion = 769;

    static PacketFactory()
    {
        Dictionary<long, Func<IServerPacket>> login = new();
        Dictionary<long, Func<IServerPacket>> configuration = new();
        Dictionary<long, Func<IServerPacket>> play = new();

        foreach (var func in ServerPacketRegistry.Packets)
        {
            PacketIdentifier packet = func().GetPacketId();
            for (int version = MinVersion; version < MaxVersion; version++)
            {
                try
                {
                    int packetId = PacketIdHelper.GetPacketId(version, packet);
                    switch (packet.State)
                    {
                        case PacketState.Login:
                            login.Add(Combine(version, packetId), func);
                            break;
                        case PacketState.Play:
                            play.Add(Combine(version, packetId), func);
                            break;
                        case PacketState.Configuration:
                            configuration.Add(Combine(version, packetId), func);
                            break;
                    }
                }
                catch
                {
                    // ignored
                }
            }
        }

        loginPackets = login.ToFrozenDictionary();
        configurationPackets = configuration.ToFrozenDictionary();
        playPackets = play.ToFrozenDictionary();
    }

    private static readonly FrozenDictionary<long, Func<IServerPacket>> loginPackets;
    private static readonly FrozenDictionary<long, Func<IServerPacket>> configurationPackets;
    private static readonly FrozenDictionary<long, Func<IServerPacket>> playPackets;


    public static IServerPacket CreateClientboundPacket(int protocolVersion, int packetId, PacketState state)
    {
        long key = Combine(protocolVersion, packetId);

        return state switch
        {
            PacketState.Login => loginPackets[key](),
            PacketState.Play => playPackets[key](),
            PacketState.Configuration => configurationPackets[key](),
            _ => throw new NotSupportedException("Not supported state.")
        };
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static long Combine(int a, int b)
    {
        return (long)a << 32 | (uint)b;
    }
}

public interface ITest
{
    static bool IsSupportedVersionStatic(int protocolVersion) => throw new NotImplementedException();
    bool IsSupportedVersion(int protocolVersion);
}

