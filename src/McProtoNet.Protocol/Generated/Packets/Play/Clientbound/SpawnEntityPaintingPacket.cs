using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;
[ProtocolSupport(MinecraftVersion.StartProtocol, 758)]
[Packet("play.toClient.spawn_entity_painting", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("EntityId", "int")]
[PacketField("EntityUuid", "Guid")]
[PacketField("Title", "int")]
[PacketField("Location", "Position")]
[PacketField("Direction", "int")]
public sealed partial record SpawnEntityPaintingPacket(int EntityId, Guid EntityUuid, int Title, Position Location, int Direction) : IPacket<SpawnEntityPaintingPacket>, IPacket
{
    public static SpawnEntityPaintingPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<SpawnEntityPaintingPacket>(protocolVersion);
        var entityId = reader.ReadVarInt();
        var entityUuid = reader.ReadUUID();
        var title = reader.ReadVarInt();
        var location = reader.ReadType<Position>(protocolVersion);
        var direction = reader.ReadUnsignedByte();
        return new SpawnEntityPaintingPacket(entityId, entityUuid, title, location, direction);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<SpawnEntityPaintingPacket>(protocolVersion);
        writer.WriteVarInt(EntityId);
        writer.WriteUUID(EntityUuid);
        writer.WriteVarInt(Title);
        writer.WriteType<Position>(Location, protocolVersion);
        writer.WriteUnsignedByte((byte)Direction);
    }

    public static PacketIdentity Identity => new("play.toClient.spawn_entity_painting", "SpawnEntityPainting", PacketPhase.Play, PacketDirection.Clientbound, 92);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 735 && protocolVersion <= 736)
        {
            id = 0x03;
            return true;
        }

        if (protocolVersion >= 751 && protocolVersion <= 758)
        {
            id = 0x03;
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
