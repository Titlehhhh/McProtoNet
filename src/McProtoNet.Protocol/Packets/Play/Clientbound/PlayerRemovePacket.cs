using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("PlayerRemove", PacketState.Play, PacketDirection.Clientbound)]
[ProtocolSupport(761, MinecraftVersion.LatestProtocol)]
[PacketId(761, 761, 0x35)]
[PacketId(762, 763, 0x39)]
[PacketId(764, 765, 0x3B)]
[PacketId(766, 767, 0x3D)]
[PacketId(768, 769, 0x3F)]
[PacketId(770, MinecraftVersion.LatestProtocol, 0x3E)]
public sealed partial class PlayerRemovePacket : IServerPacket
{
    public Guid[] Players { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
        => writer.WriteArray<Guid>(Players);

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Players = reader.ReadArray<Guid>(LengthFormat.VarInt);
}