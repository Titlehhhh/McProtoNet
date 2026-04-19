using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("WorldBorderWarningDelay", PacketState.Play, PacketDirection.Clientbound)]
[ProtocolSupport(755, MinecraftVersion.LatestProtocol)]
[PacketId(755, 758, 0x45)]
[PacketId(759, 759, 0x44)]
[PacketId(760, 760, 0x47)]
[PacketId(761, 761, 0x46)]
[PacketId(762, 763, 0x4A)]
[PacketId(764, 764, 0x4C)]
[PacketId(765, 765, 0x4E)]
[PacketId(766, 767, 0x50)]
[PacketId(768, 769, 0x55)]
[PacketId(770, MinecraftVersion.LatestProtocol, 0x54)]
public sealed partial class WorldBorderWarningDelayPacket : IServerPacket
{
    public int WarningTime { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
        => writer.WriteVarInt(WarningTime);

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => WarningTime = reader.ReadVarInt();
}