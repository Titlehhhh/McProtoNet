namespace McProtoNet.PacketRepository754.Packets.Client
{

    [PacketInfo(0x00, 754, PacketCategory.Game, PacketSide.Client)]
    public class ClientTeleportConfirmPacket : IPacket
    {
        public int Id { get; private set; }

        public void Write(IMinecraftPrimitiveWriter stream)
        {
            stream.WriteVarInt(Id);
        }
        public void Read(IMinecraftPrimitiveReader stream)
        {
            Id = stream.ReadVarInt();
        }
        public ClientTeleportConfirmPacket() { }

        public ClientTeleportConfirmPacket(int Id)
        {
            this.Id = Id;
        }
    }
}
