using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
using System.Text.Json;

namespace McProtoNet.Protocol;

[ProtocolSupport(MinecraftVersion.StartProtocol, 767)]
public readonly partial record struct ExplosionBlockOffset(int X, int Y, int Z) : IProtocolType<ExplosionBlockOffset>
{
    public static ExplosionBlockOffset Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ExplosionBlockOffset>(protocolVersion);
        var x = reader.ReadSignedByte();
        var y = reader.ReadSignedByte();
        var z = reader.ReadSignedByte();
        return new ExplosionBlockOffset(x, y, z);
    }

    public readonly void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ExplosionBlockOffset>(protocolVersion);
        writer.WriteSignedByte((sbyte)X);
        writer.WriteSignedByte((sbyte)Y);
        writer.WriteSignedByte((sbyte)Z);
    }

    public readonly void WriteJson(Utf8JsonWriter writer)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("X");
        writer.WriteNumberValue(X);
        writer.WritePropertyName("Y");
        writer.WriteNumberValue(Y);
        writer.WritePropertyName("Z");
        writer.WriteNumberValue(Z);
        writer.WriteEndObject();
    }
}
