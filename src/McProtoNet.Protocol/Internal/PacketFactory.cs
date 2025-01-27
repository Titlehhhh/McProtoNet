using System.Runtime.CompilerServices;

namespace McProtoNet.Protocol;

public static partial class PacketFactory
{
    public static IServerPacket CreateClientboundPacket(int protocolVersion, int packetId)
    {
        return ClientboundPlayPackets[Combine(protocolVersion, packetId)]();
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static long Combine(int a, int b)
    {
        return (long)a << 32 | (uint)b;
    }
}