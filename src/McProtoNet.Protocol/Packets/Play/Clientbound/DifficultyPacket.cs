using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound
{
    [PacketInfo("Difficulty", PacketState.Play, PacketDirection.Clientbound)]
    public abstract partial class DifficultyPacket : IServerPacket
    {
        public byte Difficulty { get; set; }

        [PacketSubInfo(340, 404)]
        public sealed partial class V340_404 : DifficultyPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                Difficulty = reader.ReadUnsignedByte();
            }
        }

        [PacketSubInfo(477, 769)]
        public sealed partial class V477_769 : DifficultyPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                Difficulty = reader.ReadUnsignedByte();
                DifficultyLocked = reader.ReadBoolean();
            }

            public bool DifficultyLocked { get; set; }
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
    }
}