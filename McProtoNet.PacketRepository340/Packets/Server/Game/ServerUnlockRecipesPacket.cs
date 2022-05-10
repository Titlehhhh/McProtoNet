using McProtoNet.API.IO;
using McProtoNet.API.Networking;


namespace McProtoNet.PacketRepository340.Packets.Server
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
        public void Read(IMinecraftPrimitiveReader stream)
        {

        }

        public void Write(IMinecraftPrimitiveWriter stream)
        {

        }

        public ServerUnlockRecipesPacket() { }
    }

}
