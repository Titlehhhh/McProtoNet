using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound
{
    [PacketInfo("DestroyEntity", PacketState.Play, PacketDirection.Clientbound)]
    public abstract partial class DestroyEntityPacket : IServerPacket
    {
        public int EntityId { get; set; }

        [PacketSubInfo(755, 755)]
        public sealed partial class V755 : DestroyEntityPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                EntityId = reader.ReadVarInt();
            }
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
    }
}