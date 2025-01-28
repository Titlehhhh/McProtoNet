using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound
{
    [PacketInfo("EntityEffect", PacketState.Play, PacketDirection.Clientbound)]
    public abstract partial class EntityEffectPacket : IServerPacket
    {
        public int EntityId { get; set; }
        public int Duration { get; set; }

        [PacketSubInfo(340, 757)]
        public sealed partial class V340_757 : EntityEffectPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                EntityId = reader.ReadVarInt();
                EffectId = reader.ReadSignedByte();
                Amplifier = reader.ReadSignedByte();
                Duration = reader.ReadVarInt();
                HideParticles = reader.ReadSignedByte();
            }

            public sbyte EffectId { get; set; }
            public sbyte Amplifier { get; set; }
            public sbyte HideParticles { get; set; }
        }

        [PacketSubInfo(758, 758)]
        public sealed partial class V758 : EntityEffectPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                EntityId = reader.ReadVarInt();
                EffectId = reader.ReadVarInt();
                Amplifier = reader.ReadSignedByte();
                Duration = reader.ReadVarInt();
                HideParticles = reader.ReadSignedByte();
            }

            public int EffectId { get; set; }
            public sbyte Amplifier { get; set; }
            public sbyte HideParticles { get; set; }
        }

        [PacketSubInfo(759, 763)]
        public sealed partial class V759_763 : EntityEffectPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                EntityId = reader.ReadVarInt();
                EffectId = reader.ReadVarInt();
                Amplifier = reader.ReadSignedByte();
                Duration = reader.ReadVarInt();
                HideParticles = reader.ReadSignedByte();
                FactorCodec = reader.ReadOptional((ref MinecraftPrimitiveReader r_0) => r_0.ReadNbtTag(true));
            }

            public int EffectId { get; set; }
            public sbyte Amplifier { get; set; }
            public sbyte HideParticles { get; set; }
            public NbtTag? FactorCodec { get; set; }
        }

        [PacketSubInfo(764, 765)]
        public sealed partial class V764_765 : EntityEffectPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                EntityId = reader.ReadVarInt();
                EffectId = reader.ReadVarInt();
                Amplifier = reader.ReadSignedByte();
                Duration = reader.ReadVarInt();
                HideParticles = reader.ReadSignedByte();
                FactorCodec = reader.ReadOptional((ref MinecraftPrimitiveReader r_0) => r_0.ReadNbtTag(false));
            }

            public int EffectId { get; set; }
            public sbyte Amplifier { get; set; }
            public sbyte HideParticles { get; set; }
            public NbtTag? FactorCodec { get; set; }
        }

        [PacketSubInfo(766, 769)]
        public sealed partial class V766_769 : EntityEffectPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                EntityId = reader.ReadVarInt();
                EffectId = reader.ReadVarInt();
                Amplifier = reader.ReadVarInt();
                Duration = reader.ReadVarInt();
                Flags = reader.ReadUnsignedByte();
            }

            public int EffectId { get; set; }
            public int Amplifier { get; set; }
            public byte Flags { get; set; }
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
    }
}