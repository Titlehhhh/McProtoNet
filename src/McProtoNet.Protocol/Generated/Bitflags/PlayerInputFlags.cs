using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol;
[ProtocolSupport(768, MinecraftVersion.LatestProtocol)]
public readonly partial record struct PlayerInputFlags(bool Forward, bool Backward, bool Left, bool Right, bool Jump, bool Shift, bool Sprint) : IProtocolType<PlayerInputFlags>
{
    public static PlayerInputFlags Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<PlayerInputFlags>(protocolVersion);
        byte flags = reader.ReadUnsignedByte();
        return new PlayerInputFlags((flags & (1 << 0)) != 0, (flags & (1 << 1)) != 0, (flags & (1 << 2)) != 0, (flags & (1 << 3)) != 0, (flags & (1 << 4)) != 0, (flags & (1 << 5)) != 0, (flags & (1 << 6)) != 0);
    }

    public readonly void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<PlayerInputFlags>(protocolVersion);
        byte flags = 0;
        if (Forward)
            flags |= (1 << 0);
        if (Backward)
            flags |= (1 << 1);
        if (Left)
            flags |= (1 << 2);
        if (Right)
            flags |= (1 << 3);
        if (Jump)
            flags |= (1 << 4);
        if (Shift)
            flags |= (1 << 5);
        if (Sprint)
            flags |= (1 << 6);
        writer.WriteUnsignedByte(flags);
    }
}
