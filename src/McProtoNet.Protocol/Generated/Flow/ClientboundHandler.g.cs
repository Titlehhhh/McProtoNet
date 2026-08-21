using System.Threading.Tasks;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol;
/// <summary>Generated handler base over every clientbound phase. The truth about
/// the current phase is the consumer's: set <see cref = "Phase"/> as the connection
/// advances. <c>HandleAsync</c> decodes synchronously (the raw data window must not
/// cross an await) and awaits the handler's result after. <c>OnUnknown</c> must not
/// hold on to <c>raw</c> beyond the call.</summary>
public abstract partial class ClientboundHandler : IPacketVisitor
{
    private ValueTask _pending;
    public PacketPhase Phase { get; protected set; } = PacketPhase.Login;
    protected static PacketDirection Direction => PacketDirection.Clientbound;

    public ValueTask HandleAsync(in IncomingPacket raw, int protocolVersion)
    {
        _pending = default;
        var self = this;
        PacketFlow.Dispatch(in raw, protocolVersion, Phase, PacketDirection.Clientbound, ref self);
        return _pending;
    }

    void IPacketVisitor.Visit<T>(T packet)
    {
        var identity = T.Identity;
        switch (identity.Phase)
        {
            case PacketPhase.Status:
                switch (identity.Ordinal)
                {
                    case 0:
                        _pending = OnPongResponse((Packets.Status.Clientbound.PongResponsePacket)(object)packet);
                        return;
                    case 1:
                        _pending = OnServerInfo((Packets.Status.Clientbound.ServerInfoPacket)(object)packet);
                        return;
                }

                return;
            case PacketPhase.Login:
                switch (identity.Ordinal)
                {
                    case 0:
                        _pending = OnLoginCompress((Packets.Login.Clientbound.LoginCompressPacket)(object)packet);
                        return;
                    case 1:
                        _pending = OnLoginCookieRequest((Packets.Login.Clientbound.LoginCookieRequestPacket)(object)packet);
                        return;
                    case 2:
                        _pending = OnLoginDisconnect((Packets.Login.Clientbound.LoginDisconnectPacket)(object)packet);
                        return;
                    case 3:
                        _pending = OnEncryptionRequest((Packets.Login.Clientbound.EncryptionRequestPacket)(object)packet);
                        return;
                    case 4:
                        _pending = OnLoginPluginRequest((Packets.Login.Clientbound.LoginPluginRequestPacket)(object)packet);
                        return;
                    case 5:
                        _pending = OnLoginSuccess((Packets.Login.Clientbound.LoginSuccessPacket)(object)packet);
                        return;
                }

                return;
            case PacketPhase.Configuration:
                switch (identity.Ordinal)
                {
                    case 0:
                        _pending = OnConfigurationAddResourcePack((Packets.Configuration.Clientbound.AddResourcePackPacket)(object)packet);
                        return;
                    case 1:
                        _pending = OnConfigurationClearDialog((Packets.Configuration.Clientbound.ClearDialogPacket)(object)packet);
                        return;
                    case 2:
                        _pending = OnCodeOfConduct((Packets.Configuration.Clientbound.CodeOfConductPacket)(object)packet);
                        return;
                    case 3:
                        _pending = OnConfigurationCookieRequest((Packets.Configuration.Clientbound.CookieRequestPacket)(object)packet);
                        return;
                    case 4:
                        _pending = OnConfigurationCustomPayload((Packets.Configuration.Clientbound.CustomPayloadPacket)(object)packet);
                        return;
                    case 5:
                        _pending = OnConfigurationCustomReportDetails((Packets.Configuration.Clientbound.CustomReportDetailsPacket)(object)packet);
                        return;
                    case 6:
                        _pending = OnDisconnect((Packets.Configuration.Clientbound.DisconnectPacket)(object)packet);
                        return;
                    case 7:
                        _pending = OnConfigurationFeatureFlags((Packets.Configuration.Clientbound.FeatureFlagsPacket)(object)packet);
                        return;
                    case 8:
                        _pending = OnFinishConfiguration((Packets.Configuration.Clientbound.FinishConfigurationPacket)(object)packet);
                        return;
                    case 9:
                        _pending = OnConfigurationKeepAlive((Packets.Configuration.Clientbound.KeepAlivePacket)(object)packet);
                        return;
                    case 10:
                        _pending = OnConfigurationPing((Packets.Configuration.Clientbound.PingPacket)(object)packet);
                        return;
                    case 11:
                        _pending = OnConfigurationRemoveResourcePack((Packets.Configuration.Clientbound.RemoveResourcePackPacket)(object)packet);
                        return;
                    case 12:
                        _pending = OnResetChat((Packets.Configuration.Clientbound.ResetChatPacket)(object)packet);
                        return;
                    case 13:
                        _pending = OnConfigurationResourcePackSend((Packets.Configuration.Clientbound.ResourcePackSendPacket)(object)packet);
                        return;
                    case 14:
                        _pending = OnSelectKnownPacks((Packets.Configuration.Clientbound.SelectKnownPacksPacket)(object)packet);
                        return;
                    case 15:
                        _pending = OnShowDialog((Packets.Configuration.Clientbound.ShowDialogPacket)(object)packet);
                        return;
                    case 16:
                        _pending = OnConfigurationStoreCookie((Packets.Configuration.Clientbound.StoreCookiePacket)(object)packet);
                        return;
                    case 17:
                        _pending = OnConfigurationTags((Packets.Configuration.Clientbound.TagsPacket)(object)packet);
                        return;
                    case 18:
                        _pending = OnConfigurationTransfer((Packets.Configuration.Clientbound.TransferPacket)(object)packet);
                        return;
                }

                return;
            case PacketPhase.Play:
                switch (identity.Ordinal)
                {
                    case 0:
                        _pending = OnAbilities((Packets.Play.Clientbound.AbilitiesPacket)(object)packet);
                        return;
                    case 1:
                        _pending = OnAcknowledgePlayerDigging((Packets.Play.Clientbound.AcknowledgePlayerDiggingPacket)(object)packet);
                        return;
                    case 2:
                        _pending = OnActionBar((Packets.Play.Clientbound.ActionBarPacket)(object)packet);
                        return;
                    case 3:
                        _pending = OnAddResourcePack((Packets.Play.Clientbound.AddResourcePackPacket)(object)packet);
                        return;
                    case 4:
                        _pending = OnAnimation((Packets.Play.Clientbound.AnimationPacket)(object)packet);
                        return;
                    case 5:
                        _pending = OnAttachEntity((Packets.Play.Clientbound.AttachEntityPacket)(object)packet);
                        return;
                    case 6:
                        _pending = OnBlockAction((Packets.Play.Clientbound.BlockActionPacket)(object)packet);
                        return;
                    case 7:
                        _pending = OnBlockBreakAnimation((Packets.Play.Clientbound.BlockBreakAnimationPacket)(object)packet);
                        return;
                    case 8:
                        _pending = OnBlockChange((Packets.Play.Clientbound.BlockChangePacket)(object)packet);
                        return;
                    case 9:
                        _pending = OnCamera((Packets.Play.Clientbound.CameraPacket)(object)packet);
                        return;
                    case 10:
                        _pending = OnChat((Packets.Play.Clientbound.ChatPacket)(object)packet);
                        return;
                    case 11:
                        _pending = OnChatPreview((Packets.Play.Clientbound.ChatPreviewPacket)(object)packet);
                        return;
                    case 12:
                        _pending = OnChatSuggestions((Packets.Play.Clientbound.ChatSuggestionsPacket)(object)packet);
                        return;
                    case 13:
                        _pending = OnChunkBatchFinished((Packets.Play.Clientbound.ChunkBatchFinishedPacket)(object)packet);
                        return;
                    case 14:
                        _pending = OnChunkBatchStart((Packets.Play.Clientbound.ChunkBatchStartPacket)(object)packet);
                        return;
                    case 15:
                        _pending = OnChunkBiomes((Packets.Play.Clientbound.ChunkBiomesPacket)(object)packet);
                        return;
                    case 16:
                        _pending = OnClearDialog((Packets.Play.Clientbound.ClearDialogPacket)(object)packet);
                        return;
                    case 17:
                        _pending = OnClearTitles((Packets.Play.Clientbound.ClearTitlesPacket)(object)packet);
                        return;
                    case 18:
                        _pending = OnCloseWindow((Packets.Play.Clientbound.CloseWindowPacket)(object)packet);
                        return;
                    case 19:
                        _pending = OnCollect((Packets.Play.Clientbound.CollectPacket)(object)packet);
                        return;
                    case 20:
                        _pending = OnCookieRequest((Packets.Play.Clientbound.CookieRequestPacket)(object)packet);
                        return;
                    case 21:
                        _pending = OnCraftProgressBar((Packets.Play.Clientbound.CraftProgressBarPacket)(object)packet);
                        return;
                    case 22:
                        _pending = OnCustomPayload((Packets.Play.Clientbound.CustomPayloadPacket)(object)packet);
                        return;
                    case 23:
                        _pending = OnCustomReportDetails((Packets.Play.Clientbound.CustomReportDetailsPacket)(object)packet);
                        return;
                    case 24:
                        _pending = OnDamageEvent((Packets.Play.Clientbound.DamageEventPacket)(object)packet);
                        return;
                    case 25:
                        _pending = OnDeathCombatEvent((Packets.Play.Clientbound.DeathCombatEventPacket)(object)packet);
                        return;
                    case 26:
                        _pending = OnDebugSample((Packets.Play.Clientbound.DebugSamplePacket)(object)packet);
                        return;
                    case 27:
                        _pending = OnDestroyEntity((Packets.Play.Clientbound.DestroyEntityPacket)(object)packet);
                        return;
                    case 28:
                        _pending = OnDifficulty((Packets.Play.Clientbound.DifficultyPacket)(object)packet);
                        return;
                    case 29:
                        _pending = OnEndCombatEvent((Packets.Play.Clientbound.EndCombatEventPacket)(object)packet);
                        return;
                    case 30:
                        _pending = OnEnterCombatEvent((Packets.Play.Clientbound.EnterCombatEventPacket)(object)packet);
                        return;
                    case 31:
                        _pending = OnEntity((Packets.Play.Clientbound.EntityPacket)(object)packet);
                        return;
                    case 32:
                        _pending = OnEntityDestroy((Packets.Play.Clientbound.EntityDestroyPacket)(object)packet);
                        return;
                    case 33:
                        _pending = OnEntityHeadRotation((Packets.Play.Clientbound.EntityHeadRotationPacket)(object)packet);
                        return;
                    case 34:
                        _pending = OnEntityLook((Packets.Play.Clientbound.EntityLookPacket)(object)packet);
                        return;
                    case 36:
                        _pending = OnEntityMoveLook((Packets.Play.Clientbound.EntityMoveLookPacket)(object)packet);
                        return;
                    case 37:
                        _pending = OnEntityStatus((Packets.Play.Clientbound.EntityStatusPacket)(object)packet);
                        return;
                    case 38:
                        _pending = OnEntityTeleport((Packets.Play.Clientbound.EntityTeleportPacket)(object)packet);
                        return;
                    case 39:
                        _pending = OnEntityUpdateAttributes((Packets.Play.Clientbound.EntityUpdateAttributesPacket)(object)packet);
                        return;
                    case 41:
                        _pending = OnExperience((Packets.Play.Clientbound.ExperiencePacket)(object)packet);
                        return;
                    case 43:
                        _pending = OnFeatureFlags((Packets.Play.Clientbound.FeatureFlagsPacket)(object)packet);
                        return;
                    case 44:
                        _pending = OnGameRuleValues((Packets.Play.Clientbound.GameRuleValuesPacket)(object)packet);
                        return;
                    case 45:
                        _pending = OnGameStateChange((Packets.Play.Clientbound.GameStateChangePacket)(object)packet);
                        return;
                    case 46:
                        _pending = OnGameTestHighlightPos((Packets.Play.Clientbound.GameTestHighlightPosPacket)(object)packet);
                        return;
                    case 47:
                        _pending = OnHeldItemSlot((Packets.Play.Clientbound.HeldItemSlotPacket)(object)packet);
                        return;
                    case 48:
                        _pending = OnHurtAnimation((Packets.Play.Clientbound.HurtAnimationPacket)(object)packet);
                        return;
                    case 49:
                        _pending = OnInitializeWorldBorder((Packets.Play.Clientbound.InitializeWorldBorderPacket)(object)packet);
                        return;
                    case 50:
                        _pending = OnKeepAlive((Packets.Play.Clientbound.KeepAlivePacket)(object)packet);
                        return;
                    case 51:
                        _pending = OnKickDisconnect((Packets.Play.Clientbound.KickDisconnectPacket)(object)packet);
                        return;
                    case 52:
                        _pending = OnLogin((Packets.Play.Clientbound.LoginPacket)(object)packet);
                        return;
                    case 53:
                        _pending = OnLowDiskSpaceWarning((Packets.Play.Clientbound.LowDiskSpaceWarningPacket)(object)packet);
                        return;
                    case 55:
                        _pending = OnMessageHeader((Packets.Play.Clientbound.MessageHeaderPacket)(object)packet);
                        return;
                    case 56:
                        _pending = OnMoveMinecart((Packets.Play.Clientbound.MoveMinecartPacket)(object)packet);
                        return;
                    case 57:
                        _pending = OnNamedEntitySpawn((Packets.Play.Clientbound.NamedEntitySpawnPacket)(object)packet);
                        return;
                    case 58:
                        _pending = OnNamedSoundEffect((Packets.Play.Clientbound.NamedSoundEffectPacket)(object)packet);
                        return;
                    case 59:
                        _pending = OnNbtQueryResponse((Packets.Play.Clientbound.NbtQueryResponsePacket)(object)packet);
                        return;
                    case 60:
                        _pending = OnOpenBook((Packets.Play.Clientbound.OpenBookPacket)(object)packet);
                        return;
                    case 61:
                        _pending = OnOpenHorseWindow((Packets.Play.Clientbound.OpenHorseWindowPacket)(object)packet);
                        return;
                    case 62:
                        _pending = OnOpenSignEntity((Packets.Play.Clientbound.OpenSignEntityPacket)(object)packet);
                        return;
                    case 63:
                        _pending = OnOpenWindow((Packets.Play.Clientbound.OpenWindowPacket)(object)packet);
                        return;
                    case 64:
                        _pending = OnPing((Packets.Play.Clientbound.PingPacket)(object)packet);
                        return;
                    case 65:
                        _pending = OnPingResponse((Packets.Play.Clientbound.PingResponsePacket)(object)packet);
                        return;
                    case 66:
                        _pending = OnPlayerRemove((Packets.Play.Clientbound.PlayerRemovePacket)(object)packet);
                        return;
                    case 67:
                        _pending = OnPlayerRotation((Packets.Play.Clientbound.PlayerRotationPacket)(object)packet);
                        return;
                    case 68:
                        _pending = OnPlayerlistHeader((Packets.Play.Clientbound.PlayerlistHeaderPacket)(object)packet);
                        return;
                    case 69:
                        _pending = OnPlayerPosition((Packets.Play.Clientbound.PlayerPositionPacket)(object)packet);
                        return;
                    case 70:
                        _pending = OnRecipeBookRemove((Packets.Play.Clientbound.RecipeBookRemovePacket)(object)packet);
                        return;
                    case 71:
                        _pending = OnRelEntityMove((Packets.Play.Clientbound.RelEntityMovePacket)(object)packet);
                        return;
                    case 72:
                        _pending = OnRemoveEntityEffect((Packets.Play.Clientbound.RemoveEntityEffectPacket)(object)packet);
                        return;
                    case 73:
                        _pending = OnRemoveResourcePack((Packets.Play.Clientbound.RemoveResourcePackPacket)(object)packet);
                        return;
                    case 74:
                        _pending = OnResetScore((Packets.Play.Clientbound.ResetScorePacket)(object)packet);
                        return;
                    case 75:
                        _pending = OnResourcePackSend((Packets.Play.Clientbound.ResourcePackSendPacket)(object)packet);
                        return;
                    case 76:
                        _pending = OnRespawn((Packets.Play.Clientbound.RespawnPacket)(object)packet);
                        return;
                    case 77:
                        _pending = OnScoreboardDisplayObjective((Packets.Play.Clientbound.ScoreboardDisplayObjectivePacket)(object)packet);
                        return;
                    case 78:
                        _pending = OnSelectAdvancementTab((Packets.Play.Clientbound.SelectAdvancementTabPacket)(object)packet);
                        return;
                    case 79:
                        _pending = OnServerData((Packets.Play.Clientbound.ServerDataPacket)(object)packet);
                        return;
                    case 80:
                        _pending = OnSetCooldown((Packets.Play.Clientbound.SetCooldownPacket)(object)packet);
                        return;
                    case 81:
                        _pending = OnSetPassengers((Packets.Play.Clientbound.SetPassengersPacket)(object)packet);
                        return;
                    case 82:
                        _pending = OnSetProjectilePower((Packets.Play.Clientbound.SetProjectilePowerPacket)(object)packet);
                        return;
                    case 83:
                        _pending = OnSetTickingState((Packets.Play.Clientbound.SetTickingStatePacket)(object)packet);
                        return;
                    case 84:
                        _pending = OnSetTitleSubtitle((Packets.Play.Clientbound.SetTitleSubtitlePacket)(object)packet);
                        return;
                    case 85:
                        _pending = OnSetTitleText((Packets.Play.Clientbound.SetTitleTextPacket)(object)packet);
                        return;
                    case 86:
                        _pending = OnSetTitleTime((Packets.Play.Clientbound.SetTitleTimePacket)(object)packet);
                        return;
                    case 87:
                        _pending = OnShouldDisplayChatPreview((Packets.Play.Clientbound.ShouldDisplayChatPreviewPacket)(object)packet);
                        return;
                    case 88:
                        _pending = OnSimulationDistance((Packets.Play.Clientbound.SimulationDistancePacket)(object)packet);
                        return;
                    case 90:
                        _pending = OnSpawnEntityExperienceOrb((Packets.Play.Clientbound.SpawnEntityExperienceOrbPacket)(object)packet);
                        return;
                    case 91:
                        _pending = OnSpawnEntityLiving((Packets.Play.Clientbound.SpawnEntityLivingPacket)(object)packet);
                        return;
                    case 92:
                        _pending = OnSpawnEntityPainting((Packets.Play.Clientbound.SpawnEntityPaintingPacket)(object)packet);
                        return;
                    case 93:
                        _pending = OnSpawnPosition((Packets.Play.Clientbound.SpawnPositionPacket)(object)packet);
                        return;
                    case 94:
                        _pending = OnStartConfiguration((Packets.Play.Clientbound.StartConfigurationPacket)(object)packet);
                        return;
                    case 95:
                        _pending = OnStatistics((Packets.Play.Clientbound.StatisticsPacket)(object)packet);
                        return;
                    case 96:
                        _pending = OnStepTick((Packets.Play.Clientbound.StepTickPacket)(object)packet);
                        return;
                    case 97:
                        _pending = OnStoreCookie((Packets.Play.Clientbound.StoreCookiePacket)(object)packet);
                        return;
                    case 98:
                        _pending = OnSyncEntityPosition((Packets.Play.Clientbound.SyncEntityPositionPacket)(object)packet);
                        return;
                    case 99:
                        _pending = OnSystemChat((Packets.Play.Clientbound.SystemChatPacket)(object)packet);
                        return;
                    case 100:
                        _pending = OnTabComplete((Packets.Play.Clientbound.TabCompletePacket)(object)packet);
                        return;
                    case 101:
                        _pending = OnTags((Packets.Play.Clientbound.TagsPacket)(object)packet);
                        return;
                    case 102:
                        _pending = OnTeams((Packets.Play.Clientbound.TeamsPacket)(object)packet);
                        return;
                    case 103:
                        _pending = OnTestInstanceBlockStatus((Packets.Play.Clientbound.TestInstanceBlockStatusPacket)(object)packet);
                        return;
                    case 104:
                        _pending = OnTileEntityData((Packets.Play.Clientbound.TileEntityDataPacket)(object)packet);
                        return;
                    case 105:
                        _pending = OnTransaction((Packets.Play.Clientbound.TransactionPacket)(object)packet);
                        return;
                    case 106:
                        _pending = OnTransfer((Packets.Play.Clientbound.TransferPacket)(object)packet);
                        return;
                    case 107:
                        _pending = OnUnloadChunk((Packets.Play.Clientbound.UnloadChunkPacket)(object)packet);
                        return;
                    case 108:
                        _pending = OnUpdateHealth((Packets.Play.Clientbound.UpdateHealthPacket)(object)packet);
                        return;
                    case 110:
                        _pending = OnUpdateTime((Packets.Play.Clientbound.UpdateTimePacket)(object)packet);
                        return;
                    case 111:
                        _pending = OnUpdateViewDistance((Packets.Play.Clientbound.UpdateViewDistancePacket)(object)packet);
                        return;
                    case 112:
                        _pending = OnUpdateViewPosition((Packets.Play.Clientbound.UpdateViewPositionPacket)(object)packet);
                        return;
                    case 113:
                        _pending = OnVehicleMove((Packets.Play.Clientbound.VehicleMovePacket)(object)packet);
                        return;
                    case 114:
                        _pending = OnWorldBorderCenter((Packets.Play.Clientbound.WorldBorderCenterPacket)(object)packet);
                        return;
                    case 115:
                        _pending = OnWorldBorderLerpSize((Packets.Play.Clientbound.WorldBorderLerpSizePacket)(object)packet);
                        return;
                    case 116:
                        _pending = OnWorldBorderSize((Packets.Play.Clientbound.WorldBorderSizePacket)(object)packet);
                        return;
                    case 117:
                        _pending = OnWorldBorderWarningDelay((Packets.Play.Clientbound.WorldBorderWarningDelayPacket)(object)packet);
                        return;
                    case 118:
                        _pending = OnWorldBorderWarningReach((Packets.Play.Clientbound.WorldBorderWarningReachPacket)(object)packet);
                        return;
                    case 119:
                        _pending = OnWorldEvent((Packets.Play.Clientbound.WorldEventPacket)(object)packet);
                        return;
                }

                return;
        }
    }

    void IPacketVisitor.Unknown(in IncomingPacket raw) => _pending = OnUnknown(in raw);
    protected virtual ValueTask OnUnknown(in IncomingPacket raw) => default;
    // --- Status ---
    protected virtual ValueTask OnPongResponse(Packets.Status.Clientbound.PongResponsePacket packet) => default;
    protected virtual ValueTask OnServerInfo(Packets.Status.Clientbound.ServerInfoPacket packet) => default;
    // --- Login ---
    protected virtual ValueTask OnLoginCompress(Packets.Login.Clientbound.LoginCompressPacket packet) => default;
    protected virtual ValueTask OnLoginCookieRequest(Packets.Login.Clientbound.LoginCookieRequestPacket packet) => default;
    protected virtual ValueTask OnLoginDisconnect(Packets.Login.Clientbound.LoginDisconnectPacket packet) => default;
    protected virtual ValueTask OnEncryptionRequest(Packets.Login.Clientbound.EncryptionRequestPacket packet) => default;
    protected virtual ValueTask OnLoginPluginRequest(Packets.Login.Clientbound.LoginPluginRequestPacket packet) => default;
    protected virtual ValueTask OnLoginSuccess(Packets.Login.Clientbound.LoginSuccessPacket packet) => default;
    // --- Configuration ---
    protected virtual ValueTask OnConfigurationAddResourcePack(Packets.Configuration.Clientbound.AddResourcePackPacket packet) => default;
    protected virtual ValueTask OnConfigurationClearDialog(Packets.Configuration.Clientbound.ClearDialogPacket packet) => default;
    protected virtual ValueTask OnCodeOfConduct(Packets.Configuration.Clientbound.CodeOfConductPacket packet) => default;
    protected virtual ValueTask OnConfigurationCookieRequest(Packets.Configuration.Clientbound.CookieRequestPacket packet) => default;
    protected virtual ValueTask OnConfigurationCustomPayload(Packets.Configuration.Clientbound.CustomPayloadPacket packet) => default;
    protected virtual ValueTask OnConfigurationCustomReportDetails(Packets.Configuration.Clientbound.CustomReportDetailsPacket packet) => default;
    protected virtual ValueTask OnDisconnect(Packets.Configuration.Clientbound.DisconnectPacket packet) => default;
    protected virtual ValueTask OnConfigurationFeatureFlags(Packets.Configuration.Clientbound.FeatureFlagsPacket packet) => default;
    protected virtual ValueTask OnFinishConfiguration(Packets.Configuration.Clientbound.FinishConfigurationPacket packet) => default;
    protected virtual ValueTask OnConfigurationKeepAlive(Packets.Configuration.Clientbound.KeepAlivePacket packet) => default;
    protected virtual ValueTask OnConfigurationPing(Packets.Configuration.Clientbound.PingPacket packet) => default;
    protected virtual ValueTask OnConfigurationRemoveResourcePack(Packets.Configuration.Clientbound.RemoveResourcePackPacket packet) => default;
    protected virtual ValueTask OnResetChat(Packets.Configuration.Clientbound.ResetChatPacket packet) => default;
    protected virtual ValueTask OnConfigurationResourcePackSend(Packets.Configuration.Clientbound.ResourcePackSendPacket packet) => default;
    protected virtual ValueTask OnSelectKnownPacks(Packets.Configuration.Clientbound.SelectKnownPacksPacket packet) => default;
    protected virtual ValueTask OnShowDialog(Packets.Configuration.Clientbound.ShowDialogPacket packet) => default;
    protected virtual ValueTask OnConfigurationStoreCookie(Packets.Configuration.Clientbound.StoreCookiePacket packet) => default;
    protected virtual ValueTask OnConfigurationTags(Packets.Configuration.Clientbound.TagsPacket packet) => default;
    protected virtual ValueTask OnConfigurationTransfer(Packets.Configuration.Clientbound.TransferPacket packet) => default;
    // --- Play ---
    protected virtual ValueTask OnAbilities(Packets.Play.Clientbound.AbilitiesPacket packet) => default;
    protected virtual ValueTask OnAcknowledgePlayerDigging(Packets.Play.Clientbound.AcknowledgePlayerDiggingPacket packet) => default;
    protected virtual ValueTask OnActionBar(Packets.Play.Clientbound.ActionBarPacket packet) => default;
    protected virtual ValueTask OnAddResourcePack(Packets.Play.Clientbound.AddResourcePackPacket packet) => default;
    protected virtual ValueTask OnAnimation(Packets.Play.Clientbound.AnimationPacket packet) => default;
    protected virtual ValueTask OnAttachEntity(Packets.Play.Clientbound.AttachEntityPacket packet) => default;
    protected virtual ValueTask OnBlockAction(Packets.Play.Clientbound.BlockActionPacket packet) => default;
    protected virtual ValueTask OnBlockBreakAnimation(Packets.Play.Clientbound.BlockBreakAnimationPacket packet) => default;
    protected virtual ValueTask OnBlockChange(Packets.Play.Clientbound.BlockChangePacket packet) => default;
    protected virtual ValueTask OnCamera(Packets.Play.Clientbound.CameraPacket packet) => default;
    protected virtual ValueTask OnChat(Packets.Play.Clientbound.ChatPacket packet) => default;
    protected virtual ValueTask OnChatPreview(Packets.Play.Clientbound.ChatPreviewPacket packet) => default;
    protected virtual ValueTask OnChatSuggestions(Packets.Play.Clientbound.ChatSuggestionsPacket packet) => default;
    protected virtual ValueTask OnChunkBatchFinished(Packets.Play.Clientbound.ChunkBatchFinishedPacket packet) => default;
    protected virtual ValueTask OnChunkBatchStart(Packets.Play.Clientbound.ChunkBatchStartPacket packet) => default;
    protected virtual ValueTask OnChunkBiomes(Packets.Play.Clientbound.ChunkBiomesPacket packet) => default;
    protected virtual ValueTask OnClearDialog(Packets.Play.Clientbound.ClearDialogPacket packet) => default;
    protected virtual ValueTask OnClearTitles(Packets.Play.Clientbound.ClearTitlesPacket packet) => default;
    protected virtual ValueTask OnCloseWindow(Packets.Play.Clientbound.CloseWindowPacket packet) => default;
    protected virtual ValueTask OnCollect(Packets.Play.Clientbound.CollectPacket packet) => default;
    protected virtual ValueTask OnCookieRequest(Packets.Play.Clientbound.CookieRequestPacket packet) => default;
    protected virtual ValueTask OnCraftProgressBar(Packets.Play.Clientbound.CraftProgressBarPacket packet) => default;
    protected virtual ValueTask OnCustomPayload(Packets.Play.Clientbound.CustomPayloadPacket packet) => default;
    protected virtual ValueTask OnCustomReportDetails(Packets.Play.Clientbound.CustomReportDetailsPacket packet) => default;
    protected virtual ValueTask OnDamageEvent(Packets.Play.Clientbound.DamageEventPacket packet) => default;
    protected virtual ValueTask OnDeathCombatEvent(Packets.Play.Clientbound.DeathCombatEventPacket packet) => default;
    protected virtual ValueTask OnDebugSample(Packets.Play.Clientbound.DebugSamplePacket packet) => default;
    protected virtual ValueTask OnDestroyEntity(Packets.Play.Clientbound.DestroyEntityPacket packet) => default;
    protected virtual ValueTask OnDifficulty(Packets.Play.Clientbound.DifficultyPacket packet) => default;
    protected virtual ValueTask OnEndCombatEvent(Packets.Play.Clientbound.EndCombatEventPacket packet) => default;
    protected virtual ValueTask OnEnterCombatEvent(Packets.Play.Clientbound.EnterCombatEventPacket packet) => default;
    protected virtual ValueTask OnEntity(Packets.Play.Clientbound.EntityPacket packet) => default;
    protected virtual ValueTask OnEntityDestroy(Packets.Play.Clientbound.EntityDestroyPacket packet) => default;
    protected virtual ValueTask OnEntityHeadRotation(Packets.Play.Clientbound.EntityHeadRotationPacket packet) => default;
    protected virtual ValueTask OnEntityLook(Packets.Play.Clientbound.EntityLookPacket packet) => default;
    protected virtual ValueTask OnEntityMoveLook(Packets.Play.Clientbound.EntityMoveLookPacket packet) => default;
    protected virtual ValueTask OnEntityStatus(Packets.Play.Clientbound.EntityStatusPacket packet) => default;
    protected virtual ValueTask OnEntityTeleport(Packets.Play.Clientbound.EntityTeleportPacket packet) => default;
    protected virtual ValueTask OnEntityUpdateAttributes(Packets.Play.Clientbound.EntityUpdateAttributesPacket packet) => default;
    protected virtual ValueTask OnExperience(Packets.Play.Clientbound.ExperiencePacket packet) => default;
    protected virtual ValueTask OnFeatureFlags(Packets.Play.Clientbound.FeatureFlagsPacket packet) => default;
    protected virtual ValueTask OnGameRuleValues(Packets.Play.Clientbound.GameRuleValuesPacket packet) => default;
    protected virtual ValueTask OnGameStateChange(Packets.Play.Clientbound.GameStateChangePacket packet) => default;
    protected virtual ValueTask OnGameTestHighlightPos(Packets.Play.Clientbound.GameTestHighlightPosPacket packet) => default;
    protected virtual ValueTask OnHeldItemSlot(Packets.Play.Clientbound.HeldItemSlotPacket packet) => default;
    protected virtual ValueTask OnHurtAnimation(Packets.Play.Clientbound.HurtAnimationPacket packet) => default;
    protected virtual ValueTask OnInitializeWorldBorder(Packets.Play.Clientbound.InitializeWorldBorderPacket packet) => default;
    protected virtual ValueTask OnKeepAlive(Packets.Play.Clientbound.KeepAlivePacket packet) => default;
    protected virtual ValueTask OnKickDisconnect(Packets.Play.Clientbound.KickDisconnectPacket packet) => default;
    protected virtual ValueTask OnLogin(Packets.Play.Clientbound.LoginPacket packet) => default;
    protected virtual ValueTask OnLowDiskSpaceWarning(Packets.Play.Clientbound.LowDiskSpaceWarningPacket packet) => default;
    protected virtual ValueTask OnMessageHeader(Packets.Play.Clientbound.MessageHeaderPacket packet) => default;
    protected virtual ValueTask OnMoveMinecart(Packets.Play.Clientbound.MoveMinecartPacket packet) => default;
    protected virtual ValueTask OnNamedEntitySpawn(Packets.Play.Clientbound.NamedEntitySpawnPacket packet) => default;
    protected virtual ValueTask OnNamedSoundEffect(Packets.Play.Clientbound.NamedSoundEffectPacket packet) => default;
    protected virtual ValueTask OnNbtQueryResponse(Packets.Play.Clientbound.NbtQueryResponsePacket packet) => default;
    protected virtual ValueTask OnOpenBook(Packets.Play.Clientbound.OpenBookPacket packet) => default;
    protected virtual ValueTask OnOpenHorseWindow(Packets.Play.Clientbound.OpenHorseWindowPacket packet) => default;
    protected virtual ValueTask OnOpenSignEntity(Packets.Play.Clientbound.OpenSignEntityPacket packet) => default;
    protected virtual ValueTask OnOpenWindow(Packets.Play.Clientbound.OpenWindowPacket packet) => default;
    protected virtual ValueTask OnPing(Packets.Play.Clientbound.PingPacket packet) => default;
    protected virtual ValueTask OnPingResponse(Packets.Play.Clientbound.PingResponsePacket packet) => default;
    protected virtual ValueTask OnPlayerRemove(Packets.Play.Clientbound.PlayerRemovePacket packet) => default;
    protected virtual ValueTask OnPlayerRotation(Packets.Play.Clientbound.PlayerRotationPacket packet) => default;
    protected virtual ValueTask OnPlayerlistHeader(Packets.Play.Clientbound.PlayerlistHeaderPacket packet) => default;
    protected virtual ValueTask OnPlayerPosition(Packets.Play.Clientbound.PlayerPositionPacket packet) => default;
    protected virtual ValueTask OnRecipeBookRemove(Packets.Play.Clientbound.RecipeBookRemovePacket packet) => default;
    protected virtual ValueTask OnRelEntityMove(Packets.Play.Clientbound.RelEntityMovePacket packet) => default;
    protected virtual ValueTask OnRemoveEntityEffect(Packets.Play.Clientbound.RemoveEntityEffectPacket packet) => default;
    protected virtual ValueTask OnRemoveResourcePack(Packets.Play.Clientbound.RemoveResourcePackPacket packet) => default;
    protected virtual ValueTask OnResetScore(Packets.Play.Clientbound.ResetScorePacket packet) => default;
    protected virtual ValueTask OnResourcePackSend(Packets.Play.Clientbound.ResourcePackSendPacket packet) => default;
    protected virtual ValueTask OnRespawn(Packets.Play.Clientbound.RespawnPacket packet) => default;
    protected virtual ValueTask OnScoreboardDisplayObjective(Packets.Play.Clientbound.ScoreboardDisplayObjectivePacket packet) => default;
    protected virtual ValueTask OnSelectAdvancementTab(Packets.Play.Clientbound.SelectAdvancementTabPacket packet) => default;
    protected virtual ValueTask OnServerData(Packets.Play.Clientbound.ServerDataPacket packet) => default;
    protected virtual ValueTask OnSetCooldown(Packets.Play.Clientbound.SetCooldownPacket packet) => default;
    protected virtual ValueTask OnSetPassengers(Packets.Play.Clientbound.SetPassengersPacket packet) => default;
    protected virtual ValueTask OnSetProjectilePower(Packets.Play.Clientbound.SetProjectilePowerPacket packet) => default;
    protected virtual ValueTask OnSetTickingState(Packets.Play.Clientbound.SetTickingStatePacket packet) => default;
    protected virtual ValueTask OnSetTitleSubtitle(Packets.Play.Clientbound.SetTitleSubtitlePacket packet) => default;
    protected virtual ValueTask OnSetTitleText(Packets.Play.Clientbound.SetTitleTextPacket packet) => default;
    protected virtual ValueTask OnSetTitleTime(Packets.Play.Clientbound.SetTitleTimePacket packet) => default;
    protected virtual ValueTask OnShouldDisplayChatPreview(Packets.Play.Clientbound.ShouldDisplayChatPreviewPacket packet) => default;
    protected virtual ValueTask OnSimulationDistance(Packets.Play.Clientbound.SimulationDistancePacket packet) => default;
    protected virtual ValueTask OnSpawnEntityExperienceOrb(Packets.Play.Clientbound.SpawnEntityExperienceOrbPacket packet) => default;
    protected virtual ValueTask OnSpawnEntityLiving(Packets.Play.Clientbound.SpawnEntityLivingPacket packet) => default;
    protected virtual ValueTask OnSpawnEntityPainting(Packets.Play.Clientbound.SpawnEntityPaintingPacket packet) => default;
    protected virtual ValueTask OnSpawnPosition(Packets.Play.Clientbound.SpawnPositionPacket packet) => default;
    protected virtual ValueTask OnStartConfiguration(Packets.Play.Clientbound.StartConfigurationPacket packet) => default;
    protected virtual ValueTask OnStatistics(Packets.Play.Clientbound.StatisticsPacket packet) => default;
    protected virtual ValueTask OnStepTick(Packets.Play.Clientbound.StepTickPacket packet) => default;
    protected virtual ValueTask OnStoreCookie(Packets.Play.Clientbound.StoreCookiePacket packet) => default;
    protected virtual ValueTask OnSyncEntityPosition(Packets.Play.Clientbound.SyncEntityPositionPacket packet) => default;
    protected virtual ValueTask OnSystemChat(Packets.Play.Clientbound.SystemChatPacket packet) => default;
    protected virtual ValueTask OnTabComplete(Packets.Play.Clientbound.TabCompletePacket packet) => default;
    protected virtual ValueTask OnTags(Packets.Play.Clientbound.TagsPacket packet) => default;
    protected virtual ValueTask OnTeams(Packets.Play.Clientbound.TeamsPacket packet) => default;
    protected virtual ValueTask OnTestInstanceBlockStatus(Packets.Play.Clientbound.TestInstanceBlockStatusPacket packet) => default;
    protected virtual ValueTask OnTileEntityData(Packets.Play.Clientbound.TileEntityDataPacket packet) => default;
    protected virtual ValueTask OnTransaction(Packets.Play.Clientbound.TransactionPacket packet) => default;
    protected virtual ValueTask OnTransfer(Packets.Play.Clientbound.TransferPacket packet) => default;
    protected virtual ValueTask OnUnloadChunk(Packets.Play.Clientbound.UnloadChunkPacket packet) => default;
    protected virtual ValueTask OnUpdateHealth(Packets.Play.Clientbound.UpdateHealthPacket packet) => default;
    protected virtual ValueTask OnUpdateTime(Packets.Play.Clientbound.UpdateTimePacket packet) => default;
    protected virtual ValueTask OnUpdateViewDistance(Packets.Play.Clientbound.UpdateViewDistancePacket packet) => default;
    protected virtual ValueTask OnUpdateViewPosition(Packets.Play.Clientbound.UpdateViewPositionPacket packet) => default;
    protected virtual ValueTask OnVehicleMove(Packets.Play.Clientbound.VehicleMovePacket packet) => default;
    protected virtual ValueTask OnWorldBorderCenter(Packets.Play.Clientbound.WorldBorderCenterPacket packet) => default;
    protected virtual ValueTask OnWorldBorderLerpSize(Packets.Play.Clientbound.WorldBorderLerpSizePacket packet) => default;
    protected virtual ValueTask OnWorldBorderSize(Packets.Play.Clientbound.WorldBorderSizePacket packet) => default;
    protected virtual ValueTask OnWorldBorderWarningDelay(Packets.Play.Clientbound.WorldBorderWarningDelayPacket packet) => default;
    protected virtual ValueTask OnWorldBorderWarningReach(Packets.Play.Clientbound.WorldBorderWarningReachPacket packet) => default;
    protected virtual ValueTask OnWorldEvent(Packets.Play.Clientbound.WorldEventPacket packet) => default;
}
