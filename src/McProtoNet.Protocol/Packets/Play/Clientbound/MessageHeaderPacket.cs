using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("MessageHeader", PacketState.Play, PacketDirection.Clientbound)]
[ProtocolSupport(760, 760)]
[PacketId(760, 760, 0x32)]
public sealed partial class MessageHeaderPacket : IServerPacket
{
    public byte[]? PreviousSignature { get; set; }
    public Guid SenderUuid { get; set; }
    public byte[] Signature { get; set; }
    public byte[] MessageHash { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        writer.WriteBoolean(PreviousSignature != null);
        if (PreviousSignature != null)
            writer.WriteBuffer<VarInt>(PreviousSignature);
        writer.WriteUUID(SenderUuid);
        writer.WriteBuffer<VarInt>(Signature);
        writer.WriteBuffer<VarInt>(MessageHash);
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        if (reader.ReadBoolean())
            PreviousSignature = reader.ReadBuffer<byte>(LengthFormat.VarInt);
        else
            PreviousSignature = null;
        SenderUuid = reader.ReadUUID();
        Signature = reader.ReadBuffer(LengthFormat.VarInt);
        MessageHash = reader.ReadBuffer(LengthFormat.VarInt);
    }
}