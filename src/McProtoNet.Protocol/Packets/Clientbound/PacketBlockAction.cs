using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.ClientboundPackets
{
    public abstract class BlockActionPacket : IServerPacket
    {
        public Position Location { get; set; }
        public byte Byte1 { get; set; }
        public byte Byte2 { get; set; }
        public int BlockId { get; set; }

        internal sealed class V340_769 : BlockActionPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                Location = reader.ReadPosition(protocolVersion);
                Byte1 = reader.ReadUnsignedByte();
                Byte2 = reader.ReadUnsignedByte();
                BlockId = reader.ReadVarInt();
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion is >= 340 and <= 769;
            }
        }

        public static bool SupportedVersion(int protocolVersion)
        {
            return V340_769.SupportedVersion(protocolVersion);
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
        public static ServerPacket PacketId => ServerPacket.BlockAction;

        public ServerPacket GetPacketId() => PacketId;
    }
}