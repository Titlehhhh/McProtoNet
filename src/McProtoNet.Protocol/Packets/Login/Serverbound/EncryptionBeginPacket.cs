using McProtoNet.Protocol;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.ServerboundPackets.Login;

public class EncryptionBeginPacket : IClientPacket
{
    public byte[] SharedSecret { get; set; }


    public static PacketIdentifier PacketId => ClientLoginPacket.EncryptionBegin;

    public PacketIdentifier GetPacketId() => PacketId;

    public sealed class V340_758 : EncryptionBeginPacket
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

        public new static bool SupportedVersion(int protocolVersion)
        {
            return protocolVersion is >= 340 and <= 758;
        }
    }

    public sealed class V759_760 : EncryptionBeginPacket
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

        public new static bool SupportedVersion(int protocolVersion)
        {
            return protocolVersion is >= 759 and <= 760;
        }
    }

    public sealed class V761_769 : EncryptionBeginPacket
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

        public new static bool SupportedVersion(int protocolVersion)
        {
            return protocolVersion is >= 761 and <= 769;
        }
    }

    public static bool SupportedVersion(int protocolVersion)
    {
        return V340_758.SupportedVersion(protocolVersion) ||
               V759_760.SupportedVersion(protocolVersion) ||
               V761_769.SupportedVersion(protocolVersion);
    }

    public virtual void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        if (V340_758.SupportedVersion(protocolVersion))
        {
            V340_758.SerializeInternal(ref writer, protocolVersion, SharedSecret, []);
        }
        else if (V759_760.SupportedVersion(protocolVersion))
        {
            V759_760.SerializeInternal(ref writer, protocolVersion, SharedSecret, false, [], 0, []);
        }
        else if (V761_769.SupportedVersion(protocolVersion))
        {
            V761_769.SerializeInternal(ref writer, protocolVersion, SharedSecret, []);
        }
        else
        {
            throw new ProtocolNotSupportException(ClientLoginPacket.EncryptionBegin.ToString(), protocolVersion);
        }
    }
}

/*
packet login plugin response
{
  "340-351": "empty",
  "393-769": [
    {
      "name": "messageId",
      "type": "varint"
    },
    {
      "name": "data",
      "type": [
        "option",
        "restBuffer"
      ]
    }
  ]
}
*/

public class PluginResponsePacket : IClientPacket
{
    public int MessageId { get; set; }
    public byte[]? Data { get; set; }

    public sealed class V393_769 : PluginResponsePacket
    {
        public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        {
            SerializeInternal(ref writer, protocolVersion, MessageId, Data);
        }

        internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion,
            int messageId, byte[]? data)
        {
            writer.WriteVarInt(messageId);
            writer.WriteBoolean(data is not null);
            if (data is not null)
            {
                writer.WriteBuffer(data!);
            }
        }

        public new static bool SupportedVersion(int protocolVersion)
        {
            return protocolVersion is >= 393 and <= 769;
        }
    }

    public static bool SupportedVersion(int protocolVersion)
    {
        return V393_769.SupportedVersion(protocolVersion);
    }

    public virtual void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        if (V393_769.SupportedVersion(protocolVersion))
        {
            V393_769.SerializeInternal(ref writer, protocolVersion, MessageId, Data);
        }
        else
        {
            throw new ProtocolNotSupportException(ClientLoginPacket.LoginPluginResponse.ToString(), protocolVersion);
        }
    }

    public static PacketIdentifier PacketId => ClientLoginPacket.LoginPluginResponse;

    public PacketIdentifier GetPacketId() => PacketId;
}

public class LoginStartPacket : IClientPacket
{
    public string Username { get; set; }

    public sealed class V340_758 : LoginStartPacket
    {
        public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        {
            SerializeInternal(ref writer, protocolVersion, Username);
        }

        internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion,
            string username)
        {
            writer.WriteString(username);
        }

        public new static bool SupportedVersion(int protocolVersion)
        {
            return protocolVersion is >= 340 and <= 758;
        }
    }

    public sealed class V759 : LoginStartPacket
    {
        public bool HasSignature { get; set; }
        public long? Timestamp { get; set; }
        public byte[]? PublicKey { get; set; }
        public byte[]? Signature { get; set; }

        public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        {
            SerializeInternal(ref writer, protocolVersion, Username, HasSignature, Timestamp, PublicKey, Signature);
        }

        internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion,
            string username, bool hasSignature, long? timestamp, byte[]? publicKey, byte[]? signature)
        {
            writer.WriteString(username);
            writer.WriteBoolean(hasSignature);
            if (hasSignature)
            {
                writer.WriteSignedLong(timestamp!.Value);
                writer.WriteVarInt(publicKey!.Length);
                writer.WriteBuffer(publicKey);
                writer.WriteVarInt(signature!.Length);
                writer.WriteBuffer(signature);
            }
        }

        public new static bool SupportedVersion(int protocolVersion)
        {
            return protocolVersion == 759;
        }
    }

    public sealed class V760 : LoginStartPacket
    {
        public bool HasSignature { get; set; }
        public long? Timestamp { get; set; }
        public byte[]? PublicKey { get; set; }
        public byte[]? Signature { get; set; }
        public bool HasPlayerUUID { get; set; }
        public Guid? PlayerUUID { get; set; }

        public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        {
            SerializeInternal(ref writer, protocolVersion, Username, HasSignature, Timestamp, PublicKey, Signature,
                HasPlayerUUID, PlayerUUID);
        }

        internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion,
            string username, bool hasSignature, long? timestamp, byte[]? publicKey, byte[]? signature,
            bool hasPlayerUUID, Guid? playerUUID)
        {
            writer.WriteString(username);
            writer.WriteBoolean(hasSignature);
            if (hasSignature)
            {
                writer.WriteSignedLong(timestamp!.Value);
                writer.WriteVarInt(publicKey!.Length);
                writer.WriteBuffer(publicKey);
                writer.WriteVarInt(signature!.Length);
                writer.WriteBuffer(signature);
            }

            writer.WriteBoolean(hasPlayerUUID);
            if (hasPlayerUUID)
            {
                writer.WriteUUID(playerUUID!.Value);
            }
        }

        public new static bool SupportedVersion(int protocolVersion)
        {
            return protocolVersion == 760;
        }
    }

    public sealed class V761_763 : LoginStartPacket
    {
        public bool HasPlayerUUID { get; set; }
        public Guid? PlayerUUID { get; set; }

        public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        {
            SerializeInternal(ref writer, protocolVersion, Username, HasPlayerUUID, PlayerUUID);
        }

        internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion,
            string username, bool hasPlayerUUID, Guid? playerUUID)
        {
            writer.WriteString(username);
            writer.WriteBoolean(hasPlayerUUID);
            if (hasPlayerUUID)
            {
                writer.WriteUUID(playerUUID!.Value);
            }
        }

        public new static bool SupportedVersion(int protocolVersion)
        {
            return protocolVersion is >= 761 and <= 763;
        }
    }

    public sealed class V764_769 : LoginStartPacket
    {
        public Guid PlayerUUID { get; set; }

        public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        {
            SerializeInternal(ref writer, protocolVersion, Username, PlayerUUID);
        }

        internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion,
            string username, Guid playerUUID)
        {
            writer.WriteString(username);
            writer.WriteUUID(playerUUID);
        }

        public new static bool SupportedVersion(int protocolVersion)
        {
            return protocolVersion is >= 764 and <= 769;
        }
    }

    public static bool SupportedVersion(int protocolVersion)
    {
        return V340_758.SupportedVersion(protocolVersion) ||
               V759.SupportedVersion(protocolVersion) ||
               V760.SupportedVersion(protocolVersion) ||
               V761_763.SupportedVersion(protocolVersion) ||
               V764_769.SupportedVersion(protocolVersion);
    }

    public virtual void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        if (V340_758.SupportedVersion(protocolVersion))
        {
            V340_758.SerializeInternal(ref writer, protocolVersion, Username);
        }
        else if (V759.SupportedVersion(protocolVersion))
        {
            V759.SerializeInternal(ref writer, protocolVersion, Username, false, null, null, null);
        }
        else if (V760.SupportedVersion(protocolVersion))
        {
            V760.SerializeInternal(ref writer, protocolVersion, Username, false, null, null, null, false, null);
        }
        else if (V761_763.SupportedVersion(protocolVersion))
        {
            V761_763.SerializeInternal(ref writer, protocolVersion, Username, false, null);
        }
        else if (V764_769.SupportedVersion(protocolVersion))
        {
            V764_769.SerializeInternal(ref writer, protocolVersion, Username, Guid.Empty);
        }
        else
        {
            throw new ProtocolNotSupportException(ClientLoginPacket.LoginStart.ToString(), protocolVersion);
        }
    }

    public static PacketIdentifier PacketId => ClientLoginPacket.LoginStart;

    public PacketIdentifier GetPacketId() => PacketId;
}