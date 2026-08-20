using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Configuration.Clientbound;
[ProtocolSupport(771, MinecraftVersion.LatestProtocol)]
[Packet("configuration.toClient.clear_dialog", PacketPhase.Configuration, PacketDirection.Clientbound)]
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

    public static PacketIdentity Identity => new("configuration.toClient.clear_dialog", "ClearDialog", PacketPhase.Configuration, PacketDirection.Clientbound, 1);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 771 && protocolVersion <= 776)
        {
            id = 0x11;
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
