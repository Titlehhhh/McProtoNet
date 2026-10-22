using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[ProtocolSupport(762, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.bundle_delimiter", PacketPhase.Play, PacketDirection.Clientbound)]
public sealed partial record BundleDelimiterPacket() : IPacket<BundleDelimiterPacket>, IPacket
{
    public static BundleDelimiterPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<BundleDelimiterPacket>(protocolVersion);
        return new BundleDelimiterPacket();
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<BundleDelimiterPacket>(protocolVersion);
    }

    public static PacketIdentity Identity => new("play.toClient.bundle_delimiter", "BundleDelimiter", PacketPhase.Play, PacketDirection.Clientbound, 11);

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
