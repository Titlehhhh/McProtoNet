using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
using System.Text.Json;
using System;

namespace McProtoNet.Protocol;

[ProtocolSupport(760, 760)]
public sealed partial class LastRejectedMessage : IProtocolType<LastRejectedMessage>
{
    public Guid Sender { get; }
    public byte[] Signature { get; }

    public LastRejectedMessage(Guid sender, byte[] signature)
    {
        Sender = sender;
        Signature = signature;
    }

    public static LastRejectedMessage Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<LastRejectedMessage>(protocolVersion);
        var sender = reader.ReadUUID();
        var signature = reader.ReadByteArray();
        return new LastRejectedMessage(sender, signature);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<LastRejectedMessage>(protocolVersion);
        writer.WriteUUID(Sender);
        writer.WriteByteArray(Signature);
    }

    public void WriteJson(Utf8JsonWriter writer)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("Sender");
        writer.WriteStringValue(Sender);
        writer.WritePropertyName("Signature");
        writer.WriteBase64StringValue(Signature);
        writer.WriteEndObject();
    }
}
