using McProtoNet.API.IO;
using McProtoNet.API.Networking;


namespace McProtoNet.PacketRepository340.Packets.Server
{


    public class ServerPreparedCraftingGridPacket : IPacket
    {
        //this.windowId = in.readByte();
        //this.recipeId = in.readVarInt();
        public void Read(IMinecraftPrimitiveReader stream)
        {

        }

        public void Write(IMinecraftPrimitiveWriter stream)
        {

        }

        public ServerPreparedCraftingGridPacket() { }
    }

}
