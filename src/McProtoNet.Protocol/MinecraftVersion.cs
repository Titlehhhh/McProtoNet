using System.Collections.Generic;
using System.Reflection;

namespace McProtoNet.Protocol;

/// <summary>
/// Represents a Minecraft version and the protocol number that identifies it on the wire.
/// </summary>
/// <remarks>
/// Comparison and equality use the protocol number only. Two instances with the same protocol
/// number are equal even when their names differ.
/// </remarks>
public readonly struct MinecraftVersion :
    IEquatable<MinecraftVersion>,
    IComparable<MinecraftVersion>
{
    /// <summary>
    /// Gets the protocol number of the version.
    /// </summary>
    public int Protocol { get; }

    /// <summary>
    /// Gets the display name of the version, such as <c>1.21.11</c> or <c>1.20.3–1.20.4</c>.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MinecraftVersion"/> structure with the specified
    /// protocol number and name.
    /// </summary>
    /// <param name="protocol">The protocol number of the version.</param>
    /// <param name="name">The display name of the version.</param>
    public MinecraftVersion(int protocol, string name)
    {
        Protocol = protocol;
        Name = name;
    }

    /// <summary>
    /// Compares the current instance with another <see cref="MinecraftVersion"/> and returns their
    /// relative order by protocol number.
    /// </summary>
    /// <param name="other">The version to compare with the current instance.</param>
    /// <returns>
    /// A value less than zero if this instance precedes <paramref name="other"/>; zero if both have the
    /// same protocol number; a value greater than zero if this instance follows <paramref name="other"/>.
    /// </returns>
    public int CompareTo(MinecraftVersion other)
        => Protocol.CompareTo(other.Protocol);

    /// <summary>
    /// Determines whether the current instance and another <see cref="MinecraftVersion"/> have the same
    /// protocol number.
    /// </summary>
    /// <param name="other">The version to compare with the current instance.</param>
    /// <returns>
    /// <see langword="true"/> if both have the same protocol number; otherwise, <see langword="false"/>.
    /// </returns>
    public bool Equals(MinecraftVersion other)
        => Protocol == other.Protocol;

    /// <summary>
    /// Determines whether the specified object is a <see cref="MinecraftVersion"/> with the same protocol
    /// number as the current instance.
    /// </summary>
    /// <param name="obj">The object to compare with the current instance.</param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="obj"/> is a <see cref="MinecraftVersion"/> with the same
    /// protocol number; otherwise, <see langword="false"/>.
    /// </returns>
    public override bool Equals(object? obj)
        => obj is MinecraftVersion other && Equals(other);

    /// <summary>
    /// Returns the hash code for the current instance.
    /// </summary>
    /// <returns>The protocol number of the version.</returns>
    public override int GetHashCode()
        => Protocol;

    /// <summary>
    /// Returns the string representation of the version.
    /// </summary>
    /// <returns>The name of the version followed by its protocol number.</returns>
    public override string ToString()
        => $"{Name} (protocol {Protocol})";

    /// <summary>
    /// Returns the protocol number of the version as a 32-bit signed integer.
    /// </summary>
    /// <returns>The protocol number of the version.</returns>
    public int ToInt32() => Protocol;

    /// <summary>
    /// Converts a <see cref="MinecraftVersion"/> to its protocol number.
    /// </summary>
    /// <param name="v">The version to convert.</param>
    /// <returns>The protocol number of <paramref name="v"/>.</returns>
    public static implicit operator int(MinecraftVersion v) => v.Protocol;

    /// <summary>
    /// Determines whether two specified versions have the same protocol number.
    /// </summary>
    /// <param name="left">The first version to compare.</param>
    /// <param name="right">The second version to compare.</param>
    /// <returns>
    /// <see langword="true"/> if both have the same protocol number; otherwise, <see langword="false"/>.
    /// </returns>
    public static bool operator ==(MinecraftVersion left, MinecraftVersion right)
    {
        return left.Protocol.Equals(right.Protocol);
    }

    /// <summary>
    /// Determines whether two specified versions have different protocol numbers.
    /// </summary>
    /// <param name="left">The first version to compare.</param>
    /// <param name="right">The second version to compare.</param>
    /// <returns>
    /// <see langword="true"/> if the protocol numbers differ; otherwise, <see langword="false"/>.
    /// </returns>
    public static bool operator !=(MinecraftVersion left, MinecraftVersion right)
    {
        return left.Protocol != right.Protocol;
    }

    /// <summary>
    /// Determines whether the protocol number of one version is less than that of another version.
    /// </summary>
    /// <param name="left">The first version to compare.</param>
    /// <param name="right">The second version to compare.</param>
    /// <returns>
    /// <see langword="true"/> if the protocol number of <paramref name="left"/> is less than that of
    /// <paramref name="right"/>; otherwise, <see langword="false"/>.
    /// </returns>
    public static bool operator <(MinecraftVersion left, MinecraftVersion right)
    {
        return left.Protocol < right.Protocol;
    }

    /// <summary>
    /// Determines whether the protocol number of one version is greater than that of another version.
    /// </summary>
    /// <param name="left">The first version to compare.</param>
    /// <param name="right">The second version to compare.</param>
    /// <returns>
    /// <see langword="true"/> if the protocol number of <paramref name="left"/> is greater than that of
    /// <paramref name="right"/>; otherwise, <see langword="false"/>.
    /// </returns>
    public static bool operator >(MinecraftVersion left, MinecraftVersion right)
    {
        return left.Protocol > right.Protocol;
    }

    /// <summary>
    /// Determines whether the protocol number of one version is less than or equal to that of another
    /// version.
    /// </summary>
    /// <param name="left">The first version to compare.</param>
    /// <param name="right">The second version to compare.</param>
    /// <returns>
    /// <see langword="true"/> if the protocol number of <paramref name="left"/> is less than or equal to
    /// that of <paramref name="right"/>; otherwise, <see langword="false"/>.
    /// </returns>
    public static bool operator <=(MinecraftVersion left, MinecraftVersion right)
    {
        return left.Protocol <= right.Protocol;
    }

    /// <summary>
    /// Determines whether the protocol number of one version is greater than or equal to that of another
    /// version.
    /// </summary>
    /// <param name="left">The first version to compare.</param>
    /// <param name="right">The second version to compare.</param>
    /// <returns>
    /// <see langword="true"/> if the protocol number of <paramref name="left"/> is greater than or equal
    /// to that of <paramref name="right"/>; otherwise, <see langword="false"/>.
    /// </returns>
    public static bool operator >=(MinecraftVersion left, MinecraftVersion right)
    {
        return left.Protocol >= right.Protocol;
    }

    #region Constans

    /// <summary>Minecraft 1.16, protocol 735.</summary>
    public static readonly MinecraftVersion V1_16 = new(V1_16_Protocol, "1.16");

    /// <summary>Minecraft 1.16.1, protocol 736.</summary>
    public static readonly MinecraftVersion V1_16_1 = new( 	736 , "1.16.1");

    /// <summary>The 1.16.2 snapshot 20w27a, protocol 738.</summary>
    public static readonly MinecraftVersion V1_16_2_Snapshot_20w27a = new(738, "20w27a");

    /// <summary>The 1.16.2 snapshot 20w28a, protocol 740.</summary>
    public static readonly MinecraftVersion V1_16_2_Snapshot_20w28a = new(740, "20w28a");

    /// <summary>The 1.16.2 snapshot 20w29a, protocol 741.</summary>
    public static readonly MinecraftVersion V1_16_2_Snapshot_20w29a = new(741, "20w29a");

    /// <summary>The 1.16.2 snapshot 20w30a, protocol 743.</summary>
    public static readonly MinecraftVersion V1_16_2_Snapshot_20w30a = new(743, "20w30a");

    /// <summary>Minecraft 1.16.2-pre1, protocol 744.</summary>
    public static readonly MinecraftVersion V1_16_2_Pre1 = new(744, "1.16.2-pre1");

    /// <summary>Minecraft 1.16.2-pre2, protocol 746.</summary>
    public static readonly MinecraftVersion V1_16_2_Pre2 = new(746, "1.16.2-pre2");

    /// <summary>Minecraft 1.16.2-pre3, protocol 748.</summary>
    public static readonly MinecraftVersion V1_16_2_Pre3 = new(748, "1.16.2-pre3");

    /// <summary>Minecraft 1.16.2-rc1, protocol 749.</summary>
    public static readonly MinecraftVersion V1_16_2_Rc1 = new(749, "1.16.2-rc1");

    /// <summary>Minecraft 1.16.2-rc2, protocol 750.</summary>
    public static readonly MinecraftVersion V1_16_2_Rc2 = new(750, "1.16.2-rc2");

    /// <summary>Minecraft 1.16.2, protocol 751.</summary>
    public static readonly MinecraftVersion V1_16_2 = new(751, "1.16.2");

    /// <summary>Minecraft 1.16.3-rc1, protocol 752.</summary>
    public static readonly MinecraftVersion V1_16_3_Rc1 = new(752, "1.16.3-rc1");

    /// <summary>Minecraft 1.16.3, protocol 753.</summary>
    public static readonly MinecraftVersion V1_16_3 = new(753, "1.16.3");

    /// <summary>Minecraft 1.16.4 through 1.16.5, protocol 754.</summary>
    public static readonly MinecraftVersion V1_16_4_To_1_16_5 = new(754, "1.16.4–1.16.5");

    /// <summary>Minecraft 1.17, protocol 755.</summary>
    public static readonly MinecraftVersion V1_17 = new(755, "1.17");

    /// <summary>Minecraft 1.17.1, protocol 756.</summary>
    public static readonly MinecraftVersion V1_17_1 = new(756, "1.17.1");

    /// <summary>Minecraft 1.18 through 1.18.1, protocol 757.</summary>
    public static readonly MinecraftVersion V1_18_To_1_18_1 = new(757, "1.18–1.18.1");

    /// <summary>Minecraft 1.18.2, protocol 758.</summary>
    public static readonly MinecraftVersion V1_18_2 = new(758, "1.18.2");

    /// <summary>Minecraft 1.19, protocol 759.</summary>
    public static readonly MinecraftVersion V1_19 = new(759, "1.19");

    /// <summary>Minecraft 1.19.2, protocol 760.</summary>
    public static readonly MinecraftVersion V1_19_2 = new(760, "1.19.2");

    /// <summary>Minecraft 1.19.3, protocol 761.</summary>
    public static readonly MinecraftVersion V1_19_3 = new(761, "1.19.3");

    /// <summary>Minecraft 1.19.4, protocol 762.</summary>
    public static readonly MinecraftVersion V1_19_4 = new(762, "1.19.4");

    /// <summary>Minecraft 1.20 through 1.20.1, protocol 763.</summary>
    public static readonly MinecraftVersion V1_20_To_1_20_1 = new(763, "1.20–1.20.1");

    /// <summary>Minecraft 1.20.2, protocol 764.</summary>
    public static readonly MinecraftVersion V1_20_2 = new(764, "1.20.2");

    /// <summary>Minecraft 1.20.3 through 1.20.4, protocol 765.</summary>
    public static readonly MinecraftVersion V1_20_3_To_1_20_4 = new(765, "1.20.3–1.20.4");

    /// <summary>Minecraft 1.20.5 through 1.20.6, protocol 766.</summary>
    public static readonly MinecraftVersion V1_20_5_To_1_20_6 = new(766, "1.20.5–1.20.6");

    /// <summary>Minecraft 1.21 through 1.21.1, protocol 767.</summary>
    public static readonly MinecraftVersion V1_21_To_1_21_1 = new(767, "1.21–1.21.1");

    /// <summary>Minecraft 1.21.3, protocol 768.</summary>
    public static readonly MinecraftVersion V1_21_3 = new(768, "1.21.3");

    /// <summary>Minecraft 1.21.4, protocol 769.</summary>
    public static readonly MinecraftVersion V1_21_4 = new(769, "1.21.4");

    /// <summary>Minecraft 1.21.5, protocol 770.</summary>
    public static readonly MinecraftVersion V1_21_5 = new(770, "1.21.5");

    /// <summary>Minecraft 1.21.6, protocol 771.</summary>
    public static readonly MinecraftVersion V1_21_6 = new(771, "1.21.6");

    /// <summary>Minecraft 1.21.7 through 1.21.8, protocol 772.</summary>
    public static readonly MinecraftVersion V1_21_7_To_1_21_8 = new(V1_21_7_To_1_21_8_Protocol, "1.21.7–1.21.8");

    /// <summary>Minecraft 1.21.9 through 1.21.10, protocol 773.</summary>
    public static readonly MinecraftVersion V1_21_9_To_1_21_10 =
        new(V1_21_9_To_1_21_10_Protocol, "1.21.9–1.21.10");

    /// <summary>Minecraft 1.21.11, protocol 774.</summary>
    public static readonly MinecraftVersion V1_21_11 = new(V1_21_11_Protocol, "1.21.11");

    /// <summary>Minecraft 26.1 through 26.1.2, protocol 775.</summary>
    public static readonly MinecraftVersion V26_1_To_26_1_2 =
        new(V26_1_To_26_1_2_Protocol, "26.1–26.1.2");

    /// <summary>Minecraft 26.2, protocol 776.</summary>
    public static readonly MinecraftVersion V26_2 = new(V26_2_Protocol, "26.2");

    /// <summary>The protocol number of Minecraft 1.16.</summary>
    public const int V1_16_Protocol = 735;

    /// <summary>The protocol number of Minecraft 1.21.7 through 1.21.8.</summary>
    public const int V1_21_7_To_1_21_8_Protocol = 772;

    /// <summary>The protocol number of Minecraft 1.21.9 through 1.21.10.</summary>
    public const int V1_21_9_To_1_21_10_Protocol = 773;

    /// <summary>The protocol number of Minecraft 1.21.11.</summary>
    public const int V1_21_11_Protocol = 774;

    /// <summary>The protocol number of Minecraft 26.1 through 26.1.2.</summary>
    public const int V26_1_To_26_1_2_Protocol = 775;

    /// <summary>The protocol number of Minecraft 26.2.</summary>
    public const int V26_2_Protocol = 776;

    /// <summary>The lowest protocol number this library supports.</summary>
    public const int StartProtocol = V1_16_Protocol;

    /// <summary>The highest protocol number this library supports.</summary>
    public const int LatestProtocol = V26_2_Protocol;

    /// <summary>
    /// Gets the oldest release version this library supports.
    /// </summary>
    public static MinecraftVersion StartVersion => V1_16_4_To_1_16_5;

    /// <summary>
    /// Gets the newest version this library supports.
    /// </summary>
    public static MinecraftVersion Latest => V26_2;

    /// <summary>
    /// Gets every version declared by this type, ordered by protocol number.
    /// </summary>
    /// <remarks>
    /// The list is built once through reflection over the public static fields of this type.
    /// </remarks>
    public static IReadOnlyList<MinecraftVersion> AllVersions { get; } = BuildKnownVersions();

    #endregion

    private static MinecraftVersion[] BuildKnownVersions()
    {
        var fields = typeof(MinecraftVersion).GetFields(BindingFlags.Public | BindingFlags.Static);
        var versions = new List<MinecraftVersion>(fields.Length);

        foreach (var field in fields)
        {
            if (field.FieldType == typeof(MinecraftVersion))
            {
                versions.Add((MinecraftVersion)field.GetValue(null)!);
            }
        }

        versions.Sort((left, right) => left.Protocol.CompareTo(right.Protocol));
        return versions.ToArray();
    }

    /// <summary>
    /// Returns the version that corresponds to the specified protocol number.
    /// </summary>
    /// <param name="protocol">The protocol number to look up.</param>
    /// <returns>The <see cref="MinecraftVersion"/> associated with <paramref name="protocol"/>.</returns>
    /// <exception cref="NotSupportedException"><paramref name="protocol"/> is not a known protocol
    /// number.</exception>
    /// <remarks>
    /// Protocol numbers of 1.16.2 snapshots, pre-releases and release candidates map to
    /// <see cref="V1_16_2"/>, and the protocol number of 1.16.3-rc1 maps to <see cref="V1_16_3"/>.
    /// </remarks>
    public static MinecraftVersion FromProtocol(int protocol)
    {
        return protocol switch
        {
            // === 1.16.x ===
            735 => V1_16,
            736 => V1_16_1,

            // 1.16.2 snapshots / pre / rc / release
            738 => V1_16_2,
            740 => V1_16_2,
            741 => V1_16_2,
            743 => V1_16_2,
            744 => V1_16_2,
            746 => V1_16_2,
            748 => V1_16_2,
            749 => V1_16_2,
            750 => V1_16_2,
            751 => V1_16_2,

            // 1.16.3
            752 => V1_16_3,
            753 => V1_16_3,

            // 1.16.4 – 1.16.5
            754 => V1_16_4_To_1_16_5,

            // === 1.17+ ===
            755 => V1_17,
            756 => V1_17_1,
            757 => V1_18_To_1_18_1,
            758 => V1_18_2,
            759 => V1_19,
            760 => V1_19_2,
            761 => V1_19_3,
            762 => V1_19_4,
            763 => V1_20_To_1_20_1,
            764 => V1_20_2,
            765 => V1_20_3_To_1_20_4,
            766 => V1_20_5_To_1_20_6,
            767 => V1_21_To_1_21_1,
            768 => V1_21_3,
            769 => V1_21_4,
            770 => V1_21_5,
            771 => V1_21_6,
            772 => V1_21_7_To_1_21_8,
            773 => V1_21_9_To_1_21_10,
            774 => V1_21_11,
            775 => V26_1_To_26_1_2,
            776 => V26_2,

            _ => throw new NotSupportedException($"Protocol {protocol} not supported")
        };
    }

}
