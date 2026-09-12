using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
using System.Text.Json;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.entity_move_look", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("EntityId", "int")]
[PacketField("Dx", "int")]
[PacketField("Dy", "int")]
[PacketField("Dz", "int")]
[PacketField("Yaw", "int")]
[PacketField("Pitch", "int")]
[PacketField("OnGround", "bool")]
public sealed partial record EntityMoveLookPacket(int EntityId, int Dx, int Dy, int Dz, int Yaw, int Pitch, bool OnGround) : IPacket<EntityMoveLookPacket>, IPacket
{
    public static EntityMoveLookPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<EntityMoveLookPacket>(protocolVersion);
        var entityId = reader.ReadVarInt();
        var dx = reader.ReadSignedShort();
        var dy = reader.ReadSignedShort();
        var dz = reader.ReadSignedShort();
        var yaw = reader.ReadSignedByte();
        var pitch = reader.ReadSignedByte();
        var onGround = reader.ReadBoolean();
        return new EntityMoveLookPacket(entityId, dx, dy, dz, yaw, pitch, onGround);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<EntityMoveLookPacket>(protocolVersion);
        writer.WriteVarInt(EntityId);
        writer.WriteSignedShort((short)Dx);
        writer.WriteSignedShort((short)Dy);
        writer.WriteSignedShort((short)Dz);
        writer.WriteSignedByte((sbyte)Yaw);
        writer.WriteSignedByte((sbyte)Pitch);
        writer.WriteBoolean(OnGround);
    }

    public void WriteJson(Utf8JsonWriter writer)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("EntityId");
        writer.WriteNumberValue(EntityId);
        writer.WritePropertyName("Dx");
        writer.WriteNumberValue(Dx);
        writer.WritePropertyName("Dy");
        writer.WriteNumberValue(Dy);
        writer.WritePropertyName("Dz");
        writer.WriteNumberValue(Dz);
        writer.WritePropertyName("Yaw");
        writer.WriteNumberValue(Yaw);
        writer.WritePropertyName("Pitch");
        writer.WriteNumberValue(Pitch);
        writer.WritePropertyName("OnGround");
        writer.WriteBooleanValue(OnGround);
        writer.WriteEndObject();
    }

    public static PacketIdentity Identity => new("play.toClient.entity_move_look", "EntityMoveLook", PacketPhase.Play, PacketDirection.Clientbound, 38);

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
