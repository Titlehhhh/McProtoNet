using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Configuration.Serverbound;
[ProtocolSupport(767, MinecraftVersion.LatestProtocol)]
[Packet("configuration.toServer.custom_report_details", PacketPhase.Configuration, PacketDirection.Serverbound)]
[PacketField("Details", "ReportDetail[]")]
public sealed partial record CustomReportDetailsPacket(ReportDetail[] Details) : IPacket<CustomReportDetailsPacket>, IPacket
{
    public static CustomReportDetailsPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<CustomReportDetailsPacket>(protocolVersion);
        int detailsCount = reader.ReadVarInt();
        var details = new ReportDetail[detailsCount];
        for (int i = 0; i < details.Length; i++)
            details[i] = reader.ReadType<ReportDetail>(protocolVersion);
        return new CustomReportDetailsPacket(details);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<CustomReportDetailsPacket>(protocolVersion);
        writer.WriteVarInt(Details.Length);
        foreach (var detailsItem in Details)
            writer.WriteType<ReportDetail>(detailsItem, protocolVersion);
    }

    public static PacketIdentity Identity => new("configuration.toServer.custom_report_details", "CustomReportDetails", PacketPhase.Configuration, PacketDirection.Serverbound, 4);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 767 && protocolVersion <= 770)
        {
            id = 0x08;
            return true;
        }

        id = 0;
        return false;
    }

    public static int GetPacketId(int protocolVersion)
    {
        if (TryGetPacketId(protocolVersion, out var id))
            return id;
        throw new System.NotSupportedException($"No packet id for protocol {protocolVersion}.");
    }
}
