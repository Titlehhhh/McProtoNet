using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
using System.Text.Json;

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

    public readonly void WriteJson(Utf8JsonWriter writer)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("OnGround");
        writer.WriteBooleanValue(OnGround);
        writer.WritePropertyName("HasHorizontalCollision");
        writer.WriteBooleanValue(HasHorizontalCollision);
        writer.WriteEndObject();
    }
}
