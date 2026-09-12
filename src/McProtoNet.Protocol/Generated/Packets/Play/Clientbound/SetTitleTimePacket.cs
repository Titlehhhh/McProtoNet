using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
using System.Text.Json;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[ProtocolSupport(755, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.set_title_time", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("FadeIn", "int")]
[PacketField("Stay", "int")]
[PacketField("FadeOut", "int")]
public sealed partial record SetTitleTimePacket(int FadeIn, int Stay, int FadeOut) : IPacket<SetTitleTimePacket>, IPacket
{
    public static SetTitleTimePacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<SetTitleTimePacket>(protocolVersion);
        var fadeIn = reader.ReadSignedInt();
        var stay = reader.ReadSignedInt();
        var fadeOut = reader.ReadSignedInt();
        return new SetTitleTimePacket(fadeIn, stay, fadeOut);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<SetTitleTimePacket>(protocolVersion);
        writer.WriteSignedInt(FadeIn);
        writer.WriteSignedInt(Stay);
        writer.WriteSignedInt(FadeOut);
    }

    public void WriteJson(Utf8JsonWriter writer)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("FadeIn");
        writer.WriteNumberValue(FadeIn);
        writer.WritePropertyName("Stay");
        writer.WriteNumberValue(Stay);
        writer.WritePropertyName("FadeOut");
        writer.WriteNumberValue(FadeOut);
        writer.WriteEndObject();
    }

    public static PacketIdentity Identity => new("play.toClient.set_title_time", "SetTitleTime", PacketPhase.Play, PacketDirection.Clientbound, 97);

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
