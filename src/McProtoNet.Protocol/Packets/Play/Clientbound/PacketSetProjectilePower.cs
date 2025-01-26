using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.ClientboundPackets.Play
{
    public abstract class SetProjectilePowerPacket : IServerPacket
    {
        public int Id { get; set; }

        public sealed class V766 : SetProjectilePowerPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                Id = reader.ReadVarInt();
                Power = reader.ReadVector3F64(protocolVersion);
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion == 766;
            }

            public Vector3F64 Power { get; set; }
        }

        public sealed class V767_769 : SetProjectilePowerPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                Id = reader.ReadVarInt();
                AccelerationPower = reader.ReadDouble();
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion is >= 767 and <= 769;
            }

            public double AccelerationPower { get; set; }
        }

        public static bool SupportedVersion(int protocolVersion)
        {
            return V766.SupportedVersion(protocolVersion) || V767_769.SupportedVersion(protocolVersion);
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
        public static PacketIdentifier PacketId => ServerPlayPacket.SetProjectilePower;

        public PacketIdentifier GetPacketId() => PacketId;
    }
}