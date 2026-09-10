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

    public static PacketIdentity Identity => new("play.toClient.update_time", "UpdateTime", PacketPhase.Play, PacketDirection.Clientbound, 124);

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
