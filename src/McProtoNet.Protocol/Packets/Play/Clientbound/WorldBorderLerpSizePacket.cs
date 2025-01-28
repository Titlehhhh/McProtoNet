using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound
{
    [PacketInfo("WorldBorderLerpSize", PacketState.Play, PacketDirection.Clientbound)]
    public abstract partial class WorldBorderLerpSizePacket : IServerPacket
    {
        public double OldDiameter { get; set; }
        public double NewDiameter { get; set; }

        [PacketSubInfo(755, 758)]
        public sealed partial class V755_758 : WorldBorderLerpSizePacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                OldDiameter = reader.ReadDouble();
                NewDiameter = reader.ReadDouble();
                Speed = reader.ReadVarLong();
            }

            public long Speed { get; set; }
        }

        [PacketSubInfo(759, 769)]
        public sealed partial class V759_769 : WorldBorderLerpSizePacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                OldDiameter = reader.ReadDouble();
                NewDiameter = reader.ReadDouble();
                Speed = reader.ReadVarInt();
            }

            public int Speed { get; set; }
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
    }
}