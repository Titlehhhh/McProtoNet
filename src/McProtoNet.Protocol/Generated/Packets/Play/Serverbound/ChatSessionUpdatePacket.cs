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

    public static PacketIdentity Identity => new("play.toServer.chat_session_update", "ChatSessionUpdate", PacketPhase.Play, PacketDirection.Serverbound, 12);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        return PacketRegistry.TryGetId(Identity, protocolVersion, out id);
    }

    public static int GetPacketId(int protocolVersion)
    {
        return PacketRegistry.GetId(Identity, protocolVersion);
    }
}
