# Packet stream

`MinecraftClient.ReadPacketsAsync` is a thin wrapper around one loop: the client
holds one
[`MinecraftConnection`](../08-api-reference/McProtoNet/Transport/MinecraftConnection.md),
and the method hands control to its `ReadPacketAsync` again and again. There is
no frame queue and no read-ahead buffering: in one call the transport reads
exactly one frame - the length, then the body - and hands it out as an
[`IncomingPacket`](../08-api-reference/McProtoNet/Primitives/IncomingPacket.md).
Where a frame ends and how the length is read is in [Frames](02-framing.md).

The packet stream does not know which phase the client is in - handshaking,
login, configuration, or play. Frames pass through it the same way in every
phase. Application code tracks the phase
([Phase and direction](../05-packets/01-phases-and-direction.md)).

## Who owns the body

The packet body is not its own copy of bytes - it is a window into a block that
the reader rents from a pool, and the packet holds a reference to that block.
The block goes back to the pool when the last reference is released. With
compression there are two blocks, one for the compressed bytes and one for the
decompressed bytes, but the first one is freed right after decompression, and
the rule for `Body` does not change.

`ReadPacketAsync` hands the reference over: the caller owns the packet and
disposes it.

```csharp
using var packet = await client.ReadPacketAsync(token);
Handle(packet.Id, packet.Body.Span);
```

`ReadPacketsAsync` lends instead: every packet it yields is a borrowed copy, the
loop releases the real reference before the next step, and the body is valid
inside one turn of the loop.

This is where the rule from "first bot" comes from: a packet is parsed right
away, and `Body` never crosses an `await`. If the data is needed longer - for
example, to sit in a queue for another thread - the loop is asked for a
reference of its own with `Retain`, and that packet is disposed separately.

```csharp
var kept = new List<IncomingPacket>();

await foreach (var packet in client.ReadPacketsAsync(token))
{
    if (packet.Id == interestingId)
        kept.Add(packet.Retain());
}
```

`Body.ToArray()` still copies the bytes, and a copy is still the right answer
when the data leaves for code that knows nothing about the pool - but `Retain`
allocates nothing, so keeping the packet itself is the cheaper way.

Where a parsed packet goes next - which handler method it calls, and what
happens with unknown ids - is described in
[Handlers and unknown packets](../05-packets/04-handlers.md).

## End of session

The `ReadPacketsAsync` enumeration never ends quietly - it always throws an
exception. The full "what happened -> which exception" table is in
[Cancellation, errors, closing](06-cancellation.md).

## Cancellation

A cancellation token does not cancel a single read: if the read has already
touched the socket, cancellation closes the whole connection, and the
`ReadPacketsAsync` loop stops with it. The full picture is in
[Cancellation, errors, closing](06-cancellation.md).

## Sending

`SendAsync` and `SendRawAsync` pass through a shared gate - a `SemaphoreSlim(1,
1)` inside
[`MinecraftClient`](../08-api-reference/McProtoNet/MinecraftClient.md). Each
call first takes the gate, then writes the frame through the connection, then
releases the gate. If several tasks send packets at the same time, the frames do
not mix - the calls queue up at the gate and go out to the socket one at a time,
each one whole.

```csharp
await Task.WhenAll(
    client.SendAsync(new PlaySb.KeepAlivePacket(keepAliveId), pv).AsTask(),
    client.SendRawAsync(customId, customBody).AsTask());
```

Reading is not guarded this way: a parallel `ReadPacketAsync` throws
`InvalidOperationException`, a bug in the calling code, not a data race
([Cancellation, errors, closing](06-cancellation.md)). `ReadPacketsAsync` has no
way around this: until `await foreach` hands control back, a second read on the
same connection must not start.

## Closing

`DisposeAsync` closes the connection and releases the buffers. The order of the
steps, and the table of what gets thrown, are in
[Cancellation, errors, closing](06-cancellation.md).

## When the client is not needed

The same packet stream is available without `MinecraftClient`:
`MinecraftConnection` reads and writes one frame at a time over any `Stream`,
and
[`StreamingConnection`](../08-api-reference/McProtoNet/Transport/StreamingConnection.md)
does the same in batches. Both are covered in
[Connection without a client](04-raw-connection.md).

## Next

- [Frames](02-framing.md) - how one frame is built
- [Handlers and unknown packets](../05-packets/04-handlers.md) - where a parsed
  packet goes
- [Encryption and compression](05-encryption-and-compression.md) - what turns on
  mid-stream
- [Cancellation, errors, closing](06-cancellation.md) - how a session ends
- [Connection without a client](04-raw-connection.md) - the same stream one
  level down
