using System;
using McProtoNet.NBT;
using McProtoNet.Protocol;
using McProtoNet.Serialization;

namespace McProtoNet.Protocol.Extensions;

public static partial class ProtocolSerializationExtensions
{
    public static PacketCommonAddResourcePack ReadPacketCommonAddResourcePack(this ref MinecraftPrimitiveReader reader,
        int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<PacketCommonAddResourcePack>(protocolVersion);
        Guid uuid = reader.ReadUUID();
        string url = reader.ReadString();
        string hash = reader.ReadString();
        bool forced = reader.ReadBoolean();
        NbtTag? promptMessage = reader.ReadBoolean() ? reader.ReadAnonymousNbtTag(protocolVersion) : null;
        return new PacketCommonAddResourcePack(uuid, url, hash, forced, promptMessage);
    }

    public static void WritePacketCommonAddResourcePack(this ref MinecraftPrimitiveWriter writer,
        PacketCommonAddResourcePack value, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<PacketCommonAddResourcePack>(protocolVersion);
        writer.WriteUUID(value.Uuid);
        writer.WriteString(value.Url);
        writer.WriteString(value.Hash);
        writer.WriteBoolean(value.Forced);
        if (value.PromptMessage is null)
        {
            writer.WriteBoolean(false);
        }
        else
        {
            writer.WriteBoolean(true);
            writer.WriteAnonymousNbtTag(value.PromptMessage, protocolVersion);
        }
    }

    public static PacketCommonClearDialog ReadPacketCommonClearDialog(this ref MinecraftPrimitiveReader reader,
        int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<PacketCommonClearDialog>(protocolVersion);
        return new PacketCommonClearDialog();
    }

    public static void WritePacketCommonClearDialog(this ref MinecraftPrimitiveWriter writer,
        PacketCommonClearDialog value, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<PacketCommonClearDialog>(protocolVersion);
    }

    public static PacketCommonCookieRequest ReadPacketCommonCookieRequest(this ref MinecraftPrimitiveReader reader,
        int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<PacketCommonCookieRequest>(protocolVersion);
        string cookie = reader.ReadString();
        return new PacketCommonCookieRequest(cookie);
    }

    public static void WritePacketCommonCookieRequest(this ref MinecraftPrimitiveWriter writer,
        PacketCommonCookieRequest value, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<PacketCommonCookieRequest>(protocolVersion);
        writer.WriteString(value.Cookie);
    }

    public static PacketCommonCookieResponse ReadPacketCommonCookieResponse(this ref MinecraftPrimitiveReader reader,
        int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<PacketCommonCookieResponse>(protocolVersion);
        string key = reader.ReadString();
        byte[]? value = reader.ReadBoolean() ? ReadByteArray(ref reader) : null;
        return new PacketCommonCookieResponse(key, value);
    }

    public static void WritePacketCommonCookieResponse(this ref MinecraftPrimitiveWriter writer,
        PacketCommonCookieResponse value, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<PacketCommonCookieResponse>(protocolVersion);
        writer.WriteString(value.Key);
        if (value.Value is null)
        {
            writer.WriteBoolean(false);
        }
        else
        {
            writer.WriteBoolean(true);
            WriteByteArray(ref writer, value.Value);
        }
    }

    public static PacketCommonCustomClickAction ReadPacketCommonCustomClickAction(this ref MinecraftPrimitiveReader reader,
        int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<PacketCommonCustomClickAction>(protocolVersion);
        string id = reader.ReadString();
        NbtTag? nbt = reader.ReadBoolean() ? reader.ReadAnonymousNbtTag(protocolVersion) : null;
        return new PacketCommonCustomClickAction(id, nbt);
    }

    public static void WritePacketCommonCustomClickAction(this ref MinecraftPrimitiveWriter writer,
        PacketCommonCustomClickAction value, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<PacketCommonCustomClickAction>(protocolVersion);
        writer.WriteString(value.Id);
        if (value.Nbt is null)
        {
            writer.WriteBoolean(false);
        }
        else
        {
            writer.WriteBoolean(true);
            writer.WriteAnonymousNbtTag(value.Nbt, protocolVersion);
        }
    }

    public static PacketCommonCustomReportDetails ReadPacketCommonCustomReportDetails(this ref MinecraftPrimitiveReader reader,
        int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<PacketCommonCustomReportDetails>(protocolVersion);
        PacketCommonCustomReportDetails.DetailEntry[] details = ReadArray(ref reader, (ref MinecraftPrimitiveReader r) =>
        {
            string key = r.ReadString();
            string value = r.ReadString();
            return new PacketCommonCustomReportDetails.DetailEntry(key, value);
        });
        return new PacketCommonCustomReportDetails(details);
    }

    public static void WritePacketCommonCustomReportDetails(this ref MinecraftPrimitiveWriter writer,
        PacketCommonCustomReportDetails value, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<PacketCommonCustomReportDetails>(protocolVersion);
        WriteArray(ref writer, value.Details, (ref MinecraftPrimitiveWriter w, PacketCommonCustomReportDetails.DetailEntry entry) =>
        {
            w.WriteString(entry.Key);
            w.WriteString(entry.Value);
        });
    }

    public static PacketCommonRemoveResourcePack ReadPacketCommonRemoveResourcePack(this ref MinecraftPrimitiveReader reader,
        int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<PacketCommonRemoveResourcePack>(protocolVersion);
        Guid? uuid = reader.ReadBoolean() ? reader.ReadUUID() : null;
        return new PacketCommonRemoveResourcePack(uuid);
    }

    public static void WritePacketCommonRemoveResourcePack(this ref MinecraftPrimitiveWriter writer,
        PacketCommonRemoveResourcePack value, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<PacketCommonRemoveResourcePack>(protocolVersion);
        if (value.Uuid is null)
        {
            writer.WriteBoolean(false);
        }
        else
        {
            writer.WriteBoolean(true);
            writer.WriteUUID(value.Uuid.Value);
        }
    }

    public static PacketCommonSelectKnownPacks ReadPacketCommonSelectKnownPacks(this ref MinecraftPrimitiveReader reader,
        int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<PacketCommonSelectKnownPacks>(protocolVersion);
        PacketCommonSelectKnownPacks.PackEntry[] packs = ReadArray(ref reader, (ref MinecraftPrimitiveReader r) =>
        {
            string ns = r.ReadString();
            string id = r.ReadString();
            string version = r.ReadString();
            return new PacketCommonSelectKnownPacks.PackEntry(ns, id, version);
        });
        return new PacketCommonSelectKnownPacks(packs);
    }

    public static void WritePacketCommonSelectKnownPacks(this ref MinecraftPrimitiveWriter writer,
        PacketCommonSelectKnownPacks value, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<PacketCommonSelectKnownPacks>(protocolVersion);
        WriteArray(ref writer, value.Packs, (ref MinecraftPrimitiveWriter w, PacketCommonSelectKnownPacks.PackEntry entry) =>
        {
            w.WriteString(entry.Namespace);
            w.WriteString(entry.Id);
            w.WriteString(entry.Version);
        });
    }

    public static PacketCommonServerLinks ReadPacketCommonServerLinks(this ref MinecraftPrimitiveReader reader,
        int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<PacketCommonServerLinks>(protocolVersion);
        PacketCommonServerLinks.ServerLinkEntry[] links = ReadArray(ref reader, (ref MinecraftPrimitiveReader r) =>
        {
            bool hasKnownType = r.ReadBoolean();
            ServerLinkType? knownType = hasKnownType ? r.ReadServerLinkType(protocolVersion) : null;
            NbtTag? unknownType = hasKnownType ? null : r.ReadAnonymousNbtTag(protocolVersion);
            string link = r.ReadString();
            return new PacketCommonServerLinks.ServerLinkEntry(hasKnownType, knownType, unknownType, link);
        });
        return new PacketCommonServerLinks(links);
    }

    public static void WritePacketCommonServerLinks(this ref MinecraftPrimitiveWriter writer, PacketCommonServerLinks value,
        int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<PacketCommonServerLinks>(protocolVersion);
        WriteArray(ref writer, value.Links, (ref MinecraftPrimitiveWriter w, PacketCommonServerLinks.ServerLinkEntry entry) =>
        {
            w.WriteBoolean(entry.HasKnownType);
            if (entry.HasKnownType)
            {
                w.WriteServerLinkType(entry.KnownType ?? throw new InvalidOperationException("knownType missing"), protocolVersion);
            }
            else
            {
                w.WriteAnonymousNbtTag(entry.UnknownType ?? throw new InvalidOperationException("unknownType missing"), protocolVersion);
            }
            w.WriteString(entry.Link);
        });
    }

    public static PacketCommonSettings ReadPacketCommonSettings(this ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<PacketCommonSettings>(protocolVersion);
        string locale = reader.ReadString();
        sbyte viewDistance = reader.ReadSignedByte();
        int chatFlags = reader.ReadVarInt();
        bool chatColors = reader.ReadBoolean();
        byte skinParts = reader.ReadUnsignedByte();
        int mainHand = reader.ReadVarInt();
        bool enableTextFiltering = reader.ReadBoolean();
        bool enableServerListing = reader.ReadBoolean();
        string? particleStatus = null;
        if (protocolVersion >= 768)
        {
            particleStatus = ReadParticleStatus(reader.ReadVarInt());
        }
        return new PacketCommonSettings(locale, viewDistance, chatFlags, chatColors, skinParts, mainHand,
            enableTextFiltering, enableServerListing, particleStatus);
    }

    public static void WritePacketCommonSettings(this ref MinecraftPrimitiveWriter writer, PacketCommonSettings value,
        int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<PacketCommonSettings>(protocolVersion);
        writer.WriteString(value.Locale);
        writer.WriteSignedByte(value.ViewDistance);
        writer.WriteVarInt(value.ChatFlags);
        writer.WriteBoolean(value.ChatColors);
        writer.WriteUnsignedByte(value.SkinParts);
        writer.WriteVarInt(value.MainHand);
        writer.WriteBoolean(value.EnableTextFiltering);
        writer.WriteBoolean(value.EnableServerListing);
        if (protocolVersion >= 768)
        {
            writer.WriteVarInt(WriteParticleStatus(value.ParticleStatus ?? throw new InvalidOperationException("particleStatus missing")));
        }
    }

    public static PacketCommonStoreCookie ReadPacketCommonStoreCookie(this ref MinecraftPrimitiveReader reader,
        int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<PacketCommonStoreCookie>(protocolVersion);
        string key = reader.ReadString();
        byte[] value = ReadByteArray(ref reader);
        return new PacketCommonStoreCookie(key, value);
    }

    public static void WritePacketCommonStoreCookie(this ref MinecraftPrimitiveWriter writer, PacketCommonStoreCookie value,
        int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<PacketCommonStoreCookie>(protocolVersion);
        writer.WriteString(value.Key);
        WriteByteArray(ref writer, value.Value);
    }

    public static PacketCommonTransfer ReadPacketCommonTransfer(this ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<PacketCommonTransfer>(protocolVersion);
        string host = reader.ReadString();
        int port = reader.ReadVarInt();
        return new PacketCommonTransfer(host, port);
    }

    public static void WritePacketCommonTransfer(this ref MinecraftPrimitiveWriter writer, PacketCommonTransfer value,
        int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<PacketCommonTransfer>(protocolVersion);
        writer.WriteString(value.Host);
        writer.WriteVarInt(value.Port);
    }

    private static byte[] ReadByteArray(ref MinecraftPrimitiveReader reader)
    {
        int length = reader.ReadVarInt();
        return reader.ReadBuffer(length);
    }

    private static void WriteByteArray(ref MinecraftPrimitiveWriter writer, byte[] value)
    {
        writer.WriteVarInt(value.Length);
        writer.WriteBuffer(value);
    }

    private static string ReadParticleStatus(int id)
    {
        return id switch
        {
            0 => "all",
            1 => "decreased",
            2 => "minimal",
            _ => throw new InvalidOperationException($"Unknown particle status id {id}")
        };
    }

    private static int WriteParticleStatus(string value)
    {
        return value switch
        {
            "all" => 0,
            "decreased" => 1,
            "minimal" => 2,
            _ => throw new InvalidOperationException($"Unknown particle status value {value}")
        };
    }
}
