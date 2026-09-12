using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
using System.Text.Json;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[ProtocolSupport(755, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.world_border_lerp_size", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("OldDiameter", "double")]
[PacketField("NewDiameter", "double")]
[PacketField("Speed", "long")]
public sealed partial record WorldBorderLerpSizePacket(double OldDiameter, double NewDiameter, long Speed) : IPacket<WorldBorderLerpSizePacket>, IPacket
{
    public static WorldBorderLerpSizePacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<WorldBorderLerpSizePacket>(protocolVersion);
        if (protocolVersion >= 755 && protocolVersion <= 758)
        {
            var oldDiameter = reader.ReadDouble();
            var newDiameter = reader.ReadDouble();
            var speed = reader.ReadVarLong();
            return new WorldBorderLerpSizePacket(oldDiameter, newDiameter, speed);
        }

        if (protocolVersion >= 759)
        {
            var oldDiameter = reader.ReadDouble();
            var newDiameter = reader.ReadDouble();
            var speed = reader.ReadVarInt();
            return new WorldBorderLerpSizePacket(oldDiameter, newDiameter, speed);
        }

        throw new System.NotSupportedException($"WorldBorderLerpSizePacket has no wire layout for protocol version {protocolVersion}.");
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<WorldBorderLerpSizePacket>(protocolVersion);
        if (protocolVersion >= 755 && protocolVersion <= 758)
        {
            writer.WriteDouble(OldDiameter);
            writer.WriteDouble(NewDiameter);
            writer.WriteVarLong(Speed);
            return;
        }

        if (protocolVersion >= 759)
        {
            writer.WriteDouble(OldDiameter);
            writer.WriteDouble(NewDiameter);
            writer.WriteVarInt((int)Speed);
            return;
        }

        throw new System.NotSupportedException($"WorldBorderLerpSizePacket has no wire layout for protocol version {protocolVersion}.");
    }

    public void WriteJson(Utf8JsonWriter writer)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("OldDiameter");
        if (double.IsFinite(OldDiameter))
            writer.WriteNumberValue(OldDiameter);
        else
            writer.WriteStringValue(double.IsNaN(OldDiameter) ? "NaN" : OldDiameter > 0 ? "Infinity" : "-Infinity");
        writer.WritePropertyName("NewDiameter");
        if (double.IsFinite(NewDiameter))
            writer.WriteNumberValue(NewDiameter);
        else
            writer.WriteStringValue(double.IsNaN(NewDiameter) ? "NaN" : NewDiameter > 0 ? "Infinity" : "-Infinity");
        writer.WritePropertyName("Speed");
        writer.WriteNumberValue(Speed);
        writer.WriteEndObject();
    }

    public static PacketIdentity Identity => new("play.toClient.world_border_lerp_size", "WorldBorderLerpSize", PacketPhase.Play, PacketDirection.Clientbound, 131);

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
