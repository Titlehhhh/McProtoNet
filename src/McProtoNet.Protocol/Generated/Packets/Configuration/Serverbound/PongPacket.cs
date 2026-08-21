using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Configuration.Serverbound;
[ProtocolSupport(764, MinecraftVersion.LatestProtocol)]
[Packet("configuration.toServer.pong", PacketPhase.Configuration, PacketDirection.Serverbound)]
[PacketField("Id", "int")]
public sealed partial record PongPacket(int Id) : IPacket<PongPacket>, IPacket
{
    public static PongPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<PongPacket>(protocolVersion);
        var id = reader.ReadSignedInt();
        return new PongPacket(id);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<PongPacket>(protocolVersion);
        writer.WriteSignedInt(Id);
    }

    public static PacketIdentity Identity => new("configuration.toServer.pong", "Pong", PacketPhase.Configuration, PacketDirection.Serverbound, 7);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 764 && protocolVersion <= 765)
        {
            id = 0x04;
            return true;
        }

        if (protocolVersion >= 766 && protocolVersion <= 776)
        {
            id = 0x05;
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
