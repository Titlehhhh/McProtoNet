using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.update_health", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("Health", "float")]
[PacketField("Food", "int")]
[PacketField("FoodSaturation", "float")]
public sealed partial record UpdateHealthPacket(float Health, int Food, float FoodSaturation) : IPacket<UpdateHealthPacket>, IPacket
{
    public static UpdateHealthPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<UpdateHealthPacket>(protocolVersion);
        var health = reader.ReadFloat();
        var food = reader.ReadVarInt();
        var foodSaturation = reader.ReadFloat();
        return new UpdateHealthPacket(health, food, foodSaturation);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<UpdateHealthPacket>(protocolVersion);
        writer.WriteFloat(Health);
        writer.WriteVarInt(Food);
        writer.WriteFloat(FoodSaturation);
    }

    public static PacketIdentity Identity => new("play.toClient.update_health", "UpdateHealth", PacketPhase.Play, PacketDirection.Clientbound, 110);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 735 && protocolVersion <= 736)
        {
            id = 0x49;
            return true;
        }

        if (protocolVersion >= 751 && protocolVersion <= 754)
        {
            id = 0x49;
            return true;
        }

        if (protocolVersion >= 755 && protocolVersion <= 759)
        {
            id = 0x52;
            return true;
        }

        if (protocolVersion >= 760 && protocolVersion <= 760)
        {
            id = 0x55;
            return true;
        }

        if (protocolVersion >= 761 && protocolVersion <= 761)
        {
            id = 0x53;
            return true;
        }

        if (protocolVersion >= 762 && protocolVersion <= 763)
        {
            id = 0x57;
            return true;
        }

        if (protocolVersion >= 764 && protocolVersion <= 764)
        {
            id = 0x59;
            return true;
        }

        if (protocolVersion >= 765 && protocolVersion <= 765)
        {
            id = 0x5B;
            return true;
        }

        if (protocolVersion >= 766 && protocolVersion <= 767)
        {
            id = 0x5D;
            return true;
        }

        if (protocolVersion >= 768 && protocolVersion <= 769)
        {
            id = 0x62;
            return true;
        }

        if (protocolVersion >= 770 && protocolVersion <= 772)
        {
            id = 0x61;
            return true;
        }

        if (protocolVersion >= 773 && protocolVersion <= 774)
        {
            id = 0x66;
            return true;
        }

        if (protocolVersion >= 775 && protocolVersion <= 776)
        {
            id = 0x68;
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
