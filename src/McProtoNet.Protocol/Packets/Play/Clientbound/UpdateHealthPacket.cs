using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound
{
    [PacketInfo("UpdateHealth", PacketState.Play, PacketDirection.Clientbound)]
    public abstract partial class UpdateHealthPacket : IServerPacket
    {
        public float Health { get; set; }
        public int Food { get; set; }
        public float FoodSaturation { get; set; }

        [PacketSubInfo(340, 769)]
        internal sealed partial class V340_769 : UpdateHealthPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                Health = reader.ReadFloat();
                Food = reader.ReadVarInt();
                FoodSaturation = reader.ReadFloat();
            }
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
    }
}