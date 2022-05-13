namespace McProtoNet.PacketRepository754.Packets.Client
{

    [PacketInfo(0x1F, 754, PacketCategory.Game, PacketSide.Client)]
    public class ClientDisplayedRecipePacket : IPacket
    {
        public string RecipeId { get; private set; }

        public void Write(IMinecraftPrimitiveWriter stream)
        {
            stream.WriteString(RecipeId);
        }
        public void Read(IMinecraftPrimitiveReader stream)
        {
            RecipeId = stream.ReadString();
        }
        public ClientDisplayedRecipePacket() { }

        public ClientDisplayedRecipePacket(string RecipeId)
        {
            this.RecipeId = RecipeId;
        }
    }
}
