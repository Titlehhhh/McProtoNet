using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
using McProtoNet.NBT;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;
[ProtocolSupport(759, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.player_chat", PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("SenderUuid", "Guid")]
[PacketField("Signature", "byte[]?")]
[PacketField("Timestamp", "long")]
[PacketField("Salt", "long")]
[PacketField("SignedChatContent", "string", Group = "V759", From = 759, To = 759)]
[PacketField("SenderName", "string", Group = "V759", From = 759, To = 759)]
[PacketField("SenderTeam", "string?", Group = "V759", From = 759, To = 759)]
[PacketField("UnsignedChatContentJson", "string?", Group = "V759", From = 759, To = 759)]
[PacketField("Type", "int", Group = "V759", From = 759, To = 759)]
[PacketField("PreviousSignature", "byte[]?", Group = "V760", From = 760, To = 760)]
[PacketField("FormattedMessage", "string?", Group = "V760", From = 760, To = 760)]
[PacketField("PlainMessage", "string", Group = "V760", From = 760, To = 760)]
[PacketField("PreviousMessages", "PreviousMessage[]", Group = "V760", From = 760, To = 760)]
[PacketField("UnsignedChatContentJson", "string?", Group = "V760", From = 760, To = 760)]
[PacketField("FilterType", "int", Group = "V760", From = 760, To = 760)]
[PacketField("FilterTypeMask", "long[]?", Group = "V760", From = 760, To = 760)]
[PacketField("Type", "int", Group = "V760", From = 760, To = 760)]
[PacketField("NetworkNameJson", "string", Group = "V760", From = 760, To = 760)]
[PacketField("NetworkTargetNameJson", "string?", Group = "V760", From = 760, To = 760)]
[PacketField("Index", "int", Group = "V761_764", From = 761, To = 764)]
[PacketField("PlainMessage", "string", Group = "V761_764", From = 761, To = 764)]
[PacketField("PreviousMessages", "PreviousMessage[]", Group = "V761_764", From = 761, To = 764)]
[PacketField("UnsignedChatContentJson", "string?", Group = "V761_764", From = 761, To = 764)]
[PacketField("FilterType", "int", Group = "V761_764", From = 761, To = 764)]
[PacketField("FilterTypeMask", "long[]?", Group = "V761_764", From = 761, To = 764)]
[PacketField("Type", "int", Group = "V761_764", From = 761, To = 764)]
[PacketField("NetworkNameJson", "string", Group = "V761_764", From = 761, To = 764)]
[PacketField("NetworkTargetNameJson", "string?", Group = "V761_764", From = 761, To = 764)]
[PacketField("Index", "int", Group = "V765_766", From = 765, To = 766)]
[PacketField("PlainMessage", "string", Group = "V765_766", From = 765, To = 766)]
[PacketField("PreviousMessages", "PreviousMessage[]", Group = "V765_766", From = 765, To = 766)]
[PacketField("UnsignedChatContent", "NbtTag?", Group = "V765_766", From = 765, To = 766)]
[PacketField("FilterType", "int", Group = "V765_766", From = 765, To = 766)]
[PacketField("FilterTypeMask", "long[]?", Group = "V765_766", From = 765, To = 766)]
[PacketField("Type", "int", Group = "V765_766", From = 765, To = 766)]
[PacketField("NetworkName", "NbtTag", Group = "V765_766", From = 765, To = 766)]
[PacketField("NetworkTargetName", "NbtTag?", Group = "V765_766", From = 765, To = 766)]
[PacketField("Index", "int", Group = "V767_769", From = 767, To = 769)]
[PacketField("PlainMessage", "string", Group = "V767_769", From = 767, To = 769)]
[PacketField("PreviousMessages", "PreviousMessage[]", Group = "V767_769", From = 767, To = 769)]
[PacketField("UnsignedChatContent", "NbtTag?", Group = "V767_769", From = 767, To = 769)]
[PacketField("FilterType", "int", Group = "V767_769", From = 767, To = 769)]
[PacketField("FilterTypeMask", "long[]?", Group = "V767_769", From = 767, To = 769)]
[PacketField("ChatType", "RegistryOrInline<ChatTypes>", Group = "V767_769", From = 767, To = 769)]
[PacketField("NetworkName", "NbtTag", Group = "V767_769", From = 767, To = 769)]
[PacketField("NetworkTargetName", "NbtTag?", Group = "V767_769", From = 767, To = 769)]
[PacketField("GlobalIndex", "int", Group = "V770_Last", From = 770)]
[PacketField("Index", "int", Group = "V770_Last", From = 770)]
[PacketField("PlainMessage", "string", Group = "V770_Last", From = 770)]
[PacketField("PreviousMessages", "PreviousMessage[]", Group = "V770_Last", From = 770)]
[PacketField("UnsignedChatContent", "NbtTag?", Group = "V770_Last", From = 770)]
[PacketField("FilterType", "int", Group = "V770_Last", From = 770)]
[PacketField("FilterTypeMask", "long[]?", Group = "V770_Last", From = 770)]
[PacketField("ChatType", "RegistryOrInline<ChatTypes>", Group = "V770_Last", From = 770)]
[PacketField("NetworkName", "NbtTag", Group = "V770_Last", From = 770)]
[PacketField("NetworkTargetName", "NbtTag?", Group = "V770_Last", From = 770)]
public sealed partial record PlayerChatPacket(Guid SenderUuid, byte[]? Signature, long Timestamp, long Salt, PlayerChatPacket.V759Layer? V759 = null, PlayerChatPacket.V760Layer? V760 = null, PlayerChatPacket.V761_764Layer? V761_764 = null, PlayerChatPacket.V765_766Layer? V765_766 = null, PlayerChatPacket.V767_769Layer? V767_769 = null, PlayerChatPacket.V770_LastLayer? V770_Last = null) : IPacket<PlayerChatPacket>, IPacket
{
    public readonly record struct V759Layer(string SignedChatContent, string SenderName, string? SenderTeam, string? UnsignedChatContentJson, int Type);
    public readonly record struct V760Layer(byte[]? PreviousSignature, string? FormattedMessage, string PlainMessage, PreviousMessage[] PreviousMessages, string? UnsignedChatContentJson, int FilterType, long[]? FilterTypeMask, int Type, string NetworkNameJson, string? NetworkTargetNameJson);
    public readonly record struct V761_764Layer(int Index, string PlainMessage, PreviousMessage[] PreviousMessages, string? UnsignedChatContentJson, int FilterType, long[]? FilterTypeMask, int Type, string NetworkNameJson, string? NetworkTargetNameJson);
    public readonly record struct V765_766Layer(int Index, string PlainMessage, PreviousMessage[] PreviousMessages, NbtTag? UnsignedChatContent, int FilterType, long[]? FilterTypeMask, int Type, NbtTag NetworkName, NbtTag? NetworkTargetName);
    public readonly record struct V767_769Layer(int Index, string PlainMessage, PreviousMessage[] PreviousMessages, NbtTag? UnsignedChatContent, int FilterType, long[]? FilterTypeMask, RegistryOrInline<ChatTypes> ChatType, NbtTag NetworkName, NbtTag? NetworkTargetName);
    public readonly record struct V770_LastLayer(int GlobalIndex, int Index, string PlainMessage, PreviousMessage[] PreviousMessages, NbtTag? UnsignedChatContent, int FilterType, long[]? FilterTypeMask, RegistryOrInline<ChatTypes> ChatType, NbtTag NetworkName, NbtTag? NetworkTargetName);
    public static PlayerChatPacket Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<PlayerChatPacket>(protocolVersion);
        if (protocolVersion >= 759 && protocolVersion <= 759)
        {
            var signedChatContent = reader.ReadString();
            string? unsignedChatContentJson = null;
            if (reader.ReadBoolean())
                unsignedChatContentJson = reader.ReadString();
            var type = reader.ReadVarInt();
            var senderUuid = reader.ReadUUID();
            var senderName = reader.ReadString();
            string? senderTeam = null;
            if (reader.ReadBoolean())
                senderTeam = reader.ReadString();
            var timestamp = reader.ReadSignedLong();
            var salt = reader.ReadSignedLong();
            var signature = reader.ReadByteArray();
            return new PlayerChatPacket(senderUuid, signature, timestamp, salt, V759: new V759Layer(signedChatContent, senderName, senderTeam, unsignedChatContentJson, type));
        }

        if (protocolVersion >= 760 && protocolVersion <= 760)
        {
            byte[]? previousSignature = null;
            if (reader.ReadBoolean())
                previousSignature = reader.ReadByteArray();
            var senderUuid = reader.ReadUUID();
            var signature = reader.ReadByteArray();
            var plainMessage = reader.ReadString();
            string? formattedMessage = null;
            if (reader.ReadBoolean())
                formattedMessage = reader.ReadString();
            var timestamp = reader.ReadSignedLong();
            var salt = reader.ReadSignedLong();
            int previousMessagesCount = reader.ReadVarInt();
            var previousMessages = new PreviousMessage[previousMessagesCount];
            for (int i = 0; i < previousMessages.Length; i++)
                previousMessages[i] = reader.ReadType<PreviousMessage>(protocolVersion);
            string? unsignedChatContentJson = null;
            if (reader.ReadBoolean())
                unsignedChatContentJson = reader.ReadString();
            var filterType = reader.ReadVarInt();
            long[]? filterTypeMask = default;
            if (filterType == 2)
            {
                int filterTypeMaskValueCount = reader.ReadVarInt();
                var filterTypeMaskValue = new long[filterTypeMaskValueCount];
                for (int i = 0; i < filterTypeMaskValue.Length; i++)
                    filterTypeMaskValue[i] = reader.ReadSignedLong();
                filterTypeMask = filterTypeMaskValue;
            }

            var type = reader.ReadVarInt();
            var networkNameJson = reader.ReadString();
            string? networkTargetNameJson = null;
            if (reader.ReadBoolean())
                networkTargetNameJson = reader.ReadString();
            return new PlayerChatPacket(senderUuid, signature, timestamp, salt, V760: new V760Layer(previousSignature, formattedMessage, plainMessage, previousMessages, unsignedChatContentJson, filterType, filterTypeMask, type, networkNameJson, networkTargetNameJson));
        }

        if (protocolVersion >= 761 && protocolVersion <= 764)
        {
            var senderUuid = reader.ReadUUID();
            var index = reader.ReadVarInt();
            byte[]? signature = null;
            if (reader.ReadBoolean())
                signature = reader.ReadFixedBytes(256);
            var plainMessage = reader.ReadString();
            var timestamp = reader.ReadSignedLong();
            var salt = reader.ReadSignedLong();
            int previousMessagesCount = reader.ReadVarInt();
            var previousMessages = new PreviousMessage[previousMessagesCount];
            for (int i = 0; i < previousMessages.Length; i++)
                previousMessages[i] = reader.ReadType<PreviousMessage>(protocolVersion);
            string? unsignedChatContentJson = null;
            if (reader.ReadBoolean())
                unsignedChatContentJson = reader.ReadString();
            var filterType = reader.ReadVarInt();
            long[]? filterTypeMask = default;
            if (filterType == 2)
            {
                int filterTypeMaskValueCount = reader.ReadVarInt();
                var filterTypeMaskValue = new long[filterTypeMaskValueCount];
                for (int i = 0; i < filterTypeMaskValue.Length; i++)
                    filterTypeMaskValue[i] = reader.ReadSignedLong();
                filterTypeMask = filterTypeMaskValue;
            }

            var type = reader.ReadVarInt();
            var networkNameJson = reader.ReadString();
            string? networkTargetNameJson = null;
            if (reader.ReadBoolean())
                networkTargetNameJson = reader.ReadString();
            return new PlayerChatPacket(senderUuid, signature, timestamp, salt, V761_764: new V761_764Layer(index, plainMessage, previousMessages, unsignedChatContentJson, filterType, filterTypeMask, type, networkNameJson, networkTargetNameJson));
        }

        if (protocolVersion >= 765 && protocolVersion <= 766)
        {
            var senderUuid = reader.ReadUUID();
            var index = reader.ReadVarInt();
            byte[]? signature = null;
            if (reader.ReadBoolean())
                signature = reader.ReadFixedBytes(256);
            var plainMessage = reader.ReadString();
            var timestamp = reader.ReadSignedLong();
            var salt = reader.ReadSignedLong();
            int previousMessagesCount = reader.ReadVarInt();
            var previousMessages = new PreviousMessage[previousMessagesCount];
            for (int i = 0; i < previousMessages.Length; i++)
                previousMessages[i] = reader.ReadType<PreviousMessage>(protocolVersion);
            NbtTag? unsignedChatContent = null;
            if (reader.ReadBoolean())
                unsignedChatContent = reader.ReadNbtTag(false)!;
            var filterType = reader.ReadVarInt();
            long[]? filterTypeMask = default;
            if (filterType == 2)
            {
                int filterTypeMaskValueCount = reader.ReadVarInt();
                var filterTypeMaskValue = new long[filterTypeMaskValueCount];
                for (int i = 0; i < filterTypeMaskValue.Length; i++)
                    filterTypeMaskValue[i] = reader.ReadSignedLong();
                filterTypeMask = filterTypeMaskValue;
            }

            var type = reader.ReadVarInt();
            var networkName = reader.ReadNbtTag(false)!;
            NbtTag? networkTargetName = null;
            if (reader.ReadBoolean())
                networkTargetName = reader.ReadNbtTag(false)!;
            return new PlayerChatPacket(senderUuid, signature, timestamp, salt, V765_766: new V765_766Layer(index, plainMessage, previousMessages, unsignedChatContent, filterType, filterTypeMask, type, networkName, networkTargetName));
        }

        if (protocolVersion >= 767 && protocolVersion <= 769)
        {
            var senderUuid = reader.ReadUUID();
            var index = reader.ReadVarInt();
            byte[]? signature = null;
            if (reader.ReadBoolean())
                signature = reader.ReadFixedBytes(256);
            var plainMessage = reader.ReadString();
            var timestamp = reader.ReadSignedLong();
            var salt = reader.ReadSignedLong();
            int previousMessagesCount = reader.ReadVarInt();
            var previousMessages = new PreviousMessage[previousMessagesCount];
            for (int i = 0; i < previousMessages.Length; i++)
                previousMessages[i] = reader.ReadType<PreviousMessage>(protocolVersion);
            NbtTag? unsignedChatContent = null;
            if (reader.ReadBoolean())
                unsignedChatContent = reader.ReadNbtTag(false)!;
            var filterType = reader.ReadVarInt();
            long[]? filterTypeMask = default;
            if (filterType == 2)
            {
                int filterTypeMaskValueCount = reader.ReadVarInt();
                var filterTypeMaskValue = new long[filterTypeMaskValueCount];
                for (int i = 0; i < filterTypeMaskValue.Length; i++)
                    filterTypeMaskValue[i] = reader.ReadSignedLong();
                filterTypeMask = filterTypeMaskValue;
            }

            var chatType = reader.ReadType<RegistryOrInline<ChatTypes>>(protocolVersion);
            var networkName = reader.ReadNbtTag(false)!;
            NbtTag? networkTargetName = null;
            if (reader.ReadBoolean())
                networkTargetName = reader.ReadNbtTag(false)!;
            return new PlayerChatPacket(senderUuid, signature, timestamp, salt, V767_769: new V767_769Layer(index, plainMessage, previousMessages, unsignedChatContent, filterType, filterTypeMask, chatType, networkName, networkTargetName));
        }

        if (protocolVersion >= 770)
        {
            var globalIndex = reader.ReadVarInt();
            var senderUuid = reader.ReadUUID();
            var index = reader.ReadVarInt();
            byte[]? signature = null;
            if (reader.ReadBoolean())
                signature = reader.ReadFixedBytes(256);
            var plainMessage = reader.ReadString();
            var timestamp = reader.ReadSignedLong();
            var salt = reader.ReadSignedLong();
            int previousMessagesCount = reader.ReadVarInt();
            var previousMessages = new PreviousMessage[previousMessagesCount];
            for (int i = 0; i < previousMessages.Length; i++)
                previousMessages[i] = reader.ReadType<PreviousMessage>(protocolVersion);
            NbtTag? unsignedChatContent = null;
            if (reader.ReadBoolean())
                unsignedChatContent = reader.ReadNbtTag(false)!;
            var filterType = reader.ReadVarInt();
            long[]? filterTypeMask = default;
            if (filterType == 2)
            {
                int filterTypeMaskValueCount = reader.ReadVarInt();
                var filterTypeMaskValue = new long[filterTypeMaskValueCount];
                for (int i = 0; i < filterTypeMaskValue.Length; i++)
                    filterTypeMaskValue[i] = reader.ReadSignedLong();
                filterTypeMask = filterTypeMaskValue;
            }

            var chatType = reader.ReadType<RegistryOrInline<ChatTypes>>(protocolVersion);
            var networkName = reader.ReadNbtTag(false)!;
            NbtTag? networkTargetName = null;
            if (reader.ReadBoolean())
                networkTargetName = reader.ReadNbtTag(false)!;
            return new PlayerChatPacket(senderUuid, signature, timestamp, salt, V770_Last: new V770_LastLayer(globalIndex, index, plainMessage, previousMessages, unsignedChatContent, filterType, filterTypeMask, chatType, networkName, networkTargetName));
        }

        throw new System.NotSupportedException($"PlayerChatPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<PlayerChatPacket>(protocolVersion);
        if (protocolVersion >= 759 && protocolVersion <= 759)
        {
            var layer = V759 ?? throw new WrongLayerException("PlayerChatPacket", protocolVersion, "V759");
            string SignedChatContent = layer.SignedChatContent;
            string SenderName = layer.SenderName;
            string? SenderTeam = layer.SenderTeam;
            string? UnsignedChatContentJson = layer.UnsignedChatContentJson;
            int Type = layer.Type;
            writer.WriteString(SignedChatContent);
            writer.WriteBoolean(UnsignedChatContentJson is not null);
            if (UnsignedChatContentJson is { } unsignedChatContentJsonValue)
                writer.WriteString(unsignedChatContentJsonValue);
            writer.WriteVarInt(Type);
            writer.WriteUUID(SenderUuid);
            writer.WriteString(SenderName);
            writer.WriteBoolean(SenderTeam is not null);
            if (SenderTeam is { } senderTeamValue)
                writer.WriteString(senderTeamValue);
            writer.WriteSignedLong(Timestamp);
            writer.WriteSignedLong(Salt);
            writer.WriteByteArray((Signature ?? throw new System.InvalidOperationException("Signature is required at this protocol version.")));
            return;
        }

        if (protocolVersion >= 760 && protocolVersion <= 760)
        {
            var layer = V760 ?? throw new WrongLayerException("PlayerChatPacket", protocolVersion, "V760");
            byte[]? PreviousSignature = layer.PreviousSignature;
            string? FormattedMessage = layer.FormattedMessage;
            string PlainMessage = layer.PlainMessage;
            PreviousMessage[] PreviousMessages = layer.PreviousMessages;
            string? UnsignedChatContentJson = layer.UnsignedChatContentJson;
            int FilterType = layer.FilterType;
            long[]? FilterTypeMask = layer.FilterTypeMask;
            int Type = layer.Type;
            string NetworkNameJson = layer.NetworkNameJson;
            string? NetworkTargetNameJson = layer.NetworkTargetNameJson;
            writer.WriteBoolean(PreviousSignature is not null);
            if (PreviousSignature is { } previousSignatureValue)
                writer.WriteByteArray(previousSignatureValue);
            writer.WriteUUID(SenderUuid);
            writer.WriteByteArray((Signature ?? throw new System.InvalidOperationException("Signature is required at this protocol version.")));
            writer.WriteString(PlainMessage);
            writer.WriteBoolean(FormattedMessage is not null);
            if (FormattedMessage is { } formattedMessageValue)
                writer.WriteString(formattedMessageValue);
            writer.WriteSignedLong(Timestamp);
            writer.WriteSignedLong(Salt);
            writer.WriteVarInt(PreviousMessages.Length);
            foreach (var previousMessagesItem in PreviousMessages)
                writer.WriteType<PreviousMessage>(previousMessagesItem, protocolVersion);
            writer.WriteBoolean(UnsignedChatContentJson is not null);
            if (UnsignedChatContentJson is { } unsignedChatContentJsonValue)
                writer.WriteString(unsignedChatContentJsonValue);
            writer.WriteVarInt(FilterType);
            if (FilterType == 2)
            {
                writer.WriteVarInt((FilterTypeMask ?? throw new System.InvalidOperationException("FilterTypeMask is required at this protocol version.")).Length);
                foreach (var filterTypeMaskItem in (FilterTypeMask ?? throw new System.InvalidOperationException("FilterTypeMask is required at this protocol version.")))
                    writer.WriteSignedLong(filterTypeMaskItem);
            }
            else if (FilterTypeMask is not null)
            {
                throw new System.InvalidOperationException("FilterTypeMask is set, but 'filterType' does not select it at this protocol version.");
            }

            writer.WriteVarInt(Type);
            writer.WriteString(NetworkNameJson);
            writer.WriteBoolean(NetworkTargetNameJson is not null);
            if (NetworkTargetNameJson is { } networkTargetNameJsonValue)
                writer.WriteString(networkTargetNameJsonValue);
            return;
        }

        if (protocolVersion >= 761 && protocolVersion <= 764)
        {
            var layer = V761_764 ?? throw new WrongLayerException("PlayerChatPacket", protocolVersion, "V761_764");
            int Index = layer.Index;
            string PlainMessage = layer.PlainMessage;
            PreviousMessage[] PreviousMessages = layer.PreviousMessages;
            string? UnsignedChatContentJson = layer.UnsignedChatContentJson;
            int FilterType = layer.FilterType;
            long[]? FilterTypeMask = layer.FilterTypeMask;
            int Type = layer.Type;
            string NetworkNameJson = layer.NetworkNameJson;
            string? NetworkTargetNameJson = layer.NetworkTargetNameJson;
            writer.WriteUUID(SenderUuid);
            writer.WriteVarInt(Index);
            writer.WriteBoolean(Signature is not null);
            if (Signature is { } signatureValue)
                writer.WriteFixedBytes(signatureValue, 256);
            writer.WriteString(PlainMessage);
            writer.WriteSignedLong(Timestamp);
            writer.WriteSignedLong(Salt);
            writer.WriteVarInt(PreviousMessages.Length);
            foreach (var previousMessagesItem in PreviousMessages)
                writer.WriteType<PreviousMessage>(previousMessagesItem, protocolVersion);
            writer.WriteBoolean(UnsignedChatContentJson is not null);
            if (UnsignedChatContentJson is { } unsignedChatContentJsonValue)
                writer.WriteString(unsignedChatContentJsonValue);
            writer.WriteVarInt(FilterType);
            if (FilterType == 2)
            {
                writer.WriteVarInt((FilterTypeMask ?? throw new System.InvalidOperationException("FilterTypeMask is required at this protocol version.")).Length);
                foreach (var filterTypeMaskItem in (FilterTypeMask ?? throw new System.InvalidOperationException("FilterTypeMask is required at this protocol version.")))
                    writer.WriteSignedLong(filterTypeMaskItem);
            }
            else if (FilterTypeMask is not null)
            {
                throw new System.InvalidOperationException("FilterTypeMask is set, but 'filterType' does not select it at this protocol version.");
            }

            writer.WriteVarInt(Type);
            writer.WriteString(NetworkNameJson);
            writer.WriteBoolean(NetworkTargetNameJson is not null);
            if (NetworkTargetNameJson is { } networkTargetNameJsonValue)
                writer.WriteString(networkTargetNameJsonValue);
            return;
        }

        if (protocolVersion >= 765 && protocolVersion <= 766)
        {
            var layer = V765_766 ?? throw new WrongLayerException("PlayerChatPacket", protocolVersion, "V765_766");
            int Index = layer.Index;
            string PlainMessage = layer.PlainMessage;
            PreviousMessage[] PreviousMessages = layer.PreviousMessages;
            NbtTag? UnsignedChatContent = layer.UnsignedChatContent;
            int FilterType = layer.FilterType;
            long[]? FilterTypeMask = layer.FilterTypeMask;
            int Type = layer.Type;
            NbtTag NetworkName = layer.NetworkName;
            NbtTag? NetworkTargetName = layer.NetworkTargetName;
            writer.WriteUUID(SenderUuid);
            writer.WriteVarInt(Index);
            writer.WriteBoolean(Signature is not null);
            if (Signature is { } signatureValue)
                writer.WriteFixedBytes(signatureValue, 256);
            writer.WriteString(PlainMessage);
            writer.WriteSignedLong(Timestamp);
            writer.WriteSignedLong(Salt);
            writer.WriteVarInt(PreviousMessages.Length);
            foreach (var previousMessagesItem in PreviousMessages)
                writer.WriteType<PreviousMessage>(previousMessagesItem, protocolVersion);
            writer.WriteBoolean(UnsignedChatContent is not null);
            if (UnsignedChatContent is { } unsignedChatContentValue)
                writer.WriteNbt(unsignedChatContentValue);
            writer.WriteVarInt(FilterType);
            if (FilterType == 2)
            {
                writer.WriteVarInt((FilterTypeMask ?? throw new System.InvalidOperationException("FilterTypeMask is required at this protocol version.")).Length);
                foreach (var filterTypeMaskItem in (FilterTypeMask ?? throw new System.InvalidOperationException("FilterTypeMask is required at this protocol version.")))
                    writer.WriteSignedLong(filterTypeMaskItem);
            }
            else if (FilterTypeMask is not null)
            {
                throw new System.InvalidOperationException("FilterTypeMask is set, but 'filterType' does not select it at this protocol version.");
            }

            writer.WriteVarInt(Type);
            writer.WriteNbt(NetworkName);
            writer.WriteBoolean(NetworkTargetName is not null);
            if (NetworkTargetName is { } networkTargetNameValue)
                writer.WriteNbt(networkTargetNameValue);
            return;
        }

        if (protocolVersion >= 767 && protocolVersion <= 769)
        {
            var layer = V767_769 ?? throw new WrongLayerException("PlayerChatPacket", protocolVersion, "V767_769");
            int Index = layer.Index;
            string PlainMessage = layer.PlainMessage;
            PreviousMessage[] PreviousMessages = layer.PreviousMessages;
            NbtTag? UnsignedChatContent = layer.UnsignedChatContent;
            int FilterType = layer.FilterType;
            long[]? FilterTypeMask = layer.FilterTypeMask;
            RegistryOrInline<ChatTypes> ChatType = layer.ChatType;
            NbtTag NetworkName = layer.NetworkName;
            NbtTag? NetworkTargetName = layer.NetworkTargetName;
            writer.WriteUUID(SenderUuid);
            writer.WriteVarInt(Index);
            writer.WriteBoolean(Signature is not null);
            if (Signature is { } signatureValue)
                writer.WriteFixedBytes(signatureValue, 256);
            writer.WriteString(PlainMessage);
            writer.WriteSignedLong(Timestamp);
            writer.WriteSignedLong(Salt);
            writer.WriteVarInt(PreviousMessages.Length);
            foreach (var previousMessagesItem in PreviousMessages)
                writer.WriteType<PreviousMessage>(previousMessagesItem, protocolVersion);
            writer.WriteBoolean(UnsignedChatContent is not null);
            if (UnsignedChatContent is { } unsignedChatContentValue)
                writer.WriteNbt(unsignedChatContentValue);
            writer.WriteVarInt(FilterType);
            if (FilterType == 2)
            {
                writer.WriteVarInt((FilterTypeMask ?? throw new System.InvalidOperationException("FilterTypeMask is required at this protocol version.")).Length);
                foreach (var filterTypeMaskItem in (FilterTypeMask ?? throw new System.InvalidOperationException("FilterTypeMask is required at this protocol version.")))
                    writer.WriteSignedLong(filterTypeMaskItem);
            }
            else if (FilterTypeMask is not null)
            {
                throw new System.InvalidOperationException("FilterTypeMask is set, but 'filterType' does not select it at this protocol version.");
            }

            writer.WriteType<RegistryOrInline<ChatTypes>>(ChatType, protocolVersion);
            writer.WriteNbt(NetworkName);
            writer.WriteBoolean(NetworkTargetName is not null);
            if (NetworkTargetName is { } networkTargetNameValue)
                writer.WriteNbt(networkTargetNameValue);
            return;
        }

        if (protocolVersion >= 770)
        {
            var layer = V770_Last ?? throw new WrongLayerException("PlayerChatPacket", protocolVersion, "V770_Last");
            int GlobalIndex = layer.GlobalIndex;
            int Index = layer.Index;
            string PlainMessage = layer.PlainMessage;
            PreviousMessage[] PreviousMessages = layer.PreviousMessages;
            NbtTag? UnsignedChatContent = layer.UnsignedChatContent;
            int FilterType = layer.FilterType;
            long[]? FilterTypeMask = layer.FilterTypeMask;
            RegistryOrInline<ChatTypes> ChatType = layer.ChatType;
            NbtTag NetworkName = layer.NetworkName;
            NbtTag? NetworkTargetName = layer.NetworkTargetName;
            writer.WriteVarInt(GlobalIndex);
            writer.WriteUUID(SenderUuid);
            writer.WriteVarInt(Index);
            writer.WriteBoolean(Signature is not null);
            if (Signature is { } signatureValue)
                writer.WriteFixedBytes(signatureValue, 256);
            writer.WriteString(PlainMessage);
            writer.WriteSignedLong(Timestamp);
            writer.WriteSignedLong(Salt);
            writer.WriteVarInt(PreviousMessages.Length);
            foreach (var previousMessagesItem in PreviousMessages)
                writer.WriteType<PreviousMessage>(previousMessagesItem, protocolVersion);
            writer.WriteBoolean(UnsignedChatContent is not null);
            if (UnsignedChatContent is { } unsignedChatContentValue)
                writer.WriteNbt(unsignedChatContentValue);
            writer.WriteVarInt(FilterType);
            if (FilterType == 2)
            {
                writer.WriteVarInt((FilterTypeMask ?? throw new System.InvalidOperationException("FilterTypeMask is required at this protocol version.")).Length);
                foreach (var filterTypeMaskItem in (FilterTypeMask ?? throw new System.InvalidOperationException("FilterTypeMask is required at this protocol version.")))
                    writer.WriteSignedLong(filterTypeMaskItem);
            }
            else if (FilterTypeMask is not null)
            {
                throw new System.InvalidOperationException("FilterTypeMask is set, but 'filterType' does not select it at this protocol version.");
            }

            writer.WriteType<RegistryOrInline<ChatTypes>>(ChatType, protocolVersion);
            writer.WriteNbt(NetworkName);
            writer.WriteBoolean(NetworkTargetName is not null);
            if (NetworkTargetName is { } networkTargetNameValue)
                writer.WriteNbt(networkTargetNameValue);
            return;
        }

        throw new System.NotSupportedException($"PlayerChatPacket has no wire layout for protocol version {protocolVersion}.");
    }

    public static PacketIdentity Identity => new("play.toClient.player_chat", "PlayerChat", PacketPhase.Play, PacketDirection.Clientbound, 66);

    PacketIdentity IPacket.Identity => Identity;

    public static bool TryGetPacketId(int protocolVersion, out int id)
    {
        if (protocolVersion >= 759 && protocolVersion <= 759)
        {
            id = 0x30;
            return true;
        }

        if (protocolVersion >= 760 && protocolVersion <= 760)
        {
            id = 0x33;
            return true;
        }

        if (protocolVersion >= 761 && protocolVersion <= 761)
        {
            id = 0x31;
            return true;
        }

        if (protocolVersion >= 762 && protocolVersion <= 763)
        {
            id = 0x35;
            return true;
        }

        if (protocolVersion >= 764 && protocolVersion <= 765)
        {
            id = 0x37;
            return true;
        }

        if (protocolVersion >= 766 && protocolVersion <= 767)
        {
            id = 0x39;
            return true;
        }

        if (protocolVersion >= 768 && protocolVersion <= 769)
        {
            id = 0x3B;
            return true;
        }

        if (protocolVersion >= 770 && protocolVersion <= 772)
        {
            id = 0x3A;
            return true;
        }

        if (protocolVersion >= 773 && protocolVersion <= 774)
        {
            id = 0x3F;
            return true;
        }

        if (protocolVersion >= 775 && protocolVersion <= 776)
        {
            id = 0x41;
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
