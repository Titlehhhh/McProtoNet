using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;

[PacketInfo("Settings", PacketState.Play, PacketDirection.Serverbound)]
public partial class SettingsPacket : IClientPacket
{
    public string Locale { get; set; }
    public sbyte ViewDistance { get; set; }
    public int ChatFlags { get; set; }
    public bool ChatColors { get; set; }
    public byte SkinParts { get; set; }
    public int MainHand { get; set; }
    public bool DisableTextFiltering { get; set; }
    public bool EnableTextFiltering { get; set; }
    public bool EnableServerListing { get; set; }
    public ParticleStatus ParticleStatus { get; set; }

    [PacketSubInfo(340, 754)]
    public sealed partial class V340_754 : SettingsPacket
    {
        public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        {
            SerializeInternal(ref writer, protocolVersion, Locale, ViewDistance, ChatFlags, ChatColors, SkinParts, MainHand);
        }

        internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion,
            string locale, sbyte viewDistance, int chatFlags, bool chatColors, byte skinParts, int mainHand)
        {
            writer.WriteString(locale);
            writer.WriteSignedByte(viewDistance);
            writer.WriteVarInt(chatFlags);
            writer.WriteBoolean(chatColors);
            writer.WriteUnsignedByte(skinParts);
            writer.WriteVarInt(mainHand);
        }
    }

    [PacketSubInfo(755, 756)]
    public sealed partial class V755_756 : SettingsPacket
    {
        public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        {
            SerializeInternal(ref writer, protocolVersion, Locale, ViewDistance, ChatFlags, ChatColors, SkinParts, MainHand, DisableTextFiltering);
        }

        internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion,
            string locale, sbyte viewDistance, int chatFlags, bool chatColors, byte skinParts, int mainHand,
            bool disableTextFiltering)
        {
            writer.WriteString(locale);
            writer.WriteSignedByte(viewDistance);
            writer.WriteVarInt(chatFlags);
            writer.WriteBoolean(chatColors);
            writer.WriteUnsignedByte(skinParts);
            writer.WriteVarInt(mainHand);
            writer.WriteBoolean(disableTextFiltering);
        }
    }

    [PacketSubInfo(757, 767)]
    public sealed partial class V757_767 : SettingsPacket
    {
        public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        {
            SerializeInternal(ref writer, protocolVersion, Locale, ViewDistance, ChatFlags, ChatColors, SkinParts, MainHand, EnableTextFiltering, EnableServerListing);
        }

        internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion,
            string locale, sbyte viewDistance, int chatFlags, bool chatColors, byte skinParts, int mainHand,
            bool enableTextFiltering, bool enableServerListing)
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

    [PacketSubInfo(768, 769)]
    public sealed partial class V768_769 : SettingsPacket
    {
        public override void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        {
            SerializeInternal(ref writer, protocolVersion, Locale, ViewDistance, ChatFlags, ChatColors, SkinParts, MainHand, EnableTextFiltering, EnableServerListing, ParticleStatus);
        }

        internal static void SerializeInternal(ref MinecraftPrimitiveWriter writer, int protocolVersion,
            string locale, sbyte viewDistance, int chatFlags, bool chatColors, byte skinParts, int mainHand,
            bool enableTextFiltering, bool enableServerListing, ParticleStatus particleStatus)
        {
            writer.WriteString(locale);
            writer.WriteSignedByte(viewDistance);
            writer.WriteVarInt(chatFlags);
            writer.WriteBoolean(chatColors);
            writer.WriteUnsignedByte(skinParts);
            writer.WriteVarInt(mainHand);
            writer.WriteBoolean(enableTextFiltering);
            writer.WriteBoolean(enableServerListing);
            writer.WriteVarInt((int)particleStatus);
        }
    }

    public virtual void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        if (V340_754.IsSupportedVersionStatic(protocolVersion))
            V340_754.SerializeInternal(ref writer, protocolVersion, Locale, ViewDistance, ChatFlags, ChatColors, SkinParts, MainHand);
        else if (V755_756.IsSupportedVersionStatic(protocolVersion))
            V755_756.SerializeInternal(ref writer, protocolVersion, Locale, ViewDistance, ChatFlags, ChatColors, SkinParts, MainHand, DisableTextFiltering);
        else if (V757_767.IsSupportedVersionStatic(protocolVersion))
            V757_767.SerializeInternal(ref writer, protocolVersion, Locale, ViewDistance, ChatFlags, ChatColors, SkinParts, MainHand, EnableTextFiltering, EnableServerListing);
        else if (V768_769.IsSupportedVersionStatic(protocolVersion))
            V768_769.SerializeInternal(ref writer, protocolVersion, Locale, ViewDistance, ChatFlags, ChatColors, SkinParts, MainHand, EnableTextFiltering, EnableServerListing, ParticleStatus);
        else
            throw new ProtocolNotSupportException(nameof(ClientPlayPacket.Settings), protocolVersion);
    }
}

public enum ParticleStatus
{
    All = 0,
    Decreased = 1,
    Minimal = 2
}