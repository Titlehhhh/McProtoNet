using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;
[ProtocolSupport(766, 772)]
[Packet("play.toServer.debug_sample_subscription", PacketPhase.Play, PacketDirection.Serverbound)]
[PacketField("Type", "int")]
public sealed partial record DebugSampleSubscriptionPacket(int Type) : IPacket<DebugSampleSubscriptionPacket>, IPacket
{
    public static DebugSampleSubscriptionPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<DebugSampleSubscriptionPacket>(protocolVersion);
        var type = reader.ReadVarInt();
        return new DebugSampleSubscriptionPacket(type);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<DebugSampleSubscriptionPacket>(protocolVersion);
        writer.WriteVarInt(Type);
    }

    public static PacketIdentity Identity => new("play.toServer.debug_sample_subscription", "DebugSampleSubscription", PacketPhase.Play, PacketDirection.Serverbound, 19);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 766 && protocolVersion <= 767)
        {
            id = 0x13;
            return true;
        }

        if (protocolVersion >= 768 && protocolVersion <= 770)
        {
            id = 0x15;
            return true;
        }

        if (protocolVersion >= 771 && protocolVersion <= 772)
        {
            id = 0x16;
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
