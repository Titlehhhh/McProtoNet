# Glossary

Words the API and this site use, in the order you will meet them.

## Wire and transport

**Frame** — one packet on the wire: a VarInt length, then the body. With compression on, the body is prefixed with the uncompressed size; with encryption on, the whole stream is AES/CFB8. `McProtoNet.Transport` turns a socket stream into frames and back.

**Protocol version (`pv`)** — the integer the client sends in the handshake; it names the wire format. 772 is 1.21.7–1.21.8, 776 is 26.2. See the table in [First bot](getting-started.md). Not the Minecraft version string and not the package version.

**Compression threshold** — set by the server during login (`LoginCompress`). Bodies at least that long are sent compressed; `-1` means off.

**`IncomingPacket`** — a raw frame you got from `ReadPacketsAsync`: packet id plus the body bytes. The body is a window into the transport buffer and is valid only until the next read — decode it before any `await`.

**`OutgoingPacket`** — the same for sending: id plus body; the transport adds length, compression and encryption.

## Packet layer

**Phase** — one of `handshaking`, `login`, `configuration`, `play`. Packet ids are only meaningful inside a phase, so every decode needs the phase. `ClientboundHandler.Phase` is yours to switch.

**Direction** — `clientbound` (server → you) or `serverbound` (you → server). Same name, different packet, e.g. there is an `AbilitiesPacket` in both directions.

**Catalog** — all packets of one (phase, direction) pair, e.g. `play.toClient`. There are nine catalogs.

**Ordinal** — the dense index of a packet inside its catalog (0, 1, 2 …). The dispatcher and subscriptions use it as an array index, so a lookup is one load, not a dictionary. It is an implementation detail: you never type it.

**`PacketIdentity`** — name, phase, direction and ordinal of a packet type, available statically as `SomePacket.Identity`.

**Dispatch** — id → ordinal → decode → your handler, done by generated code (`PacketFlow.Dispatch`, `ClientboundHandler.HandleAsync`). No reflection, no boxing: the packet type is known at every call site.

**Handler base** — `ClientboundHandler` / `ServerboundHandler`: generated abstract classes with one virtual `On<PacketName>` per packet. Override what you need; unknown ids go to `OnUnknown`.

**Visitor** — `IPacketVisitor`: the lower-level shape (`Visit<T>(T packet)`) the dispatcher calls; the handler base is built on the same tables. Use it when you want to fan one decoded packet out to several consumers.

**Version layers** — a packet whose fields differ between protocol versions is one class with nullable groups such as `V764_Last` or `VUntil764`; the group for your `pv` must be filled when you send, or `WrongLayerException` tells you which one.

## Performance words

**Hot path** — the per-packet code: read frame, decode, dispatch, write frame. The library's rule there is no allocation per packet except the packet object itself, no reflection, no boxing, AOT-compatible.

**Primitive reader / writer** — `MinecraftPrimitiveReader` / `MinecraftPrimitiveWriter`: VarInt, strings, NBT, UUID and friends over `Span<byte>`; every generated `Read` / `Write` is made of these.
