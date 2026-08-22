using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
using System;

namespace McProtoNet.Protocol.Packets.Play.Serverbound;
[ProtocolSupport(761, MinecraftVersion.LatestProtocol)]
[Packet("play.toServer.chat_session_update", PacketPhase.Play, PacketDirection.Serverbound)]
[PacketField("SessionUuid", "Guid")]
[PacketField("ExpireTime", "long")]
[PacketField("PublicKey", "byte[]")]
[PacketField("Signature", "byte[]")]
public sealed partial record ChatSessionUpdatePacket(Guid SessionUuid, long ExpireTime, byte[] PublicKey, byte[] Signature) : IPacket<ChatSessionUpdatePacket>, IPacket
{
    public static ChatSessionUpdatePacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ChatSessionUpdatePacket>(protocolVersion);
        var sessionUuid = reader.ReadUUID();
        var expireTime = reader.ReadSignedLong();
        var publicKey = reader.ReadByteArray();
        var signature = reader.ReadByteArray();
        return new ChatSessionUpdatePacket(sessionUuid, expireTime, publicKey, signature);
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ChatSessionUpdatePacket>(protocolVersion);
        writer.WriteUUID(SessionUuid);
        writer.WriteSignedLong(ExpireTime);
        writer.WriteByteArray(PublicKey);
        writer.WriteByteArray(Signature);
    }

    public static PacketIdentity Identity => new("play.toServer.chat_session_update", "ChatSessionUpdate", PacketPhase.Play, PacketDirection.Serverbound, 11);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 761 && protocolVersion <= 761)
        {
            id = 0x20;
            return true;
        }

        if (protocolVersion >= 762 && protocolVersion <= 765)
        {
            id = 0x06;
            return true;
        }

        if (protocolVersion >= 766 && protocolVersion <= 767)
        {
            id = 0x07;
            return true;
        }

        if (protocolVersion >= 768 && protocolVersion <= 770)
        {
            id = 0x08;
            return true;
        }

        if (protocolVersion >= 771 && protocolVersion <= 774)
        {
            id = 0x09;
            return true;
        }

        if (protocolVersion >= 775 && protocolVersion <= 776)
        {
            id = 0x0A;
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
