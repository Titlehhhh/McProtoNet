using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("SetTickingState", PacketState.Play, PacketDirection.Clientbound)]
[ProtocolSupport(765, MinecraftVersion.LatestProtocol)]
[PacketId(765, 765, 0x6E)]
[PacketId(766, 767, 0x71)]
[PacketId(768, MinecraftVersion.LatestProtocol, 0x78)]
public sealed partial class SetTickingStatePacket : IServerPacket
{
    public float TickRate { get; set; }
    public bool IsFrozen { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        writer.WriteFloat(TickRate);
        writer.WriteBoolean(IsFrozen);
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        TickRate = reader.ReadFloat();
        IsFrozen = reader.ReadBoolean();
    }
}