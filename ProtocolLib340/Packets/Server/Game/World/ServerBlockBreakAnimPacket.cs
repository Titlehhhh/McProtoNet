using ProtoLib.API.IO;
using ProtoLib.API.Networking;


namespace ProtocolLib340.Packets.Server
{


    public class ServerBlockBreakAnimPacket : IPacket
    {
        //this.breakerEntityId = in.readVarInt();
        //this.position = NetUtil.readPosition(in);
        //try {
        //this.stage = MagicValues.key(BlockBreakStage.class, in.readUnsignedByte());
        //} catch(IllegalArgumentException e) {
        //this.stage = BlockBreakStage.RESET;
        //}
        public void Read(IMinecraftStreamReader stream)
        {

        }

        public void Write(IMinecraftStreamWriter stream)
        {

        }

        public ServerBlockBreakAnimPacket() { }
    }

}
