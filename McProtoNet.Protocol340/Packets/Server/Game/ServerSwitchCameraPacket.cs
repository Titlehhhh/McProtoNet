namespace McProtoNet.Protocol340.Packets.Server
{


    public sealed class ServerSwitchCameraPacket : Packet<Protocol340>
    {
        //this.cameraEntityId = in.readVarInt();
        public override void Read(IMinecraftPrimitiveReader stream)
        {

        }

        public override void Write(IMinecraftPrimitiveWriter stream)
        {

        }

        public ServerSwitchCameraPacket() { }
    }

}
