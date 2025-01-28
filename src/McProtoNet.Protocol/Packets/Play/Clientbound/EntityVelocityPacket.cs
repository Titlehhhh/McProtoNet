using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound
{
    [PacketInfo("EntityVelocity", PacketState.Play, PacketDirection.Clientbound)]
    public abstract partial class EntityVelocityPacket : IServerPacket
    {
        public int EntityId { get; set; }
        public short VelocityX { get; set; }
        public short VelocityY { get; set; }
        public short VelocityZ { get; set; }

        [PacketSubInfo(340, 769)]
        internal sealed partial class V340_769 : EntityVelocityPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                EntityId = reader.ReadVarInt();
                VelocityX = reader.ReadSignedShort();
                VelocityY = reader.ReadSignedShort();
                VelocityZ = reader.ReadSignedShort();
            }
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
    }
}