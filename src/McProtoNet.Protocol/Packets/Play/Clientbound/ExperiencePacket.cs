using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound
{
    [PacketInfo("Experience", PacketState.Play, PacketDirection.Clientbound)]
    public abstract partial class ExperiencePacket : IServerPacket
    {
        public float ExperienceBar { get; set; }
        public int Level { get; set; }
        public int TotalExperience { get; set; }

        [PacketSubInfo(340, 760)]
        internal sealed partial class V340_760 : ExperiencePacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                ExperienceBar = reader.ReadFloat();
                Level = reader.ReadVarInt();
                TotalExperience = reader.ReadVarInt();
            }
        }

        [PacketSubInfo(761, 763)]
        internal sealed partial class V761_763 : ExperiencePacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                ExperienceBar = reader.ReadFloat();
                TotalExperience = reader.ReadVarInt();
                Level = reader.ReadVarInt();
            }
        }

        [PacketSubInfo(764, 769)]
        internal sealed partial class V764_769 : ExperiencePacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                ExperienceBar = reader.ReadFloat();
                Level = reader.ReadVarInt();
                TotalExperience = reader.ReadVarInt();
            }
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
    }
}