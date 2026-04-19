using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("OpenBook", PacketState.Play, PacketDirection.Clientbound)]
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[PacketId(MinecraftVersion.StartProtocol, 736, 0x2D)]
[PacketId(751, 754, 0x2C)]
[PacketId(755, 758, 0x2D)]
[PacketId(759, 759, 0x2A)]
[PacketId(760, 760, 0x2C)]
[PacketId(761, 761, 0x2B)]
[PacketId(762, 763, 0x2F)]
[PacketId(764, 765, 0x30)]
[PacketId(766, 767, 0x32)]
[PacketId(768, 769, 0x34)]
[PacketId(770, MinecraftVersion.LatestProtocol, 0x33)]
public sealed partial class OpenBookPacket : IServerPacket
{
    public int Hand { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
        => writer.WriteVarInt(Hand);

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Hand = reader.ReadVarInt();
}