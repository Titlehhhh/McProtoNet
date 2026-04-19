using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("BlockChange", PacketState.Play, PacketDirection.Clientbound)]
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[PacketId(MinecraftVersion.StartProtocol, 736, 0x0B)]
[PacketId(751, 754, 0x0B)]
[PacketId(755, 758, 0x0C)]
[PacketId(759, 761, 0x09)]
[PacketId(762, 763, 0x0A)]
[PacketId(764, 769, 0x09)]
[PacketId(770, MinecraftVersion.LatestProtocol, 0x08)]
public sealed partial class BlockChangePacket : IServerPacket
{
    public Position Location { get; set; }
    public int Type { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        writer.WriteType<Position>(Location, protocolVersion);
        writer.WriteVarInt(Type);
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        Location = reader.ReadType<Position>(protocolVersion);
        Type = reader.ReadVarInt();
    }
}