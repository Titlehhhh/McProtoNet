using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

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

    public static PacketIdentity Identity => new("play.toServer.transaction", "Transaction", PacketPhase.Play, PacketDirection.Serverbound, 53);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 735 && protocolVersion <= 736)
        {
            id = 0x07;
            return true;
        }

        if (protocolVersion >= 751 && protocolVersion <= 754)
        {
            id = 0x07;
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
