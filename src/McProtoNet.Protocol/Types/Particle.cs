using Dunet;
using McProtoNet.Protocol.Attributes;

namespace McProtoNet.Protocol;

[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
public sealed partial class Particle
{
    public int? ParticleId { get; }
    public ParticleData? LegacyData { get; }
    public string? Type { get; }
    public ParticlePayload? Data { get; }

    public Particle(int? particleId, ParticleData? legacyData, string? type, ParticlePayload? data)
    {
        ParticleId = particleId;
        LegacyData = legacyData;
        Type = type;
        Data = data;
    }
}

[Union]
[ProtocolSupport(766, MinecraftVersion.LatestProtocol)]
public partial record ParticlePayload
{
    public sealed partial record Block(int BlockState) : ParticlePayload;
    public sealed partial record BlockMarker(int BlockState) : ParticlePayload;
    public sealed partial record FallingDust(int BlockState) : ParticlePayload;
    public sealed partial record DustPillar(int BlockState) : ParticlePayload;
    public sealed partial record BlockCrumble(int BlockState) : ParticlePayload;
    public sealed partial record Dust(float Red, float Green, float Blue, float Scale) : ParticlePayload;
    public sealed partial record DustColorTransition(float FromRed, float FromGreen, float FromBlue, float Scale,
        float ToRed, float ToGreen, float ToBlue) : ParticlePayload;
    public sealed partial record EntityEffect(int Color) : ParticlePayload;
    public sealed partial record Item(Slot ItemStack) : ParticlePayload;
    public sealed partial record SculkCharge(float Value) : ParticlePayload;
    public sealed partial record Shriek(int Delay) : ParticlePayload;
    public sealed partial record Vibration(ParticleVibrationData Data) : ParticlePayload;
    public sealed partial record Trail(ParticleTrailData Data) : ParticlePayload;
    public sealed partial record TintedLeaves(int Color) : ParticlePayload;
    public sealed partial record Firefly() : ParticlePayload;

    public sealed partial record ParticleVibrationData(string PositionType, Position? BlockPosition, int? EntityId,
        float? EntityEyeHeight, int Ticks);

    public sealed partial record ParticleTrailData(Vec3f64 Target, byte Color);
}
