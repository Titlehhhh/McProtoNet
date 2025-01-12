using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.ClientboundPackets
{
    public abstract class PositionPacket : IServerPacket
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public float Yaw { get; set; }
        public float Pitch { get; set; }
        public int TeleportId { get; set; }

        public sealed class V340_754 : PositionPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                X = reader.ReadDouble();
                Y = reader.ReadDouble();
                Z = reader.ReadDouble();
                Yaw = reader.ReadFloat();
                Pitch = reader.ReadFloat();
                Flags = reader.ReadSignedByte();
                TeleportId = reader.ReadVarInt();
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion is >= 340 and <= 754;
            }

            public sbyte Flags { get; set; }
        }

        public sealed class V755_761 : PositionPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                X = reader.ReadDouble();
                Y = reader.ReadDouble();
                Z = reader.ReadDouble();
                Yaw = reader.ReadFloat();
                Pitch = reader.ReadFloat();
                Flags = reader.ReadSignedByte();
                TeleportId = reader.ReadVarInt();
                DismountVehicle = reader.ReadBoolean();
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion is >= 755 and <= 761;
            }

            public sbyte Flags { get; set; }
            public bool DismountVehicle { get; set; }
        }

        public sealed class V762_767 : PositionPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                X = reader.ReadDouble();
                Y = reader.ReadDouble();
                Z = reader.ReadDouble();
                Yaw = reader.ReadFloat();
                Pitch = reader.ReadFloat();
                Flags = reader.ReadSignedByte();
                TeleportId = reader.ReadVarInt();
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion is >= 762 and <= 767;
            }

            public sbyte Flags { get; set; }
        }

        public sealed class V768_769 : PositionPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                TeleportId = reader.ReadVarInt();
                X = reader.ReadDouble();
                Y = reader.ReadDouble();
                Z = reader.ReadDouble();
                Dx = reader.ReadDouble();
                Dy = reader.ReadDouble();
                Dz = reader.ReadDouble();
                Yaw = reader.ReadFloat();
                Pitch = reader.ReadFloat();
                Flags = reader.ReadUnsignedInt();
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion is >= 768 and <= 769;
            }

            public double Dx { get; set; }
            public double Dy { get; set; }
            public double Dz { get; set; }
            public uint Flags { get; set; }
        }

        public static bool SupportedVersion(int protocolVersion)
        {
            return V340_754.SupportedVersion(protocolVersion) || V755_761.SupportedVersion(protocolVersion) || V762_767.SupportedVersion(protocolVersion) || V768_769.SupportedVersion(protocolVersion);
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
        public static ServerPacket PacketId => ServerPacket.Position;

        public ServerPacket GetPacketId() => PacketId;
    }
}