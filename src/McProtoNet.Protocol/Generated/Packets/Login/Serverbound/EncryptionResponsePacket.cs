using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Login.Serverbound;
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
public sealed partial class EncryptionResponsePacket : IProtocolType<EncryptionResponsePacket>
{
    public byte[] SharedSecret { get; }
    public byte[]? VerifyToken { get; }
    public long? Salt { get; }
    public byte[]? MessageSignature { get; }

    public EncryptionResponsePacket(byte[] sharedSecret, byte[]? verifyToken, long? salt, byte[]? messageSignature)
    {
        SharedSecret = sharedSecret;
        VerifyToken = verifyToken;
        Salt = salt;
        MessageSignature = messageSignature;
    }

    public static EncryptionResponsePacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<EncryptionResponsePacket>(protocolVersion);
        if (protocolVersion <= 758)
        {
            var sharedSecret = reader.ReadByteArray();
            var verifyToken = reader.ReadByteArray();
            return new EncryptionResponsePacket(sharedSecret, verifyToken, default!, default!);
        }

        if (protocolVersion >= 759 && protocolVersion <= 760)
        {
            var sharedSecret = reader.ReadByteArray();
            var _hasVerifyToken = reader.ReadBoolean();
            // TODO(codegen): InlineUnion   ("_hasVerifyToken",    [{ Keys = [1]       Name = "VerifyToken"       Entries = [Read ("verifyToken", ByteArray, "VerifyToken")] };     { Keys = [0]       Name = "SaltSignature"       Entries =        [Read ("salt", I64, "Salt");         Read ("messageSignature", ByteArray, "MessageSignature")] }])
            return new EncryptionResponsePacket(sharedSecret, default!, default!, default!);
        }

        if (protocolVersion >= 761)
        {
            var sharedSecret = reader.ReadByteArray();
            var verifyToken = reader.ReadByteArray();
            return new EncryptionResponsePacket(sharedSecret, verifyToken, default!, default!);
        }

        throw new System.NotSupportedException($"EncryptionResponsePacket has no wire layout for protocol version {protocolVersion}.");
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<EncryptionResponsePacket>(protocolVersion);
        if (protocolVersion <= 758)
        {
            writer.WriteByteArray(SharedSecret);
            writer.WriteByteArray((VerifyToken ?? throw new System.InvalidOperationException("VerifyToken is required at this protocol version.")));
            return;
        }

        if (protocolVersion >= 759 && protocolVersion <= 760)
        {
            writer.WriteByteArray(SharedSecret);
            // TODO(codegen): write wire-only '_hasVerifyToken' (derive from model)
            // TODO(codegen): InlineUnion   ("_hasVerifyToken",    [{ Keys = [1]       Name = "VerifyToken"       Entries = [Read ("verifyToken", ByteArray, "VerifyToken")] };     { Keys = [0]       Name = "SaltSignature"       Entries =        [Read ("salt", I64, "Salt");         Read ("messageSignature", ByteArray, "MessageSignature")] }])
            return;
        }

        if (protocolVersion >= 761)
        {
            writer.WriteByteArray(SharedSecret);
            writer.WriteByteArray((VerifyToken ?? throw new System.InvalidOperationException("VerifyToken is required at this protocol version.")));
            return;
        }

        throw new System.NotSupportedException($"EncryptionResponsePacket has no wire layout for protocol version {protocolVersion}.");
    }

    public static int GetPacketId(int protocolVersion)
    {
        if (protocolVersion >= 735 && protocolVersion <= 763)
            return 0x01;
        if (protocolVersion >= 764 && protocolVersion <= 765)
            return 0x01;
        if (protocolVersion >= 766 && protocolVersion <= 772)
            return 0x01;
        throw new System.NotSupportedException($"No packet id for protocol {protocolVersion}.");
    }
}
