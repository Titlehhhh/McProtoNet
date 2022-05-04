using ProtoLib.API.IO;
using ProtoLib.API.Networking;


namespace ProtocolLib340.Packets.Server
{


    public class ServerSpawnPaintingPacket : IPacket
    {
        //this.entityId = in.readVarInt();
        //this.uuid = in.readUUID();
        //this.paintingType = MagicValues.key(PaintingType.class, in.readString());
        //this.position = NetUtil.readPosition(in);
        //this.direction = MagicValues.key(HangingDirection.class, in.readUnsignedByte());
        public void Read(IMinecraftStreamReader stream)
        {

        }

        public void Write(IMinecraftStreamWriter stream)
        {

        }

        public ServerSpawnPaintingPacket() { }
    }

}
