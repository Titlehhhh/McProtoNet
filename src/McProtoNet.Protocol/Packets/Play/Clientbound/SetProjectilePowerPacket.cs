using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound
{
    [PacketInfo("SetProjectilePower", PacketState.Play, PacketDirection.Clientbound)]
    public abstract partial class SetProjectilePowerPacket : IServerPacket
    {
        public int Id { get; set; }

        [PacketSubInfo(766, 766)]
        public sealed partial class V766 : SetProjectilePowerPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                Id = reader.ReadVarInt();
                Power = reader.ReadVector3F64(protocolVersion);
            }

            public Vector3F64 Power { get; set; }
        }

        [PacketSubInfo(767, 769)]
        public sealed partial class V767_769 : SetProjectilePowerPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                Id = reader.ReadVarInt();
                AccelerationPower = reader.ReadDouble();
            }

            public double AccelerationPower { get; set; }
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
    }
}