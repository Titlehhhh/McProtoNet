using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Configuration.Serverbound;
[ProtocolSupport(764, MinecraftVersion.LatestProtocol)]
[Packet("configuration.toServer.keep_alive", PacketPhase.Configuration, PacketDirection.Serverbound)]
[PacketField("KeepAliveId", "long")]
public sealed partial record KeepAlivePacket(long KeepAliveId) : IPacket<KeepAlivePacket>
{
    public static KeepAlivePacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<KeepAlivePacket>(protocolVersion);
        var keepAliveId = reader.ReadSignedLong();
        return new KeepAlivePacket(keepAliveId);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<KeepAlivePacket>(protocolVersion);
        writer.WriteSignedLong(KeepAliveId);
    }

    public static PacketIdentity Identity => new("configuration.toServer.keep_alive", "KeepAlive", PacketPhase.Configuration, PacketDirection.Serverbound, 1);

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 764 && protocolVersion <= 765)
        {
            id = 0x03;
            return true;
        }

        if (protocolVersion >= 766 && protocolVersion <= 772)
        {
            id = 0x04;
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
