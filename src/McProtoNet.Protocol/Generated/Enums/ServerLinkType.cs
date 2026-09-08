#pragma warning disable CA2225

using McProtoNet.Protocol.Attributes;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol;
[ProtocolSupport(767, MinecraftVersion.LatestProtocol)]
public readonly partial record struct ServerLinkType(int Value) : IProtocolType<ServerLinkType>
{
    public static readonly ServerLinkType BugReport = new(0);
    public static readonly ServerLinkType CommunityGuidelines = new(1);
    public static readonly ServerLinkType Support = new(2);
    public static readonly ServerLinkType Status = new(3);
    public static readonly ServerLinkType Feedback = new(4);
    public static readonly ServerLinkType Community = new(5);
    public static readonly ServerLinkType Website = new(6);
    public static readonly ServerLinkType Forums = new(7);
    public static readonly ServerLinkType News = new(8);
    public static readonly ServerLinkType Announcements = new(9);
    public static explicit operator int (ServerLinkType value) => value.Value;
    public static explicit operator ServerLinkType(int value) => new(value);
    public static ServerLinkType Read(ref MinecraftPrimitiveReader reader, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ServerLinkType>(protocolVersion);
        return new ServerLinkType((int)reader.ReadVarInt());
    }

    public readonly void Write(MinecraftPrimitiveWriter writer, int protocolVersion)
    {
        ThrowHelper.ThrowIfProtocolNotSupported<ServerLinkType>(protocolVersion);
        writer.WriteVarInt((int)Value);
    }

    public override string ToString() => Value switch
    {
        0 => "bug_report",
        1 => "community_guidelines",
        2 => "support",
        3 => "status",
        4 => "feedback",
        5 => "community",
        6 => "website",
        7 => "forums",
        8 => "news",
        9 => "announcements",
        _ => $"unknown({Value})"};
    public string ToString(int protocolVersion)
    {
        if (protocolVersion >= 767)
        {
            return Value switch
            {
                0 => "bug_report",
                1 => "community_guidelines",
                2 => "support",
                3 => "status",
                4 => "feedback",
                5 => "community",
                6 => "website",
                7 => "forums",
                8 => "news",
                9 => "announcements",
                _ => $"unknown({Value})"};
        }

        return $"unknown({Value})";
    }
}
