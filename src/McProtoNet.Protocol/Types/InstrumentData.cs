using System.Collections.Generic;
using McProtoNet.NBT;
using McProtoNet.Protocol.Attributes;

namespace McProtoNet.Protocol;

[ProtocolSupport(766, MinecraftVersion.LatestProtocol)]
public sealed partial class InstrumentData
{
    public ItemSoundHolder SoundEvent { get; }
    public float UseDuration { get; }
    public float Range { get; }
    public NbtTag Description { get; }

    public InstrumentData(ItemSoundHolder soundEvent, float useDuration, float range, NbtTag description)
    {
        SoundEvent = soundEvent;
        UseDuration = useDuration;
        Range = range;
        Description = description;
    }
}
