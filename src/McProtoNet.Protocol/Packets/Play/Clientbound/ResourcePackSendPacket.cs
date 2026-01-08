using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("ResourcePackSend", PacketState.Play, PacketDirection.Clientbound)]
public sealed partial class ResourcePackSendPacket : IServerPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(MinecraftVersion.StartProtocol, 754),
        new(755, 764),
    };

    public string Url { get; set; }
    public string Hash { get; set; }

    public V755_764Fields? V755_764 { get; set; }

    internal void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 754:
            {
                writer.WriteString(Url);
                writer.WriteString(Hash);
                return;
            }
            case >= 755 and <= 764:
            {
                var fields = V755_764 ?? throw new InvalidOperationException("ResourcePackSend V755_764 fields missing.");
                writer.WriteString(Url);
                writer.WriteString(Hash);
                writer.WriteBoolean(fields.Forced);
                if (fields.PromptMessage is null)
                {
                    writer.WriteBoolean(false);
                }
                else
                {
                    writer.WriteBoolean(true);
                    writer.WriteString(fields.PromptMessage);
                }
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.ResourcePackSend), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 754:
            {
                Url = reader.ReadString();
                Hash = reader.ReadString();
                return;
            }
            case >= 755 and <= 764:
            {
                var fields = new V755_764Fields();
                Url = reader.ReadString();
                Hash = reader.ReadString();
                fields.Forced = reader.ReadBoolean();
                fields.PromptMessage = reader.ReadOptional(ReadDelegates.String);
                V755_764 = fields;
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.ResourcePackSend), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    void IPacket.Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(ref writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);

    public struct V755_764Fields
    {
        public bool Forced { get; set; }
        public string? PromptMessage { get; set; }
    }

}
