# Reading and writing primitives

A packet body is just bytes. The protocol defines its own types on top of them
(full list on the
[Data types](https://minecraft.wiki/w/Java_Edition_protocol/Data_types) page):
variable-length VarInt and VarLong, strings with a VarInt length before the
bytes, a 16-byte UUID, NBT tags of arbitrary nesting. Encoding and decoding them
the same way on read and on write, without extra allocations, is the job of a
separate layer:
[`MinecraftPrimitiveReader`](../08-api-reference/McProtoNet/Primitives/MinecraftPrimitiveReader.md)
and
[`MinecraftPrimitiveWriter`](../08-api-reference/McProtoNet/Primitives/MinecraftPrimitiveWriter.md)
from `McProtoNet.Primitives`.

## What gets read and written

The reader and the writer carry symmetric method sets:
`ReadVarInt`/`WriteVarInt` and `ReadVarLong`/`WriteVarLong` for variable-length
values; `ReadBoolean` for one byte; signed and unsigned byte, short, int, long,
float, and double go big-endian, except VarInt and VarLong.
`ReadString`/`WriteString` encode a string as UTF-8 with the length in bytes in
front, using the same VarInt, and cap the length with the `maxLength` parameter
(`short.MaxValue` by default). `ReadUUID`/`WriteUUID` read and write a `Guid` as
16 big-endian bytes. `ReadNbtTag` and `WriteNbt`, along with their variants with
a presence flag byte, `ReadOptionalNbtTag`/`WriteOptionalNbt`, work with the NBT
tree. `ReadBuffer`/`ReadRestBuffer`/`WriteBuffer` copy raw bytes without a
length prefix - for when the length is known from outside.

`ReadVarInt` shows the shape of a typical signature, and where the error goes
when data runs short:

```csharp
public int ReadVarInt()
{
    if (!_reader.TryReadVarInt(out int res, out _))
    {
        ThrowHelper.ThrowNotEnoughData();
    }

    return res;
}
```

## `Span<byte>` without extra copies

`MinecraftPrimitiveReader` is a `ref struct` over `SequenceReader<byte>`. The
constructor wraps the given `ReadOnlyMemory<byte>` or `ReadOnlySequence<byte>`,
without copying anything:

```csharp
public ref struct MinecraftPrimitiveReader
{
    private SequenceReader<byte> _reader;

    public MinecraftPrimitiveReader(ReadOnlyMemory<byte> data)
        : this(new ReadOnlySequence<byte>(data))
    {
    }
}
```

A typical source of this memory is `IncomingPacket.Body`: a packet body is a
window into a buffer that lives until the next read; it must be parsed right
away, not across an `await`
([Receive buffer](../04-transport/03-packet-stream.md)). `Read(Span<byte>
output)` copies bytes straight into the caller's buffer and allocates nothing on
its own.

`MinecraftPrimitiveWriter` makes the same saving in the other direction: it
holds an `ArrayBufferWriter<byte>`, and `WrittenSpan` and `WrittenMemory` are
windows into that buffer, which the next write invalidates: the buffer can move
when it grows, and the old window will not know it.

## Who owns the memory

[`MemoryOwner<T>`](../08-api-reference/McProtoNet/Primitives/MemoryOwner-1.md)
is a struct over an array rented from `ArrayPool<T>.Shared`. `Allocate` takes an
array of the needed length, `Dispose` returns it to the pool:

```csharp
public static MemoryOwner<T> Allocate(int length)
{
    if (length == 0) return default;
    var array = ArrayPool<T>.Shared.Rent(length);
    return new MemoryOwner<T>(array, length);
}

public void Dispose()
{
    var arr = _array;
    if (arr is not null)
    {
        _array = null;
        ArrayPool<T>.Shared.Return(arr);
    }
}
```

`MemoryOwner<T>` is a mutable struct: a copy makes sense only when it passes
ownership onward, otherwise both holders return the same array to the pool.

The writer hands out finished bytes through `GetWrittenMemory`, and this is
already a copy - not a window into the writer's own buffer:

```csharp
public MemoryOwner<byte> GetWrittenMemory()
{
    var written = _writer.WrittenSpan;
    var owner = MemoryOwner<byte>.Allocate(written.Length);
    written.CopyTo(owner.Span);
    return owner;
}
```

The copy here is not wasted: the writer's buffer gets reused through
[`MinecraftPrimitiveWriterCache`](../08-api-reference/McProtoNet/Primitives/MinecraftPrimitiveWriterCache.md)
(`Rent`/`Return`, one writer per thread, writers larger than 64 kilobytes get
dropped), and it must not be touched after `Return`.
[`OutgoingPacket`](../08-api-reference/McProtoNet/Primitives/OutgoingPacket.md)
takes the finished `MemoryOwner<byte>` and must be disposed exactly once, so the
buffer returns to the pool.

## Errors on reading broken data

Running out of data is the most common read error, and it is the same
everywhere: `InvalidDataException` through the internal
`ThrowHelper.ThrowNotEnoughData`. A VarInt longer than 5 bytes and a VarLong
longer than 10 bytes also throw `InvalidDataException`, with a separate message
about the length. Strings carry more checks: a negative length prefix, a byte
count above `maxLength * 3`, a final string length above `maxLength` - each
through `ThrowHelper.ThrowInvalidData` with its own text. Broken NBT surfaces as
[`NbtFormatException`](../08-api-reference/McProtoNet/NBT/NbtFormatException.md)
from inside `ReadNbtTag`. For the methods that read a VarInt straight from a
`Stream` (`Stream.ReadVarInt`, `ReadVarIntAsync`) - extensions over the stream,
not over the reader itself - a data cutoff throws `EndOfStreamException`.

## When this touches application code

Usually, not directly: generated packets carry their own `Read`/`Write` for each
protocol version, and those are already written on top of this layer. Direct
access to `MinecraftPrimitiveReader`/`Writer` is needed in two cases: when
application code implements its own packet with its own `Read`/`Write`, and when
the body is parsed by hand - the packet type is not known in advance, or only
the first few fields are needed, without decoding the whole packet.

## Next

- [From a raw packet](03-from-raw-packet.md) - where a packet body becomes a
  typed object
- [NBT](../06-nbt/01-nbt.md) - the tag format this same layer works with
- [Frames](../04-transport/02-framing.md) - where the body that
  `MinecraftPrimitiveReader` sees comes from
