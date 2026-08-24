using System.Globalization;
using System.Text;

namespace McProtoNet.NBT;

/// <summary>
/// Represents a named binary tag. This class is abstract.
/// </summary>
public abstract class NbtTag : ICloneable
{
    /// <summary>
    /// The tag that marks the end of a compound. This field is read-only.
    /// </summary>
    public static readonly NbtTag EndTag = new NbtEnd();

    private static string _defaultIndentString = "  ";

    /// <summary>Backing field of <see cref="Name"/>, assigned directly to bypass the checks of the setter.</summary>
    internal string? StrName;

    /// <summary>
    /// Gets the tag that contains this tag.
    /// </summary>
    /// <value>
    /// The <see cref="NbtCompound"/> or <see cref="NbtList"/> that contains this tag, or <see langword="null"/> if
    /// this tag is not contained in another tag.
    /// </value>
    public NbtTag? Parent { get; internal set; }

    /// <summary>
    /// When overridden in a derived class, gets the type of this tag.
    /// </summary>
    public abstract NbtTagType TagType { get; }

    /// <summary>
    /// Gets a value indicating whether this tag carries a value.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if the type of this tag is neither <see cref="NbtTagType.Compound"/>,
    /// <see cref="NbtTagType.List"/> nor <see cref="NbtTagType.End"/>; otherwise, <see langword="false"/>.
    /// </value>
    public bool HasValue
    {
        get
        {
            return TagType switch
            {
                NbtTagType.Compound => false,
                NbtTagType.End => false,
                NbtTagType.List => false,
                _ => true
            };
        }
    }

    /// <summary>
    /// Gets or sets the name of this tag.
    /// </summary>
    /// <value>
    /// The name of this tag, or <see langword="null"/> if this tag is unnamed.
    /// </value>
    /// <exception cref="ArgumentNullException">The property is set to <see langword="null"/> while this tag is
    /// contained in an <see cref="NbtCompound"/>. A tag inside a compound must be named.</exception>
    /// <exception cref="ArgumentException">This tag is contained in an <see cref="NbtCompound"/> that already holds
    /// a tag with the new name.</exception>
    /// <remarks>
    /// Setting this property while the tag is contained in an <see cref="NbtCompound"/> also renames the entry in
    /// that compound.
    /// </remarks>
    public string? Name
    {
        get => StrName;
        set
        {
            if (StrName == value) return;

            if (Parent is NbtCompound parentAsCompound)
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value),
                        "Name of tags inside an NbtCompound may not be null.");

                if (StrName != null) parentAsCompound.RenameTag(StrName, value);
            }

            StrName = value;
        }
    }

    /// <summary>
    /// Gets the full name of this tag, which includes the names of all parent tags separated by periods.
    /// </summary>
    /// <value>
    /// The full name of this tag. An unnamed tag contributes an empty string; an element of an
    /// <see cref="NbtList"/> contributes its index in brackets.
    /// </value>
    public string Path
    {
        get
        {
            if (Parent == null) return Name ?? "";
            if (Parent is NbtList parentAsList) return parentAsList.Path + '[' + parentAsList.IndexOf(this) + ']';

            return Parent.Path + '.' + Name;
        }
    }

    /// <summary>
    /// Gets or sets the string that <see cref="ToString()"/> uses for one level of indentation.
    /// </summary>
    /// <value>
    /// The indentation string. The default is two spaces.
    /// </value>
    /// <exception cref="ArgumentNullException">The property is set to <see langword="null"/>.</exception>
    public static string DefaultIndentString
    {
        get => _defaultIndentString;
        set => _defaultIndentString = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <summary>
    /// When overridden in a derived class, creates a deep copy of this tag.
    /// </summary>
    /// <returns>A new <see cref="NbtTag"/> that is a deep copy of this instance. The copy has no parent.</returns>
    public abstract object Clone();

    internal abstract void ReadTag(NbtBinaryReader readStream);


    internal abstract void WriteTag(NbtBinaryWriter writeReader);

    internal static NbtTag CreateTag(NbtTagType type)
    {
        return type switch
        {
            NbtTagType.Byte => new NbtByte(),
            NbtTagType.Short => new NbtShort(),
            NbtTagType.Int => new NbtInt(),
            NbtTagType.Long => new NbtLong(),
            NbtTagType.Float => new NbtFloat(),
            NbtTagType.Double => new NbtDouble(),
            NbtTagType.ByteArray => new NbtByteArray(),
            NbtTagType.String => new NbtString(),
            NbtTagType.List => new NbtList(),
            NbtTagType.Compound => new NbtCompound(),
            NbtTagType.IntArray => new NbtIntArray(),
            NbtTagType.LongArray => new NbtLongArray(),
            _ => throw new NbtFormatException("Cannot create an NBT tag of type " + type)
        };
    }

    /// <summary>Writes the payload of this tag, without the type byte and without the name.</summary>
    internal abstract void WriteData(NbtBinaryWriter writeStream);

    /// <summary>
    /// Returns the canonical name of the specified tag type, such as <c>TAG_Byte_Array</c> for
    /// <see cref="NbtTagType.ByteArray"/>.
    /// </summary>
    /// <param name="type">The tag type to name.</param>
    /// <returns>The canonical name of the tag type, or <see langword="null"/> if <paramref name="type"/> is not a
    /// recognized tag type.</returns>
    public static string? GetCanonicalTagName(NbtTagType type)
    {
        return type switch
        {
            NbtTagType.Byte => "TAG_Byte",
            NbtTagType.ByteArray => "TAG_Byte_Array",
            NbtTagType.Compound => "TAG_Compound",
            NbtTagType.Double => "TAG_Double",
            NbtTagType.End => "TAG_End",
            NbtTagType.Float => "TAG_Float",
            NbtTagType.Int => "TAG_Int",
            NbtTagType.IntArray => "TAG_Int_Array",
            NbtTagType.LongArray => "TAG_Long_Array",
            NbtTagType.List => "TAG_List",
            NbtTagType.Long => "TAG_Long",
            NbtTagType.Short => "TAG_Short",
            NbtTagType.String => "TAG_String",
            _ => null
        };
    }

    /// <summary>
    /// Returns a string that represents this tag and all its child tags, indented with
    /// <see cref="DefaultIndentString"/>.
    /// </summary>
    /// <returns>A string that represents this tag and all its child tags.</returns>
    public override string ToString()
    {
        return ToString(DefaultIndentString);
    }

    /// <summary>
    /// Returns a string that represents this tag and all its child tags, indented with the specified string.
    /// </summary>
    /// <param name="indentString">The string used for one level of indentation.</param>
    /// <returns>A string that represents this tag and all its child tags.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="indentString"/> is
    /// <see langword="null"/>.</exception>
    public string ToString(string indentString)
    {
        if (indentString == null) throw new ArgumentNullException(nameof(indentString));
        var sb = new StringBuilder();
        PrettyPrint(sb, indentString, 0);
        return sb.ToString();
    }

    internal abstract void PrettyPrint(StringBuilder sb, string indentString, int indentLevel);

    #region Shortcuts

    /// <summary>
    /// Gets or sets the tag with the specified name.
    /// </summary>
    /// <param name="tagName">The name of the tag to get or set. It must match the name of the tag being set.</param>
    /// <returns>The tag with the specified name, or <see langword="null"/> if no tag with that name is
    /// present.</returns>
    /// <exception cref="InvalidOperationException">This tag is not an <see cref="NbtCompound"/>.</exception>
    /// <remarks>
    /// Only <see cref="NbtCompound"/> overrides this indexer. It is declared on <see cref="NbtTag"/> so that a
    /// caller does not have to cast.
    /// </remarks>
    public virtual NbtTag? this[string tagName]
    {
        get => throw new InvalidOperationException("String indexers only work on NbtCompound tags.");
        set => throw new InvalidOperationException("String indexers only work on NbtCompound tags.");
    }

    /// <summary>
    /// Gets or sets the tag at the specified index.
    /// </summary>
    /// <param name="tagIndex">The zero-based index of the tag to get or set.</param>
    /// <returns>The tag at the specified index.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="tagIndex"/> is not a valid index in this
    /// tag.</exception>
    /// <exception cref="ArgumentNullException">The property is set to <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">The type of the tag being set does not match
    /// <see cref="NbtList.ListType"/>.</exception>
    /// <exception cref="InvalidOperationException">This tag is not an <see cref="NbtList"/>.</exception>
    /// <remarks>
    /// Only <see cref="NbtList"/> overrides this indexer. It is declared on <see cref="NbtTag"/> so that a caller
    /// does not have to cast. The array tags declare their own indexers over their element type.
    /// </remarks>
    public virtual NbtTag this[int tagIndex]
    {
        get => throw new InvalidOperationException("Integer indexers only work on NbtList tags.");
        set => throw new InvalidOperationException("Integer indexers only work on NbtList tags.");
    }

    /// <summary>
    /// Gets a value indicating whether the value of this tag is nonzero.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if the value of this tag is not 0; otherwise, <see langword="false"/>.
    /// </value>
    /// <exception cref="InvalidCastException">This tag is not an <see cref="NbtByte"/>.</exception>
    public bool BoolValue => ByteValue != 0;

    /// <summary>
    /// Gets the value of this tag as a byte.
    /// </summary>
    /// <value>
    /// The value of this tag.
    /// </value>
    /// <exception cref="InvalidCastException">This tag is not an <see cref="NbtByte"/>.</exception>
    public byte ByteValue
    {
        get
        {
            if (TagType == NbtTagType.Byte) return ((NbtByte)this).Value;

            throw new InvalidCastException("Cannot get ByteValue from " + GetCanonicalTagName(TagType));
        }
    }

    /// <summary>
    /// Gets the value of this tag as a signed 16-bit integer.
    /// </summary>
    /// <value>
    /// The value of this tag, converted to a signed 16-bit integer.
    /// </value>
    /// <exception cref="InvalidCastException">This tag is neither an <see cref="NbtByte"/> nor an
    /// <see cref="NbtShort"/>.</exception>
    public short ShortValue
    {
        get
        {
            return TagType switch
            {
                NbtTagType.Byte => ((NbtByte)this).Value,
                NbtTagType.Short => ((NbtShort)this).Value,
                _ => throw new InvalidCastException("Cannot get ShortValue from " + GetCanonicalTagName(TagType))
            };
        }
    }

    /// <summary>
    /// Gets the value of this tag as a signed 32-bit integer.
    /// </summary>
    /// <value>
    /// The value of this tag, converted to a signed 32-bit integer.
    /// </value>
    /// <exception cref="InvalidCastException">This tag is not an <see cref="NbtByte"/>, an <see cref="NbtShort"/> or
    /// an <see cref="NbtInt"/>.</exception>
    public int IntValue
    {
        get
        {
            return TagType switch
            {
                NbtTagType.Byte => ((NbtByte)this).Value,
                NbtTagType.Short => ((NbtShort)this).Value,
                NbtTagType.Int => ((NbtInt)this).Value,
                _ => throw new InvalidCastException("Cannot get IntValue from " + GetCanonicalTagName(TagType))
            };
        }
    }

    /// <summary>
    /// Gets the value of this tag as a signed 64-bit integer.
    /// </summary>
    /// <value>
    /// The value of this tag, converted to a signed 64-bit integer.
    /// </value>
    /// <exception cref="InvalidCastException">This tag is not an <see cref="NbtByte"/>, an <see cref="NbtShort"/>,
    /// an <see cref="NbtInt"/> or an <see cref="NbtLong"/>.</exception>
    public long LongValue
    {
        get
        {
            return TagType switch
            {
                NbtTagType.Byte => ((NbtByte)this).Value,
                NbtTagType.Short => ((NbtShort)this).Value,
                NbtTagType.Int => ((NbtInt)this).Value,
                NbtTagType.Long => ((NbtLong)this).Value,
                _ => throw new InvalidCastException("Cannot get LongValue from " + GetCanonicalTagName(TagType))
            };
        }
    }

    /// <summary>
    /// Gets the value of this tag as a single-precision floating-point number.
    /// </summary>
    /// <value>
    /// The value of this tag, converted to a single-precision floating-point number. The conversion from
    /// <see cref="NbtDouble"/>, <see cref="NbtInt"/> and <see cref="NbtLong"/> can lose precision.
    /// </value>
    /// <exception cref="InvalidCastException">This tag is not an <see cref="NbtByte"/>, an <see cref="NbtShort"/>,
    /// an <see cref="NbtInt"/>, an <see cref="NbtLong"/>, an <see cref="NbtFloat"/> or an
    /// <see cref="NbtDouble"/>.</exception>
    public float FloatValue
    {
        get
        {
            return TagType switch
            {
                NbtTagType.Byte => ((NbtByte)this).Value,
                NbtTagType.Short => ((NbtShort)this).Value,
                NbtTagType.Int => ((NbtInt)this).Value,
                NbtTagType.Long => ((NbtLong)this).Value,
                NbtTagType.Float => ((NbtFloat)this).Value,
                NbtTagType.Double => (float)((NbtDouble)this).Value,
                _ => throw new InvalidCastException("Cannot get FloatValue from " + GetCanonicalTagName(TagType))
            };
        }
    }

    /// <summary>
    /// Gets the value of this tag as a double-precision floating-point number.
    /// </summary>
    /// <value>
    /// The value of this tag, converted to a double-precision floating-point number. The conversion from
    /// <see cref="NbtLong"/> can lose precision.
    /// </value>
    /// <exception cref="InvalidCastException">This tag is not an <see cref="NbtByte"/>, an <see cref="NbtShort"/>,
    /// an <see cref="NbtInt"/>, an <see cref="NbtLong"/>, an <see cref="NbtFloat"/> or an
    /// <see cref="NbtDouble"/>.</exception>
    public double DoubleValue
    {
        get
        {
            return TagType switch
            {
                NbtTagType.Byte => ((NbtByte)this).Value,
                NbtTagType.Short => ((NbtShort)this).Value,
                NbtTagType.Int => ((NbtInt)this).Value,
                NbtTagType.Long => ((NbtLong)this).Value,
                NbtTagType.Float => ((NbtFloat)this).Value,
                NbtTagType.Double => ((NbtDouble)this).Value,
                _ => throw new InvalidCastException("Cannot get DoubleValue from " + GetCanonicalTagName(TagType))
            };
        }
    }

    /// <summary>
    /// Gets the value of this tag as an array of bytes.
    /// </summary>
    /// <value>
    /// The array held by this tag. The array is returned by reference and is not copied.
    /// </value>
    /// <exception cref="InvalidCastException">This tag is not an <see cref="NbtByteArray"/>.</exception>
    public byte[] ByteArrayValue
    {
        get
        {
            if (TagType == NbtTagType.ByteArray) return ((NbtByteArray)this).Value;

            throw new InvalidCastException("Cannot get ByteArrayValue from " + GetCanonicalTagName(TagType));
        }
    }

    /// <summary>
    /// Gets the value of this tag as an array of signed 32-bit integers.
    /// </summary>
    /// <value>
    /// The array held by this tag. The array is returned by reference and is not copied.
    /// </value>
    /// <exception cref="InvalidCastException">This tag is not an <see cref="NbtIntArray"/>.</exception>
    public int[] IntArrayValue
    {
        get
        {
            if (TagType == NbtTagType.IntArray) return ((NbtIntArray)this).Value;

            throw new InvalidCastException("Cannot get IntArrayValue from " + GetCanonicalTagName(TagType));
        }
    }

    /// <summary>
    /// Gets the value of this tag as an array of signed 64-bit integers.
    /// </summary>
    /// <value>
    /// The array held by this tag. The array is returned by reference and is not copied.
    /// </value>
    /// <exception cref="InvalidCastException">This tag is not an <see cref="NbtLongArray"/>.</exception>
    public long[] LongArrayValue
    {
        get
        {
            if (TagType == NbtTagType.LongArray) return ((NbtLongArray)this).Value;

            throw new InvalidCastException("Cannot get LongArrayValue from " + GetCanonicalTagName(TagType));
        }
    }

    /// <summary>
    /// Gets the value of this tag as a string.
    /// </summary>
    /// <value>
    /// The exact value for an <see cref="NbtString"/>; for a numeric tag, the value formatted with
    /// <see cref="CultureInfo.InvariantCulture"/>.
    /// </value>
    /// <exception cref="InvalidCastException">This tag is neither an <see cref="NbtString"/> nor a numeric
    /// tag.</exception>
    public string StringValue
    {
        get
        {
            return TagType switch
            {
                NbtTagType.String => ((NbtString)this).Value,
                NbtTagType.Byte => ((NbtByte)this).Value.ToString(CultureInfo.InvariantCulture),
                NbtTagType.Double => ((NbtDouble)this).Value.ToString(CultureInfo.InvariantCulture),
                NbtTagType.Float => ((NbtFloat)this).Value.ToString(CultureInfo.InvariantCulture),
                NbtTagType.Int => ((NbtInt)this).Value.ToString(CultureInfo.InvariantCulture),
                NbtTagType.Long => ((NbtLong)this).Value.ToString(CultureInfo.InvariantCulture),
                NbtTagType.Short => ((NbtShort)this).Value.ToString(CultureInfo.InvariantCulture),
                _ => throw new InvalidCastException("Cannot get StringValue from " + GetCanonicalTagName(TagType))
            };
        }
    }

    #endregion
}