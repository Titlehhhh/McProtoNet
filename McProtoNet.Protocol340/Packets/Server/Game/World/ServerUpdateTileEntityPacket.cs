namespace McProtoNet.Protocol340.Packets.Server
{


    public sealed class ServerUpdateTileEntityPacket : MinecraftPacket<Protocol340>
    {
         

        //this.position = NetUtil.readPosition(in);
        //this.type = MagicValues.key(UpdatedTileType.class, in.readUnsignedByte());
        //this.nbt = NetUtil.readNBT(in);
        public override void Read(IMinecraftPrimitiveReader stream)
        {

        }

        public override void Write(IMinecraftPrimitiveWriter stream)
        {

        }

        public ServerUpdateTileEntityPacket() { }
    }

}
