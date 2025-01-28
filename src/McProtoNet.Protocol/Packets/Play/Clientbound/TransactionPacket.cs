using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound
{
    [PacketInfo("Transaction", PacketState.Play, PacketDirection.Clientbound)]
    public abstract partial class TransactionPacket : IServerPacket
    {
        public sbyte WindowId { get; set; }
        public short Action { get; set; }
        public bool Accepted { get; set; }

        [PacketSubInfo(340, 754)]
        public sealed partial class V340_754 : TransactionPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                WindowId = reader.ReadSignedByte();
                Action = reader.ReadSignedShort();
                Accepted = reader.ReadBoolean();
            }
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
    }
}