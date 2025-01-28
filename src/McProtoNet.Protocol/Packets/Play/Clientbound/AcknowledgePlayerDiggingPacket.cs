using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound
{
    [PacketInfo("AcknowledgePlayerDigging", PacketState.Play, PacketDirection.Clientbound)]
    public abstract partial class AcknowledgePlayerDiggingPacket : IServerPacket
    {
        [PacketSubInfo(498, 758)]
        public sealed partial class V498_758 : AcknowledgePlayerDiggingPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                Location = reader.ReadPosition(protocolVersion);
                Block = reader.ReadVarInt();
                Status = reader.ReadVarInt();
                Successful = reader.ReadBoolean();
            }

            public Position Location { get; set; }
            public int Block { get; set; }
            public int Status { get; set; }
            public bool Successful { get; set; }
        }

        [PacketSubInfo(759, 769)]
        public sealed partial class V759_769 : AcknowledgePlayerDiggingPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                SequenceId = reader.ReadVarInt();
            }

            public int SequenceId { get; set; }
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
    }
}