namespace McProtoNet.PacketRepository340.Packets.Client.Game
{


    public class ClientTeleportConfirmPacket : IPacket
    {
        public int ID { get; set; }

        public void Write(IMinecraftPrimitiveWriter stream)
        {
            stream.WriteVarInt(ID);
        }

        public void Read(IMinecraftPrimitiveReader stream)
        {

        }

        public ClientTeleportConfirmPacket(int iD)
        {
            ID = iD;
        }
    }
}
