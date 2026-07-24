using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
public readonly partial record struct UpdateTimePacket(long Age, long Time, bool TickDayTime) : IProtocolType<UpdateTimePacket>
{
    public static UpdateTimePacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<UpdateTimePacket>(protocolVersion);
        if (protocolVersion <= 767)
        {
            var age = reader.ReadSignedLong();
            var time = reader.ReadSignedLong();
            return new UpdateTimePacket(age, time, default!);
        }

        if (protocolVersion >= 768)
        {
            var age = reader.ReadSignedLong();
            var time = reader.ReadSignedLong();
            var tickDayTime = reader.ReadBoolean();
            return new UpdateTimePacket(age, time, tickDayTime);
        }

        throw new System.NotSupportedException($"UpdateTimePacket has no wire layout for protocol version {protocolVersion}.");
    }

    public readonly void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<UpdateTimePacket>(protocolVersion);
        if (protocolVersion <= 767)
        {
            writer.WriteSignedLong(Age);
            writer.WriteSignedLong(Time);
            return;
        }

        if (protocolVersion >= 768)
        {
            writer.WriteSignedLong(Age);
            writer.WriteSignedLong(Time);
            writer.WriteBoolean(TickDayTime);
            return;
        }

        throw new System.NotSupportedException($"UpdateTimePacket has no wire layout for protocol version {protocolVersion}.");
    }

    public static int GetPacketId(int protocolVersion)
    {
        if (protocolVersion >= 735 && protocolVersion <= 736)
            return 0x4E;
        if (protocolVersion >= 751 && protocolVersion <= 754)
            return 0x4E;
        if (protocolVersion >= 755 && protocolVersion <= 755)
            return 0x58;
        if (protocolVersion >= 756 && protocolVersion <= 756)
            return 0x58;
        if (protocolVersion >= 757 && protocolVersion <= 758)
            return 0x59;
        if (protocolVersion >= 759 && protocolVersion <= 759)
            return 0x59;
        if (protocolVersion >= 760 && protocolVersion <= 760)
            return 0x5C;
        if (protocolVersion >= 761 && protocolVersion <= 761)
            return 0x5A;
        if (protocolVersion >= 762 && protocolVersion <= 763)
            return 0x5E;
        if (protocolVersion >= 764 && protocolVersion <= 764)
            return 0x60;
        if (protocolVersion >= 765 && protocolVersion <= 765)
            return 0x62;
        if (protocolVersion >= 766 && protocolVersion <= 766)
            return 0x64;
        if (protocolVersion >= 767 && protocolVersion <= 767)
            return 0x64;
        if (protocolVersion >= 768 && protocolVersion <= 769)
            return 0x6B;
        if (protocolVersion >= 770 && protocolVersion <= 770)
            return 0x6A;
        if (protocolVersion >= 771 && protocolVersion <= 772)
            return 0x6A;
        throw new System.NotSupportedException($"No packet id for protocol {protocolVersion}.");
    }
}
