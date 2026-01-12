# mcdata examples

All outputs are TOON (tab-delimited, keyFolding safe).
Delimiter defaults to comma; set `--delimiter "\\t"` to use tabs.

## List roots

Command:
```bash
node tools/agent/mcdata.mjs list --roots
```

Output:
```toon
roots[6]: configuration,handshaking,login,play,status,types
```

## List types (first line)

Command:
```bash
node tools/agent/mcdata.mjs list --types | head -n 1
```

Output:
```toon
types[66]: ArmorTrimMaterial,ArmorTrimPattern,BannerPattern,BannerPatternLayer,ByteArray,ChatSession,ChunkBlockEntity,CommandNode,ContainerID,DataComponentMatchers,EntityMetadata,EntityMetadataEntry,EntityMetadataItem,EntityMetadataPaintingVariant,EntityMetadataWolfVariant,ExactComponentMatcher,GameProfile,HashedSlot,IDSet,Ingredient,InstrumentData,ItemBlockPredicate,ItemBlockProperty,ItemBookPage,ItemConsumeEffect,ItemEffectDetail,ItemFireworkExplosion,ItemPotionEffect,ItemSoundEvent,ItemSoundHolder,ItemWrittenBookPage,JukeboxSongData,MinecraftSimpleRecipeFormat,MinecraftSmeltingFormat,Optvarint,PackedChunkPos,PacketCommonAddResourcePack,PacketCommonClearDialog,PacketCommonCookieRequest,PacketCommonCookieResponse,PacketCommonCustomClickAction,PacketCommonCustomReportDetails,PacketCommonRemoveResourcePack,PacketCommonSelectKnownPacks,PacketCommonServerLinks,PacketCommonSettings,PacketCommonStoreCookie,PacketCommonTransfer,Particle,ParticleData,Position,PreviousMessages,ServerLinkType,Slot,SlotComponent,SlotComponentType,SoundSource,String,Tags,UntrustedSlot,UntrustedSlotComponent,Vec2f,Vec3f,Vec3f64,Vec3i,Vec4f
```

## List play/toClient packets (first 3 lines)

Command:
```bash
node tools/agent/mcdata.mjs list --packets --state play --direction toClient | head -n 3
```

Output:
```toon
state: play
direction: toClient
packets[146]: PacketAbilities,PacketAcknowledgePlayerDigging,PacketActionBar,PacketAddResourcePack,PacketAdvancements,PacketAnimation,PacketAttachEntity,PacketBlockAction,PacketBlockBreakAnimation,PacketBlockChange,PacketBossBar,PacketCamera,PacketChat,PacketChatPreview,PacketChatSuggestions,PacketChunkBatchFinished,PacketChunkBatchStart,PacketChunkBiomes,PacketClearTitles,PacketCloseWindow,PacketCollect,PacketCombatEvent,PacketCraftProgressBar,PacketCraftRecipeResponse,PacketCustomPayload,PacketDamageEvent,PacketDeathCombatEvent,PacketDebugSample,PacketDeclareCommands,PacketDeclareRecipes,PacketDestroyEntity,PacketDifficulty,PacketEndCombatEvent,PacketEnterCombatEvent,PacketEntity,PacketEntityDestroy,PacketEntityEffect,PacketEntityEquipment,PacketEntityHeadRotation,PacketEntityLook,PacketEntityMetadata,PacketEntityMoveLook,PacketEntitySoundEffect,PacketEntityStatus,PacketEntityTeleport,PacketEntityUpdateAttributes,PacketEntityVelocity,PacketExperience,PacketExplosion,PacketFacePlayer,PacketFeatureFlags,PacketGameStateChange,PacketHeldItemSlot,PacketHideMessage,PacketHurtAnimation,PacketInitializeWorldBorder,PacketKeepAlive,PacketKickDisconnect,PacketLogin,PacketMap,PacketMapChunk,PacketMessageHeader,PacketMoveMinecart,PacketMultiBlockChange,PacketNamedEntitySpawn,PacketNamedSoundEffect,PacketNbtQueryResponse,PacketOpenBook,PacketOpenHorseWindow,PacketOpenSignEntity,PacketOpenWindow,PacketPing,PacketPingResponse,PacketPlayerChat,PacketPlayerInfo,PacketPlayerRemove,PacketPlayerRotation,PacketPlayerlistHeader,PacketPosition,PacketProfilelessChat,PacketRecipeBookAdd,PacketRecipeBookRemove,PacketRecipeBookSettings,PacketRelEntityMove,PacketRemoveEntityEffect,PacketRemoveResourcePack,PacketResetScore,PacketResourcePackSend,PacketRespawn,PacketScoreboardDisplayObjective,PacketScoreboardObjective,PacketScoreboardScore,PacketSculkVibrationSignal,PacketSelectAdvancementTab,PacketServerData,PacketSetCooldown,PacketSetCursorItem,PacketSetPassengers,PacketSetPlayerInventory,PacketSetProjectilePower,PacketSetSlot,PacketSetTickingState,PacketSetTitleSubtitle,PacketSetTitleText,PacketSetTitleTime,PacketShouldDisplayChatPreview,PacketShowDialog,PacketSimulationDistance,PacketSoundEffect,PacketSpawnEntity,PacketSpawnEntityExperienceOrb,PacketSpawnEntityLiving,PacketSpawnEntityPainting,PacketSpawnPosition,PacketStartConfiguration,PacketStatistics,PacketStepTick,PacketStopSound,PacketSyncEntityPosition,PacketSystemChat,PacketTabComplete,PacketTags,PacketTeams,PacketTestInstanceBlockStatus,PacketTileEntityData,PacketTitle,PacketTrackedWaypoint,PacketTradeList,PacketTransaction,PacketUnloadChunk,PacketUnlockRecipes,PacketUpdateHealth,PacketUpdateLight,PacketUpdateTime,PacketUpdateViewDistance,PacketUpdateViewPosition,PacketVehicleMove,PacketWindowItems,PacketWorldBorder,PacketWorldBorderCenter,PacketWorldBorderLerpSize,PacketWorldBorderSize,PacketWorldBorderWarningDelay,PacketWorldBorderWarningReach,PacketWorldEvent,PacketWorldParticles
```

## History for Slot

Command:
```bash
node tools/agent/mcdata.mjs history --type Slot
```

Output:
```toon
name: Slot
ranges[4]{range,exists}:
  735-763	true
  764-765	true
  "766"	true
  767-772	true
```

## History for PacketChat (play/toClient)

Command:
```bash
node tools/agent/mcdata.mjs history --packet PacketChat --state play --direction toClient --include-missing | head -n 6
```

Output:
```toon
name: PacketChat
ranges[2]{range,exists}:
  735-758	true
  759-772	false
```

## Schema for Slot in protocol 764

Command:
```bash
node tools/agent/mcdata.mjs schema --type Slot --version 764 | head -n 8
```

Output:
```toon
name: Slot
version: 764
range: 764-765
schema[2]:
  - container
  - [2]:
    - name: present
      type: bool
```

Note: this example uses `head`, so the schema output is truncated.
