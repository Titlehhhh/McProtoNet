using System;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Configuration.Serverbound;

[PacketInfo("Settings", PacketState.Configuration, PacketDirection.Serverbound)]
public sealed partial class SettingsPacket : IClientPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(764, 765),
        new(766, MinecraftVersion.LatestProtocol)
    };

    public V764_765Fields? V764_765 { get; set; }
    public PacketCommonSettings? Data { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= 764 and <= 765:
            {
                var fields = V764_765 ?? throw new InvalidOperationException("Settings V764_765 fields missing.");
                writer.WriteString(fields.Locale);
                writer.WriteSignedByte(fields.ViewDistance);
                writer.WriteVarInt(fields.ChatFlags);
                writer.WriteBoolean(fields.ChatColors);
                writer.WriteUnsignedByte(fields.SkinParts);
                writer.WriteVarInt(fields.MainHand);
                writer.WriteBoolean(fields.EnableTextFiltering);
                writer.WriteBoolean(fields.EnableServerListing);
                return;
            }
            case >= 766 and <= MinecraftVersion.LatestProtocol:
            {
                var data = Data ?? throw new InvalidOperationException("Settings data missing.");
                writer.WritePacketCommonSettings(data, protocolVersion);
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ClientConfigurationPacket.Settings), protocolVersion,
                    SupportedVersionsStatic);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= 764 and <= 765:
                V764_765 = new V764_765Fields
                {
                    Locale = reader.ReadString(),
                    ViewDistance = reader.ReadSignedByte(),
                    ChatFlags = reader.ReadVarInt(),
                    ChatColors = reader.ReadBoolean(),
                    SkinParts = reader.ReadUnsignedByte(),
                    MainHand = reader.ReadVarInt(),
                    EnableTextFiltering = reader.ReadBoolean(),
                    EnableServerListing = reader.ReadBoolean()
                };
                Data = null;
                return;
            case >= 766 and <= MinecraftVersion.LatestProtocol:
                Data = reader.ReadPacketCommonSettings(protocolVersion);
                V764_765 = null;
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ClientConfigurationPacket.Settings), protocolVersion,
                    SupportedVersionsStatic);
                return;
        }
    }

    void IPacket.Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);

    public struct V764_765Fields
    {
        public string Locale { get; set; }
        public sbyte ViewDistance { get; set; }
        public int ChatFlags { get; set; }
        public bool ChatColors { get; set; }
        public byte SkinParts { get; set; }
        public int MainHand { get; set; }
        public bool EnableTextFiltering { get; set; }
        public bool EnableServerListing { get; set; }
    }
}
