using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.ClientboundPackets.Play
{
    public abstract class AbilitiesPacket : IServerPacket
    {
        public sbyte Flags { get; set; }
        public float FlyingSpeed { get; set; }
        public float WalkingSpeed { get; set; }

        internal sealed class V340_769 : AbilitiesPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                Flags = reader.ReadSignedByte();
                FlyingSpeed = reader.ReadFloat();
                WalkingSpeed = reader.ReadFloat();
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
        public static PacketIdentifier PacketId => ServerPlayPacket.Abilities;

        public PacketIdentifier GetPacketId() => PacketId;
    }
}