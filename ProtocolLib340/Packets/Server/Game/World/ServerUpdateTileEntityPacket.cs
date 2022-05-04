using ProtoLib.API.IO;
using ProtoLib.API.Networking;


namespace ProtocolLib340.Packets.Server
{


    public class ServerUpdateTileEntityPacket : IPacket
    {
        //this.position = NetUtil.readPosition(in);
        //this.type = MagicValues.key(UpdatedTileType.class, in.readUnsignedByte());
        //this.nbt = NetUtil.readNBT(in);
        public void Read(IMinecraftStreamReader stream)
        {

        }

        public void Write(IMinecraftStreamWriter stream)
        {

        }

        public ServerUpdateTileEntityPacket() { }
    }

}
