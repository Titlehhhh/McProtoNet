using McProtoNet.API.IO;
using McProtoNet.API.Networking;


namespace McProtoNet.PacketRepository340.Packets.Client.Game
{


    public class ClientPlayerPlaceBlockPacket : IPacket
    {
        public void Read(IMinecraftPrimitiveReader stream)
        {

        }

        //NetUtil.writePosition(out, this.position);
        //out.writeVarInt(MagicValues.value(Integer.class, this.face));
        //out.writeVarInt(MagicValues.value(Integer.class, this.hand));
        //out.writeFloat(this.cursorX);
        //out.writeFloat(this.cursorY);
        //out.writeFloat(this.cursorZ);
        public void Write(IMinecraftPrimitiveWriter stream)
        {

        }
    }
}
