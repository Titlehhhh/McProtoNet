using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
using System.Text.Json;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.game_state_change", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("Reason", "int")]
[PacketField("GameMode", "float")]
public sealed partial record GameStateChangePacket(int Reason, float GameMode) : IPacket<GameStateChangePacket>, IPacket
{
    public static GameStateChangePacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<GameStateChangePacket>(protocolVersion);
        var reason = reader.ReadUnsignedByte();
        var gameMode = reader.ReadFloat();
        return new GameStateChangePacket(reason, gameMode);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<GameStateChangePacket>(protocolVersion);
        writer.WriteUnsignedByte((byte)Reason);
        writer.WriteFloat(GameMode);
    }

    public void WriteJson(Utf8JsonWriter writer)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("Reason");
        writer.WriteNumberValue(Reason);
        writer.WritePropertyName("GameMode");
        if (double.IsFinite(GameMode))
            writer.WriteNumberValue(GameMode);
        else
            writer.WriteStringValue(double.IsNaN(GameMode) ? "NaN" : GameMode > 0 ? "Infinity" : "-Infinity");
        writer.WriteEndObject();
    }

    public static PacketIdentity Identity => new("play.toClient.game_state_change", "GameStateChange", PacketPhase.Play, PacketDirection.Clientbound, 48);

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
