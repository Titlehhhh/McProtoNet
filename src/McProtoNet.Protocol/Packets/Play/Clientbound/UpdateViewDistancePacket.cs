using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("UpdateViewDistance", PacketState.Play, PacketDirection.Clientbound)]
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[PacketId(MinecraftVersion.StartProtocol, 736, 0x41)]
[PacketId(751, 754, 0x41)]
[PacketId(755, 758, 0x4A)]
[PacketId(759, 759, 0x49)]
[PacketId(760, 760, 0x4C)]
[PacketId(761, 761, 0x4B)]
[PacketId(762, 763, 0x4F)]
[PacketId(764, 764, 0x51)]
[PacketId(765, 765, 0x53)]
[PacketId(766, 767, 0x55)]
[PacketId(768, 769, 0x59)]
[PacketId(770, MinecraftVersion.LatestProtocol, 0x58)]
public sealed partial class UpdateViewDistancePacket : IServerPacket
{
    public int ViewDistance { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        writer.WriteVarInt(ViewDistance);
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ViewDistance = reader.ReadVarInt();
    }
}