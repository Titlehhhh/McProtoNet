using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toServer.held_item_slot", PacketPhase.Play, PacketDirection.Serverbound)]
[PacketField("SlotId", "int")]
public sealed partial record HeldItemSlotPacket(int SlotId) : IPacket<HeldItemSlotPacket>, IPacket
{
    public static HeldItemSlotPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<HeldItemSlotPacket>(protocolVersion);
        var slotId = reader.ReadSignedShort();
        return new HeldItemSlotPacket(slotId);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<HeldItemSlotPacket>(protocolVersion);
        writer.WriteSignedShort((short)SlotId);
    }

    public static PacketIdentity Identity => new("play.toServer.held_item_slot", "HeldItemSlot", PacketPhase.Play, PacketDirection.Serverbound, 28);

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
