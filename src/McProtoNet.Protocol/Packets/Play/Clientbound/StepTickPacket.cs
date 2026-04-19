using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("StepTick", PacketState.Play, PacketDirection.Clientbound)]
[ProtocolSupport(765, MinecraftVersion.LatestProtocol)]
[PacketId(765, 765, 0x6F)]
[PacketId(766, 767, 0x72)]
[PacketId(768, MinecraftVersion.LatestProtocol, 0x79)]
public sealed partial class StepTickPacket : IServerPacket
{
    public int TickSteps { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
        => writer.WriteVarInt(TickSteps);

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => TickSteps = reader.ReadVarInt();
}