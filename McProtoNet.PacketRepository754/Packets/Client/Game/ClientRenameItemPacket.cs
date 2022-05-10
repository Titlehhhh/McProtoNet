using McProtoNet.API;
using McProtoNet.API.IO;
using McProtoNet.API.Networking;
using McProtoNet.API.Protocol;


namespace McProtoNet.PacketRepository754.Packets.Client
{

    [PacketInfo(0x20, 754, PacketCategory.Game, PacketSide.Client)]
    public class ClientRenameItemPacket : IPacket
    {
        public string Name { get; private set; }

        public void Write(IMinecraftPrimitiveWriter stream)
        {
            stream.WriteString(Name);
        }
        public void Read(IMinecraftPrimitiveReader stream)
        {
            Name = stream.ReadString();
        }
        public ClientRenameItemPacket() { }

        public ClientRenameItemPacket(string Name)
        {
            this.Name = Name;
        }
    }
}
