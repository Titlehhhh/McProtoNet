using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
using System.Text.Json;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.entity_head_rotation", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("EntityId", "int")]
[PacketField("HeadYaw", "int")]
public sealed partial record EntityHeadRotationPacket(int EntityId, int HeadYaw) : IPacket<EntityHeadRotationPacket>, IPacket
{
    public static EntityHeadRotationPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<EntityHeadRotationPacket>(protocolVersion);
        var entityId = reader.ReadVarInt();
        var headYaw = reader.ReadSignedByte();
        return new EntityHeadRotationPacket(entityId, headYaw);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<EntityHeadRotationPacket>(protocolVersion);
        writer.WriteVarInt(EntityId);
        writer.WriteSignedByte((sbyte)HeadYaw);
    }

    public void WriteJson(Utf8JsonWriter writer)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("EntityId");
        writer.WriteNumberValue(EntityId);
        writer.WritePropertyName("HeadYaw");
        writer.WriteNumberValue(HeadYaw);
        writer.WriteEndObject();
    }

    public static PacketIdentity Identity => new("play.toClient.entity_head_rotation", "EntityHeadRotation", PacketPhase.Play, PacketDirection.Clientbound, 35);

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
