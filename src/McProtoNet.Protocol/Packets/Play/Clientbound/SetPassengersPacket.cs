using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound
{
    [PacketInfo("SetPassengers", PacketState.Play, PacketDirection.Clientbound)]
    public abstract partial class SetPassengersPacket : IServerPacket
    {
        public int EntityId { get; set; }
        public int[] Passengers { get; set; }

        [PacketSubInfo(340, 769)]
        internal sealed partial class V340_769 : SetPassengersPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                EntityId = reader.ReadVarInt();
                Passengers = reader.ReadArray<int, VarIntArrayReader>(LengthFormat.VarInt);
            }
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
    }
}