using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;
[ProtocolSupport(771, MinecraftVersion.LatestProtocol)]
[Packet("play.toServer.change_gamemode", PacketPhase.Play, PacketDirection.Serverbound)]
[PacketField("Mode", "Gamemode")]
public sealed partial record ChangeGamemodePacket(Gamemode Mode) : IPacket<ChangeGamemodePacket>, IPacket
{
    public static ChangeGamemodePacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ChangeGamemodePacket>(protocolVersion);
        var mode = new Gamemode((int)reader.ReadVarInt());
        return new ChangeGamemodePacket(mode);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ChangeGamemodePacket>(protocolVersion);
        writer.WriteVarInt((int)Mode.Value);
    }

    public static PacketIdentity Identity => new("play.toServer.change_gamemode", "ChangeGamemode", PacketPhase.Play, PacketDirection.Serverbound, 5);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 771 && protocolVersion <= 774)
        {
            id = 0x04;
            return true;
        }

        if (protocolVersion >= 775 && protocolVersion <= 776)
        {
            id = 0x05;
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
