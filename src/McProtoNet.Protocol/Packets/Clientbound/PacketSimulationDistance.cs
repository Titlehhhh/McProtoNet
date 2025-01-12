using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.ClientboundPackets
{
    public abstract class SimulationDistancePacket : IServerPacket
    {
        public int Distance { get; set; }

        public sealed class V757_769 : SimulationDistancePacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                Distance = reader.ReadVarInt();
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion is >= 757 and <= 769;
            }
        }

        public static bool SupportedVersion(int protocolVersion)
        {
            return V757_769.SupportedVersion(protocolVersion);
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
        public static ServerPacket PacketId => ServerPacket.SimulationDistance;

        public ServerPacket GetPacketId() => PacketId;
    }
}