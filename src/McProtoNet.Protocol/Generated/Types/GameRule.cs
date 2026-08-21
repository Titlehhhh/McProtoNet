using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol;
[ProtocolSupport(775, MinecraftVersion.LatestProtocol)]
public sealed partial class GameRule : IProtocolType<GameRule>
{
    public string Name { get; }
    public string Value { get; }

    public GameRule(string name, string value)
    {
        Name = name;
        Value = value;
    }

    public static GameRule Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<GameRule>(protocolVersion);
        var name = reader.ReadString();
        var value = reader.ReadString();
        return new GameRule(name, value);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<GameRule>(protocolVersion);
        writer.WriteString(Name);
        writer.WriteString(Value);
    }
}
