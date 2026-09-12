using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
using System.Text.Json;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[ProtocolSupport(768, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.sync_entity_position", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("EntityId", "int")]
[PacketField("X", "double")]
[PacketField("Y", "double")]
[PacketField("Z", "double")]
[PacketField("Dx", "double")]
[PacketField("Dy", "double")]
[PacketField("Dz", "double")]
[PacketField("Yaw", "float")]
[PacketField("Pitch", "float")]
[PacketField("OnGround", "bool")]
public sealed partial record SyncEntityPositionPacket(int EntityId, double X, double Y, double Z, double Dx, double Dy, double Dz, float Yaw, float Pitch, bool OnGround) : IPacket<SyncEntityPositionPacket>, IPacket
{
    public static SyncEntityPositionPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<SyncEntityPositionPacket>(protocolVersion);
        var entityId = reader.ReadVarInt();
        var x = reader.ReadDouble();
        var y = reader.ReadDouble();
        var z = reader.ReadDouble();
        var dx = reader.ReadDouble();
        var dy = reader.ReadDouble();
        var dz = reader.ReadDouble();
        var yaw = reader.ReadFloat();
        var pitch = reader.ReadFloat();
        var onGround = reader.ReadBoolean();
        return new SyncEntityPositionPacket(entityId, x, y, z, dx, dy, dz, yaw, pitch, onGround);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<SyncEntityPositionPacket>(protocolVersion);
        writer.WriteVarInt(EntityId);
        writer.WriteDouble(X);
        writer.WriteDouble(Y);
        writer.WriteDouble(Z);
        writer.WriteDouble(Dx);
        writer.WriteDouble(Dy);
        writer.WriteDouble(Dz);
        writer.WriteFloat(Yaw);
        writer.WriteFloat(Pitch);
        writer.WriteBoolean(OnGround);
    }

    public void WriteJson(Utf8JsonWriter writer)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("EntityId");
        writer.WriteNumberValue(EntityId);
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
        writer.WritePropertyName("Dx");
        if (double.IsFinite(Dx))
            writer.WriteNumberValue(Dx);
        else
            writer.WriteStringValue(double.IsNaN(Dx) ? "NaN" : Dx > 0 ? "Infinity" : "-Infinity");
        writer.WritePropertyName("Dy");
        if (double.IsFinite(Dy))
            writer.WriteNumberValue(Dy);
        else
            writer.WriteStringValue(double.IsNaN(Dy) ? "NaN" : Dy > 0 ? "Infinity" : "-Infinity");
        writer.WritePropertyName("Dz");
        if (double.IsFinite(Dz))
            writer.WriteNumberValue(Dz);
        else
            writer.WriteStringValue(double.IsNaN(Dz) ? "NaN" : Dz > 0 ? "Infinity" : "-Infinity");
        writer.WritePropertyName("Yaw");
        if (double.IsFinite(Yaw))
            writer.WriteNumberValue(Yaw);
        else
            writer.WriteStringValue(double.IsNaN(Yaw) ? "NaN" : Yaw > 0 ? "Infinity" : "-Infinity");
        writer.WritePropertyName("Pitch");
        if (double.IsFinite(Pitch))
            writer.WriteNumberValue(Pitch);
        else
            writer.WriteStringValue(double.IsNaN(Pitch) ? "NaN" : Pitch > 0 ? "Infinity" : "-Infinity");
        writer.WritePropertyName("OnGround");
        writer.WriteBooleanValue(OnGround);
        writer.WriteEndObject();
    }

    public static PacketIdentity Identity => new("play.toClient.sync_entity_position", "SyncEntityPosition", PacketPhase.Play, PacketDirection.Clientbound, 109);

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
