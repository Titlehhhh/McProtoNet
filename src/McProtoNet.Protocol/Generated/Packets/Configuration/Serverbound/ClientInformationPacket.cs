using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Configuration.Serverbound;
[ProtocolSupport(764, MinecraftVersion.LatestProtocol)]
[Packet("configuration.toServer.settings", PacketPhase.Configuration, PacketDirection.Serverbound)]
[PacketField("Locale", "string")]
[PacketField("ViewDistance", "int")]
[PacketField("ChatFlags", "int")]
[PacketField("ChatColors", "bool")]
[PacketField("SkinParts", "int")]
[PacketField("MainHand", "int")]
[PacketField("EnableTextFiltering", "bool")]
[PacketField("EnableServerListing", "bool")]
[PacketField("ParticleStatus", "int", Group = "V768_Last", From = 768)]
public sealed partial record ClientInformationPacket(string Locale, int ViewDistance, int ChatFlags, bool ChatColors, int SkinParts, int MainHand, bool EnableTextFiltering, bool EnableServerListing, ClientInformationPacket.V768_LastLayer? V768_Last = null) : IPacket<ClientInformationPacket>, IPacket
{
    public readonly record struct V768_LastLayer(int ParticleStatus);
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
            return new ClientInformationPacket(locale, viewDistance, chatFlags, chatColors, skinParts, mainHand, enableTextFiltering, enableServerListing);
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
            return new ClientInformationPacket(locale, viewDistance, chatFlags, chatColors, skinParts, mainHand, enableTextFiltering, enableServerListing, V768_Last: new V768_LastLayer(particleStatus));
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
            var layer = V768_Last ?? throw new WrongLayerException("ClientInformationPacket", protocolVersion, "V768_Last");
            int ParticleStatus = layer.ParticleStatus;
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

    public static PacketIdentity Identity => new("configuration.toServer.settings", "ClientInformation", PacketPhase.Configuration, PacketDirection.Serverbound, 9);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 764 && protocolVersion <= 772)
        {
            id = 0x00;
            return true;
        }

        id = 0;
        return false;
    }

    public static int GetPacketId(int protocolVersion)
    {
        if (TryGetPacketId(protocolVersion, out var id))
            return id;
        throw new System.NotSupportedException($"No packet id for protocol {protocolVersion}.");
    }
}
