using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.ClientboundPackets
{
    public abstract class ChatPacket : IServerPacket
    {
        public string Message { get; set; }
        public sbyte Position { get; set; }

        internal sealed class V340_710 : ChatPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                Message = reader.ReadString();
                Position = reader.ReadSignedByte();
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion is >= 340 and <= 710;
            }
        }

        internal sealed class V734_758 : ChatPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                Message = reader.ReadString();
                Position = reader.ReadSignedByte();
                Sender = reader.ReadUUID();
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion is >= 734 and <= 758;
            }

            public Guid Sender { get; set; }
        }

        public static bool SupportedVersion(int protocolVersion)
        {
            return V340_710.SupportedVersion(protocolVersion) || V734_758.SupportedVersion(protocolVersion);
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
        public static ServerPacket PacketId => ServerPacket.Chat;

        public ServerPacket GetPacketId() => PacketId;
    }
}