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
        
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static long Combine(int a, int b)
    {
        return (long)a << 32 | (uint)b;
    }
}
