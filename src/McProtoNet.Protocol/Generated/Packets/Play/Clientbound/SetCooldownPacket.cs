using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
public sealed partial class SetCooldownPacket : IProtocolType<SetCooldownPacket>
{
    public int ItemId { get; }
    public string CooldownGroup { get; }
    public int CooldownTicks { get; }

    public SetCooldownPacket(int itemId, string cooldownGroup, int cooldownTicks)
    {
        ItemId = itemId;
        CooldownGroup = cooldownGroup;
        CooldownTicks = cooldownTicks;
    }

    public static SetCooldownPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<SetCooldownPacket>(protocolVersion);
        if (protocolVersion <= 767)
        {
            var itemId = reader.ReadVarInt();
            var cooldownTicks = reader.ReadVarInt();
            return new SetCooldownPacket(itemId, default!, cooldownTicks);
        }

        if (protocolVersion >= 768)
        {
            var cooldownGroup = reader.ReadString();
            var cooldownTicks = reader.ReadVarInt();
            return new SetCooldownPacket(default!, cooldownGroup, cooldownTicks);
        }

        throw new System.NotSupportedException($"SetCooldownPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<SetCooldownPacket>(protocolVersion);
        if (protocolVersion <= 767)
        {
            writer.WriteVarInt(ItemId);
            writer.WriteVarInt(CooldownTicks);
            return;
        }

        if (protocolVersion >= 768)
        {
            writer.WriteString(CooldownGroup);
            writer.WriteVarInt(CooldownTicks);
            return;
        }

        throw new System.NotSupportedException($"SetCooldownPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public static int GetPacketId(int protocolVersion)
    {
        if (protocolVersion >= 735 && protocolVersion <= 736)
            return 0x17;
        if (protocolVersion >= 751 && protocolVersion <= 754)
            return 0x16;
        if (protocolVersion >= 755 && protocolVersion <= 755)
            return 0x17;
        if (protocolVersion >= 756 && protocolVersion <= 756)
            return 0x17;
        if (protocolVersion >= 757 && protocolVersion <= 758)
            return 0x17;
        if (protocolVersion >= 759 && protocolVersion <= 759)
            return 0x14;
        if (protocolVersion >= 760 && protocolVersion <= 760)
            return 0x14;
        if (protocolVersion >= 761 && protocolVersion <= 761)
            return 0x13;
        if (protocolVersion >= 762 && protocolVersion <= 763)
            return 0x15;
        if (protocolVersion >= 764 && protocolVersion <= 764)
            return 0x16;
        if (protocolVersion >= 765 && protocolVersion <= 765)
            return 0x16;
        if (protocolVersion >= 766 && protocolVersion <= 766)
            return 0x17;
        if (protocolVersion >= 767 && protocolVersion <= 767)
            return 0x17;
        if (protocolVersion >= 768 && protocolVersion <= 769)
            return 0x17;
        if (protocolVersion >= 770 && protocolVersion <= 770)
            return 0x16;
        if (protocolVersion >= 771 && protocolVersion <= 772)
            return 0x16;
        throw new System.NotSupportedException($"No packet id for protocol {protocolVersion}.");
    }
}
