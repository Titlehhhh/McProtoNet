using System.Text;

namespace McProtoNet.NBT;

/// <summary>
/// Represents an NBT tag that holds a single signed 64-bit integer.
/// </summary>
public sealed class NbtLong : NbtTag
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NbtLong"/> class that is unnamed and has a value of 0.
    /// </summary>
    public NbtLong()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NbtLong"/> class that is unnamed and has the specified value.
    /// </summary>
    /// <param name="value">The value of the tag.</param>
    public NbtLong(long value)
        : this(null!, value)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NbtLong"/> class with the specified name and a value of 0.
    /// </summary>
    /// <param name="tagName">The name of the tag, or <see langword="null"/> for an unnamed tag.</param>
    public NbtLong(string? tagName)
        : this(tagName, 0)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NbtLong"/> class with the specified name and value.
    /// </summary>
    /// <param name="tagName">The name of the tag, or <see langword="null"/> for an unnamed tag.</param>
    /// <param name="value">The value of the tag.</param>
    public NbtLong(string? tagName, long value)
    {
        Name = tagName;
        Value = value;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NbtLong"/> class that is a copy of the specified tag.
    /// </summary>
    /// <param name="other">The tag to copy. The name and the value are copied.</param>
    /// <exception cref="ArgumentNullException"><paramref name="other"/> is <see langword="null"/>.</exception>
    public NbtLong(NbtLong other)
    {
        if (other == null) throw new ArgumentNullException(nameof(other));
        Name = other.Name;
        Value = other.Value;
    }

    /// <summary>
    /// Gets the type of this tag, which is always <see cref="NbtTagType.Long"/>.
    /// </summary>
    public override NbtTagType TagType => NbtTagType.Long;

    /// <summary>
    /// Gets or sets the value of this tag.
    /// </summary>
    public long Value { get; set; }

    /// <inheritdoc />
    public override object Clone()
    {
        return new NbtLong(this);
    }

    internal override void PrettyPrint(StringBuilder sb, string indentString, int indentLevel)
    {
        for (var i = 0; i < indentLevel; i++) sb.Append(indentString);
        sb.Append("TAG_Long");
        if (!string.IsNullOrEmpty(Name)) sb.AppendFormat("(\"{0}\")", Name);
        sb.Append(": ");
        sb.Append(Value);
    }

    #region Reading / Writing

    internal override void ReadTag(NbtBinaryReader readStream)
    {
        Value = readStream.ReadInt64();
    }

    internal override void WriteTag(NbtBinaryWriter writeStream)
    {
        writeStream.Write(NbtTagType.Long);
        if (Name == null) throw new NbtFormatException("Name is null");
        writeStream.Write(Name);
        writeStream.Write(Value);
    }

    internal override void WriteData(NbtBinaryWriter writeStream)
    {
        writeStream.Write(Value);
    }

    #endregion
}