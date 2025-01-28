using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound
{
    [PacketInfo("SpawnEntityWeather", PacketState.Play, PacketDirection.Clientbound)]
    public abstract partial class SpawnEntityWeatherPacket : IServerPacket
    {
        public int EntityId { get; set; }
        public sbyte Type { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        [PacketSubInfo(340, 710)]
        public sealed partial class V340_710 : SpawnEntityWeatherPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                EntityId = reader.ReadVarInt();
                Type = reader.ReadSignedByte();
                X = reader.ReadDouble();
                Y = reader.ReadDouble();
                Z = reader.ReadDouble();
            }
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
    }
}