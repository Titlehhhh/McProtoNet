using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
using System.Text.Json;

namespace McProtoNet.Protocol.Packets.Status.Clientbound;

[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("status.toClient.ping", PacketPhase.Status, PacketDirection.Clientbound)]
[PacketField("Time", "long")]
public sealed partial record PongResponsePacket(long Time) : IPacket<PongResponsePacket>, IPacket
{
    public static PongResponsePacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<PongResponsePacket>(protocolVersion);
        var time = reader.ReadSignedLong();
        return new PongResponsePacket(time);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<PongResponsePacket>(protocolVersion);
        writer.WriteSignedLong(Time);
    }

    public void WriteJson(Utf8JsonWriter writer)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("Time");
        writer.WriteNumberValue(Time);
        writer.WriteEndObject();
    }

    public static PacketIdentity Identity => new("status.toClient.ping", "PongResponse", PacketPhase.Status, PacketDirection.Clientbound, 0);

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
