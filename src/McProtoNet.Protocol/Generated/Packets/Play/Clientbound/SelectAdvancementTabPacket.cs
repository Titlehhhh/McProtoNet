using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.select_advancement_tab", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("Id", "string?")]
public sealed partial record SelectAdvancementTabPacket(string? Id) : IPacket<SelectAdvancementTabPacket>, IPacket
{
    public static SelectAdvancementTabPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<SelectAdvancementTabPacket>(protocolVersion);
        string? id = null;
        if (reader.ReadBoolean())
            id = reader.ReadString();
        return new SelectAdvancementTabPacket(id);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<SelectAdvancementTabPacket>(protocolVersion);
        writer.WriteBoolean(Id is not null);
        if (Id is { } idValue)
            writer.WriteString(idValue);
    }

    public static PacketIdentity Identity => new("play.toClient.select_advancement_tab", "SelectAdvancementTab", PacketPhase.Play, PacketDirection.Clientbound, 86);

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
