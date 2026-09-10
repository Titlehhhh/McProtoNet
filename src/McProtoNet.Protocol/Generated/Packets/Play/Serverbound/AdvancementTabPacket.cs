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
        return PacketRegistry.TryGetId(Identity, protocolVersion, out id);
    }

    public static int GetPacketId(int protocolVersion)
    {
        return PacketRegistry.GetId(Identity, protocolVersion);
    }
}
