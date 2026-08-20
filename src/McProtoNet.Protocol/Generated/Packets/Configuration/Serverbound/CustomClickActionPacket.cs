using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;
using McProtoNet.NBT;

namespace McProtoNet.Protocol.Packets.Configuration.Serverbound;
[ProtocolSupport(771, MinecraftVersion.LatestProtocol)]
[Packet("configuration.toServer.custom_click_action", PacketPhase.Configuration, PacketDirection.Serverbound)]
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

    public static PacketIdentity Identity => new("configuration.toServer.custom_click_action", "CustomClickAction", PacketPhase.Configuration, PacketDirection.Serverbound, 2);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 771 && protocolVersion <= 776)
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
