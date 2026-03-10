# Data Type Rules (AI)

This file is the single source of truth for AI data type generation.
If any rule cannot be satisfied, STOP and ask for guidance.

## 1. Sources of truth (use in this order)

1) `src/McProtoNet.Protocol/AGENTS_SERIALIZATION.MD` — read/write helper index.

Do NOT scan `ProtocolSerializationExtensions*` files. If a helper is missing from
`AGENTS_SERIALIZATION.MD`, STOP and ask.



## 2. Data types vs packets (CRITICAL)

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

## 3. Read / Write methods (CRITICAL)

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

## 4. Complex / evolving types (CRITICAL)

Some types are evolving families (example: `Slot`).
Type names are NOT a source of truth.
ONLY protocol structure and `history` define semantics.

These types may:
- change internal structure across versions
- be decomposed into components
- become aliases of other types
- gain semantic modifiers (e.g. `Untrusted`, `Hashed`)

Do NOT generate duplicate semantic types for `*_1` aliases.

## 5. ProtoDef native types (CRITICAL)

If a schema references a native ProtoDef type:
- Use the project’s serialization primitives (reader/writer extensions) that implement it.
- If the primitive is missing, STOP and ask.
