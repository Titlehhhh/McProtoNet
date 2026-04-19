using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;

[PacketInfo("SetSlotState", PacketState.Play, PacketDirection.Serverbound)]
[ProtocolSupport(765, MinecraftVersion.LatestProtocol)]
[PacketId(765, 765, 0x0F)]
[PacketId(766, 767, 0x10)]
[PacketId(768, 770, 0x12)]
[PacketId(771, MinecraftVersion.LatestProtocol, 0x13)]
public sealed partial class SetSlotStatePacket : IClientPacket
{
    public int SlotId { get; set; }
    public int WindowId { get; set; }
    public bool State { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        writer.WriteVarInt(SlotId);
        writer.WriteVarInt(WindowId);
        writer.WriteBoolean(State);
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        SlotId = reader.ReadVarInt();
        WindowId = reader.ReadVarInt();
        State = reader.ReadBoolean();
    }
}