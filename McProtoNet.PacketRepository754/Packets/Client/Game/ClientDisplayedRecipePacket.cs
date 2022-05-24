namespace McProtoNet.PacketRepository754.Packets.Client
{

    [PacketInfo(0x1F, 754, PacketSide.Client)]
    public sealed class ClientDisplayedRecipePacket : Packet
    {
        public string RecipeId { get; private set; }

        public override void Write(IMinecraftPrimitiveWriter stream)
        {
            stream.WriteString(RecipeId);
        }
        public override void Read(IMinecraftPrimitiveReader stream)
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
