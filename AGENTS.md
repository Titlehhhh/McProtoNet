# McProtoNet agent guide

.NET library for the Minecraft Java Edition protocol (client side): transport,
serialization, packets, multi-version support. Active branch:
`feature/spec-codegen`. Honest protocol range in code: **735–772**
(1.16 → 1.21.8; `MinecraftVersion.StartProtocol` / `LatestProtocol`) — the
README's "1.12.2–1.21.4" claim is stale; trust code and this file over README.

Maintenance rule: if a PR changes an architectural fact stated here, update
this file in the same PR. Snapshot facts below are marked with their commit;
unmarked statements are design invariants.

## Repository map

| Path | Role | State (5608469, 2026-08-10) |
| --- | --- | --- |
| src/McProtoNet | client, transport, crypto, zlib | alive |
| src/McProtoNet.Protocol | packet layer: hand-written `Flow/` + delivered `Generated/` | alive, builds |
| src/McProtoNet.Serialization | primitive reader/writer, buffers | alive, tested |
| src/McProtoNet.NBT | own NBT parser | alive; round-trip tests currently red |
| src/McProtoNet.SourceGenerator | ONE Roslyn generator: `VersionRangeGenerator` | alive |
| src/McProtoNet.Utils | SRV lookup, LAN detect | periphery |
| src/McProtoNet.FSharp | F# facade | stub |
| src/McProtoNet.Abstractions | — | defunct: no csproj; types moved into McProtoNet |
| examples/MinimalBot | executable documentation of the packet layer | alive; in the slnx under /samples/ |
| tests/McProtoNet.Tests | xUnit v3: transport, serialization, NBT, packet-flow round-trips | alive; 33 tests red (see Commands) |
| benchmarks/ | BenchmarkDotNet, perf contract lives here | alive |
| TestServer/ | manual test server | broken: uses removed Abstractions; fails the solution build |
| docs/ | Writerside site | mixed, stale in places |
| build/ | Nuke build; Tests/Pack targets are hollowed out | stale |

## Architecture in one screen

Two floors, joined since 2026-08-09 (the "packet symbiosis").
`McProtoNet.Protocol` references `McProtoNet` — the typed packet layer sits on
top of the transport; a client app references `McProtoNet.Protocol`.

- **Transport (bottom):** `PipelinesMinecraftClient` (`Client/`) +
  `MinecraftPacketPipeReader/Writer` (`Net/Pipelines/`) on
  System.IO.Pipelines. Yields raw `InputPacket { int Id;
  ReadOnlySequence<byte> Data }` via `ReadPacketsAsync()`. `Data` is a window
  into the pipe buffer — valid only until the next read; decode immediately,
  never across an `await`. Compression is libdeflate
  (`Tomat.LibDeflate.Native`), encryption is BouncyCastle AES/CFB8. The
  handshake/login state machine lives in consumer code, not in the library.
- **Packet layer, hand-written contract (`src/McProtoNet.Protocol/Flow/`):**
  `IPacket<TSelf>` with static abstract `Identity` and
  `TryGetPacketId(pv, out id)`; `PacketIdentity` (manifest key, name, phase,
  direction, dense per-catalog `Ordinal` — the currency of the hot path);
  `IPacketVisitor` (`Visit<T>` typed, `Unknown` for unmapped ids — a normal
  stream condition, not an error); `PacketIo` (`TryDecode`/`Decode`:
  InputPacket → concrete packet, trailing bytes = `DecodeError.TrailingBytes`);
  `PacketSubscriptions` (ordinal-indexed handler slots);
  `ClientPacketExtensions` (`SendAsync<T>` — id comes from the type;
  `SendRawAsync` keeps the low-level path open); `DecodeError`,
  `PacketExceptions`.
- **Packet layer, generated implementation
  (`src/McProtoNet.Protocol/Generated/`, 67 files at 5608469):**
  `Packets/<Phase>/<Direction>/*.cs` — sealed partial records carrying
  `[Packet]`, `[PacketField]`, `[ProtocolSupport]`
  (`src/McProtoNet.Protocol/Attributes/`) with per-version `Read`/`Write`;
  `Types/` and `Bitflags/` for nested protocol types; `Flow/` with
  `PacketRegistry.g.cs` (descriptor catalogs cold, dense id→ordinal span
  tables hot; every entry point is Try), `PacketFlow.g.cs`
  (`Dispatch`: id → ordinal → typed decode → `visitor.Visit<T>`; unknown ids
  fall through to `Unknown`; trailing bytes raise the `OnTrailingBytes` hook),
  and `ClientboundHandler.g.cs` (abstract handler base:
  `HandleAsync(in InputPacket, pv)`, consumer-owned `Phase`, one virtual
  `On<Name>` per packet).
- **The receive loop is consumer code:** `await foreach` over
  `client.ReadPacketsAsync()` → `handler.HandleAsync(raw, pv)`.
  `examples/MinimalBot` walks the whole path (handshake → login →
  configuration → play) with no manual id switch.

## Where Generated/ comes from

`Generated/` is delivered from the sibling spec repo `minecraft-protocol-fs`
by its `scripts/deliver-to-mcprotonet.ps1`: it regenerates from the F# specs
(`dotnet run -- gen`) and replaces `src/McProtoNet.Protocol/Generated/`
wholesale; a small exclude list in the script holds back packets whose
codegen does not compile yet. Never edit `Generated/` by hand — the next
delivery erases it. To change generated code, change the specs or codegen in
`minecraft-protocol-fs` and re-deliver. Committing the delivered files is
step 2 of the end-of-packet-cycle commit pair (canon: workspace
`docs/rules.md`).

The ONE Roslyn generator —
`src/McProtoNet.SourceGenerator/VersionRangeGenerator.cs` — reads
`[ProtocolSupport]` and emits per-type
`IsSupportedVersion(int)` partials plus a `ThrowHelper`. Every Protocol build
depends on it — do not edit it casually.

## Commands

- Build the working set: `dotnet build src/McProtoNet.Protocol` (pulls
  McProtoNet + Serialization + the generator);
  `dotnet build examples/MinimalBot`. TFMs: net8.0/net9.0/net10.0.
- `dotnet build McProtoNet.slnx` FAILS at 5608469: `TestServer/` still
  references the removed `McProtoNet.Abstractions` and a missing DotNext
  package. Build projects, not the solution, until that is fixed.
- Tests: `dotnet run --project tests/McProtoNet.Tests` — the project is an
  xUnit v3 executable; plain `dotnet test` on it discovers no tests (no
  VSTest adapter). At 5608469: 10168 tests, 33 failing (NBT round-trips,
  Pipelines, one StreamBased); the packet-flow tests (`PacketIoTests`,
  `PacketSubscriptionsTests`, `GeneratedRoundTripTests`) pass.
- Nuke wrappers (`build.ps1` / `build.cmd`) exist, but their Tests/Pack
  targets are empty; NuGet publishing is effectively off.

## Contract vs clay

- **Treat as fixed:** transport and Serialization (tested, benchmarked); the
  `Flow/` contract shapes (owner decisions 2026-08-08/09: packets are
  classes — one allocation per packet, no boxing anywhere); and the
  performance philosophy the benchmarks encode — no runtime reflection (the
  single cold-path exception is `MinecraftVersion.BuildKnownVersions`), no
  boxing on dispatch, AOT-compatible.
- **Treat as clay, but only through specs:** everything under `Generated/`.
  Its shape follows the `minecraft-protocol-fs` codegen; wrong output means
  fixing specs or codegen there, never patching the delivered files.
- **Treat as clay:** the rest of Protocol's hand-written surface
  (`Exceptions/`, `Position`, `ProtocolRange`, …). Not published, no external
  consumers; breaking it is allowed when the design calls for it.

## Guardrails

- Protocol facts (packet ids, field layouts, version ranges): the workspace
  root AGENTS.md rule applies — McProtoFacts surfaces only, never raw
  minecraft-data, never memory.
- Never hand-edit `src/McProtoNet.Protocol/Generated/**`.
- Do not break the hot-path contract: no reflection, no boxing;
  `InputPacket.Data` is valid only until the next transport read — decode
  before any `await`; `MemoryOwner<byte>` must be disposed by its final
  owner.
- Do not commit or push without an explicit request. The single standing
  exception is the end-of-packet-cycle commit pair (workspace
  `docs/rules.md`); this repo's commit of delivered `Generated/` files is
  step 2 of that pair.
- When merging or rewriting docs, synthesize — do not concatenate indexes.

## Where the details are

- Packet-layer design and owner decisions:
  `../docs/design/packet-api-2026-08-08/` (workspace repo this clone lives
  inside; local only).
- Full anatomy with file/line evidence:
  `../docs/design/mcprotonet-anatomy.md` — snapshot of f9fd575, 2026-07-24.
  It predates the packet symbiosis: trust it for transport and
  serialization, not for the packet layer.
- AI-layer research and rationale for this file:
  `docs/research/mcprotonet-ai-research-compendium.md`.
- Writerside docs in `docs/topics/` — useful for intent, stale in places
  (anything mentioning McProtoNet.Abstractions).
