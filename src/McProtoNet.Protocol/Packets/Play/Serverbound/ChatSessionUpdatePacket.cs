using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Serverbound
{
    [PacketInfo("ChatSessionUpdate", PacketState.Play, PacketDirection.Serverbound)]
    public partial class ChatSessionUpdatePacket : IClientPacket
    {
        public Guid SessionUUID { get; set; }
        public long ExpireTime { get; set; }
        public byte[] PublicKey { get; set; }
        public byte[] Signature { get; set; }

        [PacketSubInfo(761, 765)]
        public sealed partial class V761_765 : ChatSessionUpdatePacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, SessionUUID, ExpireTime, PublicKey, Signature);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion,
                Guid sessionUUID, long expireTime, byte[] publicKey, byte[] signature)
            {
                writer.WriteUUID(sessionUUID);
                writer.WriteSignedLong(expireTime);
                writer.WriteVarInt(publicKey.Length);
                writer.WriteBuffer(publicKey);
                writer.WriteVarInt(signature.Length);
                writer.WriteBuffer(signature);
            }
        }

        [PacketSubInfo(766, 769)]
        public sealed partial class V766_769 : ChatSessionUpdatePacket
        {
            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, SessionUUID, ExpireTime, PublicKey, Signature);
            }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion,
                Guid sessionUUID, long expireTime, byte[] publicKey, byte[] signature)
            {
                writer.WriteUUID(sessionUUID);
                writer.WriteSignedLong(expireTime);
                writer.WriteVarInt(publicKey.Length);
                writer.WriteBuffer(publicKey);
                writer.WriteVarInt(signature.Length);
                writer.WriteBuffer(signature);
            }
        }

        public virtual void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        {
            if (V761_765.IsSupportedVersionStatic(protocolVersion))
                V761_765.SerializeInternal(ref writer, protocolVersion, SessionUUID, ExpireTime, PublicKey, Signature);
            else if (V766_769.IsSupportedVersionStatic(protocolVersion))
                V766_769.SerializeInternal(ref writer, protocolVersion, SessionUUID, ExpireTime, PublicKey, Signature);
            else
                throw new ProtocolNotSupportException(nameof(ClientPlayPacket.ChatSessionUpdate), protocolVersion);
        }
    }
}