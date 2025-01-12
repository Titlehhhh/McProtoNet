using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.ClientboundPackets
{
    public abstract class OpenHorseWindowPacket : IServerPacket
    {
        public int NbSlots { get; set; }
        public int EntityId { get; set; }

        public sealed class V477_767 : OpenHorseWindowPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                WindowId = reader.ReadUnsignedByte();
                NbSlots = reader.ReadVarInt();
                EntityId = reader.ReadSignedInt();
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion is >= 477 and <= 767;
            }

            public byte WindowId { get; set; }
        }

        public sealed class V768_769 : OpenHorseWindowPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                WindowId = reader.ReadVarInt();
                NbSlots = reader.ReadVarInt();
                EntityId = reader.ReadSignedInt();
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion is >= 768 and <= 769;
            }

            public int WindowId { get; set; }
        }

        public static bool SupportedVersion(int protocolVersion)
        {
            return V477_767.SupportedVersion(protocolVersion) || V768_769.SupportedVersion(protocolVersion);
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
        public static ServerPacket PacketId => ServerPacket.OpenHorseWindow;

        public ServerPacket GetPacketId() => PacketId;
    }
}