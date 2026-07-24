using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol;
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
public readonly partial record struct PositionUpdateRelatives(bool X, bool Y, bool Z, bool Yaw, bool Pitch, bool Dx, bool Dy, bool Dz, bool YawDelta) : IProtocolType<PositionUpdateRelatives>
{
    public static PositionUpdateRelatives Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<PositionUpdateRelatives>(protocolVersion);
        if (protocolVersion <= 767)
        {
            byte flags = reader.ReadUnsignedByte();
            return new PositionUpdateRelatives((flags & (1 << 0)) != 0, (flags & (1 << 1)) != 0, (flags & (1 << 2)) != 0, (flags & (1 << 3)) != 0, (flags & (1 << 4)) != 0, false, false, false, false);
        }

        if (protocolVersion >= 768)
        {
            uint flags = reader.ReadUnsignedInt();
            return new PositionUpdateRelatives((flags & (1 << 0)) != 0, (flags & (1 << 1)) != 0, (flags & (1 << 2)) != 0, (flags & (1 << 3)) != 0, (flags & (1 << 4)) != 0, (flags & (1 << 5)) != 0, (flags & (1 << 6)) != 0, (flags & (1 << 7)) != 0, (flags & (1 << 8)) != 0);
        }

        throw new System.NotSupportedException($"PositionUpdateRelatives has no wire layout for protocol version {protocolVersion}.");
    }

    public readonly void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<PositionUpdateRelatives>(protocolVersion);
        if (protocolVersion <= 767)
        {
            byte flags = 0;
            if (X)
                flags |= (1 << 0);
            if (Y)
                flags |= (1 << 1);
            if (Z)
                flags |= (1 << 2);
            if (Yaw)
                flags |= (1 << 3);
            if (Pitch)
                flags |= (1 << 4);
            writer.WriteUnsignedByte(flags);
            return;
        }

        if (protocolVersion >= 768)
        {
            uint flags = 0;
            if (X)
                flags |= (1 << 0);
            if (Y)
                flags |= (1 << 1);
            if (Z)
                flags |= (1 << 2);
            if (Yaw)
                flags |= (1 << 3);
            if (Pitch)
                flags |= (1 << 4);
            if (Dx)
                flags |= (1 << 5);
            if (Dy)
                flags |= (1 << 6);
            if (Dz)
                flags |= (1 << 7);
            if (YawDelta)
                flags |= (1 << 8);
            writer.WriteUnsignedInt(flags);
            return;
        }

        throw new System.NotSupportedException($"PositionUpdateRelatives has no wire layout for protocol version {protocolVersion}.");
    }
}
