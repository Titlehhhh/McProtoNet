# NBT

NBT (Named Binary Tag) is the Java Edition binary format for nested structures:
numbers, strings, arrays, lists, and compound tags, with no single predefined
schema. The format is described on the
[NBT format](https://minecraft.wiki/w/NBT_format) page on minecraft.wiki. The
protocol uses NBT wherever a packet field carries data of arbitrary shape - an
item with components, a block entity, chunk data. Such a field is read and
written alongside the other primitives: `MinecraftPrimitiveReader.ReadNbtTag`
and `MinecraftPrimitiveWriter.WriteNbt` (`McProtoNet.Primitives`) sit in the
same row as reading a `VarInt` or a string. The difference is that a whole
parser from `McProtoNet.NBT` stands behind the NBT field.

## A parser of its own

The library does not take a ready-made NBT parser and does not build a tree
through generic deserialization with reflection - the format is tied to Java
details: its own byte order, its own string encoding, a structure with no
external schema. Instead of one universal implementation, `McProtoNet.NBT` has
three, each for its own input shape:
[`NbtSpanReader`](../08-api-reference/McProtoNet/NBT/NbtSpanReader.md) reads a
contiguous `ReadOnlySpan<byte>` of a packet that fits whole;
[`NbtSequenceReader`](../08-api-reference/McProtoNet/NBT/NbtSequenceReader.md)
reads a `SequenceReader<byte>` over a packet's `ReadOnlySequence<byte>`, split
into pipe segments;
[`NbtReader`](../08-api-reference/McProtoNet/NBT/NbtReader.md) parses a `Stream`
and walks the tags without building the whole tree. All three agree on one
format: big-endian numbers, strings in modified UTF-8, a nesting limit of 512
levels.

## Tag types

[`NbtTagType`](../08-api-reference/McProtoNet/NBT/NbtTagType.md) lists 12 data
types plus `End` - the marker for the end of a compound and the element type of
an empty list:

```csharp
public enum NbtTagType : byte
{
    End = 0x00,
    Byte = 0x01,
    Short = 0x02,
    Int = 0x03,
    Long = 0x04,
    Float = 0x05,
    Double = 0x06,
    ByteArray = 0x07,
    String = 0x08,
    List = 0x09,
    Compound = 0x0a,
    IntArray = 0x0b,
    LongArray = 0x0c
}
```

In a `List`, every element has the same type and no name of its own. In a
`Compound`, elements are named and listed up to the `End` tag.

## From a packet: a tag tree

When a packet field is NBT, it is read straight into a tree of objects
([`NbtTag`](../08-api-reference/McProtoNet/NBT/NbtTag.md) and its descendants:
[`NbtCompound`](../08-api-reference/McProtoNet/NBT/NbtCompound.md),
[`NbtList`](../08-api-reference/McProtoNet/NBT/NbtList.md),
[`NbtByte`](../08-api-reference/McProtoNet/NBT/NbtByte.md)).
`MinecraftPrimitiveReader.ReadNbtTag` picks the reader by the shape of the
buffer that is still unread for the current packet:

```csharp
public NbtTag? ReadNbtTag(bool readRootTag)
{
    var unread = _reader.UnreadSequence;
    if (unread.IsSingleSegment)
    {
        // Fast path: parse straight from the contiguous buffer.
        var spanReader = new NbtSpanReader(unread.FirstSpan);
        NbtTag? result = spanReader.ReadAsTag<NbtTag>(readRootTag);
        _reader.Advance(spanReader.ConsumedCount);
        return result;
    }

    // Multi-segment path: parse straight from the sequence.
    return NbtSequenceReader.ReadTag(ref _reader, readRootTag);
}
```

## Cursor parser

`NbtReader` is built differently from both readers on the packet path. It walks
the tags of a document one by one, like `XmlReader` walks XML nodes, and does
not build a tree - its own description in the code calls it "forward-only" and
"non-cached". After each `ReadToFollowing()`, the current tag is visible through
properties, and the value is read separately, only if the calling code needs it:

```csharp
public NbtTagType TagType { get; private set; }
public string? TagName { get; private set; }
public int Depth { get; private set; }

public bool ReadToFollowing()
```

`ReadAsTag()` on the same `NbtReader` can build out the tree from the current
point, if it is needed whole. The cursor and the tree do not exclude each other.
The tree is just not built by default.

## Strings in modified UTF-8

NBT strings use not plain UTF-8 but modified UTF-8 - the same encoding as
`DataOutput.writeUTF` in Java. There are two differences. U+0000 is written as
two bytes, `C0 80`, not as the zero byte of plain UTF-8. A character outside the
basic multilingual plane is encoded as two three-byte sequences, one per UTF-16
surrogate, instead of one four-byte sequence.
[`ModifiedUtf8`](../08-api-reference/McProtoNet/NBT/ModifiedUtf8.md) provides
`GetByteCount`, `GetBytes`, and `GetString`. Of these, only `GetString`
allocates.

## Limits

`NbtLimits` sets two limits for the module: `MaxDepth = 512` for the nesting of
compounds and lists, and `MaxStringByteLength = ushort.MaxValue` (65535 bytes -
the limit of the format itself, since a string length is stored in two bytes).
Both limits are checked before allocation. `NbtSpanReader` and
`NbtSequenceReader` compare the declared length against the bytes left in the
buffer, and throw
[`NbtFormatException`](../08-api-reference/McProtoNet/NBT/NbtFormatException.md)
on a negative or an oversized length.

## Writing

Writing mirrors reading.
[`NbtBufferWriter`](../08-api-reference/McProtoNet/NBT/NbtBufferWriter.md)
writes a tag to an `IBufferWriter<byte>` and stands behind
`MinecraftPrimitiveWriter.WriteNbt` on the packet path.
[`NbtWriter`](../08-api-reference/McProtoNet/NBT/NbtWriter.md) is a forward-only
writer over a `Stream`, the write-side counterpart of `NbtReader`. `WriteTag`
walks the tree and writes the type, the name, and the payload in the same order
that `NbtSpanReader` reads them. The `End` byte for a compound is appended on
its own.

## Common mistake: the buffer runs out before the tag

The body of the packet that NBT is read from is a window into a buffer that
lives until the next read. It must be parsed right away, not across an `await`,
like the rest of the packet fields
([Receive buffer](../04-transport/03-packet-stream.md)). `ReadNbtTag` reads that
same buffer and must be called in the same synchronous frame. `NbtSpanReader`
locks this in at the compiler level: as a `ref struct`, it cannot be stored in a
field or carried across an `await`. The `NbtTag` tree that `ReadNbtTag` returns
is free of this constraint. The strings and arrays inside it are already copied
(`ModifiedUtf8.GetString` allocates a string, arrays are read into their own
memory), so the tree outlives the buffer's boundary. The limit applies only to
the moment of parsing, not to the result.

## Next

- [Reading and writing primitives](../05-packets/02-primitives.md)
- [From a raw packet: number, name, instance](../05-packets/03-from-raw-packet.md)
