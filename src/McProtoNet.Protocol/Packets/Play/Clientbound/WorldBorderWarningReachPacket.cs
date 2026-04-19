using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("WorldBorderWarningReach", PacketState.Play, PacketDirection.Clientbound)]
[ProtocolSupport(755, MinecraftVersion.LatestProtocol)]
[PacketId(755, 758, 0x46)]
[PacketId(759, 759, 0x45)]
[PacketId(760, 760, 0x48)]
[PacketId(761, 761, 0x47)]
[PacketId(762, 763, 0x4B)]
[PacketId(764, 764, 0x4D)]
[PacketId(765, 765, 0x4F)]
[PacketId(766, 767, 0x51)]
[PacketId(768, 769, 0x56)]
[PacketId(770, MinecraftVersion.LatestProtocol, 0x55)]
public sealed partial class WorldBorderWarningReachPacket : IServerPacket
{
    public int WarningBlocks { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        writer.WriteVarInt(WarningBlocks);
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        WarningBlocks = reader.ReadVarInt();
    }
}