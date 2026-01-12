using System;
using McProtoNet.NBT;
using McProtoNet.Protocol.Attributes;

namespace McProtoNet.Protocol;

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
