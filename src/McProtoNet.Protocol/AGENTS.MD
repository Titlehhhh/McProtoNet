# AGENTS.md

## Rules for Generating C# Code from `minecraft-data`

This document defines **strict rules** for AI agents that generate C# code from the `minecraft-data` repository.
The agent MUST follow these rules exactly.
If any rule cannot be satisfied, the agent MUST stop generation and report the reason.

---

## 1. Data Sources

The `minecraft-data` directory contains:

* `types/` — protocol data types
* packet folders:

    * `configuration/`
    * `login/`
    * `play/`
    * `status/`
    * `handshaking/`
* `info.txt`
* `PROTODEF.MD` — authoritative reference for ProtoDef native types and wire semantics
  (path: `src/McProtoNet.Protocol/minecraft-data/PROTODEF.MD`)
* `AGENTS_SERIALIZATION.MD` — compact index of available read/write helpers
* File names may appear with `.MD` or `.md` casing; search both when locating agent docs.

### Naming note (minecraft-data quirks)

The upstream `minecraft-data` repository contains historical name changes such as:
- `slot` → `Slot`
- `particle` → `Particle`

Some generated JSON files therefore appear as `Slot_1.json`, `Particle_1.json`, etc.
These are **name artifacts**, not distinct types. Treat them as the same semantic
types as `Slot` and `Particle`, and do NOT generate duplicate C# types.

### `info.txt`

`minecraft-data/info.txt` defines global protocol version bounds:

* `startVersion`
* `latestVersion`

These values define the **absolute supported protocol range**.

---

## 2. Protocol Version Rules

* Hardcoding **intermediate protocol versions** is ALLOWED.
* Hardcoding `startVersion` or `latestVersion` as numeric literals is FORBIDDEN.
* If a version range boundary equals `startVersion`, use:

    * `MinecraftVersion.StartProtocol`
* If a version range boundary equals `latestVersion`, use:

    * `MinecraftVersion.LatestProtocol`

### Current Update Target

* Treat existing generated code as legacy; regeneration may replace or break it.
* Current target range is **1.16–1.21.8** (may extend to **1.21.10**).

---

## 3. JSON `history` Format

* Each `history` entry key is a **version range (inclusive)**.
* A `null` value means the type or packet does NOT exist in that range.
* A non-null value means the structure exists.
* Version ranges MUST be taken **strictly from JSON**.
* Version ranges MUST NOT be guessed or inferred.

---

## 4. JSON ↔ C# Structure Mapping

### General Rule

* Generated C# structures MUST match JSON structures **1:1**.

This rule applies **unless explicitly overridden** by a more specific rule below.

---

## 5. ProtocolSupport Generation

* Intermediate protocol versions MAY be specified as numbers.
* `start` / `latest` MUST be expressed using:

    * `MinecraftVersion.StartProtocol`
    * `MinecraftVersion.LatestProtocol`

---

## 6. Read / Write Method Rules

* Read* / Write* methods MUST accept `protocolVersion`.
* The FIRST line MUST call:

    * `ThrowIfProtocolNotSupported<T>()`
* Read/write order MUST exactly follow JSON field order **for the active schema of that version**.

---

## 6.0 ThrowHelper Usage (CRITICAL)

* Use `ThrowHelper` helpers instead of `new` exceptions in generated code.
* Protocol support failures MUST call `ThrowHelper` (especially `ProtocolNotSupportException`).

---

## 6.1 Protocol Serialization Extension Layer (CRITICAL)

McProtoNet.Protocol defines protocol-level serialization primitives as
extension methods on:

* `MinecraftPrimitiveReader`
* `MinecraftPrimitiveWriter`

These live under `McProtoNet.Protocol.Extensions`
(e.g. `ProtocolSerializationExtensions*.cs`).

Rules:

* This layer implements **primitive wire-level serialization only**.
* Generic helpers such as `WriteType<T>` / `ReadType<T>` are **extension points**, NOT implementations.
* Agents MUST NOT introduce reflection-based or generic serializers.
* Data types MUST NOT embed primitive serialization logic.
* `Read*` / `Write*` methods orchestrate serialization by calling these extensions.


## 6.2 Location of Read / Write Methods (CRITICAL)

Read* / Write* methods MUST NOT be declared inside data type definitions.

Rules:
- Data types (including Slot, SlotComponent, union variants, modifiers, aliases)
  MUST contain semantic data ONLY.
- Data types MUST NOT contain:
  - instance Read/Write methods
  - static Read/Write methods
  - any serialization logic
- All Read* / Write* methods for data types MUST be implemented as
  EXTENSION METHODS on:
    - MinecraftPrimitiveReader
    - MinecraftPrimitiveWriter
- These extension methods MUST live under the
  `McProtoNet.Protocol.Extensions` namespace.

Allowed:
- `reader.ReadSlot(protocolVersion)`
- `writer.WriteSlot(slot, protocolVersion)`

Forbidden:
- `Slot.Read(ref reader, protocolVersion)`
- `slot.Write(ref writer, protocolVersion)`
- any Read/Write method declared inside a data type

---

## 7. Packets vs Data Types (CRITICAL)

### Packets

* Packets are **versioned protocol containers**.
* Packets MAY contain version-specific field groups.
* Packets MAY expose protocol-version-dependent structure.
* Packets consume data types but do NOT define data type modeling rules.

### Data Types

* Data types MUST NOT be version-aware.
* Data types MUST represent **semantic data**, not historical wire layouts.
* Protocol versions affect **HOW data is serialized**, not **WHAT the type represents**.

Forbidden for data types:

* Version-specific field groups (`VxxxToYyyFields`)
* Storing `protocolVersion` inside the type
* Duplicating historical layouts as separate fields

Allowed data type forms:

1. Simple struct/class for stable data
2. Unified semantic type with version-dependent serialization
3. Wrapper / modifier types (e.g. `UntrustedSlot`, `HashedSlot`)
4. Discriminated union types (see below)

---

## 8. Complex / Evolving Types (CRITICAL)

Some types are **evolving type families**, not single structures
(example: `Slot`).

Such types may:

* Have multiple historical names
* Change internal structure across versions
* Be decomposed into components
* Become aliases of other types
* Gain semantic modifiers (e.g. `Untrusted`, `Hashed`)

Type names are NOT a source of truth.
ONLY JSON structure and `history` define semantics.

---

## 9. ProtoDef Native Types (CRITICAL)

minecraft-data schemas use ProtoDef and reference **native types**
(see `PROTODEF.MD`).

If a schema references a native ProtoDef type:
- Do NOT look for a JSON type definition for it.
- Use the project’s serialization primitives (reader/writer extensions) that implement it.
- If the primitive is missing, treat it as a TODO in the serialization layer, not a schema ambiguity.

---

## 10. Packet Generation Notes

Packet format rules live in `src/McProtoNet.Protocol/Packets/AGENTS.MD`.
