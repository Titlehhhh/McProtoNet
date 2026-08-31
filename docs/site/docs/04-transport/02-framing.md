# Frames: where a packet ends

TCP delivers a continuous stream of bytes. The sender may call `Write` three
times in a row, and the receiver may see these bytes as one chunk, two chunks,
or five - the protocol keeps no record of call boundaries. To tell where one
packet ends and the next begins, the packet body needs a length in front of it.
This is the frame: a wrapper around the packet that makes the boundary visible
on the receiving side. The protocol describes it on the
[Packet format](https://minecraft.wiki/w/Java_Edition_protocol/Packets#Packet_format)
page. `McProtoNet.Transport.Framing` builds and parses this wrapper.

## Frame without compression

Without compression, a frame is the packet length as a VarInt, followed by the
packet itself: a VarInt id and a body.
[`PacketWriteExtensions`](../08-api-reference/McProtoNet/Transport/Framing/PacketWriteExtensions.md)
writes it in two lines:

```csharp
writer.WriteVarInt(packet.Length);
writer.Write(packet);
```

The reading side goes in reverse order: first the length VarInt, byte by byte,
then exactly as many body bytes as the length gives - not one more.

## Frame with compression

When
[`PacketStreamReader`](../08-api-reference/McProtoNet/Transport/Framing/PacketStreamReader.md)/[`PacketStreamWriter`](../08-api-reference/McProtoNet/Transport/Framing/PacketStreamWriter.md)
has `CompressionThreshold` turned on, a second VarInt appears after the frame
length. A packet shorter than the threshold goes as is, and this VarInt is just
0. A packet at or above the threshold is compressed whole (the id and the body
together) through libdeflate, and then the second VarInt carries the
uncompressed size. This value lets the reader allocate the decompression buffer
ahead of time:

```csharp
writer.WriteVarInt(compressedLength + uncompressedSize.GetVarIntLength());
writer.WriteVarInt(uncompressedSize);
writer.Write(rented.AsSpan(0, compressedLength));
```

The first VarInt here is the length of everything after it: the size of the
uncompressed value field plus the compressed bytes. The reading side looks at
the second VarInt: 0 or negative means "the packet is not compressed, its raw
bytes follow", positive means "zlib follows, decompress into a buffer of exactly
this size". If decompression produces more or fewer bytes than declared, the
frame counts as corrupt.

## Who handles this

`PacketWriteExtensions` is a static class with the frame building blocks: the
same `WritePacket` methods work over `IBufferWriter<byte>`, `PipeWriter`, and a
plain `Stream`, both synchronously and asynchronously.
[`StreamingConnection`](../08-api-reference/McProtoNet/Transport/StreamingConnection.md)
assembles frames from them in batches through `BufferedPacketReader` and
[`PacketBatch`](../08-api-reference/McProtoNet/Transport/Framing/PacketBatch.md)
- this is covered in "Connection without a client". `PooledBufferWriter` is a
helper buffer from the pool: `PacketStreamWriter` uses it when encryption is on,
to build the whole frame in memory before running it through
[`PacketCipher`](../08-api-reference/McProtoNet/Transport/Cryptography/PacketCipher.md)
- the cipher needs the whole frame at once, it cannot take it in pieces.

The transport knows nothing about the packet's content: the frame carries the
packet number and bytes, while the packet layer decides what kind of packet it
is and what fields it has.

## Limits and errors

The frame length runs from 1 to 32 MiB (`BufferedPacketReader.MaxFrameLength`).
Zero, a negative value, or a length over the cap throws `InvalidDataException`
from `ThrowHelper.ThrowInvalidFrameLength`. The length VarInt cannot take more
than five bytes - if it does, this is `ThrowVarIntTooLong`. The uncompressed
size gets the same cap check: if it exceeds 32 MiB, the frame is rejected before
the reader tries to allocate a buffer. A decompression failure, or a mismatch
between the final size and the declared uncompressed size, also throws
`InvalidDataException`, with different messages (`ThrowDecompressFailed`,
`ThrowDecompressSizeMismatch`).

A connection drop in the middle of a frame also throws `EndOfStreamException`,
with no empty packet in its place - the full "what happened -> which exception"
table is in [Cancellation, errors, closing](06-cancellation.md).

`PacketStreamReader` and `PacketStreamWriter` do not read or write two frames at
once - a parallel `ReadPacketAsync`/`WritePacketAsync` call on top of an
unfinished first call gets `InvalidOperationException`, as everywhere else in
the transport (see the same page). For the same reason, `Cipher` and
`CompressionThreshold` change only between frames - trying to change them in the
middle of a read or a write also throws `InvalidOperationException`.
[`ConnectionAbortedException`](../08-api-reference/McProtoNet/Transport/ConnectionAbortedException.md)
has nothing to do with this layer - it is a connection-level exception, covered
in "Cancellation, errors, closing".

## Where this touches application code directly

Frames are normally invisible: the connection hides them behind
[`IncomingPacket`](../08-api-reference/McProtoNet/Primitives/IncomingPacket.md)/[`OutgoingPacket`](../08-api-reference/McProtoNet/Primitives/OutgoingPacket.md).
But when
[`MinecraftConnection`](../08-api-reference/McProtoNet/Transport/MinecraftConnection.md)
is not needed - for example, for the handshake before login, or for a short
protocol exchange without buffering - `PacketStreamReader` and
`PacketStreamWriter` give the same frame format directly over any `Stream`, one
packet per call. The packet body is a window into a buffer that lives until the
next read. It must be parsed right away, not across an `await`
([Receive buffer](03-packet-stream.md)).

## Next

- [Packet stream](03-packet-stream.md) - how frames add up to a stream
- [Encryption and compression](05-encryption-and-compression.md) - what the
  frame here only mentions
- [Reading and writing primitives](../05-packets/02-primitives.md) - where
  VarInt comes from
