using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol;
[ProtocolSupport(762, MinecraftVersion.LatestProtocol)]
public readonly partial record struct PackedChunkPos(int Z, int X) : IProtocolType<PackedChunkPos>
{
    public static PackedChunkPos Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<PackedChunkPos>(protocolVersion);
        var z = reader.ReadSignedInt();
        var x = reader.ReadSignedInt();
        return new PackedChunkPos(z, x);
    }

    public readonly void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<PackedChunkPos>(protocolVersion);
        writer.WriteSignedInt(Z);
        writer.WriteSignedInt(X);
    }
}
