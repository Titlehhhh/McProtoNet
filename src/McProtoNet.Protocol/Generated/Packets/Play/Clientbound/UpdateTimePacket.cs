using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.update_time", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("Age", "long")]
[PacketField("Time", "long", Group = "VUntil767", To = 767)]
[PacketField("Time", "long", Group = "V768_774", From = 768, To = 774)]
[PacketField("TickDayTime", "bool", Group = "V768_774", From = 768, To = 774)]
[PacketField("ClockUpdates", "ClockUpdate[]", Group = "V775_Last", From = 775)]
public sealed partial record UpdateTimePacket(long Age, UpdateTimePacket.VUntil767Layer? VUntil767 = null, UpdateTimePacket.V768_774Layer? V768_774 = null, UpdateTimePacket.V775_LastLayer? V775_Last = null) : IPacket<UpdateTimePacket>, IPacket
{
    public readonly record struct VUntil767Layer(long Time);
    public readonly record struct V768_774Layer(long Time, bool TickDayTime);
    public readonly record struct V775_LastLayer(ClockUpdate[] ClockUpdates);
    public static UpdateTimePacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<UpdateTimePacket>(protocolVersion);
        if (protocolVersion <= 767)
        {
            var age = reader.ReadSignedLong();
            var time = reader.ReadSignedLong();
            return new UpdateTimePacket(age, VUntil767: new VUntil767Layer(time));
        }

        if (protocolVersion >= 768 && protocolVersion <= 774)
        {
            var age = reader.ReadSignedLong();
            var time = reader.ReadSignedLong();
            var tickDayTime = reader.ReadBoolean();
            return new UpdateTimePacket(age, V768_774: new V768_774Layer(time, tickDayTime));
        }

        if (protocolVersion >= 775)
        {
            var age = reader.ReadSignedLong();
            int clockUpdatesCount = reader.ReadVarInt();
            var clockUpdates = new ClockUpdate[clockUpdatesCount];
            for (int i = 0; i < clockUpdates.Length; i++)
                clockUpdates[i] = reader.ReadType<ClockUpdate>(protocolVersion);
            return new UpdateTimePacket(age, V775_Last: new V775_LastLayer(clockUpdates));
        }

        throw new System.NotSupportedException($"UpdateTimePacket has no wire layout for protocol version {protocolVersion}.");
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<UpdateTimePacket>(protocolVersion);
        if (protocolVersion <= 767)
        {
            var layer = VUntil767 ?? throw new WrongLayerException("UpdateTimePacket", protocolVersion, "VUntil767");
            long Time = layer.Time;
            writer.WriteSignedLong(Age);
            writer.WriteSignedLong(Time);
            return;
        }

        if (protocolVersion >= 768 && protocolVersion <= 774)
        {
            var layer = V768_774 ?? throw new WrongLayerException("UpdateTimePacket", protocolVersion, "V768_774");
            long Time = layer.Time;
            bool TickDayTime = layer.TickDayTime;
            writer.WriteSignedLong(Age);
            writer.WriteSignedLong(Time);
            writer.WriteBoolean(TickDayTime);
            return;
        }

        if (protocolVersion >= 775)
        {
            var layer = V775_Last ?? throw new WrongLayerException("UpdateTimePacket", protocolVersion, "V775_Last");
            ClockUpdate[] ClockUpdates = layer.ClockUpdates;
            writer.WriteSignedLong(Age);
            writer.WriteVarInt(ClockUpdates.Length);
            foreach (var clockUpdatesItem in ClockUpdates)
                writer.WriteType<ClockUpdate>(clockUpdatesItem, protocolVersion);
            return;
        }

        throw new System.NotSupportedException($"UpdateTimePacket has no wire layout for protocol version {protocolVersion}.");
    }

    public static PacketIdentity Identity => new("play.toClient.update_time", "UpdateTime", PacketPhase.Play, PacketDirection.Clientbound, 112);

    PacketIdentity IPacket.Identity => Identity;

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

        if (protocolVersion >= 773 && protocolVersion <= 774)
        {
            id = 0x6F;
            return true;
        }

        if (protocolVersion >= 775 && protocolVersion <= 776)
        {
            id = 0x71;
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
