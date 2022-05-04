using ProtoLib.API.IO;
using ProtoLib.API.Networking;


namespace ProtocolLib340.Packets.Server
{


    public class ServerSpawnMobPacket : IPacket
    {
        //this.entityId = in.readVarInt();
        //this.uuid = in.readUUID();
        //this.type = MagicValues.key(MobType.class, in.readVarInt());
        //this.x = in.readDouble();
        //this.y = in.readDouble();
        //this.z = in.readDouble();
        //this.yaw = in.readByte() * 360 / 256f;
        //this.pitch = in.readByte() * 360 / 256f;
        //this.headYaw = in.readByte() * 360 / 256f;
        //this.motX = in.readShort() / 8000D;
        //this.motY = in.readShort() / 8000D;
        //this.motZ = in.readShort() / 8000D;
        //this.metadata = NetUtil.readEntityMetadata(in);
        public void Read(IMinecraftStreamReader stream)
        {

        }

        public void Write(IMinecraftStreamWriter stream)
        {

        }

        public ServerSpawnMobPacket() { }
    }

}
