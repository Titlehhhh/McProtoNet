using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.open_book", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("Hand", "int")]
public sealed partial record OpenBookPacket(int Hand) : IPacket<OpenBookPacket>, IPacket
{
    public static OpenBookPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<OpenBookPacket>(protocolVersion);
        var hand = reader.ReadVarInt();
        return new OpenBookPacket(hand);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<OpenBookPacket>(protocolVersion);
        writer.WriteVarInt(Hand);
    }

    public static PacketIdentity Identity => new("play.toClient.open_book", "OpenBook", PacketPhase.Play, PacketDirection.Clientbound, 65);

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
