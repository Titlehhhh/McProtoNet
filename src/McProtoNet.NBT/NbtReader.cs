using System.Text;

namespace McProtoNet.NBT;

/// <summary>
/// Provides fast, non-cached, forward-only access to NBT data in a stream.
/// </summary>
/// <remarks>
/// Each instance reads one complete document. The input is Java Edition NBT only: every number is
/// big-endian, every string is modified UTF-8, and there is no little-endian (Bedrock) mode.
/// </remarks>
public class NbtReader
{
    private const string NoValueToReadError = "Value already read, or no value to read.",
        InvalidParentTagError = "Parent tag is neither a Compound nor a List.",
        ErroneousStateError = "NbtReader is in an erroneous state!";

    private readonly bool _canSeekStream;
    private readonly NbtBinaryReader _reader;
    private readonly bool _readRootName;
    private readonly long _streamStartOffset;
    private bool _atValue;

    private bool _cacheTagValues;
    private Stack<NbtReaderNode> _nodes;
    private NbtParseState _state = NbtParseState.AtStreamBeginning;
    private object? _valueCache;

    /// <summary>
    /// Initializes a new instance of the <see cref="NbtReader"/> class that reads from the specified stream.
    /// </summary>
    /// <param name="stream">The stream to read from.</param>
    /// <param name="readRootName"><see langword="true"/> to expect a named root tag, as an NBT file carries
    /// it; <see langword="false"/> to expect the nameless root of the network format used since 1.20.2. The
    /// default is <see langword="true"/>.</param>
    /// <exception cref="ArgumentNullException"><paramref name="stream"/> is
    /// <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="stream"/> is not readable.</exception>
    public NbtReader(Stream stream, bool readRootName = true)
    {
        ArgumentNullException.ThrowIfNull(stream);
        SkipEndTags = true;
        CacheTagValues = false;
        ParentTagType = NbtTagType.End;
        TagType = NbtTagType.End;
        _readRootName = readRootName;

        _canSeekStream = stream.CanSeek;
        if (_canSeekStream) _streamStartOffset = stream.Position;

        _reader = new NbtBinaryReader(stream);
    }

    /// <summary>
    /// Reads one complete tag from the specified stream and builds the tag tree.
    /// </summary>
    /// <param name="stream">The stream, positioned at the tag type byte.</param>
    /// <param name="readRootName"><see langword="true"/> to read the root tag's name after the type byte;
    /// <see langword="false"/> to expect the nameless root of the network format. The default is
    /// <see langword="true"/>.</param>
    /// <returns>The tag that was read, or <see langword="null"/> if the first byte is
    /// <see cref="NbtTagType.End"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="stream"/> is
    /// <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="stream"/> is not readable.</exception>
    /// <exception cref="NbtFormatException">The data is malformed, truncated, or nested too
    /// deeply.</exception>
    /// <remarks>
    /// The root tag can be of any type. A network root can be a <see cref="NbtTagType.String"/> since 1.20.3.
    /// </remarks>
    public static NbtTag? ReadTag(Stream stream, bool readRootName = true)
    {
        ArgumentNullException.ThrowIfNull(stream);
        using var reader = new NbtBinaryReader(stream);

        var type = reader.ReadTagType();
        if (type == NbtTagType.End) return null;

        var tag = NbtTag.CreateTag(type);
        if (readRootName) tag.Name = reader.ReadString();
        tag.ReadTag(reader);
        return tag;
    }


    /// <summary>
    /// Gets the name of the root tag.
    /// </summary>
    /// <value>
    /// The name of the root tag, or <see langword="null"/> if the root tag has not been read yet or the
    /// stream uses the nameless network format.
    /// </value>
    public string? RootName { get; private set; }

    /// <summary>
    /// Gets the name of the parent tag.
    /// </summary>
    /// <value>
    /// The name of the parent tag, or <see langword="null"/> for a root tag and for a descendant of a list
    /// element.
    /// </value>
    public string? ParentName { get; private set; }

    /// <summary>
    /// Gets the name of the current tag.
    /// </summary>
    /// <value>
    /// The name of the current tag, or <see langword="null"/> for a list element and for an end tag.
    /// </value>
    public string? TagName { get; private set; }

    /// <summary>
    /// Gets the type of the parent tag.
    /// </summary>
    /// <value>
    /// The type of the parent tag, or <see cref="NbtTagType.End"/> if there is no parent tag.
    /// </value>
    public NbtTagType ParentTagType { get; private set; }

    /// <summary>
    /// Gets the type of the current tag.
    /// </summary>
    public NbtTagType TagType { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the current tag is an element of a list.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if the current tag is an element of a list; otherwise,
    /// <see langword="false"/>.
    /// </value>
    public bool IsListElement => ParentTagType == NbtTagType.List;

    /// <summary>
    /// Gets a value indicating whether the current tag has a value to read.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if the current tag is neither a compound, nor a list, nor an end tag;
    /// otherwise, <see langword="false"/>.
    /// </value>
    public bool HasValue =>
        TagType is not (NbtTagType.Compound or NbtTagType.End or NbtTagType.List);

    /// <summary>
    /// Gets a value indicating whether the current tag has a name.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if the current tag has a name; otherwise, <see langword="false"/>.
    /// </value>
    public bool HasName => TagName != null;

    /// <summary>
    /// Gets a value indicating whether the reader has reached the end of the stream.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if the reader has reached the end of the stream; otherwise,
    /// <see langword="false"/>.
    /// </value>
    public bool IsAtStreamEnd => _state == NbtParseState.AtStreamEnd;

    /// <summary>
    /// Gets a value indicating whether the current tag is a compound.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if the current tag is a compound; otherwise, <see langword="false"/>.
    /// </value>
    public bool IsCompound => TagType == NbtTagType.Compound;

    /// <summary>
    /// Gets a value indicating whether the current tag is a list.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if the current tag is a list; otherwise, <see langword="false"/>.
    /// </value>
    public bool IsList => TagType == NbtTagType.List;

    /// <summary>
    /// Gets a value indicating whether the current tag declares a length.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if the current tag is a list, a byte array, an int array, or a long array;
    /// otherwise, <see langword="false"/>.
    /// </value>
    /// <remarks>
    /// A compound tag also has a length, but the length is not known until all of its child tags are read.
    /// </remarks>
    public bool HasLength =>
        TagType is NbtTagType.List or NbtTagType.ByteArray or NbtTagType.IntArray or NbtTagType.LongArray;

    /// <summary>
    /// Gets the stream that the reader reads from.
    /// </summary>
    public Stream BaseStream => _reader.BaseStream;

    /// <summary>
    /// Gets the number of bytes from the beginning of the stream to the beginning of the current tag.
    /// </summary>
    /// <value>
    /// The offset of the current tag, in bytes. The value is always 0 when the stream is not seekable.
    /// </value>
    public int TagStartOffset { get; private set; }

    /// <summary>
    /// Gets the number of tags read from the stream so far.
    /// </summary>
    /// <value>
    /// The number of tags read, including the current tag and all skipped tags.
    /// </value>
    /// <remarks>
    /// End tags are counted only when <see cref="SkipEndTags"/> is <see langword="false"/>.
    /// </remarks>
    public int TagsRead { get; private set; }

    /// <summary>
    /// Gets the depth of the current tag in the tag tree.
    /// </summary>
    /// <value>
    /// The depth of the current tag. The root tag is at depth 1, its child tags are at depth 2, and so on.
    /// </value>
    public int Depth { get; private set; }

    /// <summary>
    /// Gets the type of the elements of the current list tag.
    /// </summary>
    /// <value>
    /// The element type when the current tag is a list; otherwise, <see cref="NbtTagType.End"/>.
    /// </value>
    public NbtTagType ListType { get; private set; }

    /// <summary>
    /// Gets the number of elements of the current tag.
    /// </summary>
    /// <value>
    /// The number of elements when the current tag is a list, a byte array, an int array, or a long array;
    /// otherwise, 0.
    /// </value>
    public int TagLength { get; private set; }

    /// <summary>
    /// Gets the number of elements of the parent list tag.
    /// </summary>
    /// <value>
    /// The number of elements when the parent tag is a list; otherwise, 0.
    /// </value>
    public int ParentTagLength { get; private set; }

    /// <summary>
    /// Gets the index of the current tag within its parent list tag.
    /// </summary>
    /// <value>
    /// The zero-based index of the current tag when the parent tag is a list.
    /// </value>
    public int ListIndex { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the reader is in an error state.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if a parse error occurred; otherwise, <see langword="false"/>.
    /// </value>
    /// <remarks>
    /// No further reading is possible from an instance that is in an error state.
    /// </remarks>
    public bool IsInErrorState => _state == NbtParseState.Error;

    /// <summary>
    /// Gets or sets a value indicating whether end tags are skipped automatically while parsing.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if <see cref="ReadToFollowing()"/> skips end tags; otherwise,
    /// <see langword="false"/>. The default is <see langword="true"/>.
    /// </value>
    public bool SkipEndTags { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the most recently read tag value is kept.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if a copy of the most recently read tag value is kept; otherwise,
    /// <see langword="false"/>. The default is <see langword="false"/>.
    /// </value>
    /// <remarks>
    /// Unless this property is <see langword="true"/>, a tag value can be read only once. Setting it to
    /// <see langword="false"/> discards the value that is currently held.
    /// </remarks>
    public bool CacheTagValues
    {
        get => _cacheTagValues;
        set
        {
            _cacheTagValues = value;
            if (!_cacheTagValues) _valueCache = null;
        }
    }


    /// <summary>
    /// Reads the next tag from the stream.
    /// </summary>
    /// <returns><see langword="true"/> if the next tag was read; otherwise,
    /// <see langword="false"/>.</returns>
    /// <exception cref="NbtFormatException">An error occurred while the NBT data was parsed.</exception>
    /// <exception cref="InvalidReaderStateException">The reader cannot recover from a previous parsing
    /// error.</exception>
    public bool ReadToFollowing()
    {
        switch (_state)
        {
            case NbtParseState.AtStreamBeginning:
                _state = NbtParseState.Error;
                var rootType = _reader.ReadTagType();
                if (rootType == NbtTagType.End)
                {
                    _state = NbtParseState.AtStreamEnd;
                    return false;
                }

                Depth = 1;
                TagType = rootType;
                _state = NbtParseState.AtRootValue;
                ReadTagHeader(_readRootName);
                RootName = TagName;
                return true;
            case NbtParseState.AtRootValue:
                if (_atValue) SkipValue();
                _state = NbtParseState.AtStreamEnd;
                return false;
            case NbtParseState.AtCompoundBeginning:
                GoDown();
                _state = NbtParseState.InCompound;
                goto case NbtParseState.InCompound;
            case NbtParseState.InCompound:
                _state = NbtParseState.Error;
                if (_atValue) SkipValue();

                // Read next tag, check if we've hit the end
                if (_canSeekStream) TagStartOffset = (int)(_reader.BaseStream.Position - _streamStartOffset);

                // set state to error in case reader.ReadTagType throws.
                TagType = _reader.ReadTagType();
                _state = NbtParseState.InCompound;

                if (TagType == NbtTagType.End)
                {
                    TagName = null;
                    TagsRead++;
                    _state = NbtParseState.AtCompoundEnd;
                    if (SkipEndTags)
                    {
                        TagsRead--;
                        goto case NbtParseState.AtCompoundEnd;
                    }

                    return true;
                }

                ReadTagHeader(true);
                return true;
            case NbtParseState.AtListBeginning:
                GoDown();
                ListIndex = -1;
                TagType = ListType;
                _state = NbtParseState.InList;
                goto case NbtParseState.InList;
            case NbtParseState.InList:
                _state = NbtParseState.Error;
                if (_atValue) SkipValue();
                ListIndex++;
                if (ListIndex >= ParentTagLength)
                {
                    GoUp();
                    if (ParentTagType == NbtTagType.List)
                    {
                        _state = NbtParseState.InList;
                        TagType = NbtTagType.List;
                        goto case NbtParseState.InList;
                    }

                    if (ParentTagType == NbtTagType.Compound)
                    {
                        _state = NbtParseState.InCompound;
                        goto case NbtParseState.InCompound;
                    }

                    if (ParentTagType == NbtTagType.End)
                    {
                        _state = NbtParseState.AtStreamEnd;
                        return false;
                    }

                    // This should not happen unless NbtReader is bugged
                    throw new NbtFormatException(InvalidParentTagError);
                }

                if (_canSeekStream) TagStartOffset = (int)(_reader.BaseStream.Position - _streamStartOffset);
                _state = NbtParseState.InList;
                ReadTagHeader(false);
                return true;
            case NbtParseState.AtCompoundEnd:
                GoUp();
                if (ParentTagType == NbtTagType.List)
                {
                    _state = NbtParseState.InList;
                    TagType = NbtTagType.Compound;
                    goto case NbtParseState.InList;
                }

                if (ParentTagType == NbtTagType.Compound)
                {
                    _state = NbtParseState.InCompound;
                    goto case NbtParseState.InCompound;
                }

                if (ParentTagType == NbtTagType.End)
                {
                    _state = NbtParseState.AtStreamEnd;
                    return false;
                }

                // This should not happen unless NbtReader is bugged
                _state = NbtParseState.Error;
                throw new NbtFormatException(InvalidParentTagError);
            case NbtParseState.AtStreamEnd:
                // nothing left to read!
                return false;
            default:
                // Parsing error, or unexpected state.
                throw new InvalidReaderStateException(ErroneousStateError);
        }
    }

    private void ReadTagHeader(bool readName)
    {
        // Setting state to error in case reader throws
        var oldState = _state;
        _state = NbtParseState.Error;
        TagsRead++;
        TagName = readName ? _reader.ReadString() : null;

        _valueCache = null!;
        TagLength = 0;
        _atValue = false;
        ListType = NbtTagType.End;

        switch (TagType)
        {
            case NbtTagType.Byte:
            case NbtTagType.Short:
            case NbtTagType.Int:
            case NbtTagType.Long:
            case NbtTagType.Float:
            case NbtTagType.Double:
            case NbtTagType.String:
                _atValue = true;
                _state = oldState;
                break;
            case NbtTagType.IntArray:
            case NbtTagType.ByteArray:
            case NbtTagType.LongArray:
                TagLength = _reader.ReadInt32();
                if (TagLength < 0) throw new NbtFormatException("Negative array length given: " + TagLength);
                _atValue = true;
                _state = oldState;
                break;
            case NbtTagType.List:
                ListType = _reader.ReadTagType();
                TagLength = _reader.ReadInt32();
                if (TagLength < 0) throw new NbtFormatException("Negative tag length given: " + TagLength);
                if (TagLength > 0 && ListType == NbtTagType.End)
                    throw new NbtFormatException("Non-empty NBT list of TAG_End elements.");
                _state = NbtParseState.AtListBeginning;
                break;
            case NbtTagType.Compound:
                _state = NbtParseState.AtCompoundBeginning;
                break;
        }
    }

    // Goes one step down the tag tree, saving the current state on the node stack.
    private void GoDown()
    {
        if (Depth > NbtLimits.MaxDepth)
            throw new NbtFormatException($"NBT nesting exceeds the maximum depth of {NbtLimits.MaxDepth}.");
        if (_nodes == null) _nodes = new Stack<NbtReaderNode>();
        var newNode = new NbtReaderNode
        {
            ListIndex = ListIndex,
            ParentTagLength = ParentTagLength,
            ParentName = ParentName,
            ParentTagType = ParentTagType,
            ListType = ListType
        };
        _nodes.Push(newNode);

        ParentName = TagName;
        ParentTagType = TagType;
        ParentTagLength = TagLength;
        ListIndex = 0;
        TagLength = 0;

        Depth++;
    }

    // Goes one step up the tag tree, restoring the state saved on the node stack.
    private void GoUp()
    {
        var oldNode = _nodes.Pop();

        ParentName = oldNode.ParentName;
        ParentTagType = oldNode.ParentTagType;
        ParentTagLength = oldNode.ParentTagLength;
        ListIndex = oldNode.ListIndex;
        ListType = oldNode.ListType;
        TagLength = 0;

        Depth--;
    }

    private void SkipValue()
    {
        // Make sure to check for "atValue" before calling this method
        switch (TagType)
        {
            case NbtTagType.Byte:
                _reader.ReadByte();
                break;
            case NbtTagType.Short:
                _reader.ReadInt16();
                break;
            case NbtTagType.Float:
            case NbtTagType.Int:
                _reader.ReadInt32();
                break;
            case NbtTagType.Double:
            case NbtTagType.Long:
                _reader.ReadInt64();
                break;
            case NbtTagType.ByteArray:
                _reader.Skip(TagLength);
                break;
            case NbtTagType.IntArray:
                _reader.Skip((long)TagLength * sizeof(int));
                break;
            case NbtTagType.LongArray:
                _reader.Skip((long)TagLength * sizeof(long));
                break;
            case NbtTagType.String:
                _reader.SkipString();
                break;
        }

        _atValue = false;
        _valueCache = null;
    }


    /// <summary>
    /// Reads until a tag with the specified name is found.
    /// </summary>
    /// <param name="tagName">The name of the tag to look for. Can be <see langword="null"/> to look for the
    /// next unnamed tag.</param>
    /// <returns><see langword="true"/> if a matching tag is found; otherwise,
    /// <see langword="false"/>.</returns>
    /// <exception cref="NbtFormatException">An error occurred while the NBT data was parsed.</exception>
    /// <exception cref="InvalidReaderStateException">The reader cannot recover from a previous parsing
    /// error.</exception>
    /// <remarks>
    /// When no matching tag is found, the reader is positioned at the end of the stream.
    /// </remarks>
    public bool ReadToFollowing(string? tagName)
    {
        while (ReadToFollowing())
            if (TagName == tagName)
                return true;
        return false;
    }

    /// <summary>
    /// Advances the reader to the next descendant tag with the specified name.
    /// </summary>
    /// <param name="tagName">The name of the tag to move to. Can be <see langword="null"/> to look for the
    /// next unnamed tag.</param>
    /// <returns><see langword="true"/> if a matching descendant tag is found; otherwise,
    /// <see langword="false"/>.</returns>
    /// <exception cref="NbtFormatException">An error occurred while the NBT data was parsed.</exception>
    /// <exception cref="InvalidReaderStateException">The reader cannot recover from a previous parsing
    /// error.</exception>
    /// <remarks>
    /// When no matching child tag is found, the reader is positioned on the end tag.
    /// </remarks>
    public bool ReadToDescendant(string? tagName)
    {
        switch (_state)
        {
            case NbtParseState.Error:
                throw new InvalidReaderStateException(ErroneousStateError);
            case NbtParseState.AtStreamEnd:
                return false;
        }

        var currentDepth = Depth;
        while (ReadToFollowing())
            if (Depth <= currentDepth)
                return false;
            else if (TagName == tagName) return true;
        return false;
    }

    /// <summary>
    /// Advances the reader to the next sibling tag, skipping any child tags.
    /// </summary>
    /// <returns><see langword="true"/> if a sibling tag is found; otherwise,
    /// <see langword="false"/>.</returns>
    /// <exception cref="NbtFormatException">An error occurred while the NBT data was parsed.</exception>
    /// <exception cref="InvalidReaderStateException">The reader cannot recover from a previous parsing
    /// error.</exception>
    /// <remarks>
    /// When there are no more siblings, the reader is positioned on the tag that follows the last descendant
    /// of the current tag.
    /// </remarks>
    public bool ReadToNextSibling()
    {
        switch (_state)
        {
            case NbtParseState.Error:
                throw new InvalidReaderStateException(ErroneousStateError);
            case NbtParseState.AtStreamEnd:
                return false;
        }

        var currentDepth = Depth;
        while (ReadToFollowing())
            if (Depth == currentDepth)
                return true;
            else if (Depth < currentDepth) return false;
        return false;
    }

    /// <summary>
    /// Advances the reader to the next sibling tag with the specified name.
    /// </summary>
    /// <param name="tagName">The name of the sibling tag to move to. Can be <see langword="null"/> to look
    /// for the next unnamed sibling tag.</param>
    /// <returns><see langword="true"/> if a matching sibling tag is found; otherwise,
    /// <see langword="false"/>.</returns>
    /// <exception cref="NbtFormatException">An error occurred while the NBT data was parsed.</exception>
    /// <exception cref="InvalidReaderStateException">The reader cannot recover from a previous parsing
    /// error.</exception>
    /// <remarks>
    /// When no matching sibling tag is found, the reader is positioned on the tag that follows the last
    /// sibling.
    /// </remarks>
    public bool ReadToNextSibling(string? tagName)
    {
        while (ReadToNextSibling())
            if (TagName == tagName)
                return true;
        return false;
    }

    /// <summary>
    /// Skips the current tag, its value and descendants, and any following siblings, reading up to the
    /// sibling of the parent tag.
    /// </summary>
    /// <returns>The total number of tags that were skipped, or 0 if the end of the stream is
    /// reached.</returns>
    /// <exception cref="NbtFormatException">An error occurred while the NBT data was parsed.</exception>
    /// <exception cref="InvalidReaderStateException">The reader cannot recover from a previous parsing
    /// error.</exception>
    public int Skip()
    {
        switch (_state)
        {
            case NbtParseState.Error:
                throw new InvalidReaderStateException(ErroneousStateError);
            case NbtParseState.AtStreamEnd:
                return 0;
        }

        var startDepth = Depth;
        var skipped = 0;
        // ReSharper disable once LoopVariableIsNeverChangedInsideLoop
        while (ReadToFollowing() && Depth >= startDepth) skipped++;
        return skipped;
    }

    /// <summary>
    /// Reads the whole of the current tag, including any descendants, and builds a tag of the matching type.
    /// </summary>
    /// <returns>The tag that was read.</returns>
    /// <exception cref="NbtFormatException">An error occurred while the NBT data was parsed.</exception>
    /// <exception cref="InvalidReaderStateException">The reader cannot recover from a previous parsing
    /// error.</exception>
    /// <exception cref="EndOfStreamException">The end of the stream has been reached and no more tags can be
    /// read.</exception>
    /// <exception cref="InvalidOperationException">The current tag is an end tag, or its value has already
    /// been read.</exception>
    public NbtTag ReadAsTag()
    {
        switch (_state)
        {
            case NbtParseState.Error:
                throw new InvalidReaderStateException(ErroneousStateError);
            case NbtParseState.AtStreamEnd:
                throw new EndOfStreamException();
            case NbtParseState.AtStreamBeginning:
            case NbtParseState.AtCompoundEnd:
                ReadToFollowing();
                break;
        }

        // get this tag
        NbtTag parent;
        if (TagType == NbtTagType.Compound)
        {
            parent = new NbtCompound(TagName);
        }
        else if (TagType == NbtTagType.List)
        {
            parent = new NbtList(TagName, ListType);
        }
        else if (_atValue)
        {
            var result = ReadValueAsTag();
            ReadToFollowing();
            // if we're at a value tag, there are no child tags to read
            return result;
        }
        else
        {
            // end tags cannot be read-as-tags (there is no corresponding NbtTag object)
            throw new InvalidOperationException(NoValueToReadError);
        }

        var startingDepth = Depth;
        var parentDepth = Depth;

        do
        {
            ReadToFollowing();
            // Going up the file tree, or end of document: wrap up
            while (Depth <= parentDepth && parent.Parent != null)
            {
                parent = parent.Parent;
                parentDepth--;
            }

            if (Depth <= startingDepth) break;

            NbtTag thisTag;
            if (TagType == NbtTagType.Compound)
            {
                thisTag = new NbtCompound(TagName);
                AddToParent(thisTag, parent);
                parent = thisTag;
                parentDepth = Depth;
            }
            else if (TagType == NbtTagType.List)
            {
                thisTag = new NbtList(TagName, ListType);
                AddToParent(thisTag, parent);
                parent = thisTag;
                parentDepth = Depth;
            }
            else if (TagType != NbtTagType.End)
            {
                thisTag = ReadValueAsTag();
                AddToParent(thisTag, parent);
            }
        } while (true);

        return parent;
    }

    private void AddToParent(NbtTag thisTag, NbtTag parent)
    {
        if (parent is NbtList parentAsList)
            parentAsList.Add(thisTag);
        else if (parent is NbtCompound parentAsCompound)
            parentAsCompound.Add(thisTag);
        else
            // cannot happen unless NbtReader is bugged
            throw new NbtFormatException(InvalidParentTagError);
    }

    private NbtTag ReadValueAsTag()
    {
        if (!_atValue)
            // Should never happen
            throw new InvalidOperationException(NoValueToReadError);
        _atValue = false;
        switch (TagType)
        {
            case NbtTagType.Byte:
                return new NbtByte(TagName, _reader.ReadByte());
            case NbtTagType.Short:
                return new NbtShort(TagName, _reader.ReadInt16());
            case NbtTagType.Int:
                return new NbtInt(TagName, _reader.ReadInt32());
            case NbtTagType.Long:
                return new NbtLong(TagName, _reader.ReadInt64());
            case NbtTagType.Float:
                return new NbtFloat(TagName, _reader.ReadSingle());
            case NbtTagType.Double:
                return new NbtDouble(TagName, _reader.ReadDouble());
            case NbtTagType.String:
                return new NbtString(TagName, _reader.ReadString());
            case NbtTagType.ByteArray:
                return new NbtByteArray(TagName, _reader.ReadArrayBigEndian<byte>(TagLength));
            case NbtTagType.IntArray:
                return new NbtIntArray(TagName, _reader.ReadArrayBigEndian<int>(TagLength));
            case NbtTagType.LongArray:
                return new NbtLongArray(TagName, _reader.ReadArrayBigEndian<long>(TagLength));
            default:
                return null!;
        }
    }


    /// <summary>
    /// Reads the value of the current tag as a value of the specified type.
    /// </summary>
    /// <typeparam name="T">The type to return the value as. The tag value must be castable to this
    /// type.</typeparam>
    /// <returns>The tag value, cast to <typeparamref name="T"/>.</returns>
    /// <exception cref="EndOfStreamException">The end of the stream has been reached and no more tags can be
    /// read.</exception>
    /// <exception cref="NbtFormatException">An error occurred while the NBT data was parsed.</exception>
    /// <exception cref="InvalidOperationException">The value has already been read, or there is no value to
    /// read.</exception>
    /// <exception cref="InvalidCastException">The tag value cannot be cast to
    /// <typeparamref name="T"/>.</exception>
    public T ReadValueAs<T>()
    {
        return (T)ReadValue();
    }

    /// <summary>
    /// Reads the value of the current tag as a boxed object of the matching type.
    /// </summary>
    /// <returns>The tag value, boxed.</returns>
    /// <exception cref="EndOfStreamException">The end of the stream has been reached and no more tags can be
    /// read.</exception>
    /// <exception cref="NbtFormatException">An error occurred while the NBT data was parsed.</exception>
    /// <exception cref="InvalidOperationException">The value has already been read, or there is no value to
    /// read.</exception>
    /// <remarks>
    /// This method cannot be called on a tag that has no single-object value: a compound, a list, or an end
    /// tag.
    /// </remarks>
    public object ReadValue()
    {
        if (_state == NbtParseState.AtStreamEnd) throw new EndOfStreamException();
        if (!_atValue)
        {
            if (_cacheTagValues)
            {
                if (_valueCache == null)
                    throw new InvalidOperationException("No value to read.");
                return _valueCache;
            }

            throw new InvalidOperationException(NoValueToReadError);
        }

        _valueCache = null;
        _atValue = false;
        object value;
        switch (TagType)
        {
            case NbtTagType.Byte:
                value = _reader.ReadByte();
                break;
            case NbtTagType.Short:
                value = _reader.ReadInt16();
                break;
            case NbtTagType.Float:
                value = _reader.ReadSingle();
                break;
            case NbtTagType.Int:
                value = _reader.ReadInt32();
                break;
            case NbtTagType.Double:
                value = _reader.ReadDouble();
                break;
            case NbtTagType.Long:
                value = _reader.ReadInt64();
                break;
            case NbtTagType.ByteArray:
                value = _reader.ReadArrayBigEndian<byte>(TagLength);
                break;
            case NbtTagType.IntArray:
                value = _reader.ReadArrayBigEndian<int>(TagLength);
                break;
            case NbtTagType.LongArray:
                value = _reader.ReadArrayBigEndian<long>(TagLength);
                break;
            case NbtTagType.String:
                value = _reader.ReadString();
                break;
            default:
                value = null!;
                break;
        }

        _valueCache = _cacheTagValues ? value : null;
        return value;
    }

    /// <summary>
    /// Reads the elements of the current list tag as an array and stops after the last element.
    /// </summary>
    /// <typeparam name="T">The element type of the array to return. The tag contents must be convertible to
    /// this type.</typeparam>
    /// <returns>An array that contains the list elements, converted to <typeparamref name="T"/>.</returns>
    /// <exception cref="EndOfStreamException">The end of the stream has been reached and no more tags can be
    /// read.</exception>
    /// <exception cref="InvalidOperationException">The current tag is not a list.</exception>
    /// <exception cref="InvalidReaderStateException">The reader cannot recover from a previous parsing
    /// error.</exception>
    /// <exception cref="NbtFormatException">An error occurred while the NBT data was parsed.</exception>
    /// <remarks>
    /// The element type of the list must be byte, short, int, long, float, double, or string. When elements
    /// of the list have already been read, only the remaining elements are returned.
    /// </remarks>
    public T[] ReadListAsArray<T>()
    {
        switch (_state)
        {
            case NbtParseState.AtStreamEnd:
                throw new EndOfStreamException();
            case NbtParseState.Error:
                throw new InvalidReaderStateException(ErroneousStateError);
            case NbtParseState.AtListBeginning:
                GoDown();
                ListIndex = 0;
                TagType = ListType;
                _state = NbtParseState.InList;
                break;
            case NbtParseState.InList:
                break;
            default:
                throw new InvalidOperationException("ReadListAsArray may only be used on List tags.");
        }

        // ReadTagHeader resets ListType for every element, so the element type lives in TagType.
        var elementType = TagType;
        var elementsToRead = ParentTagLength - ListIndex;
        _atValue = false;
        _valueCache = null;

        // special handling for reading byte arrays (as byte arrays)
        if (elementType == NbtTagType.Byte && typeof(T) == typeof(byte))
        {
            TagsRead += elementsToRead;
            ListIndex = ParentTagLength - 1;
            return (T[])(object)_reader.ReadArrayBigEndian<byte>(elementsToRead);
        }

        // for everything else, gotta read elements one-by-one
        var result = new T[elementsToRead];
        switch (elementType)
        {
            case NbtTagType.Byte:
                for (var i = 0; i < elementsToRead; i++)
                    result[i] = (T)Convert.ChangeType(_reader.ReadByte(), typeof(T));

                break;
            case NbtTagType.Short:
                for (var i = 0; i < elementsToRead; i++)
                    result[i] = (T)Convert.ChangeType(_reader.ReadInt16(), typeof(T));

                break;
            case NbtTagType.Int:
                for (var i = 0; i < elementsToRead; i++)
                    result[i] = (T)Convert.ChangeType(_reader.ReadInt32(), typeof(T));

                break;
            case NbtTagType.Long:
                for (var i = 0; i < elementsToRead; i++)
                    result[i] = (T)Convert.ChangeType(_reader.ReadInt64(), typeof(T));

                break;
            case NbtTagType.Float:
                for (var i = 0; i < elementsToRead; i++)
                    result[i] = (T)Convert.ChangeType(_reader.ReadSingle(), typeof(T));

                break;
            case NbtTagType.Double:
                for (var i = 0; i < elementsToRead; i++)
                    result[i] = (T)Convert.ChangeType(_reader.ReadDouble(), typeof(T));

                break;
            case NbtTagType.String:
                for (var i = 0; i < elementsToRead; i++)
                    result[i] = (T)Convert.ChangeType(_reader.ReadString(), typeof(T));

                break;
        }

        TagsRead += elementsToRead;
        ListIndex = ParentTagLength - 1;
        return result;
    }

    /// <summary>
    /// Returns a string that represents the tag that the reader is positioned on.
    /// </summary>
    /// <returns>A string that contains the depth, ordinal number, type, name, and, for an array or a list,
    /// the size of the current tag. The value is not included.</returns>
    /// <remarks>
    /// The tag is indented with <see cref="NbtTag.DefaultIndentString"/>.
    /// </remarks>
    public override string ToString()
    {
        return ToString(false, NbtTag.DefaultIndentString);
    }

    /// <summary>
    /// Returns a string that represents the tag that the reader is positioned on, optionally including its
    /// value.
    /// </summary>
    /// <param name="includeValue"><see langword="true"/> to read and include the value of the current tag;
    /// otherwise, <see langword="false"/>.</param>
    /// <returns>A string that contains the depth, ordinal number, type, name, and, for an array or a list,
    /// the size of the current tag.</returns>
    /// <exception cref="EndOfStreamException"><paramref name="includeValue"/> is <see langword="true"/> and
    /// the end of the stream has been reached.</exception>
    /// <exception cref="InvalidOperationException"><paramref name="includeValue"/> is
    /// <see langword="true"/> and the value of the current tag has already been read.</exception>
    /// <remarks>
    /// The tag is indented with <see cref="NbtTag.DefaultIndentString"/>. Unless
    /// <see cref="CacheTagValues"/> is <see langword="true"/>, the value of a tag can be read only once, and
    /// this method reads it.
    /// </remarks>
    public string ToString(bool includeValue)
    {
        return ToString(includeValue, NbtTag.DefaultIndentString);
    }


    /// <summary>
    /// Returns a string that represents the tag that the reader is positioned on, using the specified
    /// indentation.
    /// </summary>
    /// <param name="includeValue"><see langword="true"/> to read and include the value of the current tag;
    /// otherwise, <see langword="false"/>.</param>
    /// <param name="indentString">The string used for one level of indentation. Can be an empty string, but
    /// cannot be <see langword="null"/>.</param>
    /// <returns>A string that contains the depth, ordinal number, type, name, and, for an array or a list,
    /// the size of the current tag.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="indentString"/> is
    /// <see langword="null"/>.</exception>
    /// <exception cref="EndOfStreamException"><paramref name="includeValue"/> is <see langword="true"/> and
    /// the end of the stream has been reached.</exception>
    /// <exception cref="InvalidOperationException"><paramref name="includeValue"/> is
    /// <see langword="true"/> and the value of the current tag has already been read.</exception>
    /// <remarks>
    /// Unless <see cref="CacheTagValues"/> is <see langword="true"/>, the value of a tag can be read only
    /// once, and this method reads it.
    /// </remarks>
    public string ToString(bool includeValue, string indentString)
    {
        if (indentString == null) throw new ArgumentNullException(nameof(indentString));
        var sb = new StringBuilder();
        for (var i = 0; i < Depth; i++) sb.Append(indentString);
        sb.Append('#').Append(TagsRead).Append(". ").Append(TagType);
        if (IsList) sb.Append('<').Append(ListType).Append('>');
        if (HasLength) sb.Append('[').Append(TagLength).Append(']');
        sb.Append(' ').Append(TagName);
        if (includeValue && (_atValue || (HasValue && _cacheTagValues)) && TagType != NbtTagType.IntArray &&
            TagType != NbtTagType.ByteArray && TagType != NbtTagType.LongArray)
            sb.Append(" = ").Append(ReadValue());
        return sb.ToString();
    }
}