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

    public static readonly MinecraftVersion V1_16_4_To_1_16_5 =
        new(V1_16_4_To_1_16_5_Protocol, "1.16.4–1.16.5");

    public static readonly MinecraftVersion V1_17 =
        new(755, "1.17");

    public static readonly MinecraftVersion V1_17_1 =
        new(756, "1.17.1");

    public static readonly MinecraftVersion V1_18_To_1_18_1 =
        new(757, "1.18–1.18.1");

    public static readonly MinecraftVersion V1_18_2 =
        new(758, "1.18.2");

    public static readonly MinecraftVersion V1_19 =
        new(759, "1.19");

    public static readonly MinecraftVersion V1_19_2 =
        new(760, "1.19.2");

    public static readonly MinecraftVersion V1_19_3 =
        new(761, "1.19.3");

    public static readonly MinecraftVersion V1_19_4 =
        new(762, "1.19.4");

    public static readonly MinecraftVersion V1_20_To_1_20_1 =
        new(763, "1.20–1.20.1");

    public static readonly MinecraftVersion V1_20_2 =
        new(764, "1.20.2");

    public static readonly MinecraftVersion V1_20_3_To_1_20_4 =
        new(765, "1.20.3–1.20.4");

    public static readonly MinecraftVersion V1_20_5_To_1_20_6 =
        new(766, "1.20.5–1.20.6");

    public static readonly MinecraftVersion V1_21_To_1_21_1 =
        new(767, "1.21–1.21.1");

    public static readonly MinecraftVersion V1_21_3 =
        new(768, "1.21.3");

    public static readonly MinecraftVersion V1_21_4 =
        new(769, "1.21.4");

    public static readonly MinecraftVersion V1_21_5 =
        new(770, "1.21.5");

    public static readonly MinecraftVersion V1_21_6 =
        new(771, "1.21.6");

    public static readonly MinecraftVersion V1_21_7_To_1_21_8 =
        new(772, "1.21.7–1.21.8");

    public static readonly MinecraftVersion V1_21_9_To_1_21_10 =
        new(V1_21_9_To_1_21_10_Protocol, "1.21.9-1.21.10");

    public const int V1_16_4_To_1_16_5_Protocol = 754;
    public const int V1_21_9_To_1_21_10_Protocol = 773;

    public const int StartProtocol = V1_16_4_To_1_16_5_Protocol;
    public const int LatestProtocol = V1_21_9_To_1_21_10_Protocol;

    public static MinecraftVersion StartVersion => V1_16_4_To_1_16_5;
    public static MinecraftVersion Latest => V1_21_9_To_1_21_10;

    #endregion

    public static MinecraftVersion FromProtocol(int protocol)
    {
        return protocol switch
        {
            754 => V1_16_4_To_1_16_5,
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
            _ => throw new NotSupportedException($"Protocol {protocol} not supported")
        };
    }
}