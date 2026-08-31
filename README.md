<h1 align="center">
     <img height="160" alt="McProtoNet" src="https://raw.githubusercontent.com/Titlehhhh/McProtoNet/master/assets/icon.svg">
  <br>
  <a href="https://www.nuget.org/packages/McProtoNet">
    <img alt="NuGet" src="https://img.shields.io/nuget/v/McProtoNet?style=for-the-badge">
  </a>
  <a href="https://f.feedz.io/mcprotonet/night/nuget/index.json">
    <img alt="nightly" src="https://img.shields.io/endpoint?url=https%3A%2F%2Ff.feedz.io%2Fmcprotonet%2Fnight%2Fshield%2FMcProtoNet%2Flatest&label=nightly&style=for-the-badge">
  </a>
  <a href="https://discord.gg/PWfYWRDJme">
    <img alt="Discord" src="https://img.shields.io/badge/Discord-join-5865F2?style=for-the-badge&logo=discord&logoColor=white">
  </a>
</h1>

**McProtoNet** is a .NET library for the Minecraft Java Edition protocol, client side.
It gives you the transport, the packets, and a simple client. You write the bot.

![117 bots form the word McProtoNet](https://raw.githubusercontent.com/Titlehhhh/McProtoNet/master/assets/formation.png)

*Bots join a server, listen to game chat, and walk into any text you type.
The picture shows 117 of them. This is
[examples/FormationBots](https://github.com/Titlehhhh/McProtoNet/tree/master/examples/FormationBots) —
a few hundred lines on top of the library.*

> ⚠️ McProtoNet is under active development. The API changes between versions,
> and not all packets are modeled yet.

## What is inside

- **Transport.** `MinecraftConnection` frames packets over any duplex `Stream`
  on `System.IO.Pipelines`. Compression is [libdeflate], encryption is AES/CFB8
  with hardware cores for x86 (AES-NI) and ARM64 (NEON). The cipher beats
  BouncyCastle by about 2x in our
  [benchmarks](https://github.com/Titlehhhh/McProtoNet/tree/master/benchmarks)
  and allocates nothing per call.
- **Packets.** Typed packet classes for protocols 735–776 (Minecraft 1.16 → 26.2),
  generated from [F# specs](https://github.com/Titlehhhh/minecraft-protocol-fs).
  One allocation per packet, no reflection, no boxing on the hot path.
  Unknown packets stay available as raw id + bytes.
- **Client.** `MinecraftClient` connects over TCP, resolves SRV records, and
  moves packets. It also finds LAN servers. Handshake, login, and game logic stay
  in your code — the
  [examples](https://github.com/Titlehhhh/McProtoNet/tree/master/examples) show
  the whole path.
- **NBT.** A standalone reader and writer for the NBT format, with Modified
  UTF-8 strings.
- Works with offline-mode servers. Mojang authentication is not implemented yet.

## Install

```
dotnet add package McProtoNet
```

The package pulls in the rest of the stack. You can also take a single layer:

| Package | What it holds |
| --- | --- |
| `McProtoNet` | Client glue: connect, SRV lookup, typed send, LAN search |
| `McProtoNet.Protocol` | Generated packets, types, and handler bases |
| `McProtoNet.Transport` | Framing, compression, encryption |
| `McProtoNet.Primitives` | VarInt readers and writers, packet structs |
| `McProtoNet.NBT` | NBT reader and writer |

The transport depends on the native package `McProtoNet.Native` 1.0.0, which
carries libdeflate.

## Nightly builds

Every push to the `dev` branch publishes packages to a Feedz feed. Versions look
like `2.0.0-preview.4.<height>` and are prerelease, so `--prerelease` is needed.
Stable releases go to nuget.org.

```
dotnet nuget add source https://f.feedz.io/mcprotonet/night/nuget/index.json -n mcprotonet-night
dotnet add package McProtoNet --prerelease
```

## Quick start

```csharp
using McProtoNet;
using HandshakeSb = McProtoNet.Protocol.Packets.Handshaking.Serverbound;
using LoginSb = McProtoNet.Protocol.Packets.Login.Serverbound;

const int Pv = 772;

await using var client = new MinecraftClient(new MinecraftClientOptions { Host = "localhost" });
await client.ConnectAsync();

await client.SendAsync(new HandshakeSb.SetProtocolPacket(Pv, "localhost", 25565, 2), Pv);
await client.SendAsync(new LoginSb.LoginStartPacket("Steve", V764_Last: new(Guid.NewGuid())), Pv);

await foreach (var packet in client.ReadPacketsAsync())
{
    // bot derives from ClientboundHandler and overrides the packets it wants
    await bot.HandleAsync(in packet, Pv);
}
```

The receive loop and the phase machine live in your code on purpose.
[examples/MinimalBot](https://github.com/Titlehhhh/McProtoNet/tree/master/examples/MinimalBot)
walks the full path in one file: handshake → login → configuration → play, with
keep-alives and chat.
[examples/FormationBots](https://github.com/Titlehhhh/McProtoNet/tree/master/examples/FormationBots)
runs one client per bot in the picture above.

## Versions

| | |
| --- | --- |
| Minecraft | 1.16 → 26.2 (protocols 735–776) |
| Runtime | .NET 8, .NET 9, .NET 10, .NET 11 |

## License

MIT. See [LICENSE](https://github.com/Titlehhhh/McProtoNet/blob/master/LICENSE).

## Community

Questions, bugs, ideas: [Discord](https://discord.gg/PWfYWRDJme) or
[GitHub issues](https://github.com/Titlehhhh/McProtoNet/issues).

## 💸 Support the project

You can support development with crypto. Send only in the network named
next to the address; a transfer that arrives through another network cannot
be returned. Neither address needs a memo or a tag.

**USDT, TRON network (TRC-20)**: `TVw4jCehZspPk3aT1fecQWhEzH2LEkG9WG`

**TON and USDT, TON network**: `UQDp_mDSj3VH0uIqBl2aNsyZJ8Drymo3qX-nlYNr5r6zTCaI`

The same is on the site: [Support the project](https://titlehhhh.github.io/McProtoNet/next/support).

[libdeflate]: https://github.com/ebiggers/libdeflate
