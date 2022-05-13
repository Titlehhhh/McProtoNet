namespace McProtoNet.PacketRepository340.Packets.Server
{


    public class ServerEntityCollectItemPacket : IPacket
    {
        //this.collectedEntityId = in.readVarInt();
        //this.collectorEntityId = in.readVarInt();
        //this.itemCount = in.readVarInt();
        public void Read(IMinecraftPrimitiveReader stream)
        {

        }

        public void Write(IMinecraftPrimitiveWriter stream)
        {

        }

        public ServerEntityCollectItemPacket() { }
    }

}
