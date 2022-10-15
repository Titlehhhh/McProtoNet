namespace McProtoNet.Protocol754.Packets.Server
{

    [PacketInfo(0x49, PacketCategory.Game, 754, PacketSide.Server)]
    public sealed class ServerPlayerHealthPacket : Packet
    {
        public float Health {get ; private set;}
        public int Food {get; private set;}
        public float Saturation {get;private set;}
        public override void Write(IMinecraftPrimitiveWriter stream)
        {
            
        }
        public override void Read(IMinecraftPrimitiveReader stream)
        {
            Health = stream.ReadFloat();
            Food = stream.ReadVarInt();
            Saturation = stream.ReadFloat();
        }
        public ServerPlayerHealthPacket() { }
    }
}

