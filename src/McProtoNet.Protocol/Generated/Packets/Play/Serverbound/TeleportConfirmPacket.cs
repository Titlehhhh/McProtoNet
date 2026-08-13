using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toServer.teleport_confirm", PacketPhase.Play, PacketDirection.Serverbound)]
[PacketField("TeleportId", "int")]
public sealed partial record TeleportConfirmPacket(int TeleportId) : IPacket<TeleportConfirmPacket>, IPacket
{
    public static TeleportConfirmPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<TeleportConfirmPacket>(protocolVersion);
        var teleportId = reader.ReadVarInt();
        return new TeleportConfirmPacket(teleportId);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<TeleportConfirmPacket>(protocolVersion);
        writer.WriteVarInt(TeleportId);
    }

    public static PacketIdentity Identity => new("play.toServer.teleport_confirm", "TeleportConfirm", PacketPhase.Play, PacketDirection.Serverbound, 51);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 735 && protocolVersion <= 736)
        {
            id = 0x00;
            return true;
        }

        if (protocolVersion >= 751 && protocolVersion <= 772)
        {
            id = 0x00;
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
