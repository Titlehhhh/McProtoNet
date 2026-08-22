using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.difficulty", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("Difficulty", "Difficulty")]
[PacketField("DifficultyLocked", "bool")]
public sealed partial record DifficultyPacket(Difficulty Difficulty, bool DifficultyLocked) : IPacket<DifficultyPacket>, IPacket
{
    public static DifficultyPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<DifficultyPacket>(protocolVersion);
        if (protocolVersion <= 770)
        {
            var difficulty = new Difficulty((int)reader.ReadUnsignedByte());
            var difficultyLocked = reader.ReadBoolean();
            return new DifficultyPacket(difficulty, difficultyLocked);
        }

        if (protocolVersion >= 771)
        {
            var difficulty = reader.ReadType<Difficulty>(protocolVersion);
            var difficultyLocked = reader.ReadBoolean();
            return new DifficultyPacket(difficulty, difficultyLocked);
        }

        throw new System.NotSupportedException($"DifficultyPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<DifficultyPacket>(protocolVersion);
        if (protocolVersion <= 770)
        {
            writer.WriteUnsignedByte((byte)Difficulty.Value);
            writer.WriteBoolean(DifficultyLocked);
            return;
        }

        if (protocolVersion >= 771)
        {
            writer.WriteType<Difficulty>(Difficulty, protocolVersion);
            writer.WriteBoolean(DifficultyLocked);
            return;
        }

        throw new System.NotSupportedException($"DifficultyPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public static PacketIdentity Identity => new("play.toClient.difficulty", "Difficulty", PacketPhase.Play, PacketDirection.Clientbound, 28);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 735 && protocolVersion <= 736)
        {
            id = 0x0D;
            return true;
        }

        if (protocolVersion >= 751 && protocolVersion <= 754)
        {
            id = 0x0D;
            return true;
        }

        if (protocolVersion >= 755 && protocolVersion <= 758)
        {
            id = 0x0E;
            return true;
        }

        if (protocolVersion >= 759 && protocolVersion <= 761)
        {
            id = 0x0B;
            return true;
        }

        if (protocolVersion >= 762 && protocolVersion <= 763)
        {
            id = 0x0C;
            return true;
        }

        if (protocolVersion >= 764 && protocolVersion <= 769)
        {
            id = 0x0B;
            return true;
        }

        if (protocolVersion >= 770 && protocolVersion <= 776)
        {
            id = 0x0A;
            return true;
        }

        id = 0;
        return false;
    }

    public static int GetPacketId(int protocolVersion)
    {
        if (TryGetPacketId(protocolVersion, out var id))
            return id;
        throw new System.NotSupportedException($"No packet id for protocol {protocolVersion}.");
    }
}
