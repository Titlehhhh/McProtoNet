using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Login.Clientbound;
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
public sealed partial class EncryptionRequestPacket : IProtocolType<EncryptionRequestPacket>
{
    public string ServerId { get; }
    public byte[] PublicKey { get; }
    public byte[] VerifyToken { get; }
    public bool ShouldAuthenticate { get; }

    public EncryptionRequestPacket(string serverId, byte[] publicKey, byte[] verifyToken, bool shouldAuthenticate)
    {
        ServerId = serverId;
        PublicKey = publicKey;
        VerifyToken = verifyToken;
        ShouldAuthenticate = shouldAuthenticate;
    }

    public static EncryptionRequestPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<EncryptionRequestPacket>(protocolVersion);
        if (protocolVersion <= 765)
        {
            var serverId = reader.ReadString();
            var publicKey = reader.ReadByteArray();
            var verifyToken = reader.ReadByteArray();
            return new EncryptionRequestPacket(serverId, publicKey, verifyToken, default!);
        }

        if (protocolVersion >= 766)
        {
            var serverId = reader.ReadString();
            var publicKey = reader.ReadByteArray();
            var verifyToken = reader.ReadByteArray();
            var shouldAuthenticate = reader.ReadBoolean();
            return new EncryptionRequestPacket(serverId, publicKey, verifyToken, shouldAuthenticate);
        }

        throw new System.NotSupportedException($"EncryptionRequestPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<EncryptionRequestPacket>(protocolVersion);
        if (protocolVersion <= 765)
        {
            writer.WriteString(ServerId);
            writer.WriteByteArray(PublicKey);
            writer.WriteByteArray(VerifyToken);
            return;
        }

        if (protocolVersion >= 766)
        {
            writer.WriteString(ServerId);
            writer.WriteByteArray(PublicKey);
            writer.WriteByteArray(VerifyToken);
            writer.WriteBoolean(ShouldAuthenticate);
            return;
        }

        throw new System.NotSupportedException($"EncryptionRequestPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public static int GetPacketId(int protocolVersion)
    {
        if (protocolVersion >= 735 && protocolVersion <= 765)
            return 0x01;
        if (protocolVersion >= 766 && protocolVersion <= 772)
            return 0x01;
        throw new System.NotSupportedException($"No packet id for protocol {protocolVersion}.");
    }
}
