using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.ClientboundPackets.Play
{
    public abstract class EntityTeleportPacket : IServerPacket
    {
        public int EntityId { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public sbyte Yaw { get; set; }
        public sbyte Pitch { get; set; }
        public bool OnGround { get; set; }

        internal sealed class V340_769 : EntityTeleportPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                EntityId = reader.ReadVarInt();
                X = reader.ReadDouble();
                Y = reader.ReadDouble();
                Z = reader.ReadDouble();
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
        public static PacketIdentifier PacketId => ServerPlayPacket.EntityTeleport;

        public PacketIdentifier GetPacketId() => PacketId;
    }
}