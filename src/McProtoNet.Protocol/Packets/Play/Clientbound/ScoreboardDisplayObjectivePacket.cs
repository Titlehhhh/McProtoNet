using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound
{
    [PacketInfo("ScoreboardDisplayObjective", PacketState.Play, PacketDirection.Clientbound)]
    public abstract partial class ScoreboardDisplayObjectivePacket : IServerPacket
    {
        public string Name { get; set; }

        [PacketSubInfo(340, 763)]
        public sealed partial class V340_763 : ScoreboardDisplayObjectivePacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                Position = reader.ReadSignedByte();
                Name = reader.ReadString();
            }

            public sbyte Position { get; set; }
        }

        [PacketSubInfo(764, 769)]
        public sealed partial class V764_769 : ScoreboardDisplayObjectivePacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                Position = reader.ReadVarInt();
                Name = reader.ReadString();
            }

            public int Position { get; set; }
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
    }
}