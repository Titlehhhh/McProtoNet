using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[ProtocolSupport(760, 760)]
[Packet("play.toClient.message_header", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("PreviousSignature", "byte[]?")]
[PacketField("SenderUuid", "Guid")]
[PacketField("Signature", "byte[]")]
[PacketField("MessageHash", "byte[]")]
public sealed partial record MessageHeaderPacket(byte[]? PreviousSignature, Guid SenderUuid, byte[] Signature, byte[] MessageHash) : IPacket<MessageHeaderPacket>, IPacket
{
    public static MessageHeaderPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<MessageHeaderPacket>(protocolVersion);
        byte[]? previousSignature = null;
        if (reader.ReadBoolean())
            previousSignature = reader.ReadByteArray();
        var senderUuid = reader.ReadUUID();
        var signature = reader.ReadByteArray();
        var messageHash = reader.ReadByteArray();
        return new MessageHeaderPacket(previousSignature, senderUuid, signature, messageHash);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<MessageHeaderPacket>(protocolVersion);
        writer.WriteBoolean(PreviousSignature is not null);
        if (PreviousSignature is { } previousSignatureValue)
            writer.WriteByteArray(previousSignatureValue);
        writer.WriteUUID(SenderUuid);
        writer.WriteByteArray(Signature);
        writer.WriteByteArray(MessageHash);
    }

    public static PacketIdentity Identity => new("play.toClient.message_header", "MessageHeader", PacketPhase.Play, PacketDirection.Clientbound, 60);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        return PacketRegistry.TryGetId(Identity, protocolVersion, out id);
    }

    public static int GetPacketId(int protocolVersion)
    {
        return PacketRegistry.GetId(Identity, protocolVersion);
    }
}
