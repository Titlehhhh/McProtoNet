using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.ServerboundPackets
{
    public class ChatSessionUpdatePacket : IClientPacket
    {
        public Guid SessionUUID { get; set; }
        public long ExpireTime { get; set; }
        public byte[] PublicKey { get; set; }
        public byte[] Signature { get; set; }

        public sealed class V761_765 : ChatSessionUpdatePacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, SessionUUID, ExpireTime, PublicKey, Signature);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion, Guid sessionUUID, long expireTime, byte[] publicKey, byte[] signature)
            {
                writer.WriteUUID(sessionUUID);
                writer.WriteSignedLong(expireTime);
                writer.WriteVarInt(publicKey.Length);
                writer.WriteBuffer(publicKey);
                writer.WriteVarInt(signature.Length);
                writer.WriteBuffer(signature);
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion is >= 761 and <= 765;
            }
        }

        public sealed class V766_769 : ChatSessionUpdatePacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, SessionUUID, ExpireTime, PublicKey, Signature);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion, Guid sessionUUID, long expireTime, byte[] publicKey, byte[] signature)
            {
                writer.WriteUUID(sessionUUID);
                writer.WriteSignedLong(expireTime);
                writer.WriteVarInt(publicKey.Length);
                writer.WriteBuffer(publicKey);
                writer.WriteVarInt(signature.Length);
                writer.WriteBuffer(signature);
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion is >= 766 and <= 769;
            }
        }

        public static bool SupportedVersion(int protocolVersion)
        {
            return V761_765.SupportedVersion(protocolVersion) || V766_769.SupportedVersion(protocolVersion);
        }

        public virtual void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        {
            if (V761_765.SupportedVersion(protocolVersion))
                V761_765.SerializeInternal(ref writer, protocolVersion, SessionUUID, ExpireTime, PublicKey, Signature);
            else if (V766_769.SupportedVersion(protocolVersion))
                V766_769.SerializeInternal(ref writer, protocolVersion, SessionUUID, ExpireTime, PublicKey, Signature);
            else
                throw new ProtocolNotSupportException(nameof(ClientPacket.ChatSessionUpdate), protocolVersion);
        }

        public static ClientPacket PacketId => ClientPacket.ChatSessionUpdate;

        public ClientPacket GetPacketId() => PacketId;
    }
}