using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("WorldBorderCenter", PacketState.Play, PacketDirection.Clientbound)]
[ProtocolSupport(755, MinecraftVersion.LatestProtocol)]
[PacketId(755, 758, 0x42)]
[PacketId(759, 759, 0x41)]
[PacketId(760, 760, 0x44)]
[PacketId(761, 761, 0x43)]
[PacketId(762, 763, 0x47)]
[PacketId(764, 764, 0x49)]
[PacketId(765, 765, 0x4B)]
[PacketId(766, 767, 0x4D)]
[PacketId(768, 769, 0x52)]
[PacketId(770, MinecraftVersion.LatestProtocol, 0x51)]
public sealed partial class WorldBorderCenterPacket : IServerPacket
{
    public double X { get; set; }
    public double Z { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        writer.WriteDouble(X);
        writer.WriteDouble(Z);
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        X = reader.ReadDouble();
        Z = reader.ReadDouble();
    }
}