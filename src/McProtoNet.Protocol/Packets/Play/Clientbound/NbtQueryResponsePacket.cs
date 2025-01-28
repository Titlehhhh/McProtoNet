using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound
{
    [PacketInfo("NbtQueryResponse", PacketState.Play, PacketDirection.Clientbound)]
    public abstract partial class NbtQueryResponsePacket : IServerPacket
    {
        public int TransactionId { get; set; }
        public NbtTag? Nbt { get; set; }

        [PacketSubInfo(393, 763)]
        public sealed partial class V393_763 : NbtQueryResponsePacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                TransactionId = reader.ReadVarInt();
                Nbt = reader.ReadOptionalNbtTag(true);
            }
        }

        [PacketSubInfo(764, 769)]
        public sealed partial class V764_769 : NbtQueryResponsePacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                TransactionId = reader.ReadVarInt();
                Nbt = reader.ReadOptionalNbtTag(false);
            }
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
    }
}