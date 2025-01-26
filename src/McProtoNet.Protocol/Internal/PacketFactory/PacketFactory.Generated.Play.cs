using McProtoNet.Protocol.ClientboundPackets.Play;
using System.Collections.Frozen;

namespace McProtoNet.Protocol
{
    public static partial class PacketFactory
    {
        private static readonly Func<IServerPacket> SpawnEntityPacket_V340_404Factory = () => new SpawnEntityPacket.V340_404();
        private static readonly Func<IServerPacket> SpawnEntityExperienceOrbPacket_V340_769Factory = () => new SpawnEntityExperienceOrbPacket.V340_769();
        private static readonly Func<IServerPacket> SpawnEntityWeatherPacket_V340_710Factory = () => new SpawnEntityWeatherPacket.V340_710();
        private static readonly Func<IServerPacket> SpawnEntityPaintingPacket_V340_351Factory = () => new SpawnEntityPaintingPacket.V340_351();
        private static readonly Func<IServerPacket> AnimationPacket_V340_769Factory = () => new AnimationPacket.V340_769();
        private static readonly Func<IServerPacket> BlockBreakAnimationPacket_V340_769Factory = () => new BlockBreakAnimationPacket.V340_769();
        private static readonly Func<IServerPacket> TileEntityDataPacket_V340_756Factory = () => new TileEntityDataPacket.V340_756();
        private static readonly Func<IServerPacket> BlockActionPacket_V340_769Factory = () => new BlockActionPacket.V340_769();
        private static readonly Func<IServerPacket> BlockChangePacket_V340_769Factory = () => new BlockChangePacket.V340_769();
        private static readonly Func<IServerPacket> DifficultyPacket_V340_404Factory = () => new DifficultyPacket.V340_404();
        private static readonly Func<IServerPacket> ChatPacket_V340_710Factory = () => new ChatPacket.V340_710();
        private static readonly Func<IServerPacket> TransactionPacket_V340_754Factory = () => new TransactionPacket.V340_754();
        private static readonly Func<IServerPacket> CloseWindowPacket_V340_767Factory = () => new CloseWindowPacket.V340_767();
        private static readonly Func<IServerPacket> WindowItemsPacket_V340_755Factory = () => new WindowItemsPacket.V340_755();
        private static readonly Func<IServerPacket> CraftProgressBarPacket_V340_767Factory = () => new CraftProgressBarPacket.V340_767();
        private static readonly Func<IServerPacket> SetSlotPacket_V340_755Factory = () => new SetSlotPacket.V340_755();
        private static readonly Func<IServerPacket> SetCooldownPacket_V340_767Factory = () => new SetCooldownPacket.V340_767();
        private static readonly Func<IServerPacket> CustomPayloadPacket_V340_769Factory = () => new CustomPayloadPacket.V340_769();
        private static readonly Func<IServerPacket> NamedSoundEffectPacket_V340_758Factory = () => new NamedSoundEffectPacket.V340_758();
        private static readonly Func<IServerPacket> KickDisconnectPacket_V340_764Factory = () => new KickDisconnectPacket.V340_764();
        private static readonly Func<IServerPacket> EntityStatusPacket_V340_769Factory = () => new EntityStatusPacket.V340_769();
        private static readonly Func<IServerPacket> UnloadChunkPacket_V340_763Factory = () => new UnloadChunkPacket.V340_763();
        private static readonly Func<IServerPacket> GameStateChangePacket_V340_769Factory = () => new GameStateChangePacket.V340_769();
        private static readonly Func<IServerPacket> KeepAlivePacket_V340_769Factory = () => new KeepAlivePacket.V340_769();
        private static readonly Func<IServerPacket> WorldEventPacket_V340_769Factory = () => new WorldEventPacket.V340_769();
        private static readonly Func<IServerPacket> RelEntityMovePacket_V340_769Factory = () => new RelEntityMovePacket.V340_769();
        private static readonly Func<IServerPacket> EntityMoveLookPacket_V340_769Factory = () => new EntityMoveLookPacket.V340_769();
        private static readonly Func<IServerPacket> EntityLookPacket_V340_769Factory = () => new EntityLookPacket.V340_769();
        private static readonly Func<IServerPacket> EntityPacket_V340_754Factory = () => new EntityPacket.V340_754();
        private static readonly Func<IServerPacket> VehicleMovePacket_V340_769Factory = () => new VehicleMovePacket.V340_769();
        private static readonly Func<IServerPacket> OpenSignEntityPacket_V340_762Factory = () => new OpenSignEntityPacket.V340_762();
        private static readonly Func<IServerPacket> AbilitiesPacket_V340_769Factory = () => new AbilitiesPacket.V340_769();
        private static readonly Func<IServerPacket> PositionPacket_V340_754Factory = () => new PositionPacket.V340_754();
        private static readonly Func<IServerPacket> BedPacket_V340_404Factory = () => new BedPacket.V340_404();
        private static readonly Func<IServerPacket> EntityDestroyPacket_V340_754Factory = () => new EntityDestroyPacket.V340_754();
        private static readonly Func<IServerPacket> RemoveEntityEffectPacket_V340_757Factory = () => new RemoveEntityEffectPacket.V340_757();
        private static readonly Func<IServerPacket> ResourcePackSendPacket_V340_754Factory = () => new ResourcePackSendPacket.V340_754();
        private static readonly Func<IServerPacket> EntityHeadRotationPacket_V340_769Factory = () => new EntityHeadRotationPacket.V340_769();
        private static readonly Func<IServerPacket> CameraPacket_V340_769Factory = () => new CameraPacket.V340_769();
        private static readonly Func<IServerPacket> HeldItemSlotPacket_V340_768Factory = () => new HeldItemSlotPacket.V340_768();
        private static readonly Func<IServerPacket> ScoreboardDisplayObjectivePacket_V340_763Factory = () => new ScoreboardDisplayObjectivePacket.V340_763();
        private static readonly Func<IServerPacket> AttachEntityPacket_V340_769Factory = () => new AttachEntityPacket.V340_769();
        private static readonly Func<IServerPacket> EntityVelocityPacket_V340_769Factory = () => new EntityVelocityPacket.V340_769();
        private static readonly Func<IServerPacket> ExperiencePacket_V340_760Factory = () => new ExperiencePacket.V340_760();
        private static readonly Func<IServerPacket> UpdateHealthPacket_V340_769Factory = () => new UpdateHealthPacket.V340_769();
        private static readonly Func<IServerPacket> SetPassengersPacket_V340_769Factory = () => new SetPassengersPacket.V340_769();
        private static readonly Func<IServerPacket> SpawnPositionPacket_V340_754Factory = () => new SpawnPositionPacket.V340_754();
        private static readonly Func<IServerPacket> UpdateTimePacket_V340_767Factory = () => new UpdateTimePacket.V340_767();
        private static readonly Func<IServerPacket> PlayerlistHeaderPacket_V340_764Factory = () => new PlayerlistHeaderPacket.V340_764();
        private static readonly Func<IServerPacket> CollectPacket_V340_769Factory = () => new CollectPacket.V340_769();
        private static readonly Func<IServerPacket> EntityTeleportPacket_V340_769Factory = () => new EntityTeleportPacket.V340_769();
        private static readonly Func<IServerPacket> EntityEffectPacket_V340_757Factory = () => new EntityEffectPacket.V340_757();
        private static readonly Func<IServerPacket> SelectAdvancementTabPacket_V340_769Factory = () => new SelectAdvancementTabPacket.V340_769();
        private static readonly Func<IServerPacket> SpawnEntityPaintingPacket_V393_758Factory = () => new SpawnEntityPaintingPacket.V393_758();
        private static readonly Func<IServerPacket> NbtQueryResponsePacket_V393_763Factory = () => new NbtQueryResponsePacket.V393_763();
        private static readonly Func<IServerPacket> SpawnEntityPacket_V477_758Factory = () => new SpawnEntityPacket.V477_758();
        private static readonly Func<IServerPacket> DifficultyPacket_V477_769Factory = () => new DifficultyPacket.V477_769();
        private static readonly Func<IServerPacket> OpenHorseWindowPacket_V477_767Factory = () => new OpenHorseWindowPacket.V477_767();
        private static readonly Func<IServerPacket> UpdateLightPacket_V477_710Factory = () => new UpdateLightPacket.V477_710();
        private static readonly Func<IServerPacket> OpenBookPacket_V477_769Factory = () => new OpenBookPacket.V477_769();
        private static readonly Func<IServerPacket> UpdateViewPositionPacket_V477_769Factory = () => new UpdateViewPositionPacket.V477_769();
        private static readonly Func<IServerPacket> UpdateViewDistancePacket_V477_769Factory = () => new UpdateViewDistancePacket.V477_769();
        private static readonly Func<IServerPacket> AcknowledgePlayerDiggingPacket_V498_758Factory = () => new AcknowledgePlayerDiggingPacket.V498_758();
        private static readonly Func<IServerPacket> ChatPacket_V734_758Factory = () => new ChatPacket.V734_758();
        private static readonly Func<IServerPacket> UpdateLightPacket_V734_754Factory = () => new UpdateLightPacket.V734_754();
        private static readonly Func<IServerPacket> PositionPacket_V755_761Factory = () => new PositionPacket.V755_761();
        private static readonly Func<IServerPacket> ResourcePackSendPacket_V755_764Factory = () => new ResourcePackSendPacket.V755_764();
        private static readonly Func<IServerPacket> SpawnPositionPacket_V755_769Factory = () => new SpawnPositionPacket.V755_769();
        private static readonly Func<IServerPacket> UpdateLightPacket_V755_762Factory = () => new UpdateLightPacket.V755_762();
        private static readonly Func<IServerPacket> EndCombatEventPacket_V755_762Factory = () => new EndCombatEventPacket.V755_762();
        private static readonly Func<IServerPacket> EnterCombatEventPacket_V755_769Factory = () => new EnterCombatEventPacket.V755_769();
        private static readonly Func<IServerPacket> DeathCombatEventPacket_V755_762Factory = () => new DeathCombatEventPacket.V755_762();
        private static readonly Func<IServerPacket> DestroyEntityPacket_V755Factory = () => new DestroyEntityPacket.V755();
        private static readonly Func<IServerPacket> ClearTitlesPacket_V755_769Factory = () => new ClearTitlesPacket.V755_769();
        private static readonly Func<IServerPacket> InitializeWorldBorderPacket_V755_758Factory = () => new InitializeWorldBorderPacket.V755_758();
        private static readonly Func<IServerPacket> ActionBarPacket_V755_764Factory = () => new ActionBarPacket.V755_764();
        private static readonly Func<IServerPacket> WorldBorderCenterPacket_V755_769Factory = () => new WorldBorderCenterPacket.V755_769();
        private static readonly Func<IServerPacket> WorldBorderLerpSizePacket_V755_758Factory = () => new WorldBorderLerpSizePacket.V755_758();
        private static readonly Func<IServerPacket> WorldBorderSizePacket_V755_769Factory = () => new WorldBorderSizePacket.V755_769();
        private static readonly Func<IServerPacket> WorldBorderWarningDelayPacket_V755_769Factory = () => new WorldBorderWarningDelayPacket.V755_769();
        private static readonly Func<IServerPacket> WorldBorderWarningReachPacket_V755_769Factory = () => new WorldBorderWarningReachPacket.V755_769();
        private static readonly Func<IServerPacket> PingPacket_V755_769Factory = () => new PingPacket.V755_769();
        private static readonly Func<IServerPacket> SetTitleSubtitlePacket_V755_764Factory = () => new SetTitleSubtitlePacket.V755_764();
        private static readonly Func<IServerPacket> SetTitleTextPacket_V755_764Factory = () => new SetTitleTextPacket.V755_764();
        private static readonly Func<IServerPacket> SetTitleTimePacket_V755_769Factory = () => new SetTitleTimePacket.V755_769();
        private static readonly Func<IServerPacket> WindowItemsPacket_V756_765Factory = () => new WindowItemsPacket.V756_765();
        private static readonly Func<IServerPacket> SetSlotPacket_V756_765Factory = () => new SetSlotPacket.V756_765();
        private static readonly Func<IServerPacket> EntityDestroyPacket_V756_769Factory = () => new EntityDestroyPacket.V756_769();
        private static readonly Func<IServerPacket> TileEntityDataPacket_V757_763Factory = () => new TileEntityDataPacket.V757_763();
        private static readonly Func<IServerPacket> SimulationDistancePacket_V757_769Factory = () => new SimulationDistancePacket.V757_769();
        private static readonly Func<IServerPacket> RemoveEntityEffectPacket_V758_769Factory = () => new RemoveEntityEffectPacket.V758_769();
        private static readonly Func<IServerPacket> EntityEffectPacket_V758Factory = () => new EntityEffectPacket.V758();
        private static readonly Func<IServerPacket> SpawnEntityPacket_V759_769Factory = () => new SpawnEntityPacket.V759_769();
        private static readonly Func<IServerPacket> NamedSoundEffectPacket_V759_760Factory = () => new NamedSoundEffectPacket.V759_760();
        private static readonly Func<IServerPacket> EntityEffectPacket_V759_763Factory = () => new EntityEffectPacket.V759_763();
        private static readonly Func<IServerPacket> AcknowledgePlayerDiggingPacket_V759_769Factory = () => new AcknowledgePlayerDiggingPacket.V759_769();
        private static readonly Func<IServerPacket> InitializeWorldBorderPacket_V759_769Factory = () => new InitializeWorldBorderPacket.V759_769();
        private static readonly Func<IServerPacket> WorldBorderLerpSizePacket_V759_769Factory = () => new WorldBorderLerpSizePacket.V759_769();
        private static readonly Func<IServerPacket> ChatPreviewPacket_V759_760Factory = () => new ChatPreviewPacket.V759_760();
        private static readonly Func<IServerPacket> ShouldDisplayChatPreviewPacket_V759_760Factory = () => new ShouldDisplayChatPreviewPacket.V759_760();
        private static readonly Func<IServerPacket> SystemChatPacket_V759Factory = () => new SystemChatPacket.V759();
        private static readonly Func<IServerPacket> ServerDataPacket_V759Factory = () => new ServerDataPacket.V759();
        private static readonly Func<IServerPacket> SystemChatPacket_V760_764Factory = () => new SystemChatPacket.V760_764();
        private static readonly Func<IServerPacket> ServerDataPacket_V760Factory = () => new ServerDataPacket.V760();
        private static readonly Func<IServerPacket> ChatSuggestionsPacket_V760_769Factory = () => new ChatSuggestionsPacket.V760_769();
        private static readonly Func<IServerPacket> MessageHeaderPacket_V760Factory = () => new MessageHeaderPacket.V760();
        private static readonly Func<IServerPacket> ExperiencePacket_V761_763Factory = () => new ExperiencePacket.V761_763();
        private static readonly Func<IServerPacket> ServerDataPacket_V761Factory = () => new ServerDataPacket.V761();
        private static readonly Func<IServerPacket> PlayerRemovePacket_V761_769Factory = () => new PlayerRemovePacket.V761_769();
        private static readonly Func<IServerPacket> FeatureFlagsPacket_V761_763Factory = () => new FeatureFlagsPacket.V761_763();
        private static readonly Func<IServerPacket> PositionPacket_V762_767Factory = () => new PositionPacket.V762_767();
        private static readonly Func<IServerPacket> ServerDataPacket_V762_764Factory = () => new ServerDataPacket.V762_764();
        private static readonly Func<IServerPacket> DamageEventPacket_V762_769Factory = () => new DamageEventPacket.V762_769();
        private static readonly Func<IServerPacket> HurtAnimationPacket_V762_769Factory = () => new HurtAnimationPacket.V762_769();
        private static readonly Func<IServerPacket> OpenSignEntityPacket_V763_769Factory = () => new OpenSignEntityPacket.V763_769();
        private static readonly Func<IServerPacket> UpdateLightPacket_V763_769Factory = () => new UpdateLightPacket.V763_769();
        private static readonly Func<IServerPacket> EndCombatEventPacket_V763_769Factory = () => new EndCombatEventPacket.V763_769();
        private static readonly Func<IServerPacket> DeathCombatEventPacket_V763_764Factory = () => new DeathCombatEventPacket.V763_764();
        private static readonly Func<IServerPacket> TileEntityDataPacket_V764_769Factory = () => new TileEntityDataPacket.V764_769();
        private static readonly Func<IServerPacket> UnloadChunkPacket_V764_769Factory = () => new UnloadChunkPacket.V764_769();
        private static readonly Func<IServerPacket> ScoreboardDisplayObjectivePacket_V764_769Factory = () => new ScoreboardDisplayObjectivePacket.V764_769();
        private static readonly Func<IServerPacket> ExperiencePacket_V764_769Factory = () => new ExperiencePacket.V764_769();
        private static readonly Func<IServerPacket> EntityEffectPacket_V764_765Factory = () => new EntityEffectPacket.V764_765();
        private static readonly Func<IServerPacket> NbtQueryResponsePacket_V764_769Factory = () => new NbtQueryResponsePacket.V764_769();
        private static readonly Func<IServerPacket> ChunkBatchFinishedPacket_V764_769Factory = () => new ChunkBatchFinishedPacket.V764_769();
        private static readonly Func<IServerPacket> ChunkBatchStartPacket_V764_769Factory = () => new ChunkBatchStartPacket.V764_769();
        private static readonly Func<IServerPacket> PingResponsePacket_V764_769Factory = () => new PingResponsePacket.V764_769();
        private static readonly Func<IServerPacket> StartConfigurationPacket_V764_769Factory = () => new StartConfigurationPacket.V764_769();
        private static readonly Func<IServerPacket> KickDisconnectPacket_V765_769Factory = () => new KickDisconnectPacket.V765_769();
        private static readonly Func<IServerPacket> PlayerlistHeaderPacket_V765_769Factory = () => new PlayerlistHeaderPacket.V765_769();
        private static readonly Func<IServerPacket> DeathCombatEventPacket_V765_769Factory = () => new DeathCombatEventPacket.V765_769();
        private static readonly Func<IServerPacket> ActionBarPacket_V765_769Factory = () => new ActionBarPacket.V765_769();
        private static readonly Func<IServerPacket> SetTitleSubtitlePacket_V765_769Factory = () => new SetTitleSubtitlePacket.V765_769();
        private static readonly Func<IServerPacket> SetTitleTextPacket_V765_769Factory = () => new SetTitleTextPacket.V765_769();
        private static readonly Func<IServerPacket> SystemChatPacket_V765_769Factory = () => new SystemChatPacket.V765_769();
        private static readonly Func<IServerPacket> ServerDataPacket_V765Factory = () => new ServerDataPacket.V765();
        private static readonly Func<IServerPacket> ResetScorePacket_V765_769Factory = () => new ResetScorePacket.V765_769();
        private static readonly Func<IServerPacket> RemoveResourcePackPacket_V765_767Factory = () => new RemoveResourcePackPacket.V765_767();
        private static readonly Func<IServerPacket> AddResourcePackPacket_V765_767Factory = () => new AddResourcePackPacket.V765_767();
        private static readonly Func<IServerPacket> SetTickingStatePacket_V765_769Factory = () => new SetTickingStatePacket.V765_769();
        private static readonly Func<IServerPacket> StepTickPacket_V765_769Factory = () => new StepTickPacket.V765_769();
        private static readonly Func<IServerPacket> WindowItemsPacket_V766_767Factory = () => new WindowItemsPacket.V766_767();
        private static readonly Func<IServerPacket> SetSlotPacket_V766_767Factory = () => new SetSlotPacket.V766_767();
        private static readonly Func<IServerPacket> EntityEffectPacket_V766_769Factory = () => new EntityEffectPacket.V766_769();
        private static readonly Func<IServerPacket> ServerDataPacket_V766_769Factory = () => new ServerDataPacket.V766_769();
        private static readonly Func<IServerPacket> DebugSamplePacket_V766_769Factory = () => new DebugSamplePacket.V766_769();
        private static readonly Func<IServerPacket> CloseWindowPacket_V768_769Factory = () => new CloseWindowPacket.V768_769();
        private static readonly Func<IServerPacket> WindowItemsPacket_V768_769Factory = () => new WindowItemsPacket.V768_769();
        private static readonly Func<IServerPacket> CraftProgressBarPacket_V768_769Factory = () => new CraftProgressBarPacket.V768_769();
        private static readonly Func<IServerPacket> SetSlotPacket_V768_769Factory = () => new SetSlotPacket.V768_769();
        private static readonly Func<IServerPacket> SetCooldownPacket_V768_769Factory = () => new SetCooldownPacket.V768_769();
        private static readonly Func<IServerPacket> PositionPacket_V768_769Factory = () => new PositionPacket.V768_769();
        private static readonly Func<IServerPacket> UpdateTimePacket_V768_769Factory = () => new UpdateTimePacket.V768_769();
        private static readonly Func<IServerPacket> OpenHorseWindowPacket_V768_769Factory = () => new OpenHorseWindowPacket.V768_769();
        private static readonly Func<IServerPacket> SetProjectilePowerPacket_V767_769Factory = () => new SetProjectilePowerPacket.V767_769();
        private static readonly Func<IServerPacket> SyncEntityPositionPacket_V768_769Factory = () => new SyncEntityPositionPacket.V768_769();
        private static readonly Func<IServerPacket> PlayerRotationPacket_V768_769Factory = () => new PlayerRotationPacket.V768_769();
        private static readonly Func<IServerPacket> RecipeBookRemovePacket_V768_769Factory = () => new RecipeBookRemovePacket.V768_769();
        private static readonly Func<IServerPacket> RecipeBookSettingsPacket_V768_769Factory = () => new RecipeBookSettingsPacket.V768_769();
        private static readonly Func<IServerPacket> SetCursorItemPacket_V768_769Factory = () => new SetCursorItemPacket.V768_769();
        private static readonly Func<IServerPacket> SetPlayerInventoryPacket_V768_769Factory = () => new SetPlayerInventoryPacket.V768_769();
        private static readonly Func<IServerPacket> HeldItemSlotPacket_V769Factory = () => new HeldItemSlotPacket.V769();
        private static readonly FrozenDictionary<long, Func<IServerPacket>> ClientboundPlayPackets = new Dictionary<long, Func<IServerPacket>>
        {
            {
                Combine(340, 0),
                SpawnEntityPacket_V340_404Factory
            },
            {
                Combine(340, 1),
                SpawnEntityExperienceOrbPacket_V340_769Factory
            },
            {
                Combine(340, 2),
                SpawnEntityWeatherPacket_V340_710Factory
            },
            {
                Combine(340, 4),
                SpawnEntityPaintingPacket_V340_351Factory
            },
            {
                Combine(340, 6),
                AnimationPacket_V340_769Factory
            },
            {
                Combine(340, 8),
                BlockBreakAnimationPacket_V340_769Factory
            },
            {
                Combine(340, 9),
                TileEntityDataPacket_V340_756Factory
            },
            {
                Combine(340, 10),
                BlockActionPacket_V340_769Factory
            },
            {
                Combine(340, 11),
                BlockChangePacket_V340_769Factory
            },
            {
                Combine(340, 13),
                DifficultyPacket_V340_404Factory
            },
            {
                Combine(340, 15),
                ChatPacket_V340_710Factory
            },
            {
                Combine(340, 17),
                TransactionPacket_V340_754Factory
            },
            {
                Combine(340, 18),
                CloseWindowPacket_V340_767Factory
            },
            {
                Combine(340, 20),
                WindowItemsPacket_V340_755Factory
            },
            {
                Combine(340, 21),
                CraftProgressBarPacket_V340_767Factory
            },
            {
                Combine(340, 22),
                SetSlotPacket_V340_755Factory
            },
            {
                Combine(340, 23),
                SetCooldownPacket_V340_767Factory
            },
            {
                Combine(340, 24),
                CustomPayloadPacket_V340_769Factory
            },
            {
                Combine(340, 25),
                NamedSoundEffectPacket_V340_758Factory
            },
            {
                Combine(340, 26),
                KickDisconnectPacket_V340_764Factory
            },
            {
                Combine(340, 27),
                EntityStatusPacket_V340_769Factory
            },
            {
                Combine(340, 29),
                UnloadChunkPacket_V340_763Factory
            },
            {
                Combine(340, 30),
                GameStateChangePacket_V340_769Factory
            },
            {
                Combine(340, 31),
                KeepAlivePacket_V340_769Factory
            },
            {
                Combine(340, 33),
                WorldEventPacket_V340_769Factory
            },
            {
                Combine(340, 38),
                RelEntityMovePacket_V340_769Factory
            },
            {
                Combine(340, 39),
                EntityMoveLookPacket_V340_769Factory
            },
            {
                Combine(340, 40),
                EntityLookPacket_V340_769Factory
            },
            {
                Combine(340, 37),
                EntityPacket_V340_754Factory
            },
            {
                Combine(340, 41),
                VehicleMovePacket_V340_769Factory
            },
            {
                Combine(340, 42),
                OpenSignEntityPacket_V340_762Factory
            },
            {
                Combine(340, 44),
                AbilitiesPacket_V340_769Factory
            },
            {
                Combine(340, 47),
                PositionPacket_V340_754Factory
            },
            {
                Combine(340, 48),
                BedPacket_V340_404Factory
            },
            {
                Combine(340, 50),
                EntityDestroyPacket_V340_754Factory
            },
            {
                Combine(340, 51),
                RemoveEntityEffectPacket_V340_757Factory
            },
            {
                Combine(340, 52),
                ResourcePackSendPacket_V340_754Factory
            },
            {
                Combine(340, 54),
                EntityHeadRotationPacket_V340_769Factory
            },
            {
                Combine(340, 57),
                CameraPacket_V340_769Factory
            },
            {
                Combine(340, 58),
                HeldItemSlotPacket_V340_768Factory
            },
            {
                Combine(340, 59),
                ScoreboardDisplayObjectivePacket_V340_763Factory
            },
            {
                Combine(340, 61),
                AttachEntityPacket_V340_769Factory
            },
            {
                Combine(340, 62),
                EntityVelocityPacket_V340_769Factory
            },
            {
                Combine(340, 64),
                ExperiencePacket_V340_760Factory
            },
            {
                Combine(340, 65),
                UpdateHealthPacket_V340_769Factory
            },
            {
                Combine(340, 67),
                SetPassengersPacket_V340_769Factory
            },
            {
                Combine(340, 70),
                SpawnPositionPacket_V340_754Factory
            },
            {
                Combine(340, 71),
                UpdateTimePacket_V340_767Factory
            },
            {
                Combine(340, 74),
                PlayerlistHeaderPacket_V340_764Factory
            },
            {
                Combine(340, 75),
                CollectPacket_V340_769Factory
            },
            {
                Combine(340, 76),
                EntityTeleportPacket_V340_769Factory
            },
            {
                Combine(340, 79),
                EntityEffectPacket_V340_757Factory
            },
            {
                Combine(340, 55),
                SelectAdvancementTabPacket_V340_769Factory
            },
            {
                Combine(351, 0),
                SpawnEntityPacket_V340_404Factory
            },
            {
                Combine(351, 1),
                SpawnEntityExperienceOrbPacket_V340_769Factory
            },
            {
                Combine(351, 2),
                SpawnEntityWeatherPacket_V340_710Factory
            },
            {
                Combine(351, 4),
                SpawnEntityPaintingPacket_V340_351Factory
            },
            {
                Combine(351, 6),
                AnimationPacket_V340_769Factory
            },
            {
                Combine(351, 8),
                BlockBreakAnimationPacket_V340_769Factory
            },
            {
                Combine(351, 9),
                TileEntityDataPacket_V340_756Factory
            },
            {
                Combine(351, 10),
                BlockActionPacket_V340_769Factory
            },
            {
                Combine(351, 11),
                BlockChangePacket_V340_769Factory
            },
            {
                Combine(351, 13),
                DifficultyPacket_V340_404Factory
            },
            {
                Combine(351, 14),
                ChatPacket_V340_710Factory
            },
            {
                Combine(351, 18),
                TransactionPacket_V340_754Factory
            },
            {
                Combine(351, 19),
                CloseWindowPacket_V340_767Factory
            },
            {
                Combine(351, 21),
                WindowItemsPacket_V340_755Factory
            },
            {
                Combine(351, 22),
                CraftProgressBarPacket_V340_767Factory
            },
            {
                Combine(351, 23),
                SetSlotPacket_V340_755Factory
            },
            {
                Combine(351, 24),
                SetCooldownPacket_V340_767Factory
            },
            {
                Combine(351, 25),
                CustomPayloadPacket_V340_769Factory
            },
            {
                Combine(351, 26),
                NamedSoundEffectPacket_V340_758Factory
            },
            {
                Combine(351, 27),
                KickDisconnectPacket_V340_764Factory
            },
            {
                Combine(351, 28),
                EntityStatusPacket_V340_769Factory
            },
            {
                Combine(351, 30),
                UnloadChunkPacket_V340_763Factory
            },
            {
                Combine(351, 31),
                GameStateChangePacket_V340_769Factory
            },
            {
                Combine(351, 32),
                KeepAlivePacket_V340_769Factory
            },
            {
                Combine(351, 34),
                WorldEventPacket_V340_769Factory
            },
            {
                Combine(351, 39),
                RelEntityMovePacket_V340_769Factory
            },
            {
                Combine(351, 40),
                EntityMoveLookPacket_V340_769Factory
            },
            {
                Combine(351, 41),
                EntityLookPacket_V340_769Factory
            },
            {
                Combine(351, 38),
                EntityPacket_V340_754Factory
            },
            {
                Combine(351, 42),
                VehicleMovePacket_V340_769Factory
            },
            {
                Combine(351, 43),
                OpenSignEntityPacket_V340_762Factory
            },
            {
                Combine(351, 45),
                AbilitiesPacket_V340_769Factory
            },
            {
                Combine(351, 48),
                PositionPacket_V340_754Factory
            },
            {
                Combine(351, 49),
                BedPacket_V340_404Factory
            },
            {
                Combine(351, 51),
                EntityDestroyPacket_V340_754Factory
            },
            {
                Combine(351, 52),
                RemoveEntityEffectPacket_V340_757Factory
            },
            {
                Combine(351, 53),
                ResourcePackSendPacket_V340_754Factory
            },
            {
                Combine(351, 55),
                EntityHeadRotationPacket_V340_769Factory
            },
            {
                Combine(351, 58),
                CameraPacket_V340_769Factory
            },
            {
                Combine(351, 59),
                HeldItemSlotPacket_V340_768Factory
            },
            {
                Combine(351, 60),
                ScoreboardDisplayObjectivePacket_V340_763Factory
            },
            {
                Combine(351, 62),
                AttachEntityPacket_V340_769Factory
            },
            {
                Combine(351, 63),
                EntityVelocityPacket_V340_769Factory
            },
            {
                Combine(351, 65),
                ExperiencePacket_V340_760Factory
            },
            {
                Combine(351, 66),
                UpdateHealthPacket_V340_769Factory
            },
            {
                Combine(351, 68),
                SetPassengersPacket_V340_769Factory
            },
            {
                Combine(351, 71),
                SpawnPositionPacket_V340_754Factory
            },
            {
                Combine(351, 72),
                UpdateTimePacket_V340_767Factory
            },
            {
                Combine(351, 76),
                PlayerlistHeaderPacket_V340_764Factory
            },
            {
                Combine(351, 77),
                CollectPacket_V340_769Factory
            },
            {
                Combine(351, 78),
                EntityTeleportPacket_V340_769Factory
            },
            {
                Combine(351, 81),
                EntityEffectPacket_V340_757Factory
            },
            {
                Combine(351, 56),
                SelectAdvancementTabPacket_V340_769Factory
            },
            {
                Combine(393, 0),
                SpawnEntityPacket_V340_404Factory
            },
            {
                Combine(393, 1),
                SpawnEntityExperienceOrbPacket_V340_769Factory
            },
            {
                Combine(393, 2),
                SpawnEntityWeatherPacket_V340_710Factory
            },
            {
                Combine(393, 4),
                SpawnEntityPaintingPacket_V393_758Factory
            },
            {
                Combine(393, 6),
                AnimationPacket_V340_769Factory
            },
            {
                Combine(393, 8),
                BlockBreakAnimationPacket_V340_769Factory
            },
            {
                Combine(393, 9),
                TileEntityDataPacket_V340_756Factory
            },
            {
                Combine(393, 10),
                BlockActionPacket_V340_769Factory
            },
            {
                Combine(393, 11),
                BlockChangePacket_V340_769Factory
            },
            {
                Combine(393, 13),
                DifficultyPacket_V340_404Factory
            },
            {
                Combine(393, 14),
                ChatPacket_V340_710Factory
            },
            {
                Combine(393, 18),
                TransactionPacket_V340_754Factory
            },
            {
                Combine(393, 19),
                CloseWindowPacket_V340_767Factory
            },
            {
                Combine(393, 21),
                WindowItemsPacket_V340_755Factory
            },
            {
                Combine(393, 22),
                CraftProgressBarPacket_V340_767Factory
            },
            {
                Combine(393, 23),
                SetSlotPacket_V340_755Factory
            },
            {
                Combine(393, 24),
                SetCooldownPacket_V340_767Factory
            },
            {
                Combine(393, 25),
                CustomPayloadPacket_V340_769Factory
            },
            {
                Combine(393, 26),
                NamedSoundEffectPacket_V340_758Factory
            },
            {
                Combine(393, 27),
                KickDisconnectPacket_V340_764Factory
            },
            {
                Combine(393, 28),
                EntityStatusPacket_V340_769Factory
            },
            {
                Combine(393, 31),
                UnloadChunkPacket_V340_763Factory
            },
            {
                Combine(393, 32),
                GameStateChangePacket_V340_769Factory
            },
            {
                Combine(393, 33),
                KeepAlivePacket_V340_769Factory
            },
            {
                Combine(393, 35),
                WorldEventPacket_V340_769Factory
            },
            {
                Combine(393, 40),
                RelEntityMovePacket_V340_769Factory
            },
            {
                Combine(393, 41),
                EntityMoveLookPacket_V340_769Factory
            },
            {
                Combine(393, 42),
                EntityLookPacket_V340_769Factory
            },
            {
                Combine(393, 39),
                EntityPacket_V340_754Factory
            },
            {
                Combine(393, 43),
                VehicleMovePacket_V340_769Factory
            },
            {
                Combine(393, 44),
                OpenSignEntityPacket_V340_762Factory
            },
            {
                Combine(393, 46),
                AbilitiesPacket_V340_769Factory
            },
            {
                Combine(393, 50),
                PositionPacket_V340_754Factory
            },
            {
                Combine(393, 51),
                BedPacket_V340_404Factory
            },
            {
                Combine(393, 53),
                EntityDestroyPacket_V340_754Factory
            },
            {
                Combine(393, 54),
                RemoveEntityEffectPacket_V340_757Factory
            },
            {
                Combine(393, 55),
                ResourcePackSendPacket_V340_754Factory
            },
            {
                Combine(393, 57),
                EntityHeadRotationPacket_V340_769Factory
            },
            {
                Combine(393, 60),
                CameraPacket_V340_769Factory
            },
            {
                Combine(393, 61),
                HeldItemSlotPacket_V340_768Factory
            },
            {
                Combine(393, 62),
                ScoreboardDisplayObjectivePacket_V340_763Factory
            },
            {
                Combine(393, 64),
                AttachEntityPacket_V340_769Factory
            },
            {
                Combine(393, 65),
                EntityVelocityPacket_V340_769Factory
            },
            {
                Combine(393, 67),
                ExperiencePacket_V340_760Factory
            },
            {
                Combine(393, 68),
                UpdateHealthPacket_V340_769Factory
            },
            {
                Combine(393, 70),
                SetPassengersPacket_V340_769Factory
            },
            {
                Combine(393, 73),
                SpawnPositionPacket_V340_754Factory
            },
            {
                Combine(393, 74),
                UpdateTimePacket_V340_767Factory
            },
            {
                Combine(393, 78),
                PlayerlistHeaderPacket_V340_764Factory
            },
            {
                Combine(393, 79),
                CollectPacket_V340_769Factory
            },
            {
                Combine(393, 80),
                EntityTeleportPacket_V340_769Factory
            },
            {
                Combine(393, 83),
                EntityEffectPacket_V340_757Factory
            },
            {
                Combine(393, 58),
                SelectAdvancementTabPacket_V340_769Factory
            },
            {
                Combine(393, 29),
                NbtQueryResponsePacket_V393_763Factory
            },
            {
                Combine(401, 0),
                SpawnEntityPacket_V340_404Factory
            },
            {
                Combine(401, 1),
                SpawnEntityExperienceOrbPacket_V340_769Factory
            },
            {
                Combine(401, 2),
                SpawnEntityWeatherPacket_V340_710Factory
            },
            {
                Combine(401, 4),
                SpawnEntityPaintingPacket_V393_758Factory
            },
            {
                Combine(401, 6),
                AnimationPacket_V340_769Factory
            },
            {
                Combine(401, 8),
                BlockBreakAnimationPacket_V340_769Factory
            },
            {
                Combine(401, 9),
                TileEntityDataPacket_V340_756Factory
            },
            {
                Combine(401, 10),
                BlockActionPacket_V340_769Factory
            },
            {
                Combine(401, 11),
                BlockChangePacket_V340_769Factory
            },
            {
                Combine(401, 13),
                DifficultyPacket_V340_404Factory
            },
            {
                Combine(401, 14),
                ChatPacket_V340_710Factory
            },
            {
                Combine(401, 18),
                TransactionPacket_V340_754Factory
            },
            {
                Combine(401, 19),
                CloseWindowPacket_V340_767Factory
            },
            {
                Combine(401, 21),
                WindowItemsPacket_V340_755Factory
            },
            {
                Combine(401, 22),
                CraftProgressBarPacket_V340_767Factory
            },
            {
                Combine(401, 23),
                SetSlotPacket_V340_755Factory
            },
            {
                Combine(401, 24),
                SetCooldownPacket_V340_767Factory
            },
            {
                Combine(401, 25),
                CustomPayloadPacket_V340_769Factory
            },
            {
                Combine(401, 26),
                NamedSoundEffectPacket_V340_758Factory
            },
            {
                Combine(401, 27),
                KickDisconnectPacket_V340_764Factory
            },
            {
                Combine(401, 28),
                EntityStatusPacket_V340_769Factory
            },
            {
                Combine(401, 31),
                UnloadChunkPacket_V340_763Factory
            },
            {
                Combine(401, 32),
                GameStateChangePacket_V340_769Factory
            },
            {
                Combine(401, 33),
                KeepAlivePacket_V340_769Factory
            },
            {
                Combine(401, 35),
                WorldEventPacket_V340_769Factory
            },
            {
                Combine(401, 40),
                RelEntityMovePacket_V340_769Factory
            },
            {
                Combine(401, 41),
                EntityMoveLookPacket_V340_769Factory
            },
            {
                Combine(401, 42),
                EntityLookPacket_V340_769Factory
            },
            {
                Combine(401, 39),
                EntityPacket_V340_754Factory
            },
            {
                Combine(401, 43),
                VehicleMovePacket_V340_769Factory
            },
            {
                Combine(401, 44),
                OpenSignEntityPacket_V340_762Factory
            },
            {
                Combine(401, 46),
                AbilitiesPacket_V340_769Factory
            },
            {
                Combine(401, 50),
                PositionPacket_V340_754Factory
            },
            {
                Combine(401, 51),
                BedPacket_V340_404Factory
            },
            {
                Combine(401, 53),
                EntityDestroyPacket_V340_754Factory
            },
            {
                Combine(401, 54),
                RemoveEntityEffectPacket_V340_757Factory
            },
            {
                Combine(401, 55),
                ResourcePackSendPacket_V340_754Factory
            },
            {
                Combine(401, 57),
                EntityHeadRotationPacket_V340_769Factory
            },
            {
                Combine(401, 60),
                CameraPacket_V340_769Factory
            },
            {
                Combine(401, 61),
                HeldItemSlotPacket_V340_768Factory
            },
            {
                Combine(401, 62),
                ScoreboardDisplayObjectivePacket_V340_763Factory
            },
            {
                Combine(401, 64),
                AttachEntityPacket_V340_769Factory
            },
            {
                Combine(401, 65),
                EntityVelocityPacket_V340_769Factory
            },
            {
                Combine(401, 67),
                ExperiencePacket_V340_760Factory
            },
            {
                Combine(401, 68),
                UpdateHealthPacket_V340_769Factory
            },
            {
                Combine(401, 70),
                SetPassengersPacket_V340_769Factory
            },
            {
                Combine(401, 73),
                SpawnPositionPacket_V340_754Factory
            },
            {
                Combine(401, 74),
                UpdateTimePacket_V340_767Factory
            },
            {
                Combine(401, 78),
                PlayerlistHeaderPacket_V340_764Factory
            },
            {
                Combine(401, 79),
                CollectPacket_V340_769Factory
            },
            {
                Combine(401, 80),
                EntityTeleportPacket_V340_769Factory
            },
            {
                Combine(401, 83),
                EntityEffectPacket_V340_757Factory
            },
            {
                Combine(401, 58),
                SelectAdvancementTabPacket_V340_769Factory
            },
            {
                Combine(401, 29),
                NbtQueryResponsePacket_V393_763Factory
            },
            {
                Combine(402, 0),
                SpawnEntityPacket_V340_404Factory
            },
            {
                Combine(402, 1),
                SpawnEntityExperienceOrbPacket_V340_769Factory
            },
            {
                Combine(402, 2),
                SpawnEntityWeatherPacket_V340_710Factory
            },
            {
                Combine(402, 4),
                SpawnEntityPaintingPacket_V393_758Factory
            },
            {
                Combine(402, 6),
                AnimationPacket_V340_769Factory
            },
            {
                Combine(402, 8),
                BlockBreakAnimationPacket_V340_769Factory
            },
            {
                Combine(402, 9),
                TileEntityDataPacket_V340_756Factory
            },
            {
                Combine(402, 10),
                BlockActionPacket_V340_769Factory
            },
            {
                Combine(402, 11),
                BlockChangePacket_V340_769Factory
            },
            {
                Combine(402, 13),
                DifficultyPacket_V340_404Factory
            },
            {
                Combine(402, 14),
                ChatPacket_V340_710Factory
            },
            {
                Combine(402, 18),
                TransactionPacket_V340_754Factory
            },
            {
                Combine(402, 19),
                CloseWindowPacket_V340_767Factory
            },
            {
                Combine(402, 21),
                WindowItemsPacket_V340_755Factory
            },
            {
                Combine(402, 22),
                CraftProgressBarPacket_V340_767Factory
            },
            {
                Combine(402, 23),
                SetSlotPacket_V340_755Factory
            },
            {
                Combine(402, 24),
                SetCooldownPacket_V340_767Factory
            },
            {
                Combine(402, 25),
                CustomPayloadPacket_V340_769Factory
            },
            {
                Combine(402, 26),
                NamedSoundEffectPacket_V340_758Factory
            },
            {
                Combine(402, 27),
                KickDisconnectPacket_V340_764Factory
            },
            {
                Combine(402, 28),
                EntityStatusPacket_V340_769Factory
            },
            {
                Combine(402, 31),
                UnloadChunkPacket_V340_763Factory
            },
            {
                Combine(402, 32),
                GameStateChangePacket_V340_769Factory
            },
            {
                Combine(402, 33),
                KeepAlivePacket_V340_769Factory
            },
            {
                Combine(402, 35),
                WorldEventPacket_V340_769Factory
            },
            {
                Combine(402, 40),
                RelEntityMovePacket_V340_769Factory
            },
            {
                Combine(402, 41),
                EntityMoveLookPacket_V340_769Factory
            },
            {
                Combine(402, 42),
                EntityLookPacket_V340_769Factory
            },
            {
                Combine(402, 39),
                EntityPacket_V340_754Factory
            },
            {
                Combine(402, 43),
                VehicleMovePacket_V340_769Factory
            },
            {
                Combine(402, 44),
                OpenSignEntityPacket_V340_762Factory
            },
            {
                Combine(402, 46),
                AbilitiesPacket_V340_769Factory
            },
            {
                Combine(402, 50),
                PositionPacket_V340_754Factory
            },
            {
                Combine(402, 51),
                BedPacket_V340_404Factory
            },
            {
                Combine(402, 53),
                EntityDestroyPacket_V340_754Factory
            },
            {
                Combine(402, 54),
                RemoveEntityEffectPacket_V340_757Factory
            },
            {
                Combine(402, 55),
                ResourcePackSendPacket_V340_754Factory
            },
            {
                Combine(402, 57),
                EntityHeadRotationPacket_V340_769Factory
            },
            {
                Combine(402, 60),
                CameraPacket_V340_769Factory
            },
            {
                Combine(402, 61),
                HeldItemSlotPacket_V340_768Factory
            },
            {
                Combine(402, 62),
                ScoreboardDisplayObjectivePacket_V340_763Factory
            },
            {
                Combine(402, 64),
                AttachEntityPacket_V340_769Factory
            },
            {
                Combine(402, 65),
                EntityVelocityPacket_V340_769Factory
            },
            {
                Combine(402, 67),
                ExperiencePacket_V340_760Factory
            },
            {
                Combine(402, 68),
                UpdateHealthPacket_V340_769Factory
            },
            {
                Combine(402, 70),
                SetPassengersPacket_V340_769Factory
            },
            {
                Combine(402, 73),
                SpawnPositionPacket_V340_754Factory
            },
            {
                Combine(402, 74),
                UpdateTimePacket_V340_767Factory
            },
            {
                Combine(402, 78),
                PlayerlistHeaderPacket_V340_764Factory
            },
            {
                Combine(402, 79),
                CollectPacket_V340_769Factory
            },
            {
                Combine(402, 80),
                EntityTeleportPacket_V340_769Factory
            },
            {
                Combine(402, 83),
                EntityEffectPacket_V340_757Factory
            },
            {
                Combine(402, 58),
                SelectAdvancementTabPacket_V340_769Factory
            },
            {
                Combine(402, 29),
                NbtQueryResponsePacket_V393_763Factory
            },
            {
                Combine(403, 0),
                SpawnEntityPacket_V340_404Factory
            },
            {
                Combine(403, 1),
                SpawnEntityExperienceOrbPacket_V340_769Factory
            },
            {
                Combine(403, 2),
                SpawnEntityWeatherPacket_V340_710Factory
            },
            {
                Combine(403, 4),
                SpawnEntityPaintingPacket_V393_758Factory
            },
            {
                Combine(403, 6),
                AnimationPacket_V340_769Factory
            },
            {
                Combine(403, 8),
                BlockBreakAnimationPacket_V340_769Factory
            },
            {
                Combine(403, 9),
                TileEntityDataPacket_V340_756Factory
            },
            {
                Combine(403, 10),
                BlockActionPacket_V340_769Factory
            },
            {
                Combine(403, 11),
                BlockChangePacket_V340_769Factory
            },
            {
                Combine(403, 13),
                DifficultyPacket_V340_404Factory
            },
            {
                Combine(403, 14),
                ChatPacket_V340_710Factory
            },
            {
                Combine(403, 18),
                TransactionPacket_V340_754Factory
            },
            {
                Combine(403, 19),
                CloseWindowPacket_V340_767Factory
            },
            {
                Combine(403, 21),
                WindowItemsPacket_V340_755Factory
            },
            {
                Combine(403, 22),
                CraftProgressBarPacket_V340_767Factory
            },
            {
                Combine(403, 23),
                SetSlotPacket_V340_755Factory
            },
            {
                Combine(403, 24),
                SetCooldownPacket_V340_767Factory
            },
            {
                Combine(403, 25),
                CustomPayloadPacket_V340_769Factory
            },
            {
                Combine(403, 26),
                NamedSoundEffectPacket_V340_758Factory
            },
            {
                Combine(403, 27),
                KickDisconnectPacket_V340_764Factory
            },
            {
                Combine(403, 28),
                EntityStatusPacket_V340_769Factory
            },
            {
                Combine(403, 31),
                UnloadChunkPacket_V340_763Factory
            },
            {
                Combine(403, 32),
                GameStateChangePacket_V340_769Factory
            },
            {
                Combine(403, 33),
                KeepAlivePacket_V340_769Factory
            },
            {
                Combine(403, 35),
                WorldEventPacket_V340_769Factory
            },
            {
                Combine(403, 40),
                RelEntityMovePacket_V340_769Factory
            },
            {
                Combine(403, 41),
                EntityMoveLookPacket_V340_769Factory
            },
            {
                Combine(403, 42),
                EntityLookPacket_V340_769Factory
            },
            {
                Combine(403, 39),
                EntityPacket_V340_754Factory
            },
            {
                Combine(403, 43),
                VehicleMovePacket_V340_769Factory
            },
            {
                Combine(403, 44),
                OpenSignEntityPacket_V340_762Factory
            },
            {
                Combine(403, 46),
                AbilitiesPacket_V340_769Factory
            },
            {
                Combine(403, 50),
                PositionPacket_V340_754Factory
            },
            {
                Combine(403, 51),
                BedPacket_V340_404Factory
            },
            {
                Combine(403, 53),
                EntityDestroyPacket_V340_754Factory
            },
            {
                Combine(403, 54),
                RemoveEntityEffectPacket_V340_757Factory
            },
            {
                Combine(403, 55),
                ResourcePackSendPacket_V340_754Factory
            },
            {
                Combine(403, 57),
                EntityHeadRotationPacket_V340_769Factory
            },
            {
                Combine(403, 60),
                CameraPacket_V340_769Factory
            },
            {
                Combine(403, 61),
                HeldItemSlotPacket_V340_768Factory
            },
            {
                Combine(403, 62),
                ScoreboardDisplayObjectivePacket_V340_763Factory
            },
            {
                Combine(403, 64),
                AttachEntityPacket_V340_769Factory
            },
            {
                Combine(403, 65),
                EntityVelocityPacket_V340_769Factory
            },
            {
                Combine(403, 67),
                ExperiencePacket_V340_760Factory
            },
            {
                Combine(403, 68),
                UpdateHealthPacket_V340_769Factory
            },
            {
                Combine(403, 70),
                SetPassengersPacket_V340_769Factory
            },
            {
                Combine(403, 73),
                SpawnPositionPacket_V340_754Factory
            },
            {
                Combine(403, 74),
                UpdateTimePacket_V340_767Factory
            },
            {
                Combine(403, 78),
                PlayerlistHeaderPacket_V340_764Factory
            },
            {
                Combine(403, 79),
                CollectPacket_V340_769Factory
            },
            {
                Combine(403, 80),
                EntityTeleportPacket_V340_769Factory
            },
            {
                Combine(403, 83),
                EntityEffectPacket_V340_757Factory
            },
            {
                Combine(403, 58),
                SelectAdvancementTabPacket_V340_769Factory
            },
            {
                Combine(403, 29),
                NbtQueryResponsePacket_V393_763Factory
            },
            {
                Combine(404, 0),
                SpawnEntityPacket_V340_404Factory
            },
            {
                Combine(404, 1),
                SpawnEntityExperienceOrbPacket_V340_769Factory
            },
            {
                Combine(404, 2),
                SpawnEntityWeatherPacket_V340_710Factory
            },
            {
                Combine(404, 4),
                SpawnEntityPaintingPacket_V393_758Factory
            },
            {
                Combine(404, 6),
                AnimationPacket_V340_769Factory
            },
            {
                Combine(404, 8),
                BlockBreakAnimationPacket_V340_769Factory
            },
            {
                Combine(404, 9),
                TileEntityDataPacket_V340_756Factory
            },
            {
                Combine(404, 10),
                BlockActionPacket_V340_769Factory
            },
            {
                Combine(404, 11),
                BlockChangePacket_V340_769Factory
            },
            {
                Combine(404, 13),
                DifficultyPacket_V340_404Factory
            },
            {
                Combine(404, 14),
                ChatPacket_V340_710Factory
            },
            {
                Combine(404, 18),
                TransactionPacket_V340_754Factory
            },
            {
                Combine(404, 19),
                CloseWindowPacket_V340_767Factory
            },
            {
                Combine(404, 21),
                WindowItemsPacket_V340_755Factory
            },
            {
                Combine(404, 22),
                CraftProgressBarPacket_V340_767Factory
            },
            {
                Combine(404, 23),
                SetSlotPacket_V340_755Factory
            },
            {
                Combine(404, 24),
                SetCooldownPacket_V340_767Factory
            },
            {
                Combine(404, 25),
                CustomPayloadPacket_V340_769Factory
            },
            {
                Combine(404, 26),
                NamedSoundEffectPacket_V340_758Factory
            },
            {
                Combine(404, 27),
                KickDisconnectPacket_V340_764Factory
            },
            {
                Combine(404, 28),
                EntityStatusPacket_V340_769Factory
            },
            {
                Combine(404, 31),
                UnloadChunkPacket_V340_763Factory
            },
            {
                Combine(404, 32),
                GameStateChangePacket_V340_769Factory
            },
            {
                Combine(404, 33),
                KeepAlivePacket_V340_769Factory
            },
            {
                Combine(404, 35),
                WorldEventPacket_V340_769Factory
            },
            {
                Combine(404, 40),
                RelEntityMovePacket_V340_769Factory
            },
            {
                Combine(404, 41),
                EntityMoveLookPacket_V340_769Factory
            },
            {
                Combine(404, 42),
                EntityLookPacket_V340_769Factory
            },
            {
                Combine(404, 39),
                EntityPacket_V340_754Factory
            },
            {
                Combine(404, 43),
                VehicleMovePacket_V340_769Factory
            },
            {
                Combine(404, 44),
                OpenSignEntityPacket_V340_762Factory
            },
            {
                Combine(404, 46),
                AbilitiesPacket_V340_769Factory
            },
            {
                Combine(404, 50),
                PositionPacket_V340_754Factory
            },
            {
                Combine(404, 51),
                BedPacket_V340_404Factory
            },
            {
                Combine(404, 53),
                EntityDestroyPacket_V340_754Factory
            },
            {
                Combine(404, 54),
                RemoveEntityEffectPacket_V340_757Factory
            },
            {
                Combine(404, 55),
                ResourcePackSendPacket_V340_754Factory
            },
            {
                Combine(404, 57),
                EntityHeadRotationPacket_V340_769Factory
            },
            {
                Combine(404, 60),
                CameraPacket_V340_769Factory
            },
            {
                Combine(404, 61),
                HeldItemSlotPacket_V340_768Factory
            },
            {
                Combine(404, 62),
                ScoreboardDisplayObjectivePacket_V340_763Factory
            },
            {
                Combine(404, 64),
                AttachEntityPacket_V340_769Factory
            },
            {
                Combine(404, 65),
                EntityVelocityPacket_V340_769Factory
            },
            {
                Combine(404, 67),
                ExperiencePacket_V340_760Factory
            },
            {
                Combine(404, 68),
                UpdateHealthPacket_V340_769Factory
            },
            {
                Combine(404, 70),
                SetPassengersPacket_V340_769Factory
            },
            {
                Combine(404, 73),
                SpawnPositionPacket_V340_754Factory
            },
            {
                Combine(404, 74),
                UpdateTimePacket_V340_767Factory
            },
            {
                Combine(404, 78),
                PlayerlistHeaderPacket_V340_764Factory
            },
            {
                Combine(404, 79),
                CollectPacket_V340_769Factory
            },
            {
                Combine(404, 80),
                EntityTeleportPacket_V340_769Factory
            },
            {
                Combine(404, 83),
                EntityEffectPacket_V340_757Factory
            },
            {
                Combine(404, 58),
                SelectAdvancementTabPacket_V340_769Factory
            },
            {
                Combine(404, 29),
                NbtQueryResponsePacket_V393_763Factory
            },
            {
                Combine(477, 0),
                SpawnEntityPacket_V477_758Factory
            },
            {
                Combine(477, 1),
                SpawnEntityExperienceOrbPacket_V340_769Factory
            },
            {
                Combine(477, 2),
                SpawnEntityWeatherPacket_V340_710Factory
            },
            {
                Combine(477, 4),
                SpawnEntityPaintingPacket_V393_758Factory
            },
            {
                Combine(477, 6),
                AnimationPacket_V340_769Factory
            },
            {
                Combine(477, 8),
                BlockBreakAnimationPacket_V340_769Factory
            },
            {
                Combine(477, 9),
                TileEntityDataPacket_V340_756Factory
            },
            {
                Combine(477, 10),
                BlockActionPacket_V340_769Factory
            },
            {
                Combine(477, 11),
                BlockChangePacket_V340_769Factory
            },
            {
                Combine(477, 13),
                DifficultyPacket_V477_769Factory
            },
            {
                Combine(477, 14),
                ChatPacket_V340_710Factory
            },
            {
                Combine(477, 18),
                TransactionPacket_V340_754Factory
            },
            {
                Combine(477, 19),
                CloseWindowPacket_V340_767Factory
            },
            {
                Combine(477, 20),
                WindowItemsPacket_V340_755Factory
            },
            {
                Combine(477, 21),
                CraftProgressBarPacket_V340_767Factory
            },
            {
                Combine(477, 22),
                SetSlotPacket_V340_755Factory
            },
            {
                Combine(477, 23),
                SetCooldownPacket_V340_767Factory
            },
            {
                Combine(477, 24),
                CustomPayloadPacket_V340_769Factory
            },
            {
                Combine(477, 25),
                NamedSoundEffectPacket_V340_758Factory
            },
            {
                Combine(477, 26),
                KickDisconnectPacket_V340_764Factory
            },
            {
                Combine(477, 27),
                EntityStatusPacket_V340_769Factory
            },
            {
                Combine(477, 29),
                UnloadChunkPacket_V340_763Factory
            },
            {
                Combine(477, 30),
                GameStateChangePacket_V340_769Factory
            },
            {
                Combine(477, 32),
                KeepAlivePacket_V340_769Factory
            },
            {
                Combine(477, 34),
                WorldEventPacket_V340_769Factory
            },
            {
                Combine(477, 40),
                RelEntityMovePacket_V340_769Factory
            },
            {
                Combine(477, 41),
                EntityMoveLookPacket_V340_769Factory
            },
            {
                Combine(477, 42),
                EntityLookPacket_V340_769Factory
            },
            {
                Combine(477, 43),
                EntityPacket_V340_754Factory
            },
            {
                Combine(477, 44),
                VehicleMovePacket_V340_769Factory
            },
            {
                Combine(477, 47),
                OpenSignEntityPacket_V340_762Factory
            },
            {
                Combine(477, 49),
                AbilitiesPacket_V340_769Factory
            },
            {
                Combine(477, 53),
                PositionPacket_V340_754Factory
            },
            {
                Combine(477, 55),
                EntityDestroyPacket_V340_754Factory
            },
            {
                Combine(477, 56),
                RemoveEntityEffectPacket_V340_757Factory
            },
            {
                Combine(477, 57),
                ResourcePackSendPacket_V340_754Factory
            },
            {
                Combine(477, 59),
                EntityHeadRotationPacket_V340_769Factory
            },
            {
                Combine(477, 62),
                CameraPacket_V340_769Factory
            },
            {
                Combine(477, 63),
                HeldItemSlotPacket_V340_768Factory
            },
            {
                Combine(477, 66),
                ScoreboardDisplayObjectivePacket_V340_763Factory
            },
            {
                Combine(477, 68),
                AttachEntityPacket_V340_769Factory
            },
            {
                Combine(477, 69),
                EntityVelocityPacket_V340_769Factory
            },
            {
                Combine(477, 71),
                ExperiencePacket_V340_760Factory
            },
            {
                Combine(477, 72),
                UpdateHealthPacket_V340_769Factory
            },
            {
                Combine(477, 74),
                SetPassengersPacket_V340_769Factory
            },
            {
                Combine(477, 77),
                SpawnPositionPacket_V340_754Factory
            },
            {
                Combine(477, 78),
                UpdateTimePacket_V340_767Factory
            },
            {
                Combine(477, 83),
                PlayerlistHeaderPacket_V340_764Factory
            },
            {
                Combine(477, 85),
                CollectPacket_V340_769Factory
            },
            {
                Combine(477, 86),
                EntityTeleportPacket_V340_769Factory
            },
            {
                Combine(477, 89),
                EntityEffectPacket_V340_757Factory
            },
            {
                Combine(477, 60),
                SelectAdvancementTabPacket_V340_769Factory
            },
            {
                Combine(477, 84),
                NbtQueryResponsePacket_V393_763Factory
            },
            {
                Combine(477, 31),
                OpenHorseWindowPacket_V477_767Factory
            },
            {
                Combine(477, 36),
                UpdateLightPacket_V477_710Factory
            },
            {
                Combine(477, 45),
                OpenBookPacket_V477_769Factory
            },
            {
                Combine(477, 64),
                UpdateViewPositionPacket_V477_769Factory
            },
            {
                Combine(477, 65),
                UpdateViewDistancePacket_V477_769Factory
            },
            {
                Combine(480, 0),
                SpawnEntityPacket_V477_758Factory
            },
            {
                Combine(480, 1),
                SpawnEntityExperienceOrbPacket_V340_769Factory
            },
            {
                Combine(480, 2),
                SpawnEntityWeatherPacket_V340_710Factory
            },
            {
                Combine(480, 4),
                SpawnEntityPaintingPacket_V393_758Factory
            },
            {
                Combine(480, 6),
                AnimationPacket_V340_769Factory
            },
            {
                Combine(480, 8),
                BlockBreakAnimationPacket_V340_769Factory
            },
            {
                Combine(480, 9),
                TileEntityDataPacket_V340_756Factory
            },
            {
                Combine(480, 10),
                BlockActionPacket_V340_769Factory
            },
            {
                Combine(480, 11),
                BlockChangePacket_V340_769Factory
            },
            {
                Combine(480, 13),
                DifficultyPacket_V477_769Factory
            },
            {
                Combine(480, 14),
                ChatPacket_V340_710Factory
            },
            {
                Combine(480, 18),
                TransactionPacket_V340_754Factory
            },
            {
                Combine(480, 19),
                CloseWindowPacket_V340_767Factory
            },
            {
                Combine(480, 20),
                WindowItemsPacket_V340_755Factory
            },
            {
                Combine(480, 21),
                CraftProgressBarPacket_V340_767Factory
            },
            {
                Combine(480, 22),
                SetSlotPacket_V340_755Factory
            },
            {
                Combine(480, 23),
                SetCooldownPacket_V340_767Factory
            },
            {
                Combine(480, 24),
                CustomPayloadPacket_V340_769Factory
            },
            {
                Combine(480, 25),
                NamedSoundEffectPacket_V340_758Factory
            },
            {
                Combine(480, 26),
                KickDisconnectPacket_V340_764Factory
            },
            {
                Combine(480, 27),
                EntityStatusPacket_V340_769Factory
            },
            {
                Combine(480, 29),
                UnloadChunkPacket_V340_763Factory
            },
            {
                Combine(480, 30),
                GameStateChangePacket_V340_769Factory
            },
            {
                Combine(480, 32),
                KeepAlivePacket_V340_769Factory
            },
            {
                Combine(480, 34),
                WorldEventPacket_V340_769Factory
            },
            {
                Combine(480, 40),
                RelEntityMovePacket_V340_769Factory
            },
            {
                Combine(480, 41),
                EntityMoveLookPacket_V340_769Factory
            },
            {
                Combine(480, 42),
                EntityLookPacket_V340_769Factory
            },
            {
                Combine(480, 43),
                EntityPacket_V340_754Factory
            },
            {
                Combine(480, 44),
                VehicleMovePacket_V340_769Factory
            },
            {
                Combine(480, 47),
                OpenSignEntityPacket_V340_762Factory
            },
            {
                Combine(480, 49),
                AbilitiesPacket_V340_769Factory
            },
            {
                Combine(480, 53),
                PositionPacket_V340_754Factory
            },
            {
                Combine(480, 55),
                EntityDestroyPacket_V340_754Factory
            },
            {
                Combine(480, 56),
                RemoveEntityEffectPacket_V340_757Factory
            },
            {
                Combine(480, 57),
                ResourcePackSendPacket_V340_754Factory
            },
            {
                Combine(480, 59),
                EntityHeadRotationPacket_V340_769Factory
            },
            {
                Combine(480, 62),
                CameraPacket_V340_769Factory
            },
            {
                Combine(480, 63),
                HeldItemSlotPacket_V340_768Factory
            },
            {
                Combine(480, 66),
                ScoreboardDisplayObjectivePacket_V340_763Factory
            },
            {
                Combine(480, 68),
                AttachEntityPacket_V340_769Factory
            },
            {
                Combine(480, 69),
                EntityVelocityPacket_V340_769Factory
            },
            {
                Combine(480, 71),
                ExperiencePacket_V340_760Factory
            },
            {
                Combine(480, 72),
                UpdateHealthPacket_V340_769Factory
            },
            {
                Combine(480, 74),
                SetPassengersPacket_V340_769Factory
            },
            {
                Combine(480, 77),
                SpawnPositionPacket_V340_754Factory
            },
            {
                Combine(480, 78),
                UpdateTimePacket_V340_767Factory
            },
            {
                Combine(480, 83),
                PlayerlistHeaderPacket_V340_764Factory
            },
            {
                Combine(480, 85),
                CollectPacket_V340_769Factory
            },
            {
                Combine(480, 86),
                EntityTeleportPacket_V340_769Factory
            },
            {
                Combine(480, 89),
                EntityEffectPacket_V340_757Factory
            },
            {
                Combine(480, 60),
                SelectAdvancementTabPacket_V340_769Factory
            },
            {
                Combine(480, 84),
                NbtQueryResponsePacket_V393_763Factory
            },
            {
                Combine(480, 31),
                OpenHorseWindowPacket_V477_767Factory
            },
            {
                Combine(480, 36),
                UpdateLightPacket_V477_710Factory
            },
            {
                Combine(480, 45),
                OpenBookPacket_V477_769Factory
            },
            {
                Combine(480, 64),
                UpdateViewPositionPacket_V477_769Factory
            },
            {
                Combine(480, 65),
                UpdateViewDistancePacket_V477_769Factory
            },
            {
                Combine(490, 0),
                SpawnEntityPacket_V477_758Factory
            },
            {
                Combine(490, 1),
                SpawnEntityExperienceOrbPacket_V340_769Factory
            },
            {
                Combine(490, 2),
                SpawnEntityWeatherPacket_V340_710Factory
            },
            {
                Combine(490, 4),
                SpawnEntityPaintingPacket_V393_758Factory
            },
            {
                Combine(490, 6),
                AnimationPacket_V340_769Factory
            },
            {
                Combine(490, 8),
                BlockBreakAnimationPacket_V340_769Factory
            },
            {
                Combine(490, 9),
                TileEntityDataPacket_V340_756Factory
            },
            {
                Combine(490, 10),
                BlockActionPacket_V340_769Factory
            },
            {
                Combine(490, 11),
                BlockChangePacket_V340_769Factory
            },
            {
                Combine(490, 13),
                DifficultyPacket_V477_769Factory
            },
            {
                Combine(490, 14),
                ChatPacket_V340_710Factory
            },
            {
                Combine(490, 18),
                TransactionPacket_V340_754Factory
            },
            {
                Combine(490, 19),
                CloseWindowPacket_V340_767Factory
            },
            {
                Combine(490, 20),
                WindowItemsPacket_V340_755Factory
            },
            {
                Combine(490, 21),
                CraftProgressBarPacket_V340_767Factory
            },
            {
                Combine(490, 22),
                SetSlotPacket_V340_755Factory
            },
            {
                Combine(490, 23),
                SetCooldownPacket_V340_767Factory
            },
            {
                Combine(490, 24),
                CustomPayloadPacket_V340_769Factory
            },
            {
                Combine(490, 25),
                NamedSoundEffectPacket_V340_758Factory
            },
            {
                Combine(490, 26),
                KickDisconnectPacket_V340_764Factory
            },
            {
                Combine(490, 27),
                EntityStatusPacket_V340_769Factory
            },
            {
                Combine(490, 29),
                UnloadChunkPacket_V340_763Factory
            },
            {
                Combine(490, 30),
                GameStateChangePacket_V340_769Factory
            },
            {
                Combine(490, 32),
                KeepAlivePacket_V340_769Factory
            },
            {
                Combine(490, 34),
                WorldEventPacket_V340_769Factory
            },
            {
                Combine(490, 40),
                RelEntityMovePacket_V340_769Factory
            },
            {
                Combine(490, 41),
                EntityMoveLookPacket_V340_769Factory
            },
            {
                Combine(490, 42),
                EntityLookPacket_V340_769Factory
            },
            {
                Combine(490, 43),
                EntityPacket_V340_754Factory
            },
            {
                Combine(490, 44),
                VehicleMovePacket_V340_769Factory
            },
            {
                Combine(490, 47),
                OpenSignEntityPacket_V340_762Factory
            },
            {
                Combine(490, 49),
                AbilitiesPacket_V340_769Factory
            },
            {
                Combine(490, 53),
                PositionPacket_V340_754Factory
            },
            {
                Combine(490, 55),
                EntityDestroyPacket_V340_754Factory
            },
            {
                Combine(490, 56),
                RemoveEntityEffectPacket_V340_757Factory
            },
            {
                Combine(490, 57),
                ResourcePackSendPacket_V340_754Factory
            },
            {
                Combine(490, 59),
                EntityHeadRotationPacket_V340_769Factory
            },
            {
                Combine(490, 62),
                CameraPacket_V340_769Factory
            },
            {
                Combine(490, 63),
                HeldItemSlotPacket_V340_768Factory
            },
            {
                Combine(490, 66),
                ScoreboardDisplayObjectivePacket_V340_763Factory
            },
            {
                Combine(490, 68),
                AttachEntityPacket_V340_769Factory
            },
            {
                Combine(490, 69),
                EntityVelocityPacket_V340_769Factory
            },
            {
                Combine(490, 71),
                ExperiencePacket_V340_760Factory
            },
            {
                Combine(490, 72),
                UpdateHealthPacket_V340_769Factory
            },
            {
                Combine(490, 74),
                SetPassengersPacket_V340_769Factory
            },
            {
                Combine(490, 77),
                SpawnPositionPacket_V340_754Factory
            },
            {
                Combine(490, 78),
                UpdateTimePacket_V340_767Factory
            },
            {
                Combine(490, 83),
                PlayerlistHeaderPacket_V340_764Factory
            },
            {
                Combine(490, 85),
                CollectPacket_V340_769Factory
            },
            {
                Combine(490, 86),
                EntityTeleportPacket_V340_769Factory
            },
            {
                Combine(490, 89),
                EntityEffectPacket_V340_757Factory
            },
            {
                Combine(490, 60),
                SelectAdvancementTabPacket_V340_769Factory
            },
            {
                Combine(490, 84),
                NbtQueryResponsePacket_V393_763Factory
            },
            {
                Combine(490, 31),
                OpenHorseWindowPacket_V477_767Factory
            },
            {
                Combine(490, 36),
                UpdateLightPacket_V477_710Factory
            },
            {
                Combine(490, 45),
                OpenBookPacket_V477_769Factory
            },
            {
                Combine(490, 64),
                UpdateViewPositionPacket_V477_769Factory
            },
            {
                Combine(490, 65),
                UpdateViewDistancePacket_V477_769Factory
            },
            {
                Combine(498, 0),
                SpawnEntityPacket_V477_758Factory
            },
            {
                Combine(498, 1),
                SpawnEntityExperienceOrbPacket_V340_769Factory
            },
            {
                Combine(498, 2),
                SpawnEntityWeatherPacket_V340_710Factory
            },
            {
                Combine(498, 4),
                SpawnEntityPaintingPacket_V393_758Factory
            },
            {
                Combine(498, 6),
                AnimationPacket_V340_769Factory
            },
            {
                Combine(498, 8),
                BlockBreakAnimationPacket_V340_769Factory
            },
            {
                Combine(498, 9),
                TileEntityDataPacket_V340_756Factory
            },
            {
                Combine(498, 10),
                BlockActionPacket_V340_769Factory
            },
            {
                Combine(498, 11),
                BlockChangePacket_V340_769Factory
            },
            {
                Combine(498, 13),
                DifficultyPacket_V477_769Factory
            },
            {
                Combine(498, 14),
                ChatPacket_V340_710Factory
            },
            {
                Combine(498, 18),
                TransactionPacket_V340_754Factory
            },
            {
                Combine(498, 19),
                CloseWindowPacket_V340_767Factory
            },
            {
                Combine(498, 20),
                WindowItemsPacket_V340_755Factory
            },
            {
                Combine(498, 21),
                CraftProgressBarPacket_V340_767Factory
            },
            {
                Combine(498, 22),
                SetSlotPacket_V340_755Factory
            },
            {
                Combine(498, 23),
                SetCooldownPacket_V340_767Factory
            },
            {
                Combine(498, 24),
                CustomPayloadPacket_V340_769Factory
            },
            {
                Combine(498, 25),
                NamedSoundEffectPacket_V340_758Factory
            },
            {
                Combine(498, 26),
                KickDisconnectPacket_V340_764Factory
            },
            {
                Combine(498, 27),
                EntityStatusPacket_V340_769Factory
            },
            {
                Combine(498, 29),
                UnloadChunkPacket_V340_763Factory
            },
            {
                Combine(498, 30),
                GameStateChangePacket_V340_769Factory
            },
            {
                Combine(498, 32),
                KeepAlivePacket_V340_769Factory
            },
            {
                Combine(498, 34),
                WorldEventPacket_V340_769Factory
            },
            {
                Combine(498, 40),
                RelEntityMovePacket_V340_769Factory
            },
            {
                Combine(498, 41),
                EntityMoveLookPacket_V340_769Factory
            },
            {
                Combine(498, 42),
                EntityLookPacket_V340_769Factory
            },
            {
                Combine(498, 43),
                EntityPacket_V340_754Factory
            },
            {
                Combine(498, 44),
                VehicleMovePacket_V340_769Factory
            },
            {
                Combine(498, 47),
                OpenSignEntityPacket_V340_762Factory
            },
            {
                Combine(498, 49),
                AbilitiesPacket_V340_769Factory
            },
            {
                Combine(498, 53),
                PositionPacket_V340_754Factory
            },
            {
                Combine(498, 55),
                EntityDestroyPacket_V340_754Factory
            },
            {
                Combine(498, 56),
                RemoveEntityEffectPacket_V340_757Factory
            },
            {
                Combine(498, 57),
                ResourcePackSendPacket_V340_754Factory
            },
            {
                Combine(498, 59),
                EntityHeadRotationPacket_V340_769Factory
            },
            {
                Combine(498, 62),
                CameraPacket_V340_769Factory
            },
            {
                Combine(498, 63),
                HeldItemSlotPacket_V340_768Factory
            },
            {
                Combine(498, 66),
                ScoreboardDisplayObjectivePacket_V340_763Factory
            },
            {
                Combine(498, 68),
                AttachEntityPacket_V340_769Factory
            },
            {
                Combine(498, 69),
                EntityVelocityPacket_V340_769Factory
            },
            {
                Combine(498, 71),
                ExperiencePacket_V340_760Factory
            },
            {
                Combine(498, 72),
                UpdateHealthPacket_V340_769Factory
            },
            {
                Combine(498, 74),
                SetPassengersPacket_V340_769Factory
            },
            {
                Combine(498, 77),
                SpawnPositionPacket_V340_754Factory
            },
            {
                Combine(498, 78),
                UpdateTimePacket_V340_767Factory
            },
            {
                Combine(498, 83),
                PlayerlistHeaderPacket_V340_764Factory
            },
            {
                Combine(498, 85),
                CollectPacket_V340_769Factory
            },
            {
                Combine(498, 86),
                EntityTeleportPacket_V340_769Factory
            },
            {
                Combine(498, 89),
                EntityEffectPacket_V340_757Factory
            },
            {
                Combine(498, 60),
                SelectAdvancementTabPacket_V340_769Factory
            },
            {
                Combine(498, 84),
                NbtQueryResponsePacket_V393_763Factory
            },
            {
                Combine(498, 31),
                OpenHorseWindowPacket_V477_767Factory
            },
            {
                Combine(498, 36),
                UpdateLightPacket_V477_710Factory
            },
            {
                Combine(498, 45),
                OpenBookPacket_V477_769Factory
            },
            {
                Combine(498, 64),
                UpdateViewPositionPacket_V477_769Factory
            },
            {
                Combine(498, 65),
                UpdateViewDistancePacket_V477_769Factory
            },
            {
                Combine(498, 92),
                AcknowledgePlayerDiggingPacket_V498_758Factory
            },
            {
                Combine(573, 0),
                SpawnEntityPacket_V477_758Factory
            },
            {
                Combine(573, 1),
                SpawnEntityExperienceOrbPacket_V340_769Factory
            },
            {
                Combine(573, 2),
                SpawnEntityWeatherPacket_V340_710Factory
            },
            {
                Combine(573, 4),
                SpawnEntityPaintingPacket_V393_758Factory
            },
            {
                Combine(573, 6),
                AnimationPacket_V340_769Factory
            },
            {
                Combine(573, 9),
                BlockBreakAnimationPacket_V340_769Factory
            },
            {
                Combine(573, 10),
                TileEntityDataPacket_V340_756Factory
            },
            {
                Combine(573, 11),
                BlockActionPacket_V340_769Factory
            },
            {
                Combine(573, 12),
                BlockChangePacket_V340_769Factory
            },
            {
                Combine(573, 14),
                DifficultyPacket_V477_769Factory
            },
            {
                Combine(573, 15),
                ChatPacket_V340_710Factory
            },
            {
                Combine(573, 19),
                TransactionPacket_V340_754Factory
            },
            {
                Combine(573, 20),
                CloseWindowPacket_V340_767Factory
            },
            {
                Combine(573, 21),
                WindowItemsPacket_V340_755Factory
            },
            {
                Combine(573, 22),
                CraftProgressBarPacket_V340_767Factory
            },
            {
                Combine(573, 23),
                SetSlotPacket_V340_755Factory
            },
            {
                Combine(573, 24),
                SetCooldownPacket_V340_767Factory
            },
            {
                Combine(573, 25),
                CustomPayloadPacket_V340_769Factory
            },
            {
                Combine(573, 26),
                NamedSoundEffectPacket_V340_758Factory
            },
            {
                Combine(573, 27),
                KickDisconnectPacket_V340_764Factory
            },
            {
                Combine(573, 28),
                EntityStatusPacket_V340_769Factory
            },
            {
                Combine(573, 30),
                UnloadChunkPacket_V340_763Factory
            },
            {
                Combine(573, 31),
                GameStateChangePacket_V340_769Factory
            },
            {
                Combine(573, 33),
                KeepAlivePacket_V340_769Factory
            },
            {
                Combine(573, 35),
                WorldEventPacket_V340_769Factory
            },
            {
                Combine(573, 41),
                RelEntityMovePacket_V340_769Factory
            },
            {
                Combine(573, 42),
                EntityMoveLookPacket_V340_769Factory
            },
            {
                Combine(573, 43),
                EntityLookPacket_V340_769Factory
            },
            {
                Combine(573, 44),
                EntityPacket_V340_754Factory
            },
            {
                Combine(573, 45),
                VehicleMovePacket_V340_769Factory
            },
            {
                Combine(573, 48),
                OpenSignEntityPacket_V340_762Factory
            },
            {
                Combine(573, 50),
                AbilitiesPacket_V340_769Factory
            },
            {
                Combine(573, 54),
                PositionPacket_V340_754Factory
            },
            {
                Combine(573, 56),
                EntityDestroyPacket_V340_754Factory
            },
            {
                Combine(573, 57),
                RemoveEntityEffectPacket_V340_757Factory
            },
            {
                Combine(573, 58),
                ResourcePackSendPacket_V340_754Factory
            },
            {
                Combine(573, 60),
                EntityHeadRotationPacket_V340_769Factory
            },
            {
                Combine(573, 63),
                CameraPacket_V340_769Factory
            },
            {
                Combine(573, 64),
                HeldItemSlotPacket_V340_768Factory
            },
            {
                Combine(573, 67),
                ScoreboardDisplayObjectivePacket_V340_763Factory
            },
            {
                Combine(573, 69),
                AttachEntityPacket_V340_769Factory
            },
            {
                Combine(573, 70),
                EntityVelocityPacket_V340_769Factory
            },
            {
                Combine(573, 72),
                ExperiencePacket_V340_760Factory
            },
            {
                Combine(573, 73),
                UpdateHealthPacket_V340_769Factory
            },
            {
                Combine(573, 75),
                SetPassengersPacket_V340_769Factory
            },
            {
                Combine(573, 78),
                SpawnPositionPacket_V340_754Factory
            },
            {
                Combine(573, 79),
                UpdateTimePacket_V340_767Factory
            },
            {
                Combine(573, 84),
                PlayerlistHeaderPacket_V340_764Factory
            },
            {
                Combine(573, 86),
                CollectPacket_V340_769Factory
            },
            {
                Combine(573, 87),
                EntityTeleportPacket_V340_769Factory
            },
            {
                Combine(573, 90),
                EntityEffectPacket_V340_757Factory
            },
            {
                Combine(573, 61),
                SelectAdvancementTabPacket_V340_769Factory
            },
            {
                Combine(573, 85),
                NbtQueryResponsePacket_V393_763Factory
            },
            {
                Combine(573, 32),
                OpenHorseWindowPacket_V477_767Factory
            },
            {
                Combine(573, 37),
                UpdateLightPacket_V477_710Factory
            },
            {
                Combine(573, 46),
                OpenBookPacket_V477_769Factory
            },
            {
                Combine(573, 65),
                UpdateViewPositionPacket_V477_769Factory
            },
            {
                Combine(573, 66),
                UpdateViewDistancePacket_V477_769Factory
            },
            {
                Combine(573, 8),
                AcknowledgePlayerDiggingPacket_V498_758Factory
            },
            {
                Combine(575, 0),
                SpawnEntityPacket_V477_758Factory
            },
            {
                Combine(575, 1),
                SpawnEntityExperienceOrbPacket_V340_769Factory
            },
            {
                Combine(575, 2),
                SpawnEntityWeatherPacket_V340_710Factory
            },
            {
                Combine(575, 4),
                SpawnEntityPaintingPacket_V393_758Factory
            },
            {
                Combine(575, 6),
                AnimationPacket_V340_769Factory
            },
            {
                Combine(575, 9),
                BlockBreakAnimationPacket_V340_769Factory
            },
            {
                Combine(575, 10),
                TileEntityDataPacket_V340_756Factory
            },
            {
                Combine(575, 11),
                BlockActionPacket_V340_769Factory
            },
            {
                Combine(575, 12),
                BlockChangePacket_V340_769Factory
            },
            {
                Combine(575, 14),
                DifficultyPacket_V477_769Factory
            },
            {
                Combine(575, 15),
                ChatPacket_V340_710Factory
            },
            {
                Combine(575, 19),
                TransactionPacket_V340_754Factory
            },
            {
                Combine(575, 20),
                CloseWindowPacket_V340_767Factory
            },
            {
                Combine(575, 21),
                WindowItemsPacket_V340_755Factory
            },
            {
                Combine(575, 22),
                CraftProgressBarPacket_V340_767Factory
            },
            {
                Combine(575, 23),
                SetSlotPacket_V340_755Factory
            },
            {
                Combine(575, 24),
                SetCooldownPacket_V340_767Factory
            },
            {
                Combine(575, 25),
                CustomPayloadPacket_V340_769Factory
            },
            {
                Combine(575, 26),
                NamedSoundEffectPacket_V340_758Factory
            },
            {
                Combine(575, 27),
                KickDisconnectPacket_V340_764Factory
            },
            {
                Combine(575, 28),
                EntityStatusPacket_V340_769Factory
            },
            {
                Combine(575, 30),
                UnloadChunkPacket_V340_763Factory
            },
            {
                Combine(575, 31),
                GameStateChangePacket_V340_769Factory
            },
            {
                Combine(575, 33),
                KeepAlivePacket_V340_769Factory
            },
            {
                Combine(575, 35),
                WorldEventPacket_V340_769Factory
            },
            {
                Combine(575, 41),
                RelEntityMovePacket_V340_769Factory
            },
            {
                Combine(575, 42),
                EntityMoveLookPacket_V340_769Factory
            },
            {
                Combine(575, 43),
                EntityLookPacket_V340_769Factory
            },
            {
                Combine(575, 44),
                EntityPacket_V340_754Factory
            },
            {
                Combine(575, 45),
                VehicleMovePacket_V340_769Factory
            },
            {
                Combine(575, 48),
                OpenSignEntityPacket_V340_762Factory
            },
            {
                Combine(575, 50),
                AbilitiesPacket_V340_769Factory
            },
            {
                Combine(575, 54),
                PositionPacket_V340_754Factory
            },
            {
                Combine(575, 56),
                EntityDestroyPacket_V340_754Factory
            },
            {
                Combine(575, 57),
                RemoveEntityEffectPacket_V340_757Factory
            },
            {
                Combine(575, 58),
                ResourcePackSendPacket_V340_754Factory
            },
            {
                Combine(575, 60),
                EntityHeadRotationPacket_V340_769Factory
            },
            {
                Combine(575, 63),
                CameraPacket_V340_769Factory
            },
            {
                Combine(575, 64),
                HeldItemSlotPacket_V340_768Factory
            },
            {
                Combine(575, 67),
                ScoreboardDisplayObjectivePacket_V340_763Factory
            },
            {
                Combine(575, 69),
                AttachEntityPacket_V340_769Factory
            },
            {
                Combine(575, 70),
                EntityVelocityPacket_V340_769Factory
            },
            {
                Combine(575, 72),
                ExperiencePacket_V340_760Factory
            },
            {
                Combine(575, 73),
                UpdateHealthPacket_V340_769Factory
            },
            {
                Combine(575, 75),
                SetPassengersPacket_V340_769Factory
            },
            {
                Combine(575, 78),
                SpawnPositionPacket_V340_754Factory
            },
            {
                Combine(575, 79),
                UpdateTimePacket_V340_767Factory
            },
            {
                Combine(575, 84),
                PlayerlistHeaderPacket_V340_764Factory
            },
            {
                Combine(575, 86),
                CollectPacket_V340_769Factory
            },
            {
                Combine(575, 87),
                EntityTeleportPacket_V340_769Factory
            },
            {
                Combine(575, 90),
                EntityEffectPacket_V340_757Factory
            },
            {
                Combine(575, 61),
                SelectAdvancementTabPacket_V340_769Factory
            },
            {
                Combine(575, 85),
                NbtQueryResponsePacket_V393_763Factory
            },
            {
                Combine(575, 32),
                OpenHorseWindowPacket_V477_767Factory
            },
            {
                Combine(575, 37),
                UpdateLightPacket_V477_710Factory
            },
            {
                Combine(575, 46),
                OpenBookPacket_V477_769Factory
            },
            {
                Combine(575, 65),
                UpdateViewPositionPacket_V477_769Factory
            },
            {
                Combine(575, 66),
                UpdateViewDistancePacket_V477_769Factory
            },
            {
                Combine(575, 8),
                AcknowledgePlayerDiggingPacket_V498_758Factory
            },
            {
                Combine(578, 0),
                SpawnEntityPacket_V477_758Factory
            },
            {
                Combine(578, 1),
                SpawnEntityExperienceOrbPacket_V340_769Factory
            },
            {
                Combine(578, 2),
                SpawnEntityWeatherPacket_V340_710Factory
            },
            {
                Combine(578, 4),
                SpawnEntityPaintingPacket_V393_758Factory
            },
            {
                Combine(578, 6),
                AnimationPacket_V340_769Factory
            },
            {
                Combine(578, 9),
                BlockBreakAnimationPacket_V340_769Factory
            },
            {
                Combine(578, 10),
                TileEntityDataPacket_V340_756Factory
            },
            {
                Combine(578, 11),
                BlockActionPacket_V340_769Factory
            },
            {
                Combine(578, 12),
                BlockChangePacket_V340_769Factory
            },
            {
                Combine(578, 14),
                DifficultyPacket_V477_769Factory
            },
            {
                Combine(578, 15),
                ChatPacket_V340_710Factory
            },
            {
                Combine(578, 19),
                TransactionPacket_V340_754Factory
            },
            {
                Combine(578, 20),
                CloseWindowPacket_V340_767Factory
            },
            {
                Combine(578, 21),
                WindowItemsPacket_V340_755Factory
            },
            {
                Combine(578, 22),
                CraftProgressBarPacket_V340_767Factory
            },
            {
                Combine(578, 23),
                SetSlotPacket_V340_755Factory
            },
            {
                Combine(578, 24),
                SetCooldownPacket_V340_767Factory
            },
            {
                Combine(578, 25),
                CustomPayloadPacket_V340_769Factory
            },
            {
                Combine(578, 26),
                NamedSoundEffectPacket_V340_758Factory
            },
            {
                Combine(578, 27),
                KickDisconnectPacket_V340_764Factory
            },
            {
                Combine(578, 28),
                EntityStatusPacket_V340_769Factory
            },
            {
                Combine(578, 30),
                UnloadChunkPacket_V340_763Factory
            },
            {
                Combine(578, 31),
                GameStateChangePacket_V340_769Factory
            },
            {
                Combine(578, 33),
                KeepAlivePacket_V340_769Factory
            },
            {
                Combine(578, 35),
                WorldEventPacket_V340_769Factory
            },
            {
                Combine(578, 41),
                RelEntityMovePacket_V340_769Factory
            },
            {
                Combine(578, 42),
                EntityMoveLookPacket_V340_769Factory
            },
            {
                Combine(578, 43),
                EntityLookPacket_V340_769Factory
            },
            {
                Combine(578, 44),
                EntityPacket_V340_754Factory
            },
            {
                Combine(578, 45),
                VehicleMovePacket_V340_769Factory
            },
            {
                Combine(578, 48),
                OpenSignEntityPacket_V340_762Factory
            },
            {
                Combine(578, 50),
                AbilitiesPacket_V340_769Factory
            },
            {
                Combine(578, 54),
                PositionPacket_V340_754Factory
            },
            {
                Combine(578, 56),
                EntityDestroyPacket_V340_754Factory
            },
            {
                Combine(578, 57),
                RemoveEntityEffectPacket_V340_757Factory
            },
            {
                Combine(578, 58),
                ResourcePackSendPacket_V340_754Factory
            },
            {
                Combine(578, 60),
                EntityHeadRotationPacket_V340_769Factory
            },
            {
                Combine(578, 63),
                CameraPacket_V340_769Factory
            },
            {
                Combine(578, 64),
                HeldItemSlotPacket_V340_768Factory
            },
            {
                Combine(578, 67),
                ScoreboardDisplayObjectivePacket_V340_763Factory
            },
            {
                Combine(578, 69),
                AttachEntityPacket_V340_769Factory
            },
            {
                Combine(578, 70),
                EntityVelocityPacket_V340_769Factory
            },
            {
                Combine(578, 72),
                ExperiencePacket_V340_760Factory
            },
            {
                Combine(578, 73),
                UpdateHealthPacket_V340_769Factory
            },
            {
                Combine(578, 75),
                SetPassengersPacket_V340_769Factory
            },
            {
                Combine(578, 78),
                SpawnPositionPacket_V340_754Factory
            },
            {
                Combine(578, 79),
                UpdateTimePacket_V340_767Factory
            },
            {
                Combine(578, 84),
                PlayerlistHeaderPacket_V340_764Factory
            },
            {
                Combine(578, 86),
                CollectPacket_V340_769Factory
            },
            {
                Combine(578, 87),
                EntityTeleportPacket_V340_769Factory
            },
            {
                Combine(578, 90),
                EntityEffectPacket_V340_757Factory
            },
            {
                Combine(578, 61),
                SelectAdvancementTabPacket_V340_769Factory
            },
            {
                Combine(578, 85),
                NbtQueryResponsePacket_V393_763Factory
            },
            {
                Combine(578, 32),
                OpenHorseWindowPacket_V477_767Factory
            },
            {
                Combine(578, 37),
                UpdateLightPacket_V477_710Factory
            },
            {
                Combine(578, 46),
                OpenBookPacket_V477_769Factory
            },
            {
                Combine(578, 65),
                UpdateViewPositionPacket_V477_769Factory
            },
            {
                Combine(578, 66),
                UpdateViewDistancePacket_V477_769Factory
            },
            {
                Combine(578, 8),
                AcknowledgePlayerDiggingPacket_V498_758Factory
            },
            {
                Combine(709, 0),
                SpawnEntityPacket_V477_758Factory
            },
            {
                Combine(709, 1),
                SpawnEntityExperienceOrbPacket_V340_769Factory
            },
            {
                Combine(709, 2),
                SpawnEntityWeatherPacket_V340_710Factory
            },
            {
                Combine(709, 4),
                SpawnEntityPaintingPacket_V393_758Factory
            },
            {
                Combine(709, 6),
                AnimationPacket_V340_769Factory
            },
            {
                Combine(709, 9),
                BlockBreakAnimationPacket_V340_769Factory
            },
            {
                Combine(709, 10),
                TileEntityDataPacket_V340_756Factory
            },
            {
                Combine(709, 11),
                BlockActionPacket_V340_769Factory
            },
            {
                Combine(709, 12),
                BlockChangePacket_V340_769Factory
            },
            {
                Combine(709, 14),
                DifficultyPacket_V477_769Factory
            },
            {
                Combine(709, 15),
                ChatPacket_V340_710Factory
            },
            {
                Combine(709, 19),
                TransactionPacket_V340_754Factory
            },
            {
                Combine(709, 20),
                CloseWindowPacket_V340_767Factory
            },
            {
                Combine(709, 21),
                WindowItemsPacket_V340_755Factory
            },
            {
                Combine(709, 22),
                CraftProgressBarPacket_V340_767Factory
            },
            {
                Combine(709, 23),
                SetSlotPacket_V340_755Factory
            },
            {
                Combine(709, 24),
                SetCooldownPacket_V340_767Factory
            },
            {
                Combine(709, 25),
                CustomPayloadPacket_V340_769Factory
            },
            {
                Combine(709, 26),
                NamedSoundEffectPacket_V340_758Factory
            },
            {
                Combine(709, 27),
                KickDisconnectPacket_V340_764Factory
            },
            {
                Combine(709, 28),
                EntityStatusPacket_V340_769Factory
            },
            {
                Combine(709, 30),
                UnloadChunkPacket_V340_763Factory
            },
            {
                Combine(709, 31),
                GameStateChangePacket_V340_769Factory
            },
            {
                Combine(709, 33),
                KeepAlivePacket_V340_769Factory
            },
            {
                Combine(709, 35),
                WorldEventPacket_V340_769Factory
            },
            {
                Combine(709, 41),
                RelEntityMovePacket_V340_769Factory
            },
            {
                Combine(709, 42),
                EntityMoveLookPacket_V340_769Factory
            },
            {
                Combine(709, 43),
                EntityLookPacket_V340_769Factory
            },
            {
                Combine(709, 44),
                EntityPacket_V340_754Factory
            },
            {
                Combine(709, 45),
                VehicleMovePacket_V340_769Factory
            },
            {
                Combine(709, 48),
                OpenSignEntityPacket_V340_762Factory
            },
            {
                Combine(709, 50),
                AbilitiesPacket_V340_769Factory
            },
            {
                Combine(709, 54),
                PositionPacket_V340_754Factory
            },
            {
                Combine(709, 56),
                EntityDestroyPacket_V340_754Factory
            },
            {
                Combine(709, 57),
                RemoveEntityEffectPacket_V340_757Factory
            },
            {
                Combine(709, 58),
                ResourcePackSendPacket_V340_754Factory
            },
            {
                Combine(709, 60),
                EntityHeadRotationPacket_V340_769Factory
            },
            {
                Combine(709, 63),
                CameraPacket_V340_769Factory
            },
            {
                Combine(709, 64),
                HeldItemSlotPacket_V340_768Factory
            },
            {
                Combine(709, 68),
                ScoreboardDisplayObjectivePacket_V340_763Factory
            },
            {
                Combine(709, 70),
                AttachEntityPacket_V340_769Factory
            },
            {
                Combine(709, 71),
                EntityVelocityPacket_V340_769Factory
            },
            {
                Combine(709, 73),
                ExperiencePacket_V340_760Factory
            },
            {
                Combine(709, 74),
                UpdateHealthPacket_V340_769Factory
            },
            {
                Combine(709, 76),
                SetPassengersPacket_V340_769Factory
            },
            {
                Combine(709, 67),
                SpawnPositionPacket_V340_754Factory
            },
            {
                Combine(709, 79),
                UpdateTimePacket_V340_767Factory
            },
            {
                Combine(709, 84),
                PlayerlistHeaderPacket_V340_764Factory
            },
            {
                Combine(709, 86),
                CollectPacket_V340_769Factory
            },
            {
                Combine(709, 87),
                EntityTeleportPacket_V340_769Factory
            },
            {
                Combine(709, 90),
                EntityEffectPacket_V340_757Factory
            },
            {
                Combine(709, 61),
                SelectAdvancementTabPacket_V340_769Factory
            },
            {
                Combine(709, 85),
                NbtQueryResponsePacket_V393_763Factory
            },
            {
                Combine(709, 32),
                OpenHorseWindowPacket_V477_767Factory
            },
            {
                Combine(709, 37),
                UpdateLightPacket_V477_710Factory
            },
            {
                Combine(709, 46),
                OpenBookPacket_V477_769Factory
            },
            {
                Combine(709, 65),
                UpdateViewPositionPacket_V477_769Factory
            },
            {
                Combine(709, 66),
                UpdateViewDistancePacket_V477_769Factory
            },
            {
                Combine(709, 8),
                AcknowledgePlayerDiggingPacket_V498_758Factory
            },
            {
                Combine(710, 0),
                SpawnEntityPacket_V477_758Factory
            },
            {
                Combine(710, 1),
                SpawnEntityExperienceOrbPacket_V340_769Factory
            },
            {
                Combine(710, 2),
                SpawnEntityWeatherPacket_V340_710Factory
            },
            {
                Combine(710, 4),
                SpawnEntityPaintingPacket_V393_758Factory
            },
            {
                Combine(710, 6),
                AnimationPacket_V340_769Factory
            },
            {
                Combine(710, 9),
                BlockBreakAnimationPacket_V340_769Factory
            },
            {
                Combine(710, 10),
                TileEntityDataPacket_V340_756Factory
            },
            {
                Combine(710, 11),
                BlockActionPacket_V340_769Factory
            },
            {
                Combine(710, 12),
                BlockChangePacket_V340_769Factory
            },
            {
                Combine(710, 14),
                DifficultyPacket_V477_769Factory
            },
            {
                Combine(710, 15),
                ChatPacket_V340_710Factory
            },
            {
                Combine(710, 19),
                TransactionPacket_V340_754Factory
            },
            {
                Combine(710, 20),
                CloseWindowPacket_V340_767Factory
            },
            {
                Combine(710, 21),
                WindowItemsPacket_V340_755Factory
            },
            {
                Combine(710, 22),
                CraftProgressBarPacket_V340_767Factory
            },
            {
                Combine(710, 23),
                SetSlotPacket_V340_755Factory
            },
            {
                Combine(710, 24),
                SetCooldownPacket_V340_767Factory
            },
            {
                Combine(710, 25),
                CustomPayloadPacket_V340_769Factory
            },
            {
                Combine(710, 26),
                NamedSoundEffectPacket_V340_758Factory
            },
            {
                Combine(710, 27),
                KickDisconnectPacket_V340_764Factory
            },
            {
                Combine(710, 28),
                EntityStatusPacket_V340_769Factory
            },
            {
                Combine(710, 30),
                UnloadChunkPacket_V340_763Factory
            },
            {
                Combine(710, 31),
                GameStateChangePacket_V340_769Factory
            },
            {
                Combine(710, 33),
                KeepAlivePacket_V340_769Factory
            },
            {
                Combine(710, 35),
                WorldEventPacket_V340_769Factory
            },
            {
                Combine(710, 41),
                RelEntityMovePacket_V340_769Factory
            },
            {
                Combine(710, 42),
                EntityMoveLookPacket_V340_769Factory
            },
            {
                Combine(710, 43),
                EntityLookPacket_V340_769Factory
            },
            {
                Combine(710, 44),
                EntityPacket_V340_754Factory
            },
            {
                Combine(710, 45),
                VehicleMovePacket_V340_769Factory
            },
            {
                Combine(710, 48),
                OpenSignEntityPacket_V340_762Factory
            },
            {
                Combine(710, 50),
                AbilitiesPacket_V340_769Factory
            },
            {
                Combine(710, 54),
                PositionPacket_V340_754Factory
            },
            {
                Combine(710, 56),
                EntityDestroyPacket_V340_754Factory
            },
            {
                Combine(710, 57),
                RemoveEntityEffectPacket_V340_757Factory
            },
            {
                Combine(710, 58),
                ResourcePackSendPacket_V340_754Factory
            },
            {
                Combine(710, 60),
                EntityHeadRotationPacket_V340_769Factory
            },
            {
                Combine(710, 63),
                CameraPacket_V340_769Factory
            },
            {
                Combine(710, 64),
                HeldItemSlotPacket_V340_768Factory
            },
            {
                Combine(710, 68),
                ScoreboardDisplayObjectivePacket_V340_763Factory
            },
            {
                Combine(710, 70),
                AttachEntityPacket_V340_769Factory
            },
            {
                Combine(710, 71),
                EntityVelocityPacket_V340_769Factory
            },
            {
                Combine(710, 73),
                ExperiencePacket_V340_760Factory
            },
            {
                Combine(710, 74),
                UpdateHealthPacket_V340_769Factory
            },
            {
                Combine(710, 76),
                SetPassengersPacket_V340_769Factory
            },
            {
                Combine(710, 67),
                SpawnPositionPacket_V340_754Factory
            },
            {
                Combine(710, 79),
                UpdateTimePacket_V340_767Factory
            },
            {
                Combine(710, 84),
                PlayerlistHeaderPacket_V340_764Factory
            },
            {
                Combine(710, 86),
                CollectPacket_V340_769Factory
            },
            {
                Combine(710, 87),
                EntityTeleportPacket_V340_769Factory
            },
            {
                Combine(710, 90),
                EntityEffectPacket_V340_757Factory
            },
            {
                Combine(710, 61),
                SelectAdvancementTabPacket_V340_769Factory
            },
            {
                Combine(710, 85),
                NbtQueryResponsePacket_V393_763Factory
            },
            {
                Combine(710, 32),
                OpenHorseWindowPacket_V477_767Factory
            },
            {
                Combine(710, 37),
                UpdateLightPacket_V477_710Factory
            },
            {
                Combine(710, 46),
                OpenBookPacket_V477_769Factory
            },
            {
                Combine(710, 65),
                UpdateViewPositionPacket_V477_769Factory
            },
            {
                Combine(710, 66),
                UpdateViewDistancePacket_V477_769Factory
            },
            {
                Combine(710, 8),
                AcknowledgePlayerDiggingPacket_V498_758Factory
            },
            {
                Combine(734, 0),
                SpawnEntityPacket_V477_758Factory
            },
            {
                Combine(734, 1),
                SpawnEntityExperienceOrbPacket_V340_769Factory
            },
            {
                Combine(734, 3),
                SpawnEntityPaintingPacket_V393_758Factory
            },
            {
                Combine(734, 5),
                AnimationPacket_V340_769Factory
            },
            {
                Combine(734, 8),
                BlockBreakAnimationPacket_V340_769Factory
            },
            {
                Combine(734, 9),
                TileEntityDataPacket_V340_756Factory
            },
            {
                Combine(734, 10),
                BlockActionPacket_V340_769Factory
            },
            {
                Combine(734, 11),
                BlockChangePacket_V340_769Factory
            },
            {
                Combine(734, 13),
                DifficultyPacket_V477_769Factory
            },
            {
                Combine(734, 14),
                ChatPacket_V734_758Factory
            },
            {
                Combine(734, 18),
                TransactionPacket_V340_754Factory
            },
            {
                Combine(734, 19),
                CloseWindowPacket_V340_767Factory
            },
            {
                Combine(734, 20),
                WindowItemsPacket_V340_755Factory
            },
            {
                Combine(734, 21),
                CraftProgressBarPacket_V340_767Factory
            },
            {
                Combine(734, 22),
                SetSlotPacket_V340_755Factory
            },
            {
                Combine(734, 23),
                SetCooldownPacket_V340_767Factory
            },
            {
                Combine(734, 24),
                CustomPayloadPacket_V340_769Factory
            },
            {
                Combine(734, 25),
                NamedSoundEffectPacket_V340_758Factory
            },
            {
                Combine(734, 26),
                KickDisconnectPacket_V340_764Factory
            },
            {
                Combine(734, 27),
                EntityStatusPacket_V340_769Factory
            },
            {
                Combine(734, 29),
                UnloadChunkPacket_V340_763Factory
            },
            {
                Combine(734, 30),
                GameStateChangePacket_V340_769Factory
            },
            {
                Combine(734, 32),
                KeepAlivePacket_V340_769Factory
            },
            {
                Combine(734, 34),
                WorldEventPacket_V340_769Factory
            },
            {
                Combine(734, 40),
                RelEntityMovePacket_V340_769Factory
            },
            {
                Combine(734, 41),
                EntityMoveLookPacket_V340_769Factory
            },
            {
                Combine(734, 42),
                EntityLookPacket_V340_769Factory
            },
            {
                Combine(734, 43),
                EntityPacket_V340_754Factory
            },
            {
                Combine(734, 44),
                VehicleMovePacket_V340_769Factory
            },
            {
                Combine(734, 47),
                OpenSignEntityPacket_V340_762Factory
            },
            {
                Combine(734, 49),
                AbilitiesPacket_V340_769Factory
            },
            {
                Combine(734, 53),
                PositionPacket_V340_754Factory
            },
            {
                Combine(734, 55),
                EntityDestroyPacket_V340_754Factory
            },
            {
                Combine(734, 56),
                RemoveEntityEffectPacket_V340_757Factory
            },
            {
                Combine(734, 57),
                ResourcePackSendPacket_V340_754Factory
            },
            {
                Combine(734, 59),
                EntityHeadRotationPacket_V340_769Factory
            },
            {
                Combine(734, 62),
                CameraPacket_V340_769Factory
            },
            {
                Combine(734, 63),
                HeldItemSlotPacket_V340_768Factory
            },
            {
                Combine(734, 67),
                ScoreboardDisplayObjectivePacket_V340_763Factory
            },
            {
                Combine(734, 69),
                AttachEntityPacket_V340_769Factory
            },
            {
                Combine(734, 70),
                EntityVelocityPacket_V340_769Factory
            },
            {
                Combine(734, 72),
                ExperiencePacket_V340_760Factory
            },
            {
                Combine(734, 73),
                UpdateHealthPacket_V340_769Factory
            },
            {
                Combine(734, 75),
                SetPassengersPacket_V340_769Factory
            },
            {
                Combine(734, 66),
                SpawnPositionPacket_V340_754Factory
            },
            {
                Combine(734, 78),
                UpdateTimePacket_V340_767Factory
            },
            {
                Combine(734, 83),
                PlayerlistHeaderPacket_V340_764Factory
            },
            {
                Combine(734, 85),
                CollectPacket_V340_769Factory
            },
            {
                Combine(734, 86),
                EntityTeleportPacket_V340_769Factory
            },
            {
                Combine(734, 89),
                EntityEffectPacket_V340_757Factory
            },
            {
                Combine(734, 60),
                SelectAdvancementTabPacket_V340_769Factory
            },
            {
                Combine(734, 84),
                NbtQueryResponsePacket_V393_763Factory
            },
            {
                Combine(734, 31),
                OpenHorseWindowPacket_V477_767Factory
            },
            {
                Combine(734, 36),
                UpdateLightPacket_V734_754Factory
            },
            {
                Combine(734, 45),
                OpenBookPacket_V477_769Factory
            },
            {
                Combine(734, 64),
                UpdateViewPositionPacket_V477_769Factory
            },
            {
                Combine(734, 65),
                UpdateViewDistancePacket_V477_769Factory
            },
            {
                Combine(734, 7),
                AcknowledgePlayerDiggingPacket_V498_758Factory
            },
            {
                Combine(735, 0),
                SpawnEntityPacket_V477_758Factory
            },
            {
                Combine(735, 1),
                SpawnEntityExperienceOrbPacket_V340_769Factory
            },
            {
                Combine(735, 3),
                SpawnEntityPaintingPacket_V393_758Factory
            },
            {
                Combine(735, 5),
                AnimationPacket_V340_769Factory
            },
            {
                Combine(735, 8),
                BlockBreakAnimationPacket_V340_769Factory
            },
            {
                Combine(735, 9),
                TileEntityDataPacket_V340_756Factory
            },
            {
                Combine(735, 10),
                BlockActionPacket_V340_769Factory
            },
            {
                Combine(735, 11),
                BlockChangePacket_V340_769Factory
            },
            {
                Combine(735, 13),
                DifficultyPacket_V477_769Factory
            },
            {
                Combine(735, 14),
                ChatPacket_V734_758Factory
            },
            {
                Combine(735, 18),
                TransactionPacket_V340_754Factory
            },
            {
                Combine(735, 19),
                CloseWindowPacket_V340_767Factory
            },
            {
                Combine(735, 20),
                WindowItemsPacket_V340_755Factory
            },
            {
                Combine(735, 21),
                CraftProgressBarPacket_V340_767Factory
            },
            {
                Combine(735, 22),
                SetSlotPacket_V340_755Factory
            },
            {
                Combine(735, 23),
                SetCooldownPacket_V340_767Factory
            },
            {
                Combine(735, 24),
                CustomPayloadPacket_V340_769Factory
            },
            {
                Combine(735, 25),
                NamedSoundEffectPacket_V340_758Factory
            },
            {
                Combine(735, 26),
                KickDisconnectPacket_V340_764Factory
            },
            {
                Combine(735, 27),
                EntityStatusPacket_V340_769Factory
            },
            {
                Combine(735, 29),
                UnloadChunkPacket_V340_763Factory
            },
            {
                Combine(735, 30),
                GameStateChangePacket_V340_769Factory
            },
            {
                Combine(735, 32),
                KeepAlivePacket_V340_769Factory
            },
            {
                Combine(735, 34),
                WorldEventPacket_V340_769Factory
            },
            {
                Combine(735, 40),
                RelEntityMovePacket_V340_769Factory
            },
            {
                Combine(735, 41),
                EntityMoveLookPacket_V340_769Factory
            },
            {
                Combine(735, 42),
                EntityLookPacket_V340_769Factory
            },
            {
                Combine(735, 43),
                EntityPacket_V340_754Factory
            },
            {
                Combine(735, 44),
                VehicleMovePacket_V340_769Factory
            },
            {
                Combine(735, 47),
                OpenSignEntityPacket_V340_762Factory
            },
            {
                Combine(735, 49),
                AbilitiesPacket_V340_769Factory
            },
            {
                Combine(735, 53),
                PositionPacket_V340_754Factory
            },
            {
                Combine(735, 55),
                EntityDestroyPacket_V340_754Factory
            },
            {
                Combine(735, 56),
                RemoveEntityEffectPacket_V340_757Factory
            },
            {
                Combine(735, 57),
                ResourcePackSendPacket_V340_754Factory
            },
            {
                Combine(735, 59),
                EntityHeadRotationPacket_V340_769Factory
            },
            {
                Combine(735, 62),
                CameraPacket_V340_769Factory
            },
            {
                Combine(735, 63),
                HeldItemSlotPacket_V340_768Factory
            },
            {
                Combine(735, 67),
                ScoreboardDisplayObjectivePacket_V340_763Factory
            },
            {
                Combine(735, 69),
                AttachEntityPacket_V340_769Factory
            },
            {
                Combine(735, 70),
                EntityVelocityPacket_V340_769Factory
            },
            {
                Combine(735, 72),
                ExperiencePacket_V340_760Factory
            },
            {
                Combine(735, 73),
                UpdateHealthPacket_V340_769Factory
            },
            {
                Combine(735, 75),
                SetPassengersPacket_V340_769Factory
            },
            {
                Combine(735, 66),
                SpawnPositionPacket_V340_754Factory
            },
            {
                Combine(735, 78),
                UpdateTimePacket_V340_767Factory
            },
            {
                Combine(735, 83),
                PlayerlistHeaderPacket_V340_764Factory
            },
            {
                Combine(735, 85),
                CollectPacket_V340_769Factory
            },
            {
                Combine(735, 86),
                EntityTeleportPacket_V340_769Factory
            },
            {
                Combine(735, 89),
                EntityEffectPacket_V340_757Factory
            },
            {
                Combine(735, 60),
                SelectAdvancementTabPacket_V340_769Factory
            },
            {
                Combine(735, 84),
                NbtQueryResponsePacket_V393_763Factory
            },
            {
                Combine(735, 31),
                OpenHorseWindowPacket_V477_767Factory
            },
            {
                Combine(735, 36),
                UpdateLightPacket_V734_754Factory
            },
            {
                Combine(735, 45),
                OpenBookPacket_V477_769Factory
            },
            {
                Combine(735, 64),
                UpdateViewPositionPacket_V477_769Factory
            },
            {
                Combine(735, 65),
                UpdateViewDistancePacket_V477_769Factory
            },
            {
                Combine(735, 7),
                AcknowledgePlayerDiggingPacket_V498_758Factory
            },
            {
                Combine(736, 0),
                SpawnEntityPacket_V477_758Factory
            },
            {
                Combine(736, 1),
                SpawnEntityExperienceOrbPacket_V340_769Factory
            },
            {
                Combine(736, 3),
                SpawnEntityPaintingPacket_V393_758Factory
            },
            {
                Combine(736, 5),
                AnimationPacket_V340_769Factory
            },
            {
                Combine(736, 8),
                BlockBreakAnimationPacket_V340_769Factory
            },
            {
                Combine(736, 9),
                TileEntityDataPacket_V340_756Factory
            },
            {
                Combine(736, 10),
                BlockActionPacket_V340_769Factory
            },
            {
                Combine(736, 11),
                BlockChangePacket_V340_769Factory
            },
            {
                Combine(736, 13),
                DifficultyPacket_V477_769Factory
            },
            {
                Combine(736, 14),
                ChatPacket_V734_758Factory
            },
            {
                Combine(736, 18),
                TransactionPacket_V340_754Factory
            },
            {
                Combine(736, 19),
                CloseWindowPacket_V340_767Factory
            },
            {
                Combine(736, 20),
                WindowItemsPacket_V340_755Factory
            },
            {
                Combine(736, 21),
                CraftProgressBarPacket_V340_767Factory
            },
            {
                Combine(736, 22),
                SetSlotPacket_V340_755Factory
            },
            {
                Combine(736, 23),
                SetCooldownPacket_V340_767Factory
            },
            {
                Combine(736, 24),
                CustomPayloadPacket_V340_769Factory
            },
            {
                Combine(736, 25),
                NamedSoundEffectPacket_V340_758Factory
            },
            {
                Combine(736, 26),
                KickDisconnectPacket_V340_764Factory
            },
            {
                Combine(736, 27),
                EntityStatusPacket_V340_769Factory
            },
            {
                Combine(736, 29),
                UnloadChunkPacket_V340_763Factory
            },
            {
                Combine(736, 30),
                GameStateChangePacket_V340_769Factory
            },
            {
                Combine(736, 32),
                KeepAlivePacket_V340_769Factory
            },
            {
                Combine(736, 34),
                WorldEventPacket_V340_769Factory
            },
            {
                Combine(736, 40),
                RelEntityMovePacket_V340_769Factory
            },
            {
                Combine(736, 41),
                EntityMoveLookPacket_V340_769Factory
            },
            {
                Combine(736, 42),
                EntityLookPacket_V340_769Factory
            },
            {
                Combine(736, 43),
                EntityPacket_V340_754Factory
            },
            {
                Combine(736, 44),
                VehicleMovePacket_V340_769Factory
            },
            {
                Combine(736, 47),
                OpenSignEntityPacket_V340_762Factory
            },
            {
                Combine(736, 49),
                AbilitiesPacket_V340_769Factory
            },
            {
                Combine(736, 53),
                PositionPacket_V340_754Factory
            },
            {
                Combine(736, 55),
                EntityDestroyPacket_V340_754Factory
            },
            {
                Combine(736, 56),
                RemoveEntityEffectPacket_V340_757Factory
            },
            {
                Combine(736, 57),
                ResourcePackSendPacket_V340_754Factory
            },
            {
                Combine(736, 59),
                EntityHeadRotationPacket_V340_769Factory
            },
            {
                Combine(736, 62),
                CameraPacket_V340_769Factory
            },
            {
                Combine(736, 63),
                HeldItemSlotPacket_V340_768Factory
            },
            {
                Combine(736, 67),
                ScoreboardDisplayObjectivePacket_V340_763Factory
            },
            {
                Combine(736, 69),
                AttachEntityPacket_V340_769Factory
            },
            {
                Combine(736, 70),
                EntityVelocityPacket_V340_769Factory
            },
            {
                Combine(736, 72),
                ExperiencePacket_V340_760Factory
            },
            {
                Combine(736, 73),
                UpdateHealthPacket_V340_769Factory
            },
            {
                Combine(736, 75),
                SetPassengersPacket_V340_769Factory
            },
            {
                Combine(736, 66),
                SpawnPositionPacket_V340_754Factory
            },
            {
                Combine(736, 78),
                UpdateTimePacket_V340_767Factory
            },
            {
                Combine(736, 83),
                PlayerlistHeaderPacket_V340_764Factory
            },
            {
                Combine(736, 85),
                CollectPacket_V340_769Factory
            },
            {
                Combine(736, 86),
                EntityTeleportPacket_V340_769Factory
            },
            {
                Combine(736, 89),
                EntityEffectPacket_V340_757Factory
            },
            {
                Combine(736, 60),
                SelectAdvancementTabPacket_V340_769Factory
            },
            {
                Combine(736, 84),
                NbtQueryResponsePacket_V393_763Factory
            },
            {
                Combine(736, 31),
                OpenHorseWindowPacket_V477_767Factory
            },
            {
                Combine(736, 36),
                UpdateLightPacket_V734_754Factory
            },
            {
                Combine(736, 45),
                OpenBookPacket_V477_769Factory
            },
            {
                Combine(736, 64),
                UpdateViewPositionPacket_V477_769Factory
            },
            {
                Combine(736, 65),
                UpdateViewDistancePacket_V477_769Factory
            },
            {
                Combine(736, 7),
                AcknowledgePlayerDiggingPacket_V498_758Factory
            },
            {
                Combine(751, 0),
                SpawnEntityPacket_V477_758Factory
            },
            {
                Combine(751, 1),
                SpawnEntityExperienceOrbPacket_V340_769Factory
            },
            {
                Combine(751, 3),
                SpawnEntityPaintingPacket_V393_758Factory
            },
            {
                Combine(751, 5),
                AnimationPacket_V340_769Factory
            },
            {
                Combine(751, 8),
                BlockBreakAnimationPacket_V340_769Factory
            },
            {
                Combine(751, 9),
                TileEntityDataPacket_V340_756Factory
            },
            {
                Combine(751, 10),
                BlockActionPacket_V340_769Factory
            },
            {
                Combine(751, 11),
                BlockChangePacket_V340_769Factory
            },
            {
                Combine(751, 13),
                DifficultyPacket_V477_769Factory
            },
            {
                Combine(751, 14),
                ChatPacket_V734_758Factory
            },
            {
                Combine(751, 17),
                TransactionPacket_V340_754Factory
            },
            {
                Combine(751, 18),
                CloseWindowPacket_V340_767Factory
            },
            {
                Combine(751, 19),
                WindowItemsPacket_V340_755Factory
            },
            {
                Combine(751, 20),
                CraftProgressBarPacket_V340_767Factory
            },
            {
                Combine(751, 21),
                SetSlotPacket_V340_755Factory
            },
            {
                Combine(751, 22),
                SetCooldownPacket_V340_767Factory
            },
            {
                Combine(751, 23),
                CustomPayloadPacket_V340_769Factory
            },
            {
                Combine(751, 24),
                NamedSoundEffectPacket_V340_758Factory
            },
            {
                Combine(751, 25),
                KickDisconnectPacket_V340_764Factory
            },
            {
                Combine(751, 26),
                EntityStatusPacket_V340_769Factory
            },
            {
                Combine(751, 28),
                UnloadChunkPacket_V340_763Factory
            },
            {
                Combine(751, 29),
                GameStateChangePacket_V340_769Factory
            },
            {
                Combine(751, 31),
                KeepAlivePacket_V340_769Factory
            },
            {
                Combine(751, 33),
                WorldEventPacket_V340_769Factory
            },
            {
                Combine(751, 39),
                RelEntityMovePacket_V340_769Factory
            },
            {
                Combine(751, 40),
                EntityMoveLookPacket_V340_769Factory
            },
            {
                Combine(751, 41),
                EntityLookPacket_V340_769Factory
            },
            {
                Combine(751, 42),
                EntityPacket_V340_754Factory
            },
            {
                Combine(751, 43),
                VehicleMovePacket_V340_769Factory
            },
            {
                Combine(751, 46),
                OpenSignEntityPacket_V340_762Factory
            },
            {
                Combine(751, 48),
                AbilitiesPacket_V340_769Factory
            },
            {
                Combine(751, 52),
                PositionPacket_V340_754Factory
            },
            {
                Combine(751, 54),
                EntityDestroyPacket_V340_754Factory
            },
            {
                Combine(751, 55),
                RemoveEntityEffectPacket_V340_757Factory
            },
            {
                Combine(751, 56),
                ResourcePackSendPacket_V340_754Factory
            },
            {
                Combine(751, 58),
                EntityHeadRotationPacket_V340_769Factory
            },
            {
                Combine(751, 62),
                CameraPacket_V340_769Factory
            },
            {
                Combine(751, 63),
                HeldItemSlotPacket_V340_768Factory
            },
            {
                Combine(751, 67),
                ScoreboardDisplayObjectivePacket_V340_763Factory
            },
            {
                Combine(751, 69),
                AttachEntityPacket_V340_769Factory
            },
            {
                Combine(751, 70),
                EntityVelocityPacket_V340_769Factory
            },
            {
                Combine(751, 72),
                ExperiencePacket_V340_760Factory
            },
            {
                Combine(751, 73),
                UpdateHealthPacket_V340_769Factory
            },
            {
                Combine(751, 75),
                SetPassengersPacket_V340_769Factory
            },
            {
                Combine(751, 66),
                SpawnPositionPacket_V340_754Factory
            },
            {
                Combine(751, 78),
                UpdateTimePacket_V340_767Factory
            },
            {
                Combine(751, 83),
                PlayerlistHeaderPacket_V340_764Factory
            },
            {
                Combine(751, 85),
                CollectPacket_V340_769Factory
            },
            {
                Combine(751, 86),
                EntityTeleportPacket_V340_769Factory
            },
            {
                Combine(751, 89),
                EntityEffectPacket_V340_757Factory
            },
            {
                Combine(751, 60),
                SelectAdvancementTabPacket_V340_769Factory
            },
            {
                Combine(751, 84),
                NbtQueryResponsePacket_V393_763Factory
            },
            {
                Combine(751, 30),
                OpenHorseWindowPacket_V477_767Factory
            },
            {
                Combine(751, 35),
                UpdateLightPacket_V734_754Factory
            },
            {
                Combine(751, 44),
                OpenBookPacket_V477_769Factory
            },
            {
                Combine(751, 64),
                UpdateViewPositionPacket_V477_769Factory
            },
            {
                Combine(751, 65),
                UpdateViewDistancePacket_V477_769Factory
            },
            {
                Combine(751, 7),
                AcknowledgePlayerDiggingPacket_V498_758Factory
            },
            {
                Combine(753, 0),
                SpawnEntityPacket_V477_758Factory
            },
            {
                Combine(753, 1),
                SpawnEntityExperienceOrbPacket_V340_769Factory
            },
            {
                Combine(753, 3),
                SpawnEntityPaintingPacket_V393_758Factory
            },
            {
                Combine(753, 5),
                AnimationPacket_V340_769Factory
            },
            {
                Combine(753, 8),
                BlockBreakAnimationPacket_V340_769Factory
            },
            {
                Combine(753, 9),
                TileEntityDataPacket_V340_756Factory
            },
            {
                Combine(753, 10),
                BlockActionPacket_V340_769Factory
            },
            {
                Combine(753, 11),
                BlockChangePacket_V340_769Factory
            },
            {
                Combine(753, 13),
                DifficultyPacket_V477_769Factory
            },
            {
                Combine(753, 14),
                ChatPacket_V734_758Factory
            },
            {
                Combine(753, 17),
                TransactionPacket_V340_754Factory
            },
            {
                Combine(753, 18),
                CloseWindowPacket_V340_767Factory
            },
            {
                Combine(753, 19),
                WindowItemsPacket_V340_755Factory
            },
            {
                Combine(753, 20),
                CraftProgressBarPacket_V340_767Factory
            },
            {
                Combine(753, 21),
                SetSlotPacket_V340_755Factory
            },
            {
                Combine(753, 22),
                SetCooldownPacket_V340_767Factory
            },
            {
                Combine(753, 23),
                CustomPayloadPacket_V340_769Factory
            },
            {
                Combine(753, 24),
                NamedSoundEffectPacket_V340_758Factory
            },
            {
                Combine(753, 25),
                KickDisconnectPacket_V340_764Factory
            },
            {
                Combine(753, 26),
                EntityStatusPacket_V340_769Factory
            },
            {
                Combine(753, 28),
                UnloadChunkPacket_V340_763Factory
            },
            {
                Combine(753, 29),
                GameStateChangePacket_V340_769Factory
            },
            {
                Combine(753, 31),
                KeepAlivePacket_V340_769Factory
            },
            {
                Combine(753, 33),
                WorldEventPacket_V340_769Factory
            },
            {
                Combine(753, 39),
                RelEntityMovePacket_V340_769Factory
            },
            {
                Combine(753, 40),
                EntityMoveLookPacket_V340_769Factory
            },
            {
                Combine(753, 41),
                EntityLookPacket_V340_769Factory
            },
            {
                Combine(753, 42),
                EntityPacket_V340_754Factory
            },
            {
                Combine(753, 43),
                VehicleMovePacket_V340_769Factory
            },
            {
                Combine(753, 46),
                OpenSignEntityPacket_V340_762Factory
            },
            {
                Combine(753, 48),
                AbilitiesPacket_V340_769Factory
            },
            {
                Combine(753, 52),
                PositionPacket_V340_754Factory
            },
            {
                Combine(753, 54),
                EntityDestroyPacket_V340_754Factory
            },
            {
                Combine(753, 55),
                RemoveEntityEffectPacket_V340_757Factory
            },
            {
                Combine(753, 56),
                ResourcePackSendPacket_V340_754Factory
            },
            {
                Combine(753, 58),
                EntityHeadRotationPacket_V340_769Factory
            },
            {
                Combine(753, 62),
                CameraPacket_V340_769Factory
            },
            {
                Combine(753, 63),
                HeldItemSlotPacket_V340_768Factory
            },
            {
                Combine(753, 67),
                ScoreboardDisplayObjectivePacket_V340_763Factory
            },
            {
                Combine(753, 69),
                AttachEntityPacket_V340_769Factory
            },
            {
                Combine(753, 70),
                EntityVelocityPacket_V340_769Factory
            },
            {
                Combine(753, 72),
                ExperiencePacket_V340_760Factory
            },
            {
                Combine(753, 73),
                UpdateHealthPacket_V340_769Factory
            },
            {
                Combine(753, 75),
                SetPassengersPacket_V340_769Factory
            },
            {
                Combine(753, 66),
                SpawnPositionPacket_V340_754Factory
            },
            {
                Combine(753, 78),
                UpdateTimePacket_V340_767Factory
            },
            {
                Combine(753, 83),
                PlayerlistHeaderPacket_V340_764Factory
            },
            {
                Combine(753, 85),
                CollectPacket_V340_769Factory
            },
            {
                Combine(753, 86),
                EntityTeleportPacket_V340_769Factory
            },
            {
                Combine(753, 89),
                EntityEffectPacket_V340_757Factory
            },
            {
                Combine(753, 60),
                SelectAdvancementTabPacket_V340_769Factory
            },
            {
                Combine(753, 84),
                NbtQueryResponsePacket_V393_763Factory
            },
            {
                Combine(753, 30),
                OpenHorseWindowPacket_V477_767Factory
            },
            {
                Combine(753, 35),
                UpdateLightPacket_V734_754Factory
            },
            {
                Combine(753, 44),
                OpenBookPacket_V477_769Factory
            },
            {
                Combine(753, 64),
                UpdateViewPositionPacket_V477_769Factory
            },
            {
                Combine(753, 65),
                UpdateViewDistancePacket_V477_769Factory
            },
            {
                Combine(753, 7),
                AcknowledgePlayerDiggingPacket_V498_758Factory
            },
            {
                Combine(754, 0),
                SpawnEntityPacket_V477_758Factory
            },
            {
                Combine(754, 1),
                SpawnEntityExperienceOrbPacket_V340_769Factory
            },
            {
                Combine(754, 3),
                SpawnEntityPaintingPacket_V393_758Factory
            },
            {
                Combine(754, 5),
                AnimationPacket_V340_769Factory
            },
            {
                Combine(754, 8),
                BlockBreakAnimationPacket_V340_769Factory
            },
            {
                Combine(754, 9),
                TileEntityDataPacket_V340_756Factory
            },
            {
                Combine(754, 10),
                BlockActionPacket_V340_769Factory
            },
            {
                Combine(754, 11),
                BlockChangePacket_V340_769Factory
            },
            {
                Combine(754, 13),
                DifficultyPacket_V477_769Factory
            },
            {
                Combine(754, 14),
                ChatPacket_V734_758Factory
            },
            {
                Combine(754, 17),
                TransactionPacket_V340_754Factory
            },
            {
                Combine(754, 18),
                CloseWindowPacket_V340_767Factory
            },
            {
                Combine(754, 19),
                WindowItemsPacket_V340_755Factory
            },
            {
                Combine(754, 20),
                CraftProgressBarPacket_V340_767Factory
            },
            {
                Combine(754, 21),
                SetSlotPacket_V340_755Factory
            },
            {
                Combine(754, 22),
                SetCooldownPacket_V340_767Factory
            },
            {
                Combine(754, 23),
                CustomPayloadPacket_V340_769Factory
            },
            {
                Combine(754, 24),
                NamedSoundEffectPacket_V340_758Factory
            },
            {
                Combine(754, 25),
                KickDisconnectPacket_V340_764Factory
            },
            {
                Combine(754, 26),
                EntityStatusPacket_V340_769Factory
            },
            {
                Combine(754, 28),
                UnloadChunkPacket_V340_763Factory
            },
            {
                Combine(754, 29),
                GameStateChangePacket_V340_769Factory
            },
            {
                Combine(754, 31),
                KeepAlivePacket_V340_769Factory
            },
            {
                Combine(754, 33),
                WorldEventPacket_V340_769Factory
            },
            {
                Combine(754, 39),
                RelEntityMovePacket_V340_769Factory
            },
            {
                Combine(754, 40),
                EntityMoveLookPacket_V340_769Factory
            },
            {
                Combine(754, 41),
                EntityLookPacket_V340_769Factory
            },
            {
                Combine(754, 42),
                EntityPacket_V340_754Factory
            },
            {
                Combine(754, 43),
                VehicleMovePacket_V340_769Factory
            },
            {
                Combine(754, 46),
                OpenSignEntityPacket_V340_762Factory
            },
            {
                Combine(754, 48),
                AbilitiesPacket_V340_769Factory
            },
            {
                Combine(754, 52),
                PositionPacket_V340_754Factory
            },
            {
                Combine(754, 54),
                EntityDestroyPacket_V340_754Factory
            },
            {
                Combine(754, 55),
                RemoveEntityEffectPacket_V340_757Factory
            },
            {
                Combine(754, 56),
                ResourcePackSendPacket_V340_754Factory
            },
            {
                Combine(754, 58),
                EntityHeadRotationPacket_V340_769Factory
            },
            {
                Combine(754, 62),
                CameraPacket_V340_769Factory
            },
            {
                Combine(754, 63),
                HeldItemSlotPacket_V340_768Factory
            },
            {
                Combine(754, 67),
                ScoreboardDisplayObjectivePacket_V340_763Factory
            },
            {
                Combine(754, 69),
                AttachEntityPacket_V340_769Factory
            },
            {
                Combine(754, 70),
                EntityVelocityPacket_V340_769Factory
            },
            {
                Combine(754, 72),
                ExperiencePacket_V340_760Factory
            },
            {
                Combine(754, 73),
                UpdateHealthPacket_V340_769Factory
            },
            {
                Combine(754, 75),
                SetPassengersPacket_V340_769Factory
            },
            {
                Combine(754, 66),
                SpawnPositionPacket_V340_754Factory
            },
            {
                Combine(754, 78),
                UpdateTimePacket_V340_767Factory
            },
            {
                Combine(754, 83),
                PlayerlistHeaderPacket_V340_764Factory
            },
            {
                Combine(754, 85),
                CollectPacket_V340_769Factory
            },
            {
                Combine(754, 86),
                EntityTeleportPacket_V340_769Factory
            },
            {
                Combine(754, 89),
                EntityEffectPacket_V340_757Factory
            },
            {
                Combine(754, 60),
                SelectAdvancementTabPacket_V340_769Factory
            },
            {
                Combine(754, 84),
                NbtQueryResponsePacket_V393_763Factory
            },
            {
                Combine(754, 30),
                OpenHorseWindowPacket_V477_767Factory
            },
            {
                Combine(754, 35),
                UpdateLightPacket_V734_754Factory
            },
            {
                Combine(754, 44),
                OpenBookPacket_V477_769Factory
            },
            {
                Combine(754, 64),
                UpdateViewPositionPacket_V477_769Factory
            },
            {
                Combine(754, 65),
                UpdateViewDistancePacket_V477_769Factory
            },
            {
                Combine(754, 7),
                AcknowledgePlayerDiggingPacket_V498_758Factory
            },
            {
                Combine(755, 0),
                SpawnEntityPacket_V477_758Factory
            },
            {
                Combine(755, 1),
                SpawnEntityExperienceOrbPacket_V340_769Factory
            },
            {
                Combine(755, 3),
                SpawnEntityPaintingPacket_V393_758Factory
            },
            {
                Combine(755, 6),
                AnimationPacket_V340_769Factory
            },
            {
                Combine(755, 9),
                BlockBreakAnimationPacket_V340_769Factory
            },
            {
                Combine(755, 10),
                TileEntityDataPacket_V340_756Factory
            },
            {
                Combine(755, 11),
                BlockActionPacket_V340_769Factory
            },
            {
                Combine(755, 12),
                BlockChangePacket_V340_769Factory
            },
            {
                Combine(755, 14),
                DifficultyPacket_V477_769Factory
            },
            {
                Combine(755, 15),
                ChatPacket_V734_758Factory
            },
            {
                Combine(755, 19),
                CloseWindowPacket_V340_767Factory
            },
            {
                Combine(755, 20),
                WindowItemsPacket_V340_755Factory
            },
            {
                Combine(755, 21),
                CraftProgressBarPacket_V340_767Factory
            },
            {
                Combine(755, 22),
                SetSlotPacket_V340_755Factory
            },
            {
                Combine(755, 23),
                SetCooldownPacket_V340_767Factory
            },
            {
                Combine(755, 24),
                CustomPayloadPacket_V340_769Factory
            },
            {
                Combine(755, 25),
                NamedSoundEffectPacket_V340_758Factory
            },
            {
                Combine(755, 26),
                KickDisconnectPacket_V340_764Factory
            },
            {
                Combine(755, 27),
                EntityStatusPacket_V340_769Factory
            },
            {
                Combine(755, 29),
                UnloadChunkPacket_V340_763Factory
            },
            {
                Combine(755, 30),
                GameStateChangePacket_V340_769Factory
            },
            {
                Combine(755, 33),
                KeepAlivePacket_V340_769Factory
            },
            {
                Combine(755, 35),
                WorldEventPacket_V340_769Factory
            },
            {
                Combine(755, 41),
                RelEntityMovePacket_V340_769Factory
            },
            {
                Combine(755, 42),
                EntityMoveLookPacket_V340_769Factory
            },
            {
                Combine(755, 43),
                EntityLookPacket_V340_769Factory
            },
            {
                Combine(755, 44),
                VehicleMovePacket_V340_769Factory
            },
            {
                Combine(755, 47),
                OpenSignEntityPacket_V340_762Factory
            },
            {
                Combine(755, 50),
                AbilitiesPacket_V340_769Factory
            },
            {
                Combine(755, 56),
                PositionPacket_V755_761Factory
            },
            {
                Combine(755, 59),
                RemoveEntityEffectPacket_V340_757Factory
            },
            {
                Combine(755, 60),
                ResourcePackSendPacket_V755_764Factory
            },
            {
                Combine(755, 62),
                EntityHeadRotationPacket_V340_769Factory
            },
            {
                Combine(755, 71),
                CameraPacket_V340_769Factory
            },
            {
                Combine(755, 72),
                HeldItemSlotPacket_V340_768Factory
            },
            {
                Combine(755, 76),
                ScoreboardDisplayObjectivePacket_V340_763Factory
            },
            {
                Combine(755, 78),
                AttachEntityPacket_V340_769Factory
            },
            {
                Combine(755, 79),
                EntityVelocityPacket_V340_769Factory
            },
            {
                Combine(755, 81),
                ExperiencePacket_V340_760Factory
            },
            {
                Combine(755, 82),
                UpdateHealthPacket_V340_769Factory
            },
            {
                Combine(755, 84),
                SetPassengersPacket_V340_769Factory
            },
            {
                Combine(755, 75),
                SpawnPositionPacket_V755_769Factory
            },
            {
                Combine(755, 88),
                UpdateTimePacket_V340_767Factory
            },
            {
                Combine(755, 94),
                PlayerlistHeaderPacket_V340_764Factory
            },
            {
                Combine(755, 96),
                CollectPacket_V340_769Factory
            },
            {
                Combine(755, 97),
                EntityTeleportPacket_V340_769Factory
            },
            {
                Combine(755, 100),
                EntityEffectPacket_V340_757Factory
            },
            {
                Combine(755, 64),
                SelectAdvancementTabPacket_V340_769Factory
            },
            {
                Combine(755, 95),
                NbtQueryResponsePacket_V393_763Factory
            },
            {
                Combine(755, 31),
                OpenHorseWindowPacket_V477_767Factory
            },
            {
                Combine(755, 37),
                UpdateLightPacket_V755_762Factory
            },
            {
                Combine(755, 45),
                OpenBookPacket_V477_769Factory
            },
            {
                Combine(755, 73),
                UpdateViewPositionPacket_V477_769Factory
            },
            {
                Combine(755, 74),
                UpdateViewDistancePacket_V477_769Factory
            },
            {
                Combine(755, 8),
                AcknowledgePlayerDiggingPacket_V498_758Factory
            },
            {
                Combine(755, 51),
                EndCombatEventPacket_V755_762Factory
            },
            {
                Combine(755, 52),
                EnterCombatEventPacket_V755_769Factory
            },
            {
                Combine(755, 53),
                DeathCombatEventPacket_V755_762Factory
            },
            {
                Combine(755, 58),
                DestroyEntityPacket_V755Factory
            },
            {
                Combine(755, 16),
                ClearTitlesPacket_V755_769Factory
            },
            {
                Combine(755, 32),
                InitializeWorldBorderPacket_V755_758Factory
            },
            {
                Combine(755, 65),
                ActionBarPacket_V755_764Factory
            },
            {
                Combine(755, 66),
                WorldBorderCenterPacket_V755_769Factory
            },
            {
                Combine(755, 67),
                WorldBorderLerpSizePacket_V755_758Factory
            },
            {
                Combine(755, 68),
                WorldBorderSizePacket_V755_769Factory
            },
            {
                Combine(755, 69),
                WorldBorderWarningDelayPacket_V755_769Factory
            },
            {
                Combine(755, 70),
                WorldBorderWarningReachPacket_V755_769Factory
            },
            {
                Combine(755, 48),
                PingPacket_V755_769Factory
            },
            {
                Combine(755, 87),
                SetTitleSubtitlePacket_V755_764Factory
            },
            {
                Combine(755, 89),
                SetTitleTextPacket_V755_764Factory
            },
            {
                Combine(755, 90),
                SetTitleTimePacket_V755_769Factory
            },
            {
                Combine(756, 0),
                SpawnEntityPacket_V477_758Factory
            },
            {
                Combine(756, 1),
                SpawnEntityExperienceOrbPacket_V340_769Factory
            },
            {
                Combine(756, 3),
                SpawnEntityPaintingPacket_V393_758Factory
            },
            {
                Combine(756, 6),
                AnimationPacket_V340_769Factory
            },
            {
                Combine(756, 9),
                BlockBreakAnimationPacket_V340_769Factory
            },
            {
                Combine(756, 10),
                TileEntityDataPacket_V340_756Factory
            },
            {
                Combine(756, 11),
                BlockActionPacket_V340_769Factory
            },
            {
                Combine(756, 12),
                BlockChangePacket_V340_769Factory
            },
            {
                Combine(756, 14),
                DifficultyPacket_V477_769Factory
            },
            {
                Combine(756, 15),
                ChatPacket_V734_758Factory
            },
            {
                Combine(756, 19),
                CloseWindowPacket_V340_767Factory
            },
            {
                Combine(756, 20),
                WindowItemsPacket_V756_765Factory
            },
            {
                Combine(756, 21),
                CraftProgressBarPacket_V340_767Factory
            },
            {
                Combine(756, 22),
                SetSlotPacket_V756_765Factory
            },
            {
                Combine(756, 23),
                SetCooldownPacket_V340_767Factory
            },
            {
                Combine(756, 24),
                CustomPayloadPacket_V340_769Factory
            },
            {
                Combine(756, 25),
                NamedSoundEffectPacket_V340_758Factory
            },
            {
                Combine(756, 26),
                KickDisconnectPacket_V340_764Factory
            },
            {
                Combine(756, 27),
                EntityStatusPacket_V340_769Factory
            },
            {
                Combine(756, 29),
                UnloadChunkPacket_V340_763Factory
            },
            {
                Combine(756, 30),
                GameStateChangePacket_V340_769Factory
            },
            {
                Combine(756, 33),
                KeepAlivePacket_V340_769Factory
            },
            {
                Combine(756, 35),
                WorldEventPacket_V340_769Factory
            },
            {
                Combine(756, 41),
                RelEntityMovePacket_V340_769Factory
            },
            {
                Combine(756, 42),
                EntityMoveLookPacket_V340_769Factory
            },
            {
                Combine(756, 43),
                EntityLookPacket_V340_769Factory
            },
            {
                Combine(756, 44),
                VehicleMovePacket_V340_769Factory
            },
            {
                Combine(756, 47),
                OpenSignEntityPacket_V340_762Factory
            },
            {
                Combine(756, 50),
                AbilitiesPacket_V340_769Factory
            },
            {
                Combine(756, 56),
                PositionPacket_V755_761Factory
            },
            {
                Combine(756, 58),
                EntityDestroyPacket_V756_769Factory
            },
            {
                Combine(756, 59),
                RemoveEntityEffectPacket_V340_757Factory
            },
            {
                Combine(756, 60),
                ResourcePackSendPacket_V755_764Factory
            },
            {
                Combine(756, 62),
                EntityHeadRotationPacket_V340_769Factory
            },
            {
                Combine(756, 71),
                CameraPacket_V340_769Factory
            },
            {
                Combine(756, 72),
                HeldItemSlotPacket_V340_768Factory
            },
            {
                Combine(756, 76),
                ScoreboardDisplayObjectivePacket_V340_763Factory
            },
            {
                Combine(756, 78),
                AttachEntityPacket_V340_769Factory
            },
            {
                Combine(756, 79),
                EntityVelocityPacket_V340_769Factory
            },
            {
                Combine(756, 81),
                ExperiencePacket_V340_760Factory
            },
            {
                Combine(756, 82),
                UpdateHealthPacket_V340_769Factory
            },
            {
                Combine(756, 84),
                SetPassengersPacket_V340_769Factory
            },
            {
                Combine(756, 75),
                SpawnPositionPacket_V755_769Factory
            },
            {
                Combine(756, 88),
                UpdateTimePacket_V340_767Factory
            },
            {
                Combine(756, 94),
                PlayerlistHeaderPacket_V340_764Factory
            },
            {
                Combine(756, 96),
                CollectPacket_V340_769Factory
            },
            {
                Combine(756, 97),
                EntityTeleportPacket_V340_769Factory
            },
            {
                Combine(756, 100),
                EntityEffectPacket_V340_757Factory
            },
            {
                Combine(756, 64),
                SelectAdvancementTabPacket_V340_769Factory
            },
            {
                Combine(756, 95),
                NbtQueryResponsePacket_V393_763Factory
            },
            {
                Combine(756, 31),
                OpenHorseWindowPacket_V477_767Factory
            },
            {
                Combine(756, 37),
                UpdateLightPacket_V755_762Factory
            },
            {
                Combine(756, 45),
                OpenBookPacket_V477_769Factory
            },
            {
                Combine(756, 73),
                UpdateViewPositionPacket_V477_769Factory
            },
            {
                Combine(756, 74),
                UpdateViewDistancePacket_V477_769Factory
            },
            {
                Combine(756, 8),
                AcknowledgePlayerDiggingPacket_V498_758Factory
            },
            {
                Combine(756, 51),
                EndCombatEventPacket_V755_762Factory
            },
            {
                Combine(756, 52),
                EnterCombatEventPacket_V755_769Factory
            },
            {
                Combine(756, 53),
                DeathCombatEventPacket_V755_762Factory
            },
            {
                Combine(756, 16),
                ClearTitlesPacket_V755_769Factory
            },
            {
                Combine(756, 32),
                InitializeWorldBorderPacket_V755_758Factory
            },
            {
                Combine(756, 65),
                ActionBarPacket_V755_764Factory
            },
            {
                Combine(756, 66),
                WorldBorderCenterPacket_V755_769Factory
            },
            {
                Combine(756, 67),
                WorldBorderLerpSizePacket_V755_758Factory
            },
            {
                Combine(756, 68),
                WorldBorderSizePacket_V755_769Factory
            },
            {
                Combine(756, 69),
                WorldBorderWarningDelayPacket_V755_769Factory
            },
            {
                Combine(756, 70),
                WorldBorderWarningReachPacket_V755_769Factory
            },
            {
                Combine(756, 48),
                PingPacket_V755_769Factory
            },
            {
                Combine(756, 87),
                SetTitleSubtitlePacket_V755_764Factory
            },
            {
                Combine(756, 89),
                SetTitleTextPacket_V755_764Factory
            },
            {
                Combine(756, 90),
                SetTitleTimePacket_V755_769Factory
            },
            {
                Combine(757, 0),
                SpawnEntityPacket_V477_758Factory
            },
            {
                Combine(757, 1),
                SpawnEntityExperienceOrbPacket_V340_769Factory
            },
            {
                Combine(757, 3),
                SpawnEntityPaintingPacket_V393_758Factory
            },
            {
                Combine(757, 6),
                AnimationPacket_V340_769Factory
            },
            {
                Combine(757, 9),
                BlockBreakAnimationPacket_V340_769Factory
            },
            {
                Combine(757, 10),
                TileEntityDataPacket_V757_763Factory
            },
            {
                Combine(757, 11),
                BlockActionPacket_V340_769Factory
            },
            {
                Combine(757, 12),
                BlockChangePacket_V340_769Factory
            },
            {
                Combine(757, 14),
                DifficultyPacket_V477_769Factory
            },
            {
                Combine(757, 15),
                ChatPacket_V734_758Factory
            },
            {
                Combine(757, 19),
                CloseWindowPacket_V340_767Factory
            },
            {
                Combine(757, 20),
                WindowItemsPacket_V756_765Factory
            },
            {
                Combine(757, 21),
                CraftProgressBarPacket_V340_767Factory
            },
            {
                Combine(757, 22),
                SetSlotPacket_V756_765Factory
            },
            {
                Combine(757, 23),
                SetCooldownPacket_V340_767Factory
            },
            {
                Combine(757, 24),
                CustomPayloadPacket_V340_769Factory
            },
            {
                Combine(757, 25),
                NamedSoundEffectPacket_V340_758Factory
            },
            {
                Combine(757, 26),
                KickDisconnectPacket_V340_764Factory
            },
            {
                Combine(757, 27),
                EntityStatusPacket_V340_769Factory
            },
            {
                Combine(757, 29),
                UnloadChunkPacket_V340_763Factory
            },
            {
                Combine(757, 30),
                GameStateChangePacket_V340_769Factory
            },
            {
                Combine(757, 33),
                KeepAlivePacket_V340_769Factory
            },
            {
                Combine(757, 35),
                WorldEventPacket_V340_769Factory
            },
            {
                Combine(757, 41),
                RelEntityMovePacket_V340_769Factory
            },
            {
                Combine(757, 42),
                EntityMoveLookPacket_V340_769Factory
            },
            {
                Combine(757, 43),
                EntityLookPacket_V340_769Factory
            },
            {
                Combine(757, 44),
                VehicleMovePacket_V340_769Factory
            },
            {
                Combine(757, 47),
                OpenSignEntityPacket_V340_762Factory
            },
            {
                Combine(757, 50),
                AbilitiesPacket_V340_769Factory
            },
            {
                Combine(757, 56),
                PositionPacket_V755_761Factory
            },
            {
                Combine(757, 58),
                EntityDestroyPacket_V756_769Factory
            },
            {
                Combine(757, 59),
                RemoveEntityEffectPacket_V340_757Factory
            },
            {
                Combine(757, 60),
                ResourcePackSendPacket_V755_764Factory
            },
            {
                Combine(757, 62),
                EntityHeadRotationPacket_V340_769Factory
            },
            {
                Combine(757, 71),
                CameraPacket_V340_769Factory
            },
            {
                Combine(757, 72),
                HeldItemSlotPacket_V340_768Factory
            },
            {
                Combine(757, 76),
                ScoreboardDisplayObjectivePacket_V340_763Factory
            },
            {
                Combine(757, 78),
                AttachEntityPacket_V340_769Factory
            },
            {
                Combine(757, 79),
                EntityVelocityPacket_V340_769Factory
            },
            {
                Combine(757, 81),
                ExperiencePacket_V340_760Factory
            },
            {
                Combine(757, 82),
                UpdateHealthPacket_V340_769Factory
            },
            {
                Combine(757, 84),
                SetPassengersPacket_V340_769Factory
            },
            {
                Combine(757, 75),
                SpawnPositionPacket_V755_769Factory
            },
            {
                Combine(757, 89),
                UpdateTimePacket_V340_767Factory
            },
            {
                Combine(757, 95),
                PlayerlistHeaderPacket_V340_764Factory
            },
            {
                Combine(757, 97),
                CollectPacket_V340_769Factory
            },
            {
                Combine(757, 98),
                EntityTeleportPacket_V340_769Factory
            },
            {
                Combine(757, 101),
                EntityEffectPacket_V340_757Factory
            },
            {
                Combine(757, 64),
                SelectAdvancementTabPacket_V340_769Factory
            },
            {
                Combine(757, 96),
                NbtQueryResponsePacket_V393_763Factory
            },
            {
                Combine(757, 31),
                OpenHorseWindowPacket_V477_767Factory
            },
            {
                Combine(757, 37),
                UpdateLightPacket_V755_762Factory
            },
            {
                Combine(757, 45),
                OpenBookPacket_V477_769Factory
            },
            {
                Combine(757, 73),
                UpdateViewPositionPacket_V477_769Factory
            },
            {
                Combine(757, 74),
                UpdateViewDistancePacket_V477_769Factory
            },
            {
                Combine(757, 8),
                AcknowledgePlayerDiggingPacket_V498_758Factory
            },
            {
                Combine(757, 51),
                EndCombatEventPacket_V755_762Factory
            },
            {
                Combine(757, 52),
                EnterCombatEventPacket_V755_769Factory
            },
            {
                Combine(757, 53),
                DeathCombatEventPacket_V755_762Factory
            },
            {
                Combine(757, 16),
                ClearTitlesPacket_V755_769Factory
            },
            {
                Combine(757, 32),
                InitializeWorldBorderPacket_V755_758Factory
            },
            {
                Combine(757, 65),
                ActionBarPacket_V755_764Factory
            },
            {
                Combine(757, 66),
                WorldBorderCenterPacket_V755_769Factory
            },
            {
                Combine(757, 67),
                WorldBorderLerpSizePacket_V755_758Factory
            },
            {
                Combine(757, 68),
                WorldBorderSizePacket_V755_769Factory
            },
            {
                Combine(757, 69),
                WorldBorderWarningDelayPacket_V755_769Factory
            },
            {
                Combine(757, 70),
                WorldBorderWarningReachPacket_V755_769Factory
            },
            {
                Combine(757, 48),
                PingPacket_V755_769Factory
            },
            {
                Combine(757, 88),
                SetTitleSubtitlePacket_V755_764Factory
            },
            {
                Combine(757, 90),
                SetTitleTextPacket_V755_764Factory
            },
            {
                Combine(757, 91),
                SetTitleTimePacket_V755_769Factory
            },
            {
                Combine(757, 87),
                SimulationDistancePacket_V757_769Factory
            },
            {
                Combine(758, 0),
                SpawnEntityPacket_V477_758Factory
            },
            {
                Combine(758, 1),
                SpawnEntityExperienceOrbPacket_V340_769Factory
            },
            {
                Combine(758, 3),
                SpawnEntityPaintingPacket_V393_758Factory
            },
            {
                Combine(758, 6),
                AnimationPacket_V340_769Factory
            },
            {
                Combine(758, 9),
                BlockBreakAnimationPacket_V340_769Factory
            },
            {
                Combine(758, 10),
                TileEntityDataPacket_V757_763Factory
            },
            {
                Combine(758, 11),
                BlockActionPacket_V340_769Factory
            },
            {
                Combine(758, 12),
                BlockChangePacket_V340_769Factory
            },
            {
                Combine(758, 14),
                DifficultyPacket_V477_769Factory
            },
            {
                Combine(758, 15),
                ChatPacket_V734_758Factory
            },
            {
                Combine(758, 19),
                CloseWindowPacket_V340_767Factory
            },
            {
                Combine(758, 20),
                WindowItemsPacket_V756_765Factory
            },
            {
                Combine(758, 21),
                CraftProgressBarPacket_V340_767Factory
            },
            {
                Combine(758, 22),
                SetSlotPacket_V756_765Factory
            },
            {
                Combine(758, 23),
                SetCooldownPacket_V340_767Factory
            },
            {
                Combine(758, 24),
                CustomPayloadPacket_V340_769Factory
            },
            {
                Combine(758, 25),
                NamedSoundEffectPacket_V340_758Factory
            },
            {
                Combine(758, 26),
                KickDisconnectPacket_V340_764Factory
            },
            {
                Combine(758, 27),
                EntityStatusPacket_V340_769Factory
            },
            {
                Combine(758, 29),
                UnloadChunkPacket_V340_763Factory
            },
            {
                Combine(758, 30),
                GameStateChangePacket_V340_769Factory
            },
            {
                Combine(758, 33),
                KeepAlivePacket_V340_769Factory
            },
            {
                Combine(758, 35),
                WorldEventPacket_V340_769Factory
            },
            {
                Combine(758, 41),
                RelEntityMovePacket_V340_769Factory
            },
            {
                Combine(758, 42),
                EntityMoveLookPacket_V340_769Factory
            },
            {
                Combine(758, 43),
                EntityLookPacket_V340_769Factory
            },
            {
                Combine(758, 44),
                VehicleMovePacket_V340_769Factory
            },
            {
                Combine(758, 47),
                OpenSignEntityPacket_V340_762Factory
            },
            {
                Combine(758, 50),
                AbilitiesPacket_V340_769Factory
            },
            {
                Combine(758, 56),
                PositionPacket_V755_761Factory
            },
            {
                Combine(758, 58),
                EntityDestroyPacket_V756_769Factory
            },
            {
                Combine(758, 59),
                RemoveEntityEffectPacket_V758_769Factory
            },
            {
                Combine(758, 60),
                ResourcePackSendPacket_V755_764Factory
            },
            {
                Combine(758, 62),
                EntityHeadRotationPacket_V340_769Factory
            },
            {
                Combine(758, 71),
                CameraPacket_V340_769Factory
            },
            {
                Combine(758, 72),
                HeldItemSlotPacket_V340_768Factory
            },
            {
                Combine(758, 76),
                ScoreboardDisplayObjectivePacket_V340_763Factory
            },
            {
                Combine(758, 78),
                AttachEntityPacket_V340_769Factory
            },
            {
                Combine(758, 79),
                EntityVelocityPacket_V340_769Factory
            },
            {
                Combine(758, 81),
                ExperiencePacket_V340_760Factory
            },
            {
                Combine(758, 82),
                UpdateHealthPacket_V340_769Factory
            },
            {
                Combine(758, 84),
                SetPassengersPacket_V340_769Factory
            },
            {
                Combine(758, 75),
                SpawnPositionPacket_V755_769Factory
            },
            {
                Combine(758, 89),
                UpdateTimePacket_V340_767Factory
            },
            {
                Combine(758, 95),
                PlayerlistHeaderPacket_V340_764Factory
            },
            {
                Combine(758, 97),
                CollectPacket_V340_769Factory
            },
            {
                Combine(758, 98),
                EntityTeleportPacket_V340_769Factory
            },
            {
                Combine(758, 101),
                EntityEffectPacket_V758Factory
            },
            {
                Combine(758, 64),
                SelectAdvancementTabPacket_V340_769Factory
            },
            {
                Combine(758, 96),
                NbtQueryResponsePacket_V393_763Factory
            },
            {
                Combine(758, 31),
                OpenHorseWindowPacket_V477_767Factory
            },
            {
                Combine(758, 37),
                UpdateLightPacket_V755_762Factory
            },
            {
                Combine(758, 45),
                OpenBookPacket_V477_769Factory
            },
            {
                Combine(758, 73),
                UpdateViewPositionPacket_V477_769Factory
            },
            {
                Combine(758, 74),
                UpdateViewDistancePacket_V477_769Factory
            },
            {
                Combine(758, 8),
                AcknowledgePlayerDiggingPacket_V498_758Factory
            },
            {
                Combine(758, 51),
                EndCombatEventPacket_V755_762Factory
            },
            {
                Combine(758, 52),
                EnterCombatEventPacket_V755_769Factory
            },
            {
                Combine(758, 53),
                DeathCombatEventPacket_V755_762Factory
            },
            {
                Combine(758, 16),
                ClearTitlesPacket_V755_769Factory
            },
            {
                Combine(758, 32),
                InitializeWorldBorderPacket_V755_758Factory
            },
            {
                Combine(758, 65),
                ActionBarPacket_V755_764Factory
            },
            {
                Combine(758, 66),
                WorldBorderCenterPacket_V755_769Factory
            },
            {
                Combine(758, 67),
                WorldBorderLerpSizePacket_V755_758Factory
            },
            {
                Combine(758, 68),
                WorldBorderSizePacket_V755_769Factory
            },
            {
                Combine(758, 69),
                WorldBorderWarningDelayPacket_V755_769Factory
            },
            {
                Combine(758, 70),
                WorldBorderWarningReachPacket_V755_769Factory
            },
            {
                Combine(758, 48),
                PingPacket_V755_769Factory
            },
            {
                Combine(758, 88),
                SetTitleSubtitlePacket_V755_764Factory
            },
            {
                Combine(758, 90),
                SetTitleTextPacket_V755_764Factory
            },
            {
                Combine(758, 91),
                SetTitleTimePacket_V755_769Factory
            },
            {
                Combine(758, 87),
                SimulationDistancePacket_V757_769Factory
            },
            {
                Combine(759, 0),
                SpawnEntityPacket_V759_769Factory
            },
            {
                Combine(759, 1),
                SpawnEntityExperienceOrbPacket_V340_769Factory
            },
            {
                Combine(759, 3),
                AnimationPacket_V340_769Factory
            },
            {
                Combine(759, 6),
                BlockBreakAnimationPacket_V340_769Factory
            },
            {
                Combine(759, 7),
                TileEntityDataPacket_V757_763Factory
            },
            {
                Combine(759, 8),
                BlockActionPacket_V340_769Factory
            },
            {
                Combine(759, 9),
                BlockChangePacket_V340_769Factory
            },
            {
                Combine(759, 11),
                DifficultyPacket_V477_769Factory
            },
            {
                Combine(759, 16),
                CloseWindowPacket_V340_767Factory
            },
            {
                Combine(759, 17),
                WindowItemsPacket_V756_765Factory
            },
            {
                Combine(759, 18),
                CraftProgressBarPacket_V340_767Factory
            },
            {
                Combine(759, 19),
                SetSlotPacket_V756_765Factory
            },
            {
                Combine(759, 20),
                SetCooldownPacket_V340_767Factory
            },
            {
                Combine(759, 21),
                CustomPayloadPacket_V340_769Factory
            },
            {
                Combine(759, 22),
                NamedSoundEffectPacket_V759_760Factory
            },
            {
                Combine(759, 23),
                KickDisconnectPacket_V340_764Factory
            },
            {
                Combine(759, 24),
                EntityStatusPacket_V340_769Factory
            },
            {
                Combine(759, 26),
                UnloadChunkPacket_V340_763Factory
            },
            {
                Combine(759, 27),
                GameStateChangePacket_V340_769Factory
            },
            {
                Combine(759, 30),
                KeepAlivePacket_V340_769Factory
            },
            {
                Combine(759, 32),
                WorldEventPacket_V340_769Factory
            },
            {
                Combine(759, 38),
                RelEntityMovePacket_V340_769Factory
            },
            {
                Combine(759, 39),
                EntityMoveLookPacket_V340_769Factory
            },
            {
                Combine(759, 40),
                EntityLookPacket_V340_769Factory
            },
            {
                Combine(759, 41),
                VehicleMovePacket_V340_769Factory
            },
            {
                Combine(759, 44),
                OpenSignEntityPacket_V340_762Factory
            },
            {
                Combine(759, 47),
                AbilitiesPacket_V340_769Factory
            },
            {
                Combine(759, 54),
                PositionPacket_V755_761Factory
            },
            {
                Combine(759, 56),
                EntityDestroyPacket_V756_769Factory
            },
            {
                Combine(759, 57),
                RemoveEntityEffectPacket_V758_769Factory
            },
            {
                Combine(759, 58),
                ResourcePackSendPacket_V755_764Factory
            },
            {
                Combine(759, 60),
                EntityHeadRotationPacket_V340_769Factory
            },
            {
                Combine(759, 70),
                CameraPacket_V340_769Factory
            },
            {
                Combine(759, 71),
                HeldItemSlotPacket_V340_768Factory
            },
            {
                Combine(759, 76),
                ScoreboardDisplayObjectivePacket_V340_763Factory
            },
            {
                Combine(759, 78),
                AttachEntityPacket_V340_769Factory
            },
            {
                Combine(759, 79),
                EntityVelocityPacket_V340_769Factory
            },
            {
                Combine(759, 81),
                ExperiencePacket_V340_760Factory
            },
            {
                Combine(759, 82),
                UpdateHealthPacket_V340_769Factory
            },
            {
                Combine(759, 84),
                SetPassengersPacket_V340_769Factory
            },
            {
                Combine(759, 74),
                SpawnPositionPacket_V755_769Factory
            },
            {
                Combine(759, 89),
                UpdateTimePacket_V340_767Factory
            },
            {
                Combine(759, 96),
                PlayerlistHeaderPacket_V340_764Factory
            },
            {
                Combine(759, 98),
                CollectPacket_V340_769Factory
            },
            {
                Combine(759, 99),
                EntityTeleportPacket_V340_769Factory
            },
            {
                Combine(759, 102),
                EntityEffectPacket_V759_763Factory
            },
            {
                Combine(759, 62),
                SelectAdvancementTabPacket_V340_769Factory
            },
            {
                Combine(759, 97),
                NbtQueryResponsePacket_V393_763Factory
            },
            {
                Combine(759, 28),
                OpenHorseWindowPacket_V477_767Factory
            },
            {
                Combine(759, 34),
                UpdateLightPacket_V755_762Factory
            },
            {
                Combine(759, 42),
                OpenBookPacket_V477_769Factory
            },
            {
                Combine(759, 72),
                UpdateViewPositionPacket_V477_769Factory
            },
            {
                Combine(759, 73),
                UpdateViewDistancePacket_V477_769Factory
            },
            {
                Combine(759, 5),
                AcknowledgePlayerDiggingPacket_V759_769Factory
            },
            {
                Combine(759, 49),
                EndCombatEventPacket_V755_762Factory
            },
            {
                Combine(759, 50),
                EnterCombatEventPacket_V755_769Factory
            },
            {
                Combine(759, 51),
                DeathCombatEventPacket_V755_762Factory
            },
            {
                Combine(759, 13),
                ClearTitlesPacket_V755_769Factory
            },
            {
                Combine(759, 29),
                InitializeWorldBorderPacket_V759_769Factory
            },
            {
                Combine(759, 64),
                ActionBarPacket_V755_764Factory
            },
            {
                Combine(759, 65),
                WorldBorderCenterPacket_V755_769Factory
            },
            {
                Combine(759, 66),
                WorldBorderLerpSizePacket_V759_769Factory
            },
            {
                Combine(759, 67),
                WorldBorderSizePacket_V755_769Factory
            },
            {
                Combine(759, 68),
                WorldBorderWarningDelayPacket_V755_769Factory
            },
            {
                Combine(759, 69),
                WorldBorderWarningReachPacket_V755_769Factory
            },
            {
                Combine(759, 45),
                PingPacket_V755_769Factory
            },
            {
                Combine(759, 88),
                SetTitleSubtitlePacket_V755_764Factory
            },
            {
                Combine(759, 90),
                SetTitleTextPacket_V755_764Factory
            },
            {
                Combine(759, 91),
                SetTitleTimePacket_V755_769Factory
            },
            {
                Combine(759, 87),
                SimulationDistancePacket_V757_769Factory
            },
            {
                Combine(759, 12),
                ChatPreviewPacket_V759_760Factory
            },
            {
                Combine(759, 75),
                ShouldDisplayChatPreviewPacket_V759_760Factory
            },
            {
                Combine(759, 95),
                SystemChatPacket_V759Factory
            },
            {
                Combine(759, 63),
                ServerDataPacket_V759Factory
            },
            {
                Combine(760, 0),
                SpawnEntityPacket_V759_769Factory
            },
            {
                Combine(760, 1),
                SpawnEntityExperienceOrbPacket_V340_769Factory
            },
            {
                Combine(760, 3),
                AnimationPacket_V340_769Factory
            },
            {
                Combine(760, 6),
                BlockBreakAnimationPacket_V340_769Factory
            },
            {
                Combine(760, 7),
                TileEntityDataPacket_V757_763Factory
            },
            {
                Combine(760, 8),
                BlockActionPacket_V340_769Factory
            },
            {
                Combine(760, 9),
                BlockChangePacket_V340_769Factory
            },
            {
                Combine(760, 11),
                DifficultyPacket_V477_769Factory
            },
            {
                Combine(760, 16),
                CloseWindowPacket_V340_767Factory
            },
            {
                Combine(760, 17),
                WindowItemsPacket_V756_765Factory
            },
            {
                Combine(760, 18),
                CraftProgressBarPacket_V340_767Factory
            },
            {
                Combine(760, 19),
                SetSlotPacket_V756_765Factory
            },
            {
                Combine(760, 20),
                SetCooldownPacket_V340_767Factory
            },
            {
                Combine(760, 22),
                CustomPayloadPacket_V340_769Factory
            },
            {
                Combine(760, 23),
                NamedSoundEffectPacket_V759_760Factory
            },
            {
                Combine(760, 25),
                KickDisconnectPacket_V340_764Factory
            },
            {
                Combine(760, 26),
                EntityStatusPacket_V340_769Factory
            },
            {
                Combine(760, 28),
                UnloadChunkPacket_V340_763Factory
            },
            {
                Combine(760, 29),
                GameStateChangePacket_V340_769Factory
            },
            {
                Combine(760, 32),
                KeepAlivePacket_V340_769Factory
            },
            {
                Combine(760, 34),
                WorldEventPacket_V340_769Factory
            },
            {
                Combine(760, 40),
                RelEntityMovePacket_V340_769Factory
            },
            {
                Combine(760, 41),
                EntityMoveLookPacket_V340_769Factory
            },
            {
                Combine(760, 42),
                EntityLookPacket_V340_769Factory
            },
            {
                Combine(760, 43),
                VehicleMovePacket_V340_769Factory
            },
            {
                Combine(760, 46),
                OpenSignEntityPacket_V340_762Factory
            },
            {
                Combine(760, 49),
                AbilitiesPacket_V340_769Factory
            },
            {
                Combine(760, 57),
                PositionPacket_V755_761Factory
            },
            {
                Combine(760, 59),
                EntityDestroyPacket_V756_769Factory
            },
            {
                Combine(760, 60),
                RemoveEntityEffectPacket_V758_769Factory
            },
            {
                Combine(760, 61),
                ResourcePackSendPacket_V755_764Factory
            },
            {
                Combine(760, 63),
                EntityHeadRotationPacket_V340_769Factory
            },
            {
                Combine(760, 73),
                CameraPacket_V340_769Factory
            },
            {
                Combine(760, 74),
                HeldItemSlotPacket_V340_768Factory
            },
            {
                Combine(760, 79),
                ScoreboardDisplayObjectivePacket_V340_763Factory
            },
            {
                Combine(760, 81),
                AttachEntityPacket_V340_769Factory
            },
            {
                Combine(760, 82),
                EntityVelocityPacket_V340_769Factory
            },
            {
                Combine(760, 84),
                ExperiencePacket_V340_760Factory
            },
            {
                Combine(760, 85),
                UpdateHealthPacket_V340_769Factory
            },
            {
                Combine(760, 87),
                SetPassengersPacket_V340_769Factory
            },
            {
                Combine(760, 77),
                SpawnPositionPacket_V755_769Factory
            },
            {
                Combine(760, 92),
                UpdateTimePacket_V340_767Factory
            },
            {
                Combine(760, 99),
                PlayerlistHeaderPacket_V340_764Factory
            },
            {
                Combine(760, 101),
                CollectPacket_V340_769Factory
            },
            {
                Combine(760, 102),
                EntityTeleportPacket_V340_769Factory
            },
            {
                Combine(760, 105),
                EntityEffectPacket_V759_763Factory
            },
            {
                Combine(760, 65),
                SelectAdvancementTabPacket_V340_769Factory
            },
            {
                Combine(760, 100),
                NbtQueryResponsePacket_V393_763Factory
            },
            {
                Combine(760, 30),
                OpenHorseWindowPacket_V477_767Factory
            },
            {
                Combine(760, 36),
                UpdateLightPacket_V755_762Factory
            },
            {
                Combine(760, 44),
                OpenBookPacket_V477_769Factory
            },
            {
                Combine(760, 75),
                UpdateViewPositionPacket_V477_769Factory
            },
            {
                Combine(760, 76),
                UpdateViewDistancePacket_V477_769Factory
            },
            {
                Combine(760, 5),
                AcknowledgePlayerDiggingPacket_V759_769Factory
            },
            {
                Combine(760, 52),
                EndCombatEventPacket_V755_762Factory
            },
            {
                Combine(760, 53),
                EnterCombatEventPacket_V755_769Factory
            },
            {
                Combine(760, 54),
                DeathCombatEventPacket_V755_762Factory
            },
            {
                Combine(760, 13),
                ClearTitlesPacket_V755_769Factory
            },
            {
                Combine(760, 31),
                InitializeWorldBorderPacket_V759_769Factory
            },
            {
                Combine(760, 67),
                ActionBarPacket_V755_764Factory
            },
            {
                Combine(760, 68),
                WorldBorderCenterPacket_V755_769Factory
            },
            {
                Combine(760, 69),
                WorldBorderLerpSizePacket_V759_769Factory
            },
            {
                Combine(760, 70),
                WorldBorderSizePacket_V755_769Factory
            },
            {
                Combine(760, 71),
                WorldBorderWarningDelayPacket_V755_769Factory
            },
            {
                Combine(760, 72),
                WorldBorderWarningReachPacket_V755_769Factory
            },
            {
                Combine(760, 47),
                PingPacket_V755_769Factory
            },
            {
                Combine(760, 91),
                SetTitleSubtitlePacket_V755_764Factory
            },
            {
                Combine(760, 93),
                SetTitleTextPacket_V755_764Factory
            },
            {
                Combine(760, 94),
                SetTitleTimePacket_V755_769Factory
            },
            {
                Combine(760, 90),
                SimulationDistancePacket_V757_769Factory
            },
            {
                Combine(760, 12),
                ChatPreviewPacket_V759_760Factory
            },
            {
                Combine(760, 78),
                ShouldDisplayChatPreviewPacket_V759_760Factory
            },
            {
                Combine(760, 98),
                SystemChatPacket_V760_764Factory
            },
            {
                Combine(760, 66),
                ServerDataPacket_V760Factory
            },
            {
                Combine(760, 21),
                ChatSuggestionsPacket_V760_769Factory
            },
            {
                Combine(760, 50),
                MessageHeaderPacket_V760Factory
            },
            {
                Combine(761, 0),
                SpawnEntityPacket_V759_769Factory
            },
            {
                Combine(761, 1),
                SpawnEntityExperienceOrbPacket_V340_769Factory
            },
            {
                Combine(761, 3),
                AnimationPacket_V340_769Factory
            },
            {
                Combine(761, 6),
                BlockBreakAnimationPacket_V340_769Factory
            },
            {
                Combine(761, 7),
                TileEntityDataPacket_V757_763Factory
            },
            {
                Combine(761, 8),
                BlockActionPacket_V340_769Factory
            },
            {
                Combine(761, 9),
                BlockChangePacket_V340_769Factory
            },
            {
                Combine(761, 11),
                DifficultyPacket_V477_769Factory
            },
            {
                Combine(761, 15),
                CloseWindowPacket_V340_767Factory
            },
            {
                Combine(761, 16),
                WindowItemsPacket_V756_765Factory
            },
            {
                Combine(761, 17),
                CraftProgressBarPacket_V340_767Factory
            },
            {
                Combine(761, 18),
                SetSlotPacket_V756_765Factory
            },
            {
                Combine(761, 19),
                SetCooldownPacket_V340_767Factory
            },
            {
                Combine(761, 21),
                CustomPayloadPacket_V340_769Factory
            },
            {
                Combine(761, 23),
                KickDisconnectPacket_V340_764Factory
            },
            {
                Combine(761, 25),
                EntityStatusPacket_V340_769Factory
            },
            {
                Combine(761, 27),
                UnloadChunkPacket_V340_763Factory
            },
            {
                Combine(761, 28),
                GameStateChangePacket_V340_769Factory
            },
            {
                Combine(761, 31),
                KeepAlivePacket_V340_769Factory
            },
            {
                Combine(761, 33),
                WorldEventPacket_V340_769Factory
            },
            {
                Combine(761, 39),
                RelEntityMovePacket_V340_769Factory
            },
            {
                Combine(761, 40),
                EntityMoveLookPacket_V340_769Factory
            },
            {
                Combine(761, 41),
                EntityLookPacket_V340_769Factory
            },
            {
                Combine(761, 42),
                VehicleMovePacket_V340_769Factory
            },
            {
                Combine(761, 45),
                OpenSignEntityPacket_V340_762Factory
            },
            {
                Combine(761, 48),
                AbilitiesPacket_V340_769Factory
            },
            {
                Combine(761, 56),
                PositionPacket_V755_761Factory
            },
            {
                Combine(761, 58),
                EntityDestroyPacket_V756_769Factory
            },
            {
                Combine(761, 59),
                RemoveEntityEffectPacket_V758_769Factory
            },
            {
                Combine(761, 60),
                ResourcePackSendPacket_V755_764Factory
            },
            {
                Combine(761, 62),
                EntityHeadRotationPacket_V340_769Factory
            },
            {
                Combine(761, 72),
                CameraPacket_V340_769Factory
            },
            {
                Combine(761, 73),
                HeldItemSlotPacket_V340_768Factory
            },
            {
                Combine(761, 77),
                ScoreboardDisplayObjectivePacket_V340_763Factory
            },
            {
                Combine(761, 79),
                AttachEntityPacket_V340_769Factory
            },
            {
                Combine(761, 80),
                EntityVelocityPacket_V340_769Factory
            },
            {
                Combine(761, 82),
                ExperiencePacket_V761_763Factory
            },
            {
                Combine(761, 83),
                UpdateHealthPacket_V340_769Factory
            },
            {
                Combine(761, 85),
                SetPassengersPacket_V340_769Factory
            },
            {
                Combine(761, 76),
                SpawnPositionPacket_V755_769Factory
            },
            {
                Combine(761, 90),
                UpdateTimePacket_V340_767Factory
            },
            {
                Combine(761, 97),
                PlayerlistHeaderPacket_V340_764Factory
            },
            {
                Combine(761, 99),
                CollectPacket_V340_769Factory
            },
            {
                Combine(761, 100),
                EntityTeleportPacket_V340_769Factory
            },
            {
                Combine(761, 104),
                EntityEffectPacket_V759_763Factory
            },
            {
                Combine(761, 64),
                SelectAdvancementTabPacket_V340_769Factory
            },
            {
                Combine(761, 98),
                NbtQueryResponsePacket_V393_763Factory
            },
            {
                Combine(761, 29),
                OpenHorseWindowPacket_V477_767Factory
            },
            {
                Combine(761, 35),
                UpdateLightPacket_V755_762Factory
            },
            {
                Combine(761, 43),
                OpenBookPacket_V477_769Factory
            },
            {
                Combine(761, 74),
                UpdateViewPositionPacket_V477_769Factory
            },
            {
                Combine(761, 75),
                UpdateViewDistancePacket_V477_769Factory
            },
            {
                Combine(761, 5),
                AcknowledgePlayerDiggingPacket_V759_769Factory
            },
            {
                Combine(761, 50),
                EndCombatEventPacket_V755_762Factory
            },
            {
                Combine(761, 51),
                EnterCombatEventPacket_V755_769Factory
            },
            {
                Combine(761, 52),
                DeathCombatEventPacket_V755_762Factory
            },
            {
                Combine(761, 12),
                ClearTitlesPacket_V755_769Factory
            },
            {
                Combine(761, 30),
                InitializeWorldBorderPacket_V759_769Factory
            },
            {
                Combine(761, 66),
                ActionBarPacket_V755_764Factory
            },
            {
                Combine(761, 67),
                WorldBorderCenterPacket_V755_769Factory
            },
            {
                Combine(761, 68),
                WorldBorderLerpSizePacket_V759_769Factory
            },
            {
                Combine(761, 69),
                WorldBorderSizePacket_V755_769Factory
            },
            {
                Combine(761, 70),
                WorldBorderWarningDelayPacket_V755_769Factory
            },
            {
                Combine(761, 71),
                WorldBorderWarningReachPacket_V755_769Factory
            },
            {
                Combine(761, 46),
                PingPacket_V755_769Factory
            },
            {
                Combine(761, 89),
                SetTitleSubtitlePacket_V755_764Factory
            },
            {
                Combine(761, 91),
                SetTitleTextPacket_V755_764Factory
            },
            {
                Combine(761, 92),
                SetTitleTimePacket_V755_769Factory
            },
            {
                Combine(761, 88),
                SimulationDistancePacket_V757_769Factory
            },
            {
                Combine(761, 96),
                SystemChatPacket_V760_764Factory
            },
            {
                Combine(761, 65),
                ServerDataPacket_V761Factory
            },
            {
                Combine(761, 20),
                ChatSuggestionsPacket_V760_769Factory
            },
            {
                Combine(761, 53),
                PlayerRemovePacket_V761_769Factory
            },
            {
                Combine(761, 103),
                FeatureFlagsPacket_V761_763Factory
            },
            {
                Combine(762, 1),
                SpawnEntityPacket_V759_769Factory
            },
            {
                Combine(762, 2),
                SpawnEntityExperienceOrbPacket_V340_769Factory
            },
            {
                Combine(762, 4),
                AnimationPacket_V340_769Factory
            },
            {
                Combine(762, 7),
                BlockBreakAnimationPacket_V340_769Factory
            },
            {
                Combine(762, 8),
                TileEntityDataPacket_V757_763Factory
            },
            {
                Combine(762, 9),
                BlockActionPacket_V340_769Factory
            },
            {
                Combine(762, 10),
                BlockChangePacket_V340_769Factory
            },
            {
                Combine(762, 12),
                DifficultyPacket_V477_769Factory
            },
            {
                Combine(762, 17),
                CloseWindowPacket_V340_767Factory
            },
            {
                Combine(762, 18),
                WindowItemsPacket_V756_765Factory
            },
            {
                Combine(762, 19),
                CraftProgressBarPacket_V340_767Factory
            },
            {
                Combine(762, 20),
                SetSlotPacket_V756_765Factory
            },
            {
                Combine(762, 21),
                SetCooldownPacket_V340_767Factory
            },
            {
                Combine(762, 23),
                CustomPayloadPacket_V340_769Factory
            },
            {
                Combine(762, 26),
                KickDisconnectPacket_V340_764Factory
            },
            {
                Combine(762, 28),
                EntityStatusPacket_V340_769Factory
            },
            {
                Combine(762, 30),
                UnloadChunkPacket_V340_763Factory
            },
            {
                Combine(762, 31),
                GameStateChangePacket_V340_769Factory
            },
            {
                Combine(762, 35),
                KeepAlivePacket_V340_769Factory
            },
            {
                Combine(762, 37),
                WorldEventPacket_V340_769Factory
            },
            {
                Combine(762, 43),
                RelEntityMovePacket_V340_769Factory
            },
            {
                Combine(762, 44),
                EntityMoveLookPacket_V340_769Factory
            },
            {
                Combine(762, 45),
                EntityLookPacket_V340_769Factory
            },
            {
                Combine(762, 46),
                VehicleMovePacket_V340_769Factory
            },
            {
                Combine(762, 49),
                OpenSignEntityPacket_V340_762Factory
            },
            {
                Combine(762, 52),
                AbilitiesPacket_V340_769Factory
            },
            {
                Combine(762, 60),
                PositionPacket_V762_767Factory
            },
            {
                Combine(762, 62),
                EntityDestroyPacket_V756_769Factory
            },
            {
                Combine(762, 63),
                RemoveEntityEffectPacket_V758_769Factory
            },
            {
                Combine(762, 64),
                ResourcePackSendPacket_V755_764Factory
            },
            {
                Combine(762, 66),
                EntityHeadRotationPacket_V340_769Factory
            },
            {
                Combine(762, 76),
                CameraPacket_V340_769Factory
            },
            {
                Combine(762, 77),
                HeldItemSlotPacket_V340_768Factory
            },
            {
                Combine(762, 81),
                ScoreboardDisplayObjectivePacket_V340_763Factory
            },
            {
                Combine(762, 83),
                AttachEntityPacket_V340_769Factory
            },
            {
                Combine(762, 84),
                EntityVelocityPacket_V340_769Factory
            },
            {
                Combine(762, 86),
                ExperiencePacket_V761_763Factory
            },
            {
                Combine(762, 87),
                UpdateHealthPacket_V340_769Factory
            },
            {
                Combine(762, 89),
                SetPassengersPacket_V340_769Factory
            },
            {
                Combine(762, 80),
                SpawnPositionPacket_V755_769Factory
            },
            {
                Combine(762, 94),
                UpdateTimePacket_V340_767Factory
            },
            {
                Combine(762, 101),
                PlayerlistHeaderPacket_V340_764Factory
            },
            {
                Combine(762, 103),
                CollectPacket_V340_769Factory
            },
            {
                Combine(762, 104),
                EntityTeleportPacket_V340_769Factory
            },
            {
                Combine(762, 108),
                EntityEffectPacket_V759_763Factory
            },
            {
                Combine(762, 68),
                SelectAdvancementTabPacket_V340_769Factory
            },
            {
                Combine(762, 102),
                NbtQueryResponsePacket_V393_763Factory
            },
            {
                Combine(762, 32),
                OpenHorseWindowPacket_V477_767Factory
            },
            {
                Combine(762, 39),
                UpdateLightPacket_V755_762Factory
            },
            {
                Combine(762, 47),
                OpenBookPacket_V477_769Factory
            },
            {
                Combine(762, 78),
                UpdateViewPositionPacket_V477_769Factory
            },
            {
                Combine(762, 79),
                UpdateViewDistancePacket_V477_769Factory
            },
            {
                Combine(762, 6),
                AcknowledgePlayerDiggingPacket_V759_769Factory
            },
            {
                Combine(762, 54),
                EndCombatEventPacket_V755_762Factory
            },
            {
                Combine(762, 55),
                EnterCombatEventPacket_V755_769Factory
            },
            {
                Combine(762, 56),
                DeathCombatEventPacket_V755_762Factory
            },
            {
                Combine(762, 14),
                ClearTitlesPacket_V755_769Factory
            },
            {
                Combine(762, 34),
                InitializeWorldBorderPacket_V759_769Factory
            },
            {
                Combine(762, 70),
                ActionBarPacket_V755_764Factory
            },
            {
                Combine(762, 71),
                WorldBorderCenterPacket_V755_769Factory
            },
            {
                Combine(762, 72),
                WorldBorderLerpSizePacket_V759_769Factory
            },
            {
                Combine(762, 73),
                WorldBorderSizePacket_V755_769Factory
            },
            {
                Combine(762, 74),
                WorldBorderWarningDelayPacket_V755_769Factory
            },
            {
                Combine(762, 75),
                WorldBorderWarningReachPacket_V755_769Factory
            },
            {
                Combine(762, 50),
                PingPacket_V755_769Factory
            },
            {
                Combine(762, 93),
                SetTitleSubtitlePacket_V755_764Factory
            },
            {
                Combine(762, 95),
                SetTitleTextPacket_V755_764Factory
            },
            {
                Combine(762, 96),
                SetTitleTimePacket_V755_769Factory
            },
            {
                Combine(762, 92),
                SimulationDistancePacket_V757_769Factory
            },
            {
                Combine(762, 100),
                SystemChatPacket_V760_764Factory
            },
            {
                Combine(762, 69),
                ServerDataPacket_V762_764Factory
            },
            {
                Combine(762, 22),
                ChatSuggestionsPacket_V760_769Factory
            },
            {
                Combine(762, 57),
                PlayerRemovePacket_V761_769Factory
            },
            {
                Combine(762, 107),
                FeatureFlagsPacket_V761_763Factory
            },
            {
                Combine(762, 24),
                DamageEventPacket_V762_769Factory
            },
            {
                Combine(762, 33),
                HurtAnimationPacket_V762_769Factory
            },
            {
                Combine(763, 1),
                SpawnEntityPacket_V759_769Factory
            },
            {
                Combine(763, 2),
                SpawnEntityExperienceOrbPacket_V340_769Factory
            },
            {
                Combine(763, 4),
                AnimationPacket_V340_769Factory
            },
            {
                Combine(763, 7),
                BlockBreakAnimationPacket_V340_769Factory
            },
            {
                Combine(763, 8),
                TileEntityDataPacket_V757_763Factory
            },
            {
                Combine(763, 9),
                BlockActionPacket_V340_769Factory
            },
            {
                Combine(763, 10),
                BlockChangePacket_V340_769Factory
            },
            {
                Combine(763, 12),
                DifficultyPacket_V477_769Factory
            },
            {
                Combine(763, 17),
                CloseWindowPacket_V340_767Factory
            },
            {
                Combine(763, 18),
                WindowItemsPacket_V756_765Factory
            },
            {
                Combine(763, 19),
                CraftProgressBarPacket_V340_767Factory
            },
            {
                Combine(763, 20),
                SetSlotPacket_V756_765Factory
            },
            {
                Combine(763, 21),
                SetCooldownPacket_V340_767Factory
            },
            {
                Combine(763, 23),
                CustomPayloadPacket_V340_769Factory
            },
            {
                Combine(763, 26),
                KickDisconnectPacket_V340_764Factory
            },
            {
                Combine(763, 28),
                EntityStatusPacket_V340_769Factory
            },
            {
                Combine(763, 30),
                UnloadChunkPacket_V340_763Factory
            },
            {
                Combine(763, 31),
                GameStateChangePacket_V340_769Factory
            },
            {
                Combine(763, 35),
                KeepAlivePacket_V340_769Factory
            },
            {
                Combine(763, 37),
                WorldEventPacket_V340_769Factory
            },
            {
                Combine(763, 43),
                RelEntityMovePacket_V340_769Factory
            },
            {
                Combine(763, 44),
                EntityMoveLookPacket_V340_769Factory
            },
            {
                Combine(763, 45),
                EntityLookPacket_V340_769Factory
            },
            {
                Combine(763, 46),
                VehicleMovePacket_V340_769Factory
            },
            {
                Combine(763, 49),
                OpenSignEntityPacket_V763_769Factory
            },
            {
                Combine(763, 52),
                AbilitiesPacket_V340_769Factory
            },
            {
                Combine(763, 60),
                PositionPacket_V762_767Factory
            },
            {
                Combine(763, 62),
                EntityDestroyPacket_V756_769Factory
            },
            {
                Combine(763, 63),
                RemoveEntityEffectPacket_V758_769Factory
            },
            {
                Combine(763, 64),
                ResourcePackSendPacket_V755_764Factory
            },
            {
                Combine(763, 66),
                EntityHeadRotationPacket_V340_769Factory
            },
            {
                Combine(763, 76),
                CameraPacket_V340_769Factory
            },
            {
                Combine(763, 77),
                HeldItemSlotPacket_V340_768Factory
            },
            {
                Combine(763, 81),
                ScoreboardDisplayObjectivePacket_V340_763Factory
            },
            {
                Combine(763, 83),
                AttachEntityPacket_V340_769Factory
            },
            {
                Combine(763, 84),
                EntityVelocityPacket_V340_769Factory
            },
            {
                Combine(763, 86),
                ExperiencePacket_V761_763Factory
            },
            {
                Combine(763, 87),
                UpdateHealthPacket_V340_769Factory
            },
            {
                Combine(763, 89),
                SetPassengersPacket_V340_769Factory
            },
            {
                Combine(763, 80),
                SpawnPositionPacket_V755_769Factory
            },
            {
                Combine(763, 94),
                UpdateTimePacket_V340_767Factory
            },
            {
                Combine(763, 101),
                PlayerlistHeaderPacket_V340_764Factory
            },
            {
                Combine(763, 103),
                CollectPacket_V340_769Factory
            },
            {
                Combine(763, 104),
                EntityTeleportPacket_V340_769Factory
            },
            {
                Combine(763, 108),
                EntityEffectPacket_V759_763Factory
            },
            {
                Combine(763, 68),
                SelectAdvancementTabPacket_V340_769Factory
            },
            {
                Combine(763, 102),
                NbtQueryResponsePacket_V393_763Factory
            },
            {
                Combine(763, 32),
                OpenHorseWindowPacket_V477_767Factory
            },
            {
                Combine(763, 39),
                UpdateLightPacket_V763_769Factory
            },
            {
                Combine(763, 47),
                OpenBookPacket_V477_769Factory
            },
            {
                Combine(763, 78),
                UpdateViewPositionPacket_V477_769Factory
            },
            {
                Combine(763, 79),
                UpdateViewDistancePacket_V477_769Factory
            },
            {
                Combine(763, 6),
                AcknowledgePlayerDiggingPacket_V759_769Factory
            },
            {
                Combine(763, 54),
                EndCombatEventPacket_V763_769Factory
            },
            {
                Combine(763, 55),
                EnterCombatEventPacket_V755_769Factory
            },
            {
                Combine(763, 56),
                DeathCombatEventPacket_V763_764Factory
            },
            {
                Combine(763, 14),
                ClearTitlesPacket_V755_769Factory
            },
            {
                Combine(763, 34),
                InitializeWorldBorderPacket_V759_769Factory
            },
            {
                Combine(763, 70),
                ActionBarPacket_V755_764Factory
            },
            {
                Combine(763, 71),
                WorldBorderCenterPacket_V755_769Factory
            },
            {
                Combine(763, 72),
                WorldBorderLerpSizePacket_V759_769Factory
            },
            {
                Combine(763, 73),
                WorldBorderSizePacket_V755_769Factory
            },
            {
                Combine(763, 74),
                WorldBorderWarningDelayPacket_V755_769Factory
            },
            {
                Combine(763, 75),
                WorldBorderWarningReachPacket_V755_769Factory
            },
            {
                Combine(763, 50),
                PingPacket_V755_769Factory
            },
            {
                Combine(763, 93),
                SetTitleSubtitlePacket_V755_764Factory
            },
            {
                Combine(763, 95),
                SetTitleTextPacket_V755_764Factory
            },
            {
                Combine(763, 96),
                SetTitleTimePacket_V755_769Factory
            },
            {
                Combine(763, 92),
                SimulationDistancePacket_V757_769Factory
            },
            {
                Combine(763, 100),
                SystemChatPacket_V760_764Factory
            },
            {
                Combine(763, 69),
                ServerDataPacket_V762_764Factory
            },
            {
                Combine(763, 22),
                ChatSuggestionsPacket_V760_769Factory
            },
            {
                Combine(763, 57),
                PlayerRemovePacket_V761_769Factory
            },
            {
                Combine(763, 107),
                FeatureFlagsPacket_V761_763Factory
            },
            {
                Combine(763, 24),
                DamageEventPacket_V762_769Factory
            },
            {
                Combine(763, 33),
                HurtAnimationPacket_V762_769Factory
            },
            {
                Combine(764, 1),
                SpawnEntityPacket_V759_769Factory
            },
            {
                Combine(764, 2),
                SpawnEntityExperienceOrbPacket_V340_769Factory
            },
            {
                Combine(764, 3),
                AnimationPacket_V340_769Factory
            },
            {
                Combine(764, 6),
                BlockBreakAnimationPacket_V340_769Factory
            },
            {
                Combine(764, 7),
                TileEntityDataPacket_V764_769Factory
            },
            {
                Combine(764, 8),
                BlockActionPacket_V340_769Factory
            },
            {
                Combine(764, 9),
                BlockChangePacket_V340_769Factory
            },
            {
                Combine(764, 11),
                DifficultyPacket_V477_769Factory
            },
            {
                Combine(764, 18),
                CloseWindowPacket_V340_767Factory
            },
            {
                Combine(764, 19),
                WindowItemsPacket_V756_765Factory
            },
            {
                Combine(764, 20),
                CraftProgressBarPacket_V340_767Factory
            },
            {
                Combine(764, 21),
                SetSlotPacket_V756_765Factory
            },
            {
                Combine(764, 22),
                SetCooldownPacket_V340_767Factory
            },
            {
                Combine(764, 24),
                CustomPayloadPacket_V340_769Factory
            },
            {
                Combine(764, 27),
                KickDisconnectPacket_V340_764Factory
            },
            {
                Combine(764, 29),
                EntityStatusPacket_V340_769Factory
            },
            {
                Combine(764, 31),
                UnloadChunkPacket_V764_769Factory
            },
            {
                Combine(764, 32),
                GameStateChangePacket_V340_769Factory
            },
            {
                Combine(764, 36),
                KeepAlivePacket_V340_769Factory
            },
            {
                Combine(764, 38),
                WorldEventPacket_V340_769Factory
            },
            {
                Combine(764, 44),
                RelEntityMovePacket_V340_769Factory
            },
            {
                Combine(764, 45),
                EntityMoveLookPacket_V340_769Factory
            },
            {
                Combine(764, 46),
                EntityLookPacket_V340_769Factory
            },
            {
                Combine(764, 47),
                VehicleMovePacket_V340_769Factory
            },
            {
                Combine(764, 50),
                OpenSignEntityPacket_V763_769Factory
            },
            {
                Combine(764, 54),
                AbilitiesPacket_V340_769Factory
            },
            {
                Combine(764, 62),
                PositionPacket_V762_767Factory
            },
            {
                Combine(764, 64),
                EntityDestroyPacket_V756_769Factory
            },
            {
                Combine(764, 65),
                RemoveEntityEffectPacket_V758_769Factory
            },
            {
                Combine(764, 66),
                ResourcePackSendPacket_V755_764Factory
            },
            {
                Combine(764, 68),
                EntityHeadRotationPacket_V340_769Factory
            },
            {
                Combine(764, 78),
                CameraPacket_V340_769Factory
            },
            {
                Combine(764, 79),
                HeldItemSlotPacket_V340_768Factory
            },
            {
                Combine(764, 83),
                ScoreboardDisplayObjectivePacket_V764_769Factory
            },
            {
                Combine(764, 85),
                AttachEntityPacket_V340_769Factory
            },
            {
                Combine(764, 86),
                EntityVelocityPacket_V340_769Factory
            },
            {
                Combine(764, 88),
                ExperiencePacket_V764_769Factory
            },
            {
                Combine(764, 89),
                UpdateHealthPacket_V340_769Factory
            },
            {
                Combine(764, 91),
                SetPassengersPacket_V340_769Factory
            },
            {
                Combine(764, 82),
                SpawnPositionPacket_V755_769Factory
            },
            {
                Combine(764, 96),
                UpdateTimePacket_V340_767Factory
            },
            {
                Combine(764, 104),
                PlayerlistHeaderPacket_V340_764Factory
            },
            {
                Combine(764, 106),
                CollectPacket_V340_769Factory
            },
            {
                Combine(764, 107),
                EntityTeleportPacket_V340_769Factory
            },
            {
                Combine(764, 110),
                EntityEffectPacket_V764_765Factory
            },
            {
                Combine(764, 70),
                SelectAdvancementTabPacket_V340_769Factory
            },
            {
                Combine(764, 105),
                NbtQueryResponsePacket_V764_769Factory
            },
            {
                Combine(764, 33),
                OpenHorseWindowPacket_V477_767Factory
            },
            {
                Combine(764, 40),
                UpdateLightPacket_V763_769Factory
            },
            {
                Combine(764, 48),
                OpenBookPacket_V477_769Factory
            },
            {
                Combine(764, 80),
                UpdateViewPositionPacket_V477_769Factory
            },
            {
                Combine(764, 81),
                UpdateViewDistancePacket_V477_769Factory
            },
            {
                Combine(764, 5),
                AcknowledgePlayerDiggingPacket_V759_769Factory
            },
            {
                Combine(764, 56),
                EndCombatEventPacket_V763_769Factory
            },
            {
                Combine(764, 57),
                EnterCombatEventPacket_V755_769Factory
            },
            {
                Combine(764, 58),
                DeathCombatEventPacket_V763_764Factory
            },
            {
                Combine(764, 15),
                ClearTitlesPacket_V755_769Factory
            },
            {
                Combine(764, 35),
                InitializeWorldBorderPacket_V759_769Factory
            },
            {
                Combine(764, 72),
                ActionBarPacket_V755_764Factory
            },
            {
                Combine(764, 73),
                WorldBorderCenterPacket_V755_769Factory
            },
            {
                Combine(764, 74),
                WorldBorderLerpSizePacket_V759_769Factory
            },
            {
                Combine(764, 75),
                WorldBorderSizePacket_V755_769Factory
            },
            {
                Combine(764, 76),
                WorldBorderWarningDelayPacket_V755_769Factory
            },
            {
                Combine(764, 77),
                WorldBorderWarningReachPacket_V755_769Factory
            },
            {
                Combine(764, 51),
                PingPacket_V755_769Factory
            },
            {
                Combine(764, 95),
                SetTitleSubtitlePacket_V755_764Factory
            },
            {
                Combine(764, 97),
                SetTitleTextPacket_V755_764Factory
            },
            {
                Combine(764, 98),
                SetTitleTimePacket_V755_769Factory
            },
            {
                Combine(764, 94),
                SimulationDistancePacket_V757_769Factory
            },
            {
                Combine(764, 103),
                SystemChatPacket_V760_764Factory
            },
            {
                Combine(764, 71),
                ServerDataPacket_V762_764Factory
            },
            {
                Combine(764, 23),
                ChatSuggestionsPacket_V760_769Factory
            },
            {
                Combine(764, 59),
                PlayerRemovePacket_V761_769Factory
            },
            {
                Combine(764, 25),
                DamageEventPacket_V762_769Factory
            },
            {
                Combine(764, 34),
                HurtAnimationPacket_V762_769Factory
            },
            {
                Combine(764, 12),
                ChunkBatchFinishedPacket_V764_769Factory
            },
            {
                Combine(764, 13),
                ChunkBatchStartPacket_V764_769Factory
            },
            {
                Combine(764, 52),
                PingResponsePacket_V764_769Factory
            },
            {
                Combine(764, 101),
                StartConfigurationPacket_V764_769Factory
            },
            {
                Combine(765, 1),
                SpawnEntityPacket_V759_769Factory
            },
            {
                Combine(765, 2),
                SpawnEntityExperienceOrbPacket_V340_769Factory
            },
            {
                Combine(765, 3),
                AnimationPacket_V340_769Factory
            },
            {
                Combine(765, 6),
                BlockBreakAnimationPacket_V340_769Factory
            },
            {
                Combine(765, 7),
                TileEntityDataPacket_V764_769Factory
            },
            {
                Combine(765, 8),
                BlockActionPacket_V340_769Factory
            },
            {
                Combine(765, 9),
                BlockChangePacket_V340_769Factory
            },
            {
                Combine(765, 11),
                DifficultyPacket_V477_769Factory
            },
            {
                Combine(765, 18),
                CloseWindowPacket_V340_767Factory
            },
            {
                Combine(765, 19),
                WindowItemsPacket_V756_765Factory
            },
            {
                Combine(765, 20),
                CraftProgressBarPacket_V340_767Factory
            },
            {
                Combine(765, 21),
                SetSlotPacket_V756_765Factory
            },
            {
                Combine(765, 22),
                SetCooldownPacket_V340_767Factory
            },
            {
                Combine(765, 24),
                CustomPayloadPacket_V340_769Factory
            },
            {
                Combine(765, 27),
                KickDisconnectPacket_V765_769Factory
            },
            {
                Combine(765, 29),
                EntityStatusPacket_V340_769Factory
            },
            {
                Combine(765, 31),
                UnloadChunkPacket_V764_769Factory
            },
            {
                Combine(765, 32),
                GameStateChangePacket_V340_769Factory
            },
            {
                Combine(765, 36),
                KeepAlivePacket_V340_769Factory
            },
            {
                Combine(765, 38),
                WorldEventPacket_V340_769Factory
            },
            {
                Combine(765, 44),
                RelEntityMovePacket_V340_769Factory
            },
            {
                Combine(765, 45),
                EntityMoveLookPacket_V340_769Factory
            },
            {
                Combine(765, 46),
                EntityLookPacket_V340_769Factory
            },
            {
                Combine(765, 47),
                VehicleMovePacket_V340_769Factory
            },
            {
                Combine(765, 50),
                OpenSignEntityPacket_V763_769Factory
            },
            {
                Combine(765, 54),
                AbilitiesPacket_V340_769Factory
            },
            {
                Combine(765, 62),
                PositionPacket_V762_767Factory
            },
            {
                Combine(765, 64),
                EntityDestroyPacket_V756_769Factory
            },
            {
                Combine(765, 65),
                RemoveEntityEffectPacket_V758_769Factory
            },
            {
                Combine(765, 70),
                EntityHeadRotationPacket_V340_769Factory
            },
            {
                Combine(765, 80),
                CameraPacket_V340_769Factory
            },
            {
                Combine(765, 81),
                HeldItemSlotPacket_V340_768Factory
            },
            {
                Combine(765, 85),
                ScoreboardDisplayObjectivePacket_V764_769Factory
            },
            {
                Combine(765, 87),
                AttachEntityPacket_V340_769Factory
            },
            {
                Combine(765, 88),
                EntityVelocityPacket_V340_769Factory
            },
            {
                Combine(765, 90),
                ExperiencePacket_V764_769Factory
            },
            {
                Combine(765, 91),
                UpdateHealthPacket_V340_769Factory
            },
            {
                Combine(765, 93),
                SetPassengersPacket_V340_769Factory
            },
            {
                Combine(765, 84),
                SpawnPositionPacket_V755_769Factory
            },
            {
                Combine(765, 98),
                UpdateTimePacket_V340_767Factory
            },
            {
                Combine(765, 106),
                PlayerlistHeaderPacket_V765_769Factory
            },
            {
                Combine(765, 108),
                CollectPacket_V340_769Factory
            },
            {
                Combine(765, 109),
                EntityTeleportPacket_V340_769Factory
            },
            {
                Combine(765, 114),
                EntityEffectPacket_V764_765Factory
            },
            {
                Combine(765, 72),
                SelectAdvancementTabPacket_V340_769Factory
            },
            {
                Combine(765, 107),
                NbtQueryResponsePacket_V764_769Factory
            },
            {
                Combine(765, 33),
                OpenHorseWindowPacket_V477_767Factory
            },
            {
                Combine(765, 40),
                UpdateLightPacket_V763_769Factory
            },
            {
                Combine(765, 48),
                OpenBookPacket_V477_769Factory
            },
            {
                Combine(765, 82),
                UpdateViewPositionPacket_V477_769Factory
            },
            {
                Combine(765, 83),
                UpdateViewDistancePacket_V477_769Factory
            },
            {
                Combine(765, 5),
                AcknowledgePlayerDiggingPacket_V759_769Factory
            },
            {
                Combine(765, 56),
                EndCombatEventPacket_V763_769Factory
            },
            {
                Combine(765, 57),
                EnterCombatEventPacket_V755_769Factory
            },
            {
                Combine(765, 58),
                DeathCombatEventPacket_V765_769Factory
            },
            {
                Combine(765, 15),
                ClearTitlesPacket_V755_769Factory
            },
            {
                Combine(765, 35),
                InitializeWorldBorderPacket_V759_769Factory
            },
            {
                Combine(765, 74),
                ActionBarPacket_V765_769Factory
            },
            {
                Combine(765, 75),
                WorldBorderCenterPacket_V755_769Factory
            },
            {
                Combine(765, 76),
                WorldBorderLerpSizePacket_V759_769Factory
            },
            {
                Combine(765, 77),
                WorldBorderSizePacket_V755_769Factory
            },
            {
                Combine(765, 78),
                WorldBorderWarningDelayPacket_V755_769Factory
            },
            {
                Combine(765, 79),
                WorldBorderWarningReachPacket_V755_769Factory
            },
            {
                Combine(765, 51),
                PingPacket_V755_769Factory
            },
            {
                Combine(765, 97),
                SetTitleSubtitlePacket_V765_769Factory
            },
            {
                Combine(765, 99),
                SetTitleTextPacket_V765_769Factory
            },
            {
                Combine(765, 100),
                SetTitleTimePacket_V755_769Factory
            },
            {
                Combine(765, 96),
                SimulationDistancePacket_V757_769Factory
            },
            {
                Combine(765, 105),
                SystemChatPacket_V765_769Factory
            },
            {
                Combine(765, 73),
                ServerDataPacket_V765Factory
            },
            {
                Combine(765, 23),
                ChatSuggestionsPacket_V760_769Factory
            },
            {
                Combine(765, 59),
                PlayerRemovePacket_V761_769Factory
            },
            {
                Combine(765, 25),
                DamageEventPacket_V762_769Factory
            },
            {
                Combine(765, 34),
                HurtAnimationPacket_V762_769Factory
            },
            {
                Combine(765, 12),
                ChunkBatchFinishedPacket_V764_769Factory
            },
            {
                Combine(765, 13),
                ChunkBatchStartPacket_V764_769Factory
            },
            {
                Combine(765, 52),
                PingResponsePacket_V764_769Factory
            },
            {
                Combine(765, 103),
                StartConfigurationPacket_V764_769Factory
            },
            {
                Combine(765, 66),
                ResetScorePacket_V765_769Factory
            },
            {
                Combine(765, 67),
                RemoveResourcePackPacket_V765_767Factory
            },
            {
                Combine(765, 68),
                AddResourcePackPacket_V765_767Factory
            },
            {
                Combine(765, 110),
                SetTickingStatePacket_V765_769Factory
            },
            {
                Combine(765, 111),
                StepTickPacket_V765_769Factory
            },
            {
                Combine(766, 1),
                SpawnEntityPacket_V759_769Factory
            },
            {
                Combine(766, 2),
                SpawnEntityExperienceOrbPacket_V340_769Factory
            },
            {
                Combine(766, 3),
                AnimationPacket_V340_769Factory
            },
            {
                Combine(766, 6),
                BlockBreakAnimationPacket_V340_769Factory
            },
            {
                Combine(766, 7),
                TileEntityDataPacket_V764_769Factory
            },
            {
                Combine(766, 8),
                BlockActionPacket_V340_769Factory
            },
            {
                Combine(766, 9),
                BlockChangePacket_V340_769Factory
            },
            {
                Combine(766, 11),
                DifficultyPacket_V477_769Factory
            },
            {
                Combine(766, 18),
                CloseWindowPacket_V340_767Factory
            },
            {
                Combine(766, 19),
                WindowItemsPacket_V766_767Factory
            },
            {
                Combine(766, 20),
                CraftProgressBarPacket_V340_767Factory
            },
            {
                Combine(766, 21),
                SetSlotPacket_V766_767Factory
            },
            {
                Combine(766, 23),
                SetCooldownPacket_V340_767Factory
            },
            {
                Combine(766, 25),
                CustomPayloadPacket_V340_769Factory
            },
            {
                Combine(766, 29),
                KickDisconnectPacket_V765_769Factory
            },
            {
                Combine(766, 31),
                EntityStatusPacket_V340_769Factory
            },
            {
                Combine(766, 33),
                UnloadChunkPacket_V764_769Factory
            },
            {
                Combine(766, 34),
                GameStateChangePacket_V340_769Factory
            },
            {
                Combine(766, 38),
                KeepAlivePacket_V340_769Factory
            },
            {
                Combine(766, 40),
                WorldEventPacket_V340_769Factory
            },
            {
                Combine(766, 46),
                RelEntityMovePacket_V340_769Factory
            },
            {
                Combine(766, 47),
                EntityMoveLookPacket_V340_769Factory
            },
            {
                Combine(766, 48),
                EntityLookPacket_V340_769Factory
            },
            {
                Combine(766, 49),
                VehicleMovePacket_V340_769Factory
            },
            {
                Combine(766, 52),
                OpenSignEntityPacket_V763_769Factory
            },
            {
                Combine(766, 56),
                AbilitiesPacket_V340_769Factory
            },
            {
                Combine(766, 64),
                PositionPacket_V762_767Factory
            },
            {
                Combine(766, 66),
                EntityDestroyPacket_V756_769Factory
            },
            {
                Combine(766, 67),
                RemoveEntityEffectPacket_V758_769Factory
            },
            {
                Combine(766, 72),
                EntityHeadRotationPacket_V340_769Factory
            },
            {
                Combine(766, 82),
                CameraPacket_V340_769Factory
            },
            {
                Combine(766, 83),
                HeldItemSlotPacket_V340_768Factory
            },
            {
                Combine(766, 87),
                ScoreboardDisplayObjectivePacket_V764_769Factory
            },
            {
                Combine(766, 89),
                AttachEntityPacket_V340_769Factory
            },
            {
                Combine(766, 90),
                EntityVelocityPacket_V340_769Factory
            },
            {
                Combine(766, 92),
                ExperiencePacket_V764_769Factory
            },
            {
                Combine(766, 93),
                UpdateHealthPacket_V340_769Factory
            },
            {
                Combine(766, 95),
                SetPassengersPacket_V340_769Factory
            },
            {
                Combine(766, 86),
                SpawnPositionPacket_V755_769Factory
            },
            {
                Combine(766, 100),
                UpdateTimePacket_V340_767Factory
            },
            {
                Combine(766, 109),
                PlayerlistHeaderPacket_V765_769Factory
            },
            {
                Combine(766, 111),
                CollectPacket_V340_769Factory
            },
            {
                Combine(766, 112),
                EntityTeleportPacket_V340_769Factory
            },
            {
                Combine(766, 118),
                EntityEffectPacket_V766_769Factory
            },
            {
                Combine(766, 74),
                SelectAdvancementTabPacket_V340_769Factory
            },
            {
                Combine(766, 110),
                NbtQueryResponsePacket_V764_769Factory
            },
            {
                Combine(766, 35),
                OpenHorseWindowPacket_V477_767Factory
            },
            {
                Combine(766, 42),
                UpdateLightPacket_V763_769Factory
            },
            {
                Combine(766, 50),
                OpenBookPacket_V477_769Factory
            },
            {
                Combine(766, 84),
                UpdateViewPositionPacket_V477_769Factory
            },
            {
                Combine(766, 85),
                UpdateViewDistancePacket_V477_769Factory
            },
            {
                Combine(766, 5),
                AcknowledgePlayerDiggingPacket_V759_769Factory
            },
            {
                Combine(766, 58),
                EndCombatEventPacket_V763_769Factory
            },
            {
                Combine(766, 59),
                EnterCombatEventPacket_V755_769Factory
            },
            {
                Combine(766, 60),
                DeathCombatEventPacket_V765_769Factory
            },
            {
                Combine(766, 15),
                ClearTitlesPacket_V755_769Factory
            },
            {
                Combine(766, 37),
                InitializeWorldBorderPacket_V759_769Factory
            },
            {
                Combine(766, 76),
                ActionBarPacket_V765_769Factory
            },
            {
                Combine(766, 77),
                WorldBorderCenterPacket_V755_769Factory
            },
            {
                Combine(766, 78),
                WorldBorderLerpSizePacket_V759_769Factory
            },
            {
                Combine(766, 79),
                WorldBorderSizePacket_V755_769Factory
            },
            {
                Combine(766, 80),
                WorldBorderWarningDelayPacket_V755_769Factory
            },
            {
                Combine(766, 81),
                WorldBorderWarningReachPacket_V755_769Factory
            },
            {
                Combine(766, 53),
                PingPacket_V755_769Factory
            },
            {
                Combine(766, 99),
                SetTitleSubtitlePacket_V765_769Factory
            },
            {
                Combine(766, 101),
                SetTitleTextPacket_V765_769Factory
            },
            {
                Combine(766, 102),
                SetTitleTimePacket_V755_769Factory
            },
            {
                Combine(766, 98),
                SimulationDistancePacket_V757_769Factory
            },
            {
                Combine(766, 108),
                SystemChatPacket_V765_769Factory
            },
            {
                Combine(766, 75),
                ServerDataPacket_V766_769Factory
            },
            {
                Combine(766, 24),
                ChatSuggestionsPacket_V760_769Factory
            },
            {
                Combine(766, 61),
                PlayerRemovePacket_V761_769Factory
            },
            {
                Combine(766, 26),
                DamageEventPacket_V762_769Factory
            },
            {
                Combine(766, 36),
                HurtAnimationPacket_V762_769Factory
            },
            {
                Combine(766, 12),
                ChunkBatchFinishedPacket_V764_769Factory
            },
            {
                Combine(766, 13),
                ChunkBatchStartPacket_V764_769Factory
            },
            {
                Combine(766, 54),
                PingResponsePacket_V764_769Factory
            },
            {
                Combine(766, 105),
                StartConfigurationPacket_V764_769Factory
            },
            {
                Combine(766, 68),
                ResetScorePacket_V765_769Factory
            },
            {
                Combine(766, 69),
                RemoveResourcePackPacket_V765_767Factory
            },
            {
                Combine(766, 70),
                AddResourcePackPacket_V765_767Factory
            },
            {
                Combine(766, 113),
                SetTickingStatePacket_V765_769Factory
            },
            {
                Combine(766, 114),
                StepTickPacket_V765_769Factory
            },
            {
                Combine(766, 27),
                DebugSamplePacket_V766_769Factory
            },
            {
                Combine(767, 1),
                SpawnEntityPacket_V759_769Factory
            },
            {
                Combine(767, 2),
                SpawnEntityExperienceOrbPacket_V340_769Factory
            },
            {
                Combine(767, 3),
                AnimationPacket_V340_769Factory
            },
            {
                Combine(767, 6),
                BlockBreakAnimationPacket_V340_769Factory
            },
            {
                Combine(767, 7),
                TileEntityDataPacket_V764_769Factory
            },
            {
                Combine(767, 8),
                BlockActionPacket_V340_769Factory
            },
            {
                Combine(767, 9),
                BlockChangePacket_V340_769Factory
            },
            {
                Combine(767, 11),
                DifficultyPacket_V477_769Factory
            },
            {
                Combine(767, 18),
                CloseWindowPacket_V340_767Factory
            },
            {
                Combine(767, 19),
                WindowItemsPacket_V766_767Factory
            },
            {
                Combine(767, 20),
                CraftProgressBarPacket_V340_767Factory
            },
            {
                Combine(767, 21),
                SetSlotPacket_V766_767Factory
            },
            {
                Combine(767, 23),
                SetCooldownPacket_V340_767Factory
            },
            {
                Combine(767, 25),
                CustomPayloadPacket_V340_769Factory
            },
            {
                Combine(767, 29),
                KickDisconnectPacket_V765_769Factory
            },
            {
                Combine(767, 31),
                EntityStatusPacket_V340_769Factory
            },
            {
                Combine(767, 33),
                UnloadChunkPacket_V764_769Factory
            },
            {
                Combine(767, 34),
                GameStateChangePacket_V340_769Factory
            },
            {
                Combine(767, 38),
                KeepAlivePacket_V340_769Factory
            },
            {
                Combine(767, 40),
                WorldEventPacket_V340_769Factory
            },
            {
                Combine(767, 46),
                RelEntityMovePacket_V340_769Factory
            },
            {
                Combine(767, 47),
                EntityMoveLookPacket_V340_769Factory
            },
            {
                Combine(767, 48),
                EntityLookPacket_V340_769Factory
            },
            {
                Combine(767, 49),
                VehicleMovePacket_V340_769Factory
            },
            {
                Combine(767, 52),
                OpenSignEntityPacket_V763_769Factory
            },
            {
                Combine(767, 56),
                AbilitiesPacket_V340_769Factory
            },
            {
                Combine(767, 64),
                PositionPacket_V762_767Factory
            },
            {
                Combine(767, 66),
                EntityDestroyPacket_V756_769Factory
            },
            {
                Combine(767, 67),
                RemoveEntityEffectPacket_V758_769Factory
            },
            {
                Combine(767, 72),
                EntityHeadRotationPacket_V340_769Factory
            },
            {
                Combine(767, 82),
                CameraPacket_V340_769Factory
            },
            {
                Combine(767, 83),
                HeldItemSlotPacket_V340_768Factory
            },
            {
                Combine(767, 87),
                ScoreboardDisplayObjectivePacket_V764_769Factory
            },
            {
                Combine(767, 89),
                AttachEntityPacket_V340_769Factory
            },
            {
                Combine(767, 90),
                EntityVelocityPacket_V340_769Factory
            },
            {
                Combine(767, 92),
                ExperiencePacket_V764_769Factory
            },
            {
                Combine(767, 93),
                UpdateHealthPacket_V340_769Factory
            },
            {
                Combine(767, 95),
                SetPassengersPacket_V340_769Factory
            },
            {
                Combine(767, 86),
                SpawnPositionPacket_V755_769Factory
            },
            {
                Combine(767, 100),
                UpdateTimePacket_V340_767Factory
            },
            {
                Combine(767, 109),
                PlayerlistHeaderPacket_V765_769Factory
            },
            {
                Combine(767, 111),
                CollectPacket_V340_769Factory
            },
            {
                Combine(767, 112),
                EntityTeleportPacket_V340_769Factory
            },
            {
                Combine(767, 118),
                EntityEffectPacket_V766_769Factory
            },
            {
                Combine(767, 74),
                SelectAdvancementTabPacket_V340_769Factory
            },
            {
                Combine(767, 110),
                NbtQueryResponsePacket_V764_769Factory
            },
            {
                Combine(767, 35),
                OpenHorseWindowPacket_V477_767Factory
            },
            {
                Combine(767, 42),
                UpdateLightPacket_V763_769Factory
            },
            {
                Combine(767, 50),
                OpenBookPacket_V477_769Factory
            },
            {
                Combine(767, 84),
                UpdateViewPositionPacket_V477_769Factory
            },
            {
                Combine(767, 85),
                UpdateViewDistancePacket_V477_769Factory
            },
            {
                Combine(767, 5),
                AcknowledgePlayerDiggingPacket_V759_769Factory
            },
            {
                Combine(767, 58),
                EndCombatEventPacket_V763_769Factory
            },
            {
                Combine(767, 59),
                EnterCombatEventPacket_V755_769Factory
            },
            {
                Combine(767, 60),
                DeathCombatEventPacket_V765_769Factory
            },
            {
                Combine(767, 15),
                ClearTitlesPacket_V755_769Factory
            },
            {
                Combine(767, 37),
                InitializeWorldBorderPacket_V759_769Factory
            },
            {
                Combine(767, 76),
                ActionBarPacket_V765_769Factory
            },
            {
                Combine(767, 77),
                WorldBorderCenterPacket_V755_769Factory
            },
            {
                Combine(767, 78),
                WorldBorderLerpSizePacket_V759_769Factory
            },
            {
                Combine(767, 79),
                WorldBorderSizePacket_V755_769Factory
            },
            {
                Combine(767, 80),
                WorldBorderWarningDelayPacket_V755_769Factory
            },
            {
                Combine(767, 81),
                WorldBorderWarningReachPacket_V755_769Factory
            },
            {
                Combine(767, 53),
                PingPacket_V755_769Factory
            },
            {
                Combine(767, 99),
                SetTitleSubtitlePacket_V765_769Factory
            },
            {
                Combine(767, 101),
                SetTitleTextPacket_V765_769Factory
            },
            {
                Combine(767, 102),
                SetTitleTimePacket_V755_769Factory
            },
            {
                Combine(767, 98),
                SimulationDistancePacket_V757_769Factory
            },
            {
                Combine(767, 108),
                SystemChatPacket_V765_769Factory
            },
            {
                Combine(767, 75),
                ServerDataPacket_V766_769Factory
            },
            {
                Combine(767, 24),
                ChatSuggestionsPacket_V760_769Factory
            },
            {
                Combine(767, 61),
                PlayerRemovePacket_V761_769Factory
            },
            {
                Combine(767, 26),
                DamageEventPacket_V762_769Factory
            },
            {
                Combine(767, 36),
                HurtAnimationPacket_V762_769Factory
            },
            {
                Combine(767, 12),
                ChunkBatchFinishedPacket_V764_769Factory
            },
            {
                Combine(767, 13),
                ChunkBatchStartPacket_V764_769Factory
            },
            {
                Combine(767, 54),
                PingResponsePacket_V764_769Factory
            },
            {
                Combine(767, 105),
                StartConfigurationPacket_V764_769Factory
            },
            {
                Combine(767, 68),
                ResetScorePacket_V765_769Factory
            },
            {
                Combine(767, 69),
                RemoveResourcePackPacket_V765_767Factory
            },
            {
                Combine(767, 70),
                AddResourcePackPacket_V765_767Factory
            },
            {
                Combine(767, 113),
                SetTickingStatePacket_V765_769Factory
            },
            {
                Combine(767, 114),
                StepTickPacket_V765_769Factory
            },
            {
                Combine(767, 27),
                DebugSamplePacket_V766_769Factory
            },
            {
                Combine(768, 1),
                SpawnEntityPacket_V759_769Factory
            },
            {
                Combine(768, 2),
                SpawnEntityExperienceOrbPacket_V340_769Factory
            },
            {
                Combine(768, 3),
                AnimationPacket_V340_769Factory
            },
            {
                Combine(768, 6),
                BlockBreakAnimationPacket_V340_769Factory
            },
            {
                Combine(768, 7),
                TileEntityDataPacket_V764_769Factory
            },
            {
                Combine(768, 8),
                BlockActionPacket_V340_769Factory
            },
            {
                Combine(768, 9),
                BlockChangePacket_V340_769Factory
            },
            {
                Combine(768, 11),
                DifficultyPacket_V477_769Factory
            },
            {
                Combine(768, 18),
                CloseWindowPacket_V768_769Factory
            },
            {
                Combine(768, 19),
                WindowItemsPacket_V768_769Factory
            },
            {
                Combine(768, 20),
                CraftProgressBarPacket_V768_769Factory
            },
            {
                Combine(768, 21),
                SetSlotPacket_V768_769Factory
            },
            {
                Combine(768, 23),
                SetCooldownPacket_V768_769Factory
            },
            {
                Combine(768, 25),
                CustomPayloadPacket_V340_769Factory
            },
            {
                Combine(768, 29),
                KickDisconnectPacket_V765_769Factory
            },
            {
                Combine(768, 31),
                EntityStatusPacket_V340_769Factory
            },
            {
                Combine(768, 34),
                UnloadChunkPacket_V764_769Factory
            },
            {
                Combine(768, 35),
                GameStateChangePacket_V340_769Factory
            },
            {
                Combine(768, 39),
                KeepAlivePacket_V340_769Factory
            },
            {
                Combine(768, 41),
                WorldEventPacket_V340_769Factory
            },
            {
                Combine(768, 47),
                RelEntityMovePacket_V340_769Factory
            },
            {
                Combine(768, 48),
                EntityMoveLookPacket_V340_769Factory
            },
            {
                Combine(768, 50),
                EntityLookPacket_V340_769Factory
            },
            {
                Combine(768, 51),
                VehicleMovePacket_V340_769Factory
            },
            {
                Combine(768, 54),
                OpenSignEntityPacket_V763_769Factory
            },
            {
                Combine(768, 58),
                AbilitiesPacket_V340_769Factory
            },
            {
                Combine(768, 66),
                PositionPacket_V768_769Factory
            },
            {
                Combine(768, 71),
                EntityDestroyPacket_V756_769Factory
            },
            {
                Combine(768, 72),
                RemoveEntityEffectPacket_V758_769Factory
            },
            {
                Combine(768, 77),
                EntityHeadRotationPacket_V340_769Factory
            },
            {
                Combine(768, 87),
                CameraPacket_V340_769Factory
            },
            {
                Combine(768, 99),
                HeldItemSlotPacket_V340_768Factory
            },
            {
                Combine(768, 92),
                ScoreboardDisplayObjectivePacket_V764_769Factory
            },
            {
                Combine(768, 94),
                AttachEntityPacket_V340_769Factory
            },
            {
                Combine(768, 95),
                EntityVelocityPacket_V340_769Factory
            },
            {
                Combine(768, 97),
                ExperiencePacket_V764_769Factory
            },
            {
                Combine(768, 98),
                UpdateHealthPacket_V340_769Factory
            },
            {
                Combine(768, 101),
                SetPassengersPacket_V340_769Factory
            },
            {
                Combine(768, 91),
                SpawnPositionPacket_V755_769Factory
            },
            {
                Combine(768, 107),
                UpdateTimePacket_V768_769Factory
            },
            {
                Combine(768, 116),
                PlayerlistHeaderPacket_V765_769Factory
            },
            {
                Combine(768, 118),
                CollectPacket_V340_769Factory
            },
            {
                Combine(768, 119),
                EntityTeleportPacket_V340_769Factory
            },
            {
                Combine(768, 125),
                EntityEffectPacket_V766_769Factory
            },
            {
                Combine(768, 79),
                SelectAdvancementTabPacket_V340_769Factory
            },
            {
                Combine(768, 117),
                NbtQueryResponsePacket_V764_769Factory
            },
            {
                Combine(768, 36),
                OpenHorseWindowPacket_V768_769Factory
            },
            {
                Combine(768, 43),
                UpdateLightPacket_V763_769Factory
            },
            {
                Combine(768, 52),
                OpenBookPacket_V477_769Factory
            },
            {
                Combine(768, 88),
                UpdateViewPositionPacket_V477_769Factory
            },
            {
                Combine(768, 89),
                UpdateViewDistancePacket_V477_769Factory
            },
            {
                Combine(768, 5),
                AcknowledgePlayerDiggingPacket_V759_769Factory
            },
            {
                Combine(768, 60),
                EndCombatEventPacket_V763_769Factory
            },
            {
                Combine(768, 61),
                EnterCombatEventPacket_V755_769Factory
            },
            {
                Combine(768, 62),
                DeathCombatEventPacket_V765_769Factory
            },
            {
                Combine(768, 15),
                ClearTitlesPacket_V755_769Factory
            },
            {
                Combine(768, 38),
                InitializeWorldBorderPacket_V759_769Factory
            },
            {
                Combine(768, 81),
                ActionBarPacket_V765_769Factory
            },
            {
                Combine(768, 82),
                WorldBorderCenterPacket_V755_769Factory
            },
            {
                Combine(768, 83),
                WorldBorderLerpSizePacket_V759_769Factory
            },
            {
                Combine(768, 84),
                WorldBorderSizePacket_V755_769Factory
            },
            {
                Combine(768, 85),
                WorldBorderWarningDelayPacket_V755_769Factory
            },
            {
                Combine(768, 86),
                WorldBorderWarningReachPacket_V755_769Factory
            },
            {
                Combine(768, 55),
                PingPacket_V755_769Factory
            },
            {
                Combine(768, 106),
                SetTitleSubtitlePacket_V765_769Factory
            },
            {
                Combine(768, 108),
                SetTitleTextPacket_V765_769Factory
            },
            {
                Combine(768, 109),
                SetTitleTimePacket_V755_769Factory
            },
            {
                Combine(768, 105),
                SimulationDistancePacket_V757_769Factory
            },
            {
                Combine(768, 115),
                SystemChatPacket_V765_769Factory
            },
            {
                Combine(768, 80),
                ServerDataPacket_V766_769Factory
            },
            {
                Combine(768, 24),
                ChatSuggestionsPacket_V760_769Factory
            },
            {
                Combine(768, 63),
                PlayerRemovePacket_V761_769Factory
            },
            {
                Combine(768, 26),
                DamageEventPacket_V762_769Factory
            },
            {
                Combine(768, 37),
                HurtAnimationPacket_V762_769Factory
            },
            {
                Combine(768, 12),
                ChunkBatchFinishedPacket_V764_769Factory
            },
            {
                Combine(768, 13),
                ChunkBatchStartPacket_V764_769Factory
            },
            {
                Combine(768, 56),
                PingResponsePacket_V764_769Factory
            },
            {
                Combine(768, 112),
                StartConfigurationPacket_V764_769Factory
            },
            {
                Combine(768, 73),
                ResetScorePacket_V765_769Factory
            },
            {
                Combine(768, 120),
                SetTickingStatePacket_V765_769Factory
            },
            {
                Combine(768, 121),
                StepTickPacket_V765_769Factory
            },
            {
                Combine(768, 27),
                DebugSamplePacket_V766_769Factory
            },
            {
                Combine(768, 128),
                SetProjectilePowerPacket_V767_769Factory
            },
            {
                Combine(768, 32),
                SyncEntityPositionPacket_V768_769Factory
            },
            {
                Combine(768, 67),
                PlayerRotationPacket_V768_769Factory
            },
            {
                Combine(768, 69),
                RecipeBookRemovePacket_V768_769Factory
            },
            {
                Combine(768, 70),
                RecipeBookSettingsPacket_V768_769Factory
            },
            {
                Combine(768, 90),
                SetCursorItemPacket_V768_769Factory
            },
            {
                Combine(768, 102),
                SetPlayerInventoryPacket_V768_769Factory
            },
            {
                Combine(769, 1),
                SpawnEntityPacket_V759_769Factory
            },
            {
                Combine(769, 2),
                SpawnEntityExperienceOrbPacket_V340_769Factory
            },
            {
                Combine(769, 3),
                AnimationPacket_V340_769Factory
            },
            {
                Combine(769, 6),
                BlockBreakAnimationPacket_V340_769Factory
            },
            {
                Combine(769, 7),
                TileEntityDataPacket_V764_769Factory
            },
            {
                Combine(769, 8),
                BlockActionPacket_V340_769Factory
            },
            {
                Combine(769, 9),
                BlockChangePacket_V340_769Factory
            },
            {
                Combine(769, 11),
                DifficultyPacket_V477_769Factory
            },
            {
                Combine(769, 18),
                CloseWindowPacket_V768_769Factory
            },
            {
                Combine(769, 19),
                WindowItemsPacket_V768_769Factory
            },
            {
                Combine(769, 20),
                CraftProgressBarPacket_V768_769Factory
            },
            {
                Combine(769, 21),
                SetSlotPacket_V768_769Factory
            },
            {
                Combine(769, 23),
                SetCooldownPacket_V768_769Factory
            },
            {
                Combine(769, 25),
                CustomPayloadPacket_V340_769Factory
            },
            {
                Combine(769, 29),
                KickDisconnectPacket_V765_769Factory
            },
            {
                Combine(769, 31),
                EntityStatusPacket_V340_769Factory
            },
            {
                Combine(769, 34),
                UnloadChunkPacket_V764_769Factory
            },
            {
                Combine(769, 35),
                GameStateChangePacket_V340_769Factory
            },
            {
                Combine(769, 39),
                KeepAlivePacket_V340_769Factory
            },
            {
                Combine(769, 41),
                WorldEventPacket_V340_769Factory
            },
            {
                Combine(769, 47),
                RelEntityMovePacket_V340_769Factory
            },
            {
                Combine(769, 48),
                EntityMoveLookPacket_V340_769Factory
            },
            {
                Combine(769, 50),
                EntityLookPacket_V340_769Factory
            },
            {
                Combine(769, 51),
                VehicleMovePacket_V340_769Factory
            },
            {
                Combine(769, 54),
                OpenSignEntityPacket_V763_769Factory
            },
            {
                Combine(769, 58),
                AbilitiesPacket_V340_769Factory
            },
            {
                Combine(769, 66),
                PositionPacket_V768_769Factory
            },
            {
                Combine(769, 71),
                EntityDestroyPacket_V756_769Factory
            },
            {
                Combine(769, 72),
                RemoveEntityEffectPacket_V758_769Factory
            },
            {
                Combine(769, 77),
                EntityHeadRotationPacket_V340_769Factory
            },
            {
                Combine(769, 87),
                CameraPacket_V340_769Factory
            },
            {
                Combine(769, 99),
                HeldItemSlotPacket_V769Factory
            },
            {
                Combine(769, 92),
                ScoreboardDisplayObjectivePacket_V764_769Factory
            },
            {
                Combine(769, 94),
                AttachEntityPacket_V340_769Factory
            },
            {
                Combine(769, 95),
                EntityVelocityPacket_V340_769Factory
            },
            {
                Combine(769, 97),
                ExperiencePacket_V764_769Factory
            },
            {
                Combine(769, 98),
                UpdateHealthPacket_V340_769Factory
            },
            {
                Combine(769, 101),
                SetPassengersPacket_V340_769Factory
            },
            {
                Combine(769, 91),
                SpawnPositionPacket_V755_769Factory
            },
            {
                Combine(769, 107),
                UpdateTimePacket_V768_769Factory
            },
            {
                Combine(769, 116),
                PlayerlistHeaderPacket_V765_769Factory
            },
            {
                Combine(769, 118),
                CollectPacket_V340_769Factory
            },
            {
                Combine(769, 119),
                EntityTeleportPacket_V340_769Factory
            },
            {
                Combine(769, 125),
                EntityEffectPacket_V766_769Factory
            },
            {
                Combine(769, 79),
                SelectAdvancementTabPacket_V340_769Factory
            },
            {
                Combine(769, 117),
                NbtQueryResponsePacket_V764_769Factory
            },
            {
                Combine(769, 36),
                OpenHorseWindowPacket_V768_769Factory
            },
            {
                Combine(769, 43),
                UpdateLightPacket_V763_769Factory
            },
            {
                Combine(769, 52),
                OpenBookPacket_V477_769Factory
            },
            {
                Combine(769, 88),
                UpdateViewPositionPacket_V477_769Factory
            },
            {
                Combine(769, 89),
                UpdateViewDistancePacket_V477_769Factory
            },
            {
                Combine(769, 5),
                AcknowledgePlayerDiggingPacket_V759_769Factory
            },
            {
                Combine(769, 60),
                EndCombatEventPacket_V763_769Factory
            },
            {
                Combine(769, 61),
                EnterCombatEventPacket_V755_769Factory
            },
            {
                Combine(769, 62),
                DeathCombatEventPacket_V765_769Factory
            },
            {
                Combine(769, 15),
                ClearTitlesPacket_V755_769Factory
            },
            {
                Combine(769, 38),
                InitializeWorldBorderPacket_V759_769Factory
            },
            {
                Combine(769, 81),
                ActionBarPacket_V765_769Factory
            },
            {
                Combine(769, 82),
                WorldBorderCenterPacket_V755_769Factory
            },
            {
                Combine(769, 83),
                WorldBorderLerpSizePacket_V759_769Factory
            },
            {
                Combine(769, 84),
                WorldBorderSizePacket_V755_769Factory
            },
            {
                Combine(769, 85),
                WorldBorderWarningDelayPacket_V755_769Factory
            },
            {
                Combine(769, 86),
                WorldBorderWarningReachPacket_V755_769Factory
            },
            {
                Combine(769, 55),
                PingPacket_V755_769Factory
            },
            {
                Combine(769, 106),
                SetTitleSubtitlePacket_V765_769Factory
            },
            {
                Combine(769, 108),
                SetTitleTextPacket_V765_769Factory
            },
            {
                Combine(769, 109),
                SetTitleTimePacket_V755_769Factory
            },
            {
                Combine(769, 105),
                SimulationDistancePacket_V757_769Factory
            },
            {
                Combine(769, 115),
                SystemChatPacket_V765_769Factory
            },
            {
                Combine(769, 80),
                ServerDataPacket_V766_769Factory
            },
            {
                Combine(769, 24),
                ChatSuggestionsPacket_V760_769Factory
            },
            {
                Combine(769, 63),
                PlayerRemovePacket_V761_769Factory
            },
            {
                Combine(769, 26),
                DamageEventPacket_V762_769Factory
            },
            {
                Combine(769, 37),
                HurtAnimationPacket_V762_769Factory
            },
            {
                Combine(769, 12),
                ChunkBatchFinishedPacket_V764_769Factory
            },
            {
                Combine(769, 13),
                ChunkBatchStartPacket_V764_769Factory
            },
            {
                Combine(769, 56),
                PingResponsePacket_V764_769Factory
            },
            {
                Combine(769, 112),
                StartConfigurationPacket_V764_769Factory
            },
            {
                Combine(769, 73),
                ResetScorePacket_V765_769Factory
            },
            {
                Combine(769, 120),
                SetTickingStatePacket_V765_769Factory
            },
            {
                Combine(769, 121),
                StepTickPacket_V765_769Factory
            },
            {
                Combine(769, 27),
                DebugSamplePacket_V766_769Factory
            },
            {
                Combine(769, 128),
                SetProjectilePowerPacket_V767_769Factory
            },
            {
                Combine(769, 32),
                SyncEntityPositionPacket_V768_769Factory
            },
            {
                Combine(769, 67),
                PlayerRotationPacket_V768_769Factory
            },
            {
                Combine(769, 69),
                RecipeBookRemovePacket_V768_769Factory
            },
            {
                Combine(769, 70),
                RecipeBookSettingsPacket_V768_769Factory
            },
            {
                Combine(769, 90),
                SetCursorItemPacket_V768_769Factory
            },
            {
                Combine(769, 102),
                SetPlayerInventoryPacket_V768_769Factory
            }
        }.ToFrozenDictionary();
    }
}