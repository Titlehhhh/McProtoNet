using McProtoNet.Protocol.Attributes;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Login.Clientbound;
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("login.toClient.success", PacketPhase.Login, PacketDirection.Clientbound)]
[PacketField("Uuid", "Guid")]
[PacketField("Username", "string")]
[PacketField("Properties", "ProfileProperty[]", Group = "V759_765", From = 759, To = 765)]
[PacketField("Properties", "ProfileProperty[]", Group = "V766_767", From = 766, To = 767)]
[PacketField("StrictErrorHandling", "bool", Group = "V766_767", From = 766, To = 767)]
[PacketField("Properties", "ProfileProperty[]", Group = "V768_Last", From = 768)]
public sealed partial record LoginSuccessPacket(Guid Uuid, string Username, LoginSuccessPacket.V759_765Layer? V759_765 = null, LoginSuccessPacket.V766_767Layer? V766_767 = null, LoginSuccessPacket.V768_LastLayer? V768_Last = null) : IPacket<LoginSuccessPacket>
{
    public readonly record struct V759_765Layer(ProfileProperty[] Properties);
    public readonly record struct V766_767Layer(ProfileProperty[] Properties, bool StrictErrorHandling);
    public readonly record struct V768_LastLayer(ProfileProperty[] Properties);
    public static LoginSuccessPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<LoginSuccessPacket>(protocolVersion);
        if (protocolVersion <= 758)
        {
            var uuid = reader.ReadUUID();
            var username = reader.ReadString();
            return new LoginSuccessPacket(uuid, username);
        }

        if (protocolVersion >= 759 && protocolVersion <= 765)
        {
            var uuid = reader.ReadUUID();
            var username = reader.ReadString();
            int propertiesCount = reader.ReadVarInt();
            var properties = new ProfileProperty[propertiesCount];
            for (int i = 0; i < properties.Length; i++)
                properties[i] = reader.ReadType<ProfileProperty>(protocolVersion);
            return new LoginSuccessPacket(uuid, username, V759_765: new V759_765Layer(properties));
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
            return new LoginSuccessPacket(uuid, username, V766_767: new V766_767Layer(properties, strictErrorHandling));
        }

        if (protocolVersion >= 768)
        {
            var uuid = reader.ReadUUID();
            var username = reader.ReadString();
            int propertiesCount = reader.ReadVarInt();
            var properties = new ProfileProperty[propertiesCount];
            for (int i = 0; i < properties.Length; i++)
                properties[i] = reader.ReadType<ProfileProperty>(protocolVersion);
            return new LoginSuccessPacket(uuid, username, V768_Last: new V768_LastLayer(properties));
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
            var layer = V759_765 ?? throw new WrongLayerException("LoginSuccessPacket", protocolVersion, "V759_765");
            ProfileProperty[] Properties = layer.Properties;
            writer.WriteUUID(Uuid);
            writer.WriteString(Username);
            writer.WriteVarInt(Properties.Length);
            foreach (var propertiesItem in Properties)
                writer.WriteType<ProfileProperty>(propertiesItem, protocolVersion);
            return;
        }

        if (protocolVersion >= 766 && protocolVersion <= 767)
        {
            var layer = V766_767 ?? throw new WrongLayerException("LoginSuccessPacket", protocolVersion, "V766_767");
            ProfileProperty[] Properties = layer.Properties;
            bool StrictErrorHandling = layer.StrictErrorHandling;
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
            var layer = V768_Last ?? throw new WrongLayerException("LoginSuccessPacket", protocolVersion, "V768_Last");
            ProfileProperty[] Properties = layer.Properties;
            writer.WriteUUID(Uuid);
            writer.WriteString(Username);
            writer.WriteVarInt(Properties.Length);
            foreach (var propertiesItem in Properties)
                writer.WriteType<ProfileProperty>(propertiesItem, protocolVersion);
            return;
        }

        throw new System.NotSupportedException($"LoginSuccessPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public static PacketIdentity Identity => new("login.toClient.success", "LoginSuccess", PacketPhase.Login, PacketDirection.Clientbound, 5);

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 735 && protocolVersion <= 772)
        {
            id = 0x02;
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
