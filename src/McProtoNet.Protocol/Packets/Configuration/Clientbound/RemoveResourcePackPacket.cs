using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Configuration.Clientbound;

[PacketInfo("RemoveResourcePack", PacketState.Configuration, PacketDirection.Clientbound)]
[ProtocolSupport(765, MinecraftVersion.LatestProtocol)]
[PacketId(765, 765, 0x06)]
[PacketId(766, MinecraftVersion.LatestProtocol, 0x08)]
public sealed partial class RemoveResourcePackPacket : IServerPacket
{
    public Guid? Uuid { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        if (Uuid.HasValue)
            writer.WriteUUID(Uuid.Value);
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        Uuid = reader.ReadUUID();
    }
}