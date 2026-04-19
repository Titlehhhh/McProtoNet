using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("AttachEntity", PacketState.Play, PacketDirection.Clientbound)]
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[PacketId(MinecraftVersion.StartProtocol, 736, 0x45)]
[PacketId(751, 754, 0x45)]
[PacketId(755, 759, 0x4E)]
[PacketId(760, 760, 0x51)]
[PacketId(761, 761, 0x4F)]
[PacketId(762, 763, 0x53)]
[PacketId(764, 764, 0x55)]
[PacketId(765, 765, 0x57)]
[PacketId(766, 767, 0x59)]
[PacketId(768, 769, 0x5E)]
[PacketId(770, MinecraftVersion.LatestProtocol, 0x5D)]
public sealed partial class AttachEntityPacket : IServerPacket
{
    public int EntityId { get; set; }
    public int VehicleId { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        writer.WriteSignedInt(EntityId);
        writer.WriteSignedInt(VehicleId);
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        EntityId = reader.ReadSignedInt();
        VehicleId = reader.ReadSignedInt();
    }
}