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

    public static PacketIdentity Identity => new("play.toServer.set_slot_state", "SetSlotState", PacketPhase.Play, PacketDirection.Serverbound, 51);

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
