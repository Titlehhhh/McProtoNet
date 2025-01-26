using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.ClientboundPackets.Play
{
    public abstract class WorldEventPacket : IServerPacket
    {
        public int EffectId { get; set; }
        public Position Location { get; set; }
        public int Data { get; set; }
        public bool Global { get; set; }

        internal sealed class V340_769 : WorldEventPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                EffectId = reader.ReadSignedInt();
                Location = reader.ReadPosition(protocolVersion);
                Data = reader.ReadSignedInt();
                Global = reader.ReadBoolean();
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
        public static PacketIdentifier PacketId => ServerPlayPacket.WorldEvent;

        public PacketIdentifier GetPacketId() => PacketId;
    }
}