using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
using System.Text.Json;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;

[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toServer.set_difficulty", PacketPhase.Play, PacketDirection.Serverbound)]
[PacketField("NewDifficulty", "Difficulty")]
public sealed partial record SetDifficultyPacket(Difficulty NewDifficulty) : IPacket<SetDifficultyPacket>, IPacket
{
    public static SetDifficultyPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<SetDifficultyPacket>(protocolVersion);
        if (protocolVersion <= 770)
        {
            var newDifficulty = new Difficulty((int)reader.ReadUnsignedByte());
            return new SetDifficultyPacket(newDifficulty);
        }

        if (protocolVersion >= 771)
        {
            var newDifficulty = reader.ReadType<Difficulty>(protocolVersion);
            return new SetDifficultyPacket(newDifficulty);
        }

        throw new System.NotSupportedException($"SetDifficultyPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<SetDifficultyPacket>(protocolVersion);
        if (protocolVersion <= 770)
        {
            writer.WriteUnsignedByte((byte)NewDifficulty.Value);
            return;
        }

        if (protocolVersion >= 771)
        {
            writer.WriteType<Difficulty>(NewDifficulty, protocolVersion);
            return;
        }

        throw new System.NotSupportedException($"SetDifficultyPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public void WriteJson(Utf8JsonWriter writer)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("NewDifficulty");
        writer.WriteStringValue(NewDifficulty.ToString());
        writer.WriteEndObject();
    }

    public static PacketIdentity Identity => new("play.toServer.set_difficulty", "SetDifficulty", PacketPhase.Play, PacketDirection.Serverbound, 49);

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
