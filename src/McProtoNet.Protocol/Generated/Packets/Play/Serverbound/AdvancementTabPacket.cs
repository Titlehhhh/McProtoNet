using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toServer.advancement_tab", PacketPhase.Play, PacketDirection.Serverbound)]
[PacketField("Action", "int")]
[PacketField("TabId", "string?")]
public sealed partial record AdvancementTabPacket(int Action, string? TabId) : IPacket<AdvancementTabPacket>, IPacket
{
    public static AdvancementTabPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<AdvancementTabPacket>(protocolVersion);
        var action = reader.ReadVarInt();
        string? tabId = default;
        if (action == 0)
        {
            var tabIdValue = reader.ReadString();
            tabId = tabIdValue;
        }

        return new AdvancementTabPacket(action, tabId);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<AdvancementTabPacket>(protocolVersion);
        writer.WriteVarInt(Action);
        if (Action == 0)
        {
            writer.WriteString((TabId ?? throw new System.InvalidOperationException("TabId is required at this protocol version.")));
        }
        else if (TabId is not null)
        {
            throw new System.InvalidOperationException("TabId is set, but 'action' does not select it at this protocol version.");
        }
    }

    public static PacketIdentity Identity => new("play.toServer.advancement_tab", "AdvancementTab", PacketPhase.Play, PacketDirection.Serverbound, 1);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 735 && protocolVersion <= 736)
        {
            id = 0x21;
            return true;
        }

        if (protocolVersion >= 751 && protocolVersion <= 758)
        {
            id = 0x22;
            return true;
        }

        if (protocolVersion >= 759 && protocolVersion <= 759)
        {
            id = 0x24;
            return true;
        }

        if (protocolVersion >= 760 && protocolVersion <= 763)
        {
            id = 0x25;
            return true;
        }

        if (protocolVersion >= 764 && protocolVersion <= 764)
        {
            id = 0x28;
            return true;
        }

        if (protocolVersion >= 765 && protocolVersion <= 765)
        {
            id = 0x29;
            return true;
        }

        if (protocolVersion >= 766 && protocolVersion <= 767)
        {
            id = 0x2C;
            return true;
        }

        if (protocolVersion >= 768 && protocolVersion <= 768)
        {
            id = 0x2E;
            return true;
        }

        if (protocolVersion >= 769 && protocolVersion <= 770)
        {
            id = 0x30;
            return true;
        }

        if (protocolVersion >= 771 && protocolVersion <= 774)
        {
            id = 0x31;
            return true;
        }

        if (protocolVersion >= 775 && protocolVersion <= 776)
        {
            id = 0x32;
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
