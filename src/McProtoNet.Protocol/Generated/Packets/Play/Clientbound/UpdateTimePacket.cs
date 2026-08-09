using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.update_time", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("Age", "long")]
[PacketField("Time", "long")]
[PacketField("TickDayTime", "bool", Group = "V768_Last", From = 768)]
public sealed partial record UpdateTimePacket(long Age, long Time, UpdateTimePacket.V768_LastLayer? V768_Last = null) : IPacket<UpdateTimePacket>
{
    public readonly record struct V768_LastLayer(bool TickDayTime);
    public static UpdateTimePacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<UpdateTimePacket>(protocolVersion);
        if (protocolVersion <= 767)
        {
            var age = reader.ReadSignedLong();
            var time = reader.ReadSignedLong();
            return new UpdateTimePacket(age, time);
        }

        if (protocolVersion >= 768)
        {
            var age = reader.ReadSignedLong();
            var time = reader.ReadSignedLong();
            var tickDayTime = reader.ReadBoolean();
            return new UpdateTimePacket(age, time, V768_Last: new V768_LastLayer(tickDayTime));
        }

        throw new System.NotSupportedException($"UpdateTimePacket has no wire layout for protocol version {protocolVersion}.");
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
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
            var layer = V768_Last ?? throw new WrongLayerException("UpdateTimePacket", protocolVersion, "V768_Last");
            bool TickDayTime = layer.TickDayTime;
            writer.WriteSignedLong(Age);
            writer.WriteSignedLong(Time);
            writer.WriteBoolean(TickDayTime);
            return;
        }

        throw new System.NotSupportedException($"UpdateTimePacket has no wire layout for protocol version {protocolVersion}.");
    }

    public static PacketIdentity Identity => new("play.toClient.update_time", "UpdateTime", PacketPhase.Play, PacketDirection.Clientbound, 17);

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 735 && protocolVersion <= 736)
        {
            id = 0x4E;
            return true;
        }

        if (protocolVersion >= 751 && protocolVersion <= 754)
        {
            id = 0x4E;
            return true;
        }

        if (protocolVersion >= 755 && protocolVersion <= 756)
        {
            id = 0x58;
            return true;
        }

        if (protocolVersion >= 757 && protocolVersion <= 759)
        {
            id = 0x59;
            return true;
        }

        if (protocolVersion >= 760 && protocolVersion <= 760)
        {
            id = 0x5C;
            return true;
        }

        if (protocolVersion >= 761 && protocolVersion <= 761)
        {
            id = 0x5A;
            return true;
        }

        if (protocolVersion >= 762 && protocolVersion <= 763)
        {
            id = 0x5E;
            return true;
        }

        if (protocolVersion >= 764 && protocolVersion <= 764)
        {
            id = 0x60;
            return true;
        }

        if (protocolVersion >= 765 && protocolVersion <= 765)
        {
            id = 0x62;
            return true;
        }

        if (protocolVersion >= 766 && protocolVersion <= 767)
        {
            id = 0x64;
            return true;
        }

        if (protocolVersion >= 768 && protocolVersion <= 769)
        {
            id = 0x6B;
            return true;
        }

        if (protocolVersion >= 770 && protocolVersion <= 772)
        {
            id = 0x6A;
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
