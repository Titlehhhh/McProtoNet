using McProtoNet.API.IO;
using McProtoNet.API.Networking;


namespace McProtoNet.PacketRepository340.Packets.Server
{


    public class ServerEntityEffectPacket : IPacket
    {
        //this.entityId = in.readVarInt();
        //this.effect = MagicValues.key(Effect.class, in.readByte());
        //this.amplifier = in.readByte();
        //this.duration = in.readVarInt();
        //
        //int flags = in.readByte();
        //this.ambient = (flags & 0x1) == 0x1;
        //this.showParticles = (flags & 0x2) == 0x2;
        public void Read(IMinecraftStreamReader stream)
        {

        }

        public void Write(IMinecraftStreamWriter stream)
        {

        }

        public ServerEntityEffectPacket() { }
    }

}
