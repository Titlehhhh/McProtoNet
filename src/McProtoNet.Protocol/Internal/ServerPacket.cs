namespace McProtoNet.Protocol;

public enum ServerPacket
{
    SpawnEntity,
    SpawnEntityExperienceOrb,
    SpawnEntityWeather,
    SpawnEntityLiving,
    SpawnEntityPainting,
    NamedEntitySpawn,
    Animation,
    Statistics,
    BlockBreakAnimation,
    TileEntityData,
    BlockAction,
    BlockChange,
    BossBar,
    Difficulty,
    TabComplete,
    Chat,
    MultiBlockChange,
    Transaction,
    CloseWindow,
    OpenWindow,
    WindowItems,
    CraftProgressBar,
    SetSlot,
    SetCooldown,
    CustomPayload,
    NamedSoundEffect,
    KickDisconnect,
    EntityStatus,
    Explosion,
    UnloadChunk,
    GameStateChange,
    KeepAlive,
    MapChunk,
    WorldEvent,
    WorldParticles,
    Login,
    Map,
    Entity,
    RelEntityMove,
    EntityMoveLook,
    EntityLook,
    VehicleMove,
    OpenSignEntity,
    CraftRecipeResponse,
    Abilities,
    CombatEvent,
    PlayerInfo,
    Position,
    Bed,
    UnlockRecipes,
    EntityDestroy,
    RemoveEntityEffect,
    ResourcePackSend,
    Respawn,
    EntityHeadRotation,
    SelectAdvancementTab,
    WorldBorder,
    Camera,
    HeldItemSlot,
    ScoreboardDisplayObjective,
    EntityMetadata,
    AttachEntity,
    EntityVelocity,
    EntityEquipment,
    Experience,
    UpdateHealth,
    ScoreboardObjective,
    SetPassengers,
    Teams,
    ScoreboardScore,
    SpawnPosition,
    UpdateTime,
    Title,
    SoundEffect,
    PlayerlistHeader,
    Collect,
    EntityTeleport,
    Advancements,
    EntityUpdateAttributes,
    EntityEffect,
    DeclareCommands,
    StopSound,
    DeclareRecipes,
    Tags,
    NbtQueryResponse,
    FacePlayer,
    OpenHorseWindow,
    UpdateLight,
    TradeList,
    OpenBook,
    UpdateViewPosition,
    UpdateViewDistance,
    EntitySoundEffect,
    AcknowledgePlayerDigging,
    SculkVibrationSignal,
    ClearTitles,
    InitializeWorldBorder,
    Ping,
    EndCombatEvent,
    EnterCombatEvent,
    DeathCombatEvent,
    DestroyEntity,
    ActionBar,
    WorldBorderCenter,
    WorldBorderLerpSize,
    WorldBorderSize,
    WorldBorderWarningDelay,
    WorldBorderWarningReach,
    SetTitleSubtitle,
    SetTitleText,
    SetTitleTime,
    SimulationDistance,
    ChatPreview,
    PlayerChat,
    ServerData,
    ShouldDisplayChatPreview,
    SystemChat,
    ChatSuggestions,
    HideMessage,
    MessageHeader,
    ProfilelessChat,
    PlayerRemove,
    FeatureFlags,
    BundleDelimiter,
    ChunkBiomes,
    DamageEvent,
    HurtAnimation,
    ChunkBatchFinished,
    ChunkBatchStart,
    PingResponse,
    StartConfiguration,
    ResetScore,
    RemoveResourcePack,
    AddResourcePack,
    SetTickingState,
    StepTick,
    CookieRequest,
    DebugSample,
    StoreCookie,
    Transfer,
    SyncEntityPosition,
    MoveMinecart,
    PlayerRotation,
    RecipeBookAdd,
    RecipeBookRemove,
    RecipeBookSettings,
    SetCursorItem,
    SetPlayerInventory,
    SetProjectilePower,
    CustomReportDetails,
    ServerLinks
}