using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.ClientboundPackets.Play
{
    public abstract class EnterCombatEventPacket : IServerPacket
    {
        public sealed class V755_769 : EnterCombatEventPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion is >= 755 and <= 769;
            }
        }

        public static bool SupportedVersion(int protocolVersion)
        {
            return V755_769.SupportedVersion(protocolVersion);
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
        public static PacketIdentifier PacketId => ServerPlayPacket.EnterCombatEvent;

        public PacketIdentifier GetPacketId() => PacketId;
    }
}