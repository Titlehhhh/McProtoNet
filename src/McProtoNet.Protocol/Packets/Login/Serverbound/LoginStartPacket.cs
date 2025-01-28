using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Login.Serverbound;

[PacketInfo("LoginStart", PacketState.Login, PacketDirection.Serverbound)]
public partial class LoginStartPacket : IClientPacket
{
    public string Username { get; set; }

    [PacketSubInfo(340, 758)]
    public sealed partial class V340_758 : LoginStartPacket
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
    }

    [PacketSubInfo(759, 759)]
    public sealed partial class V759 : LoginStartPacket
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
    }

    [PacketSubInfo(760, 760)]
    public sealed partial class V760 : LoginStartPacket
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
    }

    [PacketSubInfo(761, 763)]
    public sealed partial class V761_763 : LoginStartPacket
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
    }

    [PacketSubInfo(764, 769)]
    public sealed partial class V764_769 : LoginStartPacket
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
    }


    public virtual void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        if (V340_758.IsSupportedVersionStatic(protocolVersion))
        {
            V340_758.SerializeInternal(ref writer, protocolVersion, Username);
        }
        else if (V759.IsSupportedVersionStatic(protocolVersion))
        {
            V759.SerializeInternal(ref writer, protocolVersion, Username, false, null, null, null);
        }
        else if (V760.IsSupportedVersionStatic(protocolVersion))
        {
            V760.SerializeInternal(ref writer, protocolVersion, Username, false, null, null, null, false, null);
        }
        else if (V761_763.IsSupportedVersionStatic(protocolVersion))
        {
            V761_763.SerializeInternal(ref writer, protocolVersion, Username, false, null);
        }
        else if (V764_769.IsSupportedVersionStatic(protocolVersion))
        {
            V764_769.SerializeInternal(ref writer, protocolVersion, Username, Guid.Empty);
        }
        else
        {
            throw new ProtocolNotSupportException(ClientLoginPacket.LoginStart.ToString(), protocolVersion);
        }
    }
}