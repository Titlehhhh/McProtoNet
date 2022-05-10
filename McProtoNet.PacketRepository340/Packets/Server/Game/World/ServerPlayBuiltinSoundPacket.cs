using McProtoNet.API.IO;
using McProtoNet.API.Networking;


namespace McProtoNet.PacketRepository340.Packets.Server
{


    public class ServerPlayBuiltinSoundPacket : IPacket
    {
        //this.sound = MagicValues.key(BuiltinSound.class, in.readVarInt());
        //this.category = MagicValues.key(SoundCategory.class, in.readVarInt());
        //this.x = in.readInt() / 8D;
        //this.y = in.readInt() / 8D;
        //this.z = in.readInt() / 8D;
        //this.volume = in.readFloat();
        //this.pitch = in.readFloat();
        public void Read(IMinecraftPrimitiveReader stream)
        {

        }

        public void Write(IMinecraftPrimitiveWriter stream)
        {

        }

        public ServerPlayBuiltinSoundPacket() { }
    }

}
