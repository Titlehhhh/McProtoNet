using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
using System.Text.Json;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.open_sign_entity", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("Location", "Position")]
[PacketField("IsFrontText", "bool", Group = "V763_Last", From = 763)]
public sealed partial record OpenSignEntityPacket(Position Location, OpenSignEntityPacket.V763_LastLayer? V763_Last = null) : IPacket<OpenSignEntityPacket>, IPacket
{
    public readonly record struct V763_LastLayer(bool IsFrontText);
    public static OpenSignEntityPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<OpenSignEntityPacket>(protocolVersion);
        if (protocolVersion <= 762)
        {
            var location = reader.ReadType<Position>(protocolVersion);
            return new OpenSignEntityPacket(location);
        }

        if (protocolVersion >= 763)
        {
            var location = reader.ReadType<Position>(protocolVersion);
            var isFrontText = reader.ReadBoolean();
            return new OpenSignEntityPacket(location, V763_Last: new V763_LastLayer(isFrontText));
        }

        throw new System.NotSupportedException($"OpenSignEntityPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<OpenSignEntityPacket>(protocolVersion);
        if (protocolVersion <= 762)
        {
            writer.WriteType<Position>(Location, protocolVersion);
            return;
        }

        if (protocolVersion >= 763)
        {
            var layer = V763_Last ?? throw new WrongLayerException("OpenSignEntityPacket", protocolVersion, "V763_Last");
            bool IsFrontText = layer.IsFrontText;
            writer.WriteType<Position>(Location, protocolVersion);
            writer.WriteBoolean(IsFrontText);
            return;
        }

        throw new System.NotSupportedException($"OpenSignEntityPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public void WriteJson(Utf8JsonWriter writer)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("Location");
        Location.WriteJson(writer);
        if (V763_Last is { } v763_Last)
        {
            writer.WritePropertyName("IsFrontText");
            writer.WriteBooleanValue(v763_Last.IsFrontText);
        }

        writer.WriteEndObject();
    }

    public static PacketIdentity Identity => new("play.toClient.open_sign_entity", "OpenSignEntity", PacketPhase.Play, PacketDirection.Clientbound, 67);

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
