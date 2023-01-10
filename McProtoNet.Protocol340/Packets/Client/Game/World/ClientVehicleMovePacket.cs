namespace McProtoNet.Protocol340.Packets.Client.Game
{


    public sealed class ClientVehicleMovePacket : MinecraftPacket<Protocol340>
    {
         

        public override void Read(IMinecraftPrimitiveReader stream)
        {

        }

        //out.writeDouble(this.x);
        //out.writeDouble(this.y);
        //out.writeDouble(this.z);
        //out.writeFloat(this.yaw);
        //out.writeFloat(this.pitch);
        public override void Write(IMinecraftPrimitiveWriter stream)
        {

        }
    }
}
