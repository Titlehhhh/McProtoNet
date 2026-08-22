using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol;
[ProtocolSupport(766, MinecraftVersion.LatestProtocol)]
public readonly partial record struct ChatTypeParameterType(int Value) : IProtocolType<ChatTypeParameterType>
{
    public static readonly ChatTypeParameterType Content = new(0);
    public static readonly ChatTypeParameterType Sender = new(1);
    public static readonly ChatTypeParameterType Target = new(2);
    public static explicit operator int (ChatTypeParameterType value) => value.Value;
    public static explicit operator ChatTypeParameterType(int value) => new(value);
    public static ChatTypeParameterType Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ChatTypeParameterType>(protocolVersion);
        return new ChatTypeParameterType((int)reader.ReadVarInt());
    }

    public readonly void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ChatTypeParameterType>(protocolVersion);
        writer.WriteVarInt((int)Value);
    }

    public override string ToString() => Value switch
    {
        0 => "content",
        1 => "sender",
        2 => "target",
        _ => $"unknown({Value})"};
    public string ToString(int protocolVersion)
    {
        if (protocolVersion >= 766)
        {
            return Value switch
            {
                0 => "content",
                1 => "sender",
                2 => "target",
                _ => $"unknown({Value})"};
        }

        return $"unknown({Value})";
    }
}
