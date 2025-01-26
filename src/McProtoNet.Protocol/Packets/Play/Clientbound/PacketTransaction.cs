using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.ClientboundPackets.Play
{
    public abstract class TransactionPacket : IServerPacket
    {
        public sbyte WindowId { get; set; }
        public short Action { get; set; }
        public bool Accepted { get; set; }

        public sealed class V340_754 : TransactionPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                WindowId = reader.ReadSignedByte();
                Action = reader.ReadSignedShort();
                Accepted = reader.ReadBoolean();
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion is >= 340 and <= 754;
            }
        }

        public static bool SupportedVersion(int protocolVersion)
        {
            return V340_754.SupportedVersion(protocolVersion);
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
        public static PacketIdentifier PacketId => ServerPlayPacket.Transaction;

        public PacketIdentifier GetPacketId() => PacketId;
    }
}