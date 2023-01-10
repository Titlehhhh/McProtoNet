namespace McProtoNet.Protocol340.Packets.Server
{


    public sealed class ServerEntityCollectItemPacket : MinecraftPacket<Protocol340>
    {
        

        //this.collectedEntityId = in.readVarInt();
        //this.collectorEntityId = in.readVarInt();
        //this.itemCount = in.readVarInt();
        public override void Read(IMinecraftPrimitiveReader stream)
        {

        }

        public override void Write(IMinecraftPrimitiveWriter stream)
        {

        }

        public ServerEntityCollectItemPacket() { }
    }

}
