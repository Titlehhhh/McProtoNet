# Cancellation, errors, closing

A connection can end in three ways: cancel the operation through a
`CancellationToken`, call `Abort` with a reason, or call `DisposeAsync`.
All three close the connection, but they behave differently for a call
that is waiting for bytes at that moment, and they leave a different
trace in `CloseReason`. What follows covers `MinecraftConnection` and
`MinecraftClient`. `StreamingConnection` follows the same rules, with
one addition at the end of the page.

## Three ways

The token passes into `ReadPacketAsync`, `WritePacketAsync`, and
`SendAsync`. Exactly one case comes cheap: the token is already canceled
before the call. Then the check at the entry throws
`OperationCanceledException`, and the connection stays alive. Once the
call has started, cancellation closes the whole connection - even if not
a single byte has arrived from the socket yet. The next section explains
why.

`Abort(reason)` can be called from any thread and at any moment, with no
wait for a response: a read or write that was holding the socket at that
moment fails with the given reason, and `CloseReason` remembers it. This
is the tool the application code uses to tell the library that the
protocol broke the connection, not the transport - for example, the
server sent a packet that should not appear in this phase.

`DisposeAsync` uses the same `Abort`, but with no reason, and adds a
wait after it. The order of these steps is described in "Packet
stream". At the `MinecraftClient` level, its own send gate joins this:
`SendAsync` and `SendRawAsync` hold the gate for the duration of the
call, and the client's `DisposeAsync` waits for the gate to free up
within the same five-second budget, then closes the connection
regardless - even if the gate never freed up.

## Why canceling a started read breaks the connection

A started read already sits inside the frame reader, and from outside
it is not visible what exactly it managed to take: part of a frame -
the length or a piece of the body - may already be pulled from the
stream and sitting in the buffer. There is no way to give it back, and
so there is no way to return the connection to a frame boundary for the
next call: the frame is broken, the same as after a network break or an
encoding error.

So the connection does not tell a "canceled read" apart from a "failed
read" - both close the connection. The caller whose token fired gets
`OperationCanceledException`. Any other call, started later or waiting
alongside it, gets `ConnectionAbortedException` with that same
exception inside it. For the application code this means: the token
passed into `ReadPacketsAsync` works as a switch for the whole read
loop, not as a way to cancel the current call and get the next packet
as if nothing happened.

## How to learn the reason

`CloseReason` is `null` while the connection is open, and it stays
`null` after a clean end of stream. In every other case it holds an
exception: either what was passed into `Abort`, or the first failure
that the connection's own reader or writer caught. `Completion` is a
task that completes at the moment of closing and never faults: the code
can wait for it without a `try/catch` to learn that closing happened,
and then read `CloseReason` to learn why.

The first failure of the stream reaches the code that caused it as its
own exception type, and at the same moment it settles into
`CloseReason`. Every later call - of the same method and of any other
connection member - does not touch the dead socket again, and instead
throws `ConnectionAbortedException` right away, with the same reason
inside it: a second reader or writer that arrives at an already-dead
connection sees the real reason, not some unrelated later error. The
exception to this rule is `InvalidOperationException` that does not come
from `ObjectDisposedException`: a concurrent read or a call after
`ToStreaming` is a bug in the calling code that never reached the
stream, and the connection does not close because of it.

## What happened -> which exception

| What happened | Exception |
| --- | --- |
| The server closed the stream cleanly, between frames | `EndOfStreamException` |
| The stream broke off in the middle of a frame | `EndOfStreamException` |
| This side closed while a call was waiting for bytes | `ConnectionAbortedException` |
| A call after `DisposeAsync` completed | `ObjectDisposedException` |
| A second, concurrent `ReadPacketAsync` on the connection | `InvalidOperationException` |
| A broken frame (length, varint, decompression size) | `InvalidDataException` |

The reader does not tell the first two cases apart - both arrive as a
plain `EndOfStreamException`. The row about a broken frame is not the
connection closing for an outside reason, but a finding by the reader
itself: it also settles into `CloseReason`, but it is a data error, not
a broken connection.

## Closing order in application code

```csharp
await using var client = new MinecraftClient(options);
await client.ConnectAsync(token);
try
{
    await foreach (var packet in client.ReadPacketsAsync(token))
        Handle(packet);
}
catch (EndOfStreamException)
{
    // a normal end of session, the server closed the stream itself
}
catch (ConnectionAbortedException ex)
{
    Log(ex.InnerException); // the reason is already inside
}
```

There is no need to call `Abort` or `DisposeAsync` separately here -
`await using` takes care of that: the send gate waits out its budget,
and the buffers return to the pool. An explicit `Abort` is needed only
when code unrelated to the read loop closes the connection - for
example, another task that notices the server is not following the
protocol.

## The streaming path

A `StreamingConnection` obtained through `ToStreaming` inherits the
common behavior: `Abort` from any thread, `CloseReason`, `Completion`,
and the same memory of the first error - it settles into `CloseReason`,
and every later call gets it too. The difference sits in three places.

The main difference is cancellation. A `ReadBatchAsync` canceled by its
own token does not close the connection: the buffer is intact, the
frame boundary is not lost, and reading can continue. Only canceling
`FlushAsync` closes the connection, and only after bytes have already
gone into the stream.

Next to `Abort` it has `CompleteAsync` - a clean finish that sends off
whatever accumulated in the send buffer, after which `CloseReason`
stays `null`. And its `DisposeAsync` does not use the five-second
budget: it waits out `Completion` in full, and it drops any
bytes not yet flushed into the stream.

## Next

- [Packet stream](03-packet-stream.md) - the same rules for one frame
- [Connection without a client](04-raw-connection.md) -
  `MinecraftConnection` and the streaming path without `MinecraftClient`
  on top
- [Exceptions](../05-packets/06-exceptions.md) - what the packet layer
  throws on top of the transport
