using System.Threading.Tasks;
using McProtoNet.Primitives;

namespace McProtoNet.Protocol;
/// <summary>Generated handler base over every serverbound phase. The truth about
/// the current phase is the consumer's: set <see cref = "Phase"/> as the connection
/// advances. <c>HandleAsync</c> decodes synchronously (the raw data window must not
/// cross an await) and awaits the handler's result after. <c>OnUnknown</c> must not
/// hold on to <c>raw</c> beyond the call.</summary>
public abstract partial class ServerboundHandler
{
    public PacketPhase Phase { get; protected set; } = PacketPhase.Handshaking;
    protected static PacketDirection Direction => PacketDirection.Serverbound;

    /// <summary>The registry lookup and the typed read happen here, in a case block where
    /// the packet type is statically known, so nothing between the wire and
    /// <c>On&lt;Name&gt;</c> is dynamic. <see cref = "Phase"/> is read once: a handler that
    /// advances the phase does so after the switch, and this packet is read as the phase
    /// it arrived in.</summary>
    public ValueTask HandleAsync(in IncomingPacket raw, int protocolVersion)
    {
        var phase = Phase;
        if (!PacketRegistry.TryGetOrdinal(raw.Id, protocolVersion, phase, PacketDirection.Serverbound, out var ordinal))
            return OnUnknown(in raw);
        var reader = new MinecraftPrimitiveReader(raw.Body);
        ValueTask pending;
        switch (phase)
        {
            case PacketPhase.Handshaking:
                switch (ordinal)
                {
                    case 0:
                    {
                        var packet = Packets.Handshaking.Serverbound.LegacyServerListPingPacket.Read(ref reader, protocolVersion);
                        pending = OnLegacyServerListPing(packet);
                        break;
                    }

                    case 1:
                    {
                        var packet = Packets.Handshaking.Serverbound.SetProtocolPacket.Read(ref reader, protocolVersion);
                        pending = OnSetProtocol(packet);
                        break;
                    }

                    default:
                        return OnUnknown(in raw);
                }

                break;
            case PacketPhase.Status:
                switch (ordinal)
                {
                    case 0:
                    {
                        var packet = Packets.Status.Serverbound.PingRequestPacket.Read(ref reader, protocolVersion);
                        pending = OnStatusPingRequest(packet);
                        break;
                    }

                    case 1:
                    {
                        var packet = Packets.Status.Serverbound.PingStartPacket.Read(ref reader, protocolVersion);
                        pending = OnPingStart(packet);
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
                        var packet = Packets.Login.Serverbound.LoginCookieResponsePacket.Read(ref reader, protocolVersion);
                        pending = OnLoginCookieResponse(packet);
                        break;
                    }

                    case 2:
                    {
                        var packet = Packets.Login.Serverbound.LoginAcknowledgedPacket.Read(ref reader, protocolVersion);
                        pending = OnLoginAcknowledged(packet);
                        break;
                    }

                    case 3:
                    {
                        var packet = Packets.Login.Serverbound.LoginPluginResponsePacket.Read(ref reader, protocolVersion);
                        pending = OnLoginPluginResponse(packet);
                        break;
                    }

                    case 4:
                    {
                        var packet = Packets.Login.Serverbound.LoginStartPacket.Read(ref reader, protocolVersion);
                        pending = OnLoginStart(packet);
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
                        var packet = Packets.Configuration.Serverbound.AcceptCodeOfConductPacket.Read(ref reader, protocolVersion);
                        pending = OnAcceptCodeOfConduct(packet);
                        break;
                    }

                    case 1:
                    {
                        var packet = Packets.Configuration.Serverbound.CookieResponsePacket.Read(ref reader, protocolVersion);
                        pending = OnConfigurationCookieResponse(packet);
                        break;
                    }

                    case 2:
                    {
                        var packet = Packets.Configuration.Serverbound.CustomClickActionPacket.Read(ref reader, protocolVersion);
                        pending = OnConfigurationCustomClickAction(packet);
                        break;
                    }

                    case 3:
                    {
                        var packet = Packets.Configuration.Serverbound.CustomPayloadPacket.Read(ref reader, protocolVersion);
                        pending = OnConfigurationCustomPayload(packet);
                        break;
                    }

                    case 4:
                    {
                        var packet = Packets.Configuration.Serverbound.CustomReportDetailsPacket.Read(ref reader, protocolVersion);
                        pending = OnCustomReportDetails(packet);
                        break;
                    }

                    case 5:
                    {
                        var packet = Packets.Configuration.Serverbound.FinishConfigurationPacket.Read(ref reader, protocolVersion);
                        pending = OnFinishConfiguration(packet);
                        break;
                    }

                    case 6:
                    {
                        var packet = Packets.Configuration.Serverbound.KeepAlivePacket.Read(ref reader, protocolVersion);
                        pending = OnConfigurationKeepAlive(packet);
                        break;
                    }

                    case 7:
                    {
                        var packet = Packets.Configuration.Serverbound.PongPacket.Read(ref reader, protocolVersion);
                        pending = OnConfigurationPong(packet);
                        break;
                    }

                    case 8:
                    {
                        var packet = Packets.Configuration.Serverbound.ResourcePackReceivePacket.Read(ref reader, protocolVersion);
                        pending = OnConfigurationResourcePackReceive(packet);
                        break;
                    }

                    case 9:
                    {
                        var packet = Packets.Configuration.Serverbound.SelectKnownPacksPacket.Read(ref reader, protocolVersion);
                        pending = OnSelectKnownPacks(packet);
                        break;
                    }

                    case 10:
                    {
                        var packet = Packets.Configuration.Serverbound.ClientInformationPacket.Read(ref reader, protocolVersion);
                        pending = OnClientInformation(packet);
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
                        var packet = Packets.Play.Serverbound.AbilitiesPacket.Read(ref reader, protocolVersion);
                        pending = OnAbilities(packet);
                        break;
                    }

                    case 1:
                    {
                        var packet = Packets.Play.Serverbound.ArmAnimationPacket.Read(ref reader, protocolVersion);
                        pending = OnArmAnimation(packet);
                        break;
                    }

                    case 2:
                    {
                        var packet = Packets.Play.Serverbound.AttackPacket.Read(ref reader, protocolVersion);
                        pending = OnAttack(packet);
                        break;
                    }

                    case 3:
                    {
                        var packet = Packets.Play.Serverbound.BlockDigPacket.Read(ref reader, protocolVersion);
                        pending = OnBlockDig(packet);
                        break;
                    }

                    case 4:
                    {
                        var packet = Packets.Play.Serverbound.BlockPlacePacket.Read(ref reader, protocolVersion);
                        pending = OnBlockPlace(packet);
                        break;
                    }

                    case 5:
                    {
                        var packet = Packets.Play.Serverbound.ChangeGamemodePacket.Read(ref reader, protocolVersion);
                        pending = OnChangeGamemode(packet);
                        break;
                    }

                    case 6:
                    {
                        var packet = Packets.Play.Serverbound.ChatPacket.Read(ref reader, protocolVersion);
                        pending = OnChat(packet);
                        break;
                    }

                    case 7:
                    {
                        var packet = Packets.Play.Serverbound.ChatCommandSignedPacket.Read(ref reader, protocolVersion);
                        pending = OnChatCommandSigned(packet);
                        break;
                    }

                    case 8:
                    {
                        var packet = Packets.Play.Serverbound.ChatMessagePacket.Read(ref reader, protocolVersion);
                        pending = OnChatMessage(packet);
                        break;
                    }

                    case 9:
                    {
                        var packet = Packets.Play.Serverbound.ChatPreviewPacket.Read(ref reader, protocolVersion);
                        pending = OnChatPreview(packet);
                        break;
                    }

                    case 10:
                    {
                        var packet = Packets.Play.Serverbound.ChatSessionUpdatePacket.Read(ref reader, protocolVersion);
                        pending = OnChatSessionUpdate(packet);
                        break;
                    }

                    case 11:
                    {
                        var packet = Packets.Play.Serverbound.ChunkBatchReceivedPacket.Read(ref reader, protocolVersion);
                        pending = OnChunkBatchReceived(packet);
                        break;
                    }

                    case 12:
                    {
                        var packet = Packets.Play.Serverbound.ClientCommandPacket.Read(ref reader, protocolVersion);
                        pending = OnClientCommand(packet);
                        break;
                    }

                    case 13:
                    {
                        var packet = Packets.Play.Serverbound.CloseWindowPacket.Read(ref reader, protocolVersion);
                        pending = OnCloseWindow(packet);
                        break;
                    }

                    case 14:
                    {
                        var packet = Packets.Play.Serverbound.ConfigurationAcknowledgedPacket.Read(ref reader, protocolVersion);
                        pending = OnConfigurationAcknowledged(packet);
                        break;
                    }

                    case 15:
                    {
                        var packet = Packets.Play.Serverbound.CookieResponsePacket.Read(ref reader, protocolVersion);
                        pending = OnCookieResponse(packet);
                        break;
                    }

                    case 16:
                    {
                        var packet = Packets.Play.Serverbound.CraftRecipeRequestPacket.Read(ref reader, protocolVersion);
                        pending = OnCraftRecipeRequest(packet);
                        break;
                    }

                    case 17:
                    {
                        var packet = Packets.Play.Serverbound.CustomClickActionPacket.Read(ref reader, protocolVersion);
                        pending = OnCustomClickAction(packet);
                        break;
                    }

                    case 18:
                    {
                        var packet = Packets.Play.Serverbound.CustomPayloadPacket.Read(ref reader, protocolVersion);
                        pending = OnCustomPayload(packet);
                        break;
                    }

                    case 19:
                    {
                        var packet = Packets.Play.Serverbound.DebugSampleSubscriptionPacket.Read(ref reader, protocolVersion);
                        pending = OnDebugSampleSubscription(packet);
                        break;
                    }

                    case 20:
                    {
                        var packet = Packets.Play.Serverbound.DisplayedRecipePacket.Read(ref reader, protocolVersion);
                        pending = OnDisplayedRecipe(packet);
                        break;
                    }

                    case 21:
                    {
                        var packet = Packets.Play.Serverbound.EnchantItemPacket.Read(ref reader, protocolVersion);
                        pending = OnEnchantItem(packet);
                        break;
                    }

                    case 22:
                    {
                        var packet = Packets.Play.Serverbound.EntityActionPacket.Read(ref reader, protocolVersion);
                        pending = OnEntityAction(packet);
                        break;
                    }

                    case 23:
                    {
                        var packet = Packets.Play.Serverbound.FlyingPacket.Read(ref reader, protocolVersion);
                        pending = OnFlying(packet);
                        break;
                    }

                    case 24:
                    {
                        var packet = Packets.Play.Serverbound.GenerateStructurePacket.Read(ref reader, protocolVersion);
                        pending = OnGenerateStructure(packet);
                        break;
                    }

                    case 25:
                    {
                        var packet = Packets.Play.Serverbound.HeldItemSlotPacket.Read(ref reader, protocolVersion);
                        pending = OnHeldItemSlot(packet);
                        break;
                    }

                    case 26:
                    {
                        var packet = Packets.Play.Serverbound.KeepAlivePacket.Read(ref reader, protocolVersion);
                        pending = OnKeepAlive(packet);
                        break;
                    }

                    case 27:
                    {
                        var packet = Packets.Play.Serverbound.LockDifficultyPacket.Read(ref reader, protocolVersion);
                        pending = OnLockDifficulty(packet);
                        break;
                    }

                    case 28:
                    {
                        var packet = Packets.Play.Serverbound.LookPacket.Read(ref reader, protocolVersion);
                        pending = OnLook(packet);
                        break;
                    }

                    case 29:
                    {
                        var packet = Packets.Play.Serverbound.NameItemPacket.Read(ref reader, protocolVersion);
                        pending = OnNameItem(packet);
                        break;
                    }

                    case 30:
                    {
                        var packet = Packets.Play.Serverbound.PickItemPacket.Read(ref reader, protocolVersion);
                        pending = OnPickItem(packet);
                        break;
                    }

                    case 31:
                    {
                        var packet = Packets.Play.Serverbound.PickItemFromBlockPacket.Read(ref reader, protocolVersion);
                        pending = OnPickItemFromBlock(packet);
                        break;
                    }

                    case 32:
                    {
                        var packet = Packets.Play.Serverbound.PickItemFromEntityPacket.Read(ref reader, protocolVersion);
                        pending = OnPickItemFromEntity(packet);
                        break;
                    }

                    case 33:
                    {
                        var packet = Packets.Play.Serverbound.PingRequestPacket.Read(ref reader, protocolVersion);
                        pending = OnPingRequest(packet);
                        break;
                    }

                    case 34:
                    {
                        var packet = Packets.Play.Serverbound.PlayerInputPacket.Read(ref reader, protocolVersion);
                        pending = OnPlayerInput(packet);
                        break;
                    }

                    case 35:
                    {
                        var packet = Packets.Play.Serverbound.PlayerLoadedPacket.Read(ref reader, protocolVersion);
                        pending = OnPlayerLoaded(packet);
                        break;
                    }

                    case 36:
                    {
                        var packet = Packets.Play.Serverbound.PongPacket.Read(ref reader, protocolVersion);
                        pending = OnPong(packet);
                        break;
                    }

                    case 37:
                    {
                        var packet = Packets.Play.Serverbound.PositionPacket.Read(ref reader, protocolVersion);
                        pending = OnPosition(packet);
                        break;
                    }

                    case 38:
                    {
                        var packet = Packets.Play.Serverbound.PositionLookPacket.Read(ref reader, protocolVersion);
                        pending = OnPositionLook(packet);
                        break;
                    }

                    case 39:
                    {
                        var packet = Packets.Play.Serverbound.QueryBlockNbtPacket.Read(ref reader, protocolVersion);
                        pending = OnQueryBlockNbt(packet);
                        break;
                    }

                    case 40:
                    {
                        var packet = Packets.Play.Serverbound.QueryEntityNbtPacket.Read(ref reader, protocolVersion);
                        pending = OnQueryEntityNbt(packet);
                        break;
                    }

                    case 41:
                    {
                        var packet = Packets.Play.Serverbound.RecipeBookPacket.Read(ref reader, protocolVersion);
                        pending = OnRecipeBook(packet);
                        break;
                    }

                    case 42:
                    {
                        var packet = Packets.Play.Serverbound.ResourcePackReceivePacket.Read(ref reader, protocolVersion);
                        pending = OnResourcePackReceive(packet);
                        break;
                    }

                    case 43:
                    {
                        var packet = Packets.Play.Serverbound.SelectBundleItemPacket.Read(ref reader, protocolVersion);
                        pending = OnSelectBundleItem(packet);
                        break;
                    }

                    case 44:
                    {
                        var packet = Packets.Play.Serverbound.SelectTradePacket.Read(ref reader, protocolVersion);
                        pending = OnSelectTrade(packet);
                        break;
                    }

                    case 45:
                    {
                        var packet = Packets.Play.Serverbound.SetBeaconEffectPacket.Read(ref reader, protocolVersion);
                        pending = OnSetBeaconEffect(packet);
                        break;
                    }

                    case 46:
                    {
                        var packet = Packets.Play.Serverbound.SetDifficultyPacket.Read(ref reader, protocolVersion);
                        pending = OnSetDifficulty(packet);
                        break;
                    }

                    case 47:
                    {
                        var packet = Packets.Play.Serverbound.SetGameRulePacket.Read(ref reader, protocolVersion);
                        pending = OnSetGameRule(packet);
                        break;
                    }

                    case 48:
                    {
                        var packet = Packets.Play.Serverbound.SetSlotStatePacket.Read(ref reader, protocolVersion);
                        pending = OnSetSlotState(packet);
                        break;
                    }

                    case 49:
                    {
                        var packet = Packets.Play.Serverbound.SetTestBlockPacket.Read(ref reader, protocolVersion);
                        pending = OnSetTestBlock(packet);
                        break;
                    }

                    case 50:
                    {
                        var packet = Packets.Play.Serverbound.SpectatePacket.Read(ref reader, protocolVersion);
                        pending = OnSpectate(packet);
                        break;
                    }

                    case 51:
                    {
                        var packet = Packets.Play.Serverbound.SpectateEntityPacket.Read(ref reader, protocolVersion);
                        pending = OnSpectateEntity(packet);
                        break;
                    }

                    case 52:
                    {
                        var packet = Packets.Play.Serverbound.SteerBoatPacket.Read(ref reader, protocolVersion);
                        pending = OnSteerBoat(packet);
                        break;
                    }

                    case 53:
                    {
                        var packet = Packets.Play.Serverbound.SteerVehiclePacket.Read(ref reader, protocolVersion);
                        pending = OnSteerVehicle(packet);
                        break;
                    }

                    case 54:
                    {
                        var packet = Packets.Play.Serverbound.TabCompletePacket.Read(ref reader, protocolVersion);
                        pending = OnTabComplete(packet);
                        break;
                    }

                    case 55:
                    {
                        var packet = Packets.Play.Serverbound.TeleportConfirmPacket.Read(ref reader, protocolVersion);
                        pending = OnTeleportConfirm(packet);
                        break;
                    }

                    case 56:
                    {
                        var packet = Packets.Play.Serverbound.TickEndPacket.Read(ref reader, protocolVersion);
                        pending = OnTickEnd(packet);
                        break;
                    }

                    case 57:
                    {
                        var packet = Packets.Play.Serverbound.TransactionPacket.Read(ref reader, protocolVersion);
                        pending = OnTransaction(packet);
                        break;
                    }

                    case 58:
                    {
                        var packet = Packets.Play.Serverbound.UpdateCommandBlockPacket.Read(ref reader, protocolVersion);
                        pending = OnUpdateCommandBlock(packet);
                        break;
                    }

                    case 59:
                    {
                        var packet = Packets.Play.Serverbound.UpdateCommandBlockMinecartPacket.Read(ref reader, protocolVersion);
                        pending = OnUpdateCommandBlockMinecart(packet);
                        break;
                    }

                    case 60:
                    {
                        var packet = Packets.Play.Serverbound.UpdateJigsawBlockPacket.Read(ref reader, protocolVersion);
                        pending = OnUpdateJigsawBlock(packet);
                        break;
                    }

                    case 61:
                    {
                        var packet = Packets.Play.Serverbound.UpdateSignPacket.Read(ref reader, protocolVersion);
                        pending = OnUpdateSign(packet);
                        break;
                    }

                    case 62:
                    {
                        var packet = Packets.Play.Serverbound.UpdateStructureBlockPacket.Read(ref reader, protocolVersion);
                        pending = OnUpdateStructureBlock(packet);
                        break;
                    }

                    case 64:
                    {
                        var packet = Packets.Play.Serverbound.UseItemPacket.Read(ref reader, protocolVersion);
                        pending = OnUseItem(packet);
                        break;
                    }

                    case 65:
                    {
                        var packet = Packets.Play.Serverbound.VehicleMovePacket.Read(ref reader, protocolVersion);
                        pending = OnVehicleMove(packet);
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
    // --- Handshaking ---
    protected virtual ValueTask OnLegacyServerListPing(Packets.Handshaking.Serverbound.LegacyServerListPingPacket packet) => default;
    protected virtual ValueTask OnSetProtocol(Packets.Handshaking.Serverbound.SetProtocolPacket packet) => default;
    // --- Status ---
    protected virtual ValueTask OnStatusPingRequest(Packets.Status.Serverbound.PingRequestPacket packet) => default;
    protected virtual ValueTask OnPingStart(Packets.Status.Serverbound.PingStartPacket packet) => default;
    // --- Login ---
    protected virtual ValueTask OnLoginCookieResponse(Packets.Login.Serverbound.LoginCookieResponsePacket packet) => default;
    protected virtual ValueTask OnLoginAcknowledged(Packets.Login.Serverbound.LoginAcknowledgedPacket packet) => default;
    protected virtual ValueTask OnLoginPluginResponse(Packets.Login.Serverbound.LoginPluginResponsePacket packet) => default;
    protected virtual ValueTask OnLoginStart(Packets.Login.Serverbound.LoginStartPacket packet) => default;
    // --- Configuration ---
    protected virtual ValueTask OnAcceptCodeOfConduct(Packets.Configuration.Serverbound.AcceptCodeOfConductPacket packet) => default;
    protected virtual ValueTask OnConfigurationCookieResponse(Packets.Configuration.Serverbound.CookieResponsePacket packet) => default;
    protected virtual ValueTask OnConfigurationCustomClickAction(Packets.Configuration.Serverbound.CustomClickActionPacket packet) => default;
    protected virtual ValueTask OnConfigurationCustomPayload(Packets.Configuration.Serverbound.CustomPayloadPacket packet) => default;
    protected virtual ValueTask OnCustomReportDetails(Packets.Configuration.Serverbound.CustomReportDetailsPacket packet) => default;
    protected virtual ValueTask OnFinishConfiguration(Packets.Configuration.Serverbound.FinishConfigurationPacket packet) => default;
    protected virtual ValueTask OnConfigurationKeepAlive(Packets.Configuration.Serverbound.KeepAlivePacket packet) => default;
    protected virtual ValueTask OnConfigurationPong(Packets.Configuration.Serverbound.PongPacket packet) => default;
    protected virtual ValueTask OnConfigurationResourcePackReceive(Packets.Configuration.Serverbound.ResourcePackReceivePacket packet) => default;
    protected virtual ValueTask OnSelectKnownPacks(Packets.Configuration.Serverbound.SelectKnownPacksPacket packet) => default;
    protected virtual ValueTask OnClientInformation(Packets.Configuration.Serverbound.ClientInformationPacket packet) => default;
    // --- Play ---
    protected virtual ValueTask OnAbilities(Packets.Play.Serverbound.AbilitiesPacket packet) => default;
    protected virtual ValueTask OnArmAnimation(Packets.Play.Serverbound.ArmAnimationPacket packet) => default;
    protected virtual ValueTask OnAttack(Packets.Play.Serverbound.AttackPacket packet) => default;
    protected virtual ValueTask OnBlockDig(Packets.Play.Serverbound.BlockDigPacket packet) => default;
    protected virtual ValueTask OnBlockPlace(Packets.Play.Serverbound.BlockPlacePacket packet) => default;
    protected virtual ValueTask OnChangeGamemode(Packets.Play.Serverbound.ChangeGamemodePacket packet) => default;
    protected virtual ValueTask OnChat(Packets.Play.Serverbound.ChatPacket packet) => default;
    protected virtual ValueTask OnChatCommandSigned(Packets.Play.Serverbound.ChatCommandSignedPacket packet) => default;
    protected virtual ValueTask OnChatMessage(Packets.Play.Serverbound.ChatMessagePacket packet) => default;
    protected virtual ValueTask OnChatPreview(Packets.Play.Serverbound.ChatPreviewPacket packet) => default;
    protected virtual ValueTask OnChatSessionUpdate(Packets.Play.Serverbound.ChatSessionUpdatePacket packet) => default;
    protected virtual ValueTask OnChunkBatchReceived(Packets.Play.Serverbound.ChunkBatchReceivedPacket packet) => default;
    protected virtual ValueTask OnClientCommand(Packets.Play.Serverbound.ClientCommandPacket packet) => default;
    protected virtual ValueTask OnCloseWindow(Packets.Play.Serverbound.CloseWindowPacket packet) => default;
    protected virtual ValueTask OnConfigurationAcknowledged(Packets.Play.Serverbound.ConfigurationAcknowledgedPacket packet) => default;
    protected virtual ValueTask OnCookieResponse(Packets.Play.Serverbound.CookieResponsePacket packet) => default;
    protected virtual ValueTask OnCraftRecipeRequest(Packets.Play.Serverbound.CraftRecipeRequestPacket packet) => default;
    protected virtual ValueTask OnCustomClickAction(Packets.Play.Serverbound.CustomClickActionPacket packet) => default;
    protected virtual ValueTask OnCustomPayload(Packets.Play.Serverbound.CustomPayloadPacket packet) => default;
    protected virtual ValueTask OnDebugSampleSubscription(Packets.Play.Serverbound.DebugSampleSubscriptionPacket packet) => default;
    protected virtual ValueTask OnDisplayedRecipe(Packets.Play.Serverbound.DisplayedRecipePacket packet) => default;
    protected virtual ValueTask OnEnchantItem(Packets.Play.Serverbound.EnchantItemPacket packet) => default;
    protected virtual ValueTask OnEntityAction(Packets.Play.Serverbound.EntityActionPacket packet) => default;
    protected virtual ValueTask OnFlying(Packets.Play.Serverbound.FlyingPacket packet) => default;
    protected virtual ValueTask OnGenerateStructure(Packets.Play.Serverbound.GenerateStructurePacket packet) => default;
    protected virtual ValueTask OnHeldItemSlot(Packets.Play.Serverbound.HeldItemSlotPacket packet) => default;
    protected virtual ValueTask OnKeepAlive(Packets.Play.Serverbound.KeepAlivePacket packet) => default;
    protected virtual ValueTask OnLockDifficulty(Packets.Play.Serverbound.LockDifficultyPacket packet) => default;
    protected virtual ValueTask OnLook(Packets.Play.Serverbound.LookPacket packet) => default;
    protected virtual ValueTask OnNameItem(Packets.Play.Serverbound.NameItemPacket packet) => default;
    protected virtual ValueTask OnPickItem(Packets.Play.Serverbound.PickItemPacket packet) => default;
    protected virtual ValueTask OnPickItemFromBlock(Packets.Play.Serverbound.PickItemFromBlockPacket packet) => default;
    protected virtual ValueTask OnPickItemFromEntity(Packets.Play.Serverbound.PickItemFromEntityPacket packet) => default;
    protected virtual ValueTask OnPingRequest(Packets.Play.Serverbound.PingRequestPacket packet) => default;
    protected virtual ValueTask OnPlayerInput(Packets.Play.Serverbound.PlayerInputPacket packet) => default;
    protected virtual ValueTask OnPlayerLoaded(Packets.Play.Serverbound.PlayerLoadedPacket packet) => default;
    protected virtual ValueTask OnPong(Packets.Play.Serverbound.PongPacket packet) => default;
    protected virtual ValueTask OnPosition(Packets.Play.Serverbound.PositionPacket packet) => default;
    protected virtual ValueTask OnPositionLook(Packets.Play.Serverbound.PositionLookPacket packet) => default;
    protected virtual ValueTask OnQueryBlockNbt(Packets.Play.Serverbound.QueryBlockNbtPacket packet) => default;
    protected virtual ValueTask OnQueryEntityNbt(Packets.Play.Serverbound.QueryEntityNbtPacket packet) => default;
    protected virtual ValueTask OnRecipeBook(Packets.Play.Serverbound.RecipeBookPacket packet) => default;
    protected virtual ValueTask OnResourcePackReceive(Packets.Play.Serverbound.ResourcePackReceivePacket packet) => default;
    protected virtual ValueTask OnSelectBundleItem(Packets.Play.Serverbound.SelectBundleItemPacket packet) => default;
    protected virtual ValueTask OnSelectTrade(Packets.Play.Serverbound.SelectTradePacket packet) => default;
    protected virtual ValueTask OnSetBeaconEffect(Packets.Play.Serverbound.SetBeaconEffectPacket packet) => default;
    protected virtual ValueTask OnSetDifficulty(Packets.Play.Serverbound.SetDifficultyPacket packet) => default;
    protected virtual ValueTask OnSetGameRule(Packets.Play.Serverbound.SetGameRulePacket packet) => default;
    protected virtual ValueTask OnSetSlotState(Packets.Play.Serverbound.SetSlotStatePacket packet) => default;
    protected virtual ValueTask OnSetTestBlock(Packets.Play.Serverbound.SetTestBlockPacket packet) => default;
    protected virtual ValueTask OnSpectate(Packets.Play.Serverbound.SpectatePacket packet) => default;
    protected virtual ValueTask OnSpectateEntity(Packets.Play.Serverbound.SpectateEntityPacket packet) => default;
    protected virtual ValueTask OnSteerBoat(Packets.Play.Serverbound.SteerBoatPacket packet) => default;
    protected virtual ValueTask OnSteerVehicle(Packets.Play.Serverbound.SteerVehiclePacket packet) => default;
    protected virtual ValueTask OnTabComplete(Packets.Play.Serverbound.TabCompletePacket packet) => default;
    protected virtual ValueTask OnTeleportConfirm(Packets.Play.Serverbound.TeleportConfirmPacket packet) => default;
    protected virtual ValueTask OnTickEnd(Packets.Play.Serverbound.TickEndPacket packet) => default;
    protected virtual ValueTask OnTransaction(Packets.Play.Serverbound.TransactionPacket packet) => default;
    protected virtual ValueTask OnUpdateCommandBlock(Packets.Play.Serverbound.UpdateCommandBlockPacket packet) => default;
    protected virtual ValueTask OnUpdateCommandBlockMinecart(Packets.Play.Serverbound.UpdateCommandBlockMinecartPacket packet) => default;
    protected virtual ValueTask OnUpdateJigsawBlock(Packets.Play.Serverbound.UpdateJigsawBlockPacket packet) => default;
    protected virtual ValueTask OnUpdateSign(Packets.Play.Serverbound.UpdateSignPacket packet) => default;
    protected virtual ValueTask OnUpdateStructureBlock(Packets.Play.Serverbound.UpdateStructureBlockPacket packet) => default;
    protected virtual ValueTask OnUseItem(Packets.Play.Serverbound.UseItemPacket packet) => default;
    protected virtual ValueTask OnVehicleMove(Packets.Play.Serverbound.VehicleMovePacket packet) => default;
}
