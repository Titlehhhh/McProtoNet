namespace McProtoNet.Protocol340.Packets.Server
{


    public sealed class ServerEntityVelocityPacket : Packet
    {
        public int EntityId { get; private set; }
        public double MotionX { get; private set; }
        public double MotionY { get; private set; }
        public double MotionZ { get; private set; }
        
        public override void Read(IMinecraftPrimitiveReader stream)
        {
            EntityId = stream.ReadVarInt();
            MotionX = stream.ReadShort() / 8000D;
            MotionY = stream.ReadShort() / 8000D;
            MotionZ = stream.ReadShort() / 8000D;
        }

        public override void Write(IMinecraftPrimitiveWriter stream)
        {

        }

        public ServerEntityVelocityPacket() { }
    }

}
