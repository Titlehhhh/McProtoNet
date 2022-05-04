using ProtoLib.API.IO;
using ProtoLib.API.Networking;


namespace ProtocolLib340.Packets.Server
{


    public class ServerEntityRemoveEffectPacket : IPacket
    {
        //this.entityId = in.readVarInt();
        //this.effect = MagicValues.key(Effect.class, in.readUnsignedByte());
        public void Read(IMinecraftStreamReader stream)
        {

        }

        public void Write(IMinecraftStreamWriter stream)
        {

        }

        public ServerEntityRemoveEffectPacket() { }
    }

}
