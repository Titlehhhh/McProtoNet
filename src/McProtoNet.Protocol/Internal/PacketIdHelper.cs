﻿using System.Collections.Frozen;
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryGetPacketIdentifier(int packetId, int protocolVersion, PacketState state,
        PacketDirection direction, out PacketIdentifier identifier)
    {
        var dict = direction switch
        {
            PacketDirection.Clientbound when state == PacketState.Status => invertedClientboundStatusPackets,
            PacketDirection.Clientbound when state == PacketState.Login => invertedClientboundLoginPackets,
            PacketDirection.Clientbound when state == PacketState.Play => invertedClientboundPlayPackets,
            PacketDirection.Clientbound when state == PacketState.Handshaking =>
                invertedClientboundHandshakingPackets,
            PacketDirection.Clientbound when state == PacketState.Configuration =>
                invertedClientboundConfigurationPackets,

            PacketDirection.Serverbound when state == PacketState.Status => invertedServerboundStatusPackets,
            PacketDirection.Serverbound when state == PacketState.Login => invertedServerboundLoginPackets,
            PacketDirection.Serverbound when state == PacketState.Play => invertedServerboundPlayPackets,
            PacketDirection.Serverbound when state == PacketState.Handshaking =>
                invertedServerboundHandshakingPackets,
            PacketDirection.Serverbound when state == PacketState.Configuration =>
                invertedServerboundConfigurationPackets,
            _ => throw new InvalidOperationException("Unknown packet direction."),
        };
        long key = Combine(packetId, protocolVersion);
        if (dict.TryGetValue(key, out var finded))
        {
            identifier = finded;
            return true;
        }

        identifier = PacketIdentifier.Undefined;
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
        long key = Combine(identifier, protocolVersion);
        var dict = identifier.Direction switch
        {
            PacketDirection.Clientbound when identifier.State == PacketState.Status => ClientboundStatusPackets,
            PacketDirection.Clientbound when identifier.State == PacketState.Login => ClientboundLoginPackets,
            PacketDirection.Clientbound when identifier.State == PacketState.Play => ClientboundPlayPackets,
            PacketDirection.Clientbound when identifier.State == PacketState.Handshaking =>
                ClientboundHandshakingPackets,
            PacketDirection.Clientbound when identifier.State == PacketState.Configuration =>
                ClientboundConfigurationPackets,
            PacketDirection.Serverbound when identifier.State == PacketState.Status => ServerboundStatusPackets,
            PacketDirection.Serverbound when identifier.State == PacketState.Login => ServerboundLoginPackets,

            PacketDirection.Serverbound when identifier.State == PacketState.Play => ServerboundPlayPackets,
            PacketDirection.Serverbound when identifier.State == PacketState.Handshaking =>
                ServerboundHandshakingPackets,
            PacketDirection.Serverbound when identifier.State == PacketState.Configuration =>
                ServerboundConfigurationPackets,
            _ => throw new InvalidOperationException("Unknown packet identifier."),
        };

        if (dict.TryGetValue(key, out var finded))
        {
            packetId = finded;
            return true;
        }

        packetId = -1;
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