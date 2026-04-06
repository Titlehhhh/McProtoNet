using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("WorldBorderWarningDelay", PacketState.Play, PacketDirection.Clientbound)]
public sealed partial class WorldBorderWarningDelayPacket : IServerPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(755, MinecraftVersion.LatestProtocol),
    };

    public int WarningTime { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= 755 and <= MinecraftVersion.LatestProtocol:
                writer.WriteVarInt(WarningTime);
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.WorldBorderWarningDelay), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= 755 and <= MinecraftVersion.LatestProtocol:
                WarningTime = reader.ReadVarInt();
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.WorldBorderWarningDelay), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    void IPacket.Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);
}
