using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;
using McProtoNet.NBT;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("SetPassengers", PacketState.Play, PacketDirection.Clientbound)]
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[PacketId(MinecraftVersion.StartProtocol, 736, 0x4B)]
[PacketId(751, 754, 0x4B)]
[PacketId(755, 759, 0x54)]
[PacketId(760, 760, 0x57)]
[PacketId(761, 761, 0x55)]
[PacketId(762, 763, 0x59)]
[PacketId(764, 764, 0x5B)]
[PacketId(765, 765, 0x5D)]
[PacketId(766, 767, 0x5F)]
[PacketId(768, 769, 0x65)]
[PacketId(770, MinecraftVersion.LatestProtocol, 0x64)]
public sealed partial class SetPassengersPacket : IServerPacket
{
    public int EntityId { get; set; }
    public int[] Passengers { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        writer.WriteVarInt(EntityId);
        writer.WriteVarIntArray(Passengers);
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        EntityId = reader.ReadVarInt();
        Passengers = reader.ReadVarIntArray(LengthFormat.VarInt);
    }
}