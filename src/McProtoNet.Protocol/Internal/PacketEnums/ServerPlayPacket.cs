namespace McProtoNet.Protocol;

public static class ServerPlayPacket
{
    public static PacketIdentifier Abilities =>
        new(0, nameof(Abilities), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier AcknowledgePlayerDigging => new(1, nameof(AcknowledgePlayerDigging),
        PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier ActionBar =>
        new(2, nameof(ActionBar), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier AddResourcePack =>
        new(3, nameof(AddResourcePack), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier Advancements =>
        new(4, nameof(Advancements), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier Animation =>
        new(5, nameof(Animation), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier AttachEntity =>
        new(6, nameof(AttachEntity), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier Bed => new(7, nameof(Bed), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier BlockAction =>
        new(8, nameof(BlockAction), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier BlockBreakAnimation =>
        new(9, nameof(BlockBreakAnimation), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier BlockChange =>
        new(10, nameof(BlockChange), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier BossBar => new(11, nameof(BossBar), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier BundleDelimiter =>
        new(12, nameof(BundleDelimiter), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier Camera => new(13, nameof(Camera), PacketState.Play, PacketDirection.Clientbound);
    public static PacketIdentifier Chat => new(14, nameof(Chat), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier ChatPreview =>
        new(15, nameof(ChatPreview), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier ChatSuggestions =>
        new(16, nameof(ChatSuggestions), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier ChunkBatchFinished =>
        new(17, nameof(ChunkBatchFinished), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier ChunkBatchStart =>
        new(18, nameof(ChunkBatchStart), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier ChunkBiomes =>
        new(19, nameof(ChunkBiomes), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier ClearTitles =>
        new(20, nameof(ClearTitles), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier CloseWindow =>
        new(21, nameof(CloseWindow), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier Collect => new(22, nameof(Collect), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier CombatEvent =>
        new(23, nameof(CombatEvent), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier CookieRequest =>
        new(24, nameof(CookieRequest), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier CraftProgressBar =>
        new(25, nameof(CraftProgressBar), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier CraftRecipeResponse =>
        new(26, nameof(CraftRecipeResponse), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier CustomPayload =>
        new(27, nameof(CustomPayload), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier CustomReportDetails =>
        new(28, nameof(CustomReportDetails), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier DamageEvent =>
        new(29, nameof(DamageEvent), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier DeathCombatEvent =>
        new(30, nameof(DeathCombatEvent), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier DebugSample =>
        new(31, nameof(DebugSample), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier DeclareCommands =>
        new(32, nameof(DeclareCommands), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier DeclareRecipes =>
        new(33, nameof(DeclareRecipes), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier DestroyEntity =>
        new(34, nameof(DestroyEntity), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier Difficulty =>
        new(35, nameof(Difficulty), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier EndCombatEvent =>
        new(36, nameof(EndCombatEvent), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier EnterCombatEvent =>
        new(37, nameof(EnterCombatEvent), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier Entity => new(38, nameof(Entity), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier EntityDestroy =>
        new(39, nameof(EntityDestroy), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier EntityEffect =>
        new(40, nameof(EntityEffect), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier EntityEquipment =>
        new(41, nameof(EntityEquipment), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier EntityHeadRotation =>
        new(42, nameof(EntityHeadRotation), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier EntityLook =>
        new(43, nameof(EntityLook), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier EntityMetadata =>
        new(44, nameof(EntityMetadata), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier EntityMoveLook =>
        new(45, nameof(EntityMoveLook), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier EntitySoundEffect =>
        new(46, nameof(EntitySoundEffect), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier EntityStatus =>
        new(47, nameof(EntityStatus), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier EntityTeleport =>
        new(48, nameof(EntityTeleport), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier EntityUpdateAttributes => new(49, nameof(EntityUpdateAttributes), PacketState.Play,
        PacketDirection.Clientbound);

    public static PacketIdentifier EntityVelocity =>
        new(50, nameof(EntityVelocity), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier Experience =>
        new(51, nameof(Experience), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier Explosion =>
        new(52, nameof(Explosion), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier FacePlayer =>
        new(53, nameof(FacePlayer), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier FeatureFlags =>
        new(54, nameof(FeatureFlags), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier GameStateChange =>
        new(55, nameof(GameStateChange), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier HeldItemSlot =>
        new(56, nameof(HeldItemSlot), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier HideMessage =>
        new(57, nameof(HideMessage), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier HurtAnimation =>
        new(58, nameof(HurtAnimation), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier InitializeWorldBorder => new(59, nameof(InitializeWorldBorder), PacketState.Play,
        PacketDirection.Clientbound);

    public static PacketIdentifier KeepAlive =>
        new(60, nameof(KeepAlive), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier KickDisconnect =>
        new(61, nameof(KickDisconnect), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier Login => new(62, nameof(Login), PacketState.Play, PacketDirection.Clientbound);
    public static PacketIdentifier Map => new(63, nameof(Map), PacketState.Play, PacketDirection.Clientbound);
    public static PacketIdentifier MapChunk => new(64, nameof(MapChunk), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier MessageHeader =>
        new(65, nameof(MessageHeader), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier MoveMinecart =>
        new(66, nameof(MoveMinecart), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier MultiBlockChange =>
        new(67, nameof(MultiBlockChange), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier NamedEntitySpawn =>
        new(68, nameof(NamedEntitySpawn), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier NamedSoundEffect =>
        new(69, nameof(NamedSoundEffect), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier NbtQueryResponse =>
        new(70, nameof(NbtQueryResponse), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier OpenBook => new(71, nameof(OpenBook), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier OpenHorseWindow =>
        new(72, nameof(OpenHorseWindow), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier OpenSignEntity =>
        new(73, nameof(OpenSignEntity), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier OpenWindow =>
        new(74, nameof(OpenWindow), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier Ping => new(75, nameof(Ping), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier PingResponse =>
        new(76, nameof(PingResponse), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier PlayerChat =>
        new(77, nameof(PlayerChat), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier PlayerInfo =>
        new(78, nameof(PlayerInfo), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier PlayerRemove =>
        new(79, nameof(PlayerRemove), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier PlayerRotation =>
        new(80, nameof(PlayerRotation), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier PlayerlistHeader =>
        new(81, nameof(PlayerlistHeader), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier Position => new(82, nameof(Position), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier ProfilelessChat =>
        new(83, nameof(ProfilelessChat), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier RecipeBookAdd =>
        new(84, nameof(RecipeBookAdd), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier RecipeBookRemove =>
        new(85, nameof(RecipeBookRemove), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier RecipeBookSettings =>
        new(86, nameof(RecipeBookSettings), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier RelEntityMove =>
        new(87, nameof(RelEntityMove), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier RemoveEntityEffect =>
        new(88, nameof(RemoveEntityEffect), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier RemoveResourcePack =>
        new(89, nameof(RemoveResourcePack), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier ResetScore =>
        new(90, nameof(ResetScore), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier ResourcePackSend =>
        new(91, nameof(ResourcePackSend), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier Respawn => new(92, nameof(Respawn), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier ScoreboardDisplayObjective => new(93, nameof(ScoreboardDisplayObjective),
        PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier ScoreboardObjective =>
        new(94, nameof(ScoreboardObjective), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier ScoreboardScore =>
        new(95, nameof(ScoreboardScore), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier SculkVibrationSignal => new(96, nameof(SculkVibrationSignal), PacketState.Play,
        PacketDirection.Clientbound);

    public static PacketIdentifier SelectAdvancementTab => new(97, nameof(SelectAdvancementTab), PacketState.Play,
        PacketDirection.Clientbound);

    public static PacketIdentifier ServerData =>
        new(98, nameof(ServerData), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier ServerLinks =>
        new(99, nameof(ServerLinks), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier SetCooldown =>
        new(100, nameof(SetCooldown), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier SetCursorItem =>
        new(101, nameof(SetCursorItem), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier SetPassengers =>
        new(102, nameof(SetPassengers), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier SetPlayerInventory =>
        new(103, nameof(SetPlayerInventory), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier SetProjectilePower =>
        new(104, nameof(SetProjectilePower), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier SetSlot => new(105, nameof(SetSlot), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier SetTickingState =>
        new(106, nameof(SetTickingState), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier SetTitleSubtitle =>
        new(107, nameof(SetTitleSubtitle), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier SetTitleText =>
        new(108, nameof(SetTitleText), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier SetTitleTime =>
        new(109, nameof(SetTitleTime), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier ShouldDisplayChatPreview => new(110, nameof(ShouldDisplayChatPreview),
        PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier SimulationDistance =>
        new(111, nameof(SimulationDistance), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier SoundEffect =>
        new(112, nameof(SoundEffect), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier SpawnEntity =>
        new(113, nameof(SpawnEntity), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier SpawnEntityExperienceOrb => new(114, nameof(SpawnEntityExperienceOrb),
        PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier SpawnEntityLiving =>
        new(115, nameof(SpawnEntityLiving), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier SpawnEntityPainting =>
        new(116, nameof(SpawnEntityPainting), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier SpawnEntityWeather =>
        new(117, nameof(SpawnEntityWeather), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier SpawnPosition =>
        new(118, nameof(SpawnPosition), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier StartConfiguration =>
        new(119, nameof(StartConfiguration), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier Statistics =>
        new(120, nameof(Statistics), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier StepTick =>
        new(121, nameof(StepTick), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier StopSound =>
        new(122, nameof(StopSound), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier StoreCookie =>
        new(123, nameof(StoreCookie), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier SyncEntityPosition =>
        new(124, nameof(SyncEntityPosition), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier SystemChat =>
        new(125, nameof(SystemChat), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier TabComplete =>
        new(126, nameof(TabComplete), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier Tags => new(127, nameof(Tags), PacketState.Play, PacketDirection.Clientbound);
    public static PacketIdentifier Teams => new(128, nameof(Teams), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier TileEntityData =>
        new(129, nameof(TileEntityData), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier Title => new(130, nameof(Title), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier TradeList =>
        new(131, nameof(TradeList), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier Transaction =>
        new(132, nameof(Transaction), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier Transfer =>
        new(133, nameof(Transfer), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier UnloadChunk =>
        new(134, nameof(UnloadChunk), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier UnlockRecipes =>
        new(135, nameof(UnlockRecipes), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier UpdateHealth =>
        new(136, nameof(UpdateHealth), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier UpdateLight =>
        new(137, nameof(UpdateLight), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier UpdateTime =>
        new(138, nameof(UpdateTime), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier UpdateViewDistance =>
        new(139, nameof(UpdateViewDistance), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier UpdateViewPosition =>
        new(140, nameof(UpdateViewPosition), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier VehicleMove =>
        new(141, nameof(VehicleMove), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier WindowItems =>
        new(142, nameof(WindowItems), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier WorldBorder =>
        new(143, nameof(WorldBorder), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier WorldBorderCenter =>
        new(144, nameof(WorldBorderCenter), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier WorldBorderLerpSize =>
        new(145, nameof(WorldBorderLerpSize), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier WorldBorderSize =>
        new(146, nameof(WorldBorderSize), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier WorldBorderWarningDelay => new(147, nameof(WorldBorderWarningDelay),
        PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier WorldBorderWarningReach => new(148, nameof(WorldBorderWarningReach),
        PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier WorldEvent =>
        new(149, nameof(WorldEvent), PacketState.Play, PacketDirection.Clientbound);

    public static PacketIdentifier WorldParticles =>
        new(150, nameof(WorldParticles), PacketState.Play, PacketDirection.Clientbound);
}