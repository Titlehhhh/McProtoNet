using System.Text;

namespace McProtoNet.NBT;

/// <summary>
/// Represents an NBT tag that holds a single signed 32-bit integer.
/// </summary>
public sealed class NbtInt : NbtTag
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NbtInt"/> class that is unnamed and has a value of 0.
    /// </summary>
    public NbtInt()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NbtInt"/> class that is unnamed and has the specified value.
    /// </summary>
    /// <param name="value">The value of the tag.</param>
    public NbtInt(int value)
        : this(null!, value)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NbtInt"/> class with the specified name and a value of 0.
    /// </summary>
    /// <param name="tagName">The name of the tag, or <see langword="null"/> for an unnamed tag.</param>
    public NbtInt(string? tagName)
        : this(tagName!, 0)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NbtInt"/> class with the specified name and value.
    /// </summary>
    /// <param name="tagName">The name of the tag, or <see langword="null"/> for an unnamed tag.</param>
    /// <param name="value">The value of the tag.</param>
    public NbtInt(string? tagName, int value)
    {
        Name = tagName;
        Value = value;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NbtInt"/> class that is a copy of the specified tag.
    /// </summary>
    /// <param name="other">The tag to copy. The name and the value are copied.</param>
    /// <exception cref="ArgumentNullException"><paramref name="other"/> is <see langword="null"/>.</exception>
    public NbtInt(NbtInt other)
    {
        if (other == null) throw new ArgumentNullException(nameof(other));
        Name = other.Name;
        Value = other.Value;
    }

    /// <summary>
    /// Gets the type of this tag, which is always <see cref="NbtTagType.Int"/>.
    /// </summary>
    public override NbtTagType TagType => NbtTagType.Int;

    /// <summary>
    /// Gets or sets the value of this tag.
    /// </summary>
    public int Value { get; set; }

    internal override void ReadTag(NbtBinaryReader readStream)
    {
        Value = readStream.ReadInt32();
    }

    internal override void WriteTag(NbtBinaryWriter writeStream)
    {
        writeStream.Write(NbtTagType.Int);
        if (Name == null) throw new NbtFormatException("Name is null");
        writeStream.Write(Name);
        writeStream.Write(Value);
    }

    internal override void WriteData(NbtBinaryWriter writeStream)
    {
        writeStream.Write(Value);
    }

    /// <inheritdoc />
    public override object Clone()
    {
        return new NbtInt(this);
    }

    internal override void PrettyPrint(StringBuilder sb, string indentString, int indentLevel)
    {
        for (var i = 0; i < indentLevel; i++) sb.Append(indentString);
        sb.Append("TAG_Int");
        if (!string.IsNullOrEmpty(Name)) sb.AppendFormat("(\"{0}\")", Name);
        sb.Append(": ");
        sb.Append(Value);
    }
}