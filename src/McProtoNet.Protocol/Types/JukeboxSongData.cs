using System.Collections.Generic;
using McProtoNet.NBT;
using McProtoNet.Protocol.Attributes;

namespace McProtoNet.Protocol;

[ProtocolSupport(767, MinecraftVersion.LatestProtocol)]
public sealed partial class JukeboxSongData
{
    public ItemSoundHolder SoundEvent { get; }
    public NbtTag Description { get; }
    public float LengthInSeconds { get; }
    public int ComparatorOutput { get; }

    public JukeboxSongData(ItemSoundHolder soundEvent, NbtTag description, float lengthInSeconds, int comparatorOutput)
    {
        SoundEvent = soundEvent;
        Description = description;
        LengthInSeconds = lengthInSeconds;
        ComparatorOutput = comparatorOutput;
    }
}
