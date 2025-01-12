using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.ClientboundPackets
{
    public abstract class NamedSoundEffectPacket : IServerPacket
    {
        public string SoundName { get; set; }
        public int SoundCategory { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
        public float Volume { get; set; }
        public float Pitch { get; set; }

        internal sealed class V340_758 : NamedSoundEffectPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                SoundName = reader.ReadString();
                SoundCategory = reader.ReadVarInt();
                X = reader.ReadSignedInt();
                Y = reader.ReadSignedInt();
                Z = reader.ReadSignedInt();
                Volume = reader.ReadFloat();
                Pitch = reader.ReadFloat();
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion is >= 340 and <= 758;
            }
        }

        internal sealed class V759_760 : NamedSoundEffectPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                SoundName = reader.ReadString();
                SoundCategory = reader.ReadVarInt();
                X = reader.ReadSignedInt();
                Y = reader.ReadSignedInt();
                Z = reader.ReadSignedInt();
                Volume = reader.ReadFloat();
                Pitch = reader.ReadFloat();
                Seed = reader.ReadSignedLong();
            }

            public new static bool SupportedVersion(int protocolVersion)
            {
                return protocolVersion is >= 759 and <= 760;
            }

            public long Seed { get; set; }
        }

        public static bool SupportedVersion(int protocolVersion)
        {
            return V340_758.SupportedVersion(protocolVersion) || V759_760.SupportedVersion(protocolVersion);
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
        public static ServerPacket PacketId => ServerPacket.NamedSoundEffect;

        public ServerPacket GetPacketId() => PacketId;
    }
}