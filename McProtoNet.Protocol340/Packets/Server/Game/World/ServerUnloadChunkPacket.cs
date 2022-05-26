namespace McProtoNet.Protocol340.Packets.Server
{


    public sealed class ServerUnloadChunkPacket : Packet
    {
        //this.x = in.readInt();
        //this.z = in.readInt();
        public override void Read(IMinecraftPrimitiveReader stream)
        {

        }

        public override void Write(IMinecraftPrimitiveWriter stream)
        {

        }

        public ServerUnloadChunkPacket() { }
    }

}
