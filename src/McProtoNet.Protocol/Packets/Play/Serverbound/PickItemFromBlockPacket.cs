using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;

[PacketInfo("PickItemFromBlock", PacketState.Play, PacketDirection.Serverbound)]
[ProtocolSupport(769, MinecraftVersion.LatestProtocol)]
[PacketId(769, 770, 0x22)]
[PacketId(771, MinecraftVersion.LatestProtocol, 0x23)]
public sealed partial class PickItemFromBlockPacket : IPacket
{
    public Position Name { get; set; } = default!;
    public bool IncludeData { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
        {
            writer.WriteType<Position>(Name, protocolVersion);
            writer.WriteBoolean(IncludeData);
        }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        {
            Name = reader.ReadType<Position>(protocolVersion);
            IncludeData = reader.ReadBoolean();
        }
}