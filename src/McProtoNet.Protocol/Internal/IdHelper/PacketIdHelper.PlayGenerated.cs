using System.Collections.Frozen;

namespace McProtoNet.Protocol;

public static partial class PacketIdHelper
{
    private static FrozenDictionary<long, int> ClientboundPlayPackets = new Dictionary<long, int>
    {
    }.ToFrozenDictionary();

    private static FrozenDictionary<long, int> ServerboundPlayPackets = new Dictionary<long, int>
    {
    }.ToFrozenDictionary();
}