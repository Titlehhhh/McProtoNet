using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
using System.Text.Json;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;

[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toServer.client_command", PacketPhase.Play, PacketDirection.Serverbound)]
[PacketField("ActionId", "int")]
public sealed partial record ClientCommandPacket(int ActionId) : IPacket<ClientCommandPacket>, IPacket
{
    public static ClientCommandPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ClientCommandPacket>(protocolVersion);
        var actionId = reader.ReadVarInt();
        return new ClientCommandPacket(actionId);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ClientCommandPacket>(protocolVersion);
        writer.WriteVarInt(ActionId);
    }

    public void WriteJson(Utf8JsonWriter writer)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("ActionId");
        writer.WriteNumberValue(ActionId);
        writer.WriteEndObject();
    }

    public static PacketIdentity Identity => new("play.toServer.client_command", "ClientCommand", PacketPhase.Play, PacketDirection.Serverbound, 14);

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
