using McProtoNet.API.IO;
using McProtoNet.API.Networking;


namespace McProtoNet.PacketRepository340.Packets.Server
{


    public class ServerUpdateTileEntityPacket : IPacket
    {
        //this.position = NetUtil.readPosition(in);
        //this.type = MagicValues.key(UpdatedTileType.class, in.readUnsignedByte());
        //this.nbt = NetUtil.readNBT(in);
        public void Read(IMinecraftPrimitiveReader stream)
        {

        }

        public void Write(IMinecraftPrimitiveWriter stream)
        {

        }

        public ServerUpdateTileEntityPacket() { }
    }

}
