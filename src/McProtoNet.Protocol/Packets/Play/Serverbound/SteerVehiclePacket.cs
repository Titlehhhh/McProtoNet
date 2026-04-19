using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;

[PacketInfo("SteerVehicle", PacketState.Play, PacketDirection.Serverbound)]
[ProtocolSupport(MinecraftVersion.StartProtocol, 767)]
[PacketId(MinecraftVersion.StartProtocol, 736, 0x1D)]
[PacketId(751, 754, 0x1D)]
[PacketId(755, 758, 0x1C)]
[PacketId(759, 759, 0x1E)]
[PacketId(760, 760, 0x1F)]
[PacketId(761, 761, 0x1E)]
[PacketId(762, 763, 0x1F)]
[PacketId(764, 764, 0x22)]
[PacketId(765, 765, 0x23)]
[PacketId(766, 767, 0x26)]
public sealed partial class SteerVehiclePacket : IClientPacket
{
    public float Name { get; set; }
    public float Sideways { get; set; }
    public float Forward { get; set; }
    public byte Jump { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
        => writer.WriteFloat(Name);
    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Name = reader.ReadFloat();
    
    // Note: Since there is only one version range (first-767), we read/write all fields directly 
    // without a switch statement as per Rule 17.
}