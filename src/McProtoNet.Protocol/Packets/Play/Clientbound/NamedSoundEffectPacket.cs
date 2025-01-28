using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound
{
    [PacketInfo("NamedSoundEffect", PacketState.Play, PacketDirection.Clientbound)]
    public abstract partial class NamedSoundEffectPacket : IServerPacket
    {
        public string SoundName { get; set; }
        public int SoundCategory { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
        public float Volume { get; set; }
        public float Pitch { get; set; }

        [PacketSubInfo(340, 758)]
        public sealed partial class V340_758 : NamedSoundEffectPacket
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
        }

        [PacketSubInfo(759, 760)]
        public sealed partial class V759_760 : NamedSoundEffectPacket
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

            public long Seed { get; set; }
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
    }
}