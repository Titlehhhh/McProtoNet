# Four layers on one screen

The library splits into four parts. Each part lives in its own project and
ships as its own NuGet package. An application can take the whole set or a
single layer.

## Primitives

`McProtoNet.Primitives` is the bottom layer. It reads and writes simple
values: VarInt, strings, positions, and everything packets are built from.
It also holds the memory owner `MemoryOwner` and the two types the other
layers meet on: `IncomingPacket` (a packet number and its body) and
`OutgoingPacket`. Outside its own code, Primitives knows only about NBT.

## Transport

`McProtoNet.Transport` turns bytes into packets and back. It handles four
jobs: frames (`Framing`), connections (`Connection`), compression
(`Compression`, libdeflate), and encryption (`Cryptography`, its own
AES/CFB8).

Transport hands back raw packets: a number and a body. It does not
know, and does not need to know, what kind of packet it is or what fields it
carries.

## Packet layer

`McProtoNet.Protocol` knows packets by name. It has two halves. The
hand-written half (`Flow`) sets the rules: how a packet declares its
identity, how to decode it, and how handlers are sorted by phase and
direction. The generated half (`Generated`) holds the packets themselves,
nested types, enums, and the registry that maps a packet number to a
concrete type for a given protocol version.

This layer knows nothing about transport either. It receives a ready
`IncomingPacket` and returns a decoded packet.

## Glue

The `McProtoNet` project joins the two layers above. It holds
`MinecraftClient`: a TCP connection, optionally through a proxy, reading the
packet stream, sending packets, and the compression and cipher switches. It
also holds typed sending (`SendAsync<T>`), server lookup by SRV record, and
a LAN server detector.

The client is deliberately thin. Application code drives the phases
([Phase and direction](../05-packets/01-phases-and-direction.md)), and bot
logic lives there too.

## What follows from this

An application usually needs only the `McProtoNet` package. But the layers
separate on purpose. How they connect is covered in
[Who knows whom](02-who-knows-whom.md).
