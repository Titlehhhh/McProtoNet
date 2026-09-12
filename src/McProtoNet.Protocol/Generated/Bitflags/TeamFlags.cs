using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
using System.Text.Json;

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

    public readonly void WriteJson(Utf8JsonWriter writer)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("FriendlyFire");
        writer.WriteBooleanValue(FriendlyFire);
        writer.WritePropertyName("SeeFriendlyInvisible");
        writer.WriteBooleanValue(SeeFriendlyInvisible);
        writer.WriteEndObject();
    }
}
