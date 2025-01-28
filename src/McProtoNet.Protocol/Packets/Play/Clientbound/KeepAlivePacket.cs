using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound
{
    [PacketInfo("KeepAlive", PacketState.Play, PacketDirection.Clientbound)]
    public abstract partial class KeepAlivePacket : IServerPacket
    {
        public long KeepAliveId { get; set; }

        [PacketSubInfo(340, 769)]
        internal sealed partial class V340_769 : KeepAlivePacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                KeepAliveId = reader.ReadSignedLong();
            }
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
    }
}