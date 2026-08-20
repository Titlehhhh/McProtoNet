using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toServer.block_place", PacketPhase.Play, PacketDirection.Serverbound)]
[PacketField("Hand", "int")]
[PacketField("Location", "Position")]
[PacketField("Direction", "int")]
[PacketField("CursorX", "float")]
[PacketField("CursorY", "float")]
[PacketField("CursorZ", "float")]
[PacketField("InsideBlock", "bool")]
[PacketField("Sequence", "int", Group = "V759_767", From = 759, To = 767)]
[PacketField("WorldBorderHit", "bool", Group = "V768_Last", From = 768)]
[PacketField("Sequence", "int", Group = "V768_Last", From = 768)]
public sealed partial record BlockPlacePacket(int Hand, Position Location, int Direction, float CursorX, float CursorY, float CursorZ, bool InsideBlock, BlockPlacePacket.V759_767Layer? V759_767 = null, BlockPlacePacket.V768_LastLayer? V768_Last = null) : IPacket<BlockPlacePacket>, IPacket
{
    public readonly record struct V759_767Layer(int Sequence);
    public readonly record struct V768_LastLayer(bool WorldBorderHit, int Sequence);
    public static BlockPlacePacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<BlockPlacePacket>(protocolVersion);
        if (protocolVersion <= 758)
        {
            var hand = reader.ReadVarInt();
            var location = reader.ReadType<Position>(protocolVersion);
            var direction = reader.ReadVarInt();
            var cursorX = reader.ReadFloat();
            var cursorY = reader.ReadFloat();
            var cursorZ = reader.ReadFloat();
            var insideBlock = reader.ReadBoolean();
            return new BlockPlacePacket(hand, location, direction, cursorX, cursorY, cursorZ, insideBlock);
        }

        if (protocolVersion >= 759 && protocolVersion <= 767)
        {
            var hand = reader.ReadVarInt();
            var location = reader.ReadType<Position>(protocolVersion);
            var direction = reader.ReadVarInt();
            var cursorX = reader.ReadFloat();
            var cursorY = reader.ReadFloat();
            var cursorZ = reader.ReadFloat();
            var insideBlock = reader.ReadBoolean();
            var sequence = reader.ReadVarInt();
            return new BlockPlacePacket(hand, location, direction, cursorX, cursorY, cursorZ, insideBlock, V759_767: new V759_767Layer(sequence));
        }

        if (protocolVersion >= 768)
        {
            var hand = reader.ReadVarInt();
            var location = reader.ReadType<Position>(protocolVersion);
            var direction = reader.ReadVarInt();
            var cursorX = reader.ReadFloat();
            var cursorY = reader.ReadFloat();
            var cursorZ = reader.ReadFloat();
            var insideBlock = reader.ReadBoolean();
            var worldBorderHit = reader.ReadBoolean();
            var sequence = reader.ReadVarInt();
            return new BlockPlacePacket(hand, location, direction, cursorX, cursorY, cursorZ, insideBlock, V768_Last: new V768_LastLayer(worldBorderHit, sequence));
        }

        throw new System.NotSupportedException($"BlockPlacePacket has no wire layout for protocol version {protocolVersion}.");
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<BlockPlacePacket>(protocolVersion);
        if (protocolVersion <= 758)
        {
            writer.WriteVarInt(Hand);
            writer.WriteType<Position>(Location, protocolVersion);
            writer.WriteVarInt(Direction);
            writer.WriteFloat(CursorX);
            writer.WriteFloat(CursorY);
            writer.WriteFloat(CursorZ);
            writer.WriteBoolean(InsideBlock);
            return;
        }

        if (protocolVersion >= 759 && protocolVersion <= 767)
        {
            var layer = V759_767 ?? throw new WrongLayerException("BlockPlacePacket", protocolVersion, "V759_767");
            int Sequence = layer.Sequence;
            writer.WriteVarInt(Hand);
            writer.WriteType<Position>(Location, protocolVersion);
            writer.WriteVarInt(Direction);
            writer.WriteFloat(CursorX);
            writer.WriteFloat(CursorY);
            writer.WriteFloat(CursorZ);
            writer.WriteBoolean(InsideBlock);
            writer.WriteVarInt(Sequence);
            return;
        }

        if (protocolVersion >= 768)
        {
            var layer = V768_Last ?? throw new WrongLayerException("BlockPlacePacket", protocolVersion, "V768_Last");
            bool WorldBorderHit = layer.WorldBorderHit;
            int Sequence = layer.Sequence;
            writer.WriteVarInt(Hand);
            writer.WriteType<Position>(Location, protocolVersion);
            writer.WriteVarInt(Direction);
            writer.WriteFloat(CursorX);
            writer.WriteFloat(CursorY);
            writer.WriteFloat(CursorZ);
            writer.WriteBoolean(InsideBlock);
            writer.WriteBoolean(WorldBorderHit);
            writer.WriteVarInt(Sequence);
            return;
        }

        throw new System.NotSupportedException($"BlockPlacePacket has no wire layout for protocol version {protocolVersion}.");
    }

    public static PacketIdentity Identity => new("play.toServer.block_place", "BlockPlace", PacketPhase.Play, PacketDirection.Serverbound, 4);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 735 && protocolVersion <= 736)
        {
            id = 0x2D;
            return true;
        }

        if (protocolVersion >= 751 && protocolVersion <= 758)
        {
            id = 0x2E;
            return true;
        }

        if (protocolVersion >= 759 && protocolVersion <= 759)
        {
            id = 0x30;
            return true;
        }

        if (protocolVersion >= 760 && protocolVersion <= 763)
        {
            id = 0x31;
            return true;
        }

        if (protocolVersion >= 764 && protocolVersion <= 764)
        {
            id = 0x34;
            return true;
        }

        if (protocolVersion >= 765 && protocolVersion <= 765)
        {
            id = 0x35;
            return true;
        }

        if (protocolVersion >= 766 && protocolVersion <= 767)
        {
            id = 0x38;
            return true;
        }

        if (protocolVersion >= 768 && protocolVersion <= 768)
        {
            id = 0x3A;
            return true;
        }

        if (protocolVersion >= 769 && protocolVersion <= 769)
        {
            id = 0x3C;
            return true;
        }

        if (protocolVersion >= 770 && protocolVersion <= 770)
        {
            id = 0x3E;
            return true;
        }

        if (protocolVersion >= 771 && protocolVersion <= 774)
        {
            id = 0x3F;
            return true;
        }

        if (protocolVersion >= 775 && protocolVersion <= 776)
        {
            id = 0x42;
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
