using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound
{
    [PacketInfo("EntityDestroy", PacketState.Play, PacketDirection.Clientbound)]
    public abstract partial class EntityDestroyPacket : IServerPacket
    {
        public int[] EntityIds { get; set; }

        [PacketSubInfo(340, 754)]
        public sealed partial class V340_754 : EntityDestroyPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                EntityIds = reader.ReadArray(LengthFormat.VarInt, ReadDelegates.VarInt);
            }
        }

        [PacketSubInfo(756, 769)]
        public sealed partial class V756_769 : EntityDestroyPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                EntityIds = reader.ReadArray(LengthFormat.VarInt, ReadDelegates.VarInt);
            }
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
    }
}