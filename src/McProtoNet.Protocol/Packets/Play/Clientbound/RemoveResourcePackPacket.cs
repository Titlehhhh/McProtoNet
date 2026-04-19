using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("RemoveResourcePack", PacketState.Play, PacketDirection.Clientbound)]
[ProtocolSupport(765, MinecraftVersion.LatestProtocol)]
[PacketId(765, 765, 0x43)]
[PacketId(766, 767, 0x45)]
[PacketId(768, 769, 0x4A)]
[PacketId(770, MinecraftVersion.LatestProtocol, 0x49)]
public sealed partial class RemoveResourcePackPacket : IServerPacket
{
    public Guid? Uuid { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        writer.WriteBoolean(Uuid.HasValue);
        if (Uuid.HasValue)
            writer.WriteUUID(Uuid.Value, protocolVersion);
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        bool hasUuid = reader.ReadBoolean();
        if (hasUuid)
            Uuid = reader.ReadUUID(protocolVersion);
        else
            Uuid = null;
    }
}