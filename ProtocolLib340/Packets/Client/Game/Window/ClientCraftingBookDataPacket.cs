using ProtoLib.API.IO;
using ProtoLib.API.Networking;


namespace ProtocolLib340.Packets.Client.Game
{


    public class ClientCraftingBookDataPacket : IPacket
    {
        public void Read(IMinecraftStreamReader stream)
        {

        }

        //out.writeVarInt(MagicValues.value(Integer.class, this.type));
        //switch(this.type) {
        //case DISPLAYED_RECIPE:
        //out.writeInt(this.recipeId);
        //break;
        //case CRAFTING_BOOK_STATUS:
        //out.WriteBooleanean(this.craftingBookOpen);
        //out.WriteBooleanean(this.filterActive);
        //break;
        //default:
        //throw new IOException("Unknown crafting book data type: " + this.type);
        //}
        public void Write(IMinecraftStreamWriter stream)
        {

        }
    }
}
