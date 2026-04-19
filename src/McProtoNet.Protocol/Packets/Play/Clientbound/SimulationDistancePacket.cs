using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("SimulationDistance", PacketState.Play, PacketDirection.Clientbound)]
[ProtocolSupport(757, MinecraftVersion.LatestProtocol)]
[PacketId(757, 759, 0x57)]
[PacketId(760, 760, 0x5A)]
[PacketId(761, 761, 0x58)]
[PacketId(762, 763, 0x5C)]
[PacketId(764, 764, 0x5E)]
[PacketId(765, 765, 0x60)]
[PacketId(766, 767, 0x62)]
[PacketId(768, 769, 0x69)]
[PacketId(770, MinecraftVersion.LatestProtocol, 0x68)]
public sealed partial class SimulationDistancePacket : IServerPacket
{
    public int Distance { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
        => writer.WriteVarInt(Distance);

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Distance = reader.ReadVarInt();
}