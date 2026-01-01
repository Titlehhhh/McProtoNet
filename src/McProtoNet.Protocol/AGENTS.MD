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
* `PROTODEF.md` — authoritative reference for ProtoDef native types and wire semantics

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

* `Read*` / `Write*` methods MUST accept `protocolVersion`.
* The FIRST line MUST call:

    * `ThrowIfProtocolNotSupported<T>()`
* Read/write order MUST exactly follow JSON field order **for the active schema of that version**.

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
* Packets MAY contain version-specific field groups (e.g. `V765To767Fields`).
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
(e.g. `varint`, `switch`, `container`,
`registryEntryHolder`, `registryEntryHolderSet`).

Native types:

* Are built-in ProtoDef wire primitives or combinators
* Do NOT require JSON definitions in `minecraft-data/types/*.json`

The authoritative description of these types is `minecraft-data/PROTODEF.md`.

Rules:

* Agents MUST NOT stop generation due to missing JSON definitions for native types.
* Native types MUST be handled in the serialization extension layer.
* If a native type is referenced but not yet implemented:

    * The agent MUST NOT guess its wire format.
    * The agent MUST generate a TODO / placeholder in serialization code.
    * This is NOT a schema ambiguity.

---

## 10. Component-Based Variant Types (CRITICAL)

Some data types represent a collection of keyed components
(example: `SlotComponent`).

Each component ID has its own schema which may evolve across versions.

### Variant Model Rule

* Component-based types MUST be modeled as **tagged unions / discriminated unions**.
* Each component ID corresponds to ONE semantic variant.
* Variants represent **meaning**, not protocol versions.

### Dunet

* Discriminated union libraries (e.g. **Dunet**) are ALLOWED and RECOMMENDED.
* Union cases represent semantic variants, not historical layouts.

---

### Exception to 1:1 Mapping Rule (CRITICAL)

For **Component-Based Variant Types only**:

* The “1:1 JSON ↔ C# structure mapping” rule DOES NOT apply
  to the **variant data model shape**.
* Variant data models represent a **semantic superset**
  of all JSON schemas for that component ID.
* Fields absent in some versions MUST be nullable or optional.
* Primitive ↔ container ↔ void shape changes MUST be unified
  into a single semantic representation.
* Exact JSON layouts MUST be handled **exclusively in Read*/Write***.

The 1:1 mapping rule REMAINS mandatory for:

* Packet layouts
* Non-component data types
* Serialization logic inside `Read*/Write*`

---

## 11. Void / Presence Semantics

For component variants:

* `void` represents **presence-only semantics**.
* Presence-only components MAY be represented as:

    * nullable fields
    * boolean flags
    * empty marker union cases

As long as wire semantics are preserved in serialization,
the internal representation is considered valid.

---

## 12. Modifiers and Wrappers

Types such as:

* `UntrustedSlot`
* `HashedSlot`

Rules:

* MUST be wrappers or tagged types over core types.
* MUST NOT duplicate core structure.
* MUST NOT be generated before the core type exists.

---

## 13. Alias Types

If a type is an alias in some version ranges:

* Alias code MUST be generated only after the target type exists.
* Alias types MUST NOT contain serialization logic.

---

## 14. Null History Ranges

* `null` ranges in `history` MUST NOT generate any code.
* Absence is NOT a structure.

---

## 15. Partial Types

* `partial` MUST be used to separate:

    * auto-generated code
    * manual / handwritten code
* Updating `minecraft-data` MUST NOT require manual C# edits.

---

## 16. Behavior on Uncertainty

If:

* Type evolution is ambiguous **after applying the above exceptions**
* Serialization semantics cannot be implemented without guessing

The agent MUST:

* STOP generation
* Report the exact blocking reason
* NOT guess
* NOT invent structures

---

## 17. Explicitly Forbidden

* Hardcoding `startVersion` / `latestVersion` as numbers
* Inventing structures not present or permitted by rules
* Making data types version-aware
* Generating packets before completing data type generation
* Proceeding when rules are unclear

---

## Dependency Scope for Complex Types (CRITICAL)

Some complex types (e.g. SlotComponent) reference other non-slot data types
as part of their payload schemas.

Rules:
- Agents are NOT required to fully generate all transitive dependencies
  of a complex type unless explicitly instructed.
- If a referenced dependency is:
  - non-native
  - outside the current task scope
    then the agent MAY generate a placeholder data type.

Placeholder rules:
- The placeholder type MUST have a name matching the JSON reference.
- The placeholder MUST NOT invent fields or semantics.
- Read*/Write* methods for placeholders MUST:
  - throw NotImplementedException, or
  - contain an explicit TODO indicating the missing dependency.
- Using placeholders for non-native dependencies is NOT considered guessing.

This rule applies ONLY to dependencies of complex types
and MUST NOT be used to bypass core modeling rules.

## Final Rule

> When in doubt, STOP.
> Incorrect code generation is worse than no code generation.
