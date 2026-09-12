using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
using System.Text.Json;

namespace McProtoNet.Protocol.Packets.Login.Clientbound;

[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("login.toClient.disconnect", PacketPhase.Login, PacketDirection.Clientbound)]
[PacketField("Reason", "string")]
public sealed partial record LoginDisconnectPacket(string Reason) : IPacket<LoginDisconnectPacket>, IPacket
{
    public static LoginDisconnectPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<LoginDisconnectPacket>(protocolVersion);
        var reason = reader.ReadString();
        return new LoginDisconnectPacket(reason);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<LoginDisconnectPacket>(protocolVersion);
        writer.WriteString(Reason);
    }

    public void WriteJson(Utf8JsonWriter writer)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("Reason");
        writer.WriteStringValue(Reason);
        writer.WriteEndObject();
    }

    public static PacketIdentity Identity => new("login.toClient.disconnect", "LoginDisconnect", PacketPhase.Login, PacketDirection.Clientbound, 2);

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
