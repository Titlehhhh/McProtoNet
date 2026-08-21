using System.Text;

namespace McProtoNet.NBT;

/// <summary>
///     Represents a reader that provides fast, non-cached, forward-only access to NBT data.
///     Each instance of NbtReader reads one complete file.
/// </summary>
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
    ///     Opens a reader over a stream of Java Edition NBT — big-endian numbers, modified UTF-8
    ///     strings. There is no little-endian (Bedrock) mode.
    /// </summary>
    /// <param name="stream"> Stream to read from. </param>
    /// <param name="readRootName">
    ///     True (the default) expects a named root, as NBT files carry it. False expects the nameless
    ///     root of the network format used since 1.20.2.
    /// </param>
    /// <exception cref="ArgumentNullException"> <paramref name="stream" /> is <c>null</c>. </exception>
    /// <exception cref="ArgumentException"> <paramref name="stream" /> is not readable. </exception>
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
    ///     Reads one complete tag from a stream and builds the tag tree. The root may be of any
    ///     type — since 1.20.3 a network root can be a TAG_String. Returns null when the first
    ///     byte is TAG_End.
    /// </summary>
    /// <param name="stream"> Stream positioned at the tag type byte. </param>
    /// <param name="readRootName">
    ///     True (the default) reads the root tag's name after the type byte. False expects the
    ///     nameless root of the network format.
    /// </param>
    /// <exception cref="NbtFormatException"> The data is malformed, truncated, or too deeply nested. </exception>
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
    ///     Gets the name of the root tag of this NBT stream.
    /// </summary>
    public string? RootName { get; private set; }

    /// <summary>
    ///     Gets the name of the parent tag. May be null (for root tags and descendants of list elements).
    /// </summary>
    public string? ParentName { get; private set; }

    /// <summary>
    ///     Gets the name of the current tag. May be null (for list elements and end tags).
    /// </summary>
    public string? TagName { get; private set; }

    /// <summary>
    ///     Gets the type of the parent tag. Returns TAG_End when there is no parent tag.
    /// </summary>
    public NbtTagType ParentTagType { get; private set; }

    /// <summary>
    ///     Gets the type of the current tag.
    /// </summary>
    public NbtTagType TagType { get; private set; }

    /// <summary>
    ///     Whether tag that we are currently on is a list element.
    /// </summary>
    public bool IsListElement => ParentTagType == NbtTagType.List;

    /// <summary>
    ///     Whether current tag has a value to read.
    /// </summary>
    public bool HasValue =>
        TagType is not (NbtTagType.Compound or NbtTagType.End or NbtTagType.List);

    /// <summary>
    ///     Whether current tag has a name.
    /// </summary>
    public bool HasName => TagName != null;

    /// <summary>
    ///     Whether this reader has reached the end of stream.
    /// </summary>
    public bool IsAtStreamEnd => _state == NbtParseState.AtStreamEnd;

    /// <summary>
    ///     Whether the current tag is a Compound.
    /// </summary>
    public bool IsCompound => TagType == NbtTagType.Compound;

    /// <summary>
    ///     Whether the current tag is a List.
    /// </summary>
    public bool IsList => TagType == NbtTagType.List;

    /// <summary>
    ///     Whether the current tag has length (Lists, ByteArrays, and IntArrays have length).
    ///     Compound tags also have length, technically, but it is not known until all child tags are read.
    /// </summary>
    public bool HasLength =>
        TagType is NbtTagType.List or NbtTagType.ByteArray or NbtTagType.IntArray or NbtTagType.LongArray;

    /// <summary>
    ///     Gets the Stream from which data is being read.
    /// </summary>
    public Stream BaseStream => _reader.BaseStream;

    /// <summary>
    ///     Gets the number of bytes from the beginning of the stream to the beginning of this tag.
    ///     If the stream is not seekable, this value will always be 0.
    /// </summary>
    public int TagStartOffset { get; private set; }

    /// <summary>
    ///     Gets the number of tags read from the stream so far
    ///     (including the current tag and all skipped tags).
    ///     If <c>SkipEndTags</c> is <c>false</c>, all end tags are also counted.
    /// </summary>
    public int TagsRead { get; private set; }

    /// <summary>
    ///     Gets the depth of the current tag in the hierarchy.
    ///     <c>RootTag</c> is at depth 1, its descendant tags are 2, etc.
    /// </summary>
    public int Depth { get; private set; }

    /// <summary>
    ///     If the current tag is TAG_List, returns type of the list elements.
    /// </summary>
    public NbtTagType ListType { get; private set; }

    /// <summary>
    ///     If the current tag is TAG_List, TAG_Byte_Array, or TAG_Int_Array, returns the number of elements.
    /// </summary>
    public int TagLength { get; private set; }

    /// <summary>
    ///     If the parent tag is TAG_List, returns the number of elements.
    /// </summary>
    public int ParentTagLength { get; private set; }

    /// <summary>
    ///     If the parent tag is TAG_List, returns index of the current tag.
    /// </summary>
    public int ListIndex { get; private set; }

    /// <summary>
    ///     Gets whether this NbtReader instance is in state of error.
    ///     No further reading can be done from this instance if a parse error occurred.
    /// </summary>
    public bool IsInErrorState => _state == NbtParseState.Error;

    /// <summary>
    ///     Parsing option: Whether NbtReader should skip End tags in ReadToFollowing() automatically while parsing.
    ///     Default is <c>true</c>.
    /// </summary>
    public bool SkipEndTags { get; set; }

    /// <summary>
    ///     Parsing option: Whether NbtReader should save a copy of the most recently read tag's value.
    ///     Unless CacheTagValues is <c>true</c>, tag values can only be read once. Default is <c>false</c>.
    /// </summary>
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
    ///     Reads the next tag from the stream.
    /// </summary>
    /// <returns> true if the next tag was read successfully; false if there are no more tags to read. </returns>
    /// <exception cref="NbtFormatException"> If an error occurred while parsing data in NBT format. </exception>
    /// <exception cref="InvalidReaderStateException"> If NbtReader cannot recover from a previous parsing error. </exception>
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

    /// <summary>
    ///     Goes one step down the NBT file's hierarchy, preserving current state
    /// </summary>
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

    /// <summary>
    ///     Goes one step up the NBT file's hierarchy, restoring previous state
    /// </summary>
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
    ///     Reads until a tag with the specified name is found.
    ///     Returns false if are no more tags to read (end of stream is reached).
    /// </summary>
    /// <param name="tagName"> Name of the tag. May be null (to look for next unnamed tag). </param>
    /// <returns> <c>true</c> if a matching tag is found; otherwise <c>false</c>. </returns>
    /// <exception cref="NbtFormatException"> If an error occurred while parsing data in NBT format. </exception>
    /// <exception cref="InvalidOperationException"> If NbtReader cannot recover from a previous parsing error. </exception>
    public bool ReadToFollowing(string? tagName)
    {
        while (ReadToFollowing())
            if (TagName == tagName)
                return true;
        return false;
    }

    /// <summary>
    ///     Advances the NbtReader to the next descendant tag with the specified name.
    ///     If a matching child tag is not found, the NbtReader is positioned on the end tag.
    /// </summary>
    /// <param name="tagName"> Name of the tag you wish to move to. May be null (to look for next unnamed tag). </param>
    /// <returns> <c>true</c> if a matching descendant tag is found; otherwise <c>false</c>. </returns>
    /// <exception cref="NbtFormatException"> If an error occurred while parsing data in NBT format. </exception>
    /// <exception cref="InvalidReaderStateException"> If NbtReader cannot recover from a previous parsing error. </exception>
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
    ///     Advances the NbtReader to the next sibling tag, skipping any child tags.
    ///     If there are no more siblings, NbtReader is positioned on the tag following the last of this tag's descendants.
    /// </summary>
    /// <returns> <c>true</c> if a sibling element is found; otherwise <c>false</c>. </returns>
    /// <exception cref="NbtFormatException"> If an error occurred while parsing data in NBT format. </exception>
    /// <exception cref="InvalidReaderStateException"> If NbtReader cannot recover from a previous parsing error. </exception>
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
    ///     Advances the NbtReader to the next sibling tag with the specified name.
    ///     If a matching sibling tag is not found, NbtReader is positioned on the tag following the last siblings.
    /// </summary>
    /// <param name="tagName"> The name of the sibling tag you wish to move to. </param>
    /// <returns> <c>true</c> if a matching sibling element is found; otherwise <c>false</c>. </returns>
    /// <exception cref="NbtFormatException"> If an error occurred while parsing data in NBT format. </exception>
    /// <exception cref="InvalidOperationException"> If NbtReader cannot recover from a previous parsing error. </exception>
    public bool ReadToNextSibling(string? tagName)
    {
        while (ReadToNextSibling())
            if (TagName == tagName)
                return true;
        return false;
    }

    /// <summary>
    ///     Skips current tag, its value/descendants, and any following siblings.
    ///     In other words, reads until parent tag's sibling.
    /// </summary>
    /// <returns> Total number of tags that were skipped. Returns 0 if end of the stream is reached. </returns>
    /// <exception cref="NbtFormatException"> If an error occurred while parsing data in NBT format. </exception>
    /// <exception cref="InvalidReaderStateException"> If NbtReader cannot recover from a previous parsing error. </exception>
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
    ///     Reads the entirety of the current tag, including any descendants,
    ///     and constructs an NbtTag object of the appropriate type.
    /// </summary>
    /// <returns>
    ///     Constructed NbtTag object;
    ///     <c>null</c> if <c>SkipEndTags</c> is <c>true</c> and trying to read an End tag.
    /// </returns>
    /// <exception cref="NbtFormatException"> If an error occurred while parsing data in NBT format. </exception>
    /// <exception cref="InvalidReaderStateException"> If NbtReader cannot recover from a previous parsing error. </exception>
    /// <exception cref="EndOfStreamException"> End of stream has been reached (no more tags can be read). </exception>
    /// <exception cref="InvalidOperationException"> Tag value has already been read, and CacheTagValues is false. </exception>
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
    ///     Reads the value as an object of the type specified.
    /// </summary>
    /// <typeparam name="T">
    ///     The type of the value to be returned.
    ///     Tag value should be convertible to this type.
    /// </typeparam>
    /// <returns> Tag value converted to the requested type. </returns>
    /// <exception cref="EndOfStreamException"> End of stream has been reached (no more tags can be read). </exception>
    /// <exception cref="NbtFormatException"> If an error occurred while parsing data in NBT format. </exception>
    /// <exception cref="InvalidOperationException"> Value has already been read, or there is no value to read. </exception>
    /// <exception cref="InvalidReaderStateException"> If NbtReader cannot recover from a previous parsing error. </exception>
    /// <exception cref="InvalidCastException"> Tag value cannot be converted to the requested type. </exception>
    public T ReadValueAs<T>()
    {
        return (T)ReadValue();
    }

    /// <summary>
    ///     Reads the value as an object of the correct type, boxed.
    ///     Cannot be called for tags that do not have a single-object value (compound, list, and end tags).
    /// </summary>
    /// <returns> Tag value converted to the requested type. </returns>
    /// <exception cref="EndOfStreamException"> End of stream has been reached (no more tags can be read). </exception>
    /// <exception cref="NbtFormatException"> If an error occurred while parsing data in NBT format. </exception>
    /// <exception cref="InvalidOperationException"> Value has already been read, or there is no value to read. </exception>
    /// <exception cref="InvalidReaderStateException"> If NbtReader cannot recover from a previous parsing error. </exception>
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
    ///     If the current tag is a List, reads all elements of this list as an array.
    ///     If any tags/values have already been read from this list, only reads the remaining unread tags/values.
    ///     ListType must be a value type (byte, short, int, long, float, double, or string).
    ///     Stops reading after the last list element.
    /// </summary>
    /// <typeparam name="T">
    ///     Element type of the array to be returned.
    ///     Tag contents should be convertible to this type.
    /// </typeparam>
    /// <returns> List contents converted to an array of the requested type. </returns>
    /// <exception cref="EndOfStreamException"> End of stream has been reached (no more tags can be read). </exception>
    /// <exception cref="InvalidOperationException"> Current tag is not of type List. </exception>
    /// <exception cref="InvalidReaderStateException"> If NbtReader cannot recover from a previous parsing error. </exception>
    /// <exception cref="NbtFormatException"> If an error occurred while parsing data in NBT format. </exception>
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
    ///     Returns a String that represents the tag currently being read by this NbtReader instance.
    ///     Prints current tag's depth, ordinal number, type, name, and size (for arrays and lists). Does not print value.
    ///     Indents the tag according default indentation (NbtTag.DefaultIndentString).
    /// </summary>
    public override string ToString()
    {
        return ToString(false, NbtTag.DefaultIndentString);
    }

    /// <summary>
    ///     Returns a String that represents the tag currently being read by this NbtReader instance.
    ///     Prints current tag's depth, ordinal number, type, name, size (for arrays and lists), and optionally value.
    ///     Indents the tag according default indentation (NbtTag.DefaultIndentString).
    /// </summary>
    /// <param name="includeValue">
    ///     If set to <c>true</c>, also reads and prints the current tag's value.
    ///     Note that unless CacheTagValues is set to <c>true</c>, you can only read every tag's value ONCE.
    /// </param>
    public string ToString(bool includeValue)
    {
        return ToString(includeValue, NbtTag.DefaultIndentString);
    }


    /// <summary>
    ///     Returns a String that represents the current NbtReader object.
    ///     Prints current tag's depth, ordinal number, type, name, size (for arrays and lists), and optionally value.
    /// </summary>
    /// <param name="indentString"> String to be used for indentation. May be empty string, but may not be <c>null</c>. </param>
    /// <param name="includeValue"> If set to <c>true</c>, also reads and prints the current tag's value. </param>
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