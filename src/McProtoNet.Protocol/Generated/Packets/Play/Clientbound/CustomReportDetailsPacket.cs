using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;
[ProtocolSupport(767, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.custom_report_details", PacketPhase.Play, PacketDirection.Clientbound)]
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

    public static PacketIdentity Identity => new("play.toClient.custom_report_details", "CustomReportDetails", PacketPhase.Play, PacketDirection.Clientbound, 23);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 767 && protocolVersion <= 767)
        {
            id = 0x7A;
            return true;
        }

        if (protocolVersion >= 768 && protocolVersion <= 772)
        {
            id = 0x81;
            return true;
        }

        if (protocolVersion >= 773 && protocolVersion <= 774)
        {
            id = 0x86;
            return true;
        }

        if (protocolVersion >= 775 && protocolVersion <= 776)
        {
            id = 0x88;
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
