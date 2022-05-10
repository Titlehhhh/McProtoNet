using McProtoNet.API.IO;
using McProtoNet.API.Networking;


namespace McProtoNet.PacketRepository340.Packets.Client.Game
{


    public class ClientTabCompletePacket : IPacket
    {
        public void Read(IMinecraftPrimitiveReader stream)
        {

        }

        //out.writeString(this.text);
        //out.WriteBooleanean(this.assumeCommand);
        //out.WriteBooleanean(this.lookingAt != null);
        //if(this.lookingAt != null) {
        //NetUtil.writePosition(out, this.lookingAt);
        //}
        public void Write(IMinecraftPrimitiveWriter stream)
        {

        }
    }
}
