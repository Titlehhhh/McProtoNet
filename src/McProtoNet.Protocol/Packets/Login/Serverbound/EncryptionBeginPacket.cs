using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;
using McProtoNet.NBT;

namespace McProtoNet.Protocol.Packets.Login.Serverbound;

[PacketInfo("EncryptionBegin", PacketState.Login, PacketDirection.Serverbound)]
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[PacketId(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol, 0x01)]
public sealed partial class EncryptionBeginPacket : IClientPacket
{
    public byte[] SharedSecret { get; set; }
    public byte[]? VerifyToken { get; set; }
    public V759_760Fields? V759_760 { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 758:
            {
                writer.WriteBuffer(SharedSecret, LengthFormat.VarInt);
                writer.WriteBuffer(VerifyToken ?? throw new InvalidOperationException("EncryptionBeginPacket VerifyToken missing."), LengthFormat.VarInt);
                return;
            }
            case >= 759 and <= 760:
            {
                writer.WriteBuffer(SharedSecret, LengthFormat.VarInt);
                var fields = V759_760 ?? throw new InvalidOperationException("EncryptionBeginPacket 759-760 fields missing.");
                writer.WriteBoolean(fields.HasVerifyToken);
                if (fields.HasVerifyToken)
                {
                    writer.WriteBuffer(VerifyToken ?? throw new InvalidOperationException("EncryptionBeginPacket VerifyToken missing."), LengthFormat.VarInt);
                }
                else
                {
                    writer.WriteSignedLong(fields.Salt);
                    writer.WriteBuffer(fields.MessageSignature, LengthFormat.VarInt);
                }
                return;
            }
            case >= 761 and <= MinecraftVersion.LatestProtocol:
            {
                writer.WriteBuffer(SharedSecret, LengthFormat.VarInt);
                writer.WriteBuffer(VerifyToken ?? throw new InvalidOperationException("EncryptionBeginPacket VerifyToken missing."), LengthFormat.VarInt);
                return;
            }
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 758:
            {
                SharedSecret = reader.ReadBuffer(LengthFormat.VarInt);
                VerifyToken = reader.ReadBuffer(LengthFormat.VarInt);
                V759_760 = null;
                return;
            }
            case >= 759 and <= 760:
            {
                SharedSecret = reader.ReadBuffer(LengthFormat.VarInt);
                var hasVerifyToken = reader.ReadBoolean();
                var fields = new V759_760Fields { HasVerifyToken = hasVerifyToken };
                if (hasVerifyToken)
                {
                    VerifyToken = reader.ReadBuffer(LengthFormat.VarInt);
                }
                else
                {
                    fields.Salt = reader.ReadSignedLong();
                    fields.MessageSignature = reader.ReadBuffer(LengthFormat.VarInt);
                }
                V759_760 = fields;
                return;
            }
            case >= 761 and <= MinecraftVersion.LatestProtocol:
            {
                SharedSecret = reader.ReadBuffer(LengthFormat.VarInt);
                VerifyToken = reader.ReadBuffer(LengthFormat.VarInt);
                V759_760 = null;
                return;
            }
        }
    }

    public struct V759_760Fields
    {
        public bool HasVerifyToken { get; set; }
        public long Salt { get; set; }
        public byte[] MessageSignature { get; set; }
    }
}