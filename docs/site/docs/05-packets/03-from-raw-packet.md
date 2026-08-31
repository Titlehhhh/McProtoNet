# From a raw packet: number, name, instance

Parsing an incoming packet goes through three steps: first only the number is
visible, then the packet's name is found from the number, and only after that,
if needed, the packet becomes a typed object. Each next step costs more than the
previous one and needs more context.

## Number

[`IncomingPacket`](../08-api-reference/McProtoNet/Primitives/IncomingPacket.md)
is what the transport hands back after a read, before any parsing:

```csharp
public readonly struct IncomingPacket
{
    public readonly int Id;
    public readonly ReadOnlyMemory<byte> Body;
}
```

`Id` is the packet number on the wire, `Body` is the body without the number. In
the protocol this field is called Packet ID, the packet format is on the
[Packet format](https://minecraft.wiki/w/Java_Edition_protocol/Packets#Without_compression)
page. A packet body is a window into a buffer that lives until the next read; it
must be parsed right away, not across an `await`
([Receive buffer](../04-transport/03-packet-stream.md)).

`Id` alone is not enough. The same number in different phases and directions
means different packets - `0x00` in login and `0x00` in play have nothing in
common. More than that, the number of one packet changes from one protocol
version to the next. A packet cannot be parsed from a single number - it also
needs the phase, the direction, and the protocol version (where the phase and
the direction come from - [Phase and direction](01-phases-and-direction.md)).

## Name

`PacketRegistry.TryResolve` turns the number, together with the phase, the
direction, and the protocol version, into a
[`PacketDescriptor`](../08-api-reference/McProtoNet/Protocol/PacketDescriptor.md):

```csharp
public static bool TryResolve(int id, int protocolVersion,
    PacketPhase phase, PacketDirection dir,
    [NotNullWhen(true)] out PacketDescriptor? descriptor)
```

`PacketDescriptor` carries `Identity` - a
[`PacketIdentity`](../08-api-reference/McProtoNet/Protocol/PacketIdentity.md)
struct with a human-readable name, `Name` (like `Teams`), and a manifest key,
`Key` (like `play.toClient.teams`) - and `Ids`, an array of `IdRange`: which
wire number the packet has for which protocol versions. Inside, `TryResolve`
first looks up `TryGetOrdinal` - the packet's dense internal index in the
catalog of its (phase, direction) - and only then takes the matching descriptor
from `Catalog`. This index is not needed outside; outside, only the result
matters.

This is enough to log packets by name, even when there is no need to decode
them:

```csharp
if (PacketRegistry.TryResolve(raw.Id, pv, phase, direction, out var d))
    logger.LogDebug("recv {Name} ({Key})", d.Identity.Name, d.Identity.Key);
else
    logger.LogWarning("recv unmapped id 0x{Id:X2} in {Phase}/{Direction}",
        raw.Id, phase, direction);
```

`PacketRegistry.Catalog(phase, dir)` returns the full list of packets for one
phase and direction, as `ReadOnlySpan<PacketDescriptor>`. This works for
printing a table of a phase's packets:

```csharp
foreach (var d in PacketRegistry.Catalog(PacketPhase.Play,
    PacketDirection.Clientbound))
    Console.WriteLine($"{d.Identity.Name,-24} {d.Identity.Key}");
```

## Instance

A typed object is the most expensive step, and it is reached through three
different paths, depending on whether the packet type is known in advance.

When the type is known in advance - for example, right after login, exactly a
[`LoginSuccessPacket`](../08-api-reference/McProtoNet/Protocol/Packets/Login/Clientbound/LoginSuccessPacket.md)
is expected - [`PacketIo`](../08-api-reference/McProtoNet/Protocol/PacketIo.md)
is used:

```csharp
public static bool TryDecode<T>(in IncomingPacket raw, int protocolVersion,
    [NotNullWhen(true)] out T? packet, out DecodeError error)
    where T : class, IPacket<T>
```

`Decode<T>` does the same thing, but instead of `false` and `error` it throws
[`PacketDecodeException`](../08-api-reference/McProtoNet/Protocol/PacketDecodeException.md).
Both overloads read the body straight into `T`, bypassing the packet number: the
calling code already knows it expects exactly this type.

When the type is not known in advance - an arbitrary stream of packets of one
phase arrives, and it must be parsed in full - `PacketFlow.Dispatch` or
`PacketFlow.TryDispatch` is used, with a visitor:

```csharp
public static void Dispatch<TVisitor>(in IncomingPacket raw,
    int protocolVersion, PacketPhase phase, PacketDirection dir,
    ref TVisitor visitor) where TVisitor : IPacketVisitor
```

[`IPacketVisitor`](../08-api-reference/McProtoNet/Protocol/IPacketVisitor.md) is
`Visit<T>(T packet)`, which receives a statically typed packet without boxing,
and `Unknown(in IncomingPacket raw)`, for a number the registry does not know in
this phase and direction. `TryDispatch` follows the same dispatch path, but
instead of an exception on a broken body it returns `false` and a
[`DecodeError`](../08-api-reference/McProtoNet/Protocol/DecodeError.md).
[`PacketFlow`](../08-api-reference/McProtoNet/Protocol/PacketFlow.md) also has a
path with no visitor at all, its own `TryDecode`, which returns an `IPacket?`
directly.

The third path is the async handler,
[`ClientboundHandler`](../08-api-reference/McProtoNet/Protocol/ClientboundHandler.md)
(or
[`ServerboundHandler`](../08-api-reference/McProtoNet/Protocol/ServerboundHandler.md)
for the opposite direction): it finds the number itself, parses the body, and
calls the matching virtual `On<Name>` method. This is the most common path in
application code; for the details, see the
[Handlers and unknown packets](04-handlers.md) page.

## Parse errors

`DecodeError` is the reason a body failed to parse:

- `UnsupportedVersion` - no packet with this name exists for this protocol
  version;
- `TrailingBytes` - the body parsed, but extra bytes remain at the end: the
  packet's spec for this version is apparently wrong;
- `Malformed` - the body cut off early, or carries broken data.

A number the registry does not know is not a parse error but a normal state of
the stream: more on this in [Exceptions](06-exceptions.md). A real error happens
only when the number is found but the body fails to parse.

## Next

- [Handlers and unknown packets](04-handlers.md) - the async path through
  `ClientboundHandler`
- [Phase and direction](01-phases-and-direction.md) - where the phase and the
  direction come from
- [One build - many versions](05-multiversion.md) - how one packet's number
  changes between protocol versions
- [Exceptions](06-exceptions.md) - when a parse error reaches application code
  as an exception
