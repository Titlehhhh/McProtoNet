# McProtoNet agent guide

.NET library for the Minecraft Java Edition protocol (client side): transport,
primitives, packets, multi-version support. Active branch:
`refactoring/layout-2.0` (layout 2.0, phase 1 — canon:
`../diagrams/layout.md`). Honest protocol range in code: **735–772**
(1.16 → 1.21.8; `MinecraftVersion.StartProtocol` / `LatestProtocol`) — the
README agrees since 2026-08-15.

Maintenance rule: if a PR changes an architectural fact stated here, update
this file in the same PR. Snapshot facts below are marked with their commit;
unmarked statements are design invariants.

## Repository map

| Path | Role | State (2026-08-21, on 1abac13) |
| --- | --- | --- |
| src/McProtoNet.Primitives | primitive reader/writer, buffers, `IncomingPacket`/`OutgoingPacket` | alive, tested |
| src/McProtoNet.NBT | own NBT parser | alive, tested |
| src/McProtoNet.Transport | bytes to packets: `Connection/`, `Framing/`, `Compression/`, `Cryptography/`, `Pipelines/` | alive, tested |
| src/McProtoNet.Protocol | packet layer: hand-written `Flow/` + delivered `Generated/` | alive, builds |
| src/McProtoNet | glue: `MinecraftClient`, `ClientPacketExtensions`, SRV lookup, LAN detect | alive |
| src/McProtoNet.SourceGenerator | ONE Roslyn generator: `VersionRangeGenerator` | alive |
| examples/MinimalBot | executable documentation of the packet layer | alive; in the slnx under /samples/ |
| examples/FormationBots | swarm demo: 117 bots form text on chat command; carries the full phase logic incl. the offline encryption handshake | alive; in the slnx under /samples/ |
| tests/McProtoNet.Tests | xUnit v3: transport, primitives, NBT, packet-flow round-trips | alive; all green (see Commands) |
| benchmarks/ | BenchmarkDotNet, perf contract lives here | alive |
| docs/ | Writerside site | mixed, stale in places |
| build/ | Nuke build; Tests/Pack targets are hollowed out | stale |

Removed 2026-08-15: the dead `IMinecraftClient`/start options, the F# facade
stub and the `examples/SimpleBotFSharp` stub. Removed 2026-08-21 by the
layout 2.0 refactor: `src/McProtoNet.Utils` (folded into the glue),
`src/McProtoNet.Abstractions`, `src/McProtoNet.FSharp` leftovers, `TestServer/`.

Renames of the same refactor: `McProtoNet.Serialization` →
`McProtoNet.Primitives`, the old core project `McProtoNet` →
`McProtoNet.Transport` (`McProtoNet.Net` → `McProtoNet.Transport.Framing`,
`McProtoNet.Net.Pipelines` → `McProtoNet.Transport.Pipelines`,
`McProtoNet.Net.Zlib` → `McProtoNet.Transport.Compression`,
`McProtoNet.Cryptography` → `McProtoNet.Transport.Cryptography`),
`InputPacket` → `IncomingPacket`, `OutputPacket` → `OutgoingPacket`,
`MinecraftPacketReader`/`MinecraftPacketSender` →
`PacketStreamReader`/`PacketStreamWriter`, `CryptoHandler` →
`EncryptionHelpers`.

## Architecture in one screen

Since 2026-08-21 (layout 2.0, phase 1) the two floors stand side by side and a
third one joins them. `McProtoNet.Transport` and `McProtoNet.Protocol` know
nothing of each other; both reference `McProtoNet.Primitives`. The glue
project `McProtoNet` references both, and a client app references the glue.

- **Primitives (bottom):** `MinecraftPrimitiveReader`/`Writer`, `MemoryOwner`,
  and the two packet terms every floor shares —
  `IncomingPacket { int Id; ReadOnlySequence<byte> Data }` and
  `OutgoingPacket`. Depends on `McProtoNet.NBT`, nothing else.
- **Transport:** `MinecraftConnection` (`Connection/`) pumps a duplex
  `Stream` through `MinecraftPacketPipeReader/Writer` (`Pipelines/`) on
  System.IO.Pipelines. Yields raw `IncomingPacket` via `ReadPacketsAsync()`.
  `Data` is a window into the pipe buffer — valid only until the next read;
  decode immediately, never across an `await`. Sends are serialized by an
  internal gate; pump failures on either side complete the app-side pipe with
  the error; clean EOF ends enumeration without an exception; `DisposeAsync`
  fences senders and releases buffers after both pumps stop. `Framing/` holds
  the one-at-a-time `PacketStreamReader`/`PacketStreamWriter` over a `Stream`
  and the `PacketWriteExtensions` frame bricks. Compression is libdeflate
  (`McProtoNet.Native`) in `Compression/`, encryption is the library's own
  AES/CFB8 `PacketCipher` with hardware cores (x86 AES-NI, ARM64 NEON; scalar
  fallback) in `Cryptography/`.
- **Glue (`src/McProtoNet`):** `MinecraftClient` is a thin standard client by
  owner decision 2026-08-15: options, TCP connect, packet read/send, cipher
  and compression switches — nothing else. `ClientPacketExtensions` carries
  typed `SendAsync<T>` and `SendRawAsync`. SRV lookup and LAN detection live
  here too. The handshake/login state machine lives in consumer code, not in
  the library (see examples/).
- **Packet layer, hand-written contract (`src/McProtoNet.Protocol/Flow/`):**
  `IPacket<TSelf>` with static abstract `Identity` and
  `TryGetPacketId(pv, out id)`; `PacketIdentity` (manifest key, name, phase,
  direction, dense per-catalog `Ordinal` — the currency of the hot path);
  `IPacketVisitor` (`Visit<T>` typed, `Unknown` for unmapped ids — a normal
  stream condition, not an error); `PacketIo` (`TryDecode`/`Decode`:
  IncomingPacket → concrete packet, trailing bytes =
  `DecodeError.TrailingBytes`); `PacketSubscriptions` (handler slots, one
  catalog per (phase, direction) with the ordinal indexing inside it —
  `Ordinal` is dense only within one catalog, so it is not a key on its own);
  `DecodeError`, `PacketExceptions`. Typed send lives in the glue, not
  here — Protocol must not see the transport.
- **Packet layer, generated implementation
  (`src/McProtoNet.Protocol/Generated/`, 267 files at 1abac13):**
  `Packets/<Phase>/<Direction>/*.cs` — sealed partial records carrying
  `[Packet]`, `[PacketField]`, `[ProtocolSupport]`
  (`src/McProtoNet.Protocol/Attributes/`) with per-version `Read`/`Write`;
  `Types/`, `Bitflags/` and `Enums/` (record structs over one `int`, unknown
  ids kept verbatim) for nested protocol types; `Flow/` with
  `PacketRegistry.g.cs` (descriptor catalogs cold; hot is one flat blob of
  every id→ordinal table, addressed by arithmetic over `(int)phase`,
  `(int)direction` and the protocol version — phase and direction are bounded
  separately, or an out-of-range direction would alias onto another phase's
  row; `PhaseCount`/`DirectionCount`/`CatalogCount` are public so callers
  index by the same numbers instead of reflecting; every entry point is Try),
  `PacketFlow.g.cs` (`Dispatch`: id → ordinal → typed decode →
  `visitor.Visit<T>`; unknown ids fall through to `Unknown`; trailing bytes
  raise the `OnTrailingBytes` hook), and `ClientboundHandler.g.cs` /
  `ServerboundHandler.g.cs` (abstract handler base per direction:
  `HandleAsync(in IncomingPacket, pv)`, consumer-owned `Phase` — `Login` and
  `Handshaking` respectively — one virtual `On<Name>` per packet). A handler
  is deliberately **not** an `IPacketVisitor`: `HandleAsync` goes id → ordinal
  → `Read` → `On<Name>` in one case block, where the packet type is statically
  known. Routing it through `PacketFlow` instead would hand `Visit<T>` a
  `ValueTask` it has nowhere to return, silently dropping the continuation.
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
  Primitives + NBT + the generator);
  `dotnet build examples/MinimalBot`. TFMs: net8.0/net9.0/net10.0.
- `dotnet build McProtoNet.slnx -c Release` is green since 2026-08-21: the
  layout 2.0 refactor dropped `TestServer/` and pointed
  `.nuke/parameters.json` at the `.slnx`.
- Tests: `dotnet run --project tests/McProtoNet.Tests` — the project is an
  xUnit v3 executable; plain `dotnet test` on it discovers no tests (no
  VSTest adapter). Since 2026-08-15 the suite is fully green: 10671 tests,
  0 failing, 9 skipped (ARM cipher core on x86 machines).
- Nuke wrappers (`build.ps1` / `build.cmd`) exist, but their Tests/Pack
  targets are empty; NuGet publishing is effectively off.

## Contract vs clay

- **Treat as fixed:** Transport and Primitives (tested, benchmarked); the
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
  `IncomingPacket.Data` is valid only until the next transport read — decode
  before any `await`; `MemoryOwner<byte>` must be disposed by its final
  owner.
- Keep the layers apart: `McProtoNet.Protocol` must not reference
  `McProtoNet.Transport`, and the transport must not learn about phases or
  protocol versions. Anything that needs both belongs in the glue.
- Do not commit or push without an explicit request. The single standing
  exception is the end-of-packet-cycle commit pair (workspace
  `docs/rules.md`); this repo's commit of delivered `Generated/` files is
  step 2 of that pair.
- When merging or rewriting docs, synthesize — do not concatenate indexes.

## Where the details are

- Packet-layer design and owner decisions:
  `../docs/design/packet-api-2026-08-08/` (workspace repo this clone lives
  inside; local only).
- Target layout and what each package owes: `../diagrams/layout.md`; the
  phase 1 plan behind the current branch:
  `../docs/plans/2026-08-21-layout-2.0.md`.
- Full anatomy with file/line evidence:
  `../docs/design/mcprotonet-anatomy.md` — snapshot of f9fd575, 2026-07-24.
  It predates both the packet symbiosis and layout 2.0: trust it for the
  transport and primitive mechanics, not for names or the packet layer.
- AI-layer research and rationale for this file:
  `docs/research/mcprotonet-ai-research-compendium.md`.
- Writerside docs in `docs/topics/` — useful for intent, stale in places
  (anything mentioning McProtoNet.Abstractions or the old project names).

<!-- gitnexus:start -->
# GitNexus — Code Intelligence

This project is indexed by GitNexus as **McProtoNet** (6110 symbols, 13839 relationships, 300 execution flows). Use the GitNexus MCP tools to understand code, assess impact, and navigate safely.

> Index stale? Run `node .gitnexus/run.cjs analyze` from the project root — it auto-selects an available runner. No `.gitnexus/run.cjs` yet? `npx gitnexus analyze` (npm 11 crash → `npm i -g gitnexus`; #1939).

## Always Do

- **MUST run impact analysis before editing any symbol.** Before modifying a function, class, or method, run `impact({target: "symbolName", direction: "upstream"})` and report the blast radius (direct callers, affected processes, risk level) to the user.
- **MUST run `detect_changes()` before committing** to verify your changes only affect expected symbols and execution flows. For regression review, compare against the default branch: `detect_changes({scope: "compare", base_ref: "master"})`.
- **MUST warn the user** if impact analysis returns HIGH or CRITICAL risk before proceeding with edits.
- When exploring unfamiliar code, use `query({query: "concept"})` to find execution flows instead of grepping. It returns process-grouped results ranked by relevance.
- When you need full context on a specific symbol — callers, callees, which execution flows it participates in — use `context({name: "symbolName"})`.

## Never Do

- NEVER edit a function, class, or method without first running `impact` on it.
- NEVER ignore HIGH or CRITICAL risk warnings from impact analysis.
- NEVER rename symbols with find-and-replace — use `rename` which understands the call graph.
- NEVER commit changes without running `detect_changes()` to check affected scope.

## Resources

| Resource | Use for |
|----------|---------|
| `gitnexus://repo/McProtoNet/context` | Codebase overview, check index freshness |
| `gitnexus://repo/McProtoNet/clusters` | All functional areas |
| `gitnexus://repo/McProtoNet/processes` | All execution flows |
| `gitnexus://repo/McProtoNet/process/{name}` | Step-by-step execution trace |

## CLI

| Task | Read this skill file |
|------|---------------------|
| Understand architecture / "How does X work?" | `.claude/skills/gitnexus/gitnexus-exploring/SKILL.md` |
| Blast radius / "What breaks if I change X?" | `.claude/skills/gitnexus/gitnexus-impact-analysis/SKILL.md` |
| Trace bugs / "Why is X failing?" | `.claude/skills/gitnexus/gitnexus-debugging/SKILL.md` |
| Rename / extract / split / refactor | `.claude/skills/gitnexus/gitnexus-refactoring/SKILL.md` |
| Tools, resources, schema reference | `.claude/skills/gitnexus/gitnexus-guide/SKILL.md` |
| Index, status, clean, wiki CLI commands | `.claude/skills/gitnexus/gitnexus-cli/SKILL.md` |

<!-- gitnexus:end -->
