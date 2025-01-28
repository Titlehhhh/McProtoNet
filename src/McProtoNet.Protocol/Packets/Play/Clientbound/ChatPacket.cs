using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound
{
    [PacketInfo("Chat", PacketState.Play, PacketDirection.Clientbound)]
    public abstract partial class ChatPacket : IServerPacket
    {
        public string Message { get; set; }
        public sbyte Position { get; set; }

        [PacketSubInfo(340, 710)]
        public sealed partial class V340_710 : ChatPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                Message = reader.ReadString();
                Position = reader.ReadSignedByte();
            }
        }

        [PacketSubInfo(734, 758)]
        public sealed partial class V734_758 : ChatPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                Message = reader.ReadString();
                Position = reader.ReadSignedByte();
                Sender = reader.ReadUUID();
            }

            public Guid Sender { get; set; }
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
    }
}