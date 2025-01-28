using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound
{
    [PacketInfo("UpdateViewDistance", PacketState.Play, PacketDirection.Clientbound)]
    public abstract partial class UpdateViewDistancePacket : IServerPacket
    {
        public int ViewDistance { get; set; }

        [PacketSubInfo(477, 769)]
        public sealed partial class V477_769 : UpdateViewDistancePacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                ViewDistance = reader.ReadVarInt();
            }
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
    }
}