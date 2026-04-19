using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;

[PacketInfo("Pong", PacketState.Play, PacketDirection.Serverbound)]
[ProtocolSupport(755, MinecraftVersion.LatestProtocol)]
[PacketId(755, 758, 0x1D)]
[PacketId(759, 759, 0x1F)]
[PacketId(760, 760, 0x20)]
[PacketId(761, 761, 0x1F)]
[PacketId(762, 763, 0x20)]
[PacketId(764, 764, 0x23)]
[PacketId(765, 765, 0x24)]
[PacketId(766, 767, 0x27)]
[PacketId(768, 768, 0x29)]
[PacketId(769, 770, 0x2B)]
[PacketId(771, MinecraftVersion.LatestProtocol, 0x2C)]
public sealed partial class PongPacket : IClientPacket
{
    public int Id { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
        => writer.WriteSignedInt(Id);

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Id = reader.ReadSignedInt();
}