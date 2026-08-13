using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.abilities", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("Flags", "int")]
[PacketField("FlyingSpeed", "float")]
[PacketField("WalkingSpeed", "float")]
public sealed partial record AbilitiesPacket(int Flags, float FlyingSpeed, float WalkingSpeed) : IPacket<AbilitiesPacket>, IPacket
{
    public static AbilitiesPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<AbilitiesPacket>(protocolVersion);
        var flags = reader.ReadSignedByte();
        var flyingSpeed = reader.ReadFloat();
        var walkingSpeed = reader.ReadFloat();
        return new AbilitiesPacket(flags, flyingSpeed, walkingSpeed);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<AbilitiesPacket>(protocolVersion);
        writer.WriteSignedByte((sbyte)Flags);
        writer.WriteFloat(FlyingSpeed);
        writer.WriteFloat(WalkingSpeed);
    }

    public static PacketIdentity Identity => new("play.toClient.abilities", "Abilities", PacketPhase.Play, PacketDirection.Clientbound, 0);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 735 && protocolVersion <= 736)
        {
            id = 0x31;
            return true;
        }

        if (protocolVersion >= 751 && protocolVersion <= 754)
        {
            id = 0x30;
            return true;
        }

        if (protocolVersion >= 755 && protocolVersion <= 758)
        {
            id = 0x32;
            return true;
        }

        if (protocolVersion >= 759 && protocolVersion <= 759)
        {
            id = 0x2F;
            return true;
        }

        if (protocolVersion >= 760 && protocolVersion <= 760)
        {
            id = 0x31;
            return true;
        }

        if (protocolVersion >= 761 && protocolVersion <= 761)
        {
            id = 0x30;
            return true;
        }

        if (protocolVersion >= 762 && protocolVersion <= 763)
        {
            id = 0x34;
            return true;
        }

        if (protocolVersion >= 764 && protocolVersion <= 765)
        {
            id = 0x36;
            return true;
        }

        if (protocolVersion >= 766 && protocolVersion <= 767)
        {
            id = 0x38;
            return true;
        }

        if (protocolVersion >= 768 && protocolVersion <= 769)
        {
            id = 0x3A;
            return true;
        }

        if (protocolVersion >= 770 && protocolVersion <= 772)
        {
            id = 0x39;
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
