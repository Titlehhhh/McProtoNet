using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("MessageHeader", PacketState.Play, PacketDirection.Clientbound)]
public sealed partial class MessageHeaderPacket : IServerPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(760, 760),
    };

    public byte[]? PreviousSignature { get; set; }
    public Guid SenderUuid { get; set; }
    public byte[] Signature { get; set; }
    public byte[] MessageHash { get; set; }

    internal void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= 760 and <= 760:
                if (PreviousSignature is null)
                {
                    writer.WriteBoolean(false);
                }
                else
                {
                    writer.WriteBoolean(true);
                    writer.WriteVarInt(PreviousSignature.Length);
                    writer.WriteBuffer(PreviousSignature);
                }
                writer.WriteUUID(SenderUuid);
                writer.WriteVarInt(Signature.Length);
                writer.WriteBuffer(Signature);
                writer.WriteVarInt(MessageHash.Length);
                writer.WriteBuffer(MessageHash);
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.MessageHeader), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= 760 and <= 760:
                PreviousSignature = reader.ReadOptional((ref MinecraftPrimitiveReader r) => r.ReadBuffer(LengthFormat.VarInt));
                SenderUuid = reader.ReadUUID();
                Signature = reader.ReadBuffer(LengthFormat.VarInt);
                MessageHash = reader.ReadBuffer(LengthFormat.VarInt);
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.MessageHeader), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    void IPacket.Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(ref writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);
}
