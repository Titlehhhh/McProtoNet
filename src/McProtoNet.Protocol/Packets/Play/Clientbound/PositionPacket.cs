using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound
{
    [PacketInfo("Position", PacketState.Play, PacketDirection.Clientbound)]
    public abstract partial class PositionPacket : IServerPacket
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public float Yaw { get; set; }
        public float Pitch { get; set; }
        public int TeleportId { get; set; }

        [PacketSubInfo(340, 754)]
        public sealed partial class V340_754 : PositionPacket
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

            public sbyte Flags { get; set; }
        }

        [PacketSubInfo(755, 761)]
        public sealed partial class V755_761 : PositionPacket
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

            public sbyte Flags { get; set; }
            public bool DismountVehicle { get; set; }
        }

        [PacketSubInfo(762, 767)]
        public sealed partial class V762_767 : PositionPacket
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

            public sbyte Flags { get; set; }
        }

        [PacketSubInfo(768, 769)]
        public sealed partial class V768_769 : PositionPacket
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

            public double Dx { get; set; }
            public double Dy { get; set; }
            public double Dz { get; set; }
            public uint Flags { get; set; }
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
    }
}