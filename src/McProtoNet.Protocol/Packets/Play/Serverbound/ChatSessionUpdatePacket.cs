using System;
using McProtoNet.Protocol;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;

[PacketInfo("ChatSessionUpdate", PacketState.Play, PacketDirection.Serverbound)]
public sealed partial class ChatSessionUpdatePacket : IClientPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(761, MinecraftVersion.LatestProtocol)
    };

    public Guid SessionUUID { get; set; }
    public long ExpireTime { get; set; }
    public byte[] PublicKey { get; set; } = Array.Empty<byte>();
    public byte[] Signature { get; set; } = Array.Empty<byte>();

    internal void Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= 761 and <= MinecraftVersion.LatestProtocol:
                writer.WriteUUID(SessionUUID);
                writer.WriteSignedLong(ExpireTime);
                writer.WriteVarInt(PublicKey.Length);
                writer.WriteBuffer(PublicKey);
                writer.WriteVarInt(Signature.Length);
                writer.WriteBuffer(Signature);
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ClientPlayPacket.ChatSessionUpdate), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case >= 761 and <= MinecraftVersion.LatestProtocol:
                SessionUUID = reader.ReadUUID();
                ExpireTime = reader.ReadSignedLong();
                PublicKey = reader.ReadBuffer(reader.ReadVarInt());
                Signature = reader.ReadBuffer(reader.ReadVarInt());
                return;
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ClientPlayPacket.ChatSessionUpdate), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    void IPacket.Serialize(MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);
}
