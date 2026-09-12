using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
using System.Text.Json;

namespace McProtoNet.Protocol.Packets.Configuration.Clientbound;

[ProtocolSupport(764, MinecraftVersion.LatestProtocol)]
[Packet("configuration.toClient.ping", PacketPhase.Configuration, PacketDirection.Clientbound)]
[PacketField("Id", "int")]
public sealed partial record PingPacket(int Id) : IPacket<PingPacket>, IPacket
{
    public static PingPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<PingPacket>(protocolVersion);
        var id = reader.ReadSignedInt();
        return new PingPacket(id);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<PingPacket>(protocolVersion);
        writer.WriteSignedInt(Id);
    }

    public void WriteJson(Utf8JsonWriter writer)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("Id");
        writer.WriteNumberValue(Id);
        writer.WriteEndObject();
    }

    public static PacketIdentity Identity => new("configuration.toClient.ping", "Ping", PacketPhase.Configuration, PacketDirection.Clientbound, 10);

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
