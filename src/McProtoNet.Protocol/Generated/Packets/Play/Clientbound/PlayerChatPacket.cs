using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;
using System.Text.Json;
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
                var filterTypeMaskValue = FilterTypeMask ?? throw new System.InvalidOperationException("FilterTypeMask is required at this protocol version.");
                writer.WriteVarInt(filterTypeMaskValue.Length);
                foreach (var filterTypeMaskItem in filterTypeMaskValue)
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
                var filterTypeMaskValue = FilterTypeMask ?? throw new System.InvalidOperationException("FilterTypeMask is required at this protocol version.");
                writer.WriteVarInt(filterTypeMaskValue.Length);
                foreach (var filterTypeMaskItem in filterTypeMaskValue)
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
                var filterTypeMaskValue = FilterTypeMask ?? throw new System.InvalidOperationException("FilterTypeMask is required at this protocol version.");
                writer.WriteVarInt(filterTypeMaskValue.Length);
                foreach (var filterTypeMaskItem in filterTypeMaskValue)
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
                var filterTypeMaskValue = FilterTypeMask ?? throw new System.InvalidOperationException("FilterTypeMask is required at this protocol version.");
                writer.WriteVarInt(filterTypeMaskValue.Length);
                foreach (var filterTypeMaskItem in filterTypeMaskValue)
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
                var filterTypeMaskValue = FilterTypeMask ?? throw new System.InvalidOperationException("FilterTypeMask is required at this protocol version.");
                writer.WriteVarInt(filterTypeMaskValue.Length);
                foreach (var filterTypeMaskItem in filterTypeMaskValue)
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

    public void WriteJson(Utf8JsonWriter writer)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("SenderUuid");
        writer.WriteStringValue(SenderUuid);
        if (Signature is { } signatureValue)
        {
            writer.WritePropertyName("Signature");
            writer.WriteBase64StringValue(signatureValue);
        }

        writer.WritePropertyName("Timestamp");
        writer.WriteNumberValue(Timestamp);
        writer.WritePropertyName("Salt");
        writer.WriteNumberValue(Salt);
        if (V759 is { } v759)
        {
            writer.WritePropertyName("SignedChatContent");
            writer.WriteStringValue(v759.SignedChatContent);
            writer.WritePropertyName("SenderName");
            writer.WriteStringValue(v759.SenderName);
            if (v759.SenderTeam is { } senderTeamValue)
            {
                writer.WritePropertyName("SenderTeam");
                writer.WriteStringValue(senderTeamValue);
            }

            if (v759.UnsignedChatContentJson is { } unsignedChatContentJsonValue)
            {
                writer.WritePropertyName("UnsignedChatContentJson");
                writer.WriteStringValue(unsignedChatContentJsonValue);
            }

            writer.WritePropertyName("Type");
            writer.WriteNumberValue(v759.Type);
        }
        else if (V760 is { } v760)
        {
            if (v760.PreviousSignature is { } previousSignatureValue)
            {
                writer.WritePropertyName("PreviousSignature");
                writer.WriteBase64StringValue(previousSignatureValue);
            }

            if (v760.FormattedMessage is { } formattedMessageValue)
            {
                writer.WritePropertyName("FormattedMessage");
                writer.WriteStringValue(formattedMessageValue);
            }

            writer.WritePropertyName("PlainMessage");
            writer.WriteStringValue(v760.PlainMessage);
            writer.WritePropertyName("PreviousMessages");
            writer.WriteStartArray();
            foreach (var item0 in v760.PreviousMessages)
            {
                item0.WriteJson(writer);
            }

            writer.WriteEndArray();
            if (v760.UnsignedChatContentJson is { } unsignedChatContentJsonValue)
            {
                writer.WritePropertyName("UnsignedChatContentJson");
                writer.WriteStringValue(unsignedChatContentJsonValue);
            }

            writer.WritePropertyName("FilterType");
            writer.WriteNumberValue(v760.FilterType);
            if (v760.FilterTypeMask is { } filterTypeMaskValue)
            {
                writer.WritePropertyName("FilterTypeMask");
                writer.WriteStartArray();
                foreach (var item0 in filterTypeMaskValue)
                {
                    writer.WriteNumberValue(item0);
                }

                writer.WriteEndArray();
            }

            writer.WritePropertyName("Type");
            writer.WriteNumberValue(v760.Type);
            writer.WritePropertyName("NetworkNameJson");
            writer.WriteStringValue(v760.NetworkNameJson);
            if (v760.NetworkTargetNameJson is { } networkTargetNameJsonValue)
            {
                writer.WritePropertyName("NetworkTargetNameJson");
                writer.WriteStringValue(networkTargetNameJsonValue);
            }
        }
        else if (V761_764 is { } v761_764)
        {
            writer.WritePropertyName("Index");
            writer.WriteNumberValue(v761_764.Index);
            writer.WritePropertyName("PlainMessage");
            writer.WriteStringValue(v761_764.PlainMessage);
            writer.WritePropertyName("PreviousMessages");
            writer.WriteStartArray();
            foreach (var item0 in v761_764.PreviousMessages)
            {
                item0.WriteJson(writer);
            }

            writer.WriteEndArray();
            if (v761_764.UnsignedChatContentJson is { } unsignedChatContentJsonValue)
            {
                writer.WritePropertyName("UnsignedChatContentJson");
                writer.WriteStringValue(unsignedChatContentJsonValue);
            }

            writer.WritePropertyName("FilterType");
            writer.WriteNumberValue(v761_764.FilterType);
            if (v761_764.FilterTypeMask is { } filterTypeMaskValue)
            {
                writer.WritePropertyName("FilterTypeMask");
                writer.WriteStartArray();
                foreach (var item0 in filterTypeMaskValue)
                {
                    writer.WriteNumberValue(item0);
                }

                writer.WriteEndArray();
            }

            writer.WritePropertyName("Type");
            writer.WriteNumberValue(v761_764.Type);
            writer.WritePropertyName("NetworkNameJson");
            writer.WriteStringValue(v761_764.NetworkNameJson);
            if (v761_764.NetworkTargetNameJson is { } networkTargetNameJsonValue)
            {
                writer.WritePropertyName("NetworkTargetNameJson");
                writer.WriteStringValue(networkTargetNameJsonValue);
            }
        }
        else if (V765_766 is { } v765_766)
        {
            writer.WritePropertyName("Index");
            writer.WriteNumberValue(v765_766.Index);
            writer.WritePropertyName("PlainMessage");
            writer.WriteStringValue(v765_766.PlainMessage);
            writer.WritePropertyName("PreviousMessages");
            writer.WriteStartArray();
            foreach (var item0 in v765_766.PreviousMessages)
            {
                item0.WriteJson(writer);
            }

            writer.WriteEndArray();
            if (v765_766.UnsignedChatContent is { } unsignedChatContentValue)
            {
                writer.WritePropertyName("UnsignedChatContent");
                unsignedChatContentValue.WriteJson(writer);
            }

            writer.WritePropertyName("FilterType");
            writer.WriteNumberValue(v765_766.FilterType);
            if (v765_766.FilterTypeMask is { } filterTypeMaskValue)
            {
                writer.WritePropertyName("FilterTypeMask");
                writer.WriteStartArray();
                foreach (var item0 in filterTypeMaskValue)
                {
                    writer.WriteNumberValue(item0);
                }

                writer.WriteEndArray();
            }

            writer.WritePropertyName("Type");
            writer.WriteNumberValue(v765_766.Type);
            writer.WritePropertyName("NetworkName");
            v765_766.NetworkName.WriteJson(writer);
            if (v765_766.NetworkTargetName is { } networkTargetNameValue)
            {
                writer.WritePropertyName("NetworkTargetName");
                networkTargetNameValue.WriteJson(writer);
            }
        }
        else if (V767_769 is { } v767_769)
        {
            writer.WritePropertyName("Index");
            writer.WriteNumberValue(v767_769.Index);
            writer.WritePropertyName("PlainMessage");
            writer.WriteStringValue(v767_769.PlainMessage);
            writer.WritePropertyName("PreviousMessages");
            writer.WriteStartArray();
            foreach (var item0 in v767_769.PreviousMessages)
            {
                item0.WriteJson(writer);
            }

            writer.WriteEndArray();
            if (v767_769.UnsignedChatContent is { } unsignedChatContentValue)
            {
                writer.WritePropertyName("UnsignedChatContent");
                unsignedChatContentValue.WriteJson(writer);
            }

            writer.WritePropertyName("FilterType");
            writer.WriteNumberValue(v767_769.FilterType);
            if (v767_769.FilterTypeMask is { } filterTypeMaskValue)
            {
                writer.WritePropertyName("FilterTypeMask");
                writer.WriteStartArray();
                foreach (var item0 in filterTypeMaskValue)
                {
                    writer.WriteNumberValue(item0);
                }

                writer.WriteEndArray();
            }

            writer.WritePropertyName("ChatType");
            v767_769.ChatType.WriteJson(writer);
            writer.WritePropertyName("NetworkName");
            v767_769.NetworkName.WriteJson(writer);
            if (v767_769.NetworkTargetName is { } networkTargetNameValue)
            {
                writer.WritePropertyName("NetworkTargetName");
                networkTargetNameValue.WriteJson(writer);
            }
        }
        else if (V770_Last is { } v770_Last)
        {
            writer.WritePropertyName("GlobalIndex");
            writer.WriteNumberValue(v770_Last.GlobalIndex);
            writer.WritePropertyName("Index");
            writer.WriteNumberValue(v770_Last.Index);
            writer.WritePropertyName("PlainMessage");
            writer.WriteStringValue(v770_Last.PlainMessage);
            writer.WritePropertyName("PreviousMessages");
            writer.WriteStartArray();
            foreach (var item0 in v770_Last.PreviousMessages)
            {
                item0.WriteJson(writer);
            }

            writer.WriteEndArray();
            if (v770_Last.UnsignedChatContent is { } unsignedChatContentValue)
            {
                writer.WritePropertyName("UnsignedChatContent");
                unsignedChatContentValue.WriteJson(writer);
            }

            writer.WritePropertyName("FilterType");
            writer.WriteNumberValue(v770_Last.FilterType);
            if (v770_Last.FilterTypeMask is { } filterTypeMaskValue)
            {
                writer.WritePropertyName("FilterTypeMask");
                writer.WriteStartArray();
                foreach (var item0 in filterTypeMaskValue)
                {
                    writer.WriteNumberValue(item0);
                }

                writer.WriteEndArray();
            }

            writer.WritePropertyName("ChatType");
            v770_Last.ChatType.WriteJson(writer);
            writer.WritePropertyName("NetworkName");
            v770_Last.NetworkName.WriteJson(writer);
            if (v770_Last.NetworkTargetName is { } networkTargetNameValue)
            {
                writer.WritePropertyName("NetworkTargetName");
                networkTargetNameValue.WriteJson(writer);
            }
        }

        writer.WriteEndObject();
    }

    public static PacketIdentity Identity => new("play.toClient.player_chat", "PlayerChat", PacketPhase.Play, PacketDirection.Clientbound, 71);

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
