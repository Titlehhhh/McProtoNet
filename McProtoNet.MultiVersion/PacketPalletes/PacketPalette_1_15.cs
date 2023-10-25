﻿namespace McProtoNet.MultiVersion
{
	public class PacketPalette_1_15 : IPacketPallete
	{
		private readonly Dictionary<int, PacketIn> typeIn = new()
		{
		  { 0x00, PacketIn.SpawnEntity },
			{ 0x01, PacketIn.SpawnExperienceOrb },
			{ 0x02, PacketIn.SpawnWeatherEntity },
			{ 0x03, PacketIn.SpawnLivingEntity },
			{ 0x04, PacketIn.SpawnPainting },
			{ 0x05, PacketIn.SpawnPlayer },
			{ 0x06, PacketIn.EntityAnimation },
			{ 0x07, PacketIn.Statistics },
			{ 0x08, PacketIn.AcknowledgePlayerAction },
			{ 0x09, PacketIn.BlockBreakAnimation },
			{ 0x0A, PacketIn.BlockEntityData },
			{ 0x0B, PacketIn.BlockAction },
			{ 0x0C, PacketIn.BlockChange },
			{ 0x0D, PacketIn.BossBar },
			{ 0x0E, PacketIn.ServerDifficulty },
			{ 0x0F, PacketIn.ChatMessage },
			{ 0x10, PacketIn.MultiBlockChange },
			{ 0x11, PacketIn.TabComplete },
			{ 0x12, PacketIn.DeclareCommands },
			{ 0x13, PacketIn.WindowConfirmation },
			{ 0x14, PacketIn.CloseWindow },
			{ 0x15, PacketIn.WindowItems },
			{ 0x16, PacketIn.WindowProperty },
			{ 0x17, PacketIn.SetSlot },
			{ 0x18, PacketIn.SetCooldown },
			{ 0x19, PacketIn.PluginMessage },
			{ 0x1A, PacketIn.NamedSoundEffect },
			{ 0x1B, PacketIn.Disconnect },
			{ 0x1C, PacketIn.EntityStatus },
			{ 0x1D, PacketIn.Explosion },
			{ 0x1E, PacketIn.UnloadChunk },
			{ 0x1F, PacketIn.ChangeGameState },
			{ 0x20, PacketIn.OpenHorseWindow },
			{ 0x21, PacketIn.KeepAlive },
			{ 0x22, PacketIn.ChunkData },
			{ 0x23, PacketIn.Effect },
			{ 0x24, PacketIn.Particle },
			{ 0x25, PacketIn.UpdateLight },
			{ 0x26, PacketIn.JoinGame },
			{ 0x27, PacketIn.MapData },
			{ 0x28, PacketIn.TradeList },
			{ 0x29, PacketIn.EntityPosition },
			{ 0x2A, PacketIn.EntityPositionRotation },
			{ 0x2B, PacketIn.EntityRotation },
			{ 0x2C, PacketIn.EntityMovement },
			{ 0x2D, PacketIn.VehicleMove },
			{ 0x2E, PacketIn.OpenBook },
			{ 0x2F, PacketIn.OpenWindow },
			{ 0x30, PacketIn.OpenSignEditor },
			{ 0x31, PacketIn.CraftRecipeResponse },
			{ 0x32, PacketIn.PlayerAbilities },
			{ 0x33, PacketIn.CombatEvent },
			{ 0x34, PacketIn.PlayerInfo },
			{ 0x35, PacketIn.FacePlayer },
			{ 0x36, PacketIn.PlayerPositionRotation },
			{ 0x37, PacketIn.UnlockRecipes },
			{ 0x38, PacketIn.DestroyEntities },
			{ 0x39, PacketIn.RemoveEntityEffect },
			{ 0x3A, PacketIn.ResourcePackSend },
			{ 0x3B, PacketIn.Respawn },
			{ 0x3C, PacketIn.EntityHeadLook },
			{ 0x3D, PacketIn.SelectAdvancementTab },
			{ 0x3E, PacketIn.WorldBorder },
			{ 0x3F, PacketIn.Camera },
			{ 0x40, PacketIn.HeldItemChange },
			{ 0x41, PacketIn.UpdateViewPosition },
			{ 0x42, PacketIn.UpdateViewDistance },
			{ 0x43, PacketIn.DisplayScoreboard },
			{ 0x44, PacketIn.EntityMetadata },
			{ 0x45, PacketIn.AttachEntity },
			{ 0x46, PacketIn.EntityVelocity },
			{ 0x47, PacketIn.EntityEquipment },
			{ 0x48, PacketIn.SetExperience },
			{ 0x49, PacketIn.UpdateHealth },
			{ 0x4A, PacketIn.ScoreboardObjective },
			{ 0x4B, PacketIn.SetPassengers },
			{ 0x4C, PacketIn.Teams },
			{ 0x4D, PacketIn.UpdateScore },
			{ 0x4E, PacketIn.SpawnPosition },
			{ 0x4F, PacketIn.TimeUpdate },
			{ 0x50, PacketIn.Title },
			{ 0x51, PacketIn.EntitySoundEffect },
			{ 0x52, PacketIn.SoundEffect },
			{ 0x53, PacketIn.StopSound },
			{ 0x54, PacketIn.PlayerListHeaderAndFooter },
			{ 0x55, PacketIn.NBTQueryResponse },
			{ 0x56, PacketIn.CollectItem },
			{ 0x57, PacketIn.EntityTeleport },
			{ 0x58, PacketIn.Advancements },
			{ 0x59, PacketIn.EntityProperties },
			{ 0x5A, PacketIn.EntityEffect },
			{ 0x5B, PacketIn.DeclareRecipes },
			{ 0x5C, PacketIn.Tags },
		};

		private readonly Dictionary<PacketOut, int> typeOut = new()
		{
		   { PacketOut.TeleportConfirm, 0x00 },
			{ PacketOut.QueryBlockNBT, 0x01 },
			{ PacketOut.SetDifficulty, 0x02 },
			{ PacketOut.ChatMessage, 0x03 },
			{ PacketOut.ClientStatus, 0x04 },
			{ PacketOut.ClientSettings, 0x05 },
			{ PacketOut.TabComplete, 0x06 },
			{ PacketOut.WindowConfirmation, 0x07 },
			{ PacketOut.ClickWindowButton, 0x08 },
			{ PacketOut.ClickWindow, 0x09 },
			{ PacketOut.CloseWindow, 0x0A },
			{ PacketOut.PluginMessage, 0x0B },
			{ PacketOut.EditBook, 0x0C },
			{ PacketOut.EntityNBTRequest, 0x0D },
			{ PacketOut.InteractEntity, 0x0E },
			{ PacketOut.KeepAlive, 0x0F },
			{ PacketOut.LockDifficulty, 0x10 },
			{ PacketOut.PlayerPosition, 0x11 },
			{ PacketOut.PlayerPositionRotation, 0x12 },
			{ PacketOut.PlayerRotation, 0x13 },
			{ PacketOut.PlayerMovement, 0x14 },
			{ PacketOut.VehicleMove, 0x15 },
			{ PacketOut.SteerBoat, 0x16 },
			{ PacketOut.PickItem, 0x17 },
			{ PacketOut.CraftRecipeRequest, 0x18 },
			{ PacketOut.PlayerAbilities, 0x19 },
			{ PacketOut.PlayerAction, 0x1A },
			{ PacketOut.EntityAction, 0x1B },
			{ PacketOut.SteerVehicle, 0x1C },
			{ PacketOut.RecipeBookData, 0x1D },
			{ PacketOut.NameItem, 0x1E },
			{ PacketOut.ResourcePackStatus, 0x1F },
			{ PacketOut.AdvancementTab, 0x20 },
			{ PacketOut.SelectTrade, 0x21 },
			{ PacketOut.SetBeaconEffect, 0x22 },
			{ PacketOut.HeldItemChange, 0x23 },
			{ PacketOut.UpdateCommandBlock, 0x24 },
			{ PacketOut.UpdateCommandBlockMinecart, 0x25 },
			{ PacketOut.CreativeInventoryAction, 0x26 },
			{ PacketOut.UpdateJigsawBlock, 0x27 },
			{ PacketOut.UpdateStructureBlock, 0x28 },
			{ PacketOut.UpdateSign, 0x29 },
			{ PacketOut.Animation, 0x2A },
			{ PacketOut.Spectate, 0x2B },
			{ PacketOut.PlayerBlockPlacement, 0x2C },
			{ PacketOut.UseItem, 0x2D },
		};

		public int GetOut(PacketOut packet)
		{
			return typeOut[packet];
		}

		public bool TryGetIn(int id, out PacketIn packetIn)
		{
			if (typeIn.TryGetValue(id, out packetIn))
			{
				return true;
			}
			return false;
		}
	}
}
