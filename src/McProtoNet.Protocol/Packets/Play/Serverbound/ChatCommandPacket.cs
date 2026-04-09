using System;
using McProtoNet.Protocol;
using McProtoNet.Protocol.Extensions;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;

[PacketInfo("ChatCommand", PacketState.Play, PacketDirection.Serverbound)]
public sealed partial class ChatCommandPacket : IClientPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(759, 765),
        new(766, MinecraftVersion.LatestProtocol)
    };

    public string Command { get; set; } = string.Empty;

    public V759Fields? V759 { get; set; }
    public V760Fields? V760 { get; set; }
    public V761_765Fields? V761_765 { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case 759:
            {
                var fields = V759 ?? throw new InvalidOperationException("ChatCommand V759 missing.");
                writer.WriteString(Command);
                writer.WriteSignedLong(fields.Timestamp);
                writer.WriteSignedLong(fields.Salt);
                writer.WriteVarInt(fields.ArgumentSignatures.Length);
                for (int i = 0; i < fields.ArgumentSignatures.Length; i++)
                {
                    writer.WriteString(fields.ArgumentSignatures[i].ArgumentName);
                    writer.WriteVarInt(fields.ArgumentSignatures[i].Signature.Length);
                    writer.WriteBuffer(fields.ArgumentSignatures[i].Signature);
                }
                writer.WriteBoolean(fields.SignedPreview);
                return;
            }
            case 760:
            {
                var fields = V760 ?? throw new InvalidOperationException("ChatCommand V760 missing.");
                writer.WriteString(Command);
                writer.WriteSignedLong(fields.Timestamp);
                writer.WriteSignedLong(fields.Salt);
                writer.WriteVarInt(fields.ArgumentSignatures.Length);
                for (int i = 0; i < fields.ArgumentSignatures.Length; i++)
                {
                    writer.WriteString(fields.ArgumentSignatures[i].ArgumentName);
                    writer.WriteVarInt(fields.ArgumentSignatures[i].Signature.Length);
                    writer.WriteBuffer(fields.ArgumentSignatures[i].Signature);
                }
                writer.WriteBoolean(fields.SignedPreview);
                writer.WritePreviousMessages(fields.PreviousMessages, protocolVersion);
                if (fields.LastRejectedMessage is null)
                {
                    writer.WriteBoolean(false);
                }
                else
                {
                    writer.WriteBoolean(true);
                    writer.WriteUUID(fields.LastRejectedMessage.Value.Sender);
                    writer.WriteVarInt(fields.LastRejectedMessage.Value.Signature.Length);
                    writer.WriteBuffer(fields.LastRejectedMessage.Value.Signature);
                }
                return;
            }
            case >= 761 and <= 765:
            {
                var fields = V761_765 ?? throw new InvalidOperationException("ChatCommand V761_765 missing.");
                writer.WriteString(Command);
                writer.WriteSignedLong(fields.Timestamp);
                writer.WriteSignedLong(fields.Salt);
                writer.WriteVarInt(fields.ArgumentSignatures.Length);
                for (int i = 0; i < fields.ArgumentSignatures.Length; i++)
                {
                    if (fields.ArgumentSignatures[i].Signature.Length != 256)
                    {
                        throw new InvalidOperationException("ChatCommand signature must be 256 bytes.");
                    }
                    writer.WriteString(fields.ArgumentSignatures[i].ArgumentName);
                    writer.WriteBuffer(fields.ArgumentSignatures[i].Signature);
                }
                writer.WriteVarInt(fields.MessageCount);
                if (fields.Acknowledged.Length != 3)
                {
                    throw new InvalidOperationException("ChatCommand acknowledged must be 3 bytes.");
                }
                writer.WriteBuffer(fields.Acknowledged);
                return;
            }
            case >= 766 and <= MinecraftVersion.LatestProtocol:
                writer.WriteString(Command);
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ClientPlayPacket.ChatCommand), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case 759:
            {
                Command = reader.ReadString();
                long timestamp = reader.ReadSignedLong();
                long salt = reader.ReadSignedLong();
                int sigCount = reader.ReadVarInt();
                var signatures = sigCount == 0 ? Array.Empty<ArgumentSignature>() : new ArgumentSignature[sigCount];
                for (int i = 0; i < signatures.Length; i++)
                {
                    string argumentName = reader.ReadString();
                    byte[] signature = reader.ReadBuffer(reader.ReadVarInt());
                    signatures[i] = new ArgumentSignature
                    {
                        ArgumentName = argumentName,
                        Signature = signature
                    };
                }
                V759 = new V759Fields
                {
                    Timestamp = timestamp,
                    Salt = salt,
                    ArgumentSignatures = signatures,
                    SignedPreview = reader.ReadBoolean()
                };
                return;
            }
            case 760:
            {
                Command = reader.ReadString();
                long timestamp = reader.ReadSignedLong();
                long salt = reader.ReadSignedLong();
                int sigCount = reader.ReadVarInt();
                var signatures = sigCount == 0 ? Array.Empty<ArgumentSignature>() : new ArgumentSignature[sigCount];
                for (int i = 0; i < signatures.Length; i++)
                {
                    string argumentName = reader.ReadString();
                    byte[] signature = reader.ReadBuffer(reader.ReadVarInt());
                    signatures[i] = new ArgumentSignature
                    {
                        ArgumentName = argumentName,
                        Signature = signature
                    };
                }
                bool signedPreview = reader.ReadBoolean();
                PreviousMessages previousMessages = reader.ReadPreviousMessages(protocolVersion);
                bool hasLastRejected = reader.ReadBoolean();
                LastRejectedMessage? lastRejected = null;
                if (hasLastRejected)
                {
                    lastRejected = new LastRejectedMessage
                    {
                        Sender = reader.ReadUUID(),
                        Signature = reader.ReadBuffer(reader.ReadVarInt())
                    };
                }
                V760 = new V760Fields
                {
                    Timestamp = timestamp,
                    Salt = salt,
                    ArgumentSignatures = signatures,
                    SignedPreview = signedPreview,
                    PreviousMessages = previousMessages,
                    LastRejectedMessage = lastRejected
                };
                return;
            }
            case >= 761 and <= 765:
            {
                Command = reader.ReadString();
                long timestamp = reader.ReadSignedLong();
                long salt = reader.ReadSignedLong();
                int sigCount = reader.ReadVarInt();
                var signatures = sigCount == 0 ? Array.Empty<ArgumentSignature>() : new ArgumentSignature[sigCount];
                for (int i = 0; i < signatures.Length; i++)
                {
                    string argumentName = reader.ReadString();
                    byte[] signature = reader.ReadBuffer(256);
                    signatures[i] = new ArgumentSignature
                    {
                        ArgumentName = argumentName,
                        Signature = signature
                    };
                }
                V761_765 = new V761_765Fields
                {
                    Timestamp = timestamp,
                    Salt = salt,
                    ArgumentSignatures = signatures,
                    MessageCount = reader.ReadVarInt(),
                    Acknowledged = reader.ReadBuffer(3)
                };
                return;
            }
            case >= 766 and <= MinecraftVersion.LatestProtocol:
                Command = reader.ReadString();
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ClientPlayPacket.ChatCommand), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    void IPacket.Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);

    public struct V759Fields
    {
        public long Timestamp { get; set; }
        public long Salt { get; set; }
        public ArgumentSignature[] ArgumentSignatures { get; set; }
        public bool SignedPreview { get; set; }
    }

    public struct V760Fields
    {
        public long Timestamp { get; set; }
        public long Salt { get; set; }
        public ArgumentSignature[] ArgumentSignatures { get; set; }
        public bool SignedPreview { get; set; }
        public PreviousMessages PreviousMessages { get; set; }
        public LastRejectedMessage? LastRejectedMessage { get; set; }
    }

    public struct V761_765Fields
    {
        public long Timestamp { get; set; }
        public long Salt { get; set; }
        public ArgumentSignature[] ArgumentSignatures { get; set; }
        public int MessageCount { get; set; }
        public byte[] Acknowledged { get; set; }
    }

    public struct ArgumentSignature
    {
        public string ArgumentName { get; set; }
        public byte[] Signature { get; set; }
    }

    public struct LastRejectedMessage
    {
        public Guid Sender { get; set; }
        public byte[] Signature { get; set; }
    }
}
