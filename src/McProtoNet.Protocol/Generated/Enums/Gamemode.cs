using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol;
[ProtocolSupport(766, MinecraftVersion.LatestProtocol)]
public readonly partial record struct Gamemode(int Value) : IProtocolType<Gamemode>
{
    public static readonly Gamemode Survival = new(0);
    public static readonly Gamemode Creative = new(1);
    public static readonly Gamemode Adventure = new(2);
    public static readonly Gamemode Spectator = new(3);
    public static explicit operator int (Gamemode value) => value.Value;
    public static explicit operator Gamemode(int value) => new(value);
    public static Gamemode Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<Gamemode>(protocolVersion);
        return new Gamemode((int)reader.ReadSignedByte());
    }

    public readonly void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<Gamemode>(protocolVersion);
        writer.WriteSignedByte((sbyte)Value);
    }

    public override string ToString() => Value switch
    {
        0 => "survival",
        1 => "creative",
        2 => "adventure",
        3 => "spectator",
        _ => $"unknown({Value})"};
    public string ToString(int protocolVersion)
    {
        if (protocolVersion >= 766)
        {
            return Value switch
            {
                0 => "survival",
                1 => "creative",
                2 => "adventure",
                3 => "spectator",
                _ => $"unknown({Value})"};
        }

        return $"unknown({Value})";
    }
}
