using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound
{
    [PacketInfo("UpdateViewPosition", PacketState.Play, PacketDirection.Clientbound)]
    public abstract partial class UpdateViewPositionPacket : IServerPacket
    {
        public int ChunkX { get; set; }
        public int ChunkZ { get; set; }

        [PacketSubInfo(477, 769)]
        public sealed partial class V477_769 : UpdateViewPositionPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                ChunkX = reader.ReadVarInt();
                ChunkZ = reader.ReadVarInt();
            }
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
    }
}