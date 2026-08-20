using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Login.Serverbound;
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("login.toServer.login_start", PacketPhase.Login, PacketDirection.Serverbound)]
[PacketField("Username", "string")]
[PacketField("Signature", "LoginSignature?", Group = "V759", From = 759, To = 759)]
[PacketField("Signature", "LoginSignature?", Group = "V760", From = 760, To = 760)]
[PacketField("PlayerUuid", "Guid?", Group = "V760", From = 760, To = 760)]
[PacketField("PlayerUuid", "Guid?", Group = "V761_763", From = 761, To = 763)]
[PacketField("PlayerUuid", "Guid", Group = "V764_Last", From = 764)]
public sealed partial record LoginStartPacket(string Username, LoginStartPacket.V759Layer? V759 = null, LoginStartPacket.V760Layer? V760 = null, LoginStartPacket.V761_763Layer? V761_763 = null, LoginStartPacket.V764_LastLayer? V764_Last = null) : IPacket<LoginStartPacket>, IPacket
{
    public readonly record struct V759Layer(LoginSignature? Signature);
    public readonly record struct V760Layer(LoginSignature? Signature, Guid? PlayerUuid);
    public readonly record struct V761_763Layer(Guid? PlayerUuid);
    public readonly record struct V764_LastLayer(Guid PlayerUuid);
    public static LoginStartPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<LoginStartPacket>(protocolVersion);
        if (protocolVersion <= 758)
        {
            var username = reader.ReadString();
            return new LoginStartPacket(username);
        }

        if (protocolVersion >= 759 && protocolVersion <= 759)
        {
            var username = reader.ReadString();
            LoginSignature? signature = null;
            if (reader.ReadBoolean())
                signature = reader.ReadType<LoginSignature>(protocolVersion);
            return new LoginStartPacket(username, V759: new V759Layer(signature));
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
            return new LoginStartPacket(username, V760: new V760Layer(signature, playerUuid));
        }

        if (protocolVersion >= 761 && protocolVersion <= 763)
        {
            var username = reader.ReadString();
            Guid? playerUuid = null;
            if (reader.ReadBoolean())
                playerUuid = reader.ReadUUID();
            return new LoginStartPacket(username, V761_763: new V761_763Layer(playerUuid));
        }

        if (protocolVersion >= 764)
        {
            var username = reader.ReadString();
            var playerUuid = reader.ReadUUID();
            return new LoginStartPacket(username, V764_Last: new V764_LastLayer(playerUuid));
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
            var layer = V759 ?? throw new WrongLayerException("LoginStartPacket", protocolVersion, "V759");
            LoginSignature? Signature = layer.Signature;
            writer.WriteString(Username);
            writer.WriteBoolean(Signature is not null);
            if (Signature is { } signatureValue)
                writer.WriteType<LoginSignature>(signatureValue, protocolVersion);
            return;
        }

        if (protocolVersion >= 760 && protocolVersion <= 760)
        {
            var layer = V760 ?? throw new WrongLayerException("LoginStartPacket", protocolVersion, "V760");
            LoginSignature? Signature = layer.Signature;
            Guid? PlayerUuid = layer.PlayerUuid;
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
            var layer = V761_763 ?? throw new WrongLayerException("LoginStartPacket", protocolVersion, "V761_763");
            Guid? PlayerUuid = layer.PlayerUuid;
            writer.WriteString(Username);
            writer.WriteBoolean(PlayerUuid is not null);
            if (PlayerUuid is { } playerUuidValue)
                writer.WriteUUID(playerUuidValue);
            return;
        }

        if (protocolVersion >= 764)
        {
            var layer = V764_Last ?? throw new WrongLayerException("LoginStartPacket", protocolVersion, "V764_Last");
            Guid? PlayerUuid = layer.PlayerUuid;
            writer.WriteString(Username);
            writer.WriteUUID((PlayerUuid ?? throw new System.InvalidOperationException("PlayerUuid is required at this protocol version.")));
            return;
        }

        throw new System.NotSupportedException($"LoginStartPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public static PacketIdentity Identity => new("login.toServer.login_start", "LoginStart", PacketPhase.Login, PacketDirection.Serverbound, 4);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 735 && protocolVersion <= 776)
        {
            id = 0x00;
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
