using System;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Configuration.Clientbound;

[PacketInfo("ResourcePackSend", PacketState.Configuration, PacketDirection.Clientbound)]
public sealed partial class ResourcePackSendPacket : IServerPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(764, 764)
    };
    public string Url { get; set; } = string.Empty;
    public string Hash { get; set; } = string.Empty;
    public bool Forced { get; set; }
    public string? PromptMessage { get; set; }

    internal void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case 764:
                writer.WriteString(Url);
                writer.WriteString(Hash);
                writer.WriteBoolean(Forced);
                if (PromptMessage is null)
                {
                    writer.WriteBoolean(false);
                }
                else
                {
                    writer.WriteBoolean(true);
                    writer.WriteString(PromptMessage);
                }
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerConfigurationPacket.ResourcePackSend), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case 764:
                Url = reader.ReadString();
                Hash = reader.ReadString();
                Forced = reader.ReadBoolean();
                PromptMessage = reader.ReadOptional((ref MinecraftPrimitiveReader r) => r.ReadString());
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerConfigurationPacket.ResourcePackSend), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    void IPacket.Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(ref writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);
}