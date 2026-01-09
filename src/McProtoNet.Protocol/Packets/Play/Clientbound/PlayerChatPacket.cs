using McProtoNet.Protocol;
using McProtoNet.NBT;
using McProtoNet.Serialization;
using System;

namespace McProtoNet.Protocol.Packets.Play.Clientbound;

[PacketInfo("PlayerChat", PacketState.Play, PacketDirection.Clientbound)]
public sealed partial class PlayerChatPacket : IServerPacket
{
    public static readonly ProtocolRange[] SupportedVersionsStatic =
    {
        new(759, 759),
        new(760, 760),
        new(761, 764),
        new(765, 766),
        new(767, 769),
        new(770, MinecraftVersion.LatestProtocol),
    };

    public V759Fields? V759 { get; set; }
    public V760Fields? V760 { get; set; }
    public V761_764Fields? V761_764 { get; set; }
    public V765_766Fields? V765_766 { get; set; }
    public V767_769Fields? V767_769 { get; set; }
    public V770_LastFields? V770_Last { get; set; }

    internal void Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case 759:
            {
                var fields = V759 ?? throw new InvalidOperationException("PlayerChat V759 fields missing.");
                writer.WriteString(fields.SignedChatContent);
                if (fields.UnsignedChatContent is null)
                {
                    writer.WriteBoolean(false);
                }
                else
                {
                    writer.WriteBoolean(true);
                    writer.WriteString(fields.UnsignedChatContent);
                }
                writer.WriteVarInt(fields.Type);
                writer.WriteUUID(fields.SenderUuid);
                writer.WriteString(fields.SenderName);
                if (fields.SenderTeam is null)
                {
                    writer.WriteBoolean(false);
                }
                else
                {
                    writer.WriteBoolean(true);
                    writer.WriteString(fields.SenderTeam);
                }
                writer.WriteSignedLong(fields.Timestamp);
                writer.WriteSignedLong(fields.Salt);
                writer.WriteBuffer<VarInt>(fields.Signature);
                return;
            }
            case 760:
            {
                var fields = V760 ?? throw new InvalidOperationException("PlayerChat V760 fields missing.");
                if (fields.PreviousSignature is null)
                {
                    writer.WriteBoolean(false);
                }
                else
                {
                    writer.WriteBoolean(true);
                    writer.WriteBuffer<VarInt>(fields.PreviousSignature);
                }
                writer.WriteUUID(fields.SenderUuid);
                writer.WriteBuffer<VarInt>(fields.Signature);
                writer.WriteString(fields.PlainMessage);
                if (fields.FormattedMessage is null)
                {
                    writer.WriteBoolean(false);
                }
                else
                {
                    writer.WriteBoolean(true);
                    writer.WriteString(fields.FormattedMessage);
                }
                writer.WriteSignedLong(fields.Timestamp);
                writer.WriteSignedLong(fields.Salt);
                writer.WritePreviousMessages(fields.PreviousMessages, protocolVersion);
                if (fields.UnsignedChatContent is null)
                {
                    writer.WriteBoolean(false);
                }
                else
                {
                    writer.WriteBoolean(true);
                    writer.WriteString(fields.UnsignedChatContent);
                }
                writer.WriteVarInt(fields.FilterType);
                if (fields.FilterType == 2)
                {
                    WriteFilterMask(ref writer, fields.FilterTypeMask);
                }
                writer.WriteVarInt(fields.Type);
                writer.WriteString(fields.NetworkName);
                if (fields.NetworkTargetName is null)
                {
                    writer.WriteBoolean(false);
                }
                else
                {
                    writer.WriteBoolean(true);
                    writer.WriteString(fields.NetworkTargetName);
                }
                return;
            }
            case >= 761 and <= 764:
            {
                var fields = V761_764 ?? throw new InvalidOperationException("PlayerChat V761_764 fields missing.");
                writer.WriteUUID(fields.SenderUuid);
                writer.WriteVarInt(fields.Index);
                if (fields.Signature is null)
                {
                    writer.WriteBoolean(false);
                }
                else
                {
                    writer.WriteBoolean(true);
                    writer.WriteBuffer(fields.Signature, 256);
                }
                writer.WriteString(fields.PlainMessage);
                writer.WriteSignedLong(fields.Timestamp);
                writer.WriteSignedLong(fields.Salt);
                writer.WritePreviousMessages(fields.PreviousMessages, protocolVersion);
                if (fields.UnsignedChatContent is null)
                {
                    writer.WriteBoolean(false);
                }
                else
                {
                    writer.WriteBoolean(true);
                    writer.WriteString(fields.UnsignedChatContent);
                }
                writer.WriteVarInt(fields.FilterType);
                if (fields.FilterType == 2)
                {
                    WriteFilterMask(ref writer, fields.FilterTypeMask);
                }
                writer.WriteVarInt(fields.Type);
                writer.WriteString(fields.NetworkName);
                if (fields.NetworkTargetName is null)
                {
                    writer.WriteBoolean(false);
                }
                else
                {
                    writer.WriteBoolean(true);
                    writer.WriteString(fields.NetworkTargetName);
                }
                return;
            }
            case >= 765 and <= 766:
            {
                var fields = V765_766 ?? throw new InvalidOperationException("PlayerChat V765_766 fields missing.");
                writer.WriteUUID(fields.SenderUuid);
                writer.WriteVarInt(fields.Index);
                if (fields.Signature is null)
                {
                    writer.WriteBoolean(false);
                }
                else
                {
                    writer.WriteBoolean(true);
                    writer.WriteBuffer(fields.Signature, 256);
                }
                writer.WriteString(fields.PlainMessage);
                writer.WriteSignedLong(fields.Timestamp);
                writer.WriteSignedLong(fields.Salt);
                writer.WritePreviousMessages(fields.PreviousMessages, protocolVersion);
                if (fields.UnsignedChatContent is null)
                {
                    writer.WriteBoolean(false);
                }
                else
                {
                    writer.WriteBoolean(true);
                    writer.WriteAnonymousNbtTag(fields.UnsignedChatContent, protocolVersion);
                }
                writer.WriteVarInt(fields.FilterType);
                if (fields.FilterType == 2)
                {
                    WriteFilterMask(ref writer, fields.FilterTypeMask);
                }
                writer.WriteVarInt(fields.Type);
                writer.WriteAnonymousNbtTag(fields.NetworkName, protocolVersion);
                if (fields.NetworkTargetName is null)
                {
                    writer.WriteBoolean(false);
                }
                else
                {
                    writer.WriteBoolean(true);
                    writer.WriteAnonymousNbtTag(fields.NetworkTargetName, protocolVersion);
                }
                return;
            }
            case >= 767 and <= 769:
            {
                var fields = V767_769 ?? throw new InvalidOperationException("PlayerChat V767_769 fields missing.");
                writer.WriteUUID(fields.SenderUuid);
                writer.WriteVarInt(fields.Index);
                if (fields.Signature is null)
                {
                    writer.WriteBoolean(false);
                }
                else
                {
                    writer.WriteBoolean(true);
                    writer.WriteBuffer(fields.Signature, 256);
                }
                writer.WriteString(fields.PlainMessage);
                writer.WriteSignedLong(fields.Timestamp);
                writer.WriteSignedLong(fields.Salt);
                writer.WritePreviousMessages(fields.PreviousMessages, protocolVersion);
                if (fields.UnsignedChatContent is null)
                {
                    writer.WriteBoolean(false);
                }
                else
                {
                    writer.WriteBoolean(true);
                    writer.WriteAnonymousNbtTag(fields.UnsignedChatContent, protocolVersion);
                }
                writer.WriteVarInt(fields.FilterType);
                if (fields.FilterType == 2)
                {
                    WriteFilterMask(ref writer, fields.FilterTypeMask);
                }
                writer.WriteChatTypesHolder(fields.Type, protocolVersion);
                writer.WriteAnonymousNbtTag(fields.NetworkName, protocolVersion);
                if (fields.NetworkTargetName is null)
                {
                    writer.WriteBoolean(false);
                }
                else
                {
                    writer.WriteBoolean(true);
                    writer.WriteAnonymousNbtTag(fields.NetworkTargetName, protocolVersion);
                }
                return;
            }
            case >= 770 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = V770_Last ?? throw new InvalidOperationException("PlayerChat V770_Last fields missing.");
                writer.WriteVarInt(fields.GlobalIndex);
                writer.WriteUUID(fields.SenderUuid);
                writer.WriteVarInt(fields.Index);
                if (fields.Signature is null)
                {
                    writer.WriteBoolean(false);
                }
                else
                {
                    writer.WriteBoolean(true);
                    writer.WriteBuffer(fields.Signature, 256);
                }
                writer.WriteString(fields.PlainMessage);
                writer.WriteSignedLong(fields.Timestamp);
                writer.WriteSignedLong(fields.Salt);
                writer.WritePreviousMessages(fields.PreviousMessages, protocolVersion);
                if (fields.UnsignedChatContent is null)
                {
                    writer.WriteBoolean(false);
                }
                else
                {
                    writer.WriteBoolean(true);
                    writer.WriteAnonymousNbtTag(fields.UnsignedChatContent, protocolVersion);
                }
                writer.WriteVarInt(fields.FilterType);
                if (fields.FilterType == 2)
                {
                    WriteFilterMask(ref writer, fields.FilterTypeMask);
                }
                writer.WriteChatTypesHolder(fields.Type, protocolVersion);
                writer.WriteAnonymousNbtTag(fields.NetworkName, protocolVersion);
                if (fields.NetworkTargetName is null)
                {
                    writer.WriteBoolean(false);
                }
                else
                {
                    writer.WriteBoolean(true);
                    writer.WriteAnonymousNbtTag(fields.NetworkTargetName, protocolVersion);
                }
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.PlayerChat), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    internal void Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        switch (protocolVersion)
        {
            case 759:
                V759 = new V759Fields
                {
                    SignedChatContent = reader.ReadString(),
                    UnsignedChatContent = reader.ReadOptional(ReadDelegates.String),
                    Type = reader.ReadVarInt(),
                    SenderUuid = reader.ReadUUID(),
                    SenderName = reader.ReadString(),
                    SenderTeam = reader.ReadOptional(ReadDelegates.String),
                    Timestamp = reader.ReadSignedLong(),
                    Salt = reader.ReadSignedLong(),
                    Signature = reader.ReadBuffer(LengthFormat.VarInt)
                };
                return;
            case 760:
            {
                var fields = new V760Fields
                {
                    PreviousSignature = reader.ReadOptional((ref MinecraftPrimitiveReader r) => r.ReadBuffer(LengthFormat.VarInt)),
                    SenderUuid = reader.ReadUUID(),
                    Signature = reader.ReadBuffer(LengthFormat.VarInt),
                    PlainMessage = reader.ReadString(),
                    FormattedMessage = reader.ReadOptional(ReadDelegates.String),
                    Timestamp = reader.ReadSignedLong(),
                    Salt = reader.ReadSignedLong(),
                    PreviousMessages = reader.ReadPreviousMessages(protocolVersion),
                    UnsignedChatContent = reader.ReadOptional(ReadDelegates.String),
                    FilterType = reader.ReadVarInt()
                };
                if (fields.FilterType == 2)
                {
                    fields.FilterTypeMask = ReadFilterMask(ref reader);
                }
                fields.Type = reader.ReadVarInt();
                fields.NetworkName = reader.ReadString();
                fields.NetworkTargetName = reader.ReadOptional(ReadDelegates.String);
                V760 = fields;
                return;
            }
            case >= 761 and <= 764:
            {
                var fields = new V761_764Fields
                {
                    SenderUuid = reader.ReadUUID(),
                    Index = reader.ReadVarInt(),
                    Signature = reader.ReadOptional((ref MinecraftPrimitiveReader r) => r.ReadBuffer(256)),
                    PlainMessage = reader.ReadString(),
                    Timestamp = reader.ReadSignedLong(),
                    Salt = reader.ReadSignedLong(),
                    PreviousMessages = reader.ReadPreviousMessages(protocolVersion),
                    UnsignedChatContent = reader.ReadOptional(ReadDelegates.String),
                    FilterType = reader.ReadVarInt()
                };
                if (fields.FilterType == 2)
                {
                    fields.FilterTypeMask = ReadFilterMask(ref reader);
                }
                fields.Type = reader.ReadVarInt();
                fields.NetworkName = reader.ReadString();
                fields.NetworkTargetName = reader.ReadOptional(ReadDelegates.String);
                V761_764 = fields;
                return;
            }
            case >= 765 and <= 766:
            {
                var fields = new V765_766Fields
                {
                    SenderUuid = reader.ReadUUID(),
                    Index = reader.ReadVarInt(),
                    Signature = reader.ReadOptional((ref MinecraftPrimitiveReader r) => r.ReadBuffer(256)),
                    PlainMessage = reader.ReadString(),
                    Timestamp = reader.ReadSignedLong(),
                    Salt = reader.ReadSignedLong(),
                    PreviousMessages = reader.ReadPreviousMessages(protocolVersion),
                    UnsignedChatContent = reader.ReadOptional((ref MinecraftPrimitiveReader r) => r.ReadAnonymousNbtTag(protocolVersion)),
                    FilterType = reader.ReadVarInt()
                };
                if (fields.FilterType == 2)
                {
                    fields.FilterTypeMask = ReadFilterMask(ref reader);
                }
                fields.Type = reader.ReadVarInt();
                fields.NetworkName = reader.ReadAnonymousNbtTag(protocolVersion)
                    ?? throw new InvalidOperationException("PlayerChat networkName missing.");
                fields.NetworkTargetName = reader.ReadOptional((ref MinecraftPrimitiveReader r) => r.ReadAnonymousNbtTag(protocolVersion));
                V765_766 = fields;
                return;
            }
            case >= 767 and <= 769:
            {
                var fields = new V767_769Fields
                {
                    SenderUuid = reader.ReadUUID(),
                    Index = reader.ReadVarInt(),
                    Signature = reader.ReadOptional((ref MinecraftPrimitiveReader r) => r.ReadBuffer(256)),
                    PlainMessage = reader.ReadString(),
                    Timestamp = reader.ReadSignedLong(),
                    Salt = reader.ReadSignedLong(),
                    PreviousMessages = reader.ReadPreviousMessages(protocolVersion),
                    UnsignedChatContent = reader.ReadOptional((ref MinecraftPrimitiveReader r) => r.ReadAnonymousNbtTag(protocolVersion)),
                    FilterType = reader.ReadVarInt()
                };
                if (fields.FilterType == 2)
                {
                    fields.FilterTypeMask = ReadFilterMask(ref reader);
                }
                fields.Type = reader.ReadChatTypesHolder(protocolVersion);
                fields.NetworkName = reader.ReadAnonymousNbtTag(protocolVersion)
                    ?? throw new InvalidOperationException("PlayerChat networkName missing.");
                fields.NetworkTargetName = reader.ReadOptional((ref MinecraftPrimitiveReader r) => r.ReadAnonymousNbtTag(protocolVersion));
                V767_769 = fields;
                return;
            }
            case >= 770 and <= MinecraftVersion.LatestProtocol:
            {
                var fields = new V770_LastFields
                {
                    GlobalIndex = reader.ReadVarInt(),
                    SenderUuid = reader.ReadUUID(),
                    Index = reader.ReadVarInt(),
                    Signature = reader.ReadOptional((ref MinecraftPrimitiveReader r) => r.ReadBuffer(256)),
                    PlainMessage = reader.ReadString(),
                    Timestamp = reader.ReadSignedLong(),
                    Salt = reader.ReadSignedLong(),
                    PreviousMessages = reader.ReadPreviousMessages(protocolVersion),
                    UnsignedChatContent = reader.ReadOptional((ref MinecraftPrimitiveReader r) => r.ReadAnonymousNbtTag(protocolVersion)),
                    FilterType = reader.ReadVarInt()
                };
                if (fields.FilterType == 2)
                {
                    fields.FilterTypeMask = ReadFilterMask(ref reader);
                }
                fields.Type = reader.ReadChatTypesHolder(protocolVersion);
                fields.NetworkName = reader.ReadAnonymousNbtTag(protocolVersion)
                    ?? throw new InvalidOperationException("PlayerChat networkName missing.");
                fields.NetworkTargetName = reader.ReadOptional((ref MinecraftPrimitiveReader r) => r.ReadAnonymousNbtTag(protocolVersion));
                V770_Last = fields;
                return;
            }
            default:
                ThrowHelper.ThrowProtocolNotSupported(nameof(ServerPlayPacket.PlayerChat), protocolVersion, SupportedVersionsStatic);
                return;
        }
    }

    private static long[] ReadFilterMask(ref MinecraftPrimitiveReader reader)
    {
        int count = reader.ReadVarInt();
        if (count == 0)
        {
            return Array.Empty<long>();
        }

        var mask = new long[count];
        for (int i = 0; i < mask.Length; i++)
        {
            mask[i] = reader.ReadSignedLong();
        }
        return mask;
    }

    private static void WriteFilterMask(ref MinecraftPrimitiveWriter writer, long[]? mask)
    {
        if (mask is null)
        {
            writer.WriteVarInt(0);
            return;
        }

        writer.WriteVarInt(mask.Length);
        for (int i = 0; i < mask.Length; i++)
        {
            writer.WriteSignedLong(mask[i]);
        }
    }

    void IPacket.Serialize(ref MinecraftPrimitiveWriter writer, int protocolVersion)
        => Serialize(ref writer, protocolVersion);

    void IPacket.Deserialize(ref MinecraftPrimitiveReader reader, int protocolVersion)
        => Deserialize(ref reader, protocolVersion);

    public struct V759Fields
    {
        public string SignedChatContent { get; set; }
        public string? UnsignedChatContent { get; set; }
        public int Type { get; set; }
        public Guid SenderUuid { get; set; }
        public string SenderName { get; set; }
        public string? SenderTeam { get; set; }
        public long Timestamp { get; set; }
        public long Salt { get; set; }
        public byte[] Signature { get; set; }
    }

    public struct V760Fields
    {
        public byte[]? PreviousSignature { get; set; }
        public Guid SenderUuid { get; set; }
        public byte[] Signature { get; set; }
        public string PlainMessage { get; set; }
        public string? FormattedMessage { get; set; }
        public long Timestamp { get; set; }
        public long Salt { get; set; }
        public PreviousMessages PreviousMessages { get; set; }
        public string? UnsignedChatContent { get; set; }
        public int FilterType { get; set; }
        public long[]? FilterTypeMask { get; set; }
        public int Type { get; set; }
        public string NetworkName { get; set; }
        public string? NetworkTargetName { get; set; }
    }

    public struct V761_764Fields
    {
        public Guid SenderUuid { get; set; }
        public int Index { get; set; }
        public byte[]? Signature { get; set; }
        public string PlainMessage { get; set; }
        public long Timestamp { get; set; }
        public long Salt { get; set; }
        public PreviousMessages PreviousMessages { get; set; }
        public string? UnsignedChatContent { get; set; }
        public int FilterType { get; set; }
        public long[]? FilterTypeMask { get; set; }
        public int Type { get; set; }
        public string NetworkName { get; set; }
        public string? NetworkTargetName { get; set; }
    }

    public struct V765_766Fields
    {
        public Guid SenderUuid { get; set; }
        public int Index { get; set; }
        public byte[]? Signature { get; set; }
        public string PlainMessage { get; set; }
        public long Timestamp { get; set; }
        public long Salt { get; set; }
        public PreviousMessages PreviousMessages { get; set; }
        public NbtTag? UnsignedChatContent { get; set; }
        public int FilterType { get; set; }
        public long[]? FilterTypeMask { get; set; }
        public int Type { get; set; }
        public NbtTag NetworkName { get; set; }
        public NbtTag? NetworkTargetName { get; set; }
    }

    public struct V767_769Fields
    {
        public Guid SenderUuid { get; set; }
        public int Index { get; set; }
        public byte[]? Signature { get; set; }
        public string PlainMessage { get; set; }
        public long Timestamp { get; set; }
        public long Salt { get; set; }
        public PreviousMessages PreviousMessages { get; set; }
        public NbtTag? UnsignedChatContent { get; set; }
        public int FilterType { get; set; }
        public long[]? FilterTypeMask { get; set; }
        public ChatTypesHolder Type { get; set; }
        public NbtTag NetworkName { get; set; }
        public NbtTag? NetworkTargetName { get; set; }
    }

    public struct V770_LastFields
    {
        public int GlobalIndex { get; set; }
        public Guid SenderUuid { get; set; }
        public int Index { get; set; }
        public byte[]? Signature { get; set; }
        public string PlainMessage { get; set; }
        public long Timestamp { get; set; }
        public long Salt { get; set; }
        public PreviousMessages PreviousMessages { get; set; }
        public NbtTag? UnsignedChatContent { get; set; }
        public int FilterType { get; set; }
        public long[]? FilterTypeMask { get; set; }
        public ChatTypesHolder Type { get; set; }
        public NbtTag NetworkName { get; set; }
        public NbtTag? NetworkTargetName { get; set; }
    }
}
