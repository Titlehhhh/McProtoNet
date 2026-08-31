# Exceptions

The packet layer throws an exception only when bytes that already arrived
whole from the transport cannot become a packet: the body broke off too
early, carries corrupt data, or no packet with that name exists on this
protocol version. A neighboring layer covers connection drop, cancellation,
and closing - see [Cancellation, errors,
closing](../04-transport/06-cancellation.md). This page covers only what
happens after an `IncomingPacket` has already arrived whole.

## Not an exception: an unknown number

A packet number that the registry does not know for a (phase, direction)
pair is a normal state of the stream, not an error. Such a packet reaches
`visitor.Unknown` or the handler's `OnUnknown` and throws nothing. See
[From a raw packet](03-from-raw-packet.md) for details.

## Two entry points: throwing and Try

`PacketIo` and `PacketFlow` each have two symmetric entry points.
`Decode<T>` and `Dispatch` throw an exception on a broken body.
`TryDecode<T>` and `TryDispatch` run the same parse, but instead of an
exception they return `false` and a `DecodeError` - `None`,
`UnsupportedVersion`, `TrailingBytes`, or `Malformed`.
`ClientboundHandler.HandleAsync` (and `ServerboundHandler`) is built only
as a throwing entry point: handlers have no matching Try method.

```csharp
if (!PacketIo.TryDecode<LoginSuccessPacket>(
    raw, pv, out var packet, out var error))
{
    logger.LogWarning("LoginSuccess: {Error}", error);
    return;
}

HandleLoginSuccess(packet);
```

`PacketDecodeException`, thrown by the throwing entry point, carries
`PacketType` and `Error` - the same `DecodeError` that the Try path would
return - and the original exception (`InvalidDataException`,
`NbtFormatException`, and so on) sits in `InnerException`.

## Trailing bytes

`PacketIo` is strict about trailing bytes: if something remains in the
buffer after `T.Read` runs, `Decode<T>` throws a `PacketDecodeException`
with `DecodeError.TrailingBytes`, and `TryDecode<T>` returns `false` with
the same reason.

`PacketFlow.Dispatch`/`TryDispatch` and `HandleAsync` behave differently:
the packet has already reached the visitor or `On<Name>`, and instead of an
exception, the event `PacketFlow.OnTrailingBytes` fires - the event
mechanism is covered in [Handlers and unknown
packets](04-handlers.md). Trailing bytes almost always mean that the
packet specification for this protocol version is described incorrectly -
a bug to report against the specs, not a connection failure.

## Unsupported version

`ProtocolNotSupportException` is thrown when no packet with that name
exists on the protocol version used for reading, writing, or typed
sending. It carries `TypeName`, `ActualVersion`, and `SupportedRanges` -
the version ranges where the packet does exist. On the Try paths, this
collapses into `DecodeError.UnsupportedVersion`.

## Table: what happened -> what gets thrown

| What happened | Throwing path | Try path |
| --- | --- | --- |
| The body broke off too early or carries corrupt data (`VarInt`, NBT) | `InvalidDataException`, `EndOfStreamException`, `NbtFormatException` | `DecodeError.Malformed` |
| No packet with this name exists on this protocol version | `ProtocolNotSupportException` | `DecodeError.UnsupportedVersion` |
| A version-layered packet is written without the needed layer | `WrongLayerException` | `DecodeError.Malformed` |
| `PacketIo.Decode`/`TryDecode`: the body parsed, bytes remain | `PacketDecodeException` (`TrailingBytes`) | `DecodeError.TrailingBytes` |
| `Dispatch`/`TryDispatch`/`HandleAsync`: same, but the packet already reached the visitor | does not throw, event `OnTrailingBytes` | does not throw, event `OnTrailingBytes` |
| The number is unknown for (phase, direction) | `visitor.Unknown` / `OnUnknown`, not an error | the same, `true` |

`EndOfStreamException` in this table is not the same as the clean end of
stream from "Cancellation, errors, closing": there it means the server
closed the connection, here it means the body, already received whole by
the transport, ran out before `Read` expected it to. What tells them apart
is where the exception is caught: from the `ReadPacketsAsync` loop, it is a
connection drop; from inside body parsing, it is a broken packet.

## What to catch in the bot loop, what to fix in code

A connection drop (`ConnectionAbortedException`, a clean
`EndOfStreamException` from the read loop) is routine in a bot loop: catch
it and decide what to do next, reconnect or end the session. A packet parse
error points at the packet specification, or at the protocol version in
application code, and silencing it quietly is not a good idea:
`PacketDecodeException`, `WrongLayerException`, and
`ProtocolNotSupportException` are worth logging and investigating, not
suppressing on a routine basis. The Try path is the way to skip one bad packet
and continue the session, without dropping the connection over a
specification bug.

```csharp
try
{
    await foreach (var raw in client.ReadPacketsAsync(token))
        await handler.HandleAsync(raw, pv);
}
catch (EndOfStreamException)
{
    // the server closed the stream: a normal end of session
}
catch (ConnectionAbortedException ex)
{
    Log(ex.InnerException); // a break at the transport level
}
catch (Exception ex) when (ex is InvalidDataException
    or NbtFormatException or ProtocolNotSupportException)
{
    Log(ex); // a broken packet or an unsupported version, not a lost connection
}
```

## Next

- [From a raw packet](03-from-raw-packet.md) - `PacketRegistry`, `PacketIo`,
  `PacketFlow`, and where `DecodeError` comes from
- [Handlers and unknown packets](04-handlers.md) - `ClientboundHandler` and `OnUnknown`
- [Cancellation, errors, closing](../04-transport/06-cancellation.md) -
  connection drop, `Abort`, `CloseReason`
