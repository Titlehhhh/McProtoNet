using McProtoNet.Protocol.Attributes;

namespace McProtoNet.Protocol;

[ProtocolSupport(761, MinecraftVersion.LatestProtocol)]
public sealed partial class ItemSoundEvent
{
    public string SoundName { get; }
    public float? FixedRange { get; }

    public ItemSoundEvent(string soundName, float? fixedRange)
    {
        SoundName = soundName;
        FixedRange = fixedRange;
    }
}
