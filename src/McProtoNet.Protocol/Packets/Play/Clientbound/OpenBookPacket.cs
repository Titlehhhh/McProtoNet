using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound
{
    [PacketInfo("OpenBook", PacketState.Play, PacketDirection.Clientbound)]
    public abstract partial class OpenBookPacket : IServerPacket
    {
        public int Hand { get; set; }

        [PacketSubInfo(477, 769)]
        public sealed partial class V477_769 : OpenBookPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                Hand = reader.ReadVarInt();
            }
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
    }
}