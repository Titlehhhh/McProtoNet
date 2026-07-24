using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol;
[ProtocolSupport(759, 760)]
public sealed partial class LoginSignature : IProtocolType<LoginSignature>
{
    public long Timestamp { get; }
    public byte[] PublicKey { get; }
    public byte[] Signature { get; }

    public LoginSignature(long timestamp, byte[] publicKey, byte[] signature)
    {
        Timestamp = timestamp;
        PublicKey = publicKey;
        Signature = signature;
    }

    public static LoginSignature Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<LoginSignature>(protocolVersion);
        var timestamp = reader.ReadSignedLong();
        var publicKey = reader.ReadByteArray();
        var signature = reader.ReadByteArray();
        return new LoginSignature(timestamp, publicKey, signature);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<LoginSignature>(protocolVersion);
        writer.WriteSignedLong(Timestamp);
        writer.WriteByteArray(PublicKey);
        writer.WriteByteArray(Signature);
    }
}
