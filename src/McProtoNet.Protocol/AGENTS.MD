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

* Generated C# structures MUST match JSON structures **1:1**.
* Field order MUST be preserved.
* Field types MUST match JSON definitions.
* Field names MAY be renamed ONLY for style:

   * `camelCase` → `PascalCase`
* Adding, removing, or reordering fields is FORBIDDEN.

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
* Read/write order MUST exactly follow JSON field order.

---

## 6.1 Protocol Serialization Extension Layer

McProtoNet.Protocol defines protocol-level serialization primitives as
extension methods on:

* `MinecraftPrimitiveReader`
* `MinecraftPrimitiveWriter`

These live in `Extensions/ProtocolSerializationExtensions*.cs` under the
`McProtoNet.Protocol.Extensions` namespace.

Rules:

* Extension methods here implement **primitive protocol serialization** only.
* Generic helpers like `WriteType<T>` are **extension points**, not implementations.
* Data types MUST remain semantic and version-agnostic; they must not contain
  serialization logic beyond invoking these extensions in `Read*`/`Write*`.
* Packets orchestrate serialization by calling `Read*`/`Write*` methods, not by
  embedding primitive serialization logic directly.
* Agents MUST NOT add reflection-based or generic serializers to this layer.

---

## 7. Packets vs Data Types (CRITICAL)

### Packets

* Packets are **versioned protocol containers**.
* Packets MAY contain version-specific field groups
  (e.g. `V765To767Fields`).
* Packets MAY expose protocol-version-dependent structure.
* Packets consume data types but do NOT define data type modeling rules.

### Data Types

* Data types MUST NOT be version-aware.
* Data types MUST represent **semantic data**, not historical layouts.
* Protocol versions affect **HOW data is serialized**, not **WHAT the type represents**.

Forbidden for data types:

* Version-specific field groups (`VxxxToYyyFields`)
* Storing `protocolVersion` inside the type
* Duplicating historical layouts as separate fields

Allowed data type forms:

1. Simple struct/class for stable data
2. Unified semantic type with version-dependent serialization
3. Wrapper / modifier types (e.g. `UntrustedSlot`, `HashedSlot`)

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

## Component-Based Variant Types (CRITICAL)

Some data types represent a collection of keyed components
(e.g. SlotComponent), where each component ID has its own evolving schema.

Rules:
- Such types MUST be modeled as tagged unions / variants.
- The component key (ID) selects the variant type.
- Each variant corresponds to a specific component ID.
- Variant data types MAY contain a superset of fields across versions.
- Protocol versions MUST be handled in Read*/Write*, not in the type shape.

Forbidden:
- Inferring semantics beyond JSON structure.
- Making variants version-aware.
- Splitting variants purely by version ranges.

If a component ID has incompatible schemas across versions,
a variant type MUST be generated instead of a unified struct.


## 9. Algorithm for Complex Type Generation

The agent MUST follow this order:

1. Collect ALL related types
   (e.g. `Slot`, `slot`, `*Slot`, `*Component`)
2. Analyze FULL history of ALL collected types
3. Build a complete version → structure map
4. Detect:

   * Renames
   * Decomposition into components
   * Aliases
   * Modifiers / wrappers
5. Generate **core (base) data types first**
6. Generate component types
7. Generate modifier / wrapper types
8. Generate alias types
9. ONLY AFTER ALL TYPES are generated:

   * Generate packets

Forbidden:

* Generating packets before all data types exist
* Generating code without full history analysis
* Inferring relationships not proven by JSON

---

## 10. Decomposition Rule

If a type is decomposed over time
(e.g. monolithic structure → `Item + Component[]`):

* Components MUST be generated as separate data types
* The main type MUST reference components via composition
* Historical monolithic layouts MUST NOT be duplicated as fields

---

## 11. Modifiers and Wrappers

Types such as:

* `UntrustedSlot`
* `HashedSlot`

MUST be treated as:

* Wrappers, derived types, or tagged types

Rules:

* Modifiers MUST NOT duplicate core type structure
* Modifiers MUST NOT be generated before core types
* Modifiers MAY add semantic constraints or extra fields

---

## 12. Alias Types

If a type is an alias in some version ranges:

* Alias code MUST be generated only after the target type exists
* Alias types MUST NOT contain their own serialization logic

---

## 13. Null History Ranges

* `null` ranges in `history` MUST NOT generate any code.
* Absence is NOT a structure.

---

## 14. Partial Types

* `partial` MUST be used to separate:

   * auto-generated code
   * manual / handwritten code
* Updating `minecraft-data` MUST NOT require manual C# edits.

---

## 15. Behavior on Uncertainty

If:

* Type evolution is ambiguous
* Structure changes cannot be reconciled
* Relationships cannot be determined unambiguously

The agent MUST:

* STOP generation
* Report the exact reason
* NOT guess
* NOT invent structures

---

## 16. Explicitly Forbidden

* Hardcoding `startVersion` / `latestVersion` as numbers
* Inventing structures not present in JSON
* Making data types version-aware
* Generating packets before completing data type generation
* Proceeding when rules are unclear

---

## Final Rule

> When in doubt, STOP.
> Incorrect code generation is worse than no code generation.

