using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

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
        // TODO(codegen): read 'Signature' (FixedBytes 256)
        throw new System.NotImplementedException("TODO(codegen): ArgumentSignature wire layout is not fully generated for this protocol version.");
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ArgumentSignature>(protocolVersion);
        // TODO(codegen): write 'Signature' (FixedBytes 256)
        throw new System.NotImplementedException("TODO(codegen): ArgumentSignature wire layout is not fully generated for this protocol version.");
    }
}
