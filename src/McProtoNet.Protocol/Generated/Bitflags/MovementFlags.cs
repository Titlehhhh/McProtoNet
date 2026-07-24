using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol;
[ProtocolSupport(768, MinecraftVersion.LatestProtocol)]
public readonly partial record struct MovementFlags(bool OnGround, bool HasHorizontalCollision) : IProtocolType<MovementFlags>
{
    public static MovementFlags Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<MovementFlags>(protocolVersion);
        byte flags = reader.ReadUnsignedByte();
        return new MovementFlags((flags & (1 << 0)) != 0, (flags & (1 << 1)) != 0);
    }

    public readonly void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<MovementFlags>(protocolVersion);
        byte flags = 0;
        if (OnGround)
            flags |= (1 << 0);
        if (HasHorizontalCollision)
            flags |= (1 << 1);
        writer.WriteUnsignedByte(flags);
    }
}
