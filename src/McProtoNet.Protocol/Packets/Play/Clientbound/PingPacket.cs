using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound
{
    [PacketInfo("Ping", PacketState.Play, PacketDirection.Clientbound)]
    public abstract partial class PingPacket : IServerPacket
    {
        public int Id { get; set; }

        [PacketSubInfo(755, 769)]
        public sealed partial class V755_769 : PingPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                Id = reader.ReadSignedInt();
            }
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
    }
}