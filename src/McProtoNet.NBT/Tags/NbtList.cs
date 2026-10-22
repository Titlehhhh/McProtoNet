using System.Collections;
using System.Text;

namespace McProtoNet.NBT;

/// <summary>
/// Represents an NBT tag that holds a list of unnamed tags of the same type.
/// </summary>
/// <remarks>
/// A tag added to this list becomes its child and cannot be added to another compound or list until it is removed.
/// </remarks>
public sealed class NbtList : NbtTag, IList<NbtTag>, IList
{
    private readonly List<NbtTag> _tags = new();

    private NbtTagType _listType;

    /// <summary>
    /// Initializes a new instance of the <see cref="NbtList"/> class that is unnamed and empty, with an element type
    /// of <see cref="NbtTagType.End"/>.
    /// </summary>
    public NbtList()
        : this(null!, null!, NbtTagType.End)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NbtList"/> class with the specified name that is empty, with an
    /// element type of <see cref="NbtTagType.End"/>.
    /// </summary>
    /// <param name="tagName">The name of the tag, or <see langword="null"/> for an unnamed tag.</param>
    public NbtList(string? tagName)
        : this(tagName, null, NbtTagType.End)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NbtList"/> class that is unnamed and contains the specified
    /// tags, with the element type inferred from them.
    /// </summary>
    /// <param name="tags">The tags to add. All tags must be unnamed and of the same type. The collection can be
    /// empty, in which case the element type stays <see cref="NbtTagType.End"/>.</param>
    /// <exception cref="ArgumentNullException"><paramref name="tags"/> is <see langword="null"/>.
    /// -or-
    /// One of the tags is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">The tags are of mixed types.
    /// -or-
    /// One of the tags is named or already belongs to another compound or list.</exception>
    public NbtList(IEnumerable<NbtTag> tags)
        : this(null, tags, NbtTagType.End)
    {
        // the base constructor will allow null "tags," but we don't want that in this constructor
        if (tags == null) throw new ArgumentNullException(nameof(tags));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NbtList"/> class that is unnamed and empty, with the specified
    /// element type.
    /// </summary>
    /// <param name="givenListType">The element type of the list. <see cref="NbtTagType.End"/> means that the
    /// element type is inferred from the first tag added.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="givenListType"/> is not a recognized tag
    /// type.</exception>
    public NbtList(NbtTagType givenListType)
        : this(null, null, givenListType)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NbtList"/> class with the specified name that contains the
    /// specified tags, with the element type inferred from them.
    /// </summary>
    /// <param name="tagName">The name of the tag, or <see langword="null"/> for an unnamed tag.</param>
    /// <param name="tags">The tags to add. All tags must be unnamed and of the same type. The collection can be
    /// empty, in which case the element type stays <see cref="NbtTagType.End"/>.</param>
    /// <exception cref="ArgumentNullException"><paramref name="tags"/> is <see langword="null"/>.
    /// -or-
    /// One of the tags is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">The tags are of mixed types.
    /// -or-
    /// One of the tags is named or already belongs to another compound or list.</exception>
    public NbtList(string? tagName, IEnumerable<NbtTag> tags)
        : this(tagName, tags, NbtTagType.End)
    {
        // the base constructor will allow null "tags," but we don't want that in this constructor
        if (tags == null) throw new ArgumentNullException(nameof(tags));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NbtList"/> class that is unnamed and contains the specified
    /// tags, with the specified element type.
    /// </summary>
    /// <param name="tags">The tags to add. All tags must be unnamed and must match
    /// <paramref name="givenListType"/>. The collection can be empty.</param>
    /// <param name="givenListType">The element type of the list. <see cref="NbtTagType.End"/> means that the
    /// element type is inferred from the first tag added.</param>
    /// <exception cref="ArgumentNullException"><paramref name="tags"/> is <see langword="null"/>.
    /// -or-
    /// One of the tags is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="givenListType"/> is not a recognized tag
    /// type.</exception>
    /// <exception cref="ArgumentException">The tags do not match <paramref name="givenListType"/> or are of mixed
    /// types.
    /// -or-
    /// One of the tags is named or already belongs to another compound or list.</exception>
    public NbtList(IEnumerable<NbtTag> tags, NbtTagType givenListType)
        : this(null, tags, givenListType)
    {
        // the base constructor will allow null "tags," but we don't want that in this constructor
        if (tags == null) throw new ArgumentNullException(nameof(tags));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NbtList"/> class with the specified name that is empty, with the
    /// specified element type.
    /// </summary>
    /// <param name="tagName">The name of the tag, or <see langword="null"/> for an unnamed tag.</param>
    /// <param name="givenListType">The element type of the list. <see cref="NbtTagType.End"/> means that the
    /// element type is inferred from the first tag added.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="givenListType"/> is not a recognized tag
    /// type.</exception>
    public NbtList(string? tagName, NbtTagType givenListType)
        : this(tagName, null, givenListType)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NbtList"/> class with the specified name and contents, with the
    /// specified element type.
    /// </summary>
    /// <param name="tagName">The name of the tag, or <see langword="null"/> for an unnamed tag.</param>
    /// <param name="tags">The tags to add. All tags must be unnamed and must match
    /// <paramref name="givenListType"/>. The collection can be empty or <see langword="null"/>.</param>
    /// <param name="givenListType">The element type of the list. <see cref="NbtTagType.End"/> means that the
    /// element type is inferred from the first tag added.</param>
    /// <exception cref="ArgumentNullException">One of the tags is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="givenListType"/> is not a recognized tag
    /// type.</exception>
    /// <exception cref="ArgumentException">The tags do not match <paramref name="givenListType"/> or are of mixed
    /// types.
    /// -or-
    /// One of the tags is named or already belongs to another compound or list.</exception>
    public NbtList(string? tagName, IEnumerable<NbtTag>? tags, NbtTagType givenListType)
    {
        Name = tagName;
        ListType = givenListType;

        if (tags == null) return;
        foreach (var tag in tags) Add(tag);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NbtList"/> class that is a deep copy of the specified tag.
    /// </summary>
    /// <param name="other">The tag to copy. The name, the element type and a clone of every child tag are
    /// copied.</param>
    /// <exception cref="ArgumentNullException"><paramref name="other"/> is <see langword="null"/>.</exception>
    public NbtList(NbtList other)
    {
        if (other == null) throw new ArgumentNullException(nameof(other));
        Name = other.Name;
        _listType = other._listType;
        foreach (var tag in other._tags) _tags.Add((NbtTag)tag.Clone());
    }

    /// <summary>
    /// Gets the type of this tag, which is always <see cref="NbtTagType.List"/>.
    /// </summary>
    public override NbtTagType TagType => NbtTagType.List;

    /// <summary>
    /// Gets or sets the element type of this list. All tags in the list are of this type.
    /// </summary>
    /// <exception cref="ArgumentException">The property is set to <see cref="NbtTagType.End"/> while the list is not
    /// empty.
    /// -or-
    /// The property is set to a type that does not match the type of the elements already in the list.</exception>
    /// <exception cref="ArgumentOutOfRangeException">The property is set to a value that is not a recognized tag
    /// type.</exception>
    /// <remarks>
    /// <see cref="NbtTagType.End"/> means that the element type is inferred from the first tag added.
    /// </remarks>
    public NbtTagType ListType
    {
        get => _listType;
        set
        {
            if (value == NbtTagType.End)
            {
                if (_tags.Count > 0)
                    throw new ArgumentException("Only an empty list may have an element type of TAG_End.");
            }
            else if (value > NbtTagType.LongArray)
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }

            if (_tags.Count > 0)
            {
                var actualType = _tags[0].TagType;
                // We can safely assume that ALL tags have the same TagType as the first tag.
                if (actualType != value)
                {
                    var msg = $"Given NbtTagType ({value}) does not match actual element type ({actualType})";
                    throw new ArgumentException(msg);
                }
            }

            _listType = value;
        }
    }

    /// <summary>
    /// Gets or sets the tag at the specified index.
    /// </summary>
    /// <param name="tagIndex">The zero-based index of the tag to get or set.</param>
    /// <returns>The tag at the specified index.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="tagIndex"/> is not a valid index in the
    /// list.</exception>
    /// <exception cref="ArgumentNullException">The property is set to <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">The tag being set already belongs to another compound or list.
    /// -or-
    /// The tag being set is this list or its parent.
    /// -or-
    /// The tag being set is named.
    /// -or-
    /// The type of the tag being set does not match <see cref="ListType"/>.</exception>
    public override NbtTag this[int tagIndex]
    {
        get => _tags[tagIndex];
        set
        {
            if (value == null) throw new ArgumentNullException(nameof(value));

            if (value.Parent != null)
                throw new ArgumentException("A tag may only be added to one compound/list at a time.");

            if (value == this || value == Parent)
                throw new ArgumentException("A list tag may not be added to itself or to its child tag.");

            if (value.Name != null)
                throw new ArgumentException("Named tag given. A list may only contain unnamed tags.");

            if (_listType != NbtTagType.End && value.TagType != _listType)
                throw new ArgumentException("Items must be of type " + _listType);
            _tags[tagIndex] = value;
            value.Parent = this;
        }
    }

    /// <summary>
    /// Gets the tag at the specified index, cast to the specified type.
    /// </summary>
    /// <typeparam name="T">The type to cast the tag to.</typeparam>
    /// <param name="tagIndex">The zero-based index of the tag to get.</param>
    /// <returns>The tag at the specified index.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="tagIndex"/> is not a valid index in the
    /// list.</exception>
    /// <exception cref="InvalidCastException">The tag cannot be cast to <typeparamref name="T"/>.</exception>
    public T Get<T>(int tagIndex) where T : NbtTag
    {
        return (T)_tags[tagIndex];
    }

    /// <summary>
    /// Adds all tags of the specified collection to the end of this list.
    /// </summary>
    /// <param name="newTags">The tags to add. All tags must be unnamed and must match
    /// <see cref="ListType"/>.</param>
    /// <exception cref="ArgumentNullException"><paramref name="newTags"/> is <see langword="null"/>.
    /// -or-
    /// One of the tags is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">The tags do not match <see cref="ListType"/> or are of mixed types.
    /// -or-
    /// One of the tags is named or already belongs to another compound or list.</exception>
    public void AddRange(IEnumerable<NbtTag> newTags)
    {
        if (newTags == null) throw new ArgumentNullException(nameof(newTags));
        foreach (var tag in newTags) Add(tag);
    }

    /// <summary>
    /// Copies all tags in this list to a new array.
    /// </summary>
    /// <returns>An array that contains the tags of this list.</returns>

    // ReSharper disable ReturnTypeCanBeEnumerable.Global
    public NbtTag[] ToArray()
    {
        // ReSharper restore ReturnTypeCanBeEnumerable.Global
        return _tags.ToArray();
    }

    /// <summary>
    /// Copies all tags in this list to a new array of the specified type.
    /// </summary>
    /// <typeparam name="T">The type to cast every tag to.</typeparam>
    /// <returns>An array that contains the tags of this list, cast to <typeparamref name="T"/>.</returns>
    /// <exception cref="InvalidCastException">A tag in this list cannot be cast to
    /// <typeparamref name="T"/>.</exception>
    public T[] ToArray<T>() where T : NbtTag
    {
        var result = new T[_tags.Count];
        for (var i = 0; i < result.Length; i++) result[i] = (T)_tags[i];
        return result;
    }

    /// <inheritdoc />
    public override object Clone()
    {
        return new NbtList(this);
    }

    internal override void PrettyPrint(StringBuilder sb, string indentString, int indentLevel)
    {
        for (var i = 0; i < indentLevel; i++) sb.Append(indentString);
        sb.Append("TAG_List");
        if (!string.IsNullOrEmpty(Name)) sb.AppendFormat("(\"{0}\")", Name);
        sb.AppendFormat(": {0} entries {{", _tags.Count);

        if (Count > 0)
        {
            sb.Append('\n');
            foreach (var tag in _tags)
            {
                tag.PrettyPrint(sb, indentString, indentLevel + 1);
                sb.Append('\n');
            }

            for (var i = 0; i < indentLevel; i++) sb.Append(indentString);
        }

        sb.Append('}');
    }

    #region Reading / Writing

    internal override void ReadTag(NbtBinaryReader readStream)
    {
        readStream.EnterLevel();
        ListType = readStream.ReadTagType();

        var length = readStream.ReadInt32();
        if (length < 0) throw new NbtFormatException("Negative list size given.");
        if (length > 0 && ListType == NbtTagType.End)
            throw new NbtFormatException("Non-empty NBT list of TAG_End elements.");

        for (var i = 0; i < length; i++)
        {
            var element = CreateTag(ListType);
            element.Parent = this;
            element.ReadTag(readStream);
            _tags.Add(element);
        }

        readStream.ExitLevel();
    }

    internal override void WriteTag(NbtBinaryWriter writeStream)
    {
        writeStream.Write(NbtTagType.List);
        if (Name == null) throw new NbtFormatException("Name is null");
        writeStream.Write(Name);
        WriteData(writeStream);
    }

    internal override void WriteData(NbtBinaryWriter writeStream)
    {
        writeStream.EnterLevel();
        writeStream.Write(ListType);
        writeStream.Write(_tags.Count);
        foreach (var tag in _tags) tag.WriteData(writeStream);
        writeStream.ExitLevel();
    }

    #endregion

    #region Implementation of IEnumerable<NBtTag> and IEnumerable

    /// <summary>
    /// Returns an enumerator that iterates through all tags in this list.
    /// </summary>
    /// <returns>An enumerator for the tags in this list.</returns>
    public IEnumerator<NbtTag> GetEnumerator()
    {
        return _tags.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _tags.GetEnumerator();
    }

    #endregion

    #region Implementation of IList<NbtTag> and ICollection<NbtTag>

    /// <summary>
    /// Determines the index of the specified tag in this list.
    /// </summary>
    /// <param name="tag">The tag to locate.</param>
    /// <returns>The zero-based index of the tag if it was found; otherwise, -1. This method returns -1 if
    /// <paramref name="tag"/> is <see langword="null"/>.</returns>
    public int IndexOf(NbtTag? tag)
    {
        if (tag == null) return -1;
        return _tags.IndexOf(tag);
    }

    /// <summary>
    /// Inserts a tag into this list at the specified index.
    /// </summary>
    /// <param name="tagIndex">The zero-based index at which the tag is inserted.</param>
    /// <param name="newTag">The tag to insert. It must match <see cref="ListType"/> and must have no parent.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="tagIndex"/> is not a valid index in the
    /// list.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="newTag"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">The type of <paramref name="newTag"/> does not match
    /// <see cref="ListType"/>.
    /// -or-
    /// <paramref name="newTag"/> already belongs to another compound or list.</exception>
    /// <remarks>
    /// If <see cref="ListType"/> is <see cref="NbtTagType.End"/>, it is set to the type of
    /// <paramref name="newTag"/>.
    /// </remarks>
    public void Insert(int tagIndex, NbtTag newTag)
    {
        if (newTag == null) throw new ArgumentNullException(nameof(newTag));

        if (_listType != NbtTagType.End && newTag.TagType != _listType)
            throw new ArgumentException("Items must be of type " + _listType);

        if (newTag.Parent != null)
            throw new ArgumentException("A tag may only be added to one compound/list at a time.");

        _tags.Insert(tagIndex, newTag);
        if (_listType == NbtTagType.End) _listType = newTag.TagType;
        newTag.Parent = this;
    }

    /// <summary>
    /// Removes the tag at the specified index from this list.
    /// </summary>
    /// <param name="index">The zero-based index of the tag to remove.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is not a valid index in the
    /// list.</exception>
    /// <remarks>
    /// The parent of the removed tag is set to <see langword="null"/>.
    /// </remarks>
    public void RemoveAt(int index)
    {
        var tag = this[index];
        _tags.RemoveAt(index);
        tag.Parent = null;
    }

    /// <summary>
    /// Adds a tag to the end of this list.
    /// </summary>
    /// <param name="newTag">The tag to add. It must be unnamed, must match <see cref="ListType"/> and must have no
    /// parent.</param>
    /// <exception cref="ArgumentNullException"><paramref name="newTag"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="newTag"/> already belongs to another compound or list.
    /// -or-
    /// <paramref name="newTag"/> is this list or its parent.
    /// -or-
    /// <paramref name="newTag"/> is named.
    /// -or-
    /// The type of <paramref name="newTag"/> does not match <see cref="ListType"/>.</exception>
    /// <remarks>
    /// The added tag becomes a child of this list. If <see cref="ListType"/> is <see cref="NbtTagType.End"/>, it is
    /// set to the type of <paramref name="newTag"/>.
    /// </remarks>
    public void Add(NbtTag newTag)
    {
        if (newTag == null) throw new ArgumentNullException(nameof(newTag));

        if (newTag.Parent != null)
            throw new ArgumentException("A tag may only be added to one compound/list at a time.");

        if (newTag == this || newTag == Parent)
            throw new ArgumentException("A list tag may not be added to itself or to its child tag.");

        if (newTag.Name != null) throw new ArgumentException("Named tag given. A list may only contain unnamed tags.");

        if (_listType != NbtTagType.End && newTag.TagType != _listType)
            throw new ArgumentException("Items in this list must be of type " + _listType + ". Given type: " +
                                        newTag.TagType);

        _tags.Add(newTag);
        newTag.Parent = this;
        if (_listType == NbtTagType.End) _listType = newTag.TagType;
    }

    /// <summary>
    /// Removes all tags from this list.
    /// </summary>
    /// <remarks>
    /// The parent of every removed tag is set to <see langword="null"/>. <see cref="ListType"/> is not changed.
    /// </remarks>
    public void Clear()
    {
        foreach (var t in _tags) t.Parent = null;

        _tags.Clear();
    }

    /// <summary>
    /// Determines whether this list contains the specified tag.
    /// </summary>
    /// <param name="item">The tag to locate. This value can be <see langword="null"/>.</param>
    /// <returns><see langword="true"/> if the tag was found; otherwise, <see langword="false"/>.</returns>
    public bool Contains(NbtTag? item)
    {
        return _tags.Contains(item!);
    }

    /// <summary>
    /// Copies the tags of this list to an array, starting at the specified index.
    /// </summary>
    /// <param name="array">The one-dimensional array that is the destination of the copied tags. The array must
    /// have zero-based indexing.</param>
    /// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
    /// <exception cref="ArgumentNullException"><paramref name="array"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="arrayIndex"/> is less than zero.</exception>
    /// <exception cref="ArgumentException">The number of tags in this list is greater than the available space from
    /// <paramref name="arrayIndex"/> to the end of <paramref name="array"/>.</exception>
    public void CopyTo(NbtTag[] array, int arrayIndex)
    {
        _tags.CopyTo(array, arrayIndex);
    }

    /// <summary>
    /// Removes the first occurrence of the specified tag from this list.
    /// </summary>
    /// <param name="tag">The tag to remove.</param>
    /// <returns><see langword="true"/> if the tag was found and removed; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="tag"/> is <see langword="null"/>.</exception>
    /// <remarks>
    /// The parent of the removed tag is set to <see langword="null"/>.
    /// </remarks>
    public bool Remove(NbtTag tag)
    {
        if (tag == null) throw new ArgumentNullException(nameof(tag));
        if (!_tags.Remove(tag)) return false;
        tag.Parent = null;
        return true;
    }

    /// <summary>
    /// Gets the number of tags contained in this list.
    /// </summary>
    public int Count => _tags.Count;

    bool ICollection<NbtTag>.IsReadOnly => false;

    #endregion

    #region Implementation of IList and ICollection

    void IList.Remove(object? value)
    {
        Remove((NbtTag)value!);
    }

    object? IList.this[int tagIndex]
    {
        get => _tags[tagIndex];
        set => this[tagIndex] = (NbtTag)value!;
    }

    int IList.Add(object? value)
    {
        Add((NbtTag)value!);
        return _tags.Count - 1;
    }

    bool IList.Contains(object? value)
    {
        return _tags.Contains((NbtTag)value!);
    }

    int IList.IndexOf(object? value)
    {
        return _tags.IndexOf((NbtTag)value!);
    }

    void IList.Insert(int index, object? value)
    {
        Insert(index, (NbtTag)value!);
    }

    bool IList.IsFixedSize => false;

    void ICollection.CopyTo(Array array, int index)
    {
        CopyTo((NbtTag[])array, index);
    }

    object ICollection.SyncRoot => (_tags as ICollection).SyncRoot;

    bool ICollection.IsSynchronized => false;

    bool IList.IsReadOnly => false;

    #endregion
}
