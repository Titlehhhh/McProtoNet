using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toServer.set_difficulty", PacketPhase.Play, PacketDirection.Serverbound)]
[PacketField("NewDifficulty", "int")]
public sealed partial record SetDifficultyPacket(int NewDifficulty) : IPacket<SetDifficultyPacket>, IPacket
{
    public static SetDifficultyPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<SetDifficultyPacket>(protocolVersion);
        if (protocolVersion <= 770)
        {
            var newDifficulty = reader.ReadUnsignedByte();
            return new SetDifficultyPacket(newDifficulty);
        }

        if (protocolVersion >= 771)
        {
            var newDifficulty = reader.ReadVarInt();
            return new SetDifficultyPacket(newDifficulty);
        }

        throw new System.NotSupportedException($"SetDifficultyPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<SetDifficultyPacket>(protocolVersion);
        if (protocolVersion <= 770)
        {
            writer.WriteUnsignedByte((byte)NewDifficulty);
            return;
        }

        if (protocolVersion >= 771)
        {
            writer.WriteVarInt(NewDifficulty);
            return;
        }

        throw new System.NotSupportedException($"SetDifficultyPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public static PacketIdentity Identity => new("play.toServer.set_difficulty", "SetDifficulty", PacketPhase.Play, PacketDirection.Serverbound, 44);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 735 && protocolVersion <= 736)
        {
            id = 0x02;
            return true;
        }

        if (protocolVersion >= 751 && protocolVersion <= 767)
        {
            id = 0x02;
            return true;
        }

        if (protocolVersion >= 768 && protocolVersion <= 772)
        {
            id = 0x03;
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
