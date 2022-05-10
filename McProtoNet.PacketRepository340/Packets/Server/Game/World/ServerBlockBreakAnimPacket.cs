using McProtoNet.API.IO;
using McProtoNet.API.Networking;


namespace McProtoNet.PacketRepository340.Packets.Server
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
        public void Read(IMinecraftPrimitiveReader stream)
        {

        }

        public void Write(IMinecraftPrimitiveWriter stream)
        {

        }

        public ServerBlockBreakAnimPacket() { }
    }

}
