using System;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Login.Serverbound;

[PacketInfo("EncryptionBegin", PacketState.Login, PacketDirection.Serverbound)]
public sealed partial class EncryptionBeginPacket : IClientPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(MinecraftVersion.StartProtocol, 758),
        new(759, 760),
        new(761, MinecraftVersion.LatestProtocol)
    };

    public byte[] SharedSecret { get; set; } = Array.Empty<byte>();
    public byte[]? VerifyToken { get; set; }

    public V759_760Fields? V759_760 { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 758:
            case >= 761 and <= MinecraftVersion.LatestProtocol:
                if (VerifyToken is null)
                {
                    throw new InvalidOperationException("EncryptionBegin verify token missing.");
                }
                writer.WriteVarInt(SharedSecret.Length);
                writer.WriteBuffer(SharedSecret);
                writer.WriteVarInt(VerifyToken.Length);
                writer.WriteBuffer(VerifyToken);
                return;
            case >= 759 and <= 760:
                writer.WriteVarInt(SharedSecret.Length);
                writer.WriteBuffer(SharedSecret);
                if (VerifyToken is not null)
                {
                    writer.WriteBoolean(true);
                    writer.WriteVarInt(VerifyToken.Length);
                    writer.WriteBuffer(VerifyToken);
                }
                else
                {
                    var fields = V759_760 ?? throw new InvalidOperationException("EncryptionBegin V759_760 missing.");
                    writer.WriteBoolean(false);
                    writer.WriteSignedLong(fields.Salt);
                    writer.WriteVarInt(fields.MessageSignature.Length);
                    writer.WriteBuffer(fields.MessageSignature);
                }
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ClientLoginPacket.EncryptionBegin), protocolVersion, SupportedVersionsStatic);
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 758:
            case >= 761 and <= MinecraftVersion.LatestProtocol:
                SharedSecret = reader.ReadBuffer(reader.ReadVarInt());
                VerifyToken = reader.ReadBuffer(reader.ReadVarInt());
                return;
            case >= 759 and <= 760:
            {
                SharedSecret = reader.ReadBuffer(reader.ReadVarInt());
                bool hasVerifyToken = reader.ReadBoolean();
                if (hasVerifyToken)
                {
                    VerifyToken = reader.ReadBuffer(reader.ReadVarInt());
                }
                else
                {
                    VerifyToken = null;
                    V759_760 = new V759_760Fields
                    {
                        Salt = reader.ReadSignedLong(),
                        MessageSignature = reader.ReadBuffer(reader.ReadVarInt())
                    };
                }
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ClientLoginPacket.EncryptionBegin), protocolVersion, SupportedVersionsStatic);
        }
    }

    void IPacket.Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);

    public struct V759_760Fields
    {
        public long Salt { get; set; }
        public byte[] MessageSignature { get; set; }
    }
}