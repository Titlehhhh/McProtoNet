namespace McProtoNet.Protocol754.Packets.Client
{

    [PacketInfo(0x20, PacketCategory.Game, 754, PacketSide.Client)]
    public sealed class ClientRenameItemPacket : Packet
    {
        public string Name { get; private set; }

        public override void Write(IMinecraftPrimitiveWriter stream)
        {
            stream.WriteString(Name);
        }
        public override void Read(IMinecraftPrimitiveReader stream)
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
