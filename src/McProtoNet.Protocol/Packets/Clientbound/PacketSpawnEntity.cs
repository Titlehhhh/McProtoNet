using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.ClientboundPackets
{
    public abstract class SpawnEntityPacket : IServerPacket
    {
        public int EntityId { get; set; }
        public Guid ObjectUUID { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public sbyte Pitch { get; set; }
        public sbyte Yaw { get; set; }
        public int ObjectData { get; set; }
        public short VelocityX { get; set; }
        public short VelocityY { get; set; }
        public short VelocityZ { get; set; }

        public sealed class V340_404 : SpawnEntityPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                EntityId = reader.ReadVarInt();
                ObjectUUID = reader.ReadUUID();
                Type = reader.ReadSignedByte();
                X = reader.ReadDouble();
                Y = reader.ReadDouble();
                Z = reader.ReadDouble();
                Pitch = reader.ReadSignedByte();
                Yaw = reader.ReadSignedByte();
                ObjectData = reader.ReadSignedInt();
                VelocityX = reader.ReadSignedShort();
                VelocityY = reader.ReadSignedShort();
                VelocityZ = reader.ReadSignedShort();
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion is >= 340 and <= 404;
            }

            public sbyte Type { get; set; }
        }

        public sealed class V477_758 : SpawnEntityPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                EntityId = reader.ReadVarInt();
                ObjectUUID = reader.ReadUUID();
                Type = reader.ReadVarInt();
                X = reader.ReadDouble();
                Y = reader.ReadDouble();
                Z = reader.ReadDouble();
                Pitch = reader.ReadSignedByte();
                Yaw = reader.ReadSignedByte();
                ObjectData = reader.ReadSignedInt();
                VelocityX = reader.ReadSignedShort();
                VelocityY = reader.ReadSignedShort();
                VelocityZ = reader.ReadSignedShort();
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion is >= 477 and <= 758;
            }

            public int Type { get; set; }
        }

        public sealed class V759_769 : SpawnEntityPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                EntityId = reader.ReadVarInt();
                ObjectUUID = reader.ReadUUID();
                Type = reader.ReadVarInt();
                X = reader.ReadDouble();
                Y = reader.ReadDouble();
                Z = reader.ReadDouble();
                Pitch = reader.ReadSignedByte();
                Yaw = reader.ReadSignedByte();
                HeadPitch = reader.ReadSignedByte();
                ObjectData = reader.ReadVarInt();
                VelocityX = reader.ReadSignedShort();
                VelocityY = reader.ReadSignedShort();
                VelocityZ = reader.ReadSignedShort();
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion is >= 759 and <= 769;
            }

            public int Type { get; set; }
            public sbyte HeadPitch { get; set; }
        }

        public static bool SupportedVersion(int protocolVersion)
        {
            return V340_404.SupportedVersion(protocolVersion) || V477_758.SupportedVersion(protocolVersion) || V759_769.SupportedVersion(protocolVersion);
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
        public static ServerPacket PacketId => ServerPacket.SpawnEntity;

        public ServerPacket GetPacketId() => PacketId;
    }
}