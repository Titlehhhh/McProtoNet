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
    public sealed partial record Block(int BlockState);

    public sealed partial record BlockMarker(int BlockState);

    public sealed partial record FallingDust(int BlockState);

    public sealed partial record DustPillar(int BlockState);

    public sealed partial record BlockCrumble(int BlockState);

    public sealed partial record Dust(float Red, float Green, float Blue, float Scale);

    public sealed partial record DustColorTransition(
        float FromRed,
        float FromGreen,
        float FromBlue,
        float Scale,
        float ToRed,
        float ToGreen,
        float ToBlue);

    public sealed partial record EntityEffect(int Color);

    public sealed partial record Item(Slot ItemStack);

    public sealed partial record SculkCharge(float Value);

    public sealed partial record Shriek(int Delay);

    public sealed partial record Vibration(ParticleVibrationData Data);

    public sealed partial record Trail(ParticleTrailData Data);

    public sealed partial record TintedLeaves(int Color);

    public sealed partial record Firefly();

    public sealed partial record ParticleVibrationData(
        string PositionType,
        Position? BlockPosition,
        int? EntityId,
        float? EntityEyeHeight,
        int Ticks);

    public sealed partial record ParticleTrailData(Vec3f64 Target, byte Color);
}