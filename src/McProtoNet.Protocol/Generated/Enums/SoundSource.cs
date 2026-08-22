#pragma warning disable CA2225

using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol;
[ProtocolSupport(761, MinecraftVersion.LatestProtocol)]
public readonly partial record struct SoundSource(int Value) : IProtocolType<SoundSource>
{
    public static readonly SoundSource Master = new(0);
    public static readonly SoundSource Music = new(1);
    public static readonly SoundSource Record = new(2);
    public static readonly SoundSource Weather = new(3);
    public static readonly SoundSource Block = new(4);
    public static readonly SoundSource Hostile = new(5);
    public static readonly SoundSource Neutral = new(6);
    public static readonly SoundSource Player = new(7);
    public static readonly SoundSource Ambient = new(8);
    public static readonly SoundSource Voice = new(9);
    public static readonly SoundSource Ui = new(10);
    public static explicit operator int (SoundSource value) => value.Value;
    public static explicit operator SoundSource(int value) => new(value);
    public static SoundSource Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<SoundSource>(protocolVersion);
        if (protocolVersion >= 761 && protocolVersion <= 770)
        {
            return new SoundSource((int)reader.ReadVarInt());
        }

        if (protocolVersion >= 771)
        {
            return new SoundSource((int)reader.ReadVarInt());
        }

        throw new System.NotSupportedException($"SoundSource has no wire layout for protocol version {protocolVersion}.");
    }

    public readonly void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<SoundSource>(protocolVersion);
        if (protocolVersion >= 761 && protocolVersion <= 770)
        {
            writer.WriteVarInt((int)Value);
            return;
        }

        if (protocolVersion >= 771)
        {
            writer.WriteVarInt((int)Value);
            return;
        }

        throw new System.NotSupportedException($"SoundSource has no wire layout for protocol version {protocolVersion}.");
    }

    public override string ToString() => Value switch
    {
        0 => "master",
        1 => "music",
        2 => "record",
        3 => "weather",
        4 => "block",
        5 => "hostile",
        6 => "neutral",
        7 => "player",
        8 => "ambient",
        9 => "voice",
        10 => "ui",
        _ => $"unknown({Value})"};
    public string ToString(int protocolVersion)
    {
        if (protocolVersion >= 761 && protocolVersion <= 770)
        {
            return Value switch
            {
                0 => "master",
                1 => "music",
                2 => "record",
                3 => "weather",
                4 => "block",
                5 => "hostile",
                6 => "neutral",
                7 => "player",
                8 => "ambient",
                9 => "voice",
                _ => $"unknown({Value})"};
        }

        if (protocolVersion >= 771)
        {
            return Value switch
            {
                0 => "master",
                1 => "music",
                2 => "record",
                3 => "weather",
                4 => "block",
                5 => "hostile",
                6 => "neutral",
                7 => "player",
                8 => "ambient",
                9 => "voice",
                10 => "ui",
                _ => $"unknown({Value})"};
        }

        return $"unknown({Value})";
    }
}
