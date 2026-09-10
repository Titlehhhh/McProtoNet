using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
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
        return PacketRegistry.TryGetId(Identity, protocolVersion, out id);
    }

    public static int GetPacketId(int protocolVersion)
    {
        return PacketRegistry.GetId(Identity, protocolVersion);
    }
}
