using System;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Login.Serverbound;

[PacketInfo("LoginStart", PacketState.Login, PacketDirection.Serverbound)]
public sealed partial class LoginStartPacket : IClientPacket
{
    public string Username { get; set; } = string.Empty;

    public V759Fields? V759 { get; set; }
    public V760Fields? V760 { get; set; }
    public V761_763Fields? V761_763 { get; set; }
    public V764_LastFields? V764_Last { get; set; }

    internal void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 758:
                writer.WriteString(Username);
                return;
            case 759:
            {
                var fields = V759 ?? throw new InvalidOperationException("LoginStart V759 missing.");
                writer.WriteString(Username);
                writer.WriteBoolean(fields.HasSignature);
                if (fields.HasSignature)
                {
                    if (fields.Timestamp is null || fields.PublicKey is null || fields.Signature is null)
                    {
                        throw new InvalidOperationException("LoginStart signature data missing.");
                    }
                    writer.WriteSignedLong(fields.Timestamp.Value);
                    writer.WriteVarInt(fields.PublicKey.Length);
                    writer.WriteBuffer(fields.PublicKey);
                    writer.WriteVarInt(fields.Signature.Length);
                    writer.WriteBuffer(fields.Signature);
                }
                return;
            }
            case 760:
            {
                var fields = V760 ?? throw new InvalidOperationException("LoginStart V760 missing.");
                writer.WriteString(Username);
                writer.WriteBoolean(fields.HasSignature);
                if (fields.HasSignature)
                {
                    if (fields.Timestamp is null || fields.PublicKey is null || fields.Signature is null)
                    {
                        throw new InvalidOperationException("LoginStart signature data missing.");
                    }
                    writer.WriteSignedLong(fields.Timestamp.Value);
                    writer.WriteVarInt(fields.PublicKey.Length);
                    writer.WriteBuffer(fields.PublicKey);
                    writer.WriteVarInt(fields.Signature.Length);
                    writer.WriteBuffer(fields.Signature);
                }
                writer.WriteBoolean(fields.HasPlayerUuid);
                if (fields.HasPlayerUuid)
                {
                    if (fields.PlayerUuid is null)
                    {
                        throw new InvalidOperationException("LoginStart player uuid missing.");
                    }
                    writer.WriteUUID(fields.PlayerUuid.Value);
                }
                return;
            }
            case >= 761 and <= 763:
            {
                var fields = V761_763 ?? throw new InvalidOperationException("LoginStart V761_763 missing.");
                writer.WriteString(Username);
                writer.WriteBoolean(fields.HasPlayerUuid);
                if (fields.HasPlayerUuid)
                {
                    if (fields.PlayerUuid is null)
                    {
                        throw new InvalidOperationException("LoginStart player uuid missing.");
                    }
                    writer.WriteUUID(fields.PlayerUuid.Value);
                }
                return;
            }
            case >= 764 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = V764_Last ?? throw new InvalidOperationException("LoginStart V764_Last missing.");
                writer.WriteString(Username);
                writer.WriteUUID(fields.PlayerUuid);
                return;
            }
            default:
                throw new ProtocolNotSupportException(nameof(ClientLoginPacket.LoginStart), protocolVersion);
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 758:
                Username = reader.ReadString();
                return;
            case 759:
            {
                Username = reader.ReadString();
                bool hasSignature = reader.ReadBoolean();
                long? timestamp = null;
                byte[]? publicKey = null;
                byte[]? signature = null;
                if (hasSignature)
                {
                    timestamp = reader.ReadSignedLong();
                    publicKey = reader.ReadBuffer(reader.ReadVarInt());
                    signature = reader.ReadBuffer(reader.ReadVarInt());
                }
                V759 = new V759Fields
                {
                    HasSignature = hasSignature,
                    Timestamp = timestamp,
                    PublicKey = publicKey,
                    Signature = signature
                };
                return;
            }
            case 760:
            {
                Username = reader.ReadString();
                bool hasSignature = reader.ReadBoolean();
                long? timestamp = null;
                byte[]? publicKey = null;
                byte[]? signature = null;
                if (hasSignature)
                {
                    timestamp = reader.ReadSignedLong();
                    publicKey = reader.ReadBuffer(reader.ReadVarInt());
                    signature = reader.ReadBuffer(reader.ReadVarInt());
                }
                bool hasPlayerUuid = reader.ReadBoolean();
                Guid? playerUuid = hasPlayerUuid ? reader.ReadUUID() : null;
                V760 = new V760Fields
                {
                    HasSignature = hasSignature,
                    Timestamp = timestamp,
                    PublicKey = publicKey,
                    Signature = signature,
                    HasPlayerUuid = hasPlayerUuid,
                    PlayerUuid = playerUuid
                };
                return;
            }
            case >= 761 and <= 763:
            {
                Username = reader.ReadString();
                bool hasPlayerUuid = reader.ReadBoolean();
                Guid? playerUuid = hasPlayerUuid ? reader.ReadUUID() : null;
                V761_763 = new V761_763Fields
                {
                    HasPlayerUuid = hasPlayerUuid,
                    PlayerUuid = playerUuid
                };
                return;
            }
            case >= 764 and <= MinecraftVersion.LatestProtocol:
                Username = reader.ReadString();
                V764_Last = new V764_LastFields
                {
                    PlayerUuid = reader.ReadUUID()
                };
                return;
            default:
                throw new ProtocolNotSupportException(nameof(ClientLoginPacket.LoginStart), protocolVersion);
        }
    }

    void IPacket.Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(ref writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);

    public struct V759Fields
    {
        public bool HasSignature { get; set; }
        public long? Timestamp { get; set; }
        public byte[]? PublicKey { get; set; }
        public byte[]? Signature { get; set; }
    }

    public struct V760Fields
    {
        public bool HasSignature { get; set; }
        public long? Timestamp { get; set; }
        public byte[]? PublicKey { get; set; }
        public byte[]? Signature { get; set; }
        public bool HasPlayerUuid { get; set; }
        public Guid? PlayerUuid { get; set; }
    }

    public struct V761_763Fields
    {
        public bool HasPlayerUuid { get; set; }
        public Guid? PlayerUuid { get; set; }
    }

    public struct V764_LastFields
    {
        public Guid PlayerUuid { get; set; }
    }
}
