using McProtoNet.API;
using McProtoNet.API.IO;
using McProtoNet.API.Networking;
using McProtoNet.API.Protocol;


namespace McProtoNet.PacketRepository754.Packets.Client
{

    [PacketInfo(0x1F, 754, PacketCategory.Game, PacketSide.Client)]
    public class ClientDisplayedRecipePacket : IPacket
    {
        public string RecipeId { get; private set; }

        public void Write(IMinecraftStreamWriter stream)
        {
            stream.WriteString(RecipeId);
        }
        public void Read(IMinecraftStreamReader stream)
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
