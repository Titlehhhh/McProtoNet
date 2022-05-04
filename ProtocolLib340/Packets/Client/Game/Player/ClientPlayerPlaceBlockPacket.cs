using ProtoLib.API.IO;
using ProtoLib.API.Networking;


namespace ProtocolLib340.Packets.Client.Game
{


    public class ClientPlayerPlaceBlockPacket : IPacket
    {
        public void Read(IMinecraftStreamReader stream)
        {

        }

        //NetUtil.writePosition(out, this.position);
        //out.writeVarInt(MagicValues.value(Integer.class, this.face));
        //out.writeVarInt(MagicValues.value(Integer.class, this.hand));
        //out.writeFloat(this.cursorX);
        //out.writeFloat(this.cursorY);
        //out.writeFloat(this.cursorZ);
        public void Write(IMinecraftStreamWriter stream)
        {

        }
    }
}
