using System;
using McProtoNet.NBT;
using McProtoNet.Protocol.Attributes;

namespace McProtoNet.Protocol;

[ProtocolSupport(766, MinecraftVersion.LatestProtocol)]
public sealed partial class PacketCommonAddResourcePack
{
    public Guid Uuid { get; }
    public string Url { get; }
    public string Hash { get; }
    public bool Forced { get; }
    public NbtTag? PromptMessage { get; }

    public PacketCommonAddResourcePack(Guid uuid, string url, string hash, bool forced, NbtTag? promptMessage)
    {
        Uuid = uuid;
        Url = url;
        Hash = hash;
        Forced = forced;
        PromptMessage = promptMessage;
    }
}

[ProtocolSupport(771, MinecraftVersion.LatestProtocol)]
public sealed partial class PacketCommonClearDialog
{
}

[ProtocolSupport(766, MinecraftVersion.LatestProtocol)]
public sealed partial class PacketCommonCookieRequest
{
    public string Cookie { get; }

    public PacketCommonCookieRequest(string cookie)
    {
        Cookie = cookie;
    }
}

[ProtocolSupport(766, MinecraftVersion.LatestProtocol)]
public sealed partial class PacketCommonCookieResponse
{
    public string Key { get; }
    public byte[]? Value { get; }

    public PacketCommonCookieResponse(string key, byte[]? value)
    {
        Key = key;
        Value = value;
    }
}

[ProtocolSupport(771, MinecraftVersion.LatestProtocol)]
public sealed partial class PacketCommonCustomClickAction
{
    public string Id { get; }
    public NbtTag? Nbt { get; }

    public PacketCommonCustomClickAction(string id, NbtTag? nbt)
    {
        Id = id;
        Nbt = nbt;
    }
}

[ProtocolSupport(767, MinecraftVersion.LatestProtocol)]
public sealed partial class PacketCommonCustomReportDetails
{
    public DetailEntry[] Details { get; }

    public PacketCommonCustomReportDetails(DetailEntry[] details)
    {
        Details = details;
    }

    public sealed record DetailEntry(string Key, string Value);
}

[ProtocolSupport(766, MinecraftVersion.LatestProtocol)]
public sealed partial class PacketCommonRemoveResourcePack
{
    public Guid? Uuid { get; }

    public PacketCommonRemoveResourcePack(Guid? uuid)
    {
        Uuid = uuid;
    }
}

[ProtocolSupport(766, MinecraftVersion.LatestProtocol)]
public sealed partial class PacketCommonSelectKnownPacks
{
    public PackEntry[] Packs { get; }

    public PacketCommonSelectKnownPacks(PackEntry[] packs)
    {
        Packs = packs;
    }

    public sealed record PackEntry(string Namespace, string Id, string Version);
}

[ProtocolSupport(767, MinecraftVersion.LatestProtocol)]
public sealed partial class PacketCommonServerLinks
{
    public ServerLinkEntry[] Links { get; }

    public PacketCommonServerLinks(ServerLinkEntry[] links)
    {
        Links = links;
    }

    public sealed record ServerLinkEntry(bool HasKnownType, ServerLinkType? KnownType, NbtTag? UnknownType, string Link);
}

[ProtocolSupport(766, MinecraftVersion.LatestProtocol)]
public sealed partial class PacketCommonSettings
{
    public string Locale { get; }
    public sbyte ViewDistance { get; }
    public int ChatFlags { get; }
    public bool ChatColors { get; }
    public byte SkinParts { get; }
    public int MainHand { get; }
    public bool EnableTextFiltering { get; }
    public bool EnableServerListing { get; }
    public string? ParticleStatus { get; }

    public PacketCommonSettings(string locale, sbyte viewDistance, int chatFlags, bool chatColors, byte skinParts,
        int mainHand, bool enableTextFiltering, bool enableServerListing, string? particleStatus)
    {
        Locale = locale;
        ViewDistance = viewDistance;
        ChatFlags = chatFlags;
        ChatColors = chatColors;
        SkinParts = skinParts;
        MainHand = mainHand;
        EnableTextFiltering = enableTextFiltering;
        EnableServerListing = enableServerListing;
        ParticleStatus = particleStatus;
    }
}

[ProtocolSupport(766, MinecraftVersion.LatestProtocol)]
public sealed partial class PacketCommonStoreCookie
{
    public string Key { get; }
    public byte[] Value { get; }

    public PacketCommonStoreCookie(string key, byte[] value)
    {
        Key = key;
        Value = value;
    }
}

[ProtocolSupport(766, MinecraftVersion.LatestProtocol)]
public sealed partial class PacketCommonTransfer
{
    public string Host { get; }
    public int Port { get; }

    public PacketCommonTransfer(string host, int port)
    {
        Host = host;
        Port = port;
    }
}
