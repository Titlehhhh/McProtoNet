using System.Collections.Frozen;
using System.Runtime.CompilerServices;
using System.Security.Authentication;

namespace McProtoNet.Protocol;

public static partial class PacketIdHelper
{
    

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryGetPacketIdentifier(int packetId, int protocolVersion, PacketState state,
        PacketDirection direction, out PacketIdentifier identifier)
    {
        // var dict = direction switch
        // {
        //     PacketDirection.Clientbound when state == PacketState.Status => invertedClientboundStatusPackets,
        //     PacketDirection.Clientbound when state == PacketState.Login => invertedClientboundLoginPackets,
        //     PacketDirection.Clientbound when state == PacketState.Play => invertedClientboundPlayPackets,
        //     PacketDirection.Clientbound when state == PacketState.Handshaking =>
        //         invertedClientboundHandshakingPackets,
        //     PacketDirection.Clientbound when state == PacketState.Configuration =>
        //         invertedClientboundConfigurationPackets,
        //
        //     PacketDirection.Serverbound when state == PacketState.Status => invertedServerboundStatusPackets,
        //     PacketDirection.Serverbound when state == PacketState.Login => invertedServerboundLoginPackets,
        //     PacketDirection.Serverbound when state == PacketState.Play => invertedServerboundPlayPackets,
        //     PacketDirection.Serverbound when state == PacketState.Handshaking =>
        //         invertedServerboundHandshakingPackets,
        //     PacketDirection.Serverbound when state == PacketState.Configuration =>
        //         invertedServerboundConfigurationPackets,
        //     _ => throw new InvalidOperationException("Unknown packet direction."),
        // };
        // long key = Combine(packetId, protocolVersion);
        // if (dict.TryGetValue(key, out var finded))
        // {
        //     identifier = finded;
        //     return true;
        // }
        //
        // identifier = PacketIdentifier.Undefined;
        Unsafe.SkipInit(out identifier);
        return false;
    }

    public static PacketIdentifier GetPacketIdentifier(int packetId, int protocolVersion, PacketState state,
        PacketDirection direction)
    {
        if (TryGetPacketIdentifier(packetId, protocolVersion, state, direction, out var identifier))
        {
            return identifier;
        }

        throw new KeyNotFoundException("Packet identifier not found.");
    }

    public static bool TryGetPacketId(PacketIdentifier identifier, int protocolVersion, out int packetId)
    {
        Unsafe.SkipInit(out packetId);
        return false;
    }


    public static int GetPacketId(int protocolVersion, PacketIdentifier packetIdentifier)
    {
        if (TryGetPacketId(packetIdentifier, protocolVersion, out var packetId))
        {
            return packetId;
        }

        throw new KeyNotFoundException("Packet identifier not found.");
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static long Combine(PacketIdentifier identifier, int protocol)
    {
        return Combine(identifier.Order, protocol);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static long Combine(int a, int b)
    {
        return (long)a << 32 | (uint)b;
    }

    private static long Combine((PacketIdentifier, int) tuple)
    {
        return Combine(tuple.Item1, tuple.Item2);
    }

    
}