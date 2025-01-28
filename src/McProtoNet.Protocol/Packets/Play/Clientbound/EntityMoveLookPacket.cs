using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound
{
    [PacketInfo("EntityMoveLook", PacketState.Play, PacketDirection.Clientbound)]
    public abstract partial class EntityMoveLookPacket : IServerPacket
    {
        public int EntityId { get; set; }
        public short DX { get; set; }
        public short DY { get; set; }
        public short DZ { get; set; }
        public sbyte Yaw { get; set; }
        public sbyte Pitch { get; set; }
        public bool OnGround { get; set; }

        [PacketSubInfo(340, 769)]
        internal sealed partial class V340_769 : EntityMoveLookPacket
        {
            public override void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
            {
                EntityId = reader.ReadVarInt();
                DX = reader.ReadSignedShort();
                DY = reader.ReadSignedShort();
                DZ = reader.ReadSignedShort();
                Yaw = reader.ReadSignedByte();
                Pitch = reader.ReadSignedByte();
                OnGround = reader.ReadBoolean();
            }
        }

        public abstract void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion);
    }
}