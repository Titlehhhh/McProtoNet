using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
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

    public static PacketIdentity Identity => new("play.toClient.tile_entity_data", "TileEntityData", PacketPhase.Play, PacketDirection.Clientbound, 115);

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
