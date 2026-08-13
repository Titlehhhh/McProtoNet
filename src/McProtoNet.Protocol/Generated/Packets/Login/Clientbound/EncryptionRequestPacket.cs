using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Login.Clientbound;
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("login.toClient.encryption_begin", PacketPhase.Login, PacketDirection.Clientbound)]
[PacketField("ServerId", "string")]
[PacketField("PublicKey", "byte[]")]
[PacketField("VerifyToken", "byte[]")]
[PacketField("ShouldAuthenticate", "bool", Group = "V766_Last", From = 766)]
public sealed partial record EncryptionRequestPacket(string ServerId, byte[] PublicKey, byte[] VerifyToken, EncryptionRequestPacket.V766_LastLayer? V766_Last = null) : IPacket<EncryptionRequestPacket>, IPacket
{
    public readonly record struct V766_LastLayer(bool ShouldAuthenticate);
    public static EncryptionRequestPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<EncryptionRequestPacket>(protocolVersion);
        if (protocolVersion <= 765)
        {
            var serverId = reader.ReadString();
            var publicKey = reader.ReadByteArray();
            var verifyToken = reader.ReadByteArray();
            return new EncryptionRequestPacket(serverId, publicKey, verifyToken);
        }

        if (protocolVersion >= 766)
        {
            var serverId = reader.ReadString();
            var publicKey = reader.ReadByteArray();
            var verifyToken = reader.ReadByteArray();
            var shouldAuthenticate = reader.ReadBoolean();
            return new EncryptionRequestPacket(serverId, publicKey, verifyToken, V766_Last: new V766_LastLayer(shouldAuthenticate));
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
            var layer = V766_Last ?? throw new WrongLayerException("EncryptionRequestPacket", protocolVersion, "V766_Last");
            bool ShouldAuthenticate = layer.ShouldAuthenticate;
            writer.WriteString(ServerId);
            writer.WriteByteArray(PublicKey);
            writer.WriteByteArray(VerifyToken);
            writer.WriteBoolean(ShouldAuthenticate);
            return;
        }

        throw new System.NotSupportedException($"EncryptionRequestPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public static PacketIdentity Identity => new("login.toClient.encryption_begin", "EncryptionRequest", PacketPhase.Login, PacketDirection.Clientbound, 3);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 735 && protocolVersion <= 772)
        {
            id = 0x01;
            return true;
        }

        id = 0;
        return false;
    }

    public static int GetPacketId(int protocolVersion)
    {
        if (TryGetPacketId(protocolVersion, out var id))
            return id;
        throw new System.NotSupportedException($"No packet id for protocol {protocolVersion}.");
    }
}
