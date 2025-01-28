using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound
{
    [PacketInfo("SpawnEntityExperienceOrb", PacketState.Play, PacketDirection.Clientbound)]
    public abstract partial class SpawnEntityExperienceOrbPacket : IServerPacket
    {
        public int EntityId { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public short Count { get; set; }

        [PacketSubInfo(340, 769)]
        internal sealed partial class V340_769 : SpawnEntityExperienceOrbPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                EntityId = reader.ReadVarInt();
                X = reader.ReadDouble();
                Y = reader.ReadDouble();
                Z = reader.ReadDouble();
                Count = reader.ReadSignedShort();
            }
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
    }
}