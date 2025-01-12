using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.ClientboundPackets
{
    public abstract class MessageHeaderPacket : IServerPacket
    {
        public byte[]? PreviousSignature { get; set; }
        public Guid SenderUuid { get; set; }
        public byte[] Signature { get; set; }
        public byte[] MessageHash { get; set; }

        internal sealed class V760 : MessageHeaderPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                PreviousSignature = reader.ReadOptional((ref MinecraftPrimitiveReader r_0) => r_0.ReadBuffer(LengthFormat.VarInt));
                SenderUuid = reader.ReadUUID();
                Signature = reader.ReadBuffer(LengthFormat.VarInt);
                MessageHash = reader.ReadBuffer(LengthFormat.VarInt);
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion == 760;
            }
        }

        public static bool SupportedVersion(int protocolVersion)
        {
            return V760.SupportedVersion(protocolVersion);
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
        public static ServerPacket PacketId => ServerPacket.MessageHeader;

        public ServerPacket GetPacketId() => PacketId;
    }
}