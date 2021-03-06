using McProtoNet.Protocol754.Packets.Client;
using McProtoNet.Protocol754.Packets.Server;
using System.Reflection;

namespace McProtoNet.Protocol754
{

    public sealed class PacketCollection754 : AbstractPacketCollection
    {

        public PacketCollection754()
        {
            ClientPackets.Add(PacketCategory.Status, new()
            {
                {0x00, typeof(StatusQueryPacket) },
                {0x01, typeof(StatusPingPacket) }
            });
            ServerPackets.Add(PacketCategory.Status, new()
            {
                {0x00, typeof(StatusResponsePacket) },
                {0x01, typeof(StatusPongPacket) }
            });
            ClientPackets.Add(PacketCategory.HandShake, new()
            {
                {0, typeof(HandShakePacket) }
            });
            ServerPackets.Add(PacketCategory.HandShake, new());

            ClientPackets.Add(PacketCategory.Login, new()
            {
                { 0x00, typeof(LoginStartPacket) },
                { 0x01, typeof(EncryptionResponsePacket) },
                { 0x02, typeof(LoginPluginResponsePacket) },
            });
            ServerPackets.Add(PacketCategory.Login, new()
            {
                { 0x00, typeof(LoginDisconnectPacket) },
                { 0x01, typeof(EncryptionRequestPacket) },
                { 0x02, typeof(LoginSuccessPacket) },
                { 0x03, typeof(LoginSetCompressionPacket) },
                { 0x04, typeof(LoginPluginRequestPacket) },
            });

            ClientPackets.Add(PacketCategory.Game, new()
            {
                { 0x00, typeof(ClientTeleportConfirmPacket) },
                { 0x01, typeof(ClientBlockNBTRequestPacket) },
                { 0x02, typeof(ClientSetDifficultyPacket) },
                { 0x03, typeof(ClientChatPacket) },
                { 0x04, typeof(ClientRequestPacket) },
                { 0x05, typeof(ClientSettingsPacket) },
                { 0x06, typeof(ClientTabCompletePacket) },
                { 0x07, typeof(ClientConfirmTransactionPacket) },
                { 0x08, typeof(ClientClickWindowButtonPacket) },
                { 0x09, typeof(ClientWindowActionPacket) },
                { 0x0A, typeof(ClientCloseWindowPacket) },
                { 0x0B, typeof(ClientPluginMessagePacket) },
                { 0x0C, typeof(ClientEditBookPacket) },
                { 0x0D, typeof(ClientEntityNBTRequestPacket) },
                { 0x0E, typeof(ClientPlayerInteractEntityPacket) },
                { 0x0F, typeof(ClientGenerateStructuresPacket) },
                { 0x10, typeof(ClientKeepAlivePacket) },
                { 0x11, typeof(ClientLockDifficultyPacket) },
                { 0x12, typeof(ClientPlayerPositionPacket) },
                { 0x13, typeof(ClientPlayerPositionRotationPacket) },
                { 0x14, typeof(ClientPlayerRotationPacket) },
                { 0x15, typeof(ClientPlayerMovementPacket) },
                { 0x16, typeof(ClientVehicleMovePacket) },
                { 0x17, typeof(ClientSteerBoatPacket) },
                { 0x18, typeof(ClientMoveItemToHotbarPacket) },
                { 0x19, typeof(ClientPrepareCraftingGridPacket) },
                { 0x1A, typeof(ClientPlayerAbilitiesPacket) },
                { 0x1B, typeof(ClientPlayerActionPacket) },
                { 0x1C, typeof(ClientPlayerStatePacket) },
                { 0x1D, typeof(ClientSteerVehiclePacket) },
                { 0x1E, typeof(ClientCraftingBookStatePacket) },
                { 0x1F, typeof(ClientDisplayedRecipePacket) },
                { 0x20, typeof(ClientRenameItemPacket) },
                { 0x21, typeof(ClientResourcePackStatusPacket) },
                { 0x22, typeof(ClientAdvancementTabPacket) },
                { 0x23, typeof(ClientSelectTradePacket) },
                { 0x24, typeof(ClientSetBeaconEffectPacket) },
                { 0x25, typeof(ClientPlayerChangeHeldItemPacket) },
                { 0x26, typeof(ClientUpdateCommandBlockPacket) },
                { 0x27, typeof(ClientUpdateCommandBlockMinecartPacket) },
                { 0x28, typeof(ClientCreativeInventoryActionPacket) },
                { 0x29, typeof(ClientUpdateJigsawBlockPacket) },
                { 0x2A, typeof(ClientUpdateStructureBlockPacket) },
                { 0x2B, typeof(ClientUpdateSignPacket) },
                { 0x2C, typeof(ClientPlayerSwingArmPacket) },
                { 0x2D, typeof(ClientSpectatePacket) },
                { 0x2E, typeof(ClientPlayerPlaceBlockPacket) },
                { 0x2F, typeof(ClientPlayerUseItemPacket) },
            });

            ServerPackets.Add(PacketCategory.Game, new()
            {
                { 0x00, typeof(ServerSpawnEntityPacket) },
                { 0x01, typeof(ServerSpawnExpOrbPacket) },
                { 0x02, typeof(ServerSpawnLivingEntityPacket) },
                { 0x03, typeof(ServerSpawnPaintingPacket) },
                { 0x04, typeof(ServerSpawnPlayerPacket) },
                { 0x05, typeof(ServerEntityAnimationPacket) },
                { 0x06, typeof(ServerStatisticsPacket) },
                { 0x07, typeof(ServerPlayerActionAckPacket) },
                { 0x08, typeof(ServerBlockBreakAnimPacket) },
                { 0x09, typeof(ServerUpdateTileEntityPacket) },
                { 0x0A, typeof(ServerBlockValuePacket) },
                { 0x0B, typeof(ServerBlockChangePacket) },
                { 0x0C, typeof(ServerBossBarPacket) },
                { 0x0D, typeof(ServerDifficultyPacket) },
                { 0x0E, typeof(ServerChatPacket) },
                { 0x0F, typeof(ServerTabCompletePacket) },
                { 0x10, typeof(ServerDeclareCommandsPacket) },
                { 0x11, typeof(ServerConfirmTransactionPacket) },
                { 0x12, typeof(ServerCloseWindowPacket) },
                { 0x13, typeof(ServerWindowItemsPacket) },
                { 0x14, typeof(ServerWindowPropertyPacket) },
                { 0x15, typeof(ServerSetSlotPacket) },
                { 0x16, typeof(ServerSetCooldownPacket) },
                { 0x17, typeof(ServerPluginMessagePacket) },
                { 0x18, typeof(ServerPlaySoundPacket) },
                { 0x19, typeof(ServerDisconnectPacket) },
                { 0x1A, typeof(ServerEntityStatusPacket) },
                { 0x1B, typeof(ServerExplosionPacket) },
                { 0x1C, typeof(ServerUnloadChunkPacket) },
                { 0x1D, typeof(ServerNotifyClientPacket) },
                { 0x1E, typeof(ServerOpenHorseWindowPacket) },
                { 0x1F, typeof(ServerKeepAlivePacket) },
                { 0x20, typeof(ServerChunkDataPacket) },
                { 0x21, typeof(ServerPlayEffectPacket) },
                { 0x22, typeof(ServerSpawnParticlePacket) },
                { 0x23, typeof(ServerUpdateLightPacket) },
                { 0x24, typeof(ServerJoinGamePacket) },
                { 0x25, typeof(ServerMapDataPacket) },
                { 0x26, typeof(ServerTradeListPacket) },
                { 0x27, typeof(ServerEntityPositionPacket) },
                { 0x28, typeof(ServerEntityPositionRotationPacket) },
                { 0x29, typeof(ServerEntityRotationPacket) },
                { 0x2A, typeof(ServerEntityMovementPacket) },
                { 0x2B, typeof(ServerVehicleMovePacket) },
                { 0x2C, typeof(ServerOpenBookPacket) },
                { 0x2D, typeof(ServerOpenWindowPacket) },
                { 0x2E, typeof(ServerOpenTileEntityEditorPacket) },
                { 0x2F, typeof(ServerPreparedCraftingGridPacket) },
                { 0x30, typeof(ServerPlayerAbilitiesPacket) },
                { 0x31, typeof(ServerCombatPacket) },
                { 0x32, typeof(ServerPlayerListEntryPacket) },
                { 0x33, typeof(ServerPlayerFacingPacket) },
                { 0x34, typeof(ServerPlayerPositionRotationPacket) },
                { 0x35, typeof(ServerUnlockRecipesPacket) },
                { 0x36, typeof(ServerEntityDestroyPacket) },
                { 0x37, typeof(ServerEntityRemoveEffectPacket) },
                { 0x38, typeof(ServerResourcePackSendPacket) },
                { 0x39, typeof(ServerRespawnPacket) },
                { 0x3A, typeof(ServerEntityHeadLookPacket) },
                { 0x3B, typeof(ServerMultiBlockChangePacket) },
                { 0x3C, typeof(ServerAdvancementTabPacket) },
                { 0x3D, typeof(ServerWorldBorderPacket) },
                { 0x3E, typeof(ServerSwitchCameraPacket) },
                { 0x3F, typeof(ServerPlayerChangeHeldItemPacket) },
                { 0x40, typeof(ServerUpdateViewPositionPacket) },
                { 0x41, typeof(ServerUpdateViewDistancePacket) },
                { 0x42, typeof(ServerSpawnPositionPacket) },
                { 0x43, typeof(ServerDisplayScoreboardPacket) },
                { 0x44, typeof(ServerEntityMetadataPacket) },
                { 0x45, typeof(ServerEntityAttachPacket) },
                { 0x46, typeof(ServerEntityVelocityPacket) },
                { 0x47, typeof(ServerEntityEquipmentPacket) },
                { 0x48, typeof(ServerPlayerSetExperiencePacket) },
                { 0x49, typeof(ServerPlayerHealthPacket) },
                { 0x4A, typeof(ServerScoreboardObjectivePacket) },
                { 0x4B, typeof(ServerEntitySetPassengersPacket) },
                { 0x4C, typeof(ServerTeamPacket) },
                { 0x4D, typeof(ServerUpdateScorePacket) },
                { 0x4E, typeof(ServerUpdateTimePacket) },
                { 0x4F, typeof(ServerTitlePacket) },
                { 0x50, typeof(ServerEntitySoundEffectPacket) },
                { 0x51, typeof(ServerPlayBuiltinSoundPacket) },
                { 0x52, typeof(ServerStopSoundPacket) },
                { 0x53, typeof(ServerPlayerListDataPacket) },
                { 0x54, typeof(ServerNBTResponsePacket) },
                { 0x55, typeof(ServerEntityCollectItemPacket) },
                { 0x56, typeof(ServerEntityTeleportPacket) },
                { 0x57, typeof(ServerAdvancementsPacket) },
                { 0x58, typeof(ServerEntityPropertiesPacket) },
                { 0x59, typeof(ServerEntityEffectPacket) },
                { 0x5A, typeof(ServerDeclareRecipesPacket) },
                { 0x5B, typeof(ServerDeclareTagsPacket) },
            });
        }

        public override int TargetProtocolVersion
        {
            get
            {
                ThrowIfDisposed();
                return 754;
            }
        }

        public override Dictionary<int, Type> GetClientPacketsByCategory(PacketCategory category)
        {
            ThrowIfDisposed();
            return ClientPackets[category];
        }

        public override Dictionary<int, Type> GetServerPacketsByCategory(PacketCategory category)
        {
            ThrowIfDisposed();
            return ServerPackets[category];
        }

        public override Dictionary<PacketCategory, IPacketProvider> GetAllPackets(PacketSide side)
        {
            ThrowIfDisposed();


            var categories = new List<PacketCategory>
                {
                    PacketCategory.HandShake,
                    //PacketCategory.Status,
                    PacketCategory.Login,
                    PacketCategory.Game
                };

            if (side == PacketSide.Client)
            {

                var all = categories
                    .ToDictionary(k => k, v => (IPacketProvider)new PacketProvider(ClientPackets[v], ServerPackets[v]));

                return all;
            }
            else
            {

                var all = categories
                    .ToDictionary(k => k, v => (IPacketProvider)new PacketProvider(ServerPackets[v], ClientPackets[v]));

                return all;
            }
        }
    }
}
