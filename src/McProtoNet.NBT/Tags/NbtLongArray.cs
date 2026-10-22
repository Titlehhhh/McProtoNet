using System.Text;

namespace McProtoNet.NBT;

/// <summary>
/// Represents an NBT tag that holds an array of signed 64-bit integers.
/// </summary>
public sealed class NbtLongArray : NbtTag
{
    private long[] _longs;

    /// <summary>
    /// Initializes a new instance of the <see cref="NbtLongArray"/> class that is unnamed and holds an empty array.
    /// </summary>
    public NbtLongArray()
        : this((string)null!)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NbtLongArray"/> class that is unnamed and holds a copy of the
    /// specified array.
    /// </summary>
    /// <param name="value">The array to copy into the tag. This value cannot be <see langword="null"/>.</param>
    /// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
    /// <remarks>
    /// The array is cloned. Setting <see cref="Value"/> stores an array without copying it.
    /// </remarks>
    public NbtLongArray(long[] value)
        : this(null!, value)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NbtLongArray"/> class with the specified name that holds an
    /// empty array.
    /// </summary>
    /// <param name="tagName">The name of the tag, or <see langword="null"/> for an unnamed tag.</param>
    public NbtLongArray(string? tagName)
    {
        Name = tagName;
        _longs = Array.Empty<long>();
    }

    internal static NbtLongArray CreateFromArray(long[] value, string? tagName)
    {
        NbtLongArray result = new();
        result.Name = tagName;
        result._longs = value;
        return result;
    }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="NbtLongArray"/> class with the specified name that holds a copy
    /// of the specified array.
    /// </summary>
    /// <param name="tagName">The name of the tag, or <see langword="null"/> for an unnamed tag.</param>
    /// <param name="value">The array to copy into the tag. This value cannot be <see langword="null"/>.</param>
    /// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
    /// <remarks>
    /// The array is cloned. Setting <see cref="Value"/> stores an array without copying it.
    /// </remarks>
    public NbtLongArray(string? tagName, long[] value)
    {
        if (value == null) throw new ArgumentNullException(nameof(value));
        Name = tagName;
        _longs = (long[])value.Clone();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NbtLongArray"/> class that is a deep copy of the specified tag.
    /// </summary>
    /// <param name="other">The tag to copy. This value cannot be <see langword="null"/>.</param>
    /// <exception cref="ArgumentNullException"><paramref name="other"/> is <see langword="null"/>.</exception>
    /// <remarks>
    /// The array of the source tag is cloned.
    /// </remarks>
    public NbtLongArray(NbtLongArray other)
    {
        if (other == null) throw new ArgumentNullException(nameof(other));

        Name = other.Name;
        _longs = (long[])other._longs.Clone();
    }

    /// <summary>
    /// Gets the type of this tag, which is always <see cref="NbtTagType.LongArray"/>.
    /// </summary>
    public override NbtTagType TagType => NbtTagType.LongArray;

    /// <summary>
    /// Gets or sets the array held by this tag.
    /// </summary>
    /// <exception cref="ArgumentNullException">The property is set to <see langword="null"/>.</exception>
    /// <remarks>
    /// The array is stored by reference and is not cloned.
    /// </remarks>
    public long[] Value
    {
        get => _longs;
        set => _longs = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <summary>
    /// Gets or sets the 64-bit integer at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get or set.</param>
    /// <returns>The 64-bit integer at the specified index.</returns>
    /// <exception cref="IndexOutOfRangeException"><paramref name="index"/> is outside the bounds of the
    /// array.</exception>
    public new long this[int index]
    {
        get => Value[index];
        set => Value[index] = value;
    }

    internal override void ReadTag(NbtBinaryReader readStream)
    {
        Value = readStream.ReadArrayBigEndian<long>(readStream.ReadInt32());
    }

    internal override void WriteTag(NbtBinaryWriter writeStream)
    {
        writeStream.Write(NbtTagType.LongArray);

        if (Name == null) throw new NbtFormatException("Name is null");

        writeStream.Write(Name);
        WriteData(writeStream);
    }

    internal override void WriteData(NbtBinaryWriter writeStream)
    {
        writeStream.Write(Value.Length);
        writeStream.WriteBigEndian(Value);
    }

    /// <inheritdoc />
    public override object Clone()
    {
        return new NbtLongArray(this);
    }

    internal override void PrettyPrint(StringBuilder sb, string indentString, int indentLevel)
    {
        for (var i = 0; i < indentLevel; i++) sb.Append(indentString);

        sb.Append("TAG_Long_Array");

        if (!string.IsNullOrEmpty(Name)) sb.AppendFormat("(\"{0}\")", Name);

        sb.AppendFormat(": [{0} longs]", Value.Length);
    }
}