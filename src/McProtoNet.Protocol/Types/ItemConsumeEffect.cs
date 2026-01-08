using System.Collections.Generic;
using McProtoNet.NBT;
using McProtoNet.Protocol.Attributes;

namespace McProtoNet.Protocol;

[ProtocolSupport(768, MinecraftVersion.LatestProtocol)]
public sealed partial class ItemConsumeEffect
{
    public string Type { get; }
    public ItemPotionEffect[]? Effects { get; }
    public float? Probability { get; }
    public IDSet? RemovedEffects { get; }
    public float? Diameter { get; }
    public ItemSoundHolder? Sound { get; }

    public ItemConsumeEffect(string type, ItemPotionEffect[]? effects, float? probability, IDSet? removedEffects,
        float? diameter, ItemSoundHolder? sound)
    {
        Type = type;
        Effects = effects;
        Probability = probability;
        RemovedEffects = removedEffects;
        Diameter = diameter;
        Sound = sound;
    }
}
