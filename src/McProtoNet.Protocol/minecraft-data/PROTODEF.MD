# ProtoDef Native Types Reference (minecraft-data / PrismarineJS)

This repository uses `minecraft-data` schemas that follow the ProtoDef format
from PrismarineJS. The JSON files in `minecraft-data/types/*.json` reference a
set of **native ProtoDef types**.

Native types are **built-in wire primitives / combinators** that are implemented
by the ProtoDef runtime (or by this project’s serialization layer), and therefore
do not require a separate JSON type definition.

## Source of truth

- `minecraft-data` schemas come from PrismarineJS and ProtoDef. 
- Baseline Minecraft network primitive encodings (VarInt, VarLong, Position, Identifier, etc.)
  are documented by the protocol spec. 

## Naming note

ProtoDef native type names used by minecraft-data (e.g. `varint`, `switch`,
`container`, `registryEntryHolder`) are **schema-level names**.
In C#, these map either to:
- dedicated reader/writer primitives, or
- higher-level generated code that composes these primitives.

## Native types used in this repo

### Scalar primitives

- `bool`  
  1 byte: `0x00` false, `0x01` true. 
- `i8`, `i16`, `i32`, `i64`  
  Signed integers, big-endian for fixed-width values. 
- `u8`, `u16`, `u32`, `u64`  
  Unsigned integers, big-endian for fixed-width values. 
- `f32`, `f64`  
  IEEE-754 float/double, big-endian. 
- `UUID`  
  128-bit UUID on the wire (two 64-bit parts).

### Varints / strings / byte sequences

- `varint`  
  Variable-length signed 32-bit integer (max 5 bytes). 
- `varlong`  
  Variable-length signed 64-bit integer (max 10 bytes). 
- `optvarint`  
  ProtoDef alias used by minecraft-data: treat as "optional varint" according to the schema context.
  (Exact optionality is determined by the enclosing schema, not by this name alone.)
- `pstring`  
  Length-prefixed UTF-8 string (typically length is a VarInt in Minecraft protocol). 
- `buffer`  
  Byte sequence whose length is defined by surrounding schema (e.g. known field, varint length, etc.).
- `restBuffer`  
  Remaining bytes until end of packet/field boundary.

### Structural combinators (schema composition)

These types describe *how* to encode/decode fields:

- `container`  
  An ordered set of named fields (like a struct) encoded sequentially.
- `array`  
  A repeated element type, with count/termination defined by the schema (often a VarInt count).
- `option`  
  Optional field; presence is determined by the schema (often a preceding boolean or a switch).
- `switch`  
  Tagged choice: select one of multiple schemas based on a discriminator.
- `void`  
  No payload (used for "present but empty" or "absent" branches).

### Bit-level helpers

- `bitfield`  
  Packs multiple fields into a numeric backing type (bit ranges).
- `bitflags`  
  Packs multiple boolean flags into a numeric backing type.

### Specialized native helpers

- `entityMetadataLoop`  
  Minecraft entity metadata encoding loop (format defined by protocol; schema expresses types per key).
- `topBitSetTerminatedArray`  
  Array terminated by a top-bit marker (used in some protocol encodings).
- `anonymousNbt`  
  NBT payload whose root type is not explicitly named in the schema (NBT encoding per protocol rules). 
- `anonOptionalNbt`  
  Optional NBT payload (presence defined by protocol rules or schema).

## Other types

### registryEntryHolder

Wire format:

- boolean hasInline
- if hasInline == false:
    - VarInt registryId
- else:
    - inline value T, encoded using T's Read/Write logic

Notes:
- The registry type is context-dependent.
- Inline values are used for datapack-defined registry entries.
- Reference form is used for built-in registry entries.


### registryEntryHolderSet

Wire format:

- VarInt count
- repeated registryEntryHolder<T> entries

Notes:
- Each entry follows the registryEntryHolder wire format.
- The registry type is context-dependent.


### entityMetadataLoop

Wire format:

- Repeating entries until index == 0xFF
- Each entry:
  - byte index
  - VarInt typeId
  - value encoded according to typeId

Notes:
- TypeId determines the value wire format.
- The loop terminates with index 0xFF.
- The mapping of typeId → value type is protocol-version dependent,
  but the loop structure itself is stable.


## Implementation rule for this repository

If a schema references a native ProtoDef type:
- Do NOT look for a JSON type definition for it.
- Use the project’s serialization primitives (reader/writer extensions) that implement it.
- If the primitive is missing, treat it as a TODO in the serialization layer, not a schema ambiguity.
