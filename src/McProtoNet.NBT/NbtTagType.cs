namespace McProtoNet.NBT;

/// <summary>
/// Specifies the type of a named binary tag.
/// </summary>
public enum NbtTagType : byte
{
    /// <summary>TAG_End. Marks the end of an open TAG_Compound, and is the element type of an empty
    /// TAG_List.</summary>
    End = 0x00,

    /// <summary>TAG_Byte. A single byte.</summary>
    Byte = 0x01,

    /// <summary>TAG_Short. A single signed 16-bit integer.</summary>
    Short = 0x02,

    /// <summary>TAG_Int. A single signed 32-bit integer.</summary>
    Int = 0x03,

    /// <summary>TAG_Long. A single signed 64-bit integer.</summary>
    Long = 0x04,

    /// <summary>TAG_Float. A single IEEE 754 single-precision floating-point number.</summary>
    Float = 0x05,

    /// <summary>TAG_Double. A single IEEE 754 double-precision floating-point number.</summary>
    Double = 0x06,

    /// <summary>TAG_Byte_Array. A length-prefixed array of bytes.</summary>
    ByteArray = 0x07,

    /// <summary>TAG_String. A length-prefixed string in modified UTF-8. See <see cref="ModifiedUtf8"/>.</summary>
    String = 0x08,

    /// <summary>TAG_List. A list of nameless tags that all have the same type.</summary>
    List = 0x09,

    /// <summary>TAG_Compound. A set of named tags.</summary>
    Compound = 0x0a,

    /// <summary>TAG_Int_Array. A length-prefixed array of signed 32-bit integers.</summary>
    IntArray = 0x0b,

    /// <summary>TAG_Long_Array. A length-prefixed array of signed 64-bit integers.</summary>
    LongArray = 0x0c
}
