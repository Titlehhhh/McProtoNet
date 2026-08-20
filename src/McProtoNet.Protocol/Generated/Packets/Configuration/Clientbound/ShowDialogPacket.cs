using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;
using McProtoNet.NBT;

namespace McProtoNet.Protocol.Packets.Configuration.Clientbound;
[ProtocolSupport(771, MinecraftVersion.LatestProtocol)]
[Packet("configuration.toClient.show_dialog", PacketPhase.Configuration, PacketDirection.Clientbound)]
[PacketField("Dialog", "NbtTag")]
public sealed partial record ShowDialogPacket(NbtTag Dialog) : IPacket<ShowDialogPacket>, IPacket
{
    public static ShowDialogPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ShowDialogPacket>(protocolVersion);
        var dialog = reader.ReadNbtTag(false)!;
        return new ShowDialogPacket(dialog);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ShowDialogPacket>(protocolVersion);
        writer.WriteNbt(Dialog);
    }

    public static PacketIdentity Identity => new("configuration.toClient.show_dialog", "ShowDialog", PacketPhase.Configuration, PacketDirection.Clientbound, 15);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 771 && protocolVersion <= 776)
        {
            id = 0x12;
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
