namespace McProtoNet.Protocol754.Packets.Server
{

    [PacketInfo(0x29, PacketCategory.Game, 754, PacketSide.Server)]
    public sealed class ServerEntityRotationPacket : Packet
    {
        public int EntityId { get; private set; }

        public float Yaw { get; private set; }
        public float Pitch { get; private set; }
        public bool OnGround { get; private set; }
        public override void Write(IMinecraftPrimitiveWriter stream)
        {

        }
        public override void Read(IMinecraftPrimitiveReader stream)
        {
            EntityId = stream.ReadVarInt();
            Yaw = stream.ReadSignedByte() * 360f / 256f;
            Pitch = stream.ReadSignedByte() * 360f / 256f;
            OnGround = stream.ReadBoolean();
        }
        public ServerEntityRotationPacket() { }
    }
}

