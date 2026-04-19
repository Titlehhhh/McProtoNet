using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;

[PacketInfo("SetTestBlock", PacketState.Play, PacketDirection.Serverbound)]
[ProtocolSupport(770, MinecraftVersion.LatestProtocol)]
[PacketId(770, 770, 0x39)]
[PacketId(771, MinecraftVersion.LatestProtocol, 0x3A)]
public sealed partial class SetTestBlockPacket : IPacket
{
    public Position Name { get; set; } = default!;
    public int Mode { get; set; }
    public string? Message { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        writer.WriteType(Name, protocolVersion);
        writer.WriteVarInt(Mode);
        writer.WriteString(Message ?? string.Empty);
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        Name = reader.ReadType<Position>(protocolVersion);
        Mode = reader.ReadVarInt();
        Message = reader.ReadString();
    }
}