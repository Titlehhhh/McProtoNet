using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol;
[ProtocolSupport(766, MinecraftVersion.LatestProtocol)]
public sealed partial class ArgumentSignature : IProtocolType<ArgumentSignature>
{
    public string ArgumentName { get; }
    public byte[] Signature { get; }

    public ArgumentSignature(string argumentName, byte[] signature)
    {
        ArgumentName = argumentName;
        Signature = signature;
    }

    public static ArgumentSignature Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ArgumentSignature>(protocolVersion);
        var argumentName = reader.ReadString();
        var signature = reader.ReadFixedBytes(256);
        return new ArgumentSignature(argumentName, signature);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ArgumentSignature>(protocolVersion);
        writer.WriteString(ArgumentName);
        writer.WriteFixedBytes(Signature, 256);
    }
}
