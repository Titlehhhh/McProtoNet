using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
using System.Text.Json;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;

[ProtocolSupport(769, MinecraftVersion.LatestProtocol)]
[Packet("play.toServer.player_loaded", PacketPhase.Play, PacketDirection.Serverbound)]
public sealed partial record PlayerLoadedPacket() : IPacket<PlayerLoadedPacket>, IPacket
{
    public static PlayerLoadedPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<PlayerLoadedPacket>(protocolVersion);
        return new PlayerLoadedPacket();
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<PlayerLoadedPacket>(protocolVersion);
    }

    public void WriteJson(Utf8JsonWriter writer)
    {
        writer.WriteStartObject();
        writer.WriteEndObject();
    }

    public static PacketIdentity Identity => new("play.toServer.player_loaded", "PlayerLoaded", PacketPhase.Play, PacketDirection.Serverbound, 38);

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
