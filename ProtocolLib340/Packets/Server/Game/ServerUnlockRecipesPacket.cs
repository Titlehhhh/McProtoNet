using ProtoLib.API.IO;
using ProtoLib.API.Networking;


namespace ProtocolLib340.Packets.Server
{


    public class ServerUnlockRecipesPacket : IPacket
    {
        //this.action = MagicValues.key(UnlockRecipesAction.class, in.readVarInt());
        //
        //this.openCraftingBook = in.readBoolean();
        //this.activateFiltering = in.readBoolean();
        //
        //if(this.action == UnlockRecipesAction.INIT) {
        //int size = in.readVarInt();
        //this.alreadyKnownRecipes = new ArrayList<>(size);
        //for(int i = 0; i < size; i++) {
        //this.alreadyKnownRecipes.add(in.readVarInt());
        //}
        //}
        //
        //int size = in.readVarInt();
        //this.recipes = new ArrayList<>(size);
        //for(int i = 0; i < size; i++) {
        //this.recipes.add(in.readVarInt());
        //}
        public void Read(IMinecraftStreamReader stream)
        {

        }

        public void Write(IMinecraftStreamWriter stream)
        {

        }

        public ServerUnlockRecipesPacket() { }
    }

}
