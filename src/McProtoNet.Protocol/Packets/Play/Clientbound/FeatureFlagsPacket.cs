using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("FeatureFlags", PacketState.Play, PacketDirection.Clientbound)]
[ProtocolSupport(761, 763)]
[PacketId(761, 761, 0x67)]
[PacketId(762, 763, 0x6B)]
public sealed partial class FeatureFlagsPacket : IServerPacket
{
    public string[] Features { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
        => writer.WriteArray(Features);

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Features = reader.ReadArray<string>(LengthFormat.VarInt);
}