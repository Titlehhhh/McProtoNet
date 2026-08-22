using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
using System;

namespace McProtoNet.Protocol;
[ProtocolSupport(760, MinecraftVersion.LatestProtocol)]
public sealed partial class PreviousMessage : IProtocolType<PreviousMessage>
{
    public Guid MessageSender { get; }
    public byte[] MessageSignature { get; }
    public int Id { get; }
    public byte[]? Signature { get; }

    public PreviousMessage(Guid messageSender, byte[] messageSignature, int id, byte[]? signature)
    {
        MessageSender = messageSender;
        MessageSignature = messageSignature;
        Id = id;
        Signature = signature;
    }

    public static PreviousMessage Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<PreviousMessage>(protocolVersion);
        if (protocolVersion >= 760 && protocolVersion <= 760)
        {
            var messageSender = reader.ReadUUID();
            var messageSignature = reader.ReadByteArray();
            return new PreviousMessage(messageSender, messageSignature, default!, default!);
        }

        if (protocolVersion >= 761)
        {
            var id = reader.ReadVarInt();
            byte[]? signature = default;
            if (id == 0)
            {
                var signatureValue = reader.ReadFixedBytes(256);
                signature = signatureValue;
            }

            return new PreviousMessage(default!, default!, id, signature);
        }

        throw new System.NotSupportedException($"PreviousMessage has no wire layout for protocol version {protocolVersion}.");
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<PreviousMessage>(protocolVersion);
        if (protocolVersion >= 760 && protocolVersion <= 760)
        {
            writer.WriteUUID(MessageSender);
            writer.WriteByteArray(MessageSignature);
            return;
        }

        if (protocolVersion >= 761)
        {
            writer.WriteVarInt(Id);
            if (Id == 0)
            {
                writer.WriteFixedBytes((Signature ?? throw new System.InvalidOperationException("Signature is required at this protocol version.")), 256);
            }
            else if (Signature is not null)
            {
                throw new System.InvalidOperationException("Signature is set, but 'id' does not select it at this protocol version.");
            }

            return;
        }

        throw new System.NotSupportedException($"PreviousMessage has no wire layout for protocol version {protocolVersion}.");
    }
}
