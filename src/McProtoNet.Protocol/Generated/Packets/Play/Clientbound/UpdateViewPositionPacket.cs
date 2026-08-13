using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.update_view_position", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("ChunkX", "int")]
[PacketField("ChunkZ", "int")]
public sealed partial record UpdateViewPositionPacket(int ChunkX, int ChunkZ) : IPacket<UpdateViewPositionPacket>, IPacket
{
    public static UpdateViewPositionPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<UpdateViewPositionPacket>(protocolVersion);
        var chunkX = reader.ReadVarInt();
        var chunkZ = reader.ReadVarInt();
        return new UpdateViewPositionPacket(chunkX, chunkZ);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<UpdateViewPositionPacket>(protocolVersion);
        writer.WriteVarInt(ChunkX);
        writer.WriteVarInt(ChunkZ);
    }

    public static PacketIdentity Identity => new("play.toClient.update_view_position", "UpdateViewPosition", PacketPhase.Play, PacketDirection.Clientbound, 108);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 735 && protocolVersion <= 736)
        {
            id = 0x40;
            return true;
        }

        if (protocolVersion >= 751 && protocolVersion <= 754)
        {
            id = 0x40;
            return true;
        }

        if (protocolVersion >= 755 && protocolVersion <= 758)
        {
            id = 0x49;
            return true;
        }

        if (protocolVersion >= 759 && protocolVersion <= 759)
        {
            id = 0x48;
            return true;
        }

        if (protocolVersion >= 760 && protocolVersion <= 760)
        {
            id = 0x4B;
            return true;
        }

        if (protocolVersion >= 761 && protocolVersion <= 761)
        {
            id = 0x4A;
            return true;
        }

        if (protocolVersion >= 762 && protocolVersion <= 763)
        {
            id = 0x4E;
            return true;
        }

        if (protocolVersion >= 764 && protocolVersion <= 764)
        {
            id = 0x50;
            return true;
        }

        if (protocolVersion >= 765 && protocolVersion <= 765)
        {
            id = 0x52;
            return true;
        }

        if (protocolVersion >= 766 && protocolVersion <= 767)
        {
            id = 0x54;
            return true;
        }

        if (protocolVersion >= 768 && protocolVersion <= 769)
        {
            id = 0x58;
            return true;
        }

        if (protocolVersion >= 770 && protocolVersion <= 772)
        {
            id = 0x57;
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
