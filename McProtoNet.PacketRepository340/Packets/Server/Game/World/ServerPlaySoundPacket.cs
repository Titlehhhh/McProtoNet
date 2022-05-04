using McProtoNet.API.IO;
using McProtoNet.API.Networking;


namespace McProtoNet.PacketRepository340.Packets.Server
{


    public class ServerPlaySoundPacket : IPacket
    {
        //String value = in.readString();
        //try {
        //this.sound = MagicValues.key(BuiltinSound.class, value);
        //} catch(IllegalArgumentException e) {
        //this.sound = new CustomSound(value);
        //}
        //
        //this.category = MagicValues.key(SoundCategory.class, in.readVarInt());
        //this.x = in.readInt() / 8D;
        //this.y = in.readInt() / 8D;
        //this.z = in.readInt() / 8D;
        //this.volume = in.readFloat();
        //this.pitch = in.readFloat();
        public void Read(IMinecraftStreamReader stream)
        {

        }

        public void Write(IMinecraftStreamWriter stream)
        {

        }

        public ServerPlaySoundPacket() { }
    }

}
