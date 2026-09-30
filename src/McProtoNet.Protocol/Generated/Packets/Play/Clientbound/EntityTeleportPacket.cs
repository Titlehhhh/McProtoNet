using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
using System.Text.Json;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.entity_teleport", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("EntityId", "int")]
[PacketField("X", "double")]
[PacketField("Y", "double")]
[PacketField("Z", "double")]
[PacketField("OnGround", "bool")]
[PacketField("YawByte", "int", Group = "VUntil767", To = 767)]
[PacketField("PitchByte", "int", Group = "VUntil767", To = 767)]
[PacketField("Dx", "double", Group = "V768_Last", From = 768)]
[PacketField("Dy", "double", Group = "V768_Last", From = 768)]
[PacketField("Dz", "double", Group = "V768_Last", From = 768)]
[PacketField("Yaw", "float", Group = "V768_Last", From = 768)]
[PacketField("Pitch", "float", Group = "V768_Last", From = 768)]
[PacketField("Flags", "PositionUpdateRelatives", Group = "V768_Last", From = 768)]
public sealed partial record EntityTeleportPacket(int EntityId, double X, double Y, double Z, bool OnGround, EntityTeleportPacket.VUntil767Layer? VUntil767 = null, EntityTeleportPacket.V768_LastLayer? V768_Last = null) : IPacket<EntityTeleportPacket>, IPacket
{
    public readonly record struct VUntil767Layer(int YawByte, int PitchByte);
    public readonly record struct V768_LastLayer(double Dx, double Dy, double Dz, float Yaw, float Pitch, PositionUpdateRelatives Flags);
    public static EntityTeleportPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<EntityTeleportPacket>(protocolVersion);
        if (protocolVersion <= 767)
        {
            var entityId = reader.ReadVarInt();
            var x = reader.ReadDouble();
            var y = reader.ReadDouble();
            var z = reader.ReadDouble();
            var yawByte = reader.ReadSignedByte();
            var pitchByte = reader.ReadSignedByte();
            var onGround = reader.ReadBoolean();
            return new EntityTeleportPacket(entityId, x, y, z, onGround, VUntil767: new VUntil767Layer(yawByte, pitchByte));
        }

        if (protocolVersion >= 768)
        {
            var entityId = reader.ReadVarInt();
            var x = reader.ReadDouble();
            var y = reader.ReadDouble();
            var z = reader.ReadDouble();
            var dx = reader.ReadDouble();
            var dy = reader.ReadDouble();
            var dz = reader.ReadDouble();
            var yaw = reader.ReadFloat();
            var pitch = reader.ReadFloat();
            var flags = reader.ReadType<PositionUpdateRelatives>(protocolVersion);
            var onGround = reader.ReadBoolean();
            return new EntityTeleportPacket(entityId, x, y, z, onGround, V768_Last: new V768_LastLayer(dx, dy, dz, yaw, pitch, flags));
        }

        throw new System.NotSupportedException($"EntityTeleportPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<EntityTeleportPacket>(protocolVersion);
        if (protocolVersion <= 767)
        {
            var layer = VUntil767 ?? throw new WrongLayerException("EntityTeleportPacket", protocolVersion, "VUntil767");
            int YawByte = layer.YawByte;
            int PitchByte = layer.PitchByte;
            writer.WriteVarInt(EntityId);
            writer.WriteDouble(X);
            writer.WriteDouble(Y);
            writer.WriteDouble(Z);
            writer.WriteSignedByte((sbyte)YawByte);
            writer.WriteSignedByte((sbyte)PitchByte);
            writer.WriteBoolean(OnGround);
            return;
        }

        if (protocolVersion >= 768)
        {
            var layer = V768_Last ?? throw new WrongLayerException("EntityTeleportPacket", protocolVersion, "V768_Last");
            double Dx = layer.Dx;
            double Dy = layer.Dy;
            double Dz = layer.Dz;
            float Yaw = layer.Yaw;
            float Pitch = layer.Pitch;
            PositionUpdateRelatives Flags = layer.Flags;
            writer.WriteVarInt(EntityId);
            writer.WriteDouble(X);
            writer.WriteDouble(Y);
            writer.WriteDouble(Z);
            writer.WriteDouble(Dx);
            writer.WriteDouble(Dy);
            writer.WriteDouble(Dz);
            writer.WriteFloat(Yaw);
            writer.WriteFloat(Pitch);
            writer.WriteType<PositionUpdateRelatives>(Flags, protocolVersion);
            writer.WriteBoolean(OnGround);
            return;
        }

        throw new System.NotSupportedException($"EntityTeleportPacket has no wire layout for protocol version {protocolVersion}.");
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
        writer.WritePropertyName("OnGround");
        writer.WriteBooleanValue(OnGround);
        if (VUntil767 is { } vUntil767)
        {
            writer.WritePropertyName("YawByte");
            writer.WriteNumberValue(vUntil767.YawByte);
            writer.WritePropertyName("PitchByte");
            writer.WriteNumberValue(vUntil767.PitchByte);
        }
        else if (V768_Last is { } v768_Last)
        {
            writer.WritePropertyName("Dx");
            if (double.IsFinite(v768_Last.Dx))
                writer.WriteNumberValue(v768_Last.Dx);
            else
                writer.WriteStringValue(double.IsNaN(v768_Last.Dx) ? "NaN" : v768_Last.Dx > 0 ? "Infinity" : "-Infinity");
            writer.WritePropertyName("Dy");
            if (double.IsFinite(v768_Last.Dy))
                writer.WriteNumberValue(v768_Last.Dy);
            else
                writer.WriteStringValue(double.IsNaN(v768_Last.Dy) ? "NaN" : v768_Last.Dy > 0 ? "Infinity" : "-Infinity");
            writer.WritePropertyName("Dz");
            if (double.IsFinite(v768_Last.Dz))
                writer.WriteNumberValue(v768_Last.Dz);
            else
                writer.WriteStringValue(double.IsNaN(v768_Last.Dz) ? "NaN" : v768_Last.Dz > 0 ? "Infinity" : "-Infinity");
            writer.WritePropertyName("Yaw");
            if (double.IsFinite(v768_Last.Yaw))
                writer.WriteNumberValue(v768_Last.Yaw);
            else
                writer.WriteStringValue(double.IsNaN(v768_Last.Yaw) ? "NaN" : v768_Last.Yaw > 0 ? "Infinity" : "-Infinity");
            writer.WritePropertyName("Pitch");
            if (double.IsFinite(v768_Last.Pitch))
                writer.WriteNumberValue(v768_Last.Pitch);
            else
                writer.WriteStringValue(double.IsNaN(v768_Last.Pitch) ? "NaN" : v768_Last.Pitch > 0 ? "Infinity" : "-Infinity");
            writer.WritePropertyName("Flags");
            v768_Last.Flags.WriteJson(writer);
        }

        writer.WriteEndObject();
    }

    public static PacketIdentity Identity => new("play.toClient.entity_teleport", "EntityTeleport", PacketPhase.Play, PacketDirection.Clientbound, 40);

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
