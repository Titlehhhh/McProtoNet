using System.Text;

namespace McProtoNet.NBT;

/// <summary>
/// Represents an NBT tag that holds an array of signed 32-bit integers.
/// </summary>
public sealed class NbtIntArray : NbtTag
{
    private int[] _ints;

    /// <summary>
    /// Initializes a new instance of the <see cref="NbtIntArray"/> class that is unnamed and holds an empty array.
    /// </summary>
    public NbtIntArray()
        : this((string)null!)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NbtIntArray"/> class that is unnamed and holds a copy of the
    /// specified array.
    /// </summary>
    /// <param name="value">The array to copy into the tag. This value cannot be <see langword="null"/>.</param>
    /// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
    /// <remarks>
    /// The array is cloned. Setting <see cref="Value"/> stores an array without copying it.
    /// </remarks>
    public NbtIntArray(int[] value)
        : this(null!, value)
    {
    }

    internal static NbtIntArray CreateFromArray(int[] value, string? tagName)
    {
        NbtIntArray result = new NbtIntArray();
        result.Name = tagName;
        result._ints = value;
        return result;
    } 

    /// <summary>
    /// Initializes a new instance of the <see cref="NbtIntArray"/> class with the specified name that holds an
    /// empty array.
    /// </summary>
    /// <param name="tagName">The name of the tag, or <see langword="null"/> for an unnamed tag.</param>
    public NbtIntArray(string? tagName)
    {
        Name = tagName;
        _ints = Array.Empty<int>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NbtIntArray"/> class with the specified name that holds a copy
    /// of the specified array.
    /// </summary>
    /// <param name="tagName">The name of the tag, or <see langword="null"/> for an unnamed tag.</param>
    /// <param name="value">The array to copy into the tag. This value cannot be <see langword="null"/>.</param>
    /// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
    /// <remarks>
    /// The array is cloned. Setting <see cref="Value"/> stores an array without copying it.
    /// </remarks>
    public NbtIntArray(string? tagName, int[] value)
    {
        if (value == null) throw new ArgumentNullException(nameof(value));
        Name = tagName;
        _ints = (int[])value.Clone();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NbtIntArray"/> class that is a deep copy of the specified tag.
    /// </summary>
    /// <param name="other">The tag to copy. This value cannot be <see langword="null"/>.</param>
    /// <exception cref="ArgumentNullException"><paramref name="other"/> is <see langword="null"/>.</exception>
    /// <remarks>
    /// The array of the source tag is cloned.
    /// </remarks>
    public NbtIntArray(NbtIntArray other)
    {
        if (other == null) throw new ArgumentNullException(nameof(other));
        Name = other.Name;
        _ints = (int[])other.Value.Clone();
    }

    /// <summary>
    /// Gets the type of this tag, which is always <see cref="NbtTagType.IntArray"/>.
    /// </summary>
    public override NbtTagType TagType => NbtTagType.IntArray;

    /// <summary>
    /// Gets or sets the array held by this tag.
    /// </summary>
    /// <exception cref="ArgumentNullException">The property is set to <see langword="null"/>.</exception>
    /// <remarks>
    /// The array is stored by reference and is not cloned.
    /// </remarks>
    public int[] Value
    {
        get => _ints;
        set => _ints = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <summary>
    /// Gets or sets the integer at the specified index.
    /// </summary>
    /// <param name="tagIndex">The zero-based index of the element to get or set.</param>
    /// <returns>The integer at the specified index.</returns>
    /// <exception cref="IndexOutOfRangeException"><paramref name="tagIndex"/> is outside the bounds of the
    /// array.</exception>
    public new int this[int tagIndex]
    {
        get => Value[tagIndex];
        set => Value[tagIndex] = value;
    }

    internal override void ReadTag(NbtBinaryReader readStream)
    {
        Value = readStream.ReadArrayBigEndian<int>(readStream.ReadInt32());
    }

    internal override void WriteTag(NbtBinaryWriter writeStream)
    {
        writeStream.Write(NbtTagType.IntArray);
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
        return new NbtIntArray(this);
    }

    internal override void PrettyPrint(StringBuilder sb, string indentString, int indentLevel)
    {
        for (var i = 0; i < indentLevel; i++) sb.Append(indentString);
        sb.Append("TAG_Int_Array");
        if (!string.IsNullOrEmpty(Name)) sb.AppendFormat("(\"{0}\")", Name);
        sb.AppendFormat(": [{0} ints]", _ints.Length);
    }
}