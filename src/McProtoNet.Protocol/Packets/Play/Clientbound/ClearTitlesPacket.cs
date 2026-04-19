using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("ClearTitles", PacketState.Play, PacketDirection.Clientbound)]
[ProtocolSupport(755, MinecraftVersion.LatestProtocol)]
[PacketId(755, 758, 0x10)]
[PacketId(759, 760, 0x0D)]
[PacketId(761, 761, 0x0C)]
[PacketId(762, 763, 0x0E)]
[PacketId(764, 769, 0x0F)]
[PacketId(770, MinecraftVersion.LatestProtocol, 0x0E)]
public sealed partial class ClearTitlesPacket : IServerPacket
{
    public bool Reset { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
        => writer.WriteBoolean(Reset);

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Reset = reader.ReadBoolean();
}