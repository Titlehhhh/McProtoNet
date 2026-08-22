using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol;
[ProtocolSupport(768, MinecraftVersion.LatestProtocol)]
public readonly partial record struct ParticleStatus(int Value) : IProtocolType<ParticleStatus>
{
    public static readonly ParticleStatus All = new(0);
    public static readonly ParticleStatus Decreased = new(1);
    public static readonly ParticleStatus Minimal = new(2);
    public static explicit operator int (ParticleStatus value) => value.Value;
    public static explicit operator ParticleStatus(int value) => new(value);
    public static ParticleStatus Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ParticleStatus>(protocolVersion);
        return new ParticleStatus((int)reader.ReadVarInt());
    }

    public readonly void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ParticleStatus>(protocolVersion);
        writer.WriteVarInt((int)Value);
    }

    public override string ToString() => Value switch
    {
        0 => "all",
        1 => "decreased",
        2 => "minimal",
        _ => $"unknown({Value})"};
    public string ToString(int protocolVersion)
    {
        if (protocolVersion >= 768)
        {
            return Value switch
            {
                0 => "all",
                1 => "decreased",
                2 => "minimal",
                _ => $"unknown({Value})"};
        }

        return $"unknown({Value})";
    }
}
