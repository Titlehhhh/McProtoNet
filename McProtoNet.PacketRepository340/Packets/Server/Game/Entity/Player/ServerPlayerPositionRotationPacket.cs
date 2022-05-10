using McProtoNet.API.IO;
using McProtoNet.API.Networking;


namespace McProtoNet.PacketRepository340.Packets.Server
{


    public class ServerPlayerPositionRotationPacket : IPacket
    {
        //this.x = in.readDouble();
        //this.y = in.readDouble();
        //this.z = in.readDouble();
        //this.yaw = in.readFloat();
        //this.pitch = in.readFloat();
        //this.relative = new ArrayList<PositionElement>();
        //int flags = in.readUnsignedByte();
        //for(PositionElement element : PositionElement.values()) {
        //int bit = 1 << MagicValues.value(Integer.class, element);
        //if((flags & bit) == bit) {
        //this.relative.add(element);
        //}
        //}
        //
        //this.teleportId = in.readVarInt();
        public void Read(IMinecraftPrimitiveReader stream)
        {

        }

        public void Write(IMinecraftPrimitiveWriter stream)
        {

        }

        public ServerPlayerPositionRotationPacket() { }
    }

}
