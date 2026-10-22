using System.Text;

namespace McProtoNet.NBT;

/// <summary>
/// Represents an NBT tag that holds an array of bytes.
/// </summary>
public sealed class NbtByteArray : NbtTag
{
    private byte[] _bytes;

    /// <summary>
    /// Initializes a new instance of the <see cref="NbtByteArray"/> class that is unnamed and holds an empty array.
    /// </summary>
    public NbtByteArray()
        : this((string)null!)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NbtByteArray"/> class that is unnamed and holds a copy of the
    /// specified array.
    /// </summary>
    /// <param name="value">The array to copy into the tag. This value cannot be <see langword="null"/>.</param>
    /// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
    /// <remarks>
    /// The array is cloned. Setting <see cref="Value"/> stores an array without copying it.
    /// </remarks>
    public NbtByteArray(byte[] value)
        : this(null!, value)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NbtByteArray"/> class with the specified name that holds an
    /// empty array.
    /// </summary>
    /// <param name="tagName">The name of the tag, or <see langword="null"/> for an unnamed tag.</param>
    public NbtByteArray(string? tagName)
    {
        Name = tagName;
        _bytes = Array.Empty<byte>();
    }

    internal static NbtByteArray CreateFromArray(byte[] value, string? tagName) 
    {
        NbtByteArray result = new NbtByteArray();
        result._bytes = value;
        result.Name = tagName;
        return result;
    }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="NbtByteArray"/> class with the specified name that holds a copy
    /// of the specified array.
    /// </summary>
    /// <param name="tagName">The name of the tag, or <see langword="null"/> for an unnamed tag.</param>
    /// <param name="value">The array to copy into the tag. This value cannot be <see langword="null"/>.</param>
    /// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
    /// <remarks>
    /// The array is cloned. Setting <see cref="Value"/> stores an array without copying it.
    /// </remarks>
    public NbtByteArray(string? tagName, byte[] value)
    {
        if (value == null) throw new ArgumentNullException(nameof(value));
        Name = tagName;
        _bytes = (byte[])value.Clone();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NbtByteArray"/> class that is a deep copy of the specified tag.
    /// </summary>
    /// <param name="other">The tag to copy. This value cannot be <see langword="null"/>.</param>
    /// <exception cref="ArgumentNullException"><paramref name="other"/> is <see langword="null"/>.</exception>
    /// <remarks>
    /// The array of the source tag is cloned.
    /// </remarks>
    public NbtByteArray(NbtByteArray other)
    {
        if (other == null) throw new ArgumentNullException(nameof(other));
        Name = other.Name;
        _bytes = (byte[])other.Value.Clone();
    }

    /// <summary>
    /// Gets the type of this tag, which is always <see cref="NbtTagType.ByteArray"/>.
    /// </summary>
    public override NbtTagType TagType => NbtTagType.ByteArray;

    /// <summary>
    /// Gets or sets the array held by this tag.
    /// </summary>
    /// <exception cref="ArgumentNullException">The property is set to <see langword="null"/>.</exception>
    /// <remarks>
    /// The array is stored by reference and is not cloned.
    /// </remarks>
    public byte[] Value
    {
        get => _bytes;
        set => _bytes = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <summary>
    /// Gets or sets the byte at the specified index.
    /// </summary>
    /// <param name="tagIndex">The zero-based index of the element to get or set.</param>
    /// <returns>The byte at the specified index.</returns>
    /// <exception cref="IndexOutOfRangeException"><paramref name="tagIndex"/> is outside the bounds of the
    /// array.</exception>
    public new byte this[int tagIndex]
    {
        get => Value[tagIndex];
        set => Value[tagIndex] = value;
    }

    internal override void ReadTag(NbtBinaryReader readStream)
    {
        Value = readStream.ReadArrayBigEndian<byte>(readStream.ReadInt32());
    }

    internal override void WriteTag(NbtBinaryWriter writeStream)
    {
        writeStream.Write(NbtTagType.ByteArray);
        if (Name == null) throw new NbtFormatException("Name is null");
        writeStream.Write(Name);
        WriteData(writeStream);
    }

    internal override void WriteData(NbtBinaryWriter writeStream)
    {
        writeStream.Write(Value.Length);
        writeStream.Write(Value, 0, Value.Length);
    }

    /// <inheritdoc />
    public override object Clone()
    {
        return new NbtByteArray(this);
    }

    internal override void PrettyPrint(StringBuilder sb, string indentString, int indentLevel)
    {
        for (var i = 0; i < indentLevel; i++) sb.Append(indentString);
        sb.Append("TAG_Byte_Array");
        if (!string.IsNullOrEmpty(Name)) sb.AppendFormat("(\"{0}\")", Name);
        sb.AppendFormat(": [{0} bytes]", _bytes.Length);
    }
}