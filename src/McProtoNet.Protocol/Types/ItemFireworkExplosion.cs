using System.Collections.Generic;
using McProtoNet.NBT;
using McProtoNet.Protocol.Attributes;

namespace McProtoNet.Protocol;

[ProtocolSupport(766, MinecraftVersion.LatestProtocol)]
public sealed partial class ItemFireworkExplosion
{
    public string Shape { get; }
    public int[] Colors { get; }
    public int[] FadeColors { get; }
    public bool HasTrail { get; }
    public bool HasTwinkle { get; }

    public ItemFireworkExplosion(string shape, int[] colors, int[] fadeColors, bool hasTrail, bool hasTwinkle)
    {
        Shape = shape;
        Colors = colors;
        FadeColors = fadeColors;
        HasTrail = hasTrail;
        HasTwinkle = hasTwinkle;
    }
}
