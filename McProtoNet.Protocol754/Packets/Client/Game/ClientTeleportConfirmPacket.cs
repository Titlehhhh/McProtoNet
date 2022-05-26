namespace McProtoNet.Protocol754.Packets.Client
{

    [PacketInfo(0x00, PacketCategory.Game, 754, PacketSide.Client)]
    public sealed class ClientTeleportConfirmPacket : Packet
    {
        public int Id { get; private set; }

        public override void Write(IMinecraftPrimitiveWriter stream)
        {
            stream.WriteVarInt(Id);
        }
        public override void Read(IMinecraftPrimitiveReader stream)
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
