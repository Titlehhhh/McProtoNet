using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
using System.Text.Json;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;

[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toServer.lock_difficulty", PacketPhase.Play, PacketDirection.Serverbound)]
[PacketField("Locked", "bool")]
public sealed partial record LockDifficultyPacket(bool Locked) : IPacket<LockDifficultyPacket>, IPacket
{
    public static LockDifficultyPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<LockDifficultyPacket>(protocolVersion);
        var locked = reader.ReadBoolean();
        return new LockDifficultyPacket(locked);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<LockDifficultyPacket>(protocolVersion);
        writer.WriteBoolean(Locked);
    }

    public void WriteJson(Utf8JsonWriter writer)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("Locked");
        writer.WriteBooleanValue(Locked);
        writer.WriteEndObject();
    }

    public static PacketIdentity Identity => new("play.toServer.lock_difficulty", "LockDifficulty", PacketPhase.Play, PacketDirection.Serverbound, 30);

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
