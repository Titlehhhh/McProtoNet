using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Login.Clientbound;
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
public sealed partial class LoginSuccessPacket : IProtocolType<LoginSuccessPacket>
{
    public Guid Uuid { get; }
    public string Username { get; }
    public ProfileProperty[] Properties { get; }
    public bool StrictErrorHandling { get; }

    public LoginSuccessPacket(Guid uuid, string username, ProfileProperty[] properties, bool strictErrorHandling)
    {
        Uuid = uuid;
        Username = username;
        Properties = properties;
        StrictErrorHandling = strictErrorHandling;
    }

    public static LoginSuccessPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<LoginSuccessPacket>(protocolVersion);
        if (protocolVersion <= 758)
        {
            var uuid = reader.ReadUUID();
            var username = reader.ReadString();
            return new LoginSuccessPacket(uuid, username, default!, default!);
        }

        if (protocolVersion >= 759 && protocolVersion <= 765)
        {
            var uuid = reader.ReadUUID();
            var username = reader.ReadString();
            int propertiesCount = reader.ReadVarInt();
            var properties = new ProfileProperty[propertiesCount];
            for (int i = 0; i < properties.Length; i++)
                properties[i] = reader.ReadType<ProfileProperty>(protocolVersion);
            return new LoginSuccessPacket(uuid, username, properties, default!);
        }

        if (protocolVersion >= 766 && protocolVersion <= 767)
        {
            var uuid = reader.ReadUUID();
            var username = reader.ReadString();
            int propertiesCount = reader.ReadVarInt();
            var properties = new ProfileProperty[propertiesCount];
            for (int i = 0; i < properties.Length; i++)
                properties[i] = reader.ReadType<ProfileProperty>(protocolVersion);
            var strictErrorHandling = reader.ReadBoolean();
            return new LoginSuccessPacket(uuid, username, properties, strictErrorHandling);
        }

        if (protocolVersion >= 768)
        {
            var uuid = reader.ReadUUID();
            var username = reader.ReadString();
            int propertiesCount = reader.ReadVarInt();
            var properties = new ProfileProperty[propertiesCount];
            for (int i = 0; i < properties.Length; i++)
                properties[i] = reader.ReadType<ProfileProperty>(protocolVersion);
            return new LoginSuccessPacket(uuid, username, properties, default!);
        }

        throw new System.NotSupportedException($"LoginSuccessPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<LoginSuccessPacket>(protocolVersion);
        if (protocolVersion <= 758)
        {
            writer.WriteUUID(Uuid);
            writer.WriteString(Username);
            return;
        }

        if (protocolVersion >= 759 && protocolVersion <= 765)
        {
            writer.WriteUUID(Uuid);
            writer.WriteString(Username);
            writer.WriteVarInt(Properties.Length);
            foreach (var propertiesItem in Properties)
                writer.WriteType<ProfileProperty>(propertiesItem, protocolVersion);
            return;
        }

        if (protocolVersion >= 766 && protocolVersion <= 767)
        {
            writer.WriteUUID(Uuid);
            writer.WriteString(Username);
            writer.WriteVarInt(Properties.Length);
            foreach (var propertiesItem in Properties)
                writer.WriteType<ProfileProperty>(propertiesItem, protocolVersion);
            writer.WriteBoolean(StrictErrorHandling);
            return;
        }

        if (protocolVersion >= 768)
        {
            writer.WriteUUID(Uuid);
            writer.WriteString(Username);
            writer.WriteVarInt(Properties.Length);
            foreach (var propertiesItem in Properties)
                writer.WriteType<ProfileProperty>(propertiesItem, protocolVersion);
            return;
        }

        throw new System.NotSupportedException($"LoginSuccessPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public static int GetPacketId(int protocolVersion)
    {
        if (protocolVersion >= 735 && protocolVersion <= 765)
            return 0x02;
        if (protocolVersion >= 766 && protocolVersion <= 772)
            return 0x02;
        throw new System.NotSupportedException($"No packet id for protocol {protocolVersion}.");
    }
}
