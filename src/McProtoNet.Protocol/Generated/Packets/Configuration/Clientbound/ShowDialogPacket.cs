using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
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

    public static PacketIdentity Identity => new("configuration.toClient.show_dialog", "ShowDialog", PacketPhase.Configuration, PacketDirection.Clientbound, 16);

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
