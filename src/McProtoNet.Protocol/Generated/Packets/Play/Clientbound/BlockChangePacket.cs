using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.block_change", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("Location", "Position")]
[PacketField("Type", "int")]
public sealed partial record BlockChangePacket(Position Location, int Type) : IPacket<BlockChangePacket>, IPacket
{
    public static BlockChangePacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<BlockChangePacket>(protocolVersion);
        var location = reader.ReadType<Position>(protocolVersion);
        var type = reader.ReadVarInt();
        return new BlockChangePacket(location, type);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<BlockChangePacket>(protocolVersion);
        writer.WriteType<Position>(Location, protocolVersion);
        writer.WriteVarInt(Type);
    }

    public static PacketIdentity Identity => new("play.toClient.block_change", "BlockChange", PacketPhase.Play, PacketDirection.Clientbound, 8);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 735 && protocolVersion <= 736)
        {
            id = 0x0B;
            return true;
        }

        if (protocolVersion >= 751 && protocolVersion <= 754)
        {
            id = 0x0B;
            return true;
        }

        if (protocolVersion >= 755 && protocolVersion <= 758)
        {
            id = 0x0C;
            return true;
        }

        if (protocolVersion >= 759 && protocolVersion <= 761)
        {
            id = 0x09;
            return true;
        }

        if (protocolVersion >= 762 && protocolVersion <= 763)
        {
            id = 0x0A;
            return true;
        }

        if (protocolVersion >= 764 && protocolVersion <= 769)
        {
            id = 0x09;
            return true;
        }

        if (protocolVersion >= 770 && protocolVersion <= 776)
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
