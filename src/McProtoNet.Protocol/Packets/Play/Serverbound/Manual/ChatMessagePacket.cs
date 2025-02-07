using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Serverbound
{
    [PacketInfo("ChatMessage", PacketState.Play, PacketDirection.Serverbound)]
    public partial class ChatMessagePacket : IClientPacket
    {
        public string Message { get; set; }
        public long Timestamp { get; set; }
        public long Salt { get; set; }

        [PacketSubInfo(759, 759)]
        public sealed partial class V759 : ChatMessagePacket
        {
            public byte[] Signature { get; set; }
            public bool SignedPreview { get; set; }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion,
                string message, long timestamp, long salt, byte[] signature, bool signedPreview)
            {
                writer.WriteString(message);
                writer.WriteSignedLong(timestamp);
                writer.WriteSignedLong(salt);
                writer.WriteVarInt(signature.Length);
                writer.WriteBuffer(signature);
                writer.WriteBoolean(signedPreview);
            }

            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, Message, Timestamp, Salt, Signature, SignedPreview);
            }
        }


        [PacketSubInfo(760, 760)]
        public sealed partial class V760 : ChatMessagePacket
        {
            public struct PreviousMessage
            {
                public Guid MessageSender { get; set; }
                public byte[] MessageSignature { get; set; }

                public void Serialize(ref MinecraftPrimitiveWriter writer)
                {
                    writer.WriteUUID(MessageSender);
                    writer.WriteVarInt(MessageSignature.Length);
                    writer.WriteBuffer(MessageSignature);
                }
            }

            public byte[] Signature { get; set; }
            public bool SignedPreview { get; set; }
            public PreviousMessage[] PreviousMessages { get; set; }
            public LastRejectedMessage? LastRejectedMessage { get; set; }


            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion,
                string message, long timestamp, long salt, byte[] signature, bool signedPreview,
                PreviousMessage[] previousMessages, LastRejectedMessage? lastRejectedMessage)
            {
                writer.WriteString(message);
                writer.WriteSignedLong(timestamp);
                writer.WriteSignedLong(salt);
                writer.WriteVarInt(signature.Length);
                writer.WriteBuffer(signature);
                writer.WriteBoolean(signedPreview);
                writer.WriteVarInt(previousMessages.Length);
                for (int i = 0; i < previousMessages.Length; i++)
                {
                    previousMessages[i].Serialize(ref writer);
                }

                if (lastRejectedMessage.HasValue)
                {
                    writer.WriteBoolean(true);
                    lastRejectedMessage.Value.Serialize(ref writer);
                }

                else
                {
                    writer.WriteBoolean(false);
                }
            }
        }


        [PacketSubInfo(761, 769)]
        public sealed partial class V761_769 : ChatMessagePacket
        {
            public byte[]? Signature { get; set; }
            public int Offset { get; set; }
            public byte[] Acknowledged { get; set; }

            internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion,
                string message, long timestamp, long salt, byte[]? signature, int offset, byte[] acknowledged)
            {
                if (acknowledged.Length != 3)
                {
                    throw new ArgumentException("Acknowledged length is not 3");
                }

                writer.WriteString(message);
                writer.WriteSignedLong(timestamp);
                writer.WriteSignedLong(salt);

                if (signature is not null)
                {
                    writer.WriteBoolean(true);

                    if (signature.Length != 256)
                    {
                        throw new ArgumentException("Signature length is not 256");
                    }

                    writer.WriteBuffer(signature);
                }
                else
                {
                    writer.WriteBoolean(false);
                }

                writer.WriteVarInt(offset);

                writer.WriteBuffer(acknowledged);
            }


            public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
            {
                SerializeInternal(ref writer, protocolVersion, Message, Timestamp, Salt, Signature, Offset,
                    Acknowledged);
            }
        }


        public virtual void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        {
            if (V759.IsSupportedVersionStatic(protocolVersion))
                V759.SerializeInternal(ref writer, protocolVersion, Message, Timestamp, Salt, [], false);
            else if (V760.IsSupportedVersionStatic(protocolVersion))
                V760.SerializeInternal(ref writer, protocolVersion, Message, Timestamp, Salt, [], false, [], null);
            else if (V761_769.IsSupportedVersionStatic(protocolVersion))

                V761_769.SerializeInternal(ref writer, protocolVersion, Message, Timestamp, Salt, null, 0,
                    [0, 0, 0]);
            else
                throw new ProtocolNotSupportException(nameof(ClientPlayPacket.ChatMessage), protocolVersion);
        }
    }

    public struct LastRejectedMessage
    {
        public Guid Sender { get; set; }
        public byte[] Signature { get; set; }

        public void Serialize(ref MinecraftPrimitiveWriter writer)
        {
            writer.WriteUUID(Sender);
            writer.WriteVarInt(Signature.Length);
            writer.WriteBuffer(Signature);
        }
    }
}