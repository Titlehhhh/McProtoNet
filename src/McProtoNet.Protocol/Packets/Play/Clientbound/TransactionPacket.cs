using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("Transaction", PacketState.Play, PacketDirection.Clientbound)]
[ProtocolSupport(MinecraftVersion.StartProtocol, 754)]
[PacketId(MinecraftVersion.StartProtocol, 736, 0x12)]
[PacketId(751, 754, 0x11)]
public sealed partial class TransactionPacket : IServerPacket
{
    public sbyte WindowId { get; set; }
    public short Action { get; set; }
    public bool Accepted { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        writer.WriteSignedByte(WindowId);
        writer.WriteSignedShort(Action);
        writer.WriteBoolean(Accepted);
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        WindowId = reader.ReadSignedByte();
        Action = reader.ReadSignedShort();
        Accepted = reader.ReadBoolean();
    }
}