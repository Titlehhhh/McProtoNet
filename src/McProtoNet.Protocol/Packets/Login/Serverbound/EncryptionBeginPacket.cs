using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Login.Serverbound;

[PacketInfo("EncryptionBegin", PacketState.Login, PacketDirection.Serverbound)]
public partial class EncryptionBeginPacket : IClientPacket
{
    public byte[] SharedSecret { get; set; }


    [PacketSubInfo(340, 758)]
    public sealed partial class V340_758 : EncryptionBeginPacket
    {
        public byte[] VerifyToken { get; set; }

        internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion,
            byte[] sharedSecret, byte[] verifyToken)
        {
            writer.WriteVarInt(sharedSecret.Length);
            writer.WriteBuffer(sharedSecret);
            writer.WriteVarInt(verifyToken.Length);
            writer.WriteBuffer(verifyToken);
        }

        public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        {
            SerializeInternal(ref writer, protocolVersion, SharedSecret, VerifyToken);
        }
    }

    [PacketSubInfo(759, 760)]
    public sealed partial class V759_760 : EncryptionBeginPacket
    {
        public bool HasVerifyToken { get; set; }
        public byte[] VerifyToken { get; set; }
        public long Salt { get; set; }
        public byte[] MessageSignature { get; set; }

        internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion,
            byte[] sharedSecret, bool hasVerifyToken, byte[] verifyToken, long salt, byte[] messageSignature)
        {
            writer.WriteVarInt(sharedSecret.Length);
            writer.WriteBuffer(sharedSecret);
            writer.WriteBoolean(hasVerifyToken);

            if (hasVerifyToken)
            {
                writer.WriteVarInt(verifyToken.Length);
                writer.WriteBuffer(verifyToken);
            }
            else
            {
                writer.WriteSignedLong(salt);
                writer.WriteVarInt(messageSignature.Length);
                writer.WriteBuffer(messageSignature);
            }
        }

        public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        {
            SerializeInternal(ref writer, protocolVersion, SharedSecret, HasVerifyToken, VerifyToken, Salt,
                MessageSignature);
        }
    }

    [PacketSubInfo(761, 769)]
    public sealed partial class V761_769 : EncryptionBeginPacket
    {
        public byte[] VerifyToken { get; set; }

        internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion,
            byte[] sharedSecret, byte[] verifyToken)
        {
            writer.WriteVarInt(sharedSecret.Length);
            writer.WriteBuffer(sharedSecret);
            writer.WriteVarInt(verifyToken.Length);
            writer.WriteBuffer(verifyToken);
        }

        public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        {
            SerializeInternal(ref writer, protocolVersion, SharedSecret, VerifyToken);
        }
    }


    public virtual void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        if (V340_758.IsSupportedVersionStatic(protocolVersion))
        {
            V340_758.SerializeInternal(ref writer, protocolVersion, SharedSecret, []);
        }
        else if (V759_760.IsSupportedVersionStatic(protocolVersion))
        {
            V759_760.SerializeInternal(ref writer, protocolVersion, SharedSecret, false, [], 0, []);
        }
        else if (V761_769.IsSupportedVersionStatic(protocolVersion))
        {
            V761_769.SerializeInternal(ref writer, protocolVersion, SharedSecret, []);
        }
        else
        {
            throw new ProtocolNotSupportException(ClientLoginPacket.EncryptionBegin.ToString(), protocolVersion);
        }
    }
}