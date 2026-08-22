using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol;
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
public readonly partial record struct Difficulty(int Value) : IProtocolType<Difficulty>
{
    public static readonly Difficulty Peaceful = new(0);
    public static readonly Difficulty Easy = new(1);
    public static readonly Difficulty Normal = new(2);
    public static readonly Difficulty Hard = new(3);
    public static explicit operator int (Difficulty value) => value.Value;
    public static explicit operator Difficulty(int value) => new(value);
    public static Difficulty Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<Difficulty>(protocolVersion);
        if (protocolVersion <= 770)
        {
            return new Difficulty((int)reader.ReadUnsignedByte());
        }

        if (protocolVersion >= 771)
        {
            return new Difficulty((int)reader.ReadVarInt());
        }

        throw new System.NotSupportedException($"Difficulty has no wire layout for protocol version {protocolVersion}.");
    }

    public readonly void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<Difficulty>(protocolVersion);
        if (protocolVersion <= 770)
        {
            writer.WriteUnsignedByte((byte)Value);
            return;
        }

        if (protocolVersion >= 771)
        {
            writer.WriteVarInt((int)Value);
            return;
        }

        throw new System.NotSupportedException($"Difficulty has no wire layout for protocol version {protocolVersion}.");
    }

    public override string ToString() => Value switch
    {
        0 => "peaceful",
        1 => "easy",
        2 => "normal",
        3 => "hard",
        _ => $"unknown({Value})"};
    public string ToString(int protocolVersion)
    {
        if (protocolVersion <= 770)
        {
            return Value switch
            {
                0 => "peaceful",
                1 => "easy",
                2 => "normal",
                3 => "hard",
                _ => $"unknown({Value})"};
        }

        if (protocolVersion >= 771)
        {
            return Value switch
            {
                0 => "peaceful",
                1 => "easy",
                2 => "normal",
                3 => "hard",
                _ => $"unknown({Value})"};
        }

        return $"unknown({Value})";
    }
}
