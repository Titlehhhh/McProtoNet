using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Configuration.Serverbound;
[ProtocolSupport(764, MinecraftVersion.LatestProtocol)]
public sealed partial class ClientInformationPacket : IProtocolType<ClientInformationPacket>
{
    public string Locale { get; }
    public int ViewDistance { get; }
    public int ChatFlags { get; }
    public bool ChatColors { get; }
    public int SkinParts { get; }
    public int MainHand { get; }
    public bool EnableTextFiltering { get; }
    public bool EnableServerListing { get; }
    public int ParticleStatus { get; }

    public ClientInformationPacket(string locale, int viewDistance, int chatFlags, bool chatColors, int skinParts, int mainHand, bool enableTextFiltering, bool enableServerListing, int particleStatus)
    {
        Locale = locale;
        ViewDistance = viewDistance;
        ChatFlags = chatFlags;
        ChatColors = chatColors;
        SkinParts = skinParts;
        MainHand = mainHand;
        EnableTextFiltering = enableTextFiltering;
        EnableServerListing = enableServerListing;
        ParticleStatus = particleStatus;
    }

    public static ClientInformationPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ClientInformationPacket>(protocolVersion);
        if (protocolVersion >= 764 && protocolVersion <= 767)
        {
            var locale = reader.ReadString();
            var viewDistance = reader.ReadSignedByte();
            var chatFlags = reader.ReadVarInt();
            var chatColors = reader.ReadBoolean();
            var skinParts = reader.ReadUnsignedByte();
            var mainHand = reader.ReadVarInt();
            var enableTextFiltering = reader.ReadBoolean();
            var enableServerListing = reader.ReadBoolean();
            return new ClientInformationPacket(locale, viewDistance, chatFlags, chatColors, skinParts, mainHand, enableTextFiltering, enableServerListing, default!);
        }

        if (protocolVersion >= 768)
        {
            var locale = reader.ReadString();
            var viewDistance = reader.ReadSignedByte();
            var chatFlags = reader.ReadVarInt();
            var chatColors = reader.ReadBoolean();
            var skinParts = reader.ReadUnsignedByte();
            var mainHand = reader.ReadVarInt();
            var enableTextFiltering = reader.ReadBoolean();
            var enableServerListing = reader.ReadBoolean();
            var particleStatus = reader.ReadVarInt();
            return new ClientInformationPacket(locale, viewDistance, chatFlags, chatColors, skinParts, mainHand, enableTextFiltering, enableServerListing, particleStatus);
        }

        throw new System.NotSupportedException($"ClientInformationPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ClientInformationPacket>(protocolVersion);
        if (protocolVersion >= 764 && protocolVersion <= 767)
        {
            writer.WriteString(Locale);
            writer.WriteSignedByte((sbyte)ViewDistance);
            writer.WriteVarInt(ChatFlags);
            writer.WriteBoolean(ChatColors);
            writer.WriteUnsignedByte((byte)SkinParts);
            writer.WriteVarInt(MainHand);
            writer.WriteBoolean(EnableTextFiltering);
            writer.WriteBoolean(EnableServerListing);
            return;
        }

        if (protocolVersion >= 768)
        {
            writer.WriteString(Locale);
            writer.WriteSignedByte((sbyte)ViewDistance);
            writer.WriteVarInt(ChatFlags);
            writer.WriteBoolean(ChatColors);
            writer.WriteUnsignedByte((byte)SkinParts);
            writer.WriteVarInt(MainHand);
            writer.WriteBoolean(EnableTextFiltering);
            writer.WriteBoolean(EnableServerListing);
            writer.WriteVarInt(ParticleStatus);
            return;
        }

        throw new System.NotSupportedException($"ClientInformationPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public static int GetPacketId(int protocolVersion)
    {
        if (protocolVersion >= 764 && protocolVersion <= 765)
            return 0x00;
        if (protocolVersion >= 766 && protocolVersion <= 766)
            return 0x00;
        if (protocolVersion >= 767 && protocolVersion <= 770)
            return 0x00;
        if (protocolVersion >= 771 && protocolVersion <= 772)
            return 0x00;
        throw new System.NotSupportedException($"No packet id for protocol {protocolVersion}.");
    }
}
