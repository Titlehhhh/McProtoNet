using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
using System.Text.Json;

namespace McProtoNet.Protocol.Packets.Login.Serverbound;

[ProtocolSupport(764, MinecraftVersion.LatestProtocol)]
[Packet("login.toServer.login_acknowledged", PacketPhase.Login, PacketDirection.Serverbound)]
public sealed partial record LoginAcknowledgedPacket() : IPacket<LoginAcknowledgedPacket>, IPacket
{
    public static LoginAcknowledgedPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<LoginAcknowledgedPacket>(protocolVersion);
        return new LoginAcknowledgedPacket();
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<LoginAcknowledgedPacket>(protocolVersion);
    }

    public void WriteJson(Utf8JsonWriter writer)
    {
        writer.WriteStartObject();
        writer.WriteEndObject();
    }

    public static PacketIdentity Identity => new("login.toServer.login_acknowledged", "LoginAcknowledged", PacketPhase.Login, PacketDirection.Serverbound, 2);

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
