using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound
{
    [PacketInfo("InitializeWorldBorder", PacketState.Play, PacketDirection.Clientbound)]
    public abstract partial class InitializeWorldBorderPacket : IServerPacket
    {
        public double X { get; set; }
        public double Z { get; set; }
        public double OldDiameter { get; set; }
        public double NewDiameter { get; set; }
        public int PortalTeleportBoundary { get; set; }
        public int WarningBlocks { get; set; }
        public int WarningTime { get; set; }

        [PacketSubInfo(755, 758)]
        public sealed partial class V755_758 : InitializeWorldBorderPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                X = reader.ReadDouble();
                Z = reader.ReadDouble();
                OldDiameter = reader.ReadDouble();
                NewDiameter = reader.ReadDouble();
                Speed = reader.ReadVarLong();
                PortalTeleportBoundary = reader.ReadVarInt();
                WarningBlocks = reader.ReadVarInt();
                WarningTime = reader.ReadVarInt();
            }

            public long Speed { get; set; }
        }

        [PacketSubInfo(759, 769)]
        public sealed partial class V759_769 : InitializeWorldBorderPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                X = reader.ReadDouble();
                Z = reader.ReadDouble();
                OldDiameter = reader.ReadDouble();
                NewDiameter = reader.ReadDouble();
                Speed = reader.ReadVarInt();
                PortalTeleportBoundary = reader.ReadVarInt();
                WarningBlocks = reader.ReadVarInt();
                WarningTime = reader.ReadVarInt();
            }

            public int Speed { get; set; }
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
    }
}