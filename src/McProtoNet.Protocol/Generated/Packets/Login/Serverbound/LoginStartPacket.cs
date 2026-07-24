using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Login.Serverbound;
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
public sealed partial class LoginStartPacket : IProtocolType<LoginStartPacket>
{
    public string Username { get; }
    public LoginSignature? Signature { get; }
    public Guid? PlayerUuid { get; }

    public LoginStartPacket(string username, LoginSignature? signature, Guid? playerUuid)
    {
        Username = username;
        Signature = signature;
        PlayerUuid = playerUuid;
    }

    public static LoginStartPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<LoginStartPacket>(protocolVersion);
        if (protocolVersion <= 758)
        {
            var username = reader.ReadString();
            return new LoginStartPacket(username, default!, default!);
        }

        if (protocolVersion >= 759 && protocolVersion <= 759)
        {
            var username = reader.ReadString();
            LoginSignature? signature = null;
            if (reader.ReadBoolean())
                signature = reader.ReadType<LoginSignature>(protocolVersion);
            return new LoginStartPacket(username, signature, default!);
        }

        if (protocolVersion >= 760 && protocolVersion <= 760)
        {
            var username = reader.ReadString();
            LoginSignature? signature = null;
            if (reader.ReadBoolean())
                signature = reader.ReadType<LoginSignature>(protocolVersion);
            Guid? playerUuid = null;
            if (reader.ReadBoolean())
                playerUuid = reader.ReadUUID();
            return new LoginStartPacket(username, signature, playerUuid);
        }

        if (protocolVersion >= 761 && protocolVersion <= 763)
        {
            var username = reader.ReadString();
            Guid? playerUuid = null;
            if (reader.ReadBoolean())
                playerUuid = reader.ReadUUID();
            return new LoginStartPacket(username, default!, playerUuid);
        }

        if (protocolVersion >= 764)
        {
            var username = reader.ReadString();
            var playerUuid = reader.ReadUUID();
            return new LoginStartPacket(username, default!, playerUuid);
        }

        throw new System.NotSupportedException($"LoginStartPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<LoginStartPacket>(protocolVersion);
        if (protocolVersion <= 758)
        {
            writer.WriteString(Username);
            return;
        }

        if (protocolVersion >= 759 && protocolVersion <= 759)
        {
            writer.WriteString(Username);
            writer.WriteBoolean(Signature is not null);
            if (Signature is { } signatureValue)
                writer.WriteType<LoginSignature>(signatureValue, protocolVersion);
            return;
        }

        if (protocolVersion >= 760 && protocolVersion <= 760)
        {
            writer.WriteString(Username);
            writer.WriteBoolean(Signature is not null);
            if (Signature is { } signatureValue)
                writer.WriteType<LoginSignature>(signatureValue, protocolVersion);
            writer.WriteBoolean(PlayerUuid is not null);
            if (PlayerUuid is { } playerUuidValue)
                writer.WriteUUID(playerUuidValue);
            return;
        }

        if (protocolVersion >= 761 && protocolVersion <= 763)
        {
            writer.WriteString(Username);
            writer.WriteBoolean(PlayerUuid is not null);
            if (PlayerUuid is { } playerUuidValue)
                writer.WriteUUID(playerUuidValue);
            return;
        }

        if (protocolVersion >= 764)
        {
            writer.WriteString(Username);
            writer.WriteUUID((PlayerUuid ?? throw new System.InvalidOperationException("PlayerUuid is required at this protocol version.")));
            return;
        }

        throw new System.NotSupportedException($"LoginStartPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public static int GetPacketId(int protocolVersion)
    {
        if (protocolVersion >= 735 && protocolVersion <= 763)
            return 0x00;
        if (protocolVersion >= 764 && protocolVersion <= 765)
            return 0x00;
        if (protocolVersion >= 766 && protocolVersion <= 772)
            return 0x00;
        throw new System.NotSupportedException($"No packet id for protocol {protocolVersion}.");
    }
}
