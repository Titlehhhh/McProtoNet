using Dunet;
using McProtoNet.Protocol.Attributes;

namespace McProtoNet.Protocol;

[Union]
[ProtocolSupport(MinecraftVersion.StartProtocol, 765)]
public partial record ParticleData
{
    public sealed record BlockState(int Value) : ParticleData;
    public sealed record Dust(float Red, float Green, float Blue, float Scale) : ParticleData;
    public sealed record DustColorTransition(float FromRed, float FromGreen, float FromBlue, float Scale,
        float ToRed, float ToGreen, float ToBlue) : ParticleData;
    public sealed record Item(Slot ItemStack) : ParticleData;
    public sealed record LegacyVibration(LegacyVibrationData Data) : ParticleData;
    public sealed record Vibration(VibrationData Data) : ParticleData;
    public sealed record Rotation(float Value) : ParticleData;
    public sealed record Delay(int DelayInTicksBeforeShown) : ParticleData;

    public sealed record LegacyVibrationData(Position Origin, string PositionType, Position? DestinationBlock,
        int? DestinationEntityId, int Ticks);

    public sealed record VibrationData(string PositionType, int? EntityId, int? EntityEyeHeight,
        Position? DestinationBlock, int? DestinationEntityId, int Ticks);
}
