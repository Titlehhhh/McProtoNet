# Packet Generation Rules (AI)

This file is the single source of truth for AI packet generation in this repo.
If any rule cannot be satisfied, STOP and ask for guidance.

## 1. Sources of truth (use in this order)

1) `src/McProtoNet.Protocol/AGENTS_SERIALIZATION.MD` — read/write helper index.

Do NOT scan `ProtocolSerializationExtensions*` files. If a helper is missing from
`AGENTS_SERIALIZATION.MD`, STOP and ask.



## 2. Packet list source (CRITICAL)

Packet lists are defined in the protocol.
Do NOT use `Packet.json` for enumeration (it is metadata only).

## 3. Version rules

- Do NOT hardcode `startVersion` or `latestVersion` as numbers.
- Use `MinecraftVersion.StartProtocol` / `MinecraftVersion.LatestProtocol`.
- Version ranges must come strictly from protocol history.
- Do NOT infer missing ranges or fields.

## 4. Protocol history semantics

- Each `history` key is an inclusive version range.
- `null` means the type/packet does not exist in that range.
- Non-null values are the schema for that range.

## 5. Packet class shape

- One sealed packet class, no nested impl, no base class.
- All packets MUST be `partial`.
- `Serialize` / `Deserialize` must be `internal`.
- Implement `IPacket.Serialize` / `IPacket.Deserialize` explicitly and route to internal methods.
- Use protocol-version switches inside `Serialize` / `Deserialize`.
- Use `ThrowHelper` for protocol support failures (no `new` exceptions).
- Use `ReadArray<T>` / `WriteArray<T>` helpers where possible.
- Do NOT add IDs or marshalling logic.

## 6. Data types vs packets

- Packets may be versioned; data types must be semantic and version-agnostic.
- Do NOT embed serialization logic inside data types.
- All data type read/write lives in extension methods on reader/writer classes.

## 7. ProtoDef native types

If a schema references a native ProtoDef type:
- Use the existing primitive reader/writer helpers.
- If missing, STOP and ask.
