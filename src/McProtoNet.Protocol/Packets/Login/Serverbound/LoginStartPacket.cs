using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Login.Serverbound;

[PacketInfo("LoginStart", PacketState.Login, PacketDirection.Serverbound)]
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[PacketId(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol, 0x00)]
public sealed partial class LoginStartPacket : IClientPacket
{
    public string Username { get; set; }

    public V759_759Fields? V759_759 { get; set; }
    public V760_760Fields? V760_760 { get; set; }
    public V761_763Fields? V761_763 { get; set; }
    public V764_LastFields? V764_Last { get; set; }

    internal void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 758:
                writer.WriteString(Username);
                return;
            case >= 759 and <= 759:
            {
                var fields = V759_759 ?? throw new InvalidOperationException("LoginStartPacket 759-759 fields missing.");
                writer.WriteString(Username);
                writer.WriteBoolean(fields.HasSignature);
                if (fields.HasSignature)
                {
                    writer.WriteSignedLong(fields.Timestamp);
                    writer.WriteVarInt(fields.PublicKey.Length); writer.WriteBuffer(fields.PublicKey);
                    writer.WriteVarInt(fields.Signature.Length); writer.WriteBuffer(fields.Signature);
                }
                return;
            }
            case >= 760 and <= 760:
            {
                var fields = V760_760 ?? throw new InvalidOperationException("LoginStartPacket 760-760 fields missing.");
                writer.WriteString(Username);
                writer.WriteBoolean(fields.HasSignature);
                if (fields.HasSignature)
                {
                    writer.WriteSignedLong(fields.Timestamp);
                    writer.WriteVarInt(fields.PublicKey.Length); writer.WriteBuffer(fields.PublicKey);
                    writer.WriteVarInt(fields.Signature.Length); writer.WriteBuffer(fields.Signature);
                }
                writer.WriteBoolean(fields.PlayerUUID.HasValue);
                if (fields.PlayerUUID.HasValue)
                {
                    writer.WriteUUID(fields.PlayerUUID.Value);
                }
                return;
            }
            case >= 761 and <= 763:
            {
                var fields = V761_763 ?? throw new InvalidOperationException("LoginStartPacket 761-763 fields missing.");
                writer.WriteString(Username);
                writer.WriteBoolean(fields.PlayerUUID.HasValue);
                if (fields.PlayerUUID.HasValue)
                {
                    writer.WriteUUID(fields.PlayerUUID.Value);
                }
                return;
            }
            case >= 764 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = V764_Last ?? throw new InvalidOperationException("LoginStartPacket 764-last fields missing.");
                writer.WriteString(Username);
                writer.WriteUUID(fields.PlayerUUID);
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(LoginStartPacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 758:
                Username = reader.ReadString();
                V759_759 = null;
                V760_760 = null;
                V761_763 = null;
                V764_Last = null;
                return;
            case >= 759 and <= 759:
                Username = reader.ReadString();
                var fields759 = new V759_759Fields
                {
                    HasSignature = reader.ReadBoolean()
                };
                if (fields759.HasSignature)
                {
                    fields759.Timestamp = reader.ReadSignedLong();
                    fields759.PublicKey = reader.ReadBuffer(reader.ReadVarInt());
                    fields759.Signature = reader.ReadBuffer(reader.ReadVarInt());
                }
                V759_759 = fields759;
                V760_760 = null;
                V761_763 = null;
                V764_Last = null;
                return;
            case >= 760 and <= 760:
                Username = reader.ReadString();
                var fields760 = new V760_760Fields
                {
                    HasSignature = reader.ReadBoolean()
                };
                if (fields760.HasSignature)
                {
                    fields760.Timestamp = reader.ReadSignedLong();
                    fields760.PublicKey = reader.ReadBuffer(reader.ReadVarInt());
                    fields760.Signature = reader.ReadBuffer(reader.ReadVarInt());
                }
                fields760.PlayerUUID = reader.ReadBoolean() ? reader.ReadUUID() : (Guid?)null;
                V760_760 = fields760;
                V759_759 = null;
                V761_763 = null;
                V764_Last = null;
                return;
            case >= 761 and <= 763:
                Username = reader.ReadString();
                var fields761 = new V761_763Fields
                {
                    PlayerUUID = reader.ReadBoolean() ? reader.ReadUUID() : (Guid?)null
                };
                V761_763 = fields761;
                V759_759 = null;
                V760_760 = null;
                V764_Last = null;
                return;
            case >= 764 and <= MinecraftVersion.LatestProtocol:
                Username = reader.ReadString();
                V764_Last = new V764_LastFields { PlayerUUID = reader.ReadUUID() };
                V759_759 = null;
                V760_760 = null;
                V761_763 = null;
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(LoginStartPacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    void IPacket.Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(ref writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);

    public struct V759_759Fields
    {
        public bool HasSignature { get; set; }
        public long Timestamp { get; set; }
        public byte[] PublicKey { get; set; }
        public byte[] Signature { get; set; }
    }

    public struct V760_760Fields
    {
        public bool HasSignature { get; set; }
        public long Timestamp { get; set; }
        public byte[] PublicKey { get; set; }
        public byte[] Signature { get; set; }
        public Guid? PlayerUUID { get; set; }
    }

    public struct V761_763Fields
    {
        public Guid? PlayerUUID { get; set; }
    }

    public struct V764_LastFields
    {
        public Guid PlayerUUID { get; set; }
    }
}