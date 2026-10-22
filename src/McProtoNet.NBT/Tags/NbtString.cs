using System.Text;

namespace McProtoNet.NBT;

/// <summary>
/// Represents an NBT tag that holds a single string.
/// </summary>
/// <remarks>
/// The string is encoded on the wire as modified UTF-8. See <see cref="ModifiedUtf8"/>.
/// </remarks>
public sealed class NbtString : NbtTag
{
    private string _stringVal = "";

    /// <summary>
    /// Initializes a new instance of the <see cref="NbtString"/> class that is unnamed and has an empty value.
    /// </summary>
    public NbtString()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NbtString"/> class that is unnamed and has the specified value.
    /// </summary>
    /// <param name="value">The value of the tag. This value cannot be <see langword="null"/>.</param>
    /// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
    public NbtString(string value)
        : this(null!, value)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NbtString"/> class with the specified name and value.
    /// </summary>
    /// <param name="tagName">The name of the tag, or <see langword="null"/> for an unnamed tag.</param>
    /// <param name="value">The value of the tag. This value cannot be <see langword="null"/>.</param>
    /// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
    public NbtString(string? tagName, string value)
    {
        Name = tagName;
        Value = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NbtString"/> class that is a copy of the specified tag.
    /// </summary>
    /// <param name="other">The tag to copy. The name and the value are copied.</param>
    /// <exception cref="ArgumentNullException"><paramref name="other"/> is <see langword="null"/>.</exception>
    public NbtString(NbtString other)
    {
        if (other == null) throw new ArgumentNullException(nameof(other));
        Name = other.Name;
        Value = other.Value;
    }

    /// <summary>
    /// Gets the type of this tag, which is always <see cref="NbtTagType.String"/>.
    /// </summary>
    public override NbtTagType TagType => NbtTagType.String;

    /// <summary>
    /// Gets or sets the value of this tag. The default is an empty string.
    /// </summary>
    /// <exception cref="ArgumentNullException">The property is set to <see langword="null"/>.</exception>
    public string Value
    {
        get => _stringVal;
        set => _stringVal = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <inheritdoc />
    public override object Clone()
    {
        return new NbtString(this);
    }

    internal override void PrettyPrint(StringBuilder sb, string indentString, int indentLevel)
    {
        for (var i = 0; i < indentLevel; i++) sb.Append(indentString);
        sb.Append("TAG_String");
        if (!string.IsNullOrEmpty(Name)) sb.AppendFormat("(\"{0}\")", Name);
        sb.Append(": \"");
        sb.Append(Value);
        sb.Append('"');
    }

    #region Reading / Writing

    internal override void ReadTag(NbtBinaryReader readStream)
    {
        Value = readStream.ReadString();
    }

    internal override void WriteTag(NbtBinaryWriter writeStream)
    {
        writeStream.Write(NbtTagType.String);
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