using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol;

[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
public readonly partial record struct Position(int X, int Y, int Z) : IProtocolType<Position>
{
    public static Position Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<Position>(protocolVersion);
        var encoded = reader.ReadSignedLong();
        var x = (int)(encoded >> 38);
        var y = (int)(encoded << 52 >> 52);
        var z = (int)(encoded << 26 >> 38);
        return new Position(x, y, z);
    }

    public readonly void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<Position>(protocolVersion);
        var encoded = ((long)(X & 0x3FFFFFF) << 38) |
                      ((long)(Z & 0x3FFFFFF) << 12) |
                      (long)(Y & 0xFFF);
        writer.WriteSignedLong(encoded);
    }
}
