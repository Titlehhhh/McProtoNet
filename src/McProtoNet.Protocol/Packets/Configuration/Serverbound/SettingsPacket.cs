using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Configuration.Serverbound;

public class SettingsPacket : IClientPacket
{
    public string Locale { get; set; }
    public sbyte ViewDistance { get; set; }
    public int ChatFlags { get; set; }
    public bool ChatColors { get; set; }
    public byte SkinParts { get; set; }
    public int MainHand { get; set; }
    public bool EnableTextFiltering { get; set; }
    public bool EnableServerListing { get; set; }

    public sealed class V764_767 : SettingsPacket
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

        public new static bool SupportedVersion(int protocolVersion)
        {
            return protocolVersion is >= 764 and <= 767;
        }
    }

    public sealed class V768_769 : SettingsPacket
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

        public new static bool SupportedVersion(int protocolVersion)
        {
            return protocolVersion is >= 768 and <= 769;
        }
    }

    public static bool SupportedVersion(int protocolVersion)
    {
        return V764_767.SupportedVersion(protocolVersion) || V768_769.SupportedVersion(protocolVersion);
    }

    public virtual void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        if (V764_767.SupportedVersion(protocolVersion))
            V764_767.SerializeInternal(ref writer, protocolVersion, Locale, ViewDistance, ChatFlags, ChatColors,
                SkinParts, MainHand, EnableTextFiltering, EnableServerListing);
        else if (V768_769.SupportedVersion(protocolVersion))
            V768_769.SerializeInternal(ref writer, protocolVersion, Locale, ViewDistance, ChatFlags, ChatColors,
                SkinParts, MainHand, EnableTextFiltering, EnableServerListing, 0);
        else
            throw new ProtocolNotSupportException(nameof(ClientConfigurationPacket.Settings), protocolVersion);
    }

    public static PacketIdentifier PacketId => ClientConfigurationPacket.Settings;

    public PacketIdentifier GetPacketId() => PacketId;
}