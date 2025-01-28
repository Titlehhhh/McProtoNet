using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound
{
    [PacketInfo("PingResponse", PacketState.Play, PacketDirection.Clientbound)]
    public abstract partial class PingResponsePacket : IServerPacket
    {
        public long Id { get; set; }

        [PacketSubInfo(764, 769)]
        public sealed partial class V764_769 : PingResponsePacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                Id = reader.ReadSignedLong();
            }
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
    }
}