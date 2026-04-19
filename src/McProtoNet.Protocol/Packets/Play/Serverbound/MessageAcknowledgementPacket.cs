using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;

[PacketInfo("MessageAcknowledgement", PacketState.Play, PacketDirection.Serverbound)]
[ProtocolSupport(760, MinecraftVersion.LatestProtocol)]
[PacketId(760, 767, 0x03)]
[PacketId(768, 770, 0x04)]
[PacketId(771, MinecraftVersion.LatestProtocol, 0x05)]
public sealed partial class MessageAcknowledgementPacket : IClientPacket
{
    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 759:
            {
                return;
            }
            case >= 760 and <= 760:
            {
                var fields = V760_760 ?? throw new InvalidOperationException("MessageAcknowledgementPacket 760 fields missing.");
                writer.WriteType<PreviousMessages>(fields.PreviousMessages, protocolVersion);
                writer.WriteUUID(fields.Sender);
                writer.WriteBuffer<VarInt>(fields.Signature);
                return;
            }
            case >= 761 and <= MinecraftVersion.LatestProtocol:
            {
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(MessageAcknowledgementPacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 759:
            {
                return;
            }
            case >= 760 and <= 760:
            {
                V760_760 = new V760_760Fields
                {
                    PreviousMessages = reader.ReadType<PreviousMessages>(protocolVersion),
                    Sender = reader.ReadUUID(),
                    Signature = reader.ReadBuffer(LengthFormat.VarInt)
                };
                return;
            }
            case >= 761 and <= MinecraftVersion.LatestProtocol:
            {
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(MessageAcknowledgementPacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    public struct V760_760Fields
    {
        public PreviousMessages PreviousMessages { get; set; }
        public Guid Sender { get; set; }
        public byte[] Signature { get; set; }
    }

    public V760_760Fields? V760_760 { get; set; }
}