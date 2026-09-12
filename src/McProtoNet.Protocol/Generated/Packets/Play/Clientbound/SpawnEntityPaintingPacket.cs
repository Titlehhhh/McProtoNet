using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
using System.Text.Json;
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

    public void WriteJson(Utf8JsonWriter writer)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("EntityId");
        writer.WriteNumberValue(EntityId);
        writer.WritePropertyName("EntityUuid");
        writer.WriteStringValue(EntityUuid);
        writer.WritePropertyName("Title");
        writer.WriteNumberValue(Title);
        writer.WritePropertyName("Location");
        Location.WriteJson(writer);
        writer.WritePropertyName("Direction");
        writer.WriteNumberValue(Direction);
        writer.WriteEndObject();
    }

    public static PacketIdentity Identity => new("play.toClient.spawn_entity_painting", "SpawnEntityPainting", PacketPhase.Play, PacketDirection.Clientbound, 102);

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
