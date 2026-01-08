using System;
using McProtoNet.NBT;
using McProtoNet.Protocol.Extensions;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Configuration.Clientbound;

[PacketInfo("ShowDialog", PacketState.Configuration, PacketDirection.Clientbound)]
public sealed partial class ShowDialogPacket : IServerPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(771, MinecraftVersion.LatestProtocol)
    };

    public NbtTag Dialog { get; set; } = null!;

    internal void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= 771 and <= MinecraftVersion.LatestProtocol:
                writer.WriteAnonymousNbtTag(Dialog, protocolVersion);
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerConfigurationPacket.ShowDialog), protocolVersion, SupportedVersionsStatic);
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= 771 and <= MinecraftVersion.LatestProtocol:
                Dialog = reader.ReadAnonymousNbtTag(protocolVersion)
                    ?? throw new InvalidOperationException("ShowDialog.dialog missing.");
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerConfigurationPacket.ShowDialog), protocolVersion, SupportedVersionsStatic);
        }
    }

    void IPacket.Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(ref writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);
}