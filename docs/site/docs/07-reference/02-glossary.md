# Glossary

Terms that already appear in the documentation. Each one has a short definition
and a link to the page that covers it in full.

- **protocol version** - a number the client sends in the handshake (for
  example, 772). It sets which packet field layout and which packet numbers
  apply for the session. More detail:
  [A packet and its identifier](../05-packets/03-from-raw-packet.md).

- **send gate** - a `SemaphoreSlim(1, 1)` inside
  [`MinecraftClient`](../08-api-reference/McProtoNet/MinecraftClient.md). Every
  `SendAsync` and `SendRawAsync` call passes through it. Frames from different
  calls do not mix on one socket. More detail:
  [Packet stream](../04-transport/03-packet-stream.md).

- **frame** - the framing wrapper around a packet: a length, one more varint
  when compressed, then the body. It makes the boundary between packets visible
  in a continuous TCP stream. More detail:
  [Frames](../04-transport/02-framing.md).

- **catalog** - the list of packets for one (phase, direction) pair, returned by
  `PacketRegistry.Catalog(phase, dir)`. More detail:
  [Phase and direction](../05-packets/01-phases-and-direction.md).

- **batch reading** - the transport path where frames are read and written in
  batches, not one at a time:
  [`StreamingConnection`](../08-api-reference/McProtoNet/Transport/StreamingConnection.md)
  over a shared buffer. More detail:
  [Connection without a client](../04-transport/04-raw-connection.md).

- **multi-version** - the ability of one build to work with every supported
  protocol version. Field layouts and packet numbers live inside the packet
  itself, and the protocol version picks the one to use. More detail:
  [One build - many versions](../05-packets/05-multiversion.md).

- **direction** -
  [`PacketDirection`](../08-api-reference/McProtoNet/Protocol/PacketDirection.md):
  `Clientbound` for packets from the server to the client, `Serverbound` for the
  opposite direction. Each phase has its own set of packets for each direction.
  More detail: [Phase and direction](../05-packets/01-phases-and-direction.md).

- **packet number** - the `Id` field on
  [`IncomingPacket`](../08-api-reference/McProtoNet/Primitives/IncomingPacket.md),
  the number on the wire. On its own it means nothing: the packet type is looked
  up by the number together with the phase, the direction, and the protocol
  version. More detail:
  [A packet and its identifier](../05-packets/03-from-raw-packet.md).

- **handler** -
  [`ClientboundHandler`](../08-api-reference/McProtoNet/Protocol/ClientboundHandler.md)
  and
  [`ServerboundHandler`](../08-api-reference/McProtoNet/Protocol/ServerboundHandler.md):
  a base class with an `On<Name>` method for each packet. Application code
  inherits from it and overrides only what it needs. More detail:
  [First bot](../02-getting-started/02-first-bot.md).

- **shared secret** - 16 bytes that the two sides exchange through
  [`EncryptionRequestPacket`](../08-api-reference/McProtoNet/Protocol/Packets/Login/Clientbound/EncryptionRequestPacket.md)
  and
  [`EncryptionResponsePacket`](../08-api-reference/McProtoNet/Protocol/Packets/Login/Serverbound/EncryptionResponsePacket.md).
  It serves as both the AES-128 key and the cipher's initialization vector. More
  detail:
  [Compression and encryption](../04-transport/05-encryption-and-compression.md).

- **window into a buffer** - a packet body is not a copy of the bytes but a
  region of a rented buffer. It lives only until the next read, so it must be
  parsed right away, not carried across an `await`. More detail:
  [Packet stream](../04-transport/03-packet-stream.md).

- **ordinal** - the dense packet number inside its own catalog, part of
  [`PacketIdentity`](../08-api-reference/McProtoNet/Protocol/PacketIdentity.md).
  Unlike the packet number on the wire, the ordinal stays stable across builds
  and protocol versions. More detail:
  [Phase and direction](../05-packets/01-phases-and-direction.md).

- **packet** - `IncomingPacket` on input,
  [`OutgoingPacket`](../08-api-reference/McProtoNet/Primitives/OutgoingPacket.md)
  on output: the common currency of every layer of the library, a number and a
  body. More detail:
  [What is visible from outside](../03-architecture/03-public-surface.md).

- **compression threshold** - `CompressionThreshold` on `MinecraftClient`. A
  packet shorter than the threshold goes out as is. One that is not shorter is
  compressed with libdeflate. More detail:
  [Compression and encryption](../04-transport/05-encryption-and-compression.md).

- **visitor** -
  [`IPacketVisitor`](../08-api-reference/McProtoNet/Protocol/IPacketVisitor.md),
  with the methods `Visit<T>` and `Unknown`, a synchronous way to parse a packet
  through `PacketFlow.Dispatch`. It does not work with async methods, so the
  handler is a separate concept. More detail:
  [Who knows whom](../03-architecture/02-who-knows-whom.md).

- **[`PacketRegistry`](../08-api-reference/McProtoNet/Protocol/PacketRegistry.md)**
  - `PacketRegistry`. It translates a packet number, together with the phase,
  the direction, and the protocol version, into a packet description or into the
  typed object itself. More detail:
  [A packet and its identifier](../05-packets/03-from-raw-packet.md).

- **raw packet** - what the transport returns: a number and a chunk of bytes,
  with no knowledge of which packet it is or what fields it has. More detail:
  [Four layers on one screen](../03-architecture/01-layers.md).

- **phase** -
  [`PacketPhase`](../08-api-reference/McProtoNet/Protocol/PacketPhase.md).
  Handshaking, status, login, configuration, play: consecutive stages of a
  session. The library does not derive the phase on its own. Application code
  switches it. More detail:
  [Phase and direction](../05-packets/01-phases-and-direction.md).

## Next

- [Version to protocol](01-version-to-protocol.md)
- [Four layers on one screen](../03-architecture/01-layers.md)
