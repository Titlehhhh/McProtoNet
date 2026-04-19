using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;

[PacketInfo("Transaction", PacketState.Play, PacketDirection.Serverbound)]
[ProtocolSupport(MinecraftVersion.StartProtocol, 754)]
[PacketId(MinecraftVersion.StartProtocol, 736, 0x07)]
[PacketId(751, 754, 0x07)]
public sealed partial class TransactionPacket : IClientPacket
{
    public byte WindowId { get; set; }
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