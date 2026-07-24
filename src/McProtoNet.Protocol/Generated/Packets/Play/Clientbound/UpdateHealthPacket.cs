using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
public readonly partial record struct UpdateHealthPacket(float Health, int Food, float FoodSaturation) : IProtocolType<UpdateHealthPacket>
{
    public static UpdateHealthPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<UpdateHealthPacket>(protocolVersion);
        var health = reader.ReadFloat();
        var food = reader.ReadVarInt();
        var foodSaturation = reader.ReadFloat();
        return new UpdateHealthPacket(health, food, foodSaturation);
    }

    public readonly void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<UpdateHealthPacket>(protocolVersion);
        writer.WriteFloat(Health);
        writer.WriteVarInt(Food);
        writer.WriteFloat(FoodSaturation);
    }

    public static int GetPacketId(int protocolVersion)
    {
        if (protocolVersion >= 735 && protocolVersion <= 736)
            return 0x49;
        if (protocolVersion >= 751 && protocolVersion <= 754)
            return 0x49;
        if (protocolVersion >= 755 && protocolVersion <= 755)
            return 0x52;
        if (protocolVersion >= 756 && protocolVersion <= 756)
            return 0x52;
        if (protocolVersion >= 757 && protocolVersion <= 758)
            return 0x52;
        if (protocolVersion >= 759 && protocolVersion <= 759)
            return 0x52;
        if (protocolVersion >= 760 && protocolVersion <= 760)
            return 0x55;
        if (protocolVersion >= 761 && protocolVersion <= 761)
            return 0x53;
        if (protocolVersion >= 762 && protocolVersion <= 763)
            return 0x57;
        if (protocolVersion >= 764 && protocolVersion <= 764)
            return 0x59;
        if (protocolVersion >= 765 && protocolVersion <= 765)
            return 0x5B;
        if (protocolVersion >= 766 && protocolVersion <= 766)
            return 0x5D;
        if (protocolVersion >= 767 && protocolVersion <= 767)
            return 0x5D;
        if (protocolVersion >= 768 && protocolVersion <= 769)
            return 0x62;
        if (protocolVersion >= 770 && protocolVersion <= 770)
            return 0x61;
        if (protocolVersion >= 771 && protocolVersion <= 772)
            return 0x61;
        throw new System.NotSupportedException($"No packet id for protocol {protocolVersion}.");
    }
}
