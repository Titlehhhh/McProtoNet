using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol;
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
public readonly partial record struct Rotations(float Pitch, float Yaw, float Roll) : IProtocolType<Rotations>
{
    public static Rotations Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<Rotations>(protocolVersion);
        var pitch = reader.ReadFloat();
        var yaw = reader.ReadFloat();
        var roll = reader.ReadFloat();
        return new Rotations(pitch, yaw, roll);
    }

    public readonly void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<Rotations>(protocolVersion);
        writer.WriteFloat(Pitch);
        writer.WriteFloat(Yaw);
        writer.WriteFloat(Roll);
    }
}
