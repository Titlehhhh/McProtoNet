using McProtoNet.Protocol;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("SetCursorItem", PacketState.Play, PacketDirection.Clientbound)]
public sealed partial class SetCursorItemPacket : IServerPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(768, 768),
        new(769, MinecraftVersion.LatestProtocol)
    };

    public Slot? Contents { get; set; }

    internal void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case 768:
                if (Contents is null)
                {
                    writer.WriteBoolean(false);
                }
                else
                {
                    writer.WriteBoolean(true);
                    writer.WriteSlot(Contents.Value, protocolVersion);
                }
                return;
            case >= 769 and <= MinecraftVersion.LatestProtocol:
                writer.WriteSlot(Contents ?? throw new InvalidOperationException("SetCursorItem Contents missing."), protocolVersion);
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.SetCursorItem), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case 768:
                Contents = reader.ReadOptional((ref MinecraftPrimitiveReader r) => r.ReadSlot(protocolVersion));
                return;
            case >= 769 and <= MinecraftVersion.LatestProtocol:
                Contents = reader.ReadSlot(protocolVersion);
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.SetCursorItem), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    void IPacket.Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(ref writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);
}
