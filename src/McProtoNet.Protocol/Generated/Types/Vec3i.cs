using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol;
[ProtocolSupport(770, MinecraftVersion.LatestProtocol)]
public readonly partial record struct Vec3i(int X, int Y, int Z) : IProtocolType<Vec3i>
{
    public static Vec3i Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<Vec3i>(protocolVersion);
        var x = reader.ReadVarInt();
        var y = reader.ReadVarInt();
        var z = reader.ReadVarInt();
        return new Vec3i(x, y, z);
    }

    public readonly void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<Vec3i>(protocolVersion);
        writer.WriteVarInt(X);
        writer.WriteVarInt(Y);
        writer.WriteVarInt(Z);
    }
}
