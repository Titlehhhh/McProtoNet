using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol;
[ProtocolSupport(768, MinecraftVersion.LatestProtocol)]
public sealed partial class MinecartStep : IProtocolType<MinecartStep>
{
    public Vec3f Position { get; }
    public Vec3f Movement { get; }
    public float Yaw { get; }
    public float Pitch { get; }
    public float Weight { get; }

    public MinecartStep(Vec3f position, Vec3f movement, float yaw, float pitch, float weight)
    {
        Position = position;
        Movement = movement;
        Yaw = yaw;
        Pitch = pitch;
        Weight = weight;
    }

    public static MinecartStep Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<MinecartStep>(protocolVersion);
        var position = reader.ReadType<Vec3f>(protocolVersion);
        var movement = reader.ReadType<Vec3f>(protocolVersion);
        var yaw = reader.ReadFloat();
        var pitch = reader.ReadFloat();
        var weight = reader.ReadFloat();
        return new MinecartStep(position, movement, yaw, pitch, weight);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<MinecartStep>(protocolVersion);
        writer.WriteType<Vec3f>(Position, protocolVersion);
        writer.WriteType<Vec3f>(Movement, protocolVersion);
        writer.WriteFloat(Yaw);
        writer.WriteFloat(Pitch);
        writer.WriteFloat(Weight);
    }
}
