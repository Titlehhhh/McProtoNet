using Dunet;
using McProtoNet.Protocol.Attributes;

namespace McProtoNet.Protocol;

[Union]
[ProtocolSupport(MinecraftVersion.StartProtocol, 765)]
public partial record ParticleData
{
    public sealed partial record BlockState(int Value) : ParticleData;
    public sealed partial record Dust(float Red, float Green, float Blue, float Scale) : ParticleData;
    public sealed partial record DustColorTransition(float FromRed, float FromGreen, float FromBlue, float Scale,
        float ToRed, float ToGreen, float ToBlue) : ParticleData;
    public sealed partial record Item(Slot ItemStack) : ParticleData;
    public sealed partial record LegacyVibration(LegacyVibrationData Data) : ParticleData;
    public sealed partial record Vibration(VibrationData Data) : ParticleData;
    public sealed partial record Rotation(float Value) : ParticleData;
    public sealed partial record Delay(int DelayInTicksBeforeShown) : ParticleData;

    public sealed partial record LegacyVibrationData(Position Origin, string PositionType, Position? DestinationBlock,
        int? DestinationEntityId, int Ticks);

    public sealed partial record VibrationData(string PositionType, int? EntityId, int? EntityEyeHeight,
        Position? DestinationBlock, int? DestinationEntityId, int Ticks);
}
