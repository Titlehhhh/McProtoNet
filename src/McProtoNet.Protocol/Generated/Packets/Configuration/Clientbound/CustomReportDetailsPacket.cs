using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Configuration.Clientbound;

[ProtocolSupport(767, MinecraftVersion.LatestProtocol)]
[Packet("configuration.toClient.custom_report_details", PacketPhase.Configuration, PacketDirection.Clientbound)]
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

    public static PacketIdentity Identity => new("configuration.toClient.custom_report_details", "CustomReportDetails", PacketPhase.Configuration, PacketDirection.Clientbound, 5);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        return PacketRegistry.TryGetId(Identity, protocolVersion, out id);
    }

    public static int GetPacketId(int protocolVersion)
    {
        return PacketRegistry.GetId(Identity, protocolVersion);
    }
}
