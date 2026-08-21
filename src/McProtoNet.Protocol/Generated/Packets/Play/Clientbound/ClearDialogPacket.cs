using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;
[ProtocolSupport(771, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.clear_dialog", PacketPhase.Play, PacketDirection.Clientbound)]
public sealed partial record ClearDialogPacket() : IPacket<ClearDialogPacket>, IPacket
{
    public static ClearDialogPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ClearDialogPacket>(protocolVersion);
        return new ClearDialogPacket();
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ClearDialogPacket>(protocolVersion);
    }

    public static PacketIdentity Identity => new("play.toClient.clear_dialog", "ClearDialog", PacketPhase.Play, PacketDirection.Clientbound, 16);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 771 && protocolVersion <= 772)
        {
            id = 0x84;
            return true;
        }

        if (protocolVersion >= 773 && protocolVersion <= 774)
        {
            id = 0x89;
            return true;
        }

        if (protocolVersion >= 775 && protocolVersion <= 776)
        {
            id = 0x8B;
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
