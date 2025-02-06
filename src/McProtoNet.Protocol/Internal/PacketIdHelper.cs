using System.Collections.Frozen;
using System.Runtime.CompilerServices;
using System.Security.Authentication;

namespace McProtoNet.Protocol;

public static partial class PacketIdHelper
{
    private static readonly FrozenDictionary<long, PacketIdentifier> invertedClientboundStatusPackets =
        CombineInverted(clientboundConfigurationPackets);

    private static readonly FrozenDictionary<long, PacketIdentifier> invertedClientboundLoginPackets =
        CombineInverted(clientboundLoginPackets);

    private static readonly FrozenDictionary<long, PacketIdentifier> invertedClientboundHandshakingPackets =
        new Dictionary<long, PacketIdentifier>().ToFrozenDictionary();

    private static readonly FrozenDictionary<long, PacketIdentifier> invertedClientboundConfigurationPackets =
        CombineInverted(clientboundConfigurationPackets);

    private static readonly FrozenDictionary<long, PacketIdentifier> invertedClientboundPlayPackets =
        CombineInverted(clientboundPlayPackets);


    private static readonly FrozenDictionary<long, PacketIdentifier> invertedServerboundStatusPackets =
        CombineInverted(serverboundStatusPackets);

    private static readonly FrozenDictionary<long, PacketIdentifier> invertedServerboundLoginPackets =
        CombineInverted(serverboundLoginPackets);

    private static readonly FrozenDictionary<long, PacketIdentifier> invertedServerboundHandshakingPackets =
        CombineInverted(serverboundHandshakingPackets);

    private static readonly FrozenDictionary<long, PacketIdentifier> invertedServerboundConfigurationPackets =
        CombineInverted(serverboundConfigurationPackets);

    private static readonly FrozenDictionary<long, PacketIdentifier> invertedServerboundPlayPackets =
        CombineInverted(serverboundPlayPackets);


    public static PacketIdentifier GetPacketIdentifier(int packetId, int protocolVersion, PacketState state,
        PacketDirection direction)
    {
        long key = Combine(packetId, protocolVersion);

        return direction switch
        {
            PacketDirection.Clientbound when state == PacketState.Status => invertedClientboundStatusPackets[key],
            PacketDirection.Clientbound when state == PacketState.Login => invertedClientboundLoginPackets[key],
            PacketDirection.Clientbound when state == PacketState.Play => invertedClientboundPlayPackets[key],
            PacketDirection.Clientbound when state == PacketState.Handshaking =>
                invertedClientboundHandshakingPackets[key],
            PacketDirection.Clientbound when state == PacketState.Configuration =>
                invertedClientboundConfigurationPackets[key],

            PacketDirection.Serverbound when state == PacketState.Status => invertedServerboundStatusPackets[key],
            PacketDirection.Serverbound when state == PacketState.Login => invertedServerboundLoginPackets[key],
            PacketDirection.Serverbound when state == PacketState.Play => invertedServerboundPlayPackets[key],
            PacketDirection.Serverbound when state == PacketState.Handshaking =>
                invertedServerboundHandshakingPackets[key],
            PacketDirection.Serverbound when state == PacketState.Configuration =>
                invertedServerboundConfigurationPackets[key],
            _ => throw new InvalidOperationException("Unknown packet direction."),
        };
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

    private static long Combine((PacketIdentifier, int) tuple)
    {
        return Combine(tuple.Item1, tuple.Item2);
    }


    private static FrozenDictionary<long, int> CombineAll(
        IDictionary<(PacketIdentifier, int), int> dictionary)
    {
        return dictionary.ToDictionary(kv => Combine(kv.Key), kv => kv.Value).ToFrozenDictionary();
    }

    private static FrozenDictionary<long, PacketIdentifier> CombineInverted(
        IDictionary<(PacketIdentifier, int), int> dictionary)
    {
        return dictionary.ToDictionary(static kv =>
        {
            int packetId = kv.Value;
            int protocol = kv.Key.Item2;

            return Combine(packetId, protocol);
        }, static kv =>
        {
            PacketIdentifier identifier = kv.Key.Item1;
            return identifier;
        }).ToFrozenDictionary();
    }
}