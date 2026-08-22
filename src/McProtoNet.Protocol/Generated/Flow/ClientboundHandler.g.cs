using System.Threading.Tasks;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol;
/// <summary>Generated handler base over every clientbound phase. The truth about
/// the current phase is the consumer's: set <see cref = "Phase"/> as the connection
/// advances. <c>HandleAsync</c> decodes synchronously (the raw data window must not
/// cross an await) and awaits the handler's result after. <c>OnUnknown</c> must not
/// hold on to <c>raw</c> beyond the call.</summary>
public abstract partial class ClientboundHandler
{
    public PacketPhase Phase { get; protected set; } = PacketPhase.Login;
    protected static PacketDirection Direction => PacketDirection.Clientbound;

    /// <summary>The registry lookup and the typed read happen here, in a case block where
    /// the packet type is statically known, so nothing between the wire and
    /// <c>On&lt;Name&gt;</c> is dynamic. <see cref = "Phase"/> is read once: a handler that
    /// advances the phase does so after the switch, and this packet is read as the phase
    /// it arrived in.</summary>
    public ValueTask HandleAsync(in IncomingPacket raw, int protocolVersion)
    {
        var phase = Phase;
        if (!PacketRegistry.TryGetOrdinal(raw.Id, protocolVersion, phase, PacketDirection.Clientbound, out var ordinal))
            return OnUnknown(in raw);
        var reader = new MinecraftPrimitiveReader(raw.Body);
        ValueTask pending;
        switch (phase)
        {
            case PacketPhase.Status:
                switch (ordinal)
                {
                    case 0:
                    {
                        var packet = Packets.Status.Clientbound.PongResponsePacket.Read(ref reader, protocolVersion);
                        pending = OnPongResponse(packet);
                        break;
                    }

                    case 1:
                    {
                        var packet = Packets.Status.Clientbound.ServerInfoPacket.Read(ref reader, protocolVersion);
                        pending = OnServerInfo(packet);
                        break;
                    }

                    default:
                        return OnUnknown(in raw);
                }

                break;
            case PacketPhase.Login:
                switch (ordinal)
                {
                    case 0:
                    {
                        var packet = Packets.Login.Clientbound.LoginCompressPacket.Read(ref reader, protocolVersion);
                        pending = OnLoginCompress(packet);
                        break;
                    }

                    case 1:
                    {
                        var packet = Packets.Login.Clientbound.LoginCookieRequestPacket.Read(ref reader, protocolVersion);
                        pending = OnLoginCookieRequest(packet);
                        break;
                    }

                    case 2:
                    {
                        var packet = Packets.Login.Clientbound.LoginDisconnectPacket.Read(ref reader, protocolVersion);
                        pending = OnLoginDisconnect(packet);
                        break;
                    }

                    case 3:
                    {
                        var packet = Packets.Login.Clientbound.EncryptionRequestPacket.Read(ref reader, protocolVersion);
                        pending = OnEncryptionRequest(packet);
                        break;
                    }

                    case 4:
                    {
                        var packet = Packets.Login.Clientbound.LoginPluginRequestPacket.Read(ref reader, protocolVersion);
                        pending = OnLoginPluginRequest(packet);
                        break;
                    }

                    case 5:
                    {
                        var packet = Packets.Login.Clientbound.LoginSuccessPacket.Read(ref reader, protocolVersion);
                        pending = OnLoginSuccess(packet);
                        break;
                    }

                    default:
                        return OnUnknown(in raw);
                }

                break;
            case PacketPhase.Configuration:
                switch (ordinal)
                {
                    case 0:
                    {
                        var packet = Packets.Configuration.Clientbound.AddResourcePackPacket.Read(ref reader, protocolVersion);
                        pending = OnConfigurationAddResourcePack(packet);
                        break;
                    }

                    case 1:
                    {
                        var packet = Packets.Configuration.Clientbound.ClearDialogPacket.Read(ref reader, protocolVersion);
                        pending = OnConfigurationClearDialog(packet);
                        break;
                    }

                    case 2:
                    {
                        var packet = Packets.Configuration.Clientbound.CodeOfConductPacket.Read(ref reader, protocolVersion);
                        pending = OnCodeOfConduct(packet);
                        break;
                    }

                    case 3:
                    {
                        var packet = Packets.Configuration.Clientbound.CookieRequestPacket.Read(ref reader, protocolVersion);
                        pending = OnConfigurationCookieRequest(packet);
                        break;
                    }

                    case 4:
                    {
                        var packet = Packets.Configuration.Clientbound.CustomPayloadPacket.Read(ref reader, protocolVersion);
                        pending = OnConfigurationCustomPayload(packet);
                        break;
                    }

                    case 5:
                    {
                        var packet = Packets.Configuration.Clientbound.CustomReportDetailsPacket.Read(ref reader, protocolVersion);
                        pending = OnConfigurationCustomReportDetails(packet);
                        break;
                    }

                    case 6:
                    {
                        var packet = Packets.Configuration.Clientbound.DisconnectPacket.Read(ref reader, protocolVersion);
                        pending = OnDisconnect(packet);
                        break;
                    }

                    case 7:
                    {
                        var packet = Packets.Configuration.Clientbound.FeatureFlagsPacket.Read(ref reader, protocolVersion);
                        pending = OnConfigurationFeatureFlags(packet);
                        break;
                    }

                    case 8:
                    {
                        var packet = Packets.Configuration.Clientbound.FinishConfigurationPacket.Read(ref reader, protocolVersion);
                        pending = OnFinishConfiguration(packet);
                        break;
                    }

                    case 9:
                    {
                        var packet = Packets.Configuration.Clientbound.KeepAlivePacket.Read(ref reader, protocolVersion);
                        pending = OnConfigurationKeepAlive(packet);
                        break;
                    }

                    case 10:
                    {
                        var packet = Packets.Configuration.Clientbound.PingPacket.Read(ref reader, protocolVersion);
                        pending = OnConfigurationPing(packet);
                        break;
                    }

                    case 11:
                    {
                        var packet = Packets.Configuration.Clientbound.RemoveResourcePackPacket.Read(ref reader, protocolVersion);
                        pending = OnConfigurationRemoveResourcePack(packet);
                        break;
                    }

                    case 12:
                    {
                        var packet = Packets.Configuration.Clientbound.ResetChatPacket.Read(ref reader, protocolVersion);
                        pending = OnResetChat(packet);
                        break;
                    }

                    case 13:
                    {
                        var packet = Packets.Configuration.Clientbound.ResourcePackSendPacket.Read(ref reader, protocolVersion);
                        pending = OnConfigurationResourcePackSend(packet);
                        break;
                    }

                    case 14:
                    {
                        var packet = Packets.Configuration.Clientbound.SelectKnownPacksPacket.Read(ref reader, protocolVersion);
                        pending = OnSelectKnownPacks(packet);
                        break;
                    }

                    case 15:
                    {
                        var packet = Packets.Configuration.Clientbound.ShowDialogPacket.Read(ref reader, protocolVersion);
                        pending = OnShowDialog(packet);
                        break;
                    }

                    case 16:
                    {
                        var packet = Packets.Configuration.Clientbound.StoreCookiePacket.Read(ref reader, protocolVersion);
                        pending = OnConfigurationStoreCookie(packet);
                        break;
                    }

                    case 17:
                    {
                        var packet = Packets.Configuration.Clientbound.TagsPacket.Read(ref reader, protocolVersion);
                        pending = OnConfigurationTags(packet);
                        break;
                    }

                    case 18:
                    {
                        var packet = Packets.Configuration.Clientbound.TransferPacket.Read(ref reader, protocolVersion);
                        pending = OnConfigurationTransfer(packet);
                        break;
                    }

                    default:
                        return OnUnknown(in raw);
                }

                break;
            case PacketPhase.Play:
                switch (ordinal)
                {
                    case 0:
                    {
                        var packet = Packets.Play.Clientbound.AbilitiesPacket.Read(ref reader, protocolVersion);
                        pending = OnAbilities(packet);
                        break;
                    }

                    case 1:
                    {
                        var packet = Packets.Play.Clientbound.AcknowledgePlayerDiggingPacket.Read(ref reader, protocolVersion);
                        pending = OnAcknowledgePlayerDigging(packet);
                        break;
                    }

                    case 2:
                    {
                        var packet = Packets.Play.Clientbound.ActionBarPacket.Read(ref reader, protocolVersion);
                        pending = OnActionBar(packet);
                        break;
                    }

                    case 3:
                    {
                        var packet = Packets.Play.Clientbound.AddResourcePackPacket.Read(ref reader, protocolVersion);
                        pending = OnAddResourcePack(packet);
                        break;
                    }

                    case 4:
                    {
                        var packet = Packets.Play.Clientbound.AnimationPacket.Read(ref reader, protocolVersion);
                        pending = OnAnimation(packet);
                        break;
                    }

                    case 5:
                    {
                        var packet = Packets.Play.Clientbound.AttachEntityPacket.Read(ref reader, protocolVersion);
                        pending = OnAttachEntity(packet);
                        break;
                    }

                    case 6:
                    {
                        var packet = Packets.Play.Clientbound.BlockActionPacket.Read(ref reader, protocolVersion);
                        pending = OnBlockAction(packet);
                        break;
                    }

                    case 7:
                    {
                        var packet = Packets.Play.Clientbound.BlockBreakAnimationPacket.Read(ref reader, protocolVersion);
                        pending = OnBlockBreakAnimation(packet);
                        break;
                    }

                    case 8:
                    {
                        var packet = Packets.Play.Clientbound.BlockChangePacket.Read(ref reader, protocolVersion);
                        pending = OnBlockChange(packet);
                        break;
                    }

                    case 9:
                    {
                        var packet = Packets.Play.Clientbound.CameraPacket.Read(ref reader, protocolVersion);
                        pending = OnCamera(packet);
                        break;
                    }

                    case 10:
                    {
                        var packet = Packets.Play.Clientbound.ChatPacket.Read(ref reader, protocolVersion);
                        pending = OnChat(packet);
                        break;
                    }

                    case 11:
                    {
                        var packet = Packets.Play.Clientbound.ChatPreviewPacket.Read(ref reader, protocolVersion);
                        pending = OnChatPreview(packet);
                        break;
                    }

                    case 12:
                    {
                        var packet = Packets.Play.Clientbound.ChatSuggestionsPacket.Read(ref reader, protocolVersion);
                        pending = OnChatSuggestions(packet);
                        break;
                    }

                    case 13:
                    {
                        var packet = Packets.Play.Clientbound.ChunkBatchFinishedPacket.Read(ref reader, protocolVersion);
                        pending = OnChunkBatchFinished(packet);
                        break;
                    }

                    case 14:
                    {
                        var packet = Packets.Play.Clientbound.ChunkBatchStartPacket.Read(ref reader, protocolVersion);
                        pending = OnChunkBatchStart(packet);
                        break;
                    }

                    case 15:
                    {
                        var packet = Packets.Play.Clientbound.ChunkBiomesPacket.Read(ref reader, protocolVersion);
                        pending = OnChunkBiomes(packet);
                        break;
                    }

                    case 16:
                    {
                        var packet = Packets.Play.Clientbound.ClearDialogPacket.Read(ref reader, protocolVersion);
                        pending = OnClearDialog(packet);
                        break;
                    }

                    case 17:
                    {
                        var packet = Packets.Play.Clientbound.ClearTitlesPacket.Read(ref reader, protocolVersion);
                        pending = OnClearTitles(packet);
                        break;
                    }

                    case 18:
                    {
                        var packet = Packets.Play.Clientbound.CloseWindowPacket.Read(ref reader, protocolVersion);
                        pending = OnCloseWindow(packet);
                        break;
                    }

                    case 19:
                    {
                        var packet = Packets.Play.Clientbound.CollectPacket.Read(ref reader, protocolVersion);
                        pending = OnCollect(packet);
                        break;
                    }

                    case 20:
                    {
                        var packet = Packets.Play.Clientbound.CookieRequestPacket.Read(ref reader, protocolVersion);
                        pending = OnCookieRequest(packet);
                        break;
                    }

                    case 21:
                    {
                        var packet = Packets.Play.Clientbound.CraftProgressBarPacket.Read(ref reader, protocolVersion);
                        pending = OnCraftProgressBar(packet);
                        break;
                    }

                    case 22:
                    {
                        var packet = Packets.Play.Clientbound.CustomPayloadPacket.Read(ref reader, protocolVersion);
                        pending = OnCustomPayload(packet);
                        break;
                    }

                    case 23:
                    {
                        var packet = Packets.Play.Clientbound.CustomReportDetailsPacket.Read(ref reader, protocolVersion);
                        pending = OnCustomReportDetails(packet);
                        break;
                    }

                    case 24:
                    {
                        var packet = Packets.Play.Clientbound.DamageEventPacket.Read(ref reader, protocolVersion);
                        pending = OnDamageEvent(packet);
                        break;
                    }

                    case 25:
                    {
                        var packet = Packets.Play.Clientbound.DeathCombatEventPacket.Read(ref reader, protocolVersion);
                        pending = OnDeathCombatEvent(packet);
                        break;
                    }

                    case 26:
                    {
                        var packet = Packets.Play.Clientbound.DebugSamplePacket.Read(ref reader, protocolVersion);
                        pending = OnDebugSample(packet);
                        break;
                    }

                    case 27:
                    {
                        var packet = Packets.Play.Clientbound.DestroyEntityPacket.Read(ref reader, protocolVersion);
                        pending = OnDestroyEntity(packet);
                        break;
                    }

                    case 28:
                    {
                        var packet = Packets.Play.Clientbound.DifficultyPacket.Read(ref reader, protocolVersion);
                        pending = OnDifficulty(packet);
                        break;
                    }

                    case 29:
                    {
                        var packet = Packets.Play.Clientbound.EndCombatEventPacket.Read(ref reader, protocolVersion);
                        pending = OnEndCombatEvent(packet);
                        break;
                    }

                    case 30:
                    {
                        var packet = Packets.Play.Clientbound.EnterCombatEventPacket.Read(ref reader, protocolVersion);
                        pending = OnEnterCombatEvent(packet);
                        break;
                    }

                    case 31:
                    {
                        var packet = Packets.Play.Clientbound.EntityPacket.Read(ref reader, protocolVersion);
                        pending = OnEntity(packet);
                        break;
                    }

                    case 32:
                    {
                        var packet = Packets.Play.Clientbound.EntityDestroyPacket.Read(ref reader, protocolVersion);
                        pending = OnEntityDestroy(packet);
                        break;
                    }

                    case 33:
                    {
                        var packet = Packets.Play.Clientbound.EntityHeadRotationPacket.Read(ref reader, protocolVersion);
                        pending = OnEntityHeadRotation(packet);
                        break;
                    }

                    case 34:
                    {
                        var packet = Packets.Play.Clientbound.EntityLookPacket.Read(ref reader, protocolVersion);
                        pending = OnEntityLook(packet);
                        break;
                    }

                    case 36:
                    {
                        var packet = Packets.Play.Clientbound.EntityMoveLookPacket.Read(ref reader, protocolVersion);
                        pending = OnEntityMoveLook(packet);
                        break;
                    }

                    case 37:
                    {
                        var packet = Packets.Play.Clientbound.EntityStatusPacket.Read(ref reader, protocolVersion);
                        pending = OnEntityStatus(packet);
                        break;
                    }

                    case 38:
                    {
                        var packet = Packets.Play.Clientbound.EntityTeleportPacket.Read(ref reader, protocolVersion);
                        pending = OnEntityTeleport(packet);
                        break;
                    }

                    case 39:
                    {
                        var packet = Packets.Play.Clientbound.EntityUpdateAttributesPacket.Read(ref reader, protocolVersion);
                        pending = OnEntityUpdateAttributes(packet);
                        break;
                    }

                    case 41:
                    {
                        var packet = Packets.Play.Clientbound.ExperiencePacket.Read(ref reader, protocolVersion);
                        pending = OnExperience(packet);
                        break;
                    }

                    case 43:
                    {
                        var packet = Packets.Play.Clientbound.FeatureFlagsPacket.Read(ref reader, protocolVersion);
                        pending = OnFeatureFlags(packet);
                        break;
                    }

                    case 44:
                    {
                        var packet = Packets.Play.Clientbound.GameRuleValuesPacket.Read(ref reader, protocolVersion);
                        pending = OnGameRuleValues(packet);
                        break;
                    }

                    case 45:
                    {
                        var packet = Packets.Play.Clientbound.GameStateChangePacket.Read(ref reader, protocolVersion);
                        pending = OnGameStateChange(packet);
                        break;
                    }

                    case 46:
                    {
                        var packet = Packets.Play.Clientbound.GameTestHighlightPosPacket.Read(ref reader, protocolVersion);
                        pending = OnGameTestHighlightPos(packet);
                        break;
                    }

                    case 47:
                    {
                        var packet = Packets.Play.Clientbound.HeldItemSlotPacket.Read(ref reader, protocolVersion);
                        pending = OnHeldItemSlot(packet);
                        break;
                    }

                    case 48:
                    {
                        var packet = Packets.Play.Clientbound.HurtAnimationPacket.Read(ref reader, protocolVersion);
                        pending = OnHurtAnimation(packet);
                        break;
                    }

                    case 49:
                    {
                        var packet = Packets.Play.Clientbound.InitializeWorldBorderPacket.Read(ref reader, protocolVersion);
                        pending = OnInitializeWorldBorder(packet);
                        break;
                    }

                    case 50:
                    {
                        var packet = Packets.Play.Clientbound.KeepAlivePacket.Read(ref reader, protocolVersion);
                        pending = OnKeepAlive(packet);
                        break;
                    }

                    case 51:
                    {
                        var packet = Packets.Play.Clientbound.KickDisconnectPacket.Read(ref reader, protocolVersion);
                        pending = OnKickDisconnect(packet);
                        break;
                    }

                    case 52:
                    {
                        var packet = Packets.Play.Clientbound.LoginPacket.Read(ref reader, protocolVersion);
                        pending = OnLogin(packet);
                        break;
                    }

                    case 53:
                    {
                        var packet = Packets.Play.Clientbound.LowDiskSpaceWarningPacket.Read(ref reader, protocolVersion);
                        pending = OnLowDiskSpaceWarning(packet);
                        break;
                    }

                    case 55:
                    {
                        var packet = Packets.Play.Clientbound.MessageHeaderPacket.Read(ref reader, protocolVersion);
                        pending = OnMessageHeader(packet);
                        break;
                    }

                    case 56:
                    {
                        var packet = Packets.Play.Clientbound.MoveMinecartPacket.Read(ref reader, protocolVersion);
                        pending = OnMoveMinecart(packet);
                        break;
                    }

                    case 57:
                    {
                        var packet = Packets.Play.Clientbound.NamedEntitySpawnPacket.Read(ref reader, protocolVersion);
                        pending = OnNamedEntitySpawn(packet);
                        break;
                    }

                    case 58:
                    {
                        var packet = Packets.Play.Clientbound.NamedSoundEffectPacket.Read(ref reader, protocolVersion);
                        pending = OnNamedSoundEffect(packet);
                        break;
                    }

                    case 59:
                    {
                        var packet = Packets.Play.Clientbound.NbtQueryResponsePacket.Read(ref reader, protocolVersion);
                        pending = OnNbtQueryResponse(packet);
                        break;
                    }

                    case 60:
                    {
                        var packet = Packets.Play.Clientbound.OpenBookPacket.Read(ref reader, protocolVersion);
                        pending = OnOpenBook(packet);
                        break;
                    }

                    case 61:
                    {
                        var packet = Packets.Play.Clientbound.OpenHorseWindowPacket.Read(ref reader, protocolVersion);
                        pending = OnOpenHorseWindow(packet);
                        break;
                    }

                    case 62:
                    {
                        var packet = Packets.Play.Clientbound.OpenSignEntityPacket.Read(ref reader, protocolVersion);
                        pending = OnOpenSignEntity(packet);
                        break;
                    }

                    case 63:
                    {
                        var packet = Packets.Play.Clientbound.OpenWindowPacket.Read(ref reader, protocolVersion);
                        pending = OnOpenWindow(packet);
                        break;
                    }

                    case 64:
                    {
                        var packet = Packets.Play.Clientbound.PingPacket.Read(ref reader, protocolVersion);
                        pending = OnPing(packet);
                        break;
                    }

                    case 65:
                    {
                        var packet = Packets.Play.Clientbound.PingResponsePacket.Read(ref reader, protocolVersion);
                        pending = OnPingResponse(packet);
                        break;
                    }

                    case 66:
                    {
                        var packet = Packets.Play.Clientbound.PlayerChatPacket.Read(ref reader, protocolVersion);
                        pending = OnPlayerChat(packet);
                        break;
                    }

                    case 67:
                    {
                        var packet = Packets.Play.Clientbound.PlayerRemovePacket.Read(ref reader, protocolVersion);
                        pending = OnPlayerRemove(packet);
                        break;
                    }

                    case 68:
                    {
                        var packet = Packets.Play.Clientbound.PlayerRotationPacket.Read(ref reader, protocolVersion);
                        pending = OnPlayerRotation(packet);
                        break;
                    }

                    case 69:
                    {
                        var packet = Packets.Play.Clientbound.PlayerlistHeaderPacket.Read(ref reader, protocolVersion);
                        pending = OnPlayerlistHeader(packet);
                        break;
                    }

                    case 70:
                    {
                        var packet = Packets.Play.Clientbound.PlayerPositionPacket.Read(ref reader, protocolVersion);
                        pending = OnPlayerPosition(packet);
                        break;
                    }

                    case 71:
                    {
                        var packet = Packets.Play.Clientbound.ProfilelessChatPacket.Read(ref reader, protocolVersion);
                        pending = OnProfilelessChat(packet);
                        break;
                    }

                    case 72:
                    {
                        var packet = Packets.Play.Clientbound.RecipeBookRemovePacket.Read(ref reader, protocolVersion);
                        pending = OnRecipeBookRemove(packet);
                        break;
                    }

                    case 73:
                    {
                        var packet = Packets.Play.Clientbound.RelEntityMovePacket.Read(ref reader, protocolVersion);
                        pending = OnRelEntityMove(packet);
                        break;
                    }

                    case 74:
                    {
                        var packet = Packets.Play.Clientbound.RemoveEntityEffectPacket.Read(ref reader, protocolVersion);
                        pending = OnRemoveEntityEffect(packet);
                        break;
                    }

                    case 75:
                    {
                        var packet = Packets.Play.Clientbound.RemoveResourcePackPacket.Read(ref reader, protocolVersion);
                        pending = OnRemoveResourcePack(packet);
                        break;
                    }

                    case 76:
                    {
                        var packet = Packets.Play.Clientbound.ResetScorePacket.Read(ref reader, protocolVersion);
                        pending = OnResetScore(packet);
                        break;
                    }

                    case 77:
                    {
                        var packet = Packets.Play.Clientbound.ResourcePackSendPacket.Read(ref reader, protocolVersion);
                        pending = OnResourcePackSend(packet);
                        break;
                    }

                    case 78:
                    {
                        var packet = Packets.Play.Clientbound.RespawnPacket.Read(ref reader, protocolVersion);
                        pending = OnRespawn(packet);
                        break;
                    }

                    case 79:
                    {
                        var packet = Packets.Play.Clientbound.ScoreboardDisplayObjectivePacket.Read(ref reader, protocolVersion);
                        pending = OnScoreboardDisplayObjective(packet);
                        break;
                    }

                    case 80:
                    {
                        var packet = Packets.Play.Clientbound.SelectAdvancementTabPacket.Read(ref reader, protocolVersion);
                        pending = OnSelectAdvancementTab(packet);
                        break;
                    }

                    case 81:
                    {
                        var packet = Packets.Play.Clientbound.ServerDataPacket.Read(ref reader, protocolVersion);
                        pending = OnServerData(packet);
                        break;
                    }

                    case 82:
                    {
                        var packet = Packets.Play.Clientbound.SetCooldownPacket.Read(ref reader, protocolVersion);
                        pending = OnSetCooldown(packet);
                        break;
                    }

                    case 83:
                    {
                        var packet = Packets.Play.Clientbound.SetPassengersPacket.Read(ref reader, protocolVersion);
                        pending = OnSetPassengers(packet);
                        break;
                    }

                    case 84:
                    {
                        var packet = Packets.Play.Clientbound.SetProjectilePowerPacket.Read(ref reader, protocolVersion);
                        pending = OnSetProjectilePower(packet);
                        break;
                    }

                    case 85:
                    {
                        var packet = Packets.Play.Clientbound.SetTickingStatePacket.Read(ref reader, protocolVersion);
                        pending = OnSetTickingState(packet);
                        break;
                    }

                    case 86:
                    {
                        var packet = Packets.Play.Clientbound.SetTitleSubtitlePacket.Read(ref reader, protocolVersion);
                        pending = OnSetTitleSubtitle(packet);
                        break;
                    }

                    case 87:
                    {
                        var packet = Packets.Play.Clientbound.SetTitleTextPacket.Read(ref reader, protocolVersion);
                        pending = OnSetTitleText(packet);
                        break;
                    }

                    case 88:
                    {
                        var packet = Packets.Play.Clientbound.SetTitleTimePacket.Read(ref reader, protocolVersion);
                        pending = OnSetTitleTime(packet);
                        break;
                    }

                    case 89:
                    {
                        var packet = Packets.Play.Clientbound.ShouldDisplayChatPreviewPacket.Read(ref reader, protocolVersion);
                        pending = OnShouldDisplayChatPreview(packet);
                        break;
                    }

                    case 90:
                    {
                        var packet = Packets.Play.Clientbound.SimulationDistancePacket.Read(ref reader, protocolVersion);
                        pending = OnSimulationDistance(packet);
                        break;
                    }

                    case 92:
                    {
                        var packet = Packets.Play.Clientbound.SpawnEntityExperienceOrbPacket.Read(ref reader, protocolVersion);
                        pending = OnSpawnEntityExperienceOrb(packet);
                        break;
                    }

                    case 93:
                    {
                        var packet = Packets.Play.Clientbound.SpawnEntityLivingPacket.Read(ref reader, protocolVersion);
                        pending = OnSpawnEntityLiving(packet);
                        break;
                    }

                    case 94:
                    {
                        var packet = Packets.Play.Clientbound.SpawnEntityPaintingPacket.Read(ref reader, protocolVersion);
                        pending = OnSpawnEntityPainting(packet);
                        break;
                    }

                    case 95:
                    {
                        var packet = Packets.Play.Clientbound.SpawnPositionPacket.Read(ref reader, protocolVersion);
                        pending = OnSpawnPosition(packet);
                        break;
                    }

                    case 96:
                    {
                        var packet = Packets.Play.Clientbound.StartConfigurationPacket.Read(ref reader, protocolVersion);
                        pending = OnStartConfiguration(packet);
                        break;
                    }

                    case 97:
                    {
                        var packet = Packets.Play.Clientbound.StatisticsPacket.Read(ref reader, protocolVersion);
                        pending = OnStatistics(packet);
                        break;
                    }

                    case 98:
                    {
                        var packet = Packets.Play.Clientbound.StepTickPacket.Read(ref reader, protocolVersion);
                        pending = OnStepTick(packet);
                        break;
                    }

                    case 99:
                    {
                        var packet = Packets.Play.Clientbound.StoreCookiePacket.Read(ref reader, protocolVersion);
                        pending = OnStoreCookie(packet);
                        break;
                    }

                    case 100:
                    {
                        var packet = Packets.Play.Clientbound.SyncEntityPositionPacket.Read(ref reader, protocolVersion);
                        pending = OnSyncEntityPosition(packet);
                        break;
                    }

                    case 101:
                    {
                        var packet = Packets.Play.Clientbound.SystemChatPacket.Read(ref reader, protocolVersion);
                        pending = OnSystemChat(packet);
                        break;
                    }

                    case 102:
                    {
                        var packet = Packets.Play.Clientbound.TabCompletePacket.Read(ref reader, protocolVersion);
                        pending = OnTabComplete(packet);
                        break;
                    }

                    case 103:
                    {
                        var packet = Packets.Play.Clientbound.TagsPacket.Read(ref reader, protocolVersion);
                        pending = OnTags(packet);
                        break;
                    }

                    case 104:
                    {
                        var packet = Packets.Play.Clientbound.TeamsPacket.Read(ref reader, protocolVersion);
                        pending = OnTeams(packet);
                        break;
                    }

                    case 105:
                    {
                        var packet = Packets.Play.Clientbound.TestInstanceBlockStatusPacket.Read(ref reader, protocolVersion);
                        pending = OnTestInstanceBlockStatus(packet);
                        break;
                    }

                    case 106:
                    {
                        var packet = Packets.Play.Clientbound.TileEntityDataPacket.Read(ref reader, protocolVersion);
                        pending = OnTileEntityData(packet);
                        break;
                    }

                    case 107:
                    {
                        var packet = Packets.Play.Clientbound.TransactionPacket.Read(ref reader, protocolVersion);
                        pending = OnTransaction(packet);
                        break;
                    }

                    case 108:
                    {
                        var packet = Packets.Play.Clientbound.TransferPacket.Read(ref reader, protocolVersion);
                        pending = OnTransfer(packet);
                        break;
                    }

                    case 109:
                    {
                        var packet = Packets.Play.Clientbound.UnloadChunkPacket.Read(ref reader, protocolVersion);
                        pending = OnUnloadChunk(packet);
                        break;
                    }

                    case 110:
                    {
                        var packet = Packets.Play.Clientbound.UpdateHealthPacket.Read(ref reader, protocolVersion);
                        pending = OnUpdateHealth(packet);
                        break;
                    }

                    case 112:
                    {
                        var packet = Packets.Play.Clientbound.UpdateTimePacket.Read(ref reader, protocolVersion);
                        pending = OnUpdateTime(packet);
                        break;
                    }

                    case 113:
                    {
                        var packet = Packets.Play.Clientbound.UpdateViewDistancePacket.Read(ref reader, protocolVersion);
                        pending = OnUpdateViewDistance(packet);
                        break;
                    }

                    case 114:
                    {
                        var packet = Packets.Play.Clientbound.UpdateViewPositionPacket.Read(ref reader, protocolVersion);
                        pending = OnUpdateViewPosition(packet);
                        break;
                    }

                    case 115:
                    {
                        var packet = Packets.Play.Clientbound.VehicleMovePacket.Read(ref reader, protocolVersion);
                        pending = OnVehicleMove(packet);
                        break;
                    }

                    case 116:
                    {
                        var packet = Packets.Play.Clientbound.WorldBorderCenterPacket.Read(ref reader, protocolVersion);
                        pending = OnWorldBorderCenter(packet);
                        break;
                    }

                    case 117:
                    {
                        var packet = Packets.Play.Clientbound.WorldBorderLerpSizePacket.Read(ref reader, protocolVersion);
                        pending = OnWorldBorderLerpSize(packet);
                        break;
                    }

                    case 118:
                    {
                        var packet = Packets.Play.Clientbound.WorldBorderSizePacket.Read(ref reader, protocolVersion);
                        pending = OnWorldBorderSize(packet);
                        break;
                    }

                    case 119:
                    {
                        var packet = Packets.Play.Clientbound.WorldBorderWarningDelayPacket.Read(ref reader, protocolVersion);
                        pending = OnWorldBorderWarningDelay(packet);
                        break;
                    }

                    case 120:
                    {
                        var packet = Packets.Play.Clientbound.WorldBorderWarningReachPacket.Read(ref reader, protocolVersion);
                        pending = OnWorldBorderWarningReach(packet);
                        break;
                    }

                    case 121:
                    {
                        var packet = Packets.Play.Clientbound.WorldEventPacket.Read(ref reader, protocolVersion);
                        pending = OnWorldEvent(packet);
                        break;
                    }

                    default:
                        return OnUnknown(in raw);
                }

                break;
            default:
                return OnUnknown(in raw);
        }

        if (reader.RemainingCount != 0)
            PacketFlow.RaiseTrailingBytes(raw.Id, protocolVersion, reader.RemainingCount);
        return pending;
    }

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
    protected virtual ValueTask OnPlayerChat(Packets.Play.Clientbound.PlayerChatPacket packet) => default;
    protected virtual ValueTask OnPlayerRemove(Packets.Play.Clientbound.PlayerRemovePacket packet) => default;
    protected virtual ValueTask OnPlayerRotation(Packets.Play.Clientbound.PlayerRotationPacket packet) => default;
    protected virtual ValueTask OnPlayerlistHeader(Packets.Play.Clientbound.PlayerlistHeaderPacket packet) => default;
    protected virtual ValueTask OnPlayerPosition(Packets.Play.Clientbound.PlayerPositionPacket packet) => default;
    protected virtual ValueTask OnProfilelessChat(Packets.Play.Clientbound.ProfilelessChatPacket packet) => default;
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
