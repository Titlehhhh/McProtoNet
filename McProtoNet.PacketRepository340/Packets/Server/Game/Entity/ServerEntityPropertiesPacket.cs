using McProtoNet.API.IO;
using McProtoNet.API.Networking;


namespace McProtoNet.PacketRepository340.Packets.Server
{


    public class ServerEntityPropertiesPacket : IPacket
    {
        //this.entityId = in.readVarInt();
        //this.attributes = new ArrayList<Attribute>();
        //int length = in.readInt();
        //for(int index = 0; index < length; index++) {
        //String key = in.readString();
        //double value = in.readDouble();
        //List<AttributeModifier> modifiers = new ArrayList<AttributeModifier>();
        //int len = in.readVarInt();
        //for(int ind = 0; ind < len; ind++) {
        //modifiers.add(new AttributeModifier(in.readUUID(), in.readDouble(), MagicValues.key(ModifierOperation.class, in.readByte())));
        //}
        //
        //this.attributes.add(new Attribute(MagicValues.key(AttributeType.class, key), value, modifiers));
        //}
        public void Read(IMinecraftPrimitiveReader stream)
        {

        }

        public void Write(IMinecraftPrimitiveWriter stream)
        {

        }

        public ServerEntityPropertiesPacket() { }
    }

}
