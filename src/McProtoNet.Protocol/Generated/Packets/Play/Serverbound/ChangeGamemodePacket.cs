using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
using System.Text.Json;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;

[ProtocolSupport(771, MinecraftVersion.LatestProtocol)]
[Packet("play.toServer.change_gamemode", PacketPhase.Play, PacketDirection.Serverbound)]
[PacketField("Mode", "Gamemode")]
public sealed partial record ChangeGamemodePacket(Gamemode Mode) : IPacket<ChangeGamemodePacket>, IPacket
{
    public static ChangeGamemodePacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ChangeGamemodePacket>(protocolVersion);
        var mode = new Gamemode((int)reader.ReadVarInt());
        return new ChangeGamemodePacket(mode);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ChangeGamemodePacket>(protocolVersion);
        writer.WriteVarInt((int)Mode.Value);
    }

    public void WriteJson(Utf8JsonWriter writer)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("Mode");
        writer.WriteStringValue(Mode.ToString());
        writer.WriteEndObject();
    }

    public static PacketIdentity Identity => new("play.toServer.change_gamemode", "ChangeGamemode", PacketPhase.Play, PacketDirection.Serverbound, 6);

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
