using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;

[PacketInfo("ChatPreview", PacketState.Play, PacketDirection.Serverbound)]
public sealed partial class ChatPreviewPacket : IClientPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(759, 760)
    };

    public int Query { get; set; }
    public string Message { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= 759 and <= 760:
                writer.WriteSignedInt(Query);
                writer.WriteString(Message);
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ClientPlayPacket.ChatPreview), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= 759 and <= 760:
                Query = reader.ReadSignedInt();
                Message = reader.ReadString();
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ClientPlayPacket.ChatPreview), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    void IPacket.Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);
}
