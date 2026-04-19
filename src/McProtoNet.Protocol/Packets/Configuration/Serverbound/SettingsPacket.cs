using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Configuration.Serverbound;

[PacketInfo("Settings", PacketState.Configuration, PacketDirection.Serverbound)]
[ProtocolSupport(764, MinecraftVersion.LatestProtocol)]
[PacketId(764, MinecraftVersion.LatestProtocol, 0x00)]
public sealed partial class SettingsPacket : IClientPacket
{
    public string Locale { get; set; }
    public sbyte ViewDistance { get; set; }
    public int ChatFlags { get; set; }
    public bool ChatColors { get; set; }
    public byte SkinParts { get; set; }
    public int MainHand { get; set; }
    public bool EnableTextFiltering { get; set; }
    public bool EnableServerListing { get; set; }
    public int? ParticleStatus { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 767:
            {
                writer.WriteString(Locale);
                writer.WriteSignedByte(ViewDistance);
                writer.WriteVarInt(ChatFlags);
                writer.WriteBoolean(ChatColors);
                writer.WriteUnsignedByte(SkinParts);
                writer.WriteVarInt(MainHand);
                writer.WriteBoolean(EnableTextFiltering);
                writer.WriteBoolean(EnableServerListing);
                return;
            }
            case >= 768 and <= MinecraftVersion.LatestProtocol:
            {
                writer.WriteString(Locale);
                writer.WriteSignedByte(ViewDistance);
                writer.WriteVarInt(ChatFlags);
                writer.WriteBoolean(ChatColors);
                writer.WriteUnsignedByte(SkinParts);
                writer.WriteVarInt(MainHand);
                writer.WriteBoolean(EnableTextFiltering);
                writer.WriteBoolean(EnableServerListing);
                writer.WriteVarInt(ParticleStatus.Value);
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(SettingsPacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 767:
            {
                Locale = reader.ReadString();
                ViewDistance = reader.ReadSignedByte();
                ChatFlags = reader.ReadVarInt();
                ChatColors = reader.ReadBoolean();
                SkinParts = reader.ReadUnsignedByte();
                MainHand = reader.ReadVarInt();
                EnableTextFiltering = reader.ReadBoolean();
                EnableServerListing = reader.ReadBoolean();
                return;
            }
            case >= 768 and <= MinecraftVersion.LatestProtocol:
            {
                Locale = reader.ReadString();
                ViewDistance = reader.ReadSignedByte();
                ChatFlags = reader.ReadVarInt();
                ChatColors = reader.ReadBoolean();
                SkinParts = reader.ReadUnsignedByte();
                MainHand = reader.ReadVarInt();
                EnableTextFiltering = reader.ReadBoolean();
                EnableServerListing = reader.ReadBoolean();
                ParticleStatus = reader.ReadVarInt();
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(SettingsPacket), protocolVersion, SupportedVersions);
                return;
        }
    }
}