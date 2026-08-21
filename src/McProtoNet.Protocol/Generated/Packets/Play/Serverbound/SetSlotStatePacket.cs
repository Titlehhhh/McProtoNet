using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;
[ProtocolSupport(765, MinecraftVersion.LatestProtocol)]
[Packet("play.toServer.set_slot_state", PacketPhase.Play, PacketDirection.Serverbound)]
[PacketField("SlotId", "int")]
[PacketField("WindowId", "int")]
[PacketField("State", "bool")]
public sealed partial record SetSlotStatePacket(int SlotId, int WindowId, bool State) : IPacket<SetSlotStatePacket>, IPacket
{
    public static SetSlotStatePacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<SetSlotStatePacket>(protocolVersion);
        var slotId = reader.ReadVarInt();
        var windowId = reader.ReadVarInt();
        var state = reader.ReadBoolean();
        return new SetSlotStatePacket(slotId, windowId, state);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<SetSlotStatePacket>(protocolVersion);
        writer.WriteVarInt(SlotId);
        writer.WriteVarInt(WindowId);
        writer.WriteBoolean(State);
    }

    public static PacketIdentity Identity => new("play.toServer.set_slot_state", "SetSlotState", PacketPhase.Play, PacketDirection.Serverbound, 47);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 765 && protocolVersion <= 765)
        {
            id = 0x0F;
            return true;
        }

        if (protocolVersion >= 766 && protocolVersion <= 767)
        {
            id = 0x10;
            return true;
        }

        if (protocolVersion >= 768 && protocolVersion <= 770)
        {
            id = 0x12;
            return true;
        }

        if (protocolVersion >= 771 && protocolVersion <= 774)
        {
            id = 0x13;
            return true;
        }

        if (protocolVersion >= 775 && protocolVersion <= 776)
        {
            id = 0x14;
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
