using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
using System.Text.Json;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;

[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toServer.teleport_confirm", PacketPhase.Play, PacketDirection.Serverbound)]
[PacketField("TeleportId", "int")]
public sealed partial record TeleportConfirmPacket(int TeleportId) : IPacket<TeleportConfirmPacket>, IPacket
{
    public static TeleportConfirmPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<TeleportConfirmPacket>(protocolVersion);
        var teleportId = reader.ReadVarInt();
        return new TeleportConfirmPacket(teleportId);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<TeleportConfirmPacket>(protocolVersion);
        writer.WriteVarInt(TeleportId);
    }

    public void WriteJson(Utf8JsonWriter writer)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("TeleportId");
        writer.WriteNumberValue(TeleportId);
        writer.WriteEndObject();
    }

    public static PacketIdentity Identity => new("play.toServer.teleport_confirm", "TeleportConfirm", PacketPhase.Play, PacketDirection.Serverbound, 58);

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
