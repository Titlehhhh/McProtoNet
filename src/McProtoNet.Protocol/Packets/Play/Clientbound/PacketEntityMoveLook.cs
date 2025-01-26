using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.ClientboundPackets.Play
{
    public abstract class EntityMoveLookPacket : IServerPacket
    {
        public int EntityId { get; set; }
        public short DX { get; set; }
        public short DY { get; set; }
        public short DZ { get; set; }
        public sbyte Yaw { get; set; }
        public sbyte Pitch { get; set; }
        public bool OnGround { get; set; }

        internal sealed class V340_769 : EntityMoveLookPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                EntityId = reader.ReadVarInt();
                DX = reader.ReadSignedShort();
                DY = reader.ReadSignedShort();
                DZ = reader.ReadSignedShort();
                Yaw = reader.ReadSignedByte();
                Pitch = reader.ReadSignedByte();
                OnGround = reader.ReadBoolean();
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
        public static PacketIdentifier PacketId => ServerPlayPacket.EntityMoveLook;

        public PacketIdentifier GetPacketId() => PacketId;
    }
}