using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol;
[ProtocolSupport(759, MinecraftVersion.LatestProtocol)]
public sealed partial class ProfileProperty : IProtocolType<ProfileProperty>
{
    public string Name { get; }
    public string Value { get; }
    public string? Signature { get; }

    public ProfileProperty(string name, string value, string? signature)
    {
        Name = name;
        Value = value;
        Signature = signature;
    }

    public static ProfileProperty Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ProfileProperty>(protocolVersion);
        var name = reader.ReadString();
        var value = reader.ReadString();
        string? signature = null;
        if (reader.ReadBoolean())
            signature = reader.ReadString();
        return new ProfileProperty(name, value, signature);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ProfileProperty>(protocolVersion);
        writer.WriteString(Name);
        writer.WriteString(Value);
        writer.WriteBoolean(Signature is not null);
        if (Signature is { } signatureValue)
            writer.WriteString(signatureValue);
    }
}
