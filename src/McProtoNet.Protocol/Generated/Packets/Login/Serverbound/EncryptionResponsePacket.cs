using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Login.Serverbound;
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("login.toServer.encryption_begin", PacketPhase.Login, PacketDirection.Serverbound)]
[PacketField("SharedSecret", "byte[]")]
[PacketField("VerifyToken", "byte[]?")]
public sealed partial record EncryptionResponsePacket(byte[] SharedSecret, byte[]? VerifyToken) : IPacket<EncryptionResponsePacket>, IPacket
{
    public static EncryptionResponsePacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<EncryptionResponsePacket>(protocolVersion);
        if (protocolVersion <= 758)
        {
            var sharedSecret = reader.ReadByteArray();
            var verifyToken = reader.ReadByteArray();
            return new EncryptionResponsePacket(sharedSecret, verifyToken);
        }

        if (protocolVersion >= 759 && protocolVersion <= 760)
        {
            // TODO(codegen): InlineUnion   ("_hasVerifyToken",    [{ Keys = [1]       Name = "VerifyToken"       Entries = [Read ("verifyToken", ByteArray, "VerifyToken")] };     { Keys = [0]       Name = "SaltSignature"       Entries =        [Read ("salt", I64, "Salt");         Read ("messageSignature", ByteArray, "MessageSignature")] }])
            throw new System.NotImplementedException("TODO(codegen): EncryptionResponsePacket wire layout is not fully generated for this protocol version.");
        }

        if (protocolVersion >= 761)
        {
            var sharedSecret = reader.ReadByteArray();
            var verifyToken = reader.ReadByteArray();
            return new EncryptionResponsePacket(sharedSecret, verifyToken);
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
            // TODO(codegen): write wire-only '_hasVerifyToken' (derive from model)
            // TODO(codegen): InlineUnion   ("_hasVerifyToken",    [{ Keys = [1]       Name = "VerifyToken"       Entries = [Read ("verifyToken", ByteArray, "VerifyToken")] };     { Keys = [0]       Name = "SaltSignature"       Entries =        [Read ("salt", I64, "Salt");         Read ("messageSignature", ByteArray, "MessageSignature")] }])
            throw new System.NotImplementedException("TODO(codegen): EncryptionResponsePacket wire layout is not fully generated for this protocol version.");
        }

        if (protocolVersion >= 761)
        {
            writer.WriteByteArray(SharedSecret);
            writer.WriteByteArray((VerifyToken ?? throw new System.InvalidOperationException("VerifyToken is required at this protocol version.")));
            return;
        }

        throw new System.NotSupportedException($"EncryptionResponsePacket has no wire layout for protocol version {protocolVersion}.");
    }

    public static PacketIdentity Identity => new("login.toServer.encryption_begin", "EncryptionResponse", PacketPhase.Login, PacketDirection.Serverbound, 1);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 735 && protocolVersion <= 776)
        {
            id = 0x01;
            return true;
        }

        id = 0;
        return false;
    }

    public static int GetPacketId(int protocolVersion)
    {
        if (TryGetPacketId(protocolVersion, out var id))
            return id;
        throw new System.NotSupportedException($"No packet id for protocol {protocolVersion}.");
    }
}
