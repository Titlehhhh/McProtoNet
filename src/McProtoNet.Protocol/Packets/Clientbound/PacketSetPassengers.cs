using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.ClientboundPackets
{
    public abstract class SetPassengersPacket : IServerPacket
    {
        public int EntityId { get; set; }
        public int[] Passengers { get; set; }

        internal sealed class V340_769 : SetPassengersPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                EntityId = reader.ReadVarInt();
                Passengers = reader.ReadArray(LengthFormat.VarInt, ReadDelegates.VarInt);
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion is >= 340 and <= 769;
            }
        }

        public static bool SupportedVersion(int protocolVersion)
        {
            return V340_769.SupportedVersion(protocolVersion);
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
        public static ServerPacket PacketId => ServerPacket.SetPassengers;

        public ServerPacket GetPacketId() => PacketId;
    }
}