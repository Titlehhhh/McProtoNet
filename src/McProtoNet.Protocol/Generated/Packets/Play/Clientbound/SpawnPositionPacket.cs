using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
using System.Text.Json;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.spawn_position", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("Location", "Position", Group = "VUntil754", To = 754)]
[PacketField("Location", "Position", Group = "V755_772", From = 755, To = 772)]
[PacketField("Angle", "float", Group = "V755_772", From = 755, To = 772)]
[PacketField("RespawnData", "RespawnData", Group = "V773_Last", From = 773)]
public sealed partial record SpawnPositionPacket(SpawnPositionPacket.VUntil754Layer? VUntil754 = null, SpawnPositionPacket.V755_772Layer? V755_772 = null, SpawnPositionPacket.V773_LastLayer? V773_Last = null) : IPacket<SpawnPositionPacket>, IPacket
{
    public readonly record struct VUntil754Layer(Position Location);
    public readonly record struct V755_772Layer(Position Location, float Angle);
    public readonly record struct V773_LastLayer(RespawnData RespawnData);
    public static SpawnPositionPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<SpawnPositionPacket>(protocolVersion);
        if (protocolVersion <= 754)
        {
            var location = reader.ReadType<Position>(protocolVersion);
            return new SpawnPositionPacket(VUntil754: new VUntil754Layer(location));
        }

        if (protocolVersion >= 755 && protocolVersion <= 772)
        {
            var location = reader.ReadType<Position>(protocolVersion);
            var angle = reader.ReadFloat();
            return new SpawnPositionPacket(V755_772: new V755_772Layer(location, angle));
        }

        if (protocolVersion >= 773)
        {
            var respawnData = reader.ReadType<RespawnData>(protocolVersion);
            return new SpawnPositionPacket(V773_Last: new V773_LastLayer(respawnData));
        }

        throw new System.NotSupportedException($"SpawnPositionPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<SpawnPositionPacket>(protocolVersion);
        if (protocolVersion <= 754)
        {
            var layer = VUntil754 ?? throw new WrongLayerException("SpawnPositionPacket", protocolVersion, "VUntil754");
            Position Location = layer.Location;
            writer.WriteType<Position>(Location, protocolVersion);
            return;
        }

        if (protocolVersion >= 755 && protocolVersion <= 772)
        {
            var layer = V755_772 ?? throw new WrongLayerException("SpawnPositionPacket", protocolVersion, "V755_772");
            Position Location = layer.Location;
            float Angle = layer.Angle;
            writer.WriteType<Position>(Location, protocolVersion);
            writer.WriteFloat(Angle);
            return;
        }

        if (protocolVersion >= 773)
        {
            var layer = V773_Last ?? throw new WrongLayerException("SpawnPositionPacket", protocolVersion, "V773_Last");
            RespawnData RespawnData = layer.RespawnData;
            writer.WriteType<RespawnData>(RespawnData, protocolVersion);
            return;
        }

        throw new System.NotSupportedException($"SpawnPositionPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public void WriteJson(Utf8JsonWriter writer)
    {
        writer.WriteStartObject();
        if (VUntil754 is { } vUntil754)
        {
            writer.WritePropertyName("Location");
            vUntil754.Location.WriteJson(writer);
        }
        else if (V755_772 is { } v755_772)
        {
            writer.WritePropertyName("Location");
            v755_772.Location.WriteJson(writer);
            writer.WritePropertyName("Angle");
            if (double.IsFinite(v755_772.Angle))
                writer.WriteNumberValue(v755_772.Angle);
            else
                writer.WriteStringValue(double.IsNaN(v755_772.Angle) ? "NaN" : v755_772.Angle > 0 ? "Infinity" : "-Infinity");
        }
        else if (V773_Last is { } v773_Last)
        {
            writer.WritePropertyName("RespawnData");
            v773_Last.RespawnData.WriteJson(writer);
        }

        writer.WriteEndObject();
    }

    public static PacketIdentity Identity => new("play.toClient.spawn_position", "SpawnPosition", PacketPhase.Play, PacketDirection.Clientbound, 103);

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
