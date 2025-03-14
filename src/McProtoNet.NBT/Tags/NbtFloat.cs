﻿using System.Text;

namespace McProtoNet.NBT;

/// <summary>
///     A tag containing a single-precision floating point number.
/// </summary>
public sealed class NbtFloat : NbtTag
{
    /// <summary>
    ///     Creates an unnamed NbtFloat tag with the default value of 0f.
    /// </summary>
    public NbtFloat()
    {
    }

    /// <summary>
    ///     Creates an unnamed NbtFloat tag with the given value.
    /// </summary>
    /// <param name="value"> Value to assign to this tag. </param>
    public NbtFloat(float value)
        : this(null!, value)
    {
    }

    /// <summary>
    ///     Creates an NbtFloat tag with the given name and the default value of 0f.
    /// </summary>
    /// <param name="tagName"> Name to assign to this tag. May be <c>null</c>. </param>
    public NbtFloat(string? tagName)
        : this(tagName!, 0)
    {
    }

    /// <summary>
    ///     Creates an NbtFloat tag with the given name and value.
    /// </summary>
    /// <param name="tagName"> Name to assign to this tag. May be <c>null</c>. </param>
    /// <param name="value"> Value to assign to this tag. </param>
    public NbtFloat(string? tagName, float value)
    {
        Name = tagName;
        Value = value;
    }

    /// <summary>
    ///     Creates a copy of given NbtFloat tag.
    /// </summary>
    /// <param name="other"> Tag to copy. May not be <c>null</c>. </param>
    /// <exception cref="ArgumentNullException"> <paramref name="other" /> is <c>null</c>. </exception>
    public NbtFloat(NbtFloat other)
    {
        if (other == null) throw new ArgumentNullException(nameof(other));
        Name = other.Name;
        Value = other.Value;
    }

    /// <summary>
    ///     Type of this tag (Float).
    /// </summary>
    public override NbtTagType TagType => NbtTagType.Float;

    /// <summary>
    ///     Value/payload of this tag (a single-precision floating point number).
    /// </summary>
    public float Value { get; set; }

    internal override bool ReadTag(NbtBinaryReader readStream)
    {
        if (readStream.Selector != null && !readStream.Selector(this))
        {
            readStream.ReadSingle();
            return false;
        }

        Value = readStream.ReadSingle();
        return true;
    }

    internal override void SkipTag(NbtBinaryReader readStream)
    {
        readStream.ReadSingle();
    }

    internal override void WriteTag(NbtBinaryWriter writeStream)
    {
        writeStream.Write(NbtTagType.Float);
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
        return new NbtFloat(this);
    }

    internal override void PrettyPrint(StringBuilder sb, string indentString, int indentLevel)
    {
        for (var i = 0; i < indentLevel; i++) sb.Append(indentString);
        sb.Append("TAG_Float");
        if (!string.IsNullOrEmpty(Name)) sb.AppendFormat("(\"{0}\")", Name);
        sb.Append(": ");
        sb.Append(Value);
    }
}