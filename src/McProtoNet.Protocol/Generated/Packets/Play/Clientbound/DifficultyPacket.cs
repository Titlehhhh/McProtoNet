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

    public static PacketIdentity Identity => new("play.toClient.difficulty", "Difficulty", PacketPhase.Play, PacketDirection.Clientbound, 30);

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
