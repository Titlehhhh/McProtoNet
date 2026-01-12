# Packet Generation Rules (AI)

This file is the single source of truth for AI packet generation in this repo.
If any rule cannot be satisfied, STOP and ask for guidance.

## 1. Sources of truth (use in this order)

1) `node tools/agent/mcdata.mjs` — preferred query tool for minecraft-data.
2) `src/McProtoNet.Protocol/minecraft-data/PROTODEF.MD` — native ProtoDef types.
3) `src/McProtoNet.Protocol/AGENTS_SERIALIZATION.MD` — read/write helper index.
4) `src/McProtoNet.Protocol/minecraft-data/**` — direct JSON reads **only if** the tool
   cannot answer.

Do NOT scan `ProtocolSerializationExtensions*` files. If a helper is missing from
`AGENTS_SERIALIZATION.MD`, STOP and ask.

## 2. mcdata tool usage (preferred)

Use:
```
node tools/agent/mcdata.mjs <command> [options]
```

The tool:
- outputs TOON (comma delimiter, keyFolding safe)
- merges `*_1` aliases (e.g., `Slot_1` + `Slot`)
- can filter by protocol number via `--version`

Common commands:
- `list --packets --state <state> --direction <toClient|toServer>`
- `schema --packet <PacketName> --state <state> --direction <dir> [--version <num>]`
- `history --packet <PacketName> --state <state> --direction <dir> [--include-missing]`
- `schema --type <TypeName> [--version <num>]`
- `history --type <TypeName> [--include-missing]`

Protocol version is a number from `info.txt` (e.g. 764), not a game version string.

## 3. Packet list source (CRITICAL)

Packet lists come from `Packet*.json` in `minecraft-data/<state>/<direction>/`.
Do NOT use `Packet.json` for enumeration (it is metadata only).

## 4. Version rules

- Do NOT hardcode `startVersion` or `latestVersion` as numbers.
- Use `MinecraftVersion.StartProtocol` / `MinecraftVersion.LatestProtocol`.
- Version ranges must come strictly from JSON `history`.
- Do NOT infer missing ranges or fields.

## 5. JSON history semantics

- Each `history` key is an inclusive version range.
- `null` means the type/packet does not exist in that range.
- Non-null values are the schema for that range.

## 6. Packet class shape

- One sealed packet class, no nested impl, no base class.
- All packets MUST be `partial`.
- `Serialize` / `Deserialize` must be `internal`.
- Implement `IPacket.Serialize` / `IPacket.Deserialize` explicitly and route to internal methods.
- Use protocol-version switches inside `Serialize` / `Deserialize`.
- Use `ThrowHelper` for protocol support failures (no `new` exceptions).
- Use `ReadArray<T>` / `WriteArray<T>` helpers where possible.
- Do NOT add IDs or marshalling logic.

## 7. Data types vs packets

- Packets may be versioned; data types must be semantic and version-agnostic.
- Do NOT embed serialization logic inside data types.
- All data type read/write lives in extension methods on reader/writer classes.

## 8. ProtoDef native types

If a schema references a native ProtoDef type:
- Do NOT look for a JSON definition.
- Use the existing primitive reader/writer helpers.
- If missing, STOP and ask.
