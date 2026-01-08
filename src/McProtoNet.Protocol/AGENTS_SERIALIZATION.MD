# AGENTS_SERIALIZATION.MD

## Purpose
This file is a compact index of available protocol read/write helpers to avoid scanning the full
`ProtocolSerializationExtensions*` files.

## Core Primitives (MinecraftPrimitiveReader/Writer)
- bool: ReadBoolean / WriteBoolean
- byte: ReadUnsignedByte / WriteUnsignedByte
- sbyte: ReadSignedByte / WriteSignedByte
- short: ReadSignedShort / WriteSignedShort
- ushort: ReadUnsignedShort / WriteUnsignedShort
- int: ReadSignedInt / WriteSignedInt
- uint: ReadUnsignedInt / WriteUnsignedInt
- long: ReadSignedLong / WriteSignedLong
- ulong: ReadUnsignedLong / WriteUnsignedLong
- float: ReadFloat / WriteFloat
- double: ReadDouble / WriteDouble
- string: ReadString / WriteString
- Guid: ReadUUID / WriteUUID
- buffers: ReadBuffer(len), ReadRestBuffer, WriteBuffer(ReadOnlySpan<byte>)

## Array Helpers
- ReadExtensions:
  - ReadArray<T>(LengthFormat, ReadDelegate<T>)
  - ReadArray<T, TReader>(LengthFormat[, protocolVersion]) for SIMD/readers
- ProtocolSerializationExtensions:
  - WriteArray<T>(ReadOnlySpan<T>[, LengthFormat][, protocolVersion])
  - ReadArray<T>(LengthFormat[, protocolVersion])

## Protocol-Specific Types (ReadType<T>/WriteType<T>)
These are registered in `Extensions/ProtocolSerializationExtensions.cs` and routed via
`ReadType<T>(protocolVersion)` / `WriteType<T>(value, protocolVersion)`.

- Position, Vec2f, Vec3f, Vec3f64, Vec3i, Vec4f
- Slot family: Slot, HashedSlot, UntrustedSlot, SlotComponent, UntrustedSlotComponent, SlotComponentType
- Common types: BannerPattern, DataComponentMatchers, ExactComponentMatcher, GameProfile,
  ItemBlockProperty, ItemEffectDetail, ItemSoundEvent, MinecraftSimpleRecipeFormat,
  PackedChunkPos, ChatSession, Particle, PreviousMessages, ServerLinkType, SoundSource, Tags
- Slot component dependencies: ArmorTrimMaterial, ArmorTrimPattern, BannerPatternLayer,
  EntityMetadataPaintingVariant, EntityMetadataWolfVariant, IDSet, InstrumentData,
  ItemBlockPredicate, ItemBookPage, ItemConsumeEffect, ItemFireworkExplosion,
  ItemPotionEffect, ItemSoundHolder, ItemWrittenBookPage, JukeboxSongData
- Packet common payloads: PacketCommonAddResourcePack, PacketCommonClearDialog,
  PacketCommonCookieRequest, PacketCommonCookieResponse, PacketCommonCustomClickAction,
  PacketCommonCustomReportDetails, PacketCommonRemoveResourcePack,
  PacketCommonSelectKnownPacks, PacketCommonServerLinks, PacketCommonSettings,
  PacketCommonStoreCookie, PacketCommonTransfer
- Play helper types: ChatType, ChatTypeParameterType, ChatTypes, ChatTypesHolder,
  PositionUpdateRelatives, RecipeBookSetting, RecipeDisplay, SlotDisplay, MovementFlags

## RegistryEntryHolder<T>
Supported value types are registered in `ProtocolSerializationExtensions.cs`:
- string, int, ArmorTrimMaterial, ArmorTrimPattern, BannerPattern,
  EntityMetadataPaintingVariant, EntityMetadataWolfVariant, InstrumentData,
  ItemSoundEvent, JukeboxSongData

## NBT Helpers
From `ProtocolSerializationExtensions.Nbt.cs`:
- ReadNbtTag / WriteNbtTag
- ReadOptionalNbtTag / WriteOptionalNbtTag
- ReadAnonymousNbtTag / WriteAnonymousNbtTag
- ReadAnonOptionalNbtTag / WriteAnonOptionalNbtTag

## Notes
- All Read*/Write* methods for protocol types require `protocolVersion` and must call
  `ThrowHelper.ThrowIfProtocolNotSupported<T>(protocolVersion)` on the first line.
- For arrays of protocol-dependent types, use `ReadArray<T>(..., protocolVersion)` and
  `WriteArray<T>(..., protocolVersion)`.
