# About McProtoNet

**McProtoNet** is an open-source C# library for the Minecraft Java Edition protocol, client side. It is built around two goals: **one code base for many game versions** and **high throughput** — libdeflate compression, hardware AES for the stream cipher, `Span<byte>` everywhere on the hot path, and async I/O.

## What you get

- **Protocol, not a game.** Connect to a server, send and receive typed packets, switch compression and encryption on. The library is a foundation for bots, custom clients and tools.
- **Many versions, one API.** Protocol versions 735–776 (Minecraft 1.16 → 26.2). Each packet knows its wire layout per version; you pick the protocol version once, at connect time.
- **Offline servers.** Handshake and login to offline-mode servers out of the box; the login state machine stays in your code, so nonstandard servers are not a dead end.
- **Network helpers.** SRV record lookup and LAN server detection.

## Install

```
dotnet add package McProtoNet --prerelease
```

Then read [First bot in 15 minutes](getting-started.md).

## NuGet packages

| Package | What it holds |
| --- | --- |
| `McProtoNet` | Glue: `MinecraftClient`, typed `SendAsync<T>`, SRV lookup, LAN detection |
| `McProtoNet.Transport` | Bytes between the socket and the packet: framing, compression, encryption |
| `McProtoNet.Protocol` | Packet layer: generated packet classes, registry, dispatch, handler bases |
| `McProtoNet.Primitives` | Primitive reader/writer, buffers, `IncomingPacket` / `OutgoingPacket` |
| `McProtoNet.NBT` | NBT parser and writer |

## Where to go next

- [First bot in 15 minutes](getting-started.md) — install, protocol version, the four phases, a working bot.
- [What the library does not do](non-goals.md) — read this before you plan a bot.
- [Glossary](glossary.md) — frame, phase, ordinal and other words the API uses.
- [API reference](xref:McProtoNet) — generated from the source.

