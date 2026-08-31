# Handlers and unknown packets

The previous page named the handler as the third path to a typed packet -
the async one, through `ClientboundHandler` and `ServerboundHandler`. It is
not an alternative to the visitor for a single packet. It is a tool for the
whole directional stream: one object carries the whole session, from login
to the end, with no manual `switch` on numbers.

## How it works

`HandleAsync` is the only public method of the handler. It finds the packet
number in the registry itself, parses the body, and calls the matching
virtual method `On<Name>`. The phase is read once, at the start of the call.
A handler that changes the phase itself does so after dispatch, and the
current packet is parsed with the phase it arrived in.

```csharp
public ValueTask HandleAsync(in IncomingPacket raw, int protocolVersion)
{
    var phase = Phase;
    if (!PacketRegistry.TryGetOrdinal(raw.Id, protocolVersion, phase,
            PacketDirection.Clientbound, out var ordinal))
        return OnUnknown(in raw);
    var reader = new MinecraftPrimitiveReader(raw.Body);
```

Next comes a nested `switch`: first by phase, then by the packet `ordinal`
inside it, with no reflection and no boxing. Each branch runs the same
sequence: a typed read of the body and a call to `On<Name>` with the ready
packet.

## Why there are many methods but few overrides

The handler carries one virtual method per packet of a phase and a
direction - `ClientboundHandler` has 143 of them, plus `OnUnknown` for
everything else. By default each one does nothing:

```csharp
protected virtual ValueTask OnLoginCompress(
    Packets.Login.Clientbound.LoginCompressPacket packet) => default;
protected virtual ValueTask OnEncryptionRequest(
    Packets.Login.Clientbound.EncryptionRequestPacket packet) => default;
```

Application code overrides only what it needs. The rest of the packets pass
through the no-op with no code on the application side. `MinimalBot`
inherits `ClientboundHandler` and overrides 16 packet methods out of 143,
plus `OnUnknown` - login, configuration, keep-alive, teleport, health.

## Who sets Phase

`Phase` has a public getter and a setter open only to the subclass. This is
the owner's decision: application code drives the phases (see [Phase and
direction](01-phases-and-direction.md)). The handler starts in the phase
where every connection begins: `login` for `ClientboundHandler`,
`handshaking` for `ServerboundHandler`, and then it moves `Phase` itself in
response to transition packets. An example of a transition, and what
happens when `Phase` is forgotten, is on that same page.

## Unknown packet

`OnUnknown` runs when the packet number is not in the registry for the
current pair of phase and direction. This is not a failure and not a reason
for an exception. Application code does not need to parse everything - only
what matters to it, and the rest legitimately stays unknown. By default
`OnUnknown` is also a no-op. `MinimalBot` overrides it to log each new
missed packet of a phase once, not once per instance:

```csharp
protected override ValueTask OnUnknown(in IncomingPacket raw)
{
    if (_unknownSeen.Add((Phase, raw.Id)))
    {
        var packetName = PacketRegistry.TryResolve(raw.Id, pv, Phase,
            Direction, out var desc) ? desc.Identity.Name : $"0x{raw.Id:X2}";
        Console.WriteLine($"[{Phase}] skipped {packetName} " +
            $"({raw.Body.Length} bytes)");
    }
    return default;
}
```

`raw` lives only for the duration of the call: the packet body is a window
into the buffer, and it lives only until the next read (see [Receive
buffer](../04-transport/03-packet-stream.md)). If the body bytes are needed
later, code must copy them right here.

## Trailing bytes at the end

Another situation: the number is found, the body is parsed by the `Read`
method, but bytes remain in the buffer after the read. The handler does not
throw an exception and does not stop the call to `On<Name>` - that call is
already running. Instead it raises the static event
`PacketFlow.OnTrailingBytes`:

```csharp
if (reader.RemainingCount != 0)
    PacketFlow.RaiseTrailingBytes(raw.Id, protocolVersion,
        reader.RemainingCount);
return pending;
```

Subscribing to `OnTrailingBytes` (the `TrailingBytesHook` delegate) is the
job of application code, and it happens once per process: the event is
static and shared by every handler. This is a separate channel for
reporting a suspicious specification, not the `DecodeError.TrailingBytes`
that `PacketIo.TryDecode` and `PacketFlow.TryDispatch` return. There,
parsing goes through an explicit call, and the suspicion can return as a
value. Here, parsing is hidden inside `HandleAsync`, and the only channel
out is the event.

## Handler or visitor

`IPacketVisitor.Visit<T>` returns `void` - there is nowhere to put the
`ValueTask` of an async `On<Name>`, and the continuation would get lost
silently. This is why the handler does not implement `IPacketVisitor` and
does not go through `PacketFlow.Dispatch`: it does the same thing - number,
`ordinal`, read, call - but in a single `case` block, with no visitor.

The visitor fits where processing is synchronous and does not depend on
inheriting from one class. `PacketSubscriptions` is a public implementation
of `IPacketVisitor` over a dictionary of delegates: the method
`On<T>(PacketHandler<T> handler)` registers a handler for a packet type,
`Visit<T>` finds it by `Identity.Ordinal` and calls it, and if nothing is
registered for the type, it silently skips it. It is the right choice for a set of
independently assembled subscriptions with synchronous processing. The
handler is the right choice when one object owns the whole connection and some packets
need `await`, as in `MinimalBot`.

## Next

- [From a raw packet](03-from-raw-packet.md) - the three parsing stages,
  where the handler is built in as the third path
- [Phase and direction](01-phases-and-direction.md) - where the phase and
  direction come from, and who changes them
- [Exceptions](06-exceptions.md) - what reaches application code as an
  exception, and what does not
