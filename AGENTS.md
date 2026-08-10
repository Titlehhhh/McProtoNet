# McProtoNet agent guide

.NET library for the Minecraft Java Edition protocol (client side): transport,
serialization, packets, multi-version support. Active branch:
`feature/multiversion`. Honest protocol range in code: **735–772**
(1.16 → 1.21.8) — the README's "1.12.2–1.21.4" claim is stale; trust code and
this file over README.

Maintenance rule: if a PR changes an architectural fact stated here, update
this file in the same PR. Snapshot facts below are marked with their commit;
unmarked statements are design invariants.

## Repository map

| Path | Role | State (f9fd575, 2026-07) |
| --- | --- | --- |
| src/McProtoNet | client, transport, crypto, zlib | core alive; legacy `MinecraftClient` broken mid-refactor |
| src/McProtoNet.Protocol | packet classes, types, id registry | alive; Play packets excluded from build by csproj flags |
| src/McProtoNet.Serialization | primitive reader/writer, buffers | alive, tested |
| src/McProtoNet.NBT | own NBT parser | alive |
| src/McProtoNet.SourceGenerator | 3 active Roslyn generators | alive |
| src/McProtoNet.Utils | SRV lookup, LAN detect | periphery |
| src/McProtoNet.FSharp | F# facade | stub |
| src/McProtoNet.Abstractions | — | defunct: no csproj; types moved into McProtoNet |
| tests/McProtoNet.Tests | xUnit v3: transport + serialization round-trips | alive |
| benchmarks/ | BenchmarkDotNet, perf contract lives here | alive |
| docs/ | Writerside site (topics partly stale: Abstractions pages) | mixed |
| build/ | Nuke build; Tests/Pack targets are hollowed out | stale |

## Architecture in one screen

Two working floors and a gap between them.

- **Transport (bottom, works):** `PipelinesMinecraftClient` +
  `MinecraftPacketPipeReader/Writer` on System.IO.Pipelines. Yields raw
  `InputPacket { int Id; ReadOnlySequence<byte> Data }`. `Data` is a window
  into the pipe buffer — valid only until the next read; consumers must
  deserialize immediately or copy. Compression is libdeflate, encryption is
  BouncyCastle AES/CFB8. The handshake/login state machine lives in consumer
  code, not in the library.
- **Protocol (top, works in isolation):** hand-written packet classes carry
  `[PacketInfo]` (name, phase, direction — one pair per class),
  `[ProtocolSupport(from, to)]` and repeated `[PacketId(from, to, hexId)]`
  attributes. Three Roslyn incremental generators turn those into a static
  registry (`PacketIdHelper`: FrozenDictionary keyed by id+version, factories
  are `static () => new T()` lambdas) plus per-class `GetHexId(version)` and
  `IsSupportedVersion`. Version differences inside a packet body follow one
  pattern: shared fields on the class, per-range fields as nested nullable
  `V<from>_<to>Fields` structs, `switch (protocolVersion)` in
  Serialize/Deserialize. Common protocol types live in `Protocol/Types/` as
  plain data; their versioned codecs live in
  `Protocol/Extensions/ProtocolSerializationExtensions.*`.
- **The gap:** the client project does NOT reference Protocol. There is no
  dispatch loop (bytes → typed packet → subscriber), no typed send path, no
  `OnPacket<T>` (only two competing sketches in `docs/code-samples/`). Do not
  assume any of that exists.

Known mines (f9fd575): `PacketMarshaller`'s instance `IsVersionSupported`
guard always returns false (its generator was disabled) — unverified by build
but confirmed by two independent readings; a handful of packets remain in a
pre-attribute convention and are absent from the registry.

## Commands

- Build: `dotnet build McProtoNet.slnx`
- Tests: `dotnet test tests/McProtoNet.Tests`
- Nuke wrappers (`build.ps1` / `build.cmd`) exist, but their Tests/Pack
  targets are currently empty — CI is green but vacuous; NuGet publishing is
  effectively off.
- To compile Play packets, flip `IncludePlayClientboundPackets` /
  `IncludePlayServerboundPackets` in `McProtoNet.Protocol.csproj`.

## Contract vs clay

- **Treat as fixed:** transport and Serialization (tested, benchmarked), and
  the performance philosophy the benchmarks encode — zero allocations per
  packet on the hot path, no runtime reflection (the single cold-path
  exception is `MinecraftVersion.BuildKnownVersions`), generic type dispatch
  via `typeof(T) == typeof(X)` chains that the JIT folds away,
  AOT-compatible.
- **Treat as clay:** the Protocol layer's public shape. It is not published,
  not fully compiled, and has no external consumers. Packet bodies were
  LLM-written by template and are slated for replacement by deterministic
  code generation from the F# DSL (`../minecraft-protocol-fs`, local
  workspace only). Breaking its API is allowed when the design calls for it.

## Guardrails

- Never derive packet ids, field layouts, or version ranges from memory —
  read the attributes in code, or query the McProtoFacts surfaces (see the
  workspace repo; local only).
- Do not break the hot-path contract: no reflection, no per-packet
  allocations, respect `InputPacket`/buffer lifetimes (`MemoryOwner<byte>`
  must be disposed by its final owner).
- Do not edit the source generators casually — every Protocol build depends
  on them.
- Do not commit or push without an explicit request.
- When merging or rewriting docs, synthesize — do not concatenate indexes.

## Where the details are

- Full anatomy with file/line evidence:
  `../docs/design/mcprotonet-anatomy.md` (in the workspace repo this clone
  lives inside; local only; snapshot of f9fd575, 2026-07-24 — verify line
  refs against HEAD before relying on them).
- AI-layer research and rationale for this file:
  `docs/research/mcprotonet-ai-research-compendium.md`.
- Writerside docs in `docs/topics/` — useful for intent, stale in places
  (anything mentioning McProtoNet.Abstractions).
