using ProtoLib.API;
using ProtoLib.API.IO;
using ProtoLib.API.Networking;
using ProtoLib.API.Protocol;


namespace ProtocolLib754.Packets.Client
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
