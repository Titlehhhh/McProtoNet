using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Login.Serverbound;

[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("login.toServer.encryption_begin", PacketPhase.Login, PacketDirection.Serverbound)]
[PacketField("SharedSecret", "byte[]")]
[PacketField("VerifyToken", "byte[]?")]
[PacketField("Salt", "long?", Group = "V759_760", From = 759, To = 760)]
[PacketField("MessageSignature", "byte[]?", Group = "V759_760", From = 759, To = 760)]
public sealed partial record EncryptionResponsePacket(byte[] SharedSecret, byte[]? VerifyToken, EncryptionResponsePacket.V759_760Layer? V759_760 = null) : IPacket<EncryptionResponsePacket>, IPacket
{
    public readonly record struct V759_760Layer(long? Salt, byte[]? MessageSignature);
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
            var sharedSecret = reader.ReadByteArray();
            var _hasVerifyToken = reader.ReadBoolean();
            byte[]? verifyToken = default;
            long? salt = default;
            byte[]? messageSignature = default;
            if (_hasVerifyToken)
            {
                var verifyTokenValue = reader.ReadByteArray();
                verifyToken = verifyTokenValue;
            }
            else
            {
                var saltValue = reader.ReadSignedLong();
                var messageSignatureValue = reader.ReadByteArray();
                salt = saltValue;
                messageSignature = messageSignatureValue;
            }

            return new EncryptionResponsePacket(sharedSecret, verifyToken, V759_760: new V759_760Layer(salt, messageSignature));
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
            var layer = V759_760 ?? throw new WrongLayerException("EncryptionResponsePacket", protocolVersion, "V759_760");
            long? Salt = layer.Salt;
            byte[]? MessageSignature = layer.MessageSignature;
            writer.WriteByteArray(SharedSecret);
            bool _hasVerifyToken = VerifyToken is not null ? true : Salt is not null && MessageSignature is not null ? false : throw new System.InvalidOperationException("No inline union case selected by '_hasVerifyToken' matches the fields that are set.");
            writer.WriteBoolean(_hasVerifyToken);
            if (_hasVerifyToken)
            {
                writer.WriteByteArray((VerifyToken ?? throw new System.InvalidOperationException("VerifyToken is required at this protocol version.")));
            }
            else
            {
                writer.WriteSignedLong((Salt ?? throw new System.InvalidOperationException("Salt is required at this protocol version.")));
                writer.WriteByteArray((MessageSignature ?? throw new System.InvalidOperationException("MessageSignature is required at this protocol version.")));
            }

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

    public static PacketIdentity Identity => new("login.toServer.encryption_begin", "EncryptionResponse", PacketPhase.Login, PacketDirection.Serverbound, 1);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        return PacketRegistry.TryGetId(Identity, protocolVersion, out id);
    }

    public static int GetPacketId(int protocolVersion)
    {
        return PacketRegistry.GetId(Identity, protocolVersion);
    }
}
