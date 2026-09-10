using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;
[ProtocolSupport(MinecraftVersion.StartProtocol, 754)]
[Packet("play.toServer.transaction", PacketPhase.Play, PacketDirection.Serverbound)]
[PacketField("WindowId", "int")]
[PacketField("Action", "int")]
[PacketField("Accepted", "bool")]
public sealed partial record TransactionPacket(int WindowId, int Action, bool Accepted) : IPacket<TransactionPacket>, IPacket
{
    public static TransactionPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<TransactionPacket>(protocolVersion);
        var windowId = reader.ReadSignedByte();
        var action = reader.ReadSignedShort();
        var accepted = reader.ReadBoolean();
        return new TransactionPacket(windowId, action, accepted);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<TransactionPacket>(protocolVersion);
        writer.WriteSignedByte((sbyte)WindowId);
        writer.WriteSignedShort((short)Action);
        writer.WriteBoolean(Accepted);
    }

    public static PacketIdentity Identity => new("play.toServer.transaction", "Transaction", PacketPhase.Play, PacketDirection.Serverbound, 60);

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
