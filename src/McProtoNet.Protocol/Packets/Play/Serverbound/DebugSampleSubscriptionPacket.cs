using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;

[PacketInfo("DebugSampleSubscription", PacketState.Play, PacketDirection.Serverbound)]
[ProtocolSupport(766, MinecraftVersion.LatestProtocol)]
[PacketId(766, 767, 0x13)]
[PacketId(768, 770, 0x15)]
[PacketId(771, MinecraftVersion.LatestProtocol, 0x16)]
public sealed partial class DebugSampleSubscriptionPacket : IClientPacket
{
    public int Name { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
        => writer.WriteVarInt(Name);

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Name = reader.ReadVarInt();
}