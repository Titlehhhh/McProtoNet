using System.Text;

namespace McProtoNet.NBT;

/// <summary>
/// Represents an NBT tag that holds a single byte.
/// </summary>
public class NbtByte : NbtTag
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NbtByte"/> class that is unnamed and has a value of 0.
    /// </summary>
    public NbtByte()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NbtByte"/> class that is unnamed and has the specified value.
    /// </summary>
    /// <param name="value">The value of the tag.</param>
    public NbtByte(byte value)
        : this(null!, value)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NbtByte"/> class with the specified name and a value of 0.
    /// </summary>
    /// <param name="tagName">The name of the tag, or <see langword="null"/> for an unnamed tag.</param>
    public NbtByte(string? tagName)
        : this(tagName, 0)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NbtByte"/> class with the specified name and value.
    /// </summary>
    /// <param name="tagName">The name of the tag, or <see langword="null"/> for an unnamed tag.</param>
    /// <param name="value">The value of the tag.</param>
    public NbtByte(string? tagName, byte value)
    {
        Name = tagName;
        Value = value;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NbtByte"/> class that is a copy of the specified tag.
    /// </summary>
    /// <param name="other">The tag to copy. The name and the value are copied.</param>
    /// <exception cref="ArgumentNullException"><paramref name="other"/> is <see langword="null"/>.</exception>
    public NbtByte(NbtByte other)
    {
        if (other == null) throw new ArgumentNullException(nameof(other));
        Name = other.Name;
        Value = other.Value;
    }

    /// <summary>
    /// Gets the type of this tag, which is always <see cref="NbtTagType.Byte"/>.
    /// </summary>
    public override NbtTagType TagType => NbtTagType.Byte;

    /// <summary>
    /// Gets or sets the value of this tag.
    /// </summary>
    public byte Value { get; set; }

    internal override void ReadTag(NbtBinaryReader readStream)
    {
        Value = readStream.ReadByte();
    }

    internal override void WriteTag(NbtBinaryWriter writeStream)
    {
        writeStream.Write(NbtTagType.Byte);
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
        return new NbtByte(this);
    }

    internal override void PrettyPrint(StringBuilder sb, string indentString, int indentLevel)
    {
        for (var i = 0; i < indentLevel; i++) sb.Append(indentString);
        sb.Append("TAG_Byte");
        if (!string.IsNullOrEmpty(Name)) sb.AppendFormat("(\"{0}\")", Name);
        sb.Append(": ");
        sb.Append(Value);
    }
}