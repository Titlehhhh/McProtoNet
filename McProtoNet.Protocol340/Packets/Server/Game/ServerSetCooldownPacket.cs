namespace McProtoNet.Protocol340.Packets.Server
{


    public sealed class ServerSetCooldownPacket : MinecraftPacket<Protocol340>
    {
         

        //this.itemId = in.readVarInt();
        //this.cooldownTicks = in.readVarInt();
        public override void Read(IMinecraftPrimitiveReader stream)
        {

        }

        public override void Write(IMinecraftPrimitiveWriter stream)
        {

        }

        public ServerSetCooldownPacket() { }
    }

}
