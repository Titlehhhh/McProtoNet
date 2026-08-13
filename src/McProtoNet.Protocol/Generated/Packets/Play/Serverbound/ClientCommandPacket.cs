using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toServer.client_command", PacketPhase.Play, PacketDirection.Serverbound)]
[PacketField("ActionId", "int")]
public sealed partial record ClientCommandPacket(int ActionId) : IPacket<ClientCommandPacket>, IPacket
{
    public static ClientCommandPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ClientCommandPacket>(protocolVersion);
        var actionId = reader.ReadVarInt();
        return new ClientCommandPacket(actionId);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ClientCommandPacket>(protocolVersion);
        writer.WriteVarInt(ActionId);
    }

    public static PacketIdentity Identity => new("play.toServer.client_command", "ClientCommand", PacketPhase.Play, PacketDirection.Serverbound, 10);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 735 && protocolVersion <= 736)
        {
            id = 0x04;
            return true;
        }

        if (protocolVersion >= 751 && protocolVersion <= 758)
        {
            id = 0x04;
            return true;
        }

        if (protocolVersion >= 759 && protocolVersion <= 759)
        {
            id = 0x06;
            return true;
        }

        if (protocolVersion >= 760 && protocolVersion <= 760)
        {
            id = 0x07;
            return true;
        }

        if (protocolVersion >= 761 && protocolVersion <= 761)
        {
            id = 0x06;
            return true;
        }

        if (protocolVersion >= 762 && protocolVersion <= 763)
        {
            id = 0x07;
            return true;
        }

        if (protocolVersion >= 764 && protocolVersion <= 765)
        {
            id = 0x08;
            return true;
        }

        if (protocolVersion >= 766 && protocolVersion <= 767)
        {
            id = 0x09;
            return true;
        }

        if (protocolVersion >= 768 && protocolVersion <= 770)
        {
            id = 0x0A;
            return true;
        }

        if (protocolVersion >= 771 && protocolVersion <= 772)
        {
            id = 0x0B;
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
