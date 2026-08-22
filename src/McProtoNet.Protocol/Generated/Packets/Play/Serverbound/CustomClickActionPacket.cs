using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
using McProtoNet.NBT;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;
[ProtocolSupport(771, MinecraftVersion.LatestProtocol)]
[Packet("play.toServer.custom_click_action", PacketPhase.Play, PacketDirection.Serverbound)]
[PacketField("Id", "string")]
[PacketField("Nbt", "NbtTag?")]
public sealed partial record CustomClickActionPacket(string Id, NbtTag? Nbt) : IPacket<CustomClickActionPacket>, IPacket
{
    public static CustomClickActionPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<CustomClickActionPacket>(protocolVersion);
        var id = reader.ReadString();
        NbtTag? nbt = null;
        if (reader.ReadBoolean())
            nbt = reader.ReadNbtTag(false)!;
        return new CustomClickActionPacket(id, nbt);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<CustomClickActionPacket>(protocolVersion);
        writer.WriteString(Id);
        writer.WriteBoolean(Nbt is not null);
        if (Nbt is { } nbtValue)
            writer.WriteNbt(nbtValue);
    }

    public static PacketIdentity Identity => new("play.toServer.custom_click_action", "CustomClickAction", PacketPhase.Play, PacketDirection.Serverbound, 17);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 771 && protocolVersion <= 774)
        {
            id = 0x41;
            return true;
        }

        if (protocolVersion >= 775 && protocolVersion <= 776)
        {
            id = 0x44;
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
