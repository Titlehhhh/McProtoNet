using McProtoNet.NBT;

using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("TestInstanceBlockStatus", PacketState.Play, PacketDirection.Clientbound)]
[ProtocolSupport(770, MinecraftVersion.LatestProtocol)]
[PacketId(770, MinecraftVersion.LatestProtocol, 0x77)]
public sealed partial class TestInstanceBlockStatusPacket : IServerPacket
{
    public NbtTag Status { get; set; }
    public Vec3i? Size { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        writer.WriteAnonymousNbtTag(Status, protocolVersion);
        writer.WriteOptionalType<Vec3i>(Size, protocolVersion);
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        Status = reader.ReadAnonymousNbtTag(protocolVersion);
        Size = reader.ReadOptionalType<Vec3i>(protocolVersion);
    }
}