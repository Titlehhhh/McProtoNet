using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
using System.Text.Json;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;

[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toServer.update_sign", PacketPhase.Play, PacketDirection.Serverbound)]
[PacketField("Location", "Position")]
[PacketField("Text1", "string")]
[PacketField("Text2", "string")]
[PacketField("Text3", "string")]
[PacketField("Text4", "string")]
[PacketField("IsFrontText", "bool", Group = "V763_Last", From = 763)]
public sealed partial record UpdateSignPacket(Position Location, string Text1, string Text2, string Text3, string Text4, UpdateSignPacket.V763_LastLayer? V763_Last = null) : IPacket<UpdateSignPacket>, IPacket
{
    public readonly record struct V763_LastLayer(bool IsFrontText);
    public static UpdateSignPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<UpdateSignPacket>(protocolVersion);
        if (protocolVersion <= 762)
        {
            var location = reader.ReadType<Position>(protocolVersion);
            var text1 = reader.ReadString();
            var text2 = reader.ReadString();
            var text3 = reader.ReadString();
            var text4 = reader.ReadString();
            return new UpdateSignPacket(location, text1, text2, text3, text4);
        }

        if (protocolVersion >= 763)
        {
            var location = reader.ReadType<Position>(protocolVersion);
            var isFrontText = reader.ReadBoolean();
            var text1 = reader.ReadString();
            var text2 = reader.ReadString();
            var text3 = reader.ReadString();
            var text4 = reader.ReadString();
            return new UpdateSignPacket(location, text1, text2, text3, text4, V763_Last: new V763_LastLayer(isFrontText));
        }

        throw new System.NotSupportedException($"UpdateSignPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<UpdateSignPacket>(protocolVersion);
        if (protocolVersion <= 762)
        {
            writer.WriteType<Position>(Location, protocolVersion);
            writer.WriteString(Text1);
            writer.WriteString(Text2);
            writer.WriteString(Text3);
            writer.WriteString(Text4);
            return;
        }

        if (protocolVersion >= 763)
        {
            var layer = V763_Last ?? throw new WrongLayerException("UpdateSignPacket", protocolVersion, "V763_Last");
            bool IsFrontText = layer.IsFrontText;
            writer.WriteType<Position>(Location, protocolVersion);
            writer.WriteBoolean(IsFrontText);
            writer.WriteString(Text1);
            writer.WriteString(Text2);
            writer.WriteString(Text3);
            writer.WriteString(Text4);
            return;
        }

        throw new System.NotSupportedException($"UpdateSignPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public void WriteJson(Utf8JsonWriter writer)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("Location");
        Location.WriteJson(writer);
        writer.WritePropertyName("Text1");
        writer.WriteStringValue(Text1);
        writer.WritePropertyName("Text2");
        writer.WriteStringValue(Text2);
        writer.WritePropertyName("Text3");
        writer.WriteStringValue(Text3);
        writer.WritePropertyName("Text4");
        writer.WriteStringValue(Text4);
        if (V763_Last is { } v763_Last)
        {
            writer.WritePropertyName("IsFrontText");
            writer.WriteBooleanValue(v763_Last.IsFrontText);
        }

        writer.WriteEndObject();
    }

    public static PacketIdentity Identity => new("play.toServer.update_sign", "UpdateSign", PacketPhase.Play, PacketDirection.Serverbound, 64);

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
