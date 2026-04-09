using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("WorldBorderSize", PacketState.Play, PacketDirection.Clientbound)]
[ProtocolSupport(755, MinecraftVersion.LatestProtocol)]
[PacketId(755, 758, 0x44)]
[PacketId(759, 759, 0x43)]
[PacketId(760, 760, 0x46)]
[PacketId(761, 761, 0x45)]
[PacketId(762, 763, 0x49)]
[PacketId(764, 764, 0x4B)]
[PacketId(765, 765, 0x4D)]
[PacketId(766, 767, 0x4F)]
[PacketId(768, 769, 0x54)]
[PacketId(770, MinecraftVersion.LatestProtocol, 0x53)]
public sealed partial class WorldBorderSizePacket : IPacket
{
    public double Diameter { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
        => writer.WriteFloat(Diameter);

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Diameter = reader.ReadFloat();

    void IPacket.Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);

}