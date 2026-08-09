using System;
using System.Diagnostics.CodeAnalysis;

namespace McProtoNet.Protocol;
public readonly record struct IdRange(int FromPv, int ToPv, int Id);
public sealed record PacketDescriptor(PacketIdentity Identity, IdRange[] Ids);
/// <summary>Generated packet registry: dense id->ordinal tables on the hot path,
/// descriptor catalogs on the cold one. Unknown ids are a normal stream condition:
/// every entry point is Try.</summary>
public static partial class PacketRegistry
{
    private static readonly PacketDescriptor[] CatalogHandshakingServerbound = [new(new("handshaking.toServer.legacy_server_list_ping", "LegacyServerListPing", PacketPhase.Handshaking, PacketDirection.Serverbound, 0), [new(735, 772, 0xFE)]), new(new("handshaking.toServer.set_protocol", "SetProtocol", PacketPhase.Handshaking, PacketDirection.Serverbound, 1), [new(735, 772, 0x00)]), ];
    private static readonly PacketDescriptor[] CatalogStatusClientbound = [new(new("status.toClient.ping", "PongResponse", PacketPhase.Status, PacketDirection.Clientbound, 0), [new(735, 772, 0x01)]), new(new("status.toClient.server_info", "ServerInfo", PacketPhase.Status, PacketDirection.Clientbound, 1), [new(735, 772, 0x00)]), ];
    private static readonly PacketDescriptor[] CatalogStatusServerbound = [new(new("status.toServer.ping", "PingRequest", PacketPhase.Status, PacketDirection.Serverbound, 0), [new(735, 772, 0x01)]), new(new("status.toServer.ping_start", "PingStart", PacketPhase.Status, PacketDirection.Serverbound, 1), [new(735, 772, 0x00)]), ];
    private static readonly PacketDescriptor[] CatalogLoginClientbound = [new(new("login.toClient.compress", "LoginCompress", PacketPhase.Login, PacketDirection.Clientbound, 0), [new(735, 772, 0x03)]), new(new("login.toClient.cookie_request", "LoginCookieRequest", PacketPhase.Login, PacketDirection.Clientbound, 1), [new(766, 772, 0x05)]), new(new("login.toClient.disconnect", "LoginDisconnect", PacketPhase.Login, PacketDirection.Clientbound, 2), [new(735, 772, 0x00)]), new(new("login.toClient.encryption_begin", "EncryptionRequest", PacketPhase.Login, PacketDirection.Clientbound, 3), [new(735, 772, 0x01)]), new(new("login.toClient.login_plugin_request", "LoginPluginRequest", PacketPhase.Login, PacketDirection.Clientbound, 4), [new(735, 772, 0x04)]), new(new("login.toClient.success", "LoginSuccess", PacketPhase.Login, PacketDirection.Clientbound, 5), [new(735, 772, 0x02)]), ];
    private static readonly PacketDescriptor[] CatalogLoginServerbound = [new(new("login.toServer.cookie_response", "LoginCookieResponse", PacketPhase.Login, PacketDirection.Serverbound, 0), [new(766, 772, 0x04)]), new(new("login.toServer.encryption_begin", "EncryptionResponse", PacketPhase.Login, PacketDirection.Serverbound, 1), [new(735, 772, 0x01)]), new(new("login.toServer.login_acknowledged", "LoginAcknowledged", PacketPhase.Login, PacketDirection.Serverbound, 2), [new(764, 772, 0x03)]), new(new("login.toServer.login_plugin_response", "LoginPluginResponse", PacketPhase.Login, PacketDirection.Serverbound, 3), [new(735, 772, 0x02)]), new(new("login.toServer.login_start", "LoginStart", PacketPhase.Login, PacketDirection.Serverbound, 4), [new(735, 772, 0x00)]), ];
    private static readonly PacketDescriptor[] CatalogConfigurationClientbound = [new(new("configuration.toClient.disconnect", "Disconnect", PacketPhase.Configuration, PacketDirection.Clientbound, 0), [new(764, 765, 0x01), new(766, 772, 0x02)]), new(new("configuration.toClient.finish_configuration", "FinishConfiguration", PacketPhase.Configuration, PacketDirection.Clientbound, 1), [new(764, 765, 0x02), new(766, 772, 0x03)]), new(new("configuration.toClient.keep_alive", "KeepAlive", PacketPhase.Configuration, PacketDirection.Clientbound, 2), [new(764, 765, 0x03), new(766, 772, 0x04)]), new(new("configuration.toClient.ping", "Ping", PacketPhase.Configuration, PacketDirection.Clientbound, 3), [new(764, 765, 0x04), new(766, 772, 0x05)]), new(new("configuration.toClient.select_known_packs", "SelectKnownPacks", PacketPhase.Configuration, PacketDirection.Clientbound, 4), [new(766, 772, 0x0E)]), ];
    private static readonly PacketDescriptor[] CatalogConfigurationServerbound = [new(new("configuration.toServer.finish_configuration", "FinishConfiguration", PacketPhase.Configuration, PacketDirection.Serverbound, 0), [new(764, 765, 0x02), new(766, 772, 0x03)]), new(new("configuration.toServer.keep_alive", "KeepAlive", PacketPhase.Configuration, PacketDirection.Serverbound, 1), [new(764, 765, 0x03), new(766, 772, 0x04)]), new(new("configuration.toServer.pong", "Pong", PacketPhase.Configuration, PacketDirection.Serverbound, 2), [new(764, 765, 0x04), new(766, 772, 0x05)]), new(new("configuration.toServer.select_known_packs", "SelectKnownPacks", PacketPhase.Configuration, PacketDirection.Serverbound, 3), [new(766, 772, 0x07)]), new(new("configuration.toServer.settings", "ClientInformation", PacketPhase.Configuration, PacketDirection.Serverbound, 4), [new(764, 772, 0x00)]), ];
    private static readonly PacketDescriptor[] CatalogPlayClientbound = [new(new("play.toClient.damage_event", "DamageEvent", PacketPhase.Play, PacketDirection.Clientbound, 0), [new(762, 763, 0x18), new(764, 765, 0x19), new(766, 769, 0x1A), new(770, 772, 0x19)]), new(new("play.toClient.entity_head_rotation", "EntityHeadRotation", PacketPhase.Play, PacketDirection.Clientbound, 1), [new(735, 736, 0x3B), new(751, 754, 0x3A), new(755, 758, 0x3E), new(759, 759, 0x3C), new(760, 760, 0x3F), new(761, 761, 0x3E), new(762, 763, 0x42), new(764, 764, 0x44), new(765, 765, 0x46), new(766, 767, 0x48), new(768, 769, 0x4D), new(770, 772, 0x4C)]), new(new("play.toClient.entity_metadata", "EntityMetadata", PacketPhase.Play, PacketDirection.Clientbound, 2), [new(735, 736, 0x44), new(751, 754, 0x44), new(755, 759, 0x4D), new(760, 760, 0x50), new(761, 761, 0x4E), new(762, 763, 0x52), new(764, 764, 0x54), new(765, 765, 0x56), new(766, 767, 0x58), new(768, 769, 0x5D), new(770, 772, 0x5C)]), new(new("play.toClient.explosion", "Explosion", PacketPhase.Play, PacketDirection.Clientbound, 3), [new(735, 736, 0x1C), new(751, 754, 0x1B), new(755, 758, 0x1C), new(759, 759, 0x19), new(760, 760, 0x1B), new(761, 761, 0x1A), new(762, 763, 0x1D), new(764, 765, 0x1E), new(766, 767, 0x20), new(768, 769, 0x21), new(770, 772, 0x20)]), new(new("play.toClient.hurt_animation", "HurtAnimation", PacketPhase.Play, PacketDirection.Clientbound, 4), [new(762, 763, 0x21), new(764, 765, 0x22), new(766, 767, 0x24), new(768, 769, 0x25), new(770, 772, 0x24)]), new(new("play.toClient.keep_alive", "KeepAlive", PacketPhase.Play, PacketDirection.Clientbound, 5), [new(735, 736, 0x20), new(751, 754, 0x1F), new(755, 758, 0x21), new(759, 759, 0x1E), new(760, 760, 0x20), new(761, 761, 0x1F), new(762, 763, 0x23), new(764, 765, 0x24), new(766, 767, 0x26), new(768, 769, 0x27), new(770, 772, 0x26)]), new(new("play.toClient.map", "Map", PacketPhase.Play, PacketDirection.Clientbound, 6), [new(735, 736, 0x26), new(751, 754, 0x25), new(755, 758, 0x27), new(759, 759, 0x24), new(760, 760, 0x26), new(761, 761, 0x25), new(762, 763, 0x29), new(764, 765, 0x2A), new(766, 767, 0x2C), new(768, 769, 0x2D), new(770, 772, 0x2C)]), new(new("play.toClient.move_minecart", "MoveMinecart", PacketPhase.Play, PacketDirection.Clientbound, 7), [new(768, 769, 0x31), new(770, 772, 0x30)]), new(new("play.toClient.position", "PlayerPosition", PacketPhase.Play, PacketDirection.Clientbound, 8), [new(735, 736, 0x35), new(751, 754, 0x34), new(755, 758, 0x38), new(759, 759, 0x36), new(760, 760, 0x39), new(761, 761, 0x38), new(762, 763, 0x3C), new(764, 765, 0x3E), new(766, 767, 0x40), new(768, 769, 0x42), new(770, 772, 0x41)]), new(new("play.toClient.respawn", "Respawn", PacketPhase.Play, PacketDirection.Clientbound, 9), [new(735, 736, 0x3A), new(751, 754, 0x39), new(755, 758, 0x3D), new(759, 759, 0x3B), new(760, 760, 0x3E), new(761, 761, 0x3D), new(762, 763, 0x41), new(764, 764, 0x43), new(765, 765, 0x45), new(766, 767, 0x47), new(768, 769, 0x4C), new(770, 772, 0x4B)]), new(new("play.toClient.set_cooldown", "SetCooldown", PacketPhase.Play, PacketDirection.Clientbound, 10), [new(735, 736, 0x17), new(751, 754, 0x16), new(755, 758, 0x17), new(759, 760, 0x14), new(761, 761, 0x13), new(762, 763, 0x15), new(764, 765, 0x16), new(766, 769, 0x17), new(770, 772, 0x16)]), new(new("play.toClient.set_projectile_power", "SetProjectilePower", PacketPhase.Play, PacketDirection.Clientbound, 11), [new(766, 767, 0x79), new(768, 772, 0x80)]), new(new("play.toClient.spawn_entity", "SpawnEntity", PacketPhase.Play, PacketDirection.Clientbound, 12), [new(735, 736, 0x00), new(751, 761, 0x00), new(762, 772, 0x01)]), new(new("play.toClient.spawn_position", "SpawnPosition", PacketPhase.Play, PacketDirection.Clientbound, 13), [new(735, 736, 0x42), new(751, 754, 0x42), new(755, 758, 0x4B), new(759, 759, 0x4A), new(760, 760, 0x4D), new(761, 761, 0x4C), new(762, 763, 0x50), new(764, 764, 0x52), new(765, 765, 0x54), new(766, 767, 0x56), new(768, 769, 0x5B), new(770, 772, 0x5A)]), new(new("play.toClient.teams", "Teams", PacketPhase.Play, PacketDirection.Clientbound, 14), [new(735, 736, 0x4C), new(751, 754, 0x4C), new(755, 759, 0x55), new(760, 760, 0x58), new(761, 761, 0x56), new(762, 763, 0x5A), new(764, 764, 0x5C), new(765, 765, 0x5E), new(766, 767, 0x60), new(768, 769, 0x67), new(770, 772, 0x66)]), new(new("play.toClient.unload_chunk", "UnloadChunk", PacketPhase.Play, PacketDirection.Clientbound, 15), [new(735, 736, 0x1D), new(751, 754, 0x1C), new(755, 758, 0x1D), new(759, 759, 0x1A), new(760, 760, 0x1C), new(761, 761, 0x1B), new(762, 763, 0x1E), new(764, 765, 0x1F), new(766, 767, 0x21), new(768, 769, 0x22), new(770, 772, 0x21)]), new(new("play.toClient.update_health", "UpdateHealth", PacketPhase.Play, PacketDirection.Clientbound, 16), [new(735, 736, 0x49), new(751, 754, 0x49), new(755, 759, 0x52), new(760, 760, 0x55), new(761, 761, 0x53), new(762, 763, 0x57), new(764, 764, 0x59), new(765, 765, 0x5B), new(766, 767, 0x5D), new(768, 769, 0x62), new(770, 772, 0x61)]), new(new("play.toClient.update_time", "UpdateTime", PacketPhase.Play, PacketDirection.Clientbound, 17), [new(735, 736, 0x4E), new(751, 754, 0x4E), new(755, 756, 0x58), new(757, 759, 0x59), new(760, 760, 0x5C), new(761, 761, 0x5A), new(762, 763, 0x5E), new(764, 764, 0x60), new(765, 765, 0x62), new(766, 767, 0x64), new(768, 769, 0x6B), new(770, 772, 0x6A)]), new(new("play.toClient.update_view_distance", "UpdateViewDistance", PacketPhase.Play, PacketDirection.Clientbound, 18), [new(735, 736, 0x41), new(751, 754, 0x41), new(755, 758, 0x4A), new(759, 759, 0x49), new(760, 760, 0x4C), new(761, 761, 0x4B), new(762, 763, 0x4F), new(764, 764, 0x51), new(765, 765, 0x53), new(766, 767, 0x55), new(768, 769, 0x59), new(770, 772, 0x58)]), ];
    private static readonly PacketDescriptor[] CatalogPlayServerbound = [new(new("play.toServer.keep_alive", "KeepAlive", PacketPhase.Play, PacketDirection.Serverbound, 0), [new(735, 736, 0x10), new(751, 754, 0x10), new(755, 758, 0x0F), new(759, 759, 0x11), new(760, 760, 0x12), new(761, 761, 0x11), new(762, 763, 0x12), new(764, 764, 0x14), new(765, 765, 0x15), new(766, 767, 0x18), new(768, 770, 0x1A), new(771, 772, 0x1B)]), new(new("play.toServer.lock_difficulty", "LockDifficulty", PacketPhase.Play, PacketDirection.Serverbound, 1), [new(735, 736, 0x11), new(751, 754, 0x11), new(755, 758, 0x10), new(759, 759, 0x12), new(760, 760, 0x13), new(761, 761, 0x12), new(762, 763, 0x13), new(764, 764, 0x15), new(765, 765, 0x16), new(766, 767, 0x19), new(768, 770, 0x1B), new(771, 772, 0x1C)]), new(new("play.toServer.name_item", "NameItem", PacketPhase.Play, PacketDirection.Serverbound, 2), [new(735, 736, 0x1F), new(751, 758, 0x20), new(759, 759, 0x22), new(760, 763, 0x23), new(764, 764, 0x26), new(765, 765, 0x27), new(766, 767, 0x2A), new(768, 768, 0x2C), new(769, 770, 0x2E), new(771, 772, 0x2F)]), new(new("play.toServer.spectate", "Spectate", PacketPhase.Play, PacketDirection.Serverbound, 3), [new(735, 736, 0x2C), new(751, 758, 0x2D), new(759, 759, 0x2F), new(760, 763, 0x30), new(764, 764, 0x33), new(765, 765, 0x34), new(766, 767, 0x37), new(768, 768, 0x39), new(769, 769, 0x3B), new(770, 772, 0x3D)]), new(new("play.toServer.teleport_confirm", "TeleportConfirm", PacketPhase.Play, PacketDirection.Serverbound, 4), [new(735, 736, 0x00), new(751, 772, 0x00)]), new(new("play.toServer.window_click", "WindowClick", PacketPhase.Play, PacketDirection.Serverbound, 5), [new(735, 736, 0x09), new(751, 754, 0x09), new(755, 758, 0x08), new(759, 759, 0x0A), new(760, 760, 0x0B), new(761, 761, 0x0A), new(762, 763, 0x0B), new(764, 765, 0x0D), new(766, 767, 0x0E), new(768, 770, 0x10), new(771, 772, 0x11)]), ];
    private static ReadOnlySpan<short> TableHandshakingServerbound_735_772 => [1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 0];
    private static ReadOnlySpan<short> TableStatusClientbound_735_772 => [1, 0];
    private static ReadOnlySpan<short> TableStatusServerbound_735_772 => [1, 0];
    private static ReadOnlySpan<short> TableLoginClientbound_735_765 => [2, 3, 5, 0, 4];
    private static ReadOnlySpan<short> TableLoginClientbound_766_772 => [2, 3, 5, 0, 4, 1];
    private static ReadOnlySpan<short> TableLoginServerbound_735_763 => [4, 1, 3];
    private static ReadOnlySpan<short> TableLoginServerbound_764_765 => [4, 1, 3, 2];
    private static ReadOnlySpan<short> TableLoginServerbound_766_772 => [4, 1, 3, 2, 0];
    private static ReadOnlySpan<short> TableConfigurationClientbound_764_765 => [-1, 0, 1, 2, 3];
    private static ReadOnlySpan<short> TableConfigurationClientbound_766_772 => [-1, -1, 0, 1, 2, 3, -1, -1, -1, -1, -1, -1, -1, -1, 4];
    private static ReadOnlySpan<short> TableConfigurationServerbound_764_765 => [4, -1, 0, 1, 2];
    private static ReadOnlySpan<short> TableConfigurationServerbound_766_772 => [4, -1, -1, 0, 1, 2, -1, 3];
    private static ReadOnlySpan<short> TablePlayClientbound_735_736 => [12, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 10, -1, -1, -1, -1, 3, 15, -1, -1, 5, -1, -1, -1, -1, -1, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 8, -1, -1, -1, -1, 9, 1, -1, -1, -1, -1, -1, 18, 13, -1, 2, -1, -1, -1, -1, 16, -1, -1, 14, -1, 17];
    private static ReadOnlySpan<short> TablePlayClientbound_751_754 => [12, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 10, -1, -1, -1, -1, 3, 15, -1, -1, 5, -1, -1, -1, -1, -1, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 8, -1, -1, -1, -1, 9, 1, -1, -1, -1, -1, -1, -1, 18, 13, -1, 2, -1, -1, -1, -1, 16, -1, -1, 14, -1, 17];
    private static ReadOnlySpan<short> TablePlayClientbound_755_756 => [12, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 10, -1, -1, -1, -1, 3, 15, -1, -1, -1, 5, -1, -1, -1, -1, -1, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 8, -1, -1, -1, -1, 9, 1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 18, 13, -1, 2, -1, -1, -1, -1, 16, -1, -1, 14, -1, -1, 17];
    private static ReadOnlySpan<short> TablePlayClientbound_757_758 => [12, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 10, -1, -1, -1, -1, 3, 15, -1, -1, -1, 5, -1, -1, -1, -1, -1, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 8, -1, -1, -1, -1, 9, 1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 18, 13, -1, 2, -1, -1, -1, -1, 16, -1, -1, 14, -1, -1, -1, 17];
    private static ReadOnlySpan<short> TablePlayClientbound_759_759 => [12, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 10, -1, -1, -1, -1, 3, 15, -1, -1, -1, 5, -1, -1, -1, -1, -1, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 8, -1, -1, -1, -1, 9, 1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 18, 13, -1, -1, 2, -1, -1, -1, -1, 16, -1, -1, 14, -1, -1, -1, 17];
    private static ReadOnlySpan<short> TablePlayClientbound_760_760 => [12, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 10, -1, -1, -1, -1, -1, -1, 3, 15, -1, -1, -1, 5, -1, -1, -1, -1, -1, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 8, -1, -1, -1, -1, 9, 1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 18, 13, -1, -1, 2, -1, -1, -1, -1, 16, -1, -1, 14, -1, -1, -1, 17];
    private static ReadOnlySpan<short> TablePlayClientbound_761_761 => [12, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 10, -1, -1, -1, -1, -1, -1, 3, 15, -1, -1, -1, 5, -1, -1, -1, -1, -1, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 8, -1, -1, -1, -1, 9, 1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 18, 13, -1, 2, -1, -1, -1, -1, 16, -1, -1, 14, -1, -1, -1, 17];
    private static ReadOnlySpan<short> TablePlayClientbound_762_763 => [-1, 12, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 10, -1, -1, 0, -1, -1, -1, -1, 3, 15, -1, -1, 4, -1, 5, -1, -1, -1, -1, -1, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 8, -1, -1, -1, -1, 9, 1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 18, 13, -1, 2, -1, -1, -1, -1, 16, -1, -1, 14, -1, -1, -1, 17];
    private static ReadOnlySpan<short> TablePlayClientbound_764_764 => [-1, 12, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 10, -1, -1, 0, -1, -1, -1, -1, 3, 15, -1, -1, 4, -1, 5, -1, -1, -1, -1, -1, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 8, -1, -1, -1, -1, 9, 1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 18, 13, -1, 2, -1, -1, -1, -1, 16, -1, -1, 14, -1, -1, -1, 17];
    private static ReadOnlySpan<short> TablePlayClientbound_765_765 => [-1, 12, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 10, -1, -1, 0, -1, -1, -1, -1, 3, 15, -1, -1, 4, -1, 5, -1, -1, -1, -1, -1, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 8, -1, -1, -1, -1, -1, -1, 9, 1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 18, 13, -1, 2, -1, -1, -1, -1, 16, -1, -1, 14, -1, -1, -1, 17];
    private static ReadOnlySpan<short> TablePlayClientbound_766_767 => [-1, 12, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 10, -1, -1, 0, -1, -1, -1, -1, -1, 3, 15, -1, -1, 4, -1, 5, -1, -1, -1, -1, -1, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 8, -1, -1, -1, -1, -1, -1, 9, 1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 18, 13, -1, 2, -1, -1, -1, -1, 16, -1, -1, 14, -1, -1, -1, 17, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 11];
    private static ReadOnlySpan<short> TablePlayClientbound_768_769 => [-1, 12, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 10, -1, -1, 0, -1, -1, -1, -1, -1, -1, 3, 15, -1, -1, 4, -1, 5, -1, -1, -1, -1, -1, 6, -1, -1, -1, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, 9, 1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 18, -1, 13, -1, 2, -1, -1, -1, -1, 16, -1, -1, -1, -1, 14, -1, -1, -1, 17, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 11];
    private static ReadOnlySpan<short> TablePlayClientbound_770_772 => [-1, 12, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 10, -1, -1, 0, -1, -1, -1, -1, -1, -1, 3, 15, -1, -1, 4, -1, 5, -1, -1, -1, -1, -1, 6, -1, -1, -1, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, 9, 1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 18, -1, 13, -1, 2, -1, -1, -1, -1, 16, -1, -1, -1, -1, 14, -1, -1, -1, 17, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 11];
    private static ReadOnlySpan<short> TablePlayServerbound_735_736 => [4, -1, -1, -1, -1, -1, -1, -1, -1, 5, -1, -1, -1, -1, -1, -1, 0, 1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 3];
    private static ReadOnlySpan<short> TablePlayServerbound_751_754 => [4, -1, -1, -1, -1, -1, -1, -1, -1, 5, -1, -1, -1, -1, -1, -1, 0, 1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 3];
    private static ReadOnlySpan<short> TablePlayServerbound_755_758 => [4, -1, -1, -1, -1, -1, -1, -1, 5, -1, -1, -1, -1, -1, -1, 0, 1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 3];
    private static ReadOnlySpan<short> TablePlayServerbound_759_759 => [4, -1, -1, -1, -1, -1, -1, -1, -1, -1, 5, -1, -1, -1, -1, -1, -1, 0, 1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 3];
    private static ReadOnlySpan<short> TablePlayServerbound_760_760 => [4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 5, -1, -1, -1, -1, -1, -1, 0, 1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 3];
    private static ReadOnlySpan<short> TablePlayServerbound_761_761 => [4, -1, -1, -1, -1, -1, -1, -1, -1, -1, 5, -1, -1, -1, -1, -1, -1, 0, 1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 3];
    private static ReadOnlySpan<short> TablePlayServerbound_762_763 => [4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 5, -1, -1, -1, -1, -1, -1, 0, 1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 3];
    private static ReadOnlySpan<short> TablePlayServerbound_764_764 => [4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 5, -1, -1, -1, -1, -1, -1, 0, 1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 3];
    private static ReadOnlySpan<short> TablePlayServerbound_765_765 => [4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 5, -1, -1, -1, -1, -1, -1, -1, 0, 1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 3];
    private static ReadOnlySpan<short> TablePlayServerbound_766_767 => [4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, 0, 1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 3];
    private static ReadOnlySpan<short> TablePlayServerbound_768_768 => [4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, 0, 1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 3];
    private static ReadOnlySpan<short> TablePlayServerbound_769_769 => [4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, 0, 1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 3];
    private static ReadOnlySpan<short> TablePlayServerbound_770_770 => [4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, 0, 1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 3];
    private static ReadOnlySpan<short> TablePlayServerbound_771_772 => [4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, 0, 1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 3];

    private static ReadOnlySpan<short> Table(PacketPhase phase, PacketDirection dir, int pv)
    {
        switch (phase, dir)
        {
            case (PacketPhase.Handshaking, PacketDirection.Serverbound):
                if (pv >= 735 && pv <= 772)
                    return TableHandshakingServerbound_735_772;
                return default;
            case (PacketPhase.Status, PacketDirection.Clientbound):
                if (pv >= 735 && pv <= 772)
                    return TableStatusClientbound_735_772;
                return default;
            case (PacketPhase.Status, PacketDirection.Serverbound):
                if (pv >= 735 && pv <= 772)
                    return TableStatusServerbound_735_772;
                return default;
            case (PacketPhase.Login, PacketDirection.Clientbound):
                if (pv >= 735 && pv <= 765)
                    return TableLoginClientbound_735_765;
                if (pv >= 766 && pv <= 772)
                    return TableLoginClientbound_766_772;
                return default;
            case (PacketPhase.Login, PacketDirection.Serverbound):
                if (pv >= 735 && pv <= 763)
                    return TableLoginServerbound_735_763;
                if (pv >= 764 && pv <= 765)
                    return TableLoginServerbound_764_765;
                if (pv >= 766 && pv <= 772)
                    return TableLoginServerbound_766_772;
                return default;
            case (PacketPhase.Configuration, PacketDirection.Clientbound):
                if (pv >= 764 && pv <= 765)
                    return TableConfigurationClientbound_764_765;
                if (pv >= 766 && pv <= 772)
                    return TableConfigurationClientbound_766_772;
                return default;
            case (PacketPhase.Configuration, PacketDirection.Serverbound):
                if (pv >= 764 && pv <= 765)
                    return TableConfigurationServerbound_764_765;
                if (pv >= 766 && pv <= 772)
                    return TableConfigurationServerbound_766_772;
                return default;
            case (PacketPhase.Play, PacketDirection.Clientbound):
                if (pv >= 735 && pv <= 736)
                    return TablePlayClientbound_735_736;
                if (pv >= 751 && pv <= 754)
                    return TablePlayClientbound_751_754;
                if (pv >= 755 && pv <= 756)
                    return TablePlayClientbound_755_756;
                if (pv >= 757 && pv <= 758)
                    return TablePlayClientbound_757_758;
                if (pv >= 759 && pv <= 759)
                    return TablePlayClientbound_759_759;
                if (pv >= 760 && pv <= 760)
                    return TablePlayClientbound_760_760;
                if (pv >= 761 && pv <= 761)
                    return TablePlayClientbound_761_761;
                if (pv >= 762 && pv <= 763)
                    return TablePlayClientbound_762_763;
                if (pv >= 764 && pv <= 764)
                    return TablePlayClientbound_764_764;
                if (pv >= 765 && pv <= 765)
                    return TablePlayClientbound_765_765;
                if (pv >= 766 && pv <= 767)
                    return TablePlayClientbound_766_767;
                if (pv >= 768 && pv <= 769)
                    return TablePlayClientbound_768_769;
                if (pv >= 770 && pv <= 772)
                    return TablePlayClientbound_770_772;
                return default;
            case (PacketPhase.Play, PacketDirection.Serverbound):
                if (pv >= 735 && pv <= 736)
                    return TablePlayServerbound_735_736;
                if (pv >= 751 && pv <= 754)
                    return TablePlayServerbound_751_754;
                if (pv >= 755 && pv <= 758)
                    return TablePlayServerbound_755_758;
                if (pv >= 759 && pv <= 759)
                    return TablePlayServerbound_759_759;
                if (pv >= 760 && pv <= 760)
                    return TablePlayServerbound_760_760;
                if (pv >= 761 && pv <= 761)
                    return TablePlayServerbound_761_761;
                if (pv >= 762 && pv <= 763)
                    return TablePlayServerbound_762_763;
                if (pv >= 764 && pv <= 764)
                    return TablePlayServerbound_764_764;
                if (pv >= 765 && pv <= 765)
                    return TablePlayServerbound_765_765;
                if (pv >= 766 && pv <= 767)
                    return TablePlayServerbound_766_767;
                if (pv >= 768 && pv <= 768)
                    return TablePlayServerbound_768_768;
                if (pv >= 769 && pv <= 769)
                    return TablePlayServerbound_769_769;
                if (pv >= 770 && pv <= 770)
                    return TablePlayServerbound_770_770;
                if (pv >= 771 && pv <= 772)
                    return TablePlayServerbound_771_772;
                return default;
        }

        return default;
    }

    public static bool TryGetOrdinal(int id, int protocolVersion, PacketPhase phase, PacketDirection dir, out ushort ordinal)
    {
        var table = Table(phase, dir, protocolVersion);
        if ((uint)id < (uint)table.Length && table[id] >= 0)
        {
            ordinal = (ushort)table[id];
            return true;
        }

        ordinal = 0;
        return false;
    }

    public static bool TryResolve(int id, int protocolVersion, PacketPhase phase, PacketDirection dir, [NotNullWhen(true)] out PacketDescriptor? descriptor)
    {
        if (TryGetOrdinal(id, protocolVersion, phase, dir, out var ordinal))
        {
            descriptor = Catalog(phase, dir)[ordinal];
            return true;
        }

        descriptor = null;
        return false;
    }

    public static ReadOnlySpan<PacketDescriptor> Catalog(PacketPhase phase, PacketDirection dir)
    {
        switch (phase, dir)
        {
            case (PacketPhase.Handshaking, PacketDirection.Serverbound):
                return CatalogHandshakingServerbound;
            case (PacketPhase.Status, PacketDirection.Clientbound):
                return CatalogStatusClientbound;
            case (PacketPhase.Status, PacketDirection.Serverbound):
                return CatalogStatusServerbound;
            case (PacketPhase.Login, PacketDirection.Clientbound):
                return CatalogLoginClientbound;
            case (PacketPhase.Login, PacketDirection.Serverbound):
                return CatalogLoginServerbound;
            case (PacketPhase.Configuration, PacketDirection.Clientbound):
                return CatalogConfigurationClientbound;
            case (PacketPhase.Configuration, PacketDirection.Serverbound):
                return CatalogConfigurationServerbound;
            case (PacketPhase.Play, PacketDirection.Clientbound):
                return CatalogPlayClientbound;
            case (PacketPhase.Play, PacketDirection.Serverbound):
                return CatalogPlayServerbound;
        }

        return default;
    }
}
