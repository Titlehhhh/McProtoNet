using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
using McProtoNet.NBT;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;
[ProtocolSupport(761, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.profileless_chat", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("MessageJson", "string", Group = "V761_764", From = 761, To = 764)]
[PacketField("Type", "int", Group = "V761_764", From = 761, To = 764)]
[PacketField("NameJson", "string", Group = "V761_764", From = 761, To = 764)]
[PacketField("TargetJson", "string?", Group = "V761_764", From = 761, To = 764)]
[PacketField("Message", "NbtTag", Group = "V765_766", From = 765, To = 766)]
[PacketField("Type", "int", Group = "V765_766", From = 765, To = 766)]
[PacketField("Name", "NbtTag", Group = "V765_766", From = 765, To = 766)]
[PacketField("Target", "NbtTag?", Group = "V765_766", From = 765, To = 766)]
[PacketField("Message", "NbtTag", Group = "V767_Last", From = 767)]
[PacketField("ChatType", "RegistryOrInline<ChatTypes>", Group = "V767_Last", From = 767)]
[PacketField("Name", "NbtTag", Group = "V767_Last", From = 767)]
[PacketField("Target", "NbtTag?", Group = "V767_Last", From = 767)]
public sealed partial record ProfilelessChatPacket(ProfilelessChatPacket.V761_764Layer? V761_764 = null, ProfilelessChatPacket.V765_766Layer? V765_766 = null, ProfilelessChatPacket.V767_LastLayer? V767_Last = null) : IPacket<ProfilelessChatPacket>, IPacket
{
    public readonly record struct V761_764Layer(string MessageJson, int Type, string NameJson, string? TargetJson);
    public readonly record struct V765_766Layer(NbtTag Message, int Type, NbtTag Name, NbtTag? Target);
    public readonly record struct V767_LastLayer(NbtTag Message, RegistryOrInline<ChatTypes> ChatType, NbtTag Name, NbtTag? Target);
    public static ProfilelessChatPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ProfilelessChatPacket>(protocolVersion);
        if (protocolVersion >= 761 && protocolVersion <= 764)
        {
            var messageJson = reader.ReadString();
            var type = reader.ReadVarInt();
            var nameJson = reader.ReadString();
            string? targetJson = null;
            if (reader.ReadBoolean())
                targetJson = reader.ReadString();
            return new ProfilelessChatPacket(V761_764: new V761_764Layer(messageJson, type, nameJson, targetJson));
        }

        if (protocolVersion >= 765 && protocolVersion <= 766)
        {
            var message = reader.ReadNbtTag(false)!;
            var type = reader.ReadVarInt();
            var name = reader.ReadNbtTag(false)!;
            NbtTag? target = null;
            if (reader.ReadBoolean())
                target = reader.ReadNbtTag(false)!;
            return new ProfilelessChatPacket(V765_766: new V765_766Layer(message, type, name, target));
        }

        if (protocolVersion >= 767)
        {
            var message = reader.ReadNbtTag(false)!;
            var chatType = reader.ReadType<RegistryOrInline<ChatTypes>>(protocolVersion);
            var name = reader.ReadNbtTag(false)!;
            NbtTag? target = null;
            if (reader.ReadBoolean())
                target = reader.ReadNbtTag(false)!;
            return new ProfilelessChatPacket(V767_Last: new V767_LastLayer(message, chatType, name, target));
        }

        throw new System.NotSupportedException($"ProfilelessChatPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ProfilelessChatPacket>(protocolVersion);
        if (protocolVersion >= 761 && protocolVersion <= 764)
        {
            var layer = V761_764 ?? throw new WrongLayerException("ProfilelessChatPacket", protocolVersion, "V761_764");
            string MessageJson = layer.MessageJson;
            int Type = layer.Type;
            string NameJson = layer.NameJson;
            string? TargetJson = layer.TargetJson;
            writer.WriteString(MessageJson);
            writer.WriteVarInt(Type);
            writer.WriteString(NameJson);
            writer.WriteBoolean(TargetJson is not null);
            if (TargetJson is { } targetJsonValue)
                writer.WriteString(targetJsonValue);
            return;
        }

        if (protocolVersion >= 765 && protocolVersion <= 766)
        {
            var layer = V765_766 ?? throw new WrongLayerException("ProfilelessChatPacket", protocolVersion, "V765_766");
            NbtTag Message = layer.Message;
            int Type = layer.Type;
            NbtTag Name = layer.Name;
            NbtTag? Target = layer.Target;
            writer.WriteNbt(Message);
            writer.WriteVarInt(Type);
            writer.WriteNbt(Name);
            writer.WriteBoolean(Target is not null);
            if (Target is { } targetValue)
                writer.WriteNbt(targetValue);
            return;
        }

        if (protocolVersion >= 767)
        {
            var layer = V767_Last ?? throw new WrongLayerException("ProfilelessChatPacket", protocolVersion, "V767_Last");
            NbtTag Message = layer.Message;
            RegistryOrInline<ChatTypes> ChatType = layer.ChatType;
            NbtTag Name = layer.Name;
            NbtTag? Target = layer.Target;
            writer.WriteNbt(Message);
            writer.WriteType<RegistryOrInline<ChatTypes>>(ChatType, protocolVersion);
            writer.WriteNbt(Name);
            writer.WriteBoolean(Target is not null);
            if (Target is { } targetValue)
                writer.WriteNbt(targetValue);
            return;
        }

        throw new System.NotSupportedException($"ProfilelessChatPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public static PacketIdentity Identity => new("play.toClient.profileless_chat", "ProfilelessChat", PacketPhase.Play, PacketDirection.Clientbound, 71);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 761 && protocolVersion <= 761)
        {
            id = 0x18;
            return true;
        }

        if (protocolVersion >= 762 && protocolVersion <= 763)
        {
            id = 0x1B;
            return true;
        }

        if (protocolVersion >= 764 && protocolVersion <= 765)
        {
            id = 0x1C;
            return true;
        }

        if (protocolVersion >= 766 && protocolVersion <= 769)
        {
            id = 0x1E;
            return true;
        }

        if (protocolVersion >= 770 && protocolVersion <= 772)
        {
            id = 0x1D;
            return true;
        }

        if (protocolVersion >= 773 && protocolVersion <= 776)
        {
            id = 0x21;
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
