using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;

[PacketInfo("SteerBoat", PacketState.Play, PacketDirection.Serverbound)]
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[PacketId(MinecraftVersion.StartProtocol, 736, 0x17)]
[PacketId(751, 754, 0x17)]
[PacketId(755, 758, 0x16)]
[PacketId(759, 759, 0x18)]
[PacketId(760, 760, 0x19)]
[PacketId(761, 761, 0x18)]
[PacketId(762, 763, 0x19)]
[PacketId(764, 764, 0x1B)]
[PacketId(765, 765, 0x1C)]
[PacketId(766, 767, 0x1F)]
[PacketId(768, 770, 0x21)]
[PacketId(771, MinecraftVersion.LatestProtocol, 0x22)]
public sealed partial class SteerBoatPacket : IClientPacket
{
    public bool LeftPaddle { get; set; }
    public bool RightPaddle { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
        => writer.WriteBoolean(LeftPaddle);
    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => LeftPaddle = reader.ReadBoolean();
}