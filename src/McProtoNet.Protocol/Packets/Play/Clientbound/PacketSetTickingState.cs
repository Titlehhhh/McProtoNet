using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.ClientboundPackets.Play
{
    public abstract class SetTickingStatePacket : IServerPacket
    {
        public float TickRate { get; set; }
        public bool IsFrozen { get; set; }

        public sealed class V765_769 : SetTickingStatePacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                TickRate = reader.ReadFloat();
                IsFrozen = reader.ReadBoolean();
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion is >= 765 and <= 769;
            }
        }

        public static bool SupportedVersion(int protocolVersion)
        {
            return V765_769.SupportedVersion(protocolVersion);
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
        public static PacketIdentifier PacketId => ServerPlayPacket.SetTickingState;

        public PacketIdentifier GetPacketId() => PacketId;
    }
}