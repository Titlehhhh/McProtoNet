using System.Collections.Generic;
using System.Reflection;

namespace McProtoNet.Protocol;

/// <summary>
/// Represents Minecraft versions with their corresponding protocol numbers
/// </summary>
public readonly struct MinecraftVersion :
    IEquatable<MinecraftVersion>,
    IComparable<MinecraftVersion>
{
    public int Protocol { get; }
    public string Name { get; }

    public MinecraftVersion(int protocol, string name)
    {
        Protocol = protocol;
        Name = name;
    }

    public int CompareTo(MinecraftVersion other)
        => Protocol.CompareTo(other.Protocol);

    public bool Equals(MinecraftVersion other)
        => Protocol == other.Protocol;

    public override bool Equals(object? obj)
        => obj is MinecraftVersion other && Equals(other);

    public override int GetHashCode()
        => Protocol;

    public override string ToString()
        => $"{Name} (protocol {Protocol})";

    public int ToInt32() => Protocol;
    public static implicit operator int(MinecraftVersion v) => v.Protocol;

    public static bool operator ==(MinecraftVersion left, MinecraftVersion right)
    {
        return left.Protocol.Equals(right.Protocol);
    }

    public static bool operator !=(MinecraftVersion left, MinecraftVersion right)
    {
        return left.Protocol != right.Protocol;
    }

    public static bool operator <(MinecraftVersion left, MinecraftVersion right)
    {
        return left.Protocol < right.Protocol;
    }

    public static bool operator >(MinecraftVersion left, MinecraftVersion right)
    {
        return left.Protocol > right.Protocol;
    }

    public static bool operator <=(MinecraftVersion left, MinecraftVersion right)
    {
        return left.Protocol <= right.Protocol;
    }

    public static bool operator >=(MinecraftVersion left, MinecraftVersion right)
    {
        return left.Protocol >= right.Protocol;
    }

    #region Constans

    public static readonly MinecraftVersion V1_16 = new(V1_16_Protocol, "1.16");
    public static readonly MinecraftVersion V1_16_1 = new( 	736 , "1.16.1");

    public static readonly MinecraftVersion V1_16_2_Snapshot_20w27a = new(738, "20w27a");

    public static readonly MinecraftVersion V1_16_2_Snapshot_20w28a = new(740, "20w28a");

    public static readonly MinecraftVersion V1_16_2_Snapshot_20w29a = new(741, "20w29a");

    public static readonly MinecraftVersion V1_16_2_Snapshot_20w30a = new(743, "20w30a");

    public static readonly MinecraftVersion V1_16_2_Pre1 = new(744, "1.16.2-pre1");

    public static readonly MinecraftVersion V1_16_2_Pre2 = new(746, "1.16.2-pre2");

    public static readonly MinecraftVersion V1_16_2_Pre3 = new(748, "1.16.2-pre3");

    public static readonly MinecraftVersion V1_16_2_Rc1 = new(749, "1.16.2-rc1");

    public static readonly MinecraftVersion V1_16_2_Rc2 = new(750, "1.16.2-rc2");

    public static readonly MinecraftVersion V1_16_2 = new(751, "1.16.2");

    public static readonly MinecraftVersion V1_16_3_Rc1 = new(752, "1.16.3-rc1");

    public static readonly MinecraftVersion V1_16_3 = new(753, "1.16.3");

    public static readonly MinecraftVersion V1_16_4_To_1_16_5 = new(754, "1.16.4–1.16.5");

    public static readonly MinecraftVersion V1_17 = new(755, "1.17");

    public static readonly MinecraftVersion V1_17_1 = new(756, "1.17.1");

    public static readonly MinecraftVersion V1_18_To_1_18_1 = new(757, "1.18–1.18.1");

    public static readonly MinecraftVersion V1_18_2 = new(758, "1.18.2");

    public static readonly MinecraftVersion V1_19 = new(759, "1.19");

    public static readonly MinecraftVersion V1_19_2 = new(760, "1.19.2");

    public static readonly MinecraftVersion V1_19_3 = new(761, "1.19.3");

    public static readonly MinecraftVersion V1_19_4 = new(762, "1.19.4");
    public static readonly MinecraftVersion V1_20_To_1_20_1 = new(763, "1.20–1.20.1");

    public static readonly MinecraftVersion V1_20_2 = new(764, "1.20.2");

    public static readonly MinecraftVersion V1_20_3_To_1_20_4 = new(765, "1.20.3–1.20.4");

    public static readonly MinecraftVersion V1_20_5_To_1_20_6 = new(766, "1.20.5–1.20.6");

    public static readonly MinecraftVersion V1_21_To_1_21_1 = new(767, "1.21–1.21.1");

    public static readonly MinecraftVersion V1_21_3 = new(768, "1.21.3");

    public static readonly MinecraftVersion V1_21_4 = new(769, "1.21.4");

    public static readonly MinecraftVersion V1_21_5 = new(770, "1.21.5");

    public static readonly MinecraftVersion V1_21_6 = new(771, "1.21.6");

    public static readonly MinecraftVersion V1_21_7_To_1_21_8 = new(V1_21_7_To_1_21_8_Protocol, "1.21.7–1.21.8");

    //public static readonly MinecraftVersion V1_21_9_To_1_21_10 =
    //     new(V1_21_9_To_1_21_10_Protocol, "1.21.9-1.21.10");

    public const int V1_16_Protocol = 735;
    public const int V1_21_7_To_1_21_8_Protocol = 772;

    public const int StartProtocol = V1_16_Protocol;
    public const int LatestProtocol = V1_21_7_To_1_21_8_Protocol;

    public static MinecraftVersion StartVersion => V1_16_4_To_1_16_5;
    public static MinecraftVersion Latest => V1_21_7_To_1_21_8;

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
            // 773 => V1_21_9_To_1_21_10,

            _ => throw new NotSupportedException($"Protocol {protocol} not supported")
        };
    }

}
