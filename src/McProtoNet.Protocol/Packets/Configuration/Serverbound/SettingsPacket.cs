using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Configuration.Serverbound;

[PacketInfo("Settings",PacketState.Configuration,PacketDirection.Serverbound)]
public partial class SettingsPacket : IClientPacket
{
    public string Locale { get; set; }
    public sbyte ViewDistance { get; set; }
    public int ChatFlags { get; set; }
    public bool ChatColors { get; set; }
    public byte SkinParts { get; set; }
    public int MainHand { get; set; }
    public bool EnableTextFiltering { get; set; }
    public bool EnableServerListing { get; set; }

    [PacketSubInfo(764,767)]
    public sealed partial class V764_767 : SettingsPacket
    {
        public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        {
            SerializeInternal(ref writer, protocolVersion, Locale, ViewDistance, ChatFlags, ChatColors, SkinParts,
                MainHand, EnableTextFiltering, EnableServerListing);
        }

        internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion, string locale,
            sbyte viewDistance, int chatFlags, bool chatColors, byte skinParts, int mainHand, bool enableTextFiltering,
            bool enableServerListing)
        {
            writer.WriteString(locale);
            writer.WriteSignedByte(viewDistance);
            writer.WriteVarInt(chatFlags);
            writer.WriteBoolean(chatColors);
            writer.WriteUnsignedByte(skinParts);
            writer.WriteVarInt(mainHand);
            writer.WriteBoolean(enableTextFiltering);
            writer.WriteBoolean(enableServerListing);
        }

       
    }
[PacketSubInfo(768,769)]
    public sealed partial class V768_769 : SettingsPacket
    {
        public int Particles { get; set; }

        public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        {
            SerializeInternal(ref writer, protocolVersion, Locale, ViewDistance, ChatFlags, ChatColors, SkinParts,
                MainHand, EnableTextFiltering, EnableServerListing, Particles);
        }

        internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion, string locale,
            sbyte viewDistance, int chatFlags, bool chatColors, byte skinParts, int mainHand, bool enableTextFiltering,
            bool enableServerListing, int particles)
        {
            writer.WriteString(locale);
            writer.WriteSignedByte(viewDistance);
            writer.WriteVarInt(chatFlags);
            writer.WriteBoolean(chatColors);
            writer.WriteUnsignedByte(skinParts);
            writer.WriteVarInt(mainHand);
            writer.WriteBoolean(enableTextFiltering);
            writer.WriteBoolean(enableServerListing);
            writer.WriteVarInt(particles);
        }

        
    }

    

    public virtual void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        if (V764_767.IsSupportedVersionStatic(protocolVersion))
            V764_767.SerializeInternal(ref writer, protocolVersion, Locale, ViewDistance, ChatFlags, ChatColors,
                SkinParts, MainHand, EnableTextFiltering, EnableServerListing);
        else if (V768_769.IsSupportedVersionStatic(protocolVersion))
            V768_769.SerializeInternal(ref writer, protocolVersion, Locale, ViewDistance, ChatFlags, ChatColors,
                SkinParts, MainHand, EnableTextFiltering, EnableServerListing, 0);
        else
            throw new ProtocolNotSupportException(nameof(ClientConfigurationPacket.Settings), protocolVersion);
    }

}