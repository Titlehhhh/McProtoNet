using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol;
[ProtocolSupport(773, MinecraftVersion.LatestProtocol)]
public sealed partial class RespawnData : IProtocolType<RespawnData>
{
    public GlobalPos GlobalPos { get; }
    public float Yaw { get; }
    public float Pitch { get; }

    public RespawnData(GlobalPos globalPos, float yaw, float pitch)
    {
        GlobalPos = globalPos;
        Yaw = yaw;
        Pitch = pitch;
    }

    public static RespawnData Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<RespawnData>(protocolVersion);
        var globalPos = reader.ReadType<GlobalPos>(protocolVersion);
        var yaw = reader.ReadFloat();
        var pitch = reader.ReadFloat();
        return new RespawnData(globalPos, yaw, pitch);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<RespawnData>(protocolVersion);
        writer.WriteType<GlobalPos>(GlobalPos, protocolVersion);
        writer.WriteFloat(Yaw);
        writer.WriteFloat(Pitch);
    }
}
