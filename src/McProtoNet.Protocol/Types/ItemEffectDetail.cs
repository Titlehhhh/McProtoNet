using McProtoNet.Protocol.Attributes;

namespace McProtoNet.Protocol;

[ProtocolSupport(766, MinecraftVersion.LatestProtocol)]
public sealed partial class ItemEffectDetail
{
    public int Amplifier { get; }
    public int Duration { get; }
    public bool Ambient { get; }
    public bool ShowParticles { get; }
    public bool ShowIcon { get; }
    public ItemEffectDetail? HiddenEffect { get; }

    public ItemEffectDetail(int amplifier, int duration, bool ambient, bool showParticles, bool showIcon,
        ItemEffectDetail? hiddenEffect)
    {
        Amplifier = amplifier;
        Duration = duration;
        Ambient = ambient;
        ShowParticles = showParticles;
        ShowIcon = showIcon;
        HiddenEffect = hiddenEffect;
    }
}
