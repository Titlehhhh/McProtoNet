using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.ClientboundPackets
{
    public abstract class GameStateChangePacket : IServerPacket
    {
        public byte Reason { get; set; }
        public float GameMode { get; set; }

        internal sealed class V340_769 : GameStateChangePacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                Reason = reader.ReadUnsignedByte();
                GameMode = reader.ReadFloat();
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
        public static ServerPacket PacketId => ServerPacket.GameStateChange;

        public ServerPacket GetPacketId() => PacketId;
    }
}