using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
using System.Text.Json;
using McProtoNet.NBT;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[ProtocolSupport(755, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.set_title_text", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("TextJson", "string", Group = "V755_764", From = 755, To = 764)]
[PacketField("Text", "NbtTag", Group = "V765_Last", From = 765)]
public sealed partial record SetTitleTextPacket(SetTitleTextPacket.V755_764Layer? V755_764 = null, SetTitleTextPacket.V765_LastLayer? V765_Last = null) : IPacket<SetTitleTextPacket>, IPacket
{
    public readonly record struct V755_764Layer(string TextJson);
    public readonly record struct V765_LastLayer(NbtTag Text);
    public static SetTitleTextPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<SetTitleTextPacket>(protocolVersion);
        if (protocolVersion >= 755 && protocolVersion <= 764)
        {
            var textJson = reader.ReadString();
            return new SetTitleTextPacket(V755_764: new V755_764Layer(textJson));
        }

        if (protocolVersion >= 765)
        {
            var text = reader.ReadNbtTag(false)!;
            return new SetTitleTextPacket(V765_Last: new V765_LastLayer(text));
        }

        throw new System.NotSupportedException($"SetTitleTextPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<SetTitleTextPacket>(protocolVersion);
        if (protocolVersion >= 755 && protocolVersion <= 764)
        {
            var layer = V755_764 ?? throw new WrongLayerException("SetTitleTextPacket", protocolVersion, "V755_764");
            string TextJson = layer.TextJson;
            writer.WriteString(TextJson);
            return;
        }

        if (protocolVersion >= 765)
        {
            var layer = V765_Last ?? throw new WrongLayerException("SetTitleTextPacket", protocolVersion, "V765_Last");
            NbtTag Text = layer.Text;
            writer.WriteNbt(Text);
            return;
        }

        throw new System.NotSupportedException($"SetTitleTextPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public void WriteJson(Utf8JsonWriter writer)
    {
        writer.WriteStartObject();
        if (V755_764 is { } v755_764)
        {
            writer.WritePropertyName("TextJson");
            writer.WriteStringValue(v755_764.TextJson);
        }
        else if (V765_Last is { } v765_Last)
        {
            writer.WritePropertyName("Text");
            v765_Last.Text.WriteJson(writer);
        }

        writer.WriteEndObject();
    }

    public static PacketIdentity Identity => new("play.toClient.set_title_text", "SetTitleText", PacketPhase.Play, PacketDirection.Clientbound, 96);

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
