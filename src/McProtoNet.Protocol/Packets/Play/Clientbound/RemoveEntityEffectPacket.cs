using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound
{
    [PacketInfo("RemoveEntityEffect", PacketState.Play, PacketDirection.Clientbound)]
    public abstract partial class RemoveEntityEffectPacket : IServerPacket
    {
        public int EntityId { get; set; }

        [PacketSubInfo(340, 757)]
        public sealed partial class V340_757 : RemoveEntityEffectPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                EntityId = reader.ReadVarInt();
                EffectId = reader.ReadSignedByte();
            }

            public sbyte EffectId { get; set; }
        }

        [PacketSubInfo(758, 769)]
        public sealed partial class V758_769 : RemoveEntityEffectPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                EntityId = reader.ReadVarInt();
                EffectId = reader.ReadVarInt();
            }

            public int EffectId { get; set; }
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
    }
}