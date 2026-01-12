# Data Type Rules (AI)

This file is the single source of truth for AI data type generation.
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
- `schema --type <TypeName> [--version <num>]`
- `history --type <TypeName> [--include-missing]`
- `find --type <substring>`

Protocol version is a number from `info.txt` (e.g. 764), not a game version string.

## 3. Data types vs packets (CRITICAL)

Data types MUST NOT be version-aware.
Protocol versions affect **HOW data is serialized**, not **WHAT the type represents**.

Forbidden for data types:
- Version-specific field groups (`VxxxToYyyFields`)
- Storing `protocolVersion` inside the type
- Duplicating historical layouts as separate fields

Allowed data type forms:
1) Simple struct/class for stable data
2) Unified semantic type with version-dependent serialization
3) Wrapper / modifier types (e.g. `UntrustedSlot`, `HashedSlot`)
4) Discriminated union types (when required by schema)

## 4. Read / Write methods (CRITICAL)

Read* / Write* methods MUST NOT be declared inside data type definitions.

Rules:
- Data types contain semantic data ONLY.
- Data types MUST NOT contain any serialization logic.
- All Read* / Write* methods for data types MUST be extension methods on:
  - `MinecraftPrimitiveReader`
  - `MinecraftPrimitiveWriter`
- These extension methods MUST live under `McProtoNet.Protocol.Extensions`.
- First line MUST call `ThrowHelper.ThrowIfProtocolNotSupported<T>(protocolVersion)`.

Allowed:
- `reader.ReadSlot(protocolVersion)`
- `writer.WriteSlot(slot, protocolVersion)`

Forbidden:
- `Slot.Read(ref reader, protocolVersion)`
- `slot.Write(ref writer, protocolVersion)`
- any Read/Write method declared inside a data type

## 5. Complex / evolving types (CRITICAL)

Some types are evolving families (example: `Slot`).
Type names are NOT a source of truth.
ONLY JSON structure and `history` define semantics.

These types may:
- change internal structure across versions
- be decomposed into components
- become aliases of other types
- gain semantic modifiers (e.g. `Untrusted`, `Hashed`)

Do NOT generate duplicate semantic types for `*_1` aliases.

## 6. ProtoDef native types (CRITICAL)

If a schema references a native ProtoDef type:
- Do NOT look for a JSON type definition.
- Use the project’s serialization primitives (reader/writer extensions) that implement it.
- If the primitive is missing, STOP and ask.
