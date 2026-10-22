using System.Collections;
using System.Diagnostics;
using System.Text;

namespace McProtoNet.NBT;

/// <summary>
/// Represents an NBT tag that holds a set of named tags.
/// </summary>
/// <remarks>
/// The order in which the tags are enumerated is not guaranteed. A tag added to this compound becomes its child and
/// cannot be added to another compound or list until it is removed.
/// </remarks>
public sealed class NbtCompound : NbtTag, ICollection<NbtTag>, ICollection
{
    private readonly Dictionary<string, NbtTag> _tags = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="NbtCompound"/> class that is unnamed and empty.
    /// </summary>
    public NbtCompound()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NbtCompound"/> class with the specified name and no child tags.
    /// </summary>
    /// <param name="tagName">The name of the tag, or <see langword="null"/> for an unnamed tag.</param>
    public NbtCompound(string? tagName)
    {
        Name = tagName;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NbtCompound"/> class that is unnamed and contains the specified
    /// tags.
    /// </summary>
    /// <param name="tags">The tags to add. Every tag must be named and must have no parent.</param>
    /// <exception cref="ArgumentNullException"><paramref name="tags"/> is <see langword="null"/>.
    /// -or-
    /// One of the tags is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">One of the tags is unnamed.
    /// -or-
    /// Two tags have the same name.
    /// -or-
    /// One of the tags already belongs to another compound or list.</exception>
    public NbtCompound(IEnumerable<NbtTag> tags)
        : this(null!, tags)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NbtCompound"/> class with the specified name that contains the
    /// specified tags.
    /// </summary>
    /// <param name="tagName">The name of the tag, or <see langword="null"/> for an unnamed tag.</param>
    /// <param name="tags">The tags to add. Every tag must be named and must have no parent.</param>
    /// <exception cref="ArgumentNullException"><paramref name="tags"/> is <see langword="null"/>.
    /// -or-
    /// One of the tags is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">One of the tags is unnamed.
    /// -or-
    /// Two tags have the same name.
    /// -or-
    /// One of the tags already belongs to another compound or list.</exception>
    public NbtCompound(string? tagName, IEnumerable<NbtTag> tags)
    {
        if (tags == null) throw new ArgumentNullException(nameof(tags));
        Name = tagName;
        foreach (var tag in tags) Add(tag);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NbtCompound"/> class that is a deep copy of the specified tag.
    /// </summary>
    /// <param name="other">The tag to copy. The name and a clone of every child tag are copied.</param>
    /// <exception cref="ArgumentNullException"><paramref name="other"/> is <see langword="null"/>.</exception>
    public NbtCompound(NbtCompound other)
    {
        if (other == null) throw new ArgumentNullException(nameof(other));
        Name = other.Name;
        foreach (var tag in other._tags.Values) Add((NbtTag)tag.Clone());
    }

    /// <summary>
    /// Gets the type of this tag, which is always <see cref="NbtTagType.Compound"/>.
    /// </summary>
    public override NbtTagType TagType => NbtTagType.Compound;

    /// <summary>
    /// Gets or sets the tag with the specified name.
    /// </summary>
    /// <param name="tagName">The name of the tag to get or set. It must match the name of the tag being set.</param>
    /// <returns>The tag with the specified name, or <see langword="null"/> if no tag with that name is
    /// present.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="tagName"/> is <see langword="null"/>.
    /// -or-
    /// The property is set to <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="tagName"/> does not match the name of the tag being set.
    /// -or-
    /// The tag being set already belongs to another compound or list.
    /// -or-
    /// The tag being set is this compound.</exception>
    /// <remarks>
    /// The setter replaces any tag already stored under the specified name.
    /// </remarks>
    public override NbtTag? this[string tagName]
    {
        get => Get<NbtTag>(tagName);
        set
        {
            if (tagName == null) throw new ArgumentNullException(nameof(tagName));

            if (value == null) throw new ArgumentNullException(nameof(value));
            if (value.Name != tagName) throw new ArgumentException("Given tag name must match tag's actual name.");
            if (value.Parent != null)
                throw new ArgumentException("A tag may only be added to one compound/list at a time.");
            if (value == this) throw new ArgumentException("Cannot add tag to itself");

            _tags[tagName] = value;
            value.Parent = this;
        }
    }

    /// <summary>
    /// Gets a collection that contains the names of all tags in this compound.
    /// </summary>
    public IEnumerable<string> Names => _tags.Keys;

    /// <summary>
    /// Gets a collection that contains all tags in this compound.
    /// </summary>
    public IEnumerable<NbtTag> Tags => _tags.Values;

    /// <summary>
    /// Gets the tag with the specified name, cast to the specified type.
    /// </summary>
    /// <typeparam name="T">The type to cast the tag to.</typeparam>
    /// <param name="tagName">The name of the tag to get.</param>
    /// <returns>The tag with the specified name, or <see langword="null"/> if no tag with that name is
    /// present.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="tagName"/> is <see langword="null"/>.</exception>
    /// <exception cref="InvalidCastException">The tag cannot be cast to <typeparamref name="T"/>.</exception>
    public T? Get<T>(string tagName) where T : NbtTag
    {
        if (tagName == null) throw new ArgumentNullException(nameof(tagName));
        if (_tags.TryGetValue(tagName, out var result)) return (T)result;
        return null;
    }

    /// <summary>
    /// Gets the tag with the specified name.
    /// </summary>
    /// <param name="tagName">The name of the tag to get.</param>
    /// <returns>The tag with the specified name, or <see langword="null"/> if no tag with that name is
    /// present.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="tagName"/> is <see langword="null"/>.</exception>
    public NbtTag? Get(string tagName)
    {
        if (tagName == null) throw new ArgumentNullException(nameof(tagName));
        return _tags.TryGetValue(tagName, out var result) ? result : null;
    }

    /// <summary>
    /// Attempts to get the tag with the specified name, cast to the specified type.
    /// </summary>
    /// <typeparam name="T">The type to cast the tag to.</typeparam>
    /// <param name="tagName">The name of the tag to get.</param>
    /// <param name="result">When this method returns, contains the tag with the specified name, if it was found;
    /// otherwise, <see langword="null"/>. This parameter is passed uninitialized.</param>
    /// <returns><see langword="true"/> if a tag with the specified name was found; otherwise,
    /// <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="tagName"/> is <see langword="null"/>.</exception>
    /// <exception cref="InvalidCastException">The tag cannot be cast to <typeparamref name="T"/>.</exception>
    public bool TryGet<T>(string tagName, out T result) where T : NbtTag
    {
        if (tagName == null) throw new ArgumentNullException(nameof(tagName));
        if (_tags.TryGetValue(tagName, out var tempResult))
        {
            result = (T)tempResult;
            return true;
        }

        result = null!;
        return false;
    }

    /// <summary>
    /// Attempts to get the tag with the specified name.
    /// </summary>
    /// <param name="tagName">The name of the tag to get.</param>
    /// <param name="result">When this method returns, contains the tag with the specified name, if it was found;
    /// otherwise, <see langword="null"/>. This parameter is passed uninitialized.</param>
    /// <returns><see langword="true"/> if a tag with the specified name was found; otherwise,
    /// <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="tagName"/> is <see langword="null"/>.</exception>
    public bool TryGet(string tagName, out NbtTag result)
    {
        if (tagName == null) throw new ArgumentNullException(nameof(tagName));
        if (_tags.TryGetValue(tagName, out var tempResult))
        {
            result = tempResult;
            return true;
        }

        result = null!;
        return false;
    }

    /// <summary>
    /// Adds all tags of the specified collection to this compound.
    /// </summary>
    /// <param name="newTags">The tags to add. Every tag must be named and must have no parent.</param>
    /// <exception cref="ArgumentNullException"><paramref name="newTags"/> is <see langword="null"/>.
    /// -or-
    /// One of the tags is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">One of the tags is unnamed.
    /// -or-
    /// A tag with the same name is already present in this compound.
    /// -or-
    /// One of the tags already belongs to another compound or list.</exception>
    public void AddRange(IEnumerable<NbtTag> newTags)
    {
        if (newTags == null) throw new ArgumentNullException(nameof(newTags));
        foreach (var tag in newTags) Add(tag);
    }

    /// <summary>
    /// Determines whether this compound contains a tag with the specified name.
    /// </summary>
    /// <param name="tagName">The name to locate. This value cannot be <see langword="null"/>.</param>
    /// <returns><see langword="true"/> if a tag with the specified name was found; otherwise,
    /// <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="tagName"/> is <see langword="null"/>.</exception>
    public bool Contains(string tagName)
    {
        if (tagName == null) throw new ArgumentNullException(nameof(tagName));
        return _tags.ContainsKey(tagName);
    }

    /// <summary>
    /// Removes the tag with the specified name from this compound.
    /// </summary>
    /// <param name="tagName">The name of the tag to remove.</param>
    /// <returns><see langword="true"/> if the tag was found and removed; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="tagName"/> is <see langword="null"/>.</exception>
    /// <remarks>
    /// The parent of a removed tag is set to <see langword="null"/>.
    /// </remarks>
    public bool Remove(string tagName)
    {
        if (tagName == null) throw new ArgumentNullException(nameof(tagName));
        if (!_tags.TryGetValue(tagName, out var tag)) return false;
        _tags.Remove(tagName);
        tag.Parent = null;
        return true;
    }

    internal void RenameTag(string oldName, string newName)
    {
        Debug.Assert(oldName != null);
        Debug.Assert(newName != null);
        Debug.Assert(newName != oldName);
        if (_tags.TryGetValue(newName, out _))
            throw new ArgumentException("Cannot rename: a tag with the name already exists in this compound.");
        if (!_tags.TryGetValue(oldName, out var tag))
            throw new ArgumentException("Cannot rename: no tag found to rename.");
        _tags.Remove(oldName);
        _tags.Add(newName, tag);
    }

    /// <inheritdoc />
    public override object Clone()
    {
        return new NbtCompound(this);
    }

    internal override void PrettyPrint(StringBuilder sb, string indentString, int indentLevel)
    {
        for (var i = 0; i < indentLevel; i++) sb.Append(indentString);
        sb.Append("TAG_Compound");
        if (!string.IsNullOrEmpty(Name)) sb.AppendFormat("(\"{0}\")", Name);
        sb.AppendFormat(": {0} entries {{", _tags.Count);

        if (Count > 0)
        {
            sb.Append('\n');
            foreach (var tag in _tags.Values)
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
        while (true)
        {
            var childType = readStream.ReadTagType();
            if (childType == NbtTagType.End)
            {
                readStream.ExitLevel();
                return;
            }

            var child = CreateTag(childType);
            child.Name = readStream.ReadString();
            child.ReadTag(readStream);
            SetOrReplace(child);
        }
    }

    internal override void WriteTag(NbtBinaryWriter writeStream)
    {
        writeStream.Write(NbtTagType.Compound);
        if (Name == null) throw new NbtFormatException("Name is null");
        writeStream.Write(Name);
        WriteData(writeStream);
    }

    internal override void WriteData(NbtBinaryWriter writeStream)
    {
        writeStream.EnterLevel();
        foreach (var tag in _tags.Values) tag.WriteTag(writeStream);
        writeStream.Write(NbtTagType.End);
        writeStream.ExitLevel();
    }

    #endregion

    #region Implementation of IEnumerable<NbtTag>

    /// <summary>
    /// Returns an enumerator that iterates through all tags in this compound.
    /// </summary>
    /// <returns>An enumerator for the tags in this compound.</returns>
    public IEnumerator<NbtTag> GetEnumerator()
    {
        return _tags.Values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _tags.Values.GetEnumerator();
    }

    #endregion

    #region Implementation of ICollection<NbtTag>

    /// <summary>
    /// Adds a tag to this compound.
    /// </summary>
    /// <param name="newTag">The tag to add. It must be named and must have no parent.</param>
    /// <exception cref="ArgumentNullException"><paramref name="newTag"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="newTag"/> is this compound.
    /// -or-
    /// <paramref name="newTag"/> is unnamed.
    /// -or-
    /// <paramref name="newTag"/> already belongs to another compound or list.
    /// -or-
    /// A tag with the same name is already present in this compound.</exception>
    /// <remarks>
    /// The added tag becomes a child of this compound.
    /// </remarks>
    public void Add(NbtTag newTag)
    {
        if (newTag == null) throw new ArgumentNullException(nameof(newTag));

        if (newTag == this) throw new ArgumentException("Cannot add tag to self");

        if (newTag.Name == null) throw new ArgumentException("Only named tags are allowed in compound tags.");

        if (newTag.Parent != null)
            throw new ArgumentException("A tag may only be added to one compound/list at a time.");

        _tags.Add(newTag.Name, newTag);
        newTag.Parent = this;
    }

    /// <summary>Adds a named tag, replacing any tag already stored under that name, as vanilla does when a compound holds duplicate names.</summary>
    internal void SetOrReplace(NbtTag tag)
    {
        if (_tags.Remove(tag.Name!, out var replaced)) replaced.Parent = null;
        _tags.Add(tag.Name!, tag);
        tag.Parent = this;
    }

    /// <summary>
    /// Removes all tags from this compound.
    /// </summary>
    /// <remarks>
    /// The parent of every removed tag is set to <see langword="null"/>.
    /// </remarks>
    public void Clear()
    {
        foreach (var tag in _tags.Values) tag.Parent = null;
        _tags.Clear();
    }

    /// <summary>
    /// Determines whether this compound contains the specified tag.
    /// </summary>
    /// <param name="tag">The tag to locate. This value cannot be <see langword="null"/>.</param>
    /// <returns><see langword="true"/> if the tag was found; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="tag"/> is <see langword="null"/>.</exception>
    /// <remarks>
    /// The comparison matches the tag instance, not the tag name.
    /// </remarks>
    public bool Contains(NbtTag tag)
    {
        if (tag == null) throw new ArgumentNullException(nameof(tag));
        return _tags.ContainsValue(tag);
    }

    /// <summary>
    /// Copies the tags of this compound to an array, starting at the specified index.
    /// </summary>
    /// <param name="array">The one-dimensional array that is the destination of the copied tags. The array must
    /// have zero-based indexing.</param>
    /// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
    /// <exception cref="ArgumentNullException"><paramref name="array"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="arrayIndex"/> is less than zero.</exception>
    /// <exception cref="ArgumentException">The number of tags in this compound is greater than the available space
    /// from <paramref name="arrayIndex"/> to the end of <paramref name="array"/>.</exception>
    public void CopyTo(NbtTag[] array, int arrayIndex)
    {
        _tags.Values.CopyTo(array, arrayIndex);
    }

    /// <summary>
    /// Removes the specified tag from this compound.
    /// </summary>
    /// <param name="tag">The tag to remove. It must be named.</param>
    /// <returns><see langword="true"/> if the tag was found and removed; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="tag"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="tag"/> is unnamed.</exception>
    /// <remarks>
    /// The comparison matches the tag instance, not the tag name. The parent of a removed tag is set to
    /// <see langword="null"/>.
    /// </remarks>
    public bool Remove(NbtTag tag)
    {
        if (tag == null) throw new ArgumentNullException(nameof(tag));
        if (tag.Name == null) throw new ArgumentException("Trying to remove an unnamed tag.");
        if (!_tags.TryGetValue(tag.Name, out var maybeItem)) return false;
        if (maybeItem != tag || !_tags.Remove(tag.Name)) return false;
        tag.Parent = null;
        return true;
    }

    /// <summary>
    /// Gets the number of tags contained in this compound.
    /// </summary>
    public int Count => _tags.Count;

    bool ICollection<NbtTag>.IsReadOnly => false;

    #endregion

    #region Implementation of ICollection

    void ICollection.CopyTo(Array array, int index)
    {
        CopyTo((NbtTag[])array, index);
    }

    object ICollection.SyncRoot => (_tags as ICollection).SyncRoot;

    bool ICollection.IsSynchronized => false;

    #endregion
}