using System;
using McProtoNet.Protocol;
using McProtoNet.Protocol.Extensions;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("CustomReportDetails", PacketState.Play, PacketDirection.Clientbound)]
public sealed partial class CustomReportDetailsPacket : IServerPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(767, MinecraftVersion.LatestProtocol)
    };

    public PacketCommonCustomReportDetails Data { get; set; } = null!;

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= 767 and <= MinecraftVersion.LatestProtocol:
                writer.WritePacketCommonCustomReportDetails(Data, protocolVersion);
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.CustomReportDetails), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= 767 and <= MinecraftVersion.LatestProtocol:
                Data = reader.ReadPacketCommonCustomReportDetails(protocolVersion);
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.CustomReportDetails), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    void IPacket.Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);
}
