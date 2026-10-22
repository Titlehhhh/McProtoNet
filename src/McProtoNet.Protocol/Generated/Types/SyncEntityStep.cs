using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol;

[ProtocolSupport(777, MinecraftVersion.LatestProtocol)]
public readonly partial record struct SyncEntityStep(double X, double Y, double Z, int TickOffset) : IProtocolType<SyncEntityStep>
{
    public static SyncEntityStep Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<SyncEntityStep>(protocolVersion);
        var x = reader.ReadDouble();
        var y = reader.ReadDouble();
        var z = reader.ReadDouble();
        var tickOffset = reader.ReadVarInt();
        return new SyncEntityStep(x, y, z, tickOffset);
    }

    public readonly void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<SyncEntityStep>(protocolVersion);
        writer.WriteDouble(X);
        writer.WriteDouble(Y);
        writer.WriteDouble(Z);
        writer.WriteVarInt(TickOffset);
    }
}
