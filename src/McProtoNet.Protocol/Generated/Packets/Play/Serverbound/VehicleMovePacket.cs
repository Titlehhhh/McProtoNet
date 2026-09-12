using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
using System.Text.Json;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;

[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toServer.vehicle_move", PacketPhase.Play, PacketDirection.Serverbound)]
[PacketField("X", "double")]
[PacketField("Y", "double")]
[PacketField("Z", "double")]
[PacketField("Yaw", "float")]
[PacketField("Pitch", "float")]
[PacketField("OnGround", "bool", Group = "V769_Last", From = 769)]
public sealed partial record VehicleMovePacket(double X, double Y, double Z, float Yaw, float Pitch, VehicleMovePacket.V769_LastLayer? V769_Last = null) : IPacket<VehicleMovePacket>, IPacket
{
    public readonly record struct V769_LastLayer(bool OnGround);
    public static VehicleMovePacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<VehicleMovePacket>(protocolVersion);
        if (protocolVersion <= 768)
        {
            var x = reader.ReadDouble();
            var y = reader.ReadDouble();
            var z = reader.ReadDouble();
            var yaw = reader.ReadFloat();
            var pitch = reader.ReadFloat();
            return new VehicleMovePacket(x, y, z, yaw, pitch);
        }

        if (protocolVersion >= 769)
        {
            var x = reader.ReadDouble();
            var y = reader.ReadDouble();
            var z = reader.ReadDouble();
            var yaw = reader.ReadFloat();
            var pitch = reader.ReadFloat();
            var onGround = reader.ReadBoolean();
            return new VehicleMovePacket(x, y, z, yaw, pitch, V769_Last: new V769_LastLayer(onGround));
        }

        throw new System.NotSupportedException($"VehicleMovePacket has no wire layout for protocol version {protocolVersion}.");
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<VehicleMovePacket>(protocolVersion);
        if (protocolVersion <= 768)
        {
            writer.WriteDouble(X);
            writer.WriteDouble(Y);
            writer.WriteDouble(Z);
            writer.WriteFloat(Yaw);
            writer.WriteFloat(Pitch);
            return;
        }

        if (protocolVersion >= 769)
        {
            var layer = V769_Last ?? throw new WrongLayerException("VehicleMovePacket", protocolVersion, "V769_Last");
            bool OnGround = layer.OnGround;
            writer.WriteDouble(X);
            writer.WriteDouble(Y);
            writer.WriteDouble(Z);
            writer.WriteFloat(Yaw);
            writer.WriteFloat(Pitch);
            writer.WriteBoolean(OnGround);
            return;
        }

        throw new System.NotSupportedException($"VehicleMovePacket has no wire layout for protocol version {protocolVersion}.");
    }

    public void WriteJson(Utf8JsonWriter writer)
    {
        writer.WriteStartObject();
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
        if (double.IsFinite(Yaw))
            writer.WriteNumberValue(Yaw);
        else
            writer.WriteStringValue(double.IsNaN(Yaw) ? "NaN" : Yaw > 0 ? "Infinity" : "-Infinity");
        writer.WritePropertyName("Pitch");
        if (double.IsFinite(Pitch))
            writer.WriteNumberValue(Pitch);
        else
            writer.WriteStringValue(double.IsNaN(Pitch) ? "NaN" : Pitch > 0 ? "Infinity" : "-Infinity");
        if (V769_Last is { } v769_Last)
        {
            writer.WritePropertyName("OnGround");
            writer.WriteBooleanValue(v769_Last.OnGround);
        }

        writer.WriteEndObject();
    }

    public static PacketIdentity Identity => new("play.toServer.vehicle_move", "VehicleMove", PacketPhase.Play, PacketDirection.Serverbound, 68);

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
