using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.ClientboundPackets
{
    public abstract class BlockBreakAnimationPacket : IServerPacket
    {
        public int EntityId { get; set; }
        public Position Location { get; set; }
        public sbyte DestroyStage { get; set; }

        internal sealed class V340_769 : BlockBreakAnimationPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                EntityId = reader.ReadVarInt();
                Location = reader.ReadPosition(protocolVersion);
                DestroyStage = reader.ReadSignedByte();
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
        public static ServerPacket PacketId => ServerPacket.BlockBreakAnimation;

        public ServerPacket GetPacketId() => PacketId;
    }
}