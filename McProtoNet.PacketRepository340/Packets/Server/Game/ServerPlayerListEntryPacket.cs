using McProtoNet.API.IO;
using McProtoNet.API.Networking;


namespace McProtoNet.PacketRepository340.Packets.Server
{


    public class ServerPlayerListEntryPacket : IPacket
    {
        //this.action = MagicValues.key(PlayerListEntryAction.class, in.readVarInt());
        //this.entries = new PlayerListEntry[in.readVarInt()];
        //for(int count = 0; count < this.entries.length; count++) {
        //UUID uuid = in.readUUID();
        //GameProfile profile;
        //if(this.action == PlayerListEntryAction.ADD_PLAYER) {
        //profile = new GameProfile(uuid, in.readString());
        //} else {
        //profile = new GameProfile(uuid, null);
        //}
        //
        //PlayerListEntry entry = null;
        //switch(this.action) {
        //case ADD_PLAYER:
        //int properties = in.readVarInt();
        //for(int index = 0; index < properties; index++) {
        //String propertyName = in.readString();
        //String value = in.readString();
        //String signature = null;
        //if(in.readBoolean()) {
        //signature = in.readString();
        //}
        //
        //profile.getProperties().add(new GameProfile.Property(propertyName, value, signature));
        //}
        //
        //int g = in.readVarInt();
        //GameMode gameMode = MagicValues.key(GameMode.class, g < 0 ? 0 : g);
        //int ping = in.readVarInt();
        //Message displayName = null;
        //if(in.readBoolean()) {
        //displayName = Message.fromString(in.readString());
        //}
        //
        //entry = new PlayerListEntry(profile, gameMode, ping, displayName);
        //break;
        //case UPDATE_GAMEMODE:
        //g = in.readVarInt();
        //GameMode mode = MagicValues.key(GameMode.class, g < 0 ? 0 : g);
        //entry = new PlayerListEntry(profile, mode);
        //break;
        //case UPDATE_LATENCY:
        //int png = in.readVarInt();
        //entry = new PlayerListEntry(profile, png);
        //break;
        //case UPDATE_DISPLAY_NAME:
        //Message disp = null;
        //if(in.readBoolean()) {
        //disp = Message.fromString(in.readString());
        //}
        //
        //entry = new PlayerListEntry(profile, disp);
        //case REMOVE_PLAYER:
        //entry = new PlayerListEntry(profile);
        //break;
        //}
        //
        //this.entries[count] = entry;
        //}
        public void Read(IMinecraftStreamReader stream)
        {

        }

        public void Write(IMinecraftStreamWriter stream)
        {

        }

        public ServerPlayerListEntryPacket() { }
    }

}
