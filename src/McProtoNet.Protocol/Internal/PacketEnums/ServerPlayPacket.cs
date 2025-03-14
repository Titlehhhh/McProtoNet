namespace McProtoNet.Protocol;

public static class ServerPlayPacket
{
    public static readonly PacketIdentifier Abilities = new(0, nameof(Abilities), PacketState.Play,
        PacketDirection.Clientbound);

    public static readonly PacketIdentifier AcknowledgePlayerDigging =
        new(1, nameof(AcknowledgePlayerDigging), PacketState.Play, PacketDirection.Clientbound);

    public static readonly PacketIdentifier ActionBar = new(2, nameof(ActionBar), PacketState.Play,
        PacketDirection.Clientbound);

    public static readonly PacketIdentifier AddResourcePack =
        new(3, nameof(AddResourcePack), PacketState.Play, PacketDirection.Clientbound);

    public static readonly PacketIdentifier Advancements =
        new(4, nameof(Advancements), PacketState.Play, PacketDirection.Clientbound);

    public static readonly PacketIdentifier Animation = new(5, nameof(Animation), PacketState.Play,
        PacketDirection.Clientbound);

    public static readonly PacketIdentifier AttachEntity =
        new(6, nameof(AttachEntity), PacketState.Play, PacketDirection.Clientbound);

    public static readonly PacketIdentifier Bed = new(7, nameof(Bed), PacketState.Play, PacketDirection.Clientbound);

    public static readonly PacketIdentifier BlockAction = new(8, nameof(BlockAction), PacketState.Play,
        PacketDirection.Clientbound);

    public static readonly PacketIdentifier BlockBreakAnimation =
        new(9, nameof(BlockBreakAnimation), PacketState.Play, PacketDirection.Clientbound);

    public static readonly PacketIdentifier BlockChange = new(10, nameof(BlockChange), PacketState.Play,
        PacketDirection.Clientbound);

    public static readonly PacketIdentifier BossBar = new(11, nameof(BossBar), PacketState.Play,
        PacketDirection.Clientbound);

    public static readonly PacketIdentifier BundleDelimiter =
        new(12, nameof(BundleDelimiter), PacketState.Play, PacketDirection.Clientbound);

    public static readonly PacketIdentifier Camera = new(13, nameof(Camera), PacketState.Play,
        PacketDirection.Clientbound);

    public static readonly PacketIdentifier Chat = new(14, nameof(Chat), PacketState.Play, PacketDirection.Clientbound);

    public static readonly PacketIdentifier ChatPreview = new(15, nameof(ChatPreview), PacketState.Play,
        PacketDirection.Clientbound);

    public static readonly PacketIdentifier ChatSuggestions =
        new(16, nameof(ChatSuggestions), PacketState.Play, PacketDirection.Clientbound);

    public static readonly PacketIdentifier ChunkBatchFinished =
        new(17, nameof(ChunkBatchFinished), PacketState.Play, PacketDirection.Clientbound);

    public static readonly PacketIdentifier ChunkBatchStart =
        new(18, nameof(ChunkBatchStart), PacketState.Play, PacketDirection.Clientbound);

    public static readonly PacketIdentifier ChunkBiomes = new(19, nameof(ChunkBiomes), PacketState.Play,
        PacketDirection.Clientbound);

    public static readonly PacketIdentifier ClearTitles = new(20, nameof(ClearTitles), PacketState.Play,
        PacketDirection.Clientbound);

    public static readonly PacketIdentifier CloseWindow = new(21, nameof(CloseWindow), PacketState.Play,
        PacketDirection.Clientbound);

    public static readonly PacketIdentifier Collect = new(22, nameof(Collect), PacketState.Play,
        PacketDirection.Clientbound);

    public static readonly PacketIdentifier CombatEvent = new(23, nameof(CombatEvent), PacketState.Play,
        PacketDirection.Clientbound);

    public static readonly PacketIdentifier CookieRequest =
        new(24, nameof(CookieRequest), PacketState.Play, PacketDirection.Clientbound);

    public static readonly PacketIdentifier CraftProgressBar =
        new(25, nameof(CraftProgressBar), PacketState.Play, PacketDirection.Clientbound);

    public static readonly PacketIdentifier CraftRecipeResponse =
        new(26, nameof(CraftRecipeResponse), PacketState.Play, PacketDirection.Clientbound);

    public static readonly PacketIdentifier CustomPayload =
        new(27, nameof(CustomPayload), PacketState.Play, PacketDirection.Clientbound);

    public static readonly PacketIdentifier CustomReportDetails =
        new(28, nameof(CustomReportDetails), PacketState.Play, PacketDirection.Clientbound);

    public static readonly PacketIdentifier DamageEvent = new(29, nameof(DamageEvent), PacketState.Play,
        PacketDirection.Clientbound);

    public static readonly PacketIdentifier DeathCombatEvent =
        new(30, nameof(DeathCombatEvent), PacketState.Play, PacketDirection.Clientbound);

    public static readonly PacketIdentifier DebugSample = new(31, nameof(DebugSample), PacketState.Play,
        PacketDirection.Clientbound);

    public static readonly PacketIdentifier DeclareCommands =
        new(32, nameof(DeclareCommands), PacketState.Play, PacketDirection.Clientbound);

    public static readonly PacketIdentifier DeclareRecipes =
        new(33, nameof(DeclareRecipes), PacketState.Play, PacketDirection.Clientbound);

    public static readonly PacketIdentifier DestroyEntity =
        new(34, nameof(DestroyEntity), PacketState.Play, PacketDirection.Clientbound);

    public static readonly PacketIdentifier Difficulty = new(35, nameof(Difficulty), PacketState.Play,
        PacketDirection.Clientbound);

    public static readonly PacketIdentifier EndCombatEvent =
        new(36, nameof(EndCombatEvent), PacketState.Play, PacketDirection.Clientbound);

    public static readonly PacketIdentifier EnterCombatEvent =
        new(37, nameof(EnterCombatEvent), PacketState.Play, PacketDirection.Clientbound);

    public static readonly PacketIdentifier Entity = new(38, nameof(Entity), PacketState.Play,
        PacketDirection.Clientbound);

    public static readonly PacketIdentifier EntityDestroy =
        new(39, nameof(EntityDestroy), PacketState.Play, PacketDirection.Clientbound);

    public static readonly PacketIdentifier EntityEffect =
        new(40, nameof(EntityEffect), PacketState.Play, PacketDirection.Clientbound);

    public static readonly PacketIdentifier EntityEquipment =
        new(41, nameof(EntityEquipment), PacketState.Play, PacketDirection.Clientbound);

    public static readonly PacketIdentifier EntityHeadRotation =
        new(42, nameof(EntityHeadRotation), PacketState.Play, PacketDirection.Clientbound);

    public static readonly PacketIdentifier EntityLook = new(43, nameof(EntityLook), PacketState.Play,
        PacketDirection.Clientbound);

    public static readonly PacketIdentifier EntityMetadata =
        new(44, nameof(EntityMetadata), PacketState.Play, PacketDirection.Clientbound);

    public static readonly PacketIdentifier EntityMoveLook =
        new(45, nameof(EntityMoveLook), PacketState.Play, PacketDirection.Clientbound);

    public static readonly PacketIdentifier EntitySoundEffect =
        new(46, nameof(EntitySoundEffect), PacketState.Play, PacketDirection.Clientbound);

    public static readonly PacketIdentifier EntityStatus =
        new(47, nameof(EntityStatus), PacketState.Play, PacketDirection.Clientbound);

    public static readonly PacketIdentifier EntityTeleport =
        new(48, nameof(EntityTeleport), PacketState.Play, PacketDirection.Clientbound);

    public static readonly PacketIdentifier EntityUpdateAttributes =
        new(49, nameof(EntityUpdateAttributes), PacketState.Play, PacketDirection.Clientbound);

    public static readonly PacketIdentifier EntityVelocity =
        new(50, nameof(EntityVelocity), PacketState.Play, PacketDirection.Clientbound);

    public static readonly PacketIdentifier Experience = new(51, nameof(Experience), PacketState.Play,
        PacketDirection.Clientbound);

    public static readonly PacketIdentifier Explosion = new(52, nameof(Explosion), PacketState.Play,
        PacketDirection.Clientbound);

    public static readonly PacketIdentifier FacePlayer = new(53, nameof(FacePlayer), PacketState.Play,
        PacketDirection.Clientbound);

    public static readonly PacketIdentifier FeatureFlags =
        new(54, nameof(FeatureFlags), PacketState.Play, PacketDirection.Clientbound);

    public static readonly PacketIdentifier GameStateChange =
        new(55, nameof(GameStateChange), PacketState.Play, PacketDirection.Clientbound);

    public static readonly PacketIdentifier HeldItemSlot =
        new(56, nameof(HeldItemSlot), PacketState.Play, PacketDirection.Clientbound);

    public static readonly PacketIdentifier HideMessage = new(57, nameof(HideMessage), PacketState.Play,
        PacketDirection.Clientbound);

    public static readonly PacketIdentifier HurtAnimation =
        new(58, nameof(HurtAnimation), PacketState.Play, PacketDirection.Clientbound);

    public static readonly PacketIdentifier InitializeWorldBorder =
        new(59, nameof(InitializeWorldBorder), PacketState.Play, PacketDirection.Clientbound);

    public static readonly PacketIdentifier KeepAlive = new(60, nameof(KeepAlive), PacketState.Play,
        PacketDirection.Clientbound);

    public static readonly PacketIdentifier KickDisconnect =
        new(61, nameof(KickDisconnect), PacketState.Play, PacketDirection.Clientbound);

    public static readonly PacketIdentifier Login = new(62, nameof(Login), PacketState.Play,
        PacketDirection.Clientbound);

    public static readonly PacketIdentifier Map = new(63, nameof(Map), PacketState.Play, PacketDirection.Clientbound);

    public static readonly PacketIdentifier MapChunk = new(64, nameof(MapChunk), PacketState.Play,
        PacketDirection.Clientbound);

    public static readonly PacketIdentifier MessageHeader =
        new(65, nameof(MessageHeader), PacketState.Play, PacketDirection.Clientbound);

    public static readonly PacketIdentifier MoveMinecart =
        new(66, nameof(MoveMinecart), PacketState.Play, PacketDirection.Clientbound);

    public static readonly PacketIdentifier MultiBlockChange =
        new(67, nameof(MultiBlockChange), PacketState.Play, PacketDirection.Clientbound);

    public static readonly PacketIdentifier NamedEntitySpawn =
        new(68, nameof(NamedEntitySpawn), PacketState.Play, PacketDirection.Clientbound);

    public static readonly PacketIdentifier NamedSoundEffect =
        new(69, nameof(NamedSoundEffect), PacketState.Play, PacketDirection.Clientbound);

    public static readonly PacketIdentifier NbtQueryResponse =
        new(70, nameof(NbtQueryResponse), PacketState.Play, PacketDirection.Clientbound);

    public static readonly PacketIdentifier OpenBook = new(71, nameof(OpenBook), PacketState.Play,
        PacketDirection.Clientbound);

    public static readonly PacketIdentifier OpenHorseWindow =
        new(72, nameof(OpenHorseWindow), PacketState.Play, PacketDirection.Clientbound);

    public static readonly PacketIdentifier OpenSignEntity =
        new(73, nameof(OpenSignEntity), PacketState.Play, PacketDirection.Clientbound);

    public static readonly PacketIdentifier OpenWindow = new(74, nameof(OpenWindow), PacketState.Play,
        PacketDirection.Clientbound);

    public static readonly PacketIdentifier Ping = new(75, nameof(Ping), PacketState.Play, PacketDirection.Clientbound);

    public static readonly PacketIdentifier PingResponse =
        new(76, nameof(PingResponse), PacketState.Play, PacketDirection.Clientbound);

    public static readonly PacketIdentifier PlayerChat = new(77, nameof(PlayerChat), PacketState.Play,
        PacketDirection.Clientbound);

    public static readonly PacketIdentifier PlayerInfo = new(78, nameof(PlayerInfo), PacketState.Play,
        PacketDirection.Clientbound);

    public static readonly PacketIdentifier PlayerRemove =
        new(79, nameof(PlayerRemove), PacketState.Play, PacketDirection.Clientbound);

    public static readonly PacketIdentifier PlayerRotation =
        new(80, nameof(PlayerRotation), PacketState.Play, PacketDirection.Clientbound);

    public static readonly PacketIdentifier PlayerlistHeader =
        new(81, nameof(PlayerlistHeader), PacketState.Play, PacketDirection.Clientbound);

    public static readonly PacketIdentifier Position = new(82, nameof(Position), PacketState.Play,
        PacketDirection.Clientbound);

    public static readonly PacketIdentifier ProfilelessChat =
        new(83, nameof(ProfilelessChat), PacketState.Play, PacketDirection.Clientbound);

    public static readonly PacketIdentifier RecipeBookAdd =
        new(84, nameof(RecipeBookAdd), PacketState.Play, PacketDirection.Clientbound);

    public static readonly PacketIdentifier RecipeBookRemove =
        new(85, nameof(RecipeBookRemove), PacketState.Play, PacketDirection.Clientbound);

    public static readonly PacketIdentifier RecipeBookSettings =
        new(86, nameof(RecipeBookSettings), PacketState.Play, PacketDirection.Clientbound);

    public static readonly PacketIdentifier RelEntityMove =
        new(87, nameof(RelEntityMove), PacketState.Play, PacketDirection.Clientbound);

    public static readonly PacketIdentifier RemoveEntityEffect =
        new(88, nameof(RemoveEntityEffect), PacketState.Play, PacketDirection.Clientbound);

    public static readonly PacketIdentifier RemoveResourcePack =
        new(89, nameof(RemoveResourcePack), PacketState.Play, PacketDirection.Clientbound);

    public static readonly PacketIdentifier ResetScore = new(90, nameof(ResetScore), PacketState.Play,
        PacketDirection.Clientbound);

    public static readonly PacketIdentifier ResourcePackSend =
        new(91, nameof(ResourcePackSend), PacketState.Play, PacketDirection.Clientbound);

    public static readonly PacketIdentifier Respawn = new(92, nameof(Respawn), PacketState.Play,
        PacketDirection.Clientbound);

    public static readonly PacketIdentifier ScoreboardDisplayObjective = new(93, nameof(ScoreboardDisplayObjective),
        PacketState.Play, PacketDirection.Clientbound);

    public static readonly PacketIdentifier ScoreboardObjective =
        new(94, nameof(ScoreboardObjective), PacketState.Play, PacketDirection.Clientbound);

    public static readonly PacketIdentifier ScoreboardScore =
        new(95, nameof(ScoreboardScore), PacketState.Play, PacketDirection.Clientbound);

    public static readonly PacketIdentifier SculkVibrationSignal =
        new(96, nameof(SculkVibrationSignal), PacketState.Play, PacketDirection.Clientbound);

    public static readonly PacketIdentifier SelectAdvancementTab =
        new(97, nameof(SelectAdvancementTab), PacketState.Play, PacketDirection.Clientbound);

    public static readonly PacketIdentifier ServerData = new(98, nameof(ServerData), PacketState.Play,
        PacketDirection.Clientbound);

    public static readonly PacketIdentifier ServerLinks = new(99, nameof(ServerLinks), PacketState.Play,
        PacketDirection.Clientbound);

    public static readonly PacketIdentifier SetCooldown = new(100, nameof(SetCooldown), PacketState.Play,
        PacketDirection.Clientbound);

    public static readonly PacketIdentifier SetCursorItem =
        new(101, nameof(SetCursorItem), PacketState.Play, PacketDirection.Clientbound);

    public static readonly PacketIdentifier SetPassengers =
        new(102, nameof(SetPassengers), PacketState.Play, PacketDirection.Clientbound);

    public static readonly PacketIdentifier SetPlayerInventory =
        new(103, nameof(SetPlayerInventory), PacketState.Play, PacketDirection.Clientbound);

    public static readonly PacketIdentifier SetProjectilePower =
        new(104, nameof(SetProjectilePower), PacketState.Play, PacketDirection.Clientbound);

    public static readonly PacketIdentifier SetSlot = new(105, nameof(SetSlot), PacketState.Play,
        PacketDirection.Clientbound);

    public static readonly PacketIdentifier SetTickingState =
        new(106, nameof(SetTickingState), PacketState.Play, PacketDirection.Clientbound);

    public static readonly PacketIdentifier SetTitleSubtitle =
        new(107, nameof(SetTitleSubtitle), PacketState.Play, PacketDirection.Clientbound);

    public static readonly PacketIdentifier SetTitleText =
        new(108, nameof(SetTitleText), PacketState.Play, PacketDirection.Clientbound);

    public static readonly PacketIdentifier SetTitleTime =
        new(109, nameof(SetTitleTime), PacketState.Play, PacketDirection.Clientbound);

    public static readonly PacketIdentifier ShouldDisplayChatPreview = new(110, nameof(ShouldDisplayChatPreview),
        PacketState.Play, PacketDirection.Clientbound);

    public static readonly PacketIdentifier SimulationDistance =
        new(111, nameof(SimulationDistance), PacketState.Play, PacketDirection.Clientbound);

    public static readonly PacketIdentifier SoundEffect = new(112, nameof(SoundEffect), PacketState.Play,
        PacketDirection.Clientbound);

    public static readonly PacketIdentifier SpawnEntity = new(113, nameof(SpawnEntity), PacketState.Play,
        PacketDirection.Clientbound);

    public static readonly PacketIdentifier SpawnEntityExperienceOrb = new(114, nameof(SpawnEntityExperienceOrb),
        PacketState.Play, PacketDirection.Clientbound);

    public static readonly PacketIdentifier SpawnEntityLiving =
        new(115, nameof(SpawnEntityLiving), PacketState.Play, PacketDirection.Clientbound);

    public static readonly PacketIdentifier SpawnEntityPainting =
        new(116, nameof(SpawnEntityPainting), PacketState.Play, PacketDirection.Clientbound);

    public static readonly PacketIdentifier SpawnEntityWeather =
        new(117, nameof(SpawnEntityWeather), PacketState.Play, PacketDirection.Clientbound);

    public static readonly PacketIdentifier SpawnPosition =
        new(118, nameof(SpawnPosition), PacketState.Play, PacketDirection.Clientbound);

    public static readonly PacketIdentifier StartConfiguration =
        new(119, nameof(StartConfiguration), PacketState.Play, PacketDirection.Clientbound);

    public static readonly PacketIdentifier Statistics = new(120, nameof(Statistics), PacketState.Play,
        PacketDirection.Clientbound);

    public static readonly PacketIdentifier StepTick = new(121, nameof(StepTick), PacketState.Play,
        PacketDirection.Clientbound);

    public static readonly PacketIdentifier StopSound = new(122, nameof(StopSound), PacketState.Play,
        PacketDirection.Clientbound);

    public static readonly PacketIdentifier StoreCookie = new(123, nameof(StoreCookie), PacketState.Play,
        PacketDirection.Clientbound);

    public static readonly PacketIdentifier SyncEntityPosition =
        new(124, nameof(SyncEntityPosition), PacketState.Play, PacketDirection.Clientbound);

    public static readonly PacketIdentifier SystemChat = new(125, nameof(SystemChat), PacketState.Play,
        PacketDirection.Clientbound);

    public static readonly PacketIdentifier TabComplete = new(126, nameof(TabComplete), PacketState.Play,
        PacketDirection.Clientbound);

    public static readonly PacketIdentifier
        Tags = new(127, nameof(Tags), PacketState.Play, PacketDirection.Clientbound);

    public static readonly PacketIdentifier Teams = new(128, nameof(Teams), PacketState.Play,
        PacketDirection.Clientbound);

    public static readonly PacketIdentifier TileEntityData =
        new(129, nameof(TileEntityData), PacketState.Play, PacketDirection.Clientbound);

    public static readonly PacketIdentifier Title = new(130, nameof(Title), PacketState.Play,
        PacketDirection.Clientbound);

    public static readonly PacketIdentifier TradeList = new(131, nameof(TradeList), PacketState.Play,
        PacketDirection.Clientbound);

    public static readonly PacketIdentifier Transaction = new(132, nameof(Transaction), PacketState.Play,
        PacketDirection.Clientbound);

    public static readonly PacketIdentifier Transfer = new(133, nameof(Transfer), PacketState.Play,
        PacketDirection.Clientbound);

    public static readonly PacketIdentifier UnloadChunk = new(134, nameof(UnloadChunk), PacketState.Play,
        PacketDirection.Clientbound);

    public static readonly PacketIdentifier UnlockRecipes =
        new(135, nameof(UnlockRecipes), PacketState.Play, PacketDirection.Clientbound);

    public static readonly PacketIdentifier UpdateHealth =
        new(136, nameof(UpdateHealth), PacketState.Play, PacketDirection.Clientbound);

    public static readonly PacketIdentifier UpdateLight = new(137, nameof(UpdateLight), PacketState.Play,
        PacketDirection.Clientbound);

    public static readonly PacketIdentifier UpdateTime = new(138, nameof(UpdateTime), PacketState.Play,
        PacketDirection.Clientbound);

    public static readonly PacketIdentifier UpdateViewDistance =
        new(139, nameof(UpdateViewDistance), PacketState.Play, PacketDirection.Clientbound);

    public static readonly PacketIdentifier UpdateViewPosition =
        new(140, nameof(UpdateViewPosition), PacketState.Play, PacketDirection.Clientbound);

    public static readonly PacketIdentifier VehicleMove = new(141, nameof(VehicleMove), PacketState.Play,
        PacketDirection.Clientbound);

    public static readonly PacketIdentifier WindowItems = new(142, nameof(WindowItems), PacketState.Play,
        PacketDirection.Clientbound);

    public static readonly PacketIdentifier WorldBorder = new(143, nameof(WorldBorder), PacketState.Play,
        PacketDirection.Clientbound);

    public static readonly PacketIdentifier WorldBorderCenter =
        new(144, nameof(WorldBorderCenter), PacketState.Play, PacketDirection.Clientbound);

    public static readonly PacketIdentifier WorldBorderLerpSize =
        new(145, nameof(WorldBorderLerpSize), PacketState.Play, PacketDirection.Clientbound);

    public static readonly PacketIdentifier WorldBorderSize =
        new(146, nameof(WorldBorderSize), PacketState.Play, PacketDirection.Clientbound);

    public static readonly PacketIdentifier WorldBorderWarningDelay =
        new(147, nameof(WorldBorderWarningDelay), PacketState.Play, PacketDirection.Clientbound);

    public static readonly PacketIdentifier WorldBorderWarningReach =
        new(148, nameof(WorldBorderWarningReach), PacketState.Play, PacketDirection.Clientbound);

    public static readonly PacketIdentifier WorldEvent = new(149, nameof(WorldEvent), PacketState.Play,
        PacketDirection.Clientbound);

    public static readonly PacketIdentifier WorldParticles =
        new(150, nameof(WorldParticles), PacketState.Play, PacketDirection.Clientbound);
}