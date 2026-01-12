using McProtoNet.Protocol;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("Title", PacketState.Play, PacketDirection.Clientbound)]
public sealed partial class TitlePacket : IServerPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(MinecraftVersion.StartProtocol, 754),
    };

    public int Action { get; set; }
    public string? Text { get; set; }
    public int? FadeIn { get; set; }
    public int? Stay { get; set; }
    public int? FadeOut { get; set; }

    internal void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 754:
                writer.WriteVarInt(Action);
                switch (Action)
                {
                    case 0:
                    case 1:
                    case 2:
                        writer.WriteString(Text ?? throw new InvalidOperationException("Title text missing."));
                        break;
                    case 3:
                        writer.WriteSignedInt(FadeIn ?? throw new InvalidOperationException("Title fadeIn missing."));
                        writer.WriteSignedInt(Stay ?? throw new InvalidOperationException("Title stay missing."));
                        writer.WriteSignedInt(FadeOut ?? throw new InvalidOperationException("Title fadeOut missing."));
                        break;
                }
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.Title), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 754:
                Action = reader.ReadVarInt();
                switch (Action)
                {
                    case 0:
                    case 1:
                    case 2:
                        Text = reader.ReadString();
                        break;
                    case 3:
                        FadeIn = reader.ReadSignedInt();
                        Stay = reader.ReadSignedInt();
                        FadeOut = reader.ReadSignedInt();
                        break;
                }
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.Title), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    void IPacket.Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(ref writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);
}
