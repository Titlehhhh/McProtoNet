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
        return PacketRegistry.TryGetId(Identity, protocolVersion, out id);
    }

    public static int GetPacketId(int protocolVersion)
    {
        return PacketRegistry.GetId(Identity, protocolVersion);
    }
}
