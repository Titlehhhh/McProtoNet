using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.block_action", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("Location", "Position")]
[PacketField("Byte1", "int")]
[PacketField("Byte2", "int")]
[PacketField("BlockId", "int")]
public sealed partial record BlockActionPacket(Position Location, int Byte1, int Byte2, int BlockId) : IPacket<BlockActionPacket>, IPacket
{
    public static BlockActionPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<BlockActionPacket>(protocolVersion);
        var location = reader.ReadType<Position>(protocolVersion);
        var byte1 = reader.ReadUnsignedByte();
        var byte2 = reader.ReadUnsignedByte();
        var blockId = reader.ReadVarInt();
        return new BlockActionPacket(location, byte1, byte2, blockId);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<BlockActionPacket>(protocolVersion);
        writer.WriteType<Position>(Location, protocolVersion);
        writer.WriteUnsignedByte((byte)Byte1);
        writer.WriteUnsignedByte((byte)Byte2);
        writer.WriteVarInt(BlockId);
    }

    public static PacketIdentity Identity => new("play.toClient.block_action", "BlockAction", PacketPhase.Play, PacketDirection.Clientbound, 6);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 735 && protocolVersion <= 736)
        {
            id = 0x0A;
            return true;
        }

        if (protocolVersion >= 751 && protocolVersion <= 754)
        {
            id = 0x0A;
            return true;
        }

        if (protocolVersion >= 755 && protocolVersion <= 758)
        {
            id = 0x0B;
            return true;
        }

        if (protocolVersion >= 759 && protocolVersion <= 761)
        {
            id = 0x08;
            return true;
        }

        if (protocolVersion >= 762 && protocolVersion <= 763)
        {
            id = 0x09;
            return true;
        }

        if (protocolVersion >= 764 && protocolVersion <= 769)
        {
            id = 0x08;
            return true;
        }

        if (protocolVersion >= 770 && protocolVersion <= 776)
        {
            id = 0x07;
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
