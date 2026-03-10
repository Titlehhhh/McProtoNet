using System.Runtime.CompilerServices;

namespace McProtoNet.Protocol;

public static partial class PacketIdHelper
{
    public static PacketIdentifier GetPacketIdentifier(int packetId, int protocolVersion, PacketState state, PacketDirection direction)
    {
        if (TryGetPacketIdentifier(packetId, protocolVersion, state, direction, out var identifier))
            return identifier;
        throw new KeyNotFoundException("Packet identifier not found.");
    }

    public static int GetPacketId(int protocolVersion, PacketIdentifier packetIdentifier)
    {
        if (TryGetPacketId(packetIdentifier, protocolVersion, out var packetId))
            return packetId;
        throw new KeyNotFoundException("Packet identifier not found.");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static long Combine(PacketIdentifier identifier, int protocol) =>
        Combine(identifier.Order, protocol);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static long Combine(int a, int b) =>
        (long)a << 32 | (uint)b;
}
