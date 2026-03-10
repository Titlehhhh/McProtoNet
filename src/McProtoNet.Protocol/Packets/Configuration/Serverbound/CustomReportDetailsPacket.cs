using System;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Configuration.Serverbound;

[PacketInfo("CustomReportDetails", PacketState.Configuration, PacketDirection.Serverbound)]
public sealed partial class CustomReportDetailsPacket : IClientPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(767, 770)
    };

    public PacketCommonCustomReportDetails? Data { get; set; }

    internal void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= 767 and <= 770:
            {
                var data = Data ?? throw new InvalidOperationException("CustomReportDetails data missing.");
                writer.WritePacketCommonCustomReportDetails(data, protocolVersion);
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ClientConfigurationPacket.CustomReportDetails), protocolVersion,
                    SupportedVersionsStatic);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= 767 and <= 770:
                Data = reader.ReadPacketCommonCustomReportDetails(protocolVersion);
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ClientConfigurationPacket.CustomReportDetails), protocolVersion,
                    SupportedVersionsStatic);
                return;
        }
    }

    void IPacket.Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(ref writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);
}
