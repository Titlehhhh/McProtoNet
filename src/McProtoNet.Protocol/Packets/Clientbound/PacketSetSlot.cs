using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.ClientboundPackets
{
    public abstract class SetSlotPacket : IServerPacket
    {
        public short Slot { get; set; }
        public Slot Item { get; set; }

        public sealed class V340_755 : SetSlotPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                WindowId = reader.ReadSignedByte();
                Slot = reader.ReadSignedShort();
                Item = reader.ReadSlot(protocolVersion);
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion is >= 340 and <= 755;
            }

            public sbyte WindowId { get; set; }
        }

        public sealed class V756_765 : SetSlotPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                WindowId = reader.ReadSignedByte();
                StateId = reader.ReadVarInt();
                Slot = reader.ReadSignedShort();
                Item = reader.ReadSlot(protocolVersion);
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion is >= 756 and <= 765;
            }

            public sbyte WindowId { get; set; }
            public int StateId { get; set; }
        }

        public sealed class V766_767 : SetSlotPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                WindowId = reader.ReadSignedByte();
                StateId = reader.ReadVarInt();
                Slot = reader.ReadSignedShort();
                Item = reader.ReadSlot(protocolVersion);
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion is >= 766 and <= 767;
            }

            public sbyte WindowId { get; set; }
            public int StateId { get; set; }
        }

        public sealed class V768_769 : SetSlotPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                WindowId = reader.ReadVarInt();
                StateId = reader.ReadVarInt();
                Slot = reader.ReadSignedShort();
                Item = reader.ReadSlot(protocolVersion);
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion is >= 768 and <= 769;
            }

            public int WindowId { get; set; }
            public int StateId { get; set; }
        }

        public static bool SupportedVersion(int protocolVersion)
        {
            return V340_755.SupportedVersion(protocolVersion) || V756_765.SupportedVersion(protocolVersion) || V766_767.SupportedVersion(protocolVersion) || V768_769.SupportedVersion(protocolVersion);
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
        public static ServerPacket PacketId => ServerPacket.SetSlot;

        public ServerPacket GetPacketId() => PacketId;
    }
}