using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol;
[ProtocolSupport(771, MinecraftVersion.LatestProtocol)]
public readonly partial record struct TeamFlags(bool FriendlyFire, bool SeeFriendlyInvisible) : IProtocolType<TeamFlags>
{
    public static TeamFlags Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<TeamFlags>(protocolVersion);
        byte flags = reader.ReadUnsignedByte();
        return new TeamFlags((flags & (1 << 0)) != 0, (flags & (1 << 1)) != 0);
    }

    public readonly void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<TeamFlags>(protocolVersion);
        byte flags = 0;
        if (FriendlyFire)
            flags |= (1 << 0);
        if (SeeFriendlyInvisible)
            flags |= (1 << 1);
        writer.WriteUnsignedByte(flags);
    }
}
