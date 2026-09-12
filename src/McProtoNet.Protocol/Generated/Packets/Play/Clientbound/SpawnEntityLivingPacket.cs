using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
using System.Text.Json;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[ProtocolSupport(MinecraftVersion.StartProtocol, 758)]
[Packet("play.toClient.spawn_entity_living", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("EntityId", "int")]
[PacketField("EntityUuid", "Guid")]
[PacketField("Type", "int")]
[PacketField("X", "double")]
[PacketField("Y", "double")]
[PacketField("Z", "double")]
[PacketField("Yaw", "int")]
[PacketField("Pitch", "int")]
[PacketField("HeadPitch", "int")]
[PacketField("VelocityX", "int")]
[PacketField("VelocityY", "int")]
[PacketField("VelocityZ", "int")]
public sealed partial record SpawnEntityLivingPacket(int EntityId, Guid EntityUuid, int Type, double X, double Y, double Z, int Yaw, int Pitch, int HeadPitch, int VelocityX, int VelocityY, int VelocityZ) : IPacket<SpawnEntityLivingPacket>, IPacket
{
    public static SpawnEntityLivingPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<SpawnEntityLivingPacket>(protocolVersion);
        var entityId = reader.ReadVarInt();
        var entityUuid = reader.ReadUUID();
        var type = reader.ReadVarInt();
        var x = reader.ReadDouble();
        var y = reader.ReadDouble();
        var z = reader.ReadDouble();
        var yaw = reader.ReadSignedByte();
        var pitch = reader.ReadSignedByte();
        var headPitch = reader.ReadSignedByte();
        var velocityX = reader.ReadSignedShort();
        var velocityY = reader.ReadSignedShort();
        var velocityZ = reader.ReadSignedShort();
        return new SpawnEntityLivingPacket(entityId, entityUuid, type, x, y, z, yaw, pitch, headPitch, velocityX, velocityY, velocityZ);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<SpawnEntityLivingPacket>(protocolVersion);
        writer.WriteVarInt(EntityId);
        writer.WriteUUID(EntityUuid);
        writer.WriteVarInt(Type);
        writer.WriteDouble(X);
        writer.WriteDouble(Y);
        writer.WriteDouble(Z);
        writer.WriteSignedByte((sbyte)Yaw);
        writer.WriteSignedByte((sbyte)Pitch);
        writer.WriteSignedByte((sbyte)HeadPitch);
        writer.WriteSignedShort((short)VelocityX);
        writer.WriteSignedShort((short)VelocityY);
        writer.WriteSignedShort((short)VelocityZ);
    }

    public void WriteJson(Utf8JsonWriter writer)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("EntityId");
        writer.WriteNumberValue(EntityId);
        writer.WritePropertyName("EntityUuid");
        writer.WriteStringValue(EntityUuid);
        writer.WritePropertyName("Type");
        writer.WriteNumberValue(Type);
        writer.WritePropertyName("X");
        if (double.IsFinite(X))
            writer.WriteNumberValue(X);
        else
            writer.WriteStringValue(double.IsNaN(X) ? "NaN" : X > 0 ? "Infinity" : "-Infinity");
        writer.WritePropertyName("Y");
        if (double.IsFinite(Y))
            writer.WriteNumberValue(Y);
        else
            writer.WriteStringValue(double.IsNaN(Y) ? "NaN" : Y > 0 ? "Infinity" : "-Infinity");
        writer.WritePropertyName("Z");
        if (double.IsFinite(Z))
            writer.WriteNumberValue(Z);
        else
            writer.WriteStringValue(double.IsNaN(Z) ? "NaN" : Z > 0 ? "Infinity" : "-Infinity");
        writer.WritePropertyName("Yaw");
        writer.WriteNumberValue(Yaw);
        writer.WritePropertyName("Pitch");
        writer.WriteNumberValue(Pitch);
        writer.WritePropertyName("HeadPitch");
        writer.WriteNumberValue(HeadPitch);
        writer.WritePropertyName("VelocityX");
        writer.WriteNumberValue(VelocityX);
        writer.WritePropertyName("VelocityY");
        writer.WriteNumberValue(VelocityY);
        writer.WritePropertyName("VelocityZ");
        writer.WriteNumberValue(VelocityZ);
        writer.WriteEndObject();
    }

    public static PacketIdentity Identity => new("play.toClient.spawn_entity_living", "SpawnEntityLiving", PacketPhase.Play, PacketDirection.Clientbound, 102);

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
