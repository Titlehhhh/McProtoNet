using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;
using McProtoNet.NBT;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.tile_entity_data", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("Location", "Position")]
[PacketField("Action", "int")]
[PacketField("NbtData", "NbtTag?")]
public sealed partial record TileEntityDataPacket(Position Location, int Action, NbtTag? NbtData) : IPacket<TileEntityDataPacket>, IPacket
{
    public static TileEntityDataPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<TileEntityDataPacket>(protocolVersion);
        if (protocolVersion <= 756)
        {
            var location = reader.ReadType<Position>(protocolVersion);
            var action = reader.ReadUnsignedByte();
            NbtTag? nbtData = null;
            if (reader.ReadBoolean())
                nbtData = reader.ReadNbtTag(true)!;
            return new TileEntityDataPacket(location, action, nbtData);
        }

        if (protocolVersion >= 757 && protocolVersion <= 763)
        {
            var location = reader.ReadType<Position>(protocolVersion);
            var action = reader.ReadVarInt();
            NbtTag? nbtData = null;
            if (reader.ReadBoolean())
                nbtData = reader.ReadNbtTag(true)!;
            return new TileEntityDataPacket(location, action, nbtData);
        }

        if (protocolVersion >= 764)
        {
            var location = reader.ReadType<Position>(protocolVersion);
            var action = reader.ReadVarInt();
            NbtTag? nbtData = null;
            if (reader.ReadBoolean())
                nbtData = reader.ReadNbtTag(false)!;
            return new TileEntityDataPacket(location, action, nbtData);
        }

        throw new System.NotSupportedException($"TileEntityDataPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<TileEntityDataPacket>(protocolVersion);
        if (protocolVersion <= 756)
        {
            writer.WriteType<Position>(Location, protocolVersion);
            writer.WriteUnsignedByte((byte)Action);
            writer.WriteBoolean(NbtData is not null);
            if (NbtData is { } nbtDataValue)
                writer.WriteNbt(nbtDataValue, true);
            return;
        }

        if (protocolVersion >= 757 && protocolVersion <= 763)
        {
            writer.WriteType<Position>(Location, protocolVersion);
            writer.WriteVarInt(Action);
            writer.WriteBoolean(NbtData is not null);
            if (NbtData is { } nbtDataValue)
                writer.WriteNbt(nbtDataValue, true);
            return;
        }

        if (protocolVersion >= 764)
        {
            writer.WriteType<Position>(Location, protocolVersion);
            writer.WriteVarInt(Action);
            writer.WriteBoolean(NbtData is not null);
            if (NbtData is { } nbtDataValue)
                writer.WriteNbt(nbtDataValue);
            return;
        }

        throw new System.NotSupportedException($"TileEntityDataPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public static PacketIdentity Identity => new("play.toClient.tile_entity_data", "TileEntityData", PacketPhase.Play, PacketDirection.Clientbound, 100);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 735 && protocolVersion <= 736)
        {
            id = 0x09;
            return true;
        }

        if (protocolVersion >= 751 && protocolVersion <= 754)
        {
            id = 0x09;
            return true;
        }

        if (protocolVersion >= 755 && protocolVersion <= 758)
        {
            id = 0x0A;
            return true;
        }

        if (protocolVersion >= 759 && protocolVersion <= 761)
        {
            id = 0x07;
            return true;
        }

        if (protocolVersion >= 762 && protocolVersion <= 763)
        {
            id = 0x08;
            return true;
        }

        if (protocolVersion >= 764 && protocolVersion <= 769)
        {
            id = 0x07;
            return true;
        }

        if (protocolVersion >= 770 && protocolVersion <= 772)
        {
            id = 0x06;
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
