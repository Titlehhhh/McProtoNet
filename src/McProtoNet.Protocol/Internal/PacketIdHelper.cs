using System.Collections.Frozen;
using System.Runtime.CompilerServices;
using System.Security.Authentication;

namespace McProtoNet.Protocol;

public static partial class PacketIdHelper
{
    private static readonly FrozenDictionary<long, PacketIdentifier> invertedClientboundStatusPackets;
    private static readonly FrozenDictionary<long, PacketIdentifier> invertedClientboundLoginPackets;
    private static readonly FrozenDictionary<long, PacketIdentifier> invertedClientboundHandshakingPackets;
    private static readonly FrozenDictionary<long, PacketIdentifier> invertedClientboundConfigurationPackets;
    private static readonly FrozenDictionary<long, PacketIdentifier> invertedClientboundPlayPackets;

    private static readonly FrozenDictionary<long, PacketIdentifier> invertedServerboundStatusPackets;
    private static readonly FrozenDictionary<long, PacketIdentifier> invertedServerboundLoginPackets;
    private static readonly FrozenDictionary<long, PacketIdentifier> invertedServerboundHandshakingPackets;
    private static readonly FrozenDictionary<long, PacketIdentifier> invertedServerboundConfigurationPackets;
    private static readonly FrozenDictionary<long, PacketIdentifier> invertedServerboundPlayPackets;
    
    static PacketIdHelper()
    {
        
    }
    
    
    
    public static PacketIdentifier GetPacketIdentifier(int protocolVersion, int packetId, PacketState state, PacketDirection direction)
    {
        throw new NotImplementedException();
    }
    
    public static int GetPacketId(int protocolVersion, PacketIdentifier packetIdentifier)
    {
        long key = Combine(packetIdentifier, protocolVersion);
        
        return packetIdentifier.Direction switch
        {
            PacketDirection.Clientbound when packetIdentifier.State == PacketState.Status => ClientboundStatusPackets
                [key],
            PacketDirection.Clientbound when packetIdentifier.State == PacketState.Login => ClientboundLoginPackets
                [key],
            PacketDirection.Clientbound when packetIdentifier.State == PacketState.Play => ClientboundPlayPackets[key],
            PacketDirection.Clientbound when packetIdentifier.State == PacketState.Handshaking =>
                ClientboundHandshakingPackets[key],
            PacketDirection.Clientbound when packetIdentifier.State == PacketState.Configuration =>
                ClientboundConfigurationPackets[key],
            
            
            PacketDirection.Serverbound when packetIdentifier.State == PacketState.Status => ServerboundStatusPackets
                [key],
            PacketDirection.Serverbound when packetIdentifier.State == PacketState.Login => ServerboundLoginPackets
                [key],
            PacketDirection.Serverbound when packetIdentifier.State == PacketState.Play => ServerboundPlayPackets[key],
            PacketDirection.Serverbound when packetIdentifier.State == PacketState.Handshaking =>
                ServerboundHandshakingPackets[key],
            PacketDirection.Serverbound when packetIdentifier.State == PacketState.Configuration =>
                ServerboundConfigurationPackets[key],
            _ => throw new InvalidOperationException("Unknown packet identifier."),
        };
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void LongToInts(long l, out int a, out int b)
    {
        a = (int)(l & uint.MaxValue);
        b = (int)(l >> 32);
    }

    private static int GetOrder(long key)
    {
        LongToInts(key, out var a, out var b);
        return a;
    }
    
    
}