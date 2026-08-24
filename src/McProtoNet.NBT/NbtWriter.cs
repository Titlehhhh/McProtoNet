using System.Diagnostics;

namespace McProtoNet.NBT;

/// <summary>
/// Provides a forward-only writer that writes NBT data to a stream.
/// </summary>
/// <remarks>
/// Each instance writes one complete document. The writer enforces the constraints of the NBT format, except
/// that it does not check for duplicate tag names within a compound. The output is Java Edition NBT only:
/// every number is big-endian and every string is modified UTF-8. The writer does not close the underlying
/// stream.
/// </remarks>
public sealed class NbtWriter
{
    private const int MaxStreamCopyBufferSize = 8 * 1024;

    private readonly NbtBinaryWriter _writer;
    private int _listIndex;
    private int _listSize;
    private NbtTagType _listType;
    private Stack<NbtWriterNode>? _nodes;
    private NbtTagType _parentType;


    /// <summary>
    /// Initializes a new instance of the <see cref="NbtWriter"/> class that starts a named root compound,
    /// which is the NBT file format.
    /// </summary>
    /// <param name="stream">The stream to write to.</param>
    /// <param name="rootTagName">The name of the root tag. It is written immediately.</param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="stream"/> is <see langword="null"/>.
    /// -or-
    /// <paramref name="rootTagName"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException"><paramref name="stream"/> is not writable.</exception>
    /// <exception cref="NbtFormatException">The encoded <paramref name="rootTagName"/> is longer than
    /// 65535 bytes.</exception>
    public NbtWriter(Stream stream, string rootTagName)
    {
        ArgumentNullException.ThrowIfNull(rootTagName);
        _writer = new NbtBinaryWriter(stream);
        _writer.Write((byte)NbtTagType.Compound);
        _writer.Write(rootTagName);
        _parentType = NbtTagType.Compound;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NbtWriter"/> class that starts a nameless root compound,
    /// which is the network format used since 1.20.2.
    /// </summary>
    /// <param name="stream">The stream to write to.</param>
    /// <exception cref="ArgumentNullException"><paramref name="stream"/> is
    /// <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="stream"/> is not writable.</exception>
    public NbtWriter(Stream stream)
    {
        _writer = new NbtBinaryWriter(stream);
        _writer.Write((byte)NbtTagType.Compound);
        _parentType = NbtTagType.Compound;
    }

    /// <summary>
    /// Writes one complete tag to the specified stream: the type byte, the root name when requested, then
    /// the payload.
    /// </summary>
    /// <param name="stream">The stream to write to.</param>
    /// <param name="tag">The tag to write.</param>
    /// <param name="writeRootName"><see langword="true"/> to write the root tag's name after the type byte,
    /// as the file format requires; <see langword="false"/> to write the nameless root of the network
    /// format. The default is <see langword="true"/>.</param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="stream"/> is <see langword="null"/>.
    /// -or-
    /// <paramref name="tag"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException"><paramref name="stream"/> is not writable.</exception>
    /// <remarks>
    /// The root tag can be of any type. A network root can be a TAG_String since 1.20.3.
    /// </remarks>
    public static void WriteTag(Stream stream, NbtTag tag, bool writeRootName = true)
    {
        ArgumentNullException.ThrowIfNull(stream);
        ArgumentNullException.ThrowIfNull(tag);

        var writer = new NbtBinaryWriter(stream);
        writer.Write((byte)tag.TagType);
        if (writeRootName) writer.Write(tag.Name ?? string.Empty);
        tag.WriteData(writer);
    }

    /// <summary>
    /// Gets a value indicating whether the root tag has been closed.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if the root tag has been closed and no more tags can be written; otherwise,
    /// <see langword="false"/>.
    /// </value>
    public bool IsDone { get; private set; }

    /// <summary>
    /// Gets the stream that the writer writes to.
    /// </summary>
    /// <remarks>
    /// Buffered data is flushed before the stream is returned.
    /// </remarks>
    public Stream BaseStream => _writer.BaseStream;

    /// <summary>
    /// Writes the specified tag and all of its child tags.
    /// </summary>
    /// <param name="tag">The tag to write. This value cannot be <see langword="null"/>.</param>
    /// <exception cref="ArgumentNullException"><paramref name="tag"/> is <see langword="null"/>.</exception>
    /// <exception cref="NbtFormatException">
    /// No more tags can be written.
    /// -or-
    /// The tag is not acceptable in the current position.
    /// </exception>
    /// <remarks>
    /// <see cref="WriteTag(Stream,NbtTag,bool)"/> writes a complete tag tree in one call.
    /// </remarks>
    public void WriteTag(NbtTag tag)
    {
        if (tag == null) throw new ArgumentNullException(nameof(tag));
        EnforceConstraints(tag.Name, tag.TagType);
        if (tag.Name != null)
            tag.WriteTag(_writer);
        else
            tag.WriteData(_writer);
    }

    /// <summary>
    /// Closes the root compound if it is still open and marks the writer as done.
    /// </summary>
    /// <exception cref="NbtFormatException">A nested tag is still open.</exception>
    /// <remarks>
    /// The TAG_End byte of the root compound is written. Calling this method after the root compound was
    /// closed by <see cref="EndCompound"/> does nothing.
    /// </remarks>
    public void Finish()
    {
        if (IsDone) return;
        if (_nodes is { Count: > 0 })
            throw new NbtFormatException("Cannot finish: not all tags have been closed yet.");

        // Only the root compound, opened by the constructor, remains open; close it.
        _writer.Write(NbtTagType.End);
        IsDone = true;
    }

    private void GoDown(NbtTagType thisType)
    {
        if (_nodes == null) _nodes = new Stack<NbtWriterNode>();
        var newNode = new NbtWriterNode
        {
            ParentType = _parentType,
            ListType = _listType,
            ListSize = _listSize,
            ListIndex = _listIndex
        };
        _nodes.Push(newNode);

        _parentType = thisType;
        _listType = NbtTagType.End;
        _listSize = 0;
        _listIndex = 0;
    }

    private void GoUp()
    {
        if (_nodes == null || _nodes.Count == 0)
        {
            IsDone = true;
        }
        else
        {
            var oldNode = _nodes.Pop();
            _parentType = oldNode.ParentType;
            _listType = oldNode.ListType;
            _listSize = oldNode.ListSize;
            _listIndex = oldNode.ListIndex;
        }
    }

    private void EnforceConstraints(string? name, NbtTagType desiredType)
    {
        if (IsDone) throw new NbtFormatException("Cannot write any more tags: root tag has been closed.");
        if (_parentType == NbtTagType.List)
        {
            if (name != null) throw new NbtFormatException("Expecting an unnamed tag.");

            if (_listType != desiredType)
                throw new NbtFormatException("Unexpected tag type (expected: " + _listType + ", given: " +
                                             desiredType);
            if (_listIndex >= _listSize) throw new NbtFormatException("Given list size exceeded.");

            _listIndex++;
        }
        else if (name == null)
        {
            throw new NbtFormatException("Expecting a named tag.");
        }
    }

    // An element type of TAG_End is legal only for an empty list, which is how vanilla writes one.
    private static void CheckListType(NbtTagType elementType, int size)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(size);
        if (elementType > NbtTagType.LongArray || (elementType == NbtTagType.End && size > 0))
            throw new ArgumentOutOfRangeException(nameof(elementType));
    }

    private static void CheckArray(Array data, int offset, int count)
    {
        if (data == null) throw new ArgumentNullException(nameof(data));

        if (offset < 0) throw new ArgumentOutOfRangeException(nameof(offset), "offset may not be negative.");

        if (count < 0) throw new ArgumentOutOfRangeException(nameof(count), "count may not be negative.");

        if (data.Length - offset < count)
            throw new ArgumentException("count may not be greater than offset subtracted from the array length.");
    }

    private void WriteByteArrayFromStreamImpl(Stream dataSource, int count, byte[] buffer)
    {
        Debug.Assert(dataSource != null);
        Debug.Assert(buffer != null);
        _writer.Write(count);
        var maxBytesToWrite = Math.Min(buffer.Length, NbtBinaryWriter.MaxWriteChunk);
        var bytesWritten = 0;
        while (bytesWritten < count)
        {
            var bytesToRead = Math.Min(count - bytesWritten, maxBytesToWrite);
            var bytesRead = dataSource.Read(buffer, 0, bytesToRead);
            if (bytesRead == 0) throw new EndOfStreamException();
            _writer.Write(buffer, 0, bytesRead);
            bytesWritten += bytesRead;
        }
    }

    #region Compounds and Lists

    /// <summary>
    /// Begins an unnamed compound tag.
    /// </summary>
    /// <exception cref="NbtFormatException">
    /// No more tags can be written.
    /// -or-
    /// A named compound tag was expected.
    /// -or-
    /// A tag of a different type was expected.
    /// -or-
    /// The size of the parent list has been exceeded.
    /// </exception>
    public void BeginCompound()
    {
        EnforceConstraints(null!, NbtTagType.Compound);
        GoDown(NbtTagType.Compound);
    }

    /// <summary>
    /// Begins a named compound tag.
    /// </summary>
    /// <param name="tagName">The name of the compound tag. This value cannot be
    /// <see langword="null"/>.</param>
    /// <exception cref="NbtFormatException">
    /// No more tags can be written.
    /// -or-
    /// An unnamed compound tag was expected.
    /// -or-
    /// A tag of a different type was expected.
    /// -or-
    /// The encoded tag name is longer than 65535 bytes.
    /// </exception>
    public void BeginCompound(string tagName)
    {
        EnforceConstraints(tagName, NbtTagType.Compound);
        GoDown(NbtTagType.Compound);

        _writer.Write((byte)NbtTagType.Compound);
        _writer.Write(tagName);
    }

    /// <summary>
    /// Ends the current compound tag.
    /// </summary>
    /// <exception cref="NbtFormatException">The writer is not currently in a compound.</exception>
    public void EndCompound()
    {
        if (IsDone || _parentType != NbtTagType.Compound) throw new NbtFormatException("Not currently in a compound.");
        GoUp();
        _writer.Write(NbtTagType.End);
    }

    /// <summary>
    /// Begins an unnamed list tag.
    /// </summary>
    /// <param name="elementType">The type of the list elements.</param>
    /// <param name="size">The number of elements in the list. This value cannot be negative.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="size"/> is less than zero.
    /// -or-
    /// <paramref name="elementType"/> is not a valid <see cref="NbtTagType"/> value.
    /// -or-
    /// <paramref name="elementType"/> is <see cref="NbtTagType.End"/> and <paramref name="size"/> is greater
    /// than zero.
    /// </exception>
    /// <exception cref="NbtFormatException">
    /// No more tags can be written.
    /// -or-
    /// A named list tag was expected.
    /// -or-
    /// A tag of a different type was expected.
    /// -or-
    /// The size of the parent list has been exceeded.
    /// </exception>
    public void BeginList(NbtTagType elementType, int size)
    {
        CheckListType(elementType, size);
        EnforceConstraints(null!, NbtTagType.List);
        GoDown(NbtTagType.List);
        _listType = elementType;
        _listSize = size;

        _writer.Write((byte)elementType);
        _writer.Write(size);
    }

    /// <summary>
    /// Begins a named list tag.
    /// </summary>
    /// <param name="tagName">The name of the list tag. This value cannot be <see langword="null"/>.</param>
    /// <param name="elementType">The type of the list elements.</param>
    /// <param name="size">The number of elements in the list. This value cannot be negative.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="size"/> is less than zero.
    /// -or-
    /// <paramref name="elementType"/> is not a valid <see cref="NbtTagType"/> value.
    /// -or-
    /// <paramref name="elementType"/> is <see cref="NbtTagType.End"/> and <paramref name="size"/> is greater
    /// than zero.
    /// </exception>
    /// <exception cref="NbtFormatException">
    /// No more tags can be written.
    /// -or-
    /// An unnamed list tag was expected.
    /// -or-
    /// A tag of a different type was expected.
    /// -or-
    /// The encoded tag name is longer than 65535 bytes.
    /// </exception>
    public void BeginList(string tagName, NbtTagType elementType, int size)
    {
        CheckListType(elementType, size);
        EnforceConstraints(tagName, NbtTagType.List);
        GoDown(NbtTagType.List);
        _listType = elementType;
        _listSize = size;

        _writer.Write((byte)NbtTagType.List);
        _writer.Write(tagName);
        _writer.Write((byte)elementType);
        _writer.Write(size);
    }

    /// <summary>
    /// Ends the current list tag.
    /// </summary>
    /// <exception cref="NbtFormatException">
    /// The writer is not currently in a list.
    /// -or-
    /// Not all list elements have been written.
    /// </exception>
    public void EndList()
    {
        if (_parentType != NbtTagType.List || IsDone) throw new NbtFormatException("Not currently in a list.");

        if (_listIndex < _listSize)
            throw new NbtFormatException("Cannot end list: not all list elements have been written yet. " +
                                         "Expected: " + _listSize + ", written: " + _listIndex);
        GoUp();
    }

    #endregion

    #region Value Tags

    /// <summary>
    /// Writes an unnamed byte tag.
    /// </summary>
    /// <param name="value">The unsigned byte to write.</param>
    /// <exception cref="NbtFormatException">
    /// No more tags can be written.
    /// -or-
    /// A named byte tag was expected.
    /// -or-
    /// A tag of a different type was expected.
    /// -or-
    /// The size of the parent list has been exceeded.
    /// </exception>
    public void WriteByte(byte value)
    {
        EnforceConstraints(null!, NbtTagType.Byte);
        _writer.Write(value);
    }

    /// <summary>
    /// Writes a named byte tag.
    /// </summary>
    /// <param name="tagName">The name of the tag. This value cannot be <see langword="null"/>.</param>
    /// <param name="value">The unsigned byte to write.</param>
    /// <exception cref="NbtFormatException">
    /// No more tags can be written.
    /// -or-
    /// An unnamed byte tag was expected.
    /// -or-
    /// A tag of a different type was expected.
    /// -or-
    /// The encoded tag name is longer than 65535 bytes.
    /// </exception>
    public void WriteByte(string tagName, byte value)
    {
        EnforceConstraints(tagName, NbtTagType.Byte);
        _writer.Write((byte)NbtTagType.Byte);
        _writer.Write(tagName);
        _writer.Write(value);
    }

    /// <summary>
    /// Writes an unnamed double tag.
    /// </summary>
    /// <param name="value">The eight-byte floating-point value to write.</param>
    /// <exception cref="NbtFormatException">
    /// No more tags can be written.
    /// -or-
    /// A named double tag was expected.
    /// -or-
    /// A tag of a different type was expected.
    /// -or-
    /// The size of the parent list has been exceeded.
    /// </exception>
    public void WriteDouble(double value)
    {
        EnforceConstraints(null!, NbtTagType.Double);
        _writer.Write(value);
    }

    /// <summary>
    /// Writes a named double tag.
    /// </summary>
    /// <param name="tagName">The name of the tag. This value cannot be <see langword="null"/>.</param>
    /// <param name="value">The eight-byte floating-point value to write.</param>
    /// <exception cref="NbtFormatException">
    /// No more tags can be written.
    /// -or-
    /// An unnamed double tag was expected.
    /// -or-
    /// A tag of a different type was expected.
    /// -or-
    /// The encoded tag name is longer than 65535 bytes.
    /// </exception>
    public void WriteDouble(string tagName, double value)
    {
        EnforceConstraints(tagName, NbtTagType.Double);
        _writer.Write((byte)NbtTagType.Double);
        _writer.Write(tagName);
        _writer.Write(value);
    }

    /// <summary>
    /// Writes an unnamed float tag.
    /// </summary>
    /// <param name="value">The four-byte floating-point value to write.</param>
    /// <exception cref="NbtFormatException">
    /// No more tags can be written.
    /// -or-
    /// A named float tag was expected.
    /// -or-
    /// A tag of a different type was expected.
    /// -or-
    /// The size of the parent list has been exceeded.
    /// </exception>
    public void WriteFloat(float value)
    {
        EnforceConstraints(null!, NbtTagType.Float);
        _writer.Write(value);
    }

    /// <summary>
    /// Writes a named float tag.
    /// </summary>
    /// <param name="tagName">The name of the tag. This value cannot be <see langword="null"/>.</param>
    /// <param name="value">The four-byte floating-point value to write.</param>
    /// <exception cref="NbtFormatException">
    /// No more tags can be written.
    /// -or-
    /// An unnamed float tag was expected.
    /// -or-
    /// A tag of a different type was expected.
    /// -or-
    /// The encoded tag name is longer than 65535 bytes.
    /// </exception>
    public void WriteFloat(string tagName, float value)
    {
        EnforceConstraints(tagName, NbtTagType.Float);
        _writer.Write((byte)NbtTagType.Float);
        _writer.Write(tagName);
        _writer.Write(value);
    }

    /// <summary>
    /// Writes an unnamed int tag.
    /// </summary>
    /// <param name="value">The four-byte signed integer to write.</param>
    /// <exception cref="NbtFormatException">
    /// No more tags can be written.
    /// -or-
    /// A named int tag was expected.
    /// -or-
    /// A tag of a different type was expected.
    /// -or-
    /// The size of the parent list has been exceeded.
    /// </exception>
    public void WriteInt(int value)
    {
        EnforceConstraints(null!, NbtTagType.Int);
        _writer.Write(value);
    }

    /// <summary>
    /// Writes a named int tag.
    /// </summary>
    /// <param name="tagName">The name of the tag. This value cannot be <see langword="null"/>.</param>
    /// <param name="value">The four-byte signed integer to write.</param>
    /// <exception cref="NbtFormatException">
    /// No more tags can be written.
    /// -or-
    /// An unnamed int tag was expected.
    /// -or-
    /// A tag of a different type was expected.
    /// -or-
    /// The encoded tag name is longer than 65535 bytes.
    /// </exception>
    public void WriteInt(string tagName, int value)
    {
        EnforceConstraints(tagName, NbtTagType.Int);
        _writer.Write((byte)NbtTagType.Int);
        _writer.Write(tagName);
        _writer.Write(value);
    }

    /// <summary>
    /// Writes an unnamed long tag.
    /// </summary>
    /// <param name="value">The eight-byte signed integer to write.</param>
    /// <exception cref="NbtFormatException">
    /// No more tags can be written.
    /// -or-
    /// A named long tag was expected.
    /// -or-
    /// A tag of a different type was expected.
    /// -or-
    /// The size of the parent list has been exceeded.
    /// </exception>
    public void WriteLong(long value)
    {
        EnforceConstraints(null!, NbtTagType.Long);
        _writer.Write(value);
    }

    /// <summary>
    /// Writes a named long tag.
    /// </summary>
    /// <param name="tagName">The name of the tag. This value cannot be <see langword="null"/>.</param>
    /// <param name="value">The eight-byte signed integer to write.</param>
    /// <exception cref="NbtFormatException">
    /// No more tags can be written.
    /// -or-
    /// An unnamed long tag was expected.
    /// -or-
    /// A tag of a different type was expected.
    /// -or-
    /// The encoded tag name is longer than 65535 bytes.
    /// </exception>
    public void WriteLong(string tagName, long value)
    {
        EnforceConstraints(tagName, NbtTagType.Long);
        _writer.Write((byte)NbtTagType.Long);
        _writer.Write(tagName);
        _writer.Write(value);
    }

    /// <summary>
    /// Writes an unnamed short tag.
    /// </summary>
    /// <param name="value">The two-byte signed integer to write.</param>
    /// <exception cref="NbtFormatException">
    /// No more tags can be written.
    /// -or-
    /// A named short tag was expected.
    /// -or-
    /// A tag of a different type was expected.
    /// -or-
    /// The size of the parent list has been exceeded.
    /// </exception>
    public void WriteShort(short value)
    {
        EnforceConstraints(null!, NbtTagType.Short);
        _writer.Write(value);
    }

    /// <summary>
    /// Writes a named short tag.
    /// </summary>
    /// <param name="tagName">The name of the tag. This value cannot be <see langword="null"/>.</param>
    /// <param name="value">The two-byte signed integer to write.</param>
    /// <exception cref="NbtFormatException">
    /// No more tags can be written.
    /// -or-
    /// An unnamed short tag was expected.
    /// -or-
    /// A tag of a different type was expected.
    /// -or-
    /// The encoded tag name is longer than 65535 bytes.
    /// </exception>
    public void WriteShort(string tagName, short value)
    {
        EnforceConstraints(tagName, NbtTagType.Short);
        _writer.Write((byte)NbtTagType.Short);
        _writer.Write(tagName);
        _writer.Write(value);
    }

    /// <summary>
    /// Writes an unnamed string tag.
    /// </summary>
    /// <param name="value">The string to write.</param>
    /// <exception cref="ArgumentNullException"><paramref name="value"/> is
    /// <see langword="null"/>.</exception>
    /// <exception cref="NbtFormatException">
    /// No more tags can be written.
    /// -or-
    /// A named string tag was expected.
    /// -or-
    /// A tag of a different type was expected.
    /// -or-
    /// The size of the parent list has been exceeded.
    /// -or-
    /// The encoded text is longer than 65535 bytes.
    /// </exception>
    public void WriteString(string value)
    {
        if (value == null) throw new ArgumentNullException(nameof(value));
        EnforceConstraints(null!, NbtTagType.String);
        _writer.Write(value);
    }

    /// <summary>
    /// Writes a named string tag.
    /// </summary>
    /// <param name="tagName">The name of the tag. This value cannot be <see langword="null"/>.</param>
    /// <param name="value">The string to write.</param>
    /// <exception cref="ArgumentNullException"><paramref name="value"/> is
    /// <see langword="null"/>.</exception>
    /// <exception cref="NbtFormatException">
    /// No more tags can be written.
    /// -or-
    /// An unnamed string tag was expected.
    /// -or-
    /// A tag of a different type was expected.
    /// -or-
    /// The encoded tag name is longer than 65535 bytes.
    /// -or-
    /// The encoded text is longer than 65535 bytes.
    /// </exception>
    public void WriteString(string tagName, string value)
    {
        if (value == null) throw new ArgumentNullException(nameof(value));
        EnforceConstraints(tagName, NbtTagType.String);
        _writer.Write((byte)NbtTagType.String);
        _writer.Write(tagName);
        _writer.Write(value);
    }

    #endregion

    #region ByteArray, IntArray and LongArray

    /// <summary>
    /// Writes an unnamed byte array tag that contains the whole of the specified array.
    /// </summary>
    /// <param name="data">The array that holds the data to write.</param>
    /// <exception cref="ArgumentNullException"><paramref name="data"/> is <see langword="null"/>.</exception>
    /// <exception cref="NbtFormatException">
    /// No more tags can be written.
    /// -or-
    /// A named byte array tag was expected.
    /// -or-
    /// A tag of a different type was expected.
    /// -or-
    /// The size of the parent list has been exceeded.
    /// </exception>
    public void WriteByteArray(byte[] data)
    {
        if (data == null) throw new ArgumentNullException(nameof(data));
        WriteByteArray(data, 0, data.Length);
    }

    /// <summary>
    /// Writes an unnamed byte array tag that contains a range of the specified array.
    /// </summary>
    /// <param name="data">The array that holds the data to write.</param>
    /// <param name="offset">The zero-based index in <paramref name="data"/> at which writing starts. This
    /// value cannot be negative.</param>
    /// <param name="count">The number of bytes to write. This value cannot be negative.</param>
    /// <exception cref="ArgumentNullException"><paramref name="data"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="offset"/> is less than zero.
    /// -or-
    /// <paramref name="count"/> is less than zero.
    /// </exception>
    /// <exception cref="ArgumentException"><paramref name="count"/> is greater than the length of
    /// <paramref name="data"/> minus <paramref name="offset"/>.</exception>
    /// <exception cref="NbtFormatException">
    /// No more tags can be written.
    /// -or-
    /// A named byte array tag was expected.
    /// -or-
    /// A tag of a different type was expected.
    /// -or-
    /// The size of the parent list has been exceeded.
    /// </exception>
    public void WriteByteArray(byte[] data, int offset, int count)
    {
        CheckArray(data, offset, count);
        EnforceConstraints(null!, NbtTagType.ByteArray);
        _writer.Write(count);
        _writer.Write(data, offset, count);
    }

    /// <summary>
    /// Writes a named byte array tag that contains the whole of the specified array.
    /// </summary>
    /// <param name="tagName">The name of the tag. This value cannot be <see langword="null"/>.</param>
    /// <param name="data">The array that holds the data to write.</param>
    /// <exception cref="ArgumentNullException"><paramref name="data"/> is <see langword="null"/>.</exception>
    /// <exception cref="NbtFormatException">
    /// No more tags can be written.
    /// -or-
    /// An unnamed byte array tag was expected.
    /// -or-
    /// A tag of a different type was expected.
    /// -or-
    /// The encoded tag name is longer than 65535 bytes.
    /// </exception>
    public void WriteByteArray(string tagName, byte[] data)
    {
        if (data == null) throw new ArgumentNullException(nameof(data));
        WriteByteArray(tagName, data, 0, data.Length);
    }

    /// <summary>
    /// Writes a named byte array tag that contains a range of the specified array.
    /// </summary>
    /// <param name="tagName">The name of the tag. This value cannot be <see langword="null"/>.</param>
    /// <param name="data">The array that holds the data to write.</param>
    /// <param name="offset">The zero-based index in <paramref name="data"/> at which writing starts. This
    /// value cannot be negative.</param>
    /// <param name="count">The number of bytes to write. This value cannot be negative.</param>
    /// <exception cref="ArgumentNullException"><paramref name="data"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="offset"/> is less than zero.
    /// -or-
    /// <paramref name="count"/> is less than zero.
    /// </exception>
    /// <exception cref="ArgumentException"><paramref name="count"/> is greater than the length of
    /// <paramref name="data"/> minus <paramref name="offset"/>.</exception>
    /// <exception cref="NbtFormatException">
    /// No more tags can be written.
    /// -or-
    /// An unnamed byte array tag was expected.
    /// -or-
    /// A tag of a different type was expected.
    /// -or-
    /// The encoded tag name is longer than 65535 bytes.
    /// </exception>
    public void WriteByteArray(string tagName, byte[] data, int offset, int count)
    {
        CheckArray(data, offset, count);
        EnforceConstraints(tagName, NbtTagType.ByteArray);
        _writer.Write((byte)NbtTagType.ByteArray);
        _writer.Write(tagName);
        _writer.Write(count);
        _writer.Write(data, offset, count);
    }

    /// <summary>
    /// Writes an unnamed byte array tag, copying the data from the specified stream.
    /// </summary>
    /// <param name="dataSource">The stream that the data is copied from.</param>
    /// <param name="count">The number of bytes to write. This value cannot be negative.</param>
    /// <exception cref="ArgumentNullException"><paramref name="dataSource"/> is
    /// <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="count"/> is less than
    /// zero.</exception>
    /// <exception cref="ArgumentException"><paramref name="dataSource"/> does not support
    /// reading.</exception>
    /// <exception cref="EndOfStreamException">The end of <paramref name="dataSource"/> was reached before
    /// <paramref name="count"/> bytes were read.</exception>
    /// <exception cref="NbtFormatException">
    /// No more tags can be written.
    /// -or-
    /// A named byte array tag was expected.
    /// -or-
    /// A tag of a different type was expected.
    /// -or-
    /// The size of the parent list has been exceeded.
    /// </exception>
    /// <remarks>
    /// A temporary buffer of up to 8192 bytes is allocated. The overloads that take a buffer use the buffer
    /// that is passed to them instead.
    /// </remarks>
    public void WriteByteArray(Stream dataSource, int count)
    {
        if (dataSource == null) throw new ArgumentNullException(nameof(dataSource));
        if (!dataSource.CanRead)
            throw new ArgumentException("Given stream does not support reading.", nameof(dataSource));

        if (count < 0) throw new ArgumentOutOfRangeException(nameof(count), "count may not be negative");
        var bufferSize = Math.Min(count, MaxStreamCopyBufferSize);
        var streamCopyBuffer = new byte[bufferSize];
        WriteByteArray(dataSource, count, streamCopyBuffer);
    }

    /// <summary>
    /// Writes an unnamed byte array tag, copying the data from the specified stream through the specified
    /// buffer.
    /// </summary>
    /// <param name="dataSource">The stream that the data is copied from.</param>
    /// <param name="count">The number of bytes to write. This value cannot be negative.</param>
    /// <param name="buffer">The buffer used for copying. Its length must be greater than zero when
    /// <paramref name="count"/> is greater than zero.</param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="dataSource"/> is <see langword="null"/>.
    /// -or-
    /// <paramref name="buffer"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="count"/> is less than
    /// zero.</exception>
    /// <exception cref="ArgumentException">
    /// <paramref name="dataSource"/> does not support reading.
    /// -or-
    /// The length of <paramref name="buffer"/> is zero and <paramref name="count"/> is greater than zero.
    /// </exception>
    /// <exception cref="EndOfStreamException">The end of <paramref name="dataSource"/> was reached before
    /// <paramref name="count"/> bytes were read.</exception>
    /// <exception cref="NbtFormatException">
    /// No more tags can be written.
    /// -or-
    /// A named byte array tag was expected.
    /// -or-
    /// A tag of a different type was expected.
    /// -or-
    /// The size of the parent list has been exceeded.
    /// </exception>
    public void WriteByteArray(Stream dataSource, int count, byte[] buffer)
    {
        if (dataSource == null) throw new ArgumentNullException(nameof(dataSource));
        if (buffer == null) throw new ArgumentNullException(nameof(buffer));
        if (!dataSource.CanRead)
            throw new ArgumentException("Given stream does not support reading.", nameof(dataSource));

        if (count < 0) throw new ArgumentOutOfRangeException(nameof(count), "count may not be negative");
        if (buffer.Length == 0 && count > 0)
            throw new ArgumentException("buffer size must be greater than 0 when count is greater than 0",
                nameof(buffer));

        EnforceConstraints(null!, NbtTagType.ByteArray);
        WriteByteArrayFromStreamImpl(dataSource, count, buffer);
    }

    /// <summary>
    /// Writes a named byte array tag, copying the data from the specified stream.
    /// </summary>
    /// <param name="tagName">The name of the tag. This value cannot be <see langword="null"/>.</param>
    /// <param name="dataSource">The stream that the data is copied from.</param>
    /// <param name="count">The number of bytes to write. This value cannot be negative.</param>
    /// <exception cref="ArgumentNullException"><paramref name="dataSource"/> is
    /// <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="count"/> is less than
    /// zero.</exception>
    /// <exception cref="ArgumentException"><paramref name="dataSource"/> does not support
    /// reading.</exception>
    /// <exception cref="EndOfStreamException">The end of <paramref name="dataSource"/> was reached before
    /// <paramref name="count"/> bytes were read.</exception>
    /// <exception cref="NbtFormatException">
    /// No more tags can be written.
    /// -or-
    /// An unnamed byte array tag was expected.
    /// -or-
    /// A tag of a different type was expected.
    /// -or-
    /// The encoded tag name is longer than 65535 bytes.
    /// </exception>
    /// <remarks>
    /// A temporary buffer of up to 8192 bytes is allocated. The overloads that take a buffer use the buffer
    /// that is passed to them instead.
    /// </remarks>
    public void WriteByteArray(string tagName, Stream dataSource, int count)
    {
        if (dataSource == null) throw new ArgumentNullException(nameof(dataSource));
        if (count < 0) throw new ArgumentOutOfRangeException(nameof(count), "count may not be negative");
        var bufferSize = Math.Min(count, MaxStreamCopyBufferSize);
        var streamCopyBuffer = new byte[bufferSize];
        WriteByteArray(tagName, dataSource, count, streamCopyBuffer);
    }

    /// <summary>
    /// Writes a named byte array tag, copying the data from the specified stream through the specified
    /// buffer.
    /// </summary>
    /// <param name="tagName">The name of the tag. This value cannot be <see langword="null"/>.</param>
    /// <param name="dataSource">The stream that the data is copied from.</param>
    /// <param name="count">The number of bytes to write. This value cannot be negative.</param>
    /// <param name="buffer">The buffer used for copying. Its length must be greater than zero when
    /// <paramref name="count"/> is greater than zero.</param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="dataSource"/> is <see langword="null"/>.
    /// -or-
    /// <paramref name="buffer"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="count"/> is less than
    /// zero.</exception>
    /// <exception cref="ArgumentException">
    /// <paramref name="dataSource"/> does not support reading.
    /// -or-
    /// The length of <paramref name="buffer"/> is zero and <paramref name="count"/> is greater than zero.
    /// </exception>
    /// <exception cref="EndOfStreamException">The end of <paramref name="dataSource"/> was reached before
    /// <paramref name="count"/> bytes were read.</exception>
    /// <exception cref="NbtFormatException">
    /// No more tags can be written.
    /// -or-
    /// An unnamed byte array tag was expected.
    /// -or-
    /// A tag of a different type was expected.
    /// -or-
    /// The encoded tag name is longer than 65535 bytes.
    /// </exception>
    public void WriteByteArray(string tagName, Stream dataSource, int count,
        byte[] buffer)
    {
        if (dataSource == null) throw new ArgumentNullException(nameof(dataSource));
        if (buffer == null) throw new ArgumentNullException(nameof(buffer));
        if (!dataSource.CanRead)
            throw new ArgumentException("Given stream does not support reading.", nameof(dataSource));

        if (count < 0) throw new ArgumentOutOfRangeException(nameof(count), "count may not be negative");
        if (buffer.Length == 0 && count > 0)
            throw new ArgumentException("buffer size must be greater than 0 when count is greater than 0",
                nameof(buffer));

        EnforceConstraints(tagName, NbtTagType.ByteArray);
        _writer.Write((byte)NbtTagType.ByteArray);
        _writer.Write(tagName);
        WriteByteArrayFromStreamImpl(dataSource, count, buffer);
    }

    /// <summary>
    /// Writes an unnamed int array tag that contains the whole of the specified array.
    /// </summary>
    /// <param name="data">The array that holds the data to write.</param>
    /// <exception cref="ArgumentNullException"><paramref name="data"/> is <see langword="null"/>.</exception>
    /// <exception cref="NbtFormatException">
    /// No more tags can be written.
    /// -or-
    /// A named int array tag was expected.
    /// -or-
    /// A tag of a different type was expected.
    /// -or-
    /// The size of the parent list has been exceeded.
    /// </exception>
    public void WriteIntArray(int[] data)
    {
        if (data == null) throw new ArgumentNullException(nameof(data));
        WriteIntArray(data, 0, data.Length);
    }

    /// <summary>
    /// Writes an unnamed int array tag that contains a range of the specified array.
    /// </summary>
    /// <param name="data">The array that holds the data to write.</param>
    /// <param name="offset">The zero-based index in <paramref name="data"/> at which writing starts. This
    /// value cannot be negative.</param>
    /// <param name="count">The number of elements to write. This value cannot be negative.</param>
    /// <exception cref="ArgumentNullException"><paramref name="data"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="offset"/> is less than zero.
    /// -or-
    /// <paramref name="count"/> is less than zero.
    /// </exception>
    /// <exception cref="ArgumentException"><paramref name="count"/> is greater than the length of
    /// <paramref name="data"/> minus <paramref name="offset"/>.</exception>
    /// <exception cref="NbtFormatException">
    /// No more tags can be written.
    /// -or-
    /// A named int array tag was expected.
    /// -or-
    /// A tag of a different type was expected.
    /// -or-
    /// The size of the parent list has been exceeded.
    /// </exception>
    public void WriteIntArray(int[] data, int offset, int count)
    {
        CheckArray(data, offset, count);
        EnforceConstraints(null!, NbtTagType.IntArray);
        _writer.Write(count);
        _writer.WriteBigEndian(data.AsSpan(offset, count));
    }

    /// <summary>
    /// Writes a named int array tag that contains the whole of the specified array.
    /// </summary>
    /// <param name="tagName">The name of the tag. This value cannot be <see langword="null"/>.</param>
    /// <param name="data">The array that holds the data to write.</param>
    /// <exception cref="ArgumentNullException"><paramref name="data"/> is <see langword="null"/>.</exception>
    /// <exception cref="NbtFormatException">
    /// No more tags can be written.
    /// -or-
    /// An unnamed int array tag was expected.
    /// -or-
    /// A tag of a different type was expected.
    /// -or-
    /// The encoded tag name is longer than 65535 bytes.
    /// </exception>
    public void WriteIntArray(string tagName, int[] data)
    {
        if (data == null) throw new ArgumentNullException(nameof(data));
        WriteIntArray(tagName, data, 0, data.Length);
    }

    /// <summary>
    /// Writes a named int array tag that contains a range of the specified array.
    /// </summary>
    /// <param name="tagName">The name of the tag. This value cannot be <see langword="null"/>.</param>
    /// <param name="data">The array that holds the data to write.</param>
    /// <param name="offset">The zero-based index in <paramref name="data"/> at which writing starts. This
    /// value cannot be negative.</param>
    /// <param name="count">The number of elements to write. This value cannot be negative.</param>
    /// <exception cref="ArgumentNullException"><paramref name="data"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="offset"/> is less than zero.
    /// -or-
    /// <paramref name="count"/> is less than zero.
    /// </exception>
    /// <exception cref="ArgumentException"><paramref name="count"/> is greater than the length of
    /// <paramref name="data"/> minus <paramref name="offset"/>.</exception>
    /// <exception cref="NbtFormatException">
    /// No more tags can be written.
    /// -or-
    /// An unnamed int array tag was expected.
    /// -or-
    /// A tag of a different type was expected.
    /// -or-
    /// The encoded tag name is longer than 65535 bytes.
    /// </exception>
    public void WriteIntArray(string tagName, int[] data, int offset, int count)
    {
        CheckArray(data, offset, count);
        EnforceConstraints(tagName, NbtTagType.IntArray);
        _writer.Write((byte)NbtTagType.IntArray);
        _writer.Write(tagName);
        _writer.Write(count);
        _writer.WriteBigEndian(data.AsSpan(offset, count));
    }

    /// <summary>
    /// Writes an unnamed long array tag that contains the whole of the specified array.
    /// </summary>
    /// <param name="data">The array that holds the data to write.</param>
    /// <exception cref="ArgumentNullException"><paramref name="data"/> is <see langword="null"/>.</exception>
    /// <exception cref="NbtFormatException">
    /// No more tags can be written.
    /// -or-
    /// A named long array tag was expected.
    /// -or-
    /// A tag of a different type was expected.
    /// -or-
    /// The size of the parent list has been exceeded.
    /// </exception>
    public void WriteLongArray(long[] data)
    {
        if (data == null) throw new ArgumentNullException(nameof(data));
        WriteLongArray(data, 0, data.Length);
    }

    /// <summary>
    /// Writes an unnamed long array tag that contains a range of the specified array.
    /// </summary>
    /// <param name="data">The array that holds the data to write.</param>
    /// <param name="offset">The zero-based index in <paramref name="data"/> at which writing starts. This
    /// value cannot be negative.</param>
    /// <param name="count">The number of elements to write. This value cannot be negative.</param>
    /// <exception cref="ArgumentNullException"><paramref name="data"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="offset"/> is less than zero.
    /// -or-
    /// <paramref name="count"/> is less than zero.
    /// </exception>
    /// <exception cref="ArgumentException"><paramref name="count"/> is greater than the length of
    /// <paramref name="data"/> minus <paramref name="offset"/>.</exception>
    /// <exception cref="NbtFormatException">
    /// No more tags can be written.
    /// -or-
    /// A named long array tag was expected.
    /// -or-
    /// A tag of a different type was expected.
    /// -or-
    /// The size of the parent list has been exceeded.
    /// </exception>
    public void WriteLongArray(long[] data, int offset, int count)
    {
        CheckArray(data, offset, count);
        EnforceConstraints(null!, NbtTagType.LongArray);
        _writer.Write(count);
        _writer.WriteBigEndian(data.AsSpan(offset, count));
    }

    /// <summary>
    /// Writes a named long array tag that contains the whole of the specified array.
    /// </summary>
    /// <param name="tagName">The name of the tag. This value cannot be <see langword="null"/>.</param>
    /// <param name="data">The array that holds the data to write.</param>
    /// <exception cref="ArgumentNullException"><paramref name="data"/> is <see langword="null"/>.</exception>
    /// <exception cref="NbtFormatException">
    /// No more tags can be written.
    /// -or-
    /// An unnamed long array tag was expected.
    /// -or-
    /// A tag of a different type was expected.
    /// -or-
    /// The encoded tag name is longer than 65535 bytes.
    /// </exception>
    public void WriteLongArray(string tagName, long[] data)
    {
        if (data == null) throw new ArgumentNullException(nameof(data));
        WriteLongArray(tagName, data, 0, data.Length);
    }

    /// <summary>
    /// Writes a named long array tag that contains a range of the specified array.
    /// </summary>
    /// <param name="tagName">The name of the tag. This value cannot be <see langword="null"/>.</param>
    /// <param name="data">The array that holds the data to write.</param>
    /// <param name="offset">The zero-based index in <paramref name="data"/> at which writing starts. This
    /// value cannot be negative.</param>
    /// <param name="count">The number of elements to write. This value cannot be negative.</param>
    /// <exception cref="ArgumentNullException"><paramref name="data"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="offset"/> is less than zero.
    /// -or-
    /// <paramref name="count"/> is less than zero.
    /// </exception>
    /// <exception cref="ArgumentException"><paramref name="count"/> is greater than the length of
    /// <paramref name="data"/> minus <paramref name="offset"/>.</exception>
    /// <exception cref="NbtFormatException">
    /// No more tags can be written.
    /// -or-
    /// An unnamed long array tag was expected.
    /// -or-
    /// A tag of a different type was expected.
    /// -or-
    /// The encoded tag name is longer than 65535 bytes.
    /// </exception>
    public void WriteLongArray(string tagName, long[] data, int offset, int count)
    {
        CheckArray(data, offset, count);
        EnforceConstraints(tagName, NbtTagType.LongArray);
        _writer.Write((byte)NbtTagType.LongArray);
        _writer.Write(tagName);
        _writer.Write(count);
        _writer.WriteBigEndian(data.AsSpan(offset, count));
    }

    #endregion
}
