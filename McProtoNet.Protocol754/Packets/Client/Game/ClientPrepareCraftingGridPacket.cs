namespace McProtoNet.Protocol754.Packets.Client
{

    [PacketInfo(0x19, PacketCategory.Game, 754, PacketSide.Client)]
    public sealed class ClientPrepareCraftingGridPacket : Packet
    {
        public byte WindowId { get; private set; }
        public string RecipeId { get; private set; }
        public bool MakeAll { get; private set; }

        public override void Write(IMinecraftPrimitiveWriter stream)
        {
            stream.WriteUnsignedByte(WindowId);
            stream.WriteString(RecipeId);
            stream.WriteBoolean(MakeAll);
        }
        public override void Read(IMinecraftPrimitiveReader stream)
        {
            WindowId = stream.ReadUnsignedByte();
            RecipeId = stream.ReadString();
            MakeAll = stream.ReadBoolean();
        }
        public ClientPrepareCraftingGridPacket() { }

        public ClientPrepareCraftingGridPacket(byte WindowId, string RecipeId, bool MakeAll)
        {
            this.WindowId = WindowId;
            this.RecipeId = RecipeId;
            this.MakeAll = MakeAll;
        }
    }
}
