using McProtoNet.Protocol;
using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Login.Clientbound;

[PacketInfo("EncryptionBegin", PacketState.Login, PacketDirection.Clientbound)]
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[PacketId(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol, 0x01)]
public sealed partial class EncryptionBeginPacket : IServerPacket
{
    public string ServerId { get; set; }
    public byte[] PublicKey { get; set; }
    public byte[] VerifyToken { get; set; }
    public bool? ShouldAuthenticate { get; set; }

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        writer.WriteString(ServerId);
        writer.WriteBuffer<VarInt>(PublicKey);
        writer.WriteBuffer<VarInt>(VerifyToken);
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 765:
                return;
            case >= 766 and <= MinecraftVersion.LatestProtocol:
                writer.WriteBoolean(ShouldAuthenticate ?? false);
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(EncryptionBeginPacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ServerId = reader.ReadString();
        PublicKey = reader.ReadArray<byte>(LengthFormat.VarInt);
        VerifyToken = reader.ReadArray<byte>(LengthFormat.VarInt);
        switch (protocolVersion)
        {
            case >= MinecraftVersion.StartProtocol and <= 765:
                ShouldAuthenticate = null;
                return;
            case >= 766 and <= MinecraftVersion.LatestProtocol:
                ShouldAuthenticate = reader.ReadBoolean();
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(EncryptionBeginPacket), protocolVersion, SupportedVersions);
                return;
        }
    }

    void IPacket.Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);
}