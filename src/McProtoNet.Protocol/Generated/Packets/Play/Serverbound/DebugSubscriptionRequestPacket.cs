using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;

[ProtocolSupport(773, MinecraftVersion.LatestProtocol)]
[Packet("play.toServer.debug_subscription_request", PacketPhase.Play, PacketDirection.Serverbound)]
[PacketField("Subscriptions", "DebugSubscriptionDataType[]")]
public sealed partial record DebugSubscriptionRequestPacket(DebugSubscriptionDataType[] Subscriptions) : IPacket<DebugSubscriptionRequestPacket>, IPacket
{
    public static DebugSubscriptionRequestPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<DebugSubscriptionRequestPacket>(protocolVersion);
        int subscriptionsCount = reader.ReadVarInt();
        var subscriptions = new DebugSubscriptionDataType[subscriptionsCount];
        for (int i = 0; i < subscriptions.Length; i++)
            subscriptions[i] = reader.ReadType<DebugSubscriptionDataType>(protocolVersion);
        return new DebugSubscriptionRequestPacket(subscriptions);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<DebugSubscriptionRequestPacket>(protocolVersion);
        writer.WriteVarInt(Subscriptions.Length);
        foreach (var subscriptionsItem in Subscriptions)
            writer.WriteType<DebugSubscriptionDataType>(subscriptionsItem, protocolVersion);
    }

    public static PacketIdentity Identity => new("play.toServer.debug_subscription_request", "DebugSubscriptionRequest", PacketPhase.Play, PacketDirection.Serverbound, 23);

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
