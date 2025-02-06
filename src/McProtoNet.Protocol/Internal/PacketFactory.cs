using System.Collections.Frozen;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace McProtoNet.Protocol;

public static class PacketFactory
{
    internal static void Init()
    {
    }


    static PacketFactory()
    {
        Dictionary<long, Func<IServerPacket>> login = new();
        Dictionary<long, Func<IServerPacket>> configuration = new();
        Dictionary<long, Func<IServerPacket>> play = new();

        foreach (var func in ServerPacketRegistry.Packets)
        {
            IServerPacket packet = func();
            PacketIdentifier identifier = packet.GetPacketId();


            foreach (var eversion in Enum.GetValues<MinecraftVersion>())
            {
                int version = (int)eversion;
                if (packet.IsSupportedVersion(version))
                {
                    //if(identifier is { State: PacketState.Configuration, Name: "Disconnect" })
                    //    Debugger.Break();

                    int packetId;
                    try
                    {
                        packetId = PacketIdHelper.GetPacketId(version, identifier);
                    }
                    catch (KeyNotFoundException e)
                    {
                        throw new InvalidOperationException(
                            $"Not find packet: {packet.GetType().FullName} Version: {version} State: {identifier.State} Direction: {identifier.Direction}");
                    }

                    try
                    {
                        //string str = $"Version: {version} PacketId: {packetId} Name: {packet.GetType().FullName}";
                        switch (identifier.State)
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
                    catch (ArgumentException e)
                    {
                        throw new InvalidOperationException(
                            $"Fatal set packet: {packet.GetType().FullName} Version: {version} State: {identifier.State} Direction: {identifier.Direction}");
                    }
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

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
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


    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private static long Combine(int a, int b)
    {
        return (long)a << 32 | (uint)b;
    }
}