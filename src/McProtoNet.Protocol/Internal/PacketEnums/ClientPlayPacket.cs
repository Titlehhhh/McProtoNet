namespace McProtoNet.Protocol;

public static class ClientPlayPacket
{
    public static PacketIdentifier Abilities =>
        new(0, nameof(Abilities), PacketState.Play, PacketDirection.Serverbound);

    public static PacketIdentifier AdvancementTab =>
        new(1, nameof(AdvancementTab), PacketState.Play, PacketDirection.Serverbound);

    public static PacketIdentifier ArmAnimation =>
        new(2, nameof(ArmAnimation), PacketState.Play, PacketDirection.Serverbound);

    public static PacketIdentifier BlockDig => new(3, nameof(BlockDig), PacketState.Play, PacketDirection.Serverbound);

    public static PacketIdentifier BlockPlace =>
        new(4, nameof(BlockPlace), PacketState.Play, PacketDirection.Serverbound);

    public static PacketIdentifier Chat => new(5, nameof(Chat), PacketState.Play, PacketDirection.Serverbound);

    public static PacketIdentifier ChatCommand =>
        new(6, nameof(ChatCommand), PacketState.Play, PacketDirection.Serverbound);

    public static PacketIdentifier ChatCommandSigned =>
        new(7, nameof(ChatCommandSigned), PacketState.Play, PacketDirection.Serverbound);

    public static PacketIdentifier ChatMessage =>
        new(8, nameof(ChatMessage), PacketState.Play, PacketDirection.Serverbound);

    public static PacketIdentifier ChatPreview =>
        new(9, nameof(ChatPreview), PacketState.Play, PacketDirection.Serverbound);

    public static PacketIdentifier ChatSessionUpdate =>
        new(10, nameof(ChatSessionUpdate), PacketState.Play, PacketDirection.Serverbound);

    public static PacketIdentifier ChunkBatchReceived =>
        new(11, nameof(ChunkBatchReceived), PacketState.Play, PacketDirection.Serverbound);

    public static PacketIdentifier ClientCommand =>
        new(12, nameof(ClientCommand), PacketState.Play, PacketDirection.Serverbound);

    public static PacketIdentifier CloseWindow =>
        new(13, nameof(CloseWindow), PacketState.Play, PacketDirection.Serverbound);

    public static PacketIdentifier ConfigurationAcknowledged => new(14, nameof(ConfigurationAcknowledged),
        PacketState.Play, PacketDirection.Serverbound);

    public static PacketIdentifier CookieResponse =>
        new(15, nameof(CookieResponse), PacketState.Play, PacketDirection.Serverbound);

    public static PacketIdentifier CraftRecipeRequest =>
        new(16, nameof(CraftRecipeRequest), PacketState.Play, PacketDirection.Serverbound);

    public static PacketIdentifier CraftingBookData =>
        new(17, nameof(CraftingBookData), PacketState.Play, PacketDirection.Serverbound);

    public static PacketIdentifier CustomPayload =>
        new(18, nameof(CustomPayload), PacketState.Play, PacketDirection.Serverbound);

    public static PacketIdentifier DebugSampleSubscription => new(19, nameof(DebugSampleSubscription), PacketState.Play,
        PacketDirection.Serverbound);

    public static PacketIdentifier DisplayedRecipe =>
        new(20, nameof(DisplayedRecipe), PacketState.Play, PacketDirection.Serverbound);

    public static PacketIdentifier EditBook => new(21, nameof(EditBook), PacketState.Play, PacketDirection.Serverbound);

    public static PacketIdentifier EnchantItem =>
        new(22, nameof(EnchantItem), PacketState.Play, PacketDirection.Serverbound);

    public static PacketIdentifier EntityAction =>
        new(23, nameof(EntityAction), PacketState.Play, PacketDirection.Serverbound);

    public static PacketIdentifier Flying => new(24, nameof(Flying), PacketState.Play, PacketDirection.Serverbound);

    public static PacketIdentifier GenerateStructure =>
        new(25, nameof(GenerateStructure), PacketState.Play, PacketDirection.Serverbound);

    public static PacketIdentifier HeldItemSlot =>
        new(26, nameof(HeldItemSlot), PacketState.Play, PacketDirection.Serverbound);

    public static PacketIdentifier KeepAlive =>
        new(27, nameof(KeepAlive), PacketState.Play, PacketDirection.Serverbound);

    public static PacketIdentifier LockDifficulty =>
        new(28, nameof(LockDifficulty), PacketState.Play, PacketDirection.Serverbound);

    public static PacketIdentifier Look => new(29, nameof(Look), PacketState.Play, PacketDirection.Serverbound);

    public static PacketIdentifier MessageAcknowledgement => new(30, nameof(MessageAcknowledgement), PacketState.Play,
        PacketDirection.Serverbound);

    public static PacketIdentifier NameItem => new(31, nameof(NameItem), PacketState.Play, PacketDirection.Serverbound);
    public static PacketIdentifier PickItem => new(32, nameof(PickItem), PacketState.Play, PacketDirection.Serverbound);

    public static PacketIdentifier PickItemFromBlock =>
        new(33, nameof(PickItemFromBlock), PacketState.Play, PacketDirection.Serverbound);

    public static PacketIdentifier PickItemFromEntity =>
        new(34, nameof(PickItemFromEntity), PacketState.Play, PacketDirection.Serverbound);

    public static PacketIdentifier PingRequest =>
        new(35, nameof(PingRequest), PacketState.Play, PacketDirection.Serverbound);

    public static PacketIdentifier PlayerInput =>
        new(36, nameof(PlayerInput), PacketState.Play, PacketDirection.Serverbound);

    public static PacketIdentifier PlayerLoaded =>
        new(37, nameof(PlayerLoaded), PacketState.Play, PacketDirection.Serverbound);

    public static PacketIdentifier Pong => new(38, nameof(Pong), PacketState.Play, PacketDirection.Serverbound);
    public static PacketIdentifier Position => new(39, nameof(Position), PacketState.Play, PacketDirection.Serverbound);

    public static PacketIdentifier PositionLook =>
        new(40, nameof(PositionLook), PacketState.Play, PacketDirection.Serverbound);

    public static PacketIdentifier QueryBlockNbt =>
        new(41, nameof(QueryBlockNbt), PacketState.Play, PacketDirection.Serverbound);

    public static PacketIdentifier QueryEntityNbt =>
        new(42, nameof(QueryEntityNbt), PacketState.Play, PacketDirection.Serverbound);

    public static PacketIdentifier RecipeBook =>
        new(43, nameof(RecipeBook), PacketState.Play, PacketDirection.Serverbound);

    public static PacketIdentifier ResourcePackReceive =>
        new(44, nameof(ResourcePackReceive), PacketState.Play, PacketDirection.Serverbound);

    public static PacketIdentifier SelectBundleItem =>
        new(45, nameof(SelectBundleItem), PacketState.Play, PacketDirection.Serverbound);

    public static PacketIdentifier SelectTrade =>
        new(46, nameof(SelectTrade), PacketState.Play, PacketDirection.Serverbound);

    public static PacketIdentifier SetBeaconEffect =>
        new(47, nameof(SetBeaconEffect), PacketState.Play, PacketDirection.Serverbound);

    public static PacketIdentifier SetCreativeSlot =>
        new(48, nameof(SetCreativeSlot), PacketState.Play, PacketDirection.Serverbound);

    public static PacketIdentifier SetDifficulty =>
        new(49, nameof(SetDifficulty), PacketState.Play, PacketDirection.Serverbound);

    public static PacketIdentifier SetSlotState =>
        new(50, nameof(SetSlotState), PacketState.Play, PacketDirection.Serverbound);

    public static PacketIdentifier Settings => new(51, nameof(Settings), PacketState.Play, PacketDirection.Serverbound);
    public static PacketIdentifier Spectate => new(52, nameof(Spectate), PacketState.Play, PacketDirection.Serverbound);

    public static PacketIdentifier SteerBoat =>
        new(53, nameof(SteerBoat), PacketState.Play, PacketDirection.Serverbound);

    public static PacketIdentifier SteerVehicle =>
        new(54, nameof(SteerVehicle), PacketState.Play, PacketDirection.Serverbound);

    public static PacketIdentifier TabComplete =>
        new(55, nameof(TabComplete), PacketState.Play, PacketDirection.Serverbound);

    public static PacketIdentifier TeleportConfirm =>
        new(56, nameof(TeleportConfirm), PacketState.Play, PacketDirection.Serverbound);

    public static PacketIdentifier TickEnd => new(57, nameof(TickEnd), PacketState.Play, PacketDirection.Serverbound);

    public static PacketIdentifier Transaction =>
        new(58, nameof(Transaction), PacketState.Play, PacketDirection.Serverbound);

    public static PacketIdentifier UpdateCommandBlock =>
        new(59, nameof(UpdateCommandBlock), PacketState.Play, PacketDirection.Serverbound);

    public static PacketIdentifier UpdateCommandBlockMinecart => new(60, nameof(UpdateCommandBlockMinecart),
        PacketState.Play, PacketDirection.Serverbound);

    public static PacketIdentifier UpdateJigsawBlock =>
        new(61, nameof(UpdateJigsawBlock), PacketState.Play, PacketDirection.Serverbound);

    public static PacketIdentifier UpdateSign =>
        new(62, nameof(UpdateSign), PacketState.Play, PacketDirection.Serverbound);

    public static PacketIdentifier UpdateStructureBlock => new(63, nameof(UpdateStructureBlock), PacketState.Play,
        PacketDirection.Serverbound);

    public static PacketIdentifier UseEntity =>
        new(64, nameof(UseEntity), PacketState.Play, PacketDirection.Serverbound);

    public static PacketIdentifier UseItem => new(65, nameof(UseItem), PacketState.Play, PacketDirection.Serverbound);

    public static PacketIdentifier VehicleMove =>
        new(66, nameof(VehicleMove), PacketState.Play, PacketDirection.Serverbound);

    public static PacketIdentifier WindowClick =>
        new(67, nameof(WindowClick), PacketState.Play, PacketDirection.Serverbound);
}