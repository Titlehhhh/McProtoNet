using System.Threading.Tasks;
using McProtoNet.Net;

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

    public ValueTask HandleAsync(in InputPacket raw, int protocolVersion)
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
                        _pending = OnDisconnect((Packets.Configuration.Clientbound.DisconnectPacket)(object)packet);
                        return;
                    case 1:
                        _pending = OnFinishConfiguration((Packets.Configuration.Clientbound.FinishConfigurationPacket)(object)packet);
                        return;
                    case 2:
                        _pending = OnConfigurationKeepAlive((Packets.Configuration.Clientbound.KeepAlivePacket)(object)packet);
                        return;
                    case 3:
                        _pending = OnPing((Packets.Configuration.Clientbound.PingPacket)(object)packet);
                        return;
                    case 4:
                        _pending = OnSelectKnownPacks((Packets.Configuration.Clientbound.SelectKnownPacksPacket)(object)packet);
                        return;
                }

                return;
            case PacketPhase.Play:
                switch (identity.Ordinal)
                {
                    case 0:
                        _pending = OnDamageEvent((Packets.Play.Clientbound.DamageEventPacket)(object)packet);
                        return;
                    case 1:
                        _pending = OnEntityHeadRotation((Packets.Play.Clientbound.EntityHeadRotationPacket)(object)packet);
                        return;
                    case 4:
                        _pending = OnHurtAnimation((Packets.Play.Clientbound.HurtAnimationPacket)(object)packet);
                        return;
                    case 5:
                        _pending = OnKeepAlive((Packets.Play.Clientbound.KeepAlivePacket)(object)packet);
                        return;
                    case 7:
                        _pending = OnMoveMinecart((Packets.Play.Clientbound.MoveMinecartPacket)(object)packet);
                        return;
                    case 8:
                        _pending = OnPlayerPosition((Packets.Play.Clientbound.PlayerPositionPacket)(object)packet);
                        return;
                    case 9:
                        _pending = OnRespawn((Packets.Play.Clientbound.RespawnPacket)(object)packet);
                        return;
                    case 10:
                        _pending = OnSetCooldown((Packets.Play.Clientbound.SetCooldownPacket)(object)packet);
                        return;
                    case 11:
                        _pending = OnSetProjectilePower((Packets.Play.Clientbound.SetProjectilePowerPacket)(object)packet);
                        return;
                    case 12:
                        _pending = OnSpawnEntity((Packets.Play.Clientbound.SpawnEntityPacket)(object)packet);
                        return;
                    case 13:
                        _pending = OnSpawnPosition((Packets.Play.Clientbound.SpawnPositionPacket)(object)packet);
                        return;
                    case 15:
                        _pending = OnUnloadChunk((Packets.Play.Clientbound.UnloadChunkPacket)(object)packet);
                        return;
                    case 16:
                        _pending = OnUpdateHealth((Packets.Play.Clientbound.UpdateHealthPacket)(object)packet);
                        return;
                    case 17:
                        _pending = OnUpdateTime((Packets.Play.Clientbound.UpdateTimePacket)(object)packet);
                        return;
                    case 18:
                        _pending = OnUpdateViewDistance((Packets.Play.Clientbound.UpdateViewDistancePacket)(object)packet);
                        return;
                }

                return;
        }
    }

    void IPacketVisitor.Unknown(in InputPacket raw) => _pending = OnUnknown(in raw);
    protected virtual ValueTask OnUnknown(in InputPacket raw) => default;
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
    protected virtual ValueTask OnDisconnect(Packets.Configuration.Clientbound.DisconnectPacket packet) => default;
    protected virtual ValueTask OnFinishConfiguration(Packets.Configuration.Clientbound.FinishConfigurationPacket packet) => default;
    protected virtual ValueTask OnConfigurationKeepAlive(Packets.Configuration.Clientbound.KeepAlivePacket packet) => default;
    protected virtual ValueTask OnPing(Packets.Configuration.Clientbound.PingPacket packet) => default;
    protected virtual ValueTask OnSelectKnownPacks(Packets.Configuration.Clientbound.SelectKnownPacksPacket packet) => default;
    // --- Play ---
    protected virtual ValueTask OnDamageEvent(Packets.Play.Clientbound.DamageEventPacket packet) => default;
    protected virtual ValueTask OnEntityHeadRotation(Packets.Play.Clientbound.EntityHeadRotationPacket packet) => default;
    protected virtual ValueTask OnHurtAnimation(Packets.Play.Clientbound.HurtAnimationPacket packet) => default;
    protected virtual ValueTask OnKeepAlive(Packets.Play.Clientbound.KeepAlivePacket packet) => default;
    protected virtual ValueTask OnMoveMinecart(Packets.Play.Clientbound.MoveMinecartPacket packet) => default;
    protected virtual ValueTask OnPlayerPosition(Packets.Play.Clientbound.PlayerPositionPacket packet) => default;
    protected virtual ValueTask OnRespawn(Packets.Play.Clientbound.RespawnPacket packet) => default;
    protected virtual ValueTask OnSetCooldown(Packets.Play.Clientbound.SetCooldownPacket packet) => default;
    protected virtual ValueTask OnSetProjectilePower(Packets.Play.Clientbound.SetProjectilePowerPacket packet) => default;
    protected virtual ValueTask OnSpawnEntity(Packets.Play.Clientbound.SpawnEntityPacket packet) => default;
    protected virtual ValueTask OnSpawnPosition(Packets.Play.Clientbound.SpawnPositionPacket packet) => default;
    protected virtual ValueTask OnUnloadChunk(Packets.Play.Clientbound.UnloadChunkPacket packet) => default;
    protected virtual ValueTask OnUpdateHealth(Packets.Play.Clientbound.UpdateHealthPacket packet) => default;
    protected virtual ValueTask OnUpdateTime(Packets.Play.Clientbound.UpdateTimePacket packet) => default;
    protected virtual ValueTask OnUpdateViewDistance(Packets.Play.Clientbound.UpdateViewDistancePacket packet) => default;
}
