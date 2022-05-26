namespace McProtoNet.Protocol340.Packets.Client.Game
{


    public sealed class ClientSettingsPacket : Packet
    {
        public override void Read(IMinecraftPrimitiveReader stream)
        {

        }

        //out.writeString(this.locale);
        //out.writeByte(this.renderDistance);
        //out.writeVarInt(MagicValues.value(Integer.class, this.chatVisibility));
        //out.WriteBooleanean(this.chatColors);
        //
        //int flags = 0;
        //for(SkinPart part : this.visibleParts) {
        //flags |= 1 << part.ordinal();
        //}
        //
        //out.writeByte(flags);
        //
        //out.writeVarInt(MagicValues.value(Integer.class, this.mainHand));
        public override void Write(IMinecraftPrimitiveWriter stream)
        {

        }
    }
}
