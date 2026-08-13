using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toServer.abilities", PacketPhase.Play, PacketDirection.Serverbound)]
[PacketField("Flags", "int")]
public sealed partial record AbilitiesPacket(int Flags) : IPacket<AbilitiesPacket>, IPacket
{
    public static AbilitiesPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<AbilitiesPacket>(protocolVersion);
        var flags = reader.ReadSignedByte();
        return new AbilitiesPacket(flags);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<AbilitiesPacket>(protocolVersion);
        writer.WriteSignedByte((sbyte)Flags);
    }

    public static PacketIdentity Identity => new("play.toServer.abilities", "Abilities", PacketPhase.Play, PacketDirection.Serverbound, 0);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 735 && protocolVersion <= 736)
        {
            id = 0x1A;
            return true;
        }

        if (protocolVersion >= 751 && protocolVersion <= 754)
        {
            id = 0x1A;
            return true;
        }

        if (protocolVersion >= 755 && protocolVersion <= 758)
        {
            id = 0x19;
            return true;
        }

        if (protocolVersion >= 759 && protocolVersion <= 759)
        {
            id = 0x1B;
            return true;
        }

        if (protocolVersion >= 760 && protocolVersion <= 760)
        {
            id = 0x1C;
            return true;
        }

        if (protocolVersion >= 761 && protocolVersion <= 761)
        {
            id = 0x1B;
            return true;
        }

        if (protocolVersion >= 762 && protocolVersion <= 763)
        {
            id = 0x1C;
            return true;
        }

        if (protocolVersion >= 764 && protocolVersion <= 764)
        {
            id = 0x1F;
            return true;
        }

        if (protocolVersion >= 765 && protocolVersion <= 765)
        {
            id = 0x20;
            return true;
        }

        if (protocolVersion >= 766 && protocolVersion <= 767)
        {
            id = 0x23;
            return true;
        }

        if (protocolVersion >= 768 && protocolVersion <= 768)
        {
            id = 0x25;
            return true;
        }

        if (protocolVersion >= 769 && protocolVersion <= 770)
        {
            id = 0x26;
            return true;
        }

        if (protocolVersion >= 771 && protocolVersion <= 772)
        {
            id = 0x27;
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
