using McProtoNet.Protocol340.Util;

namespace McProtoNet.Protocol340.Packets.Server
{


    public sealed class ServerSpawnPositionPacket : Packet
    {
        public Point3_Int Position { get; private set; }
        //this.position = NetUtil.readPosition(in);
        public override void Read(IMinecraftPrimitiveReader stream)
        {
            Position = stream.ReadPoint3_Int();
        }

        public override void Write(IMinecraftPrimitiveWriter stream)
        {

        }

        public ServerSpawnPositionPacket() { }
    }

}
