using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound
{
    [PacketInfo("MessageHeader", PacketState.Play, PacketDirection.Clientbound)]
    public abstract partial class MessageHeaderPacket : IServerPacket
    {
        public byte[]? PreviousSignature { get; set; }
        public Guid SenderUuid { get; set; }
        public byte[] Signature { get; set; }
        public byte[] MessageHash { get; set; }

        [PacketSubInfo(760, 760)]
        public sealed partial class V760 : MessageHeaderPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                PreviousSignature = reader.ReadOptional((ref MinecraftPrimitiveReader r_0) => r_0.ReadBuffer(LengthFormat.VarInt));
                SenderUuid = reader.ReadUUID();
                Signature = reader.ReadBuffer(LengthFormat.VarInt);
                MessageHash = reader.ReadBuffer(LengthFormat.VarInt);
            }
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
    }
}