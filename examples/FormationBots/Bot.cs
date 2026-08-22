// Бот примера: вся фазовая логика (рукопожатие → логин → шифр → configuration →
// play) живёт ЗДЕСЬ, поверх стандартного клиента. Очередь отправки внутри клиента,
// поэтому цикл пакетов и ходьба пишут из двух потоков без своего замка.

using System.Security.Cryptography;
using McProtoNet;
using McProtoNet.NBT;
using McProtoNet.Primitives;
using McProtoNet.Protocol;
using ConfCb = McProtoNet.Protocol.Packets.Configuration.Clientbound;
using ConfSb = McProtoNet.Protocol.Packets.Configuration.Serverbound;
using HandshakeSb = McProtoNet.Protocol.Packets.Handshaking.Serverbound;
using LoginCb = McProtoNet.Protocol.Packets.Login.Clientbound;
using LoginSb = McProtoNet.Protocol.Packets.Login.Serverbound;
using PlayCb = McProtoNet.Protocol.Packets.Play.Clientbound;
using PlaySb = McProtoNet.Protocol.Packets.Play.Serverbound;

namespace FormationBots;

// record-КЛАСС: ссылка меняется атомарно, читатели из других потоков не видят рваных координат
public sealed record BotLocation(double X, double Y, double Z, float Yaw, float Pitch);

public sealed class Bot(string name, string host, int port, int pv) : ClientboundHandler
{
    private readonly MinecraftClient _client = new(new MinecraftClientOptions { Host = host, Port = port });
    private readonly TaskCompletionSource _spawned = new(TaskCreationOptions.RunContinuationsAsynchronously);
    private volatile bool _stopped;
    private CancellationToken _session;

    public string Name { get; } = name;
    public BotLocation Location { get; private set; } = new(0, 0, 0, 0, 0);
    public string? DisconnectReason { get; private set; }
    public bool Alive => !_stopped;
    public bool Spawned => _spawned.Task.IsCompletedSuccessfully;

    /// <summary>Услышанный чат плоским текстом: системный И игровой (player_chat).</summary>
    public event Action<Bot, string>? ChatHeard;

    /// <summary>Сессия целиком: подключение, логин и цикл пакетов до разрыва.</summary>
    public async Task RunAsync(CancellationToken token)
    {
        _session = token;
        try
        {
            await _client.ConnectAsync(token);
            await SendAsync(new HandshakeSb.SetProtocolPacket(pv, host, port, 2), token);
            await SendAsync(new LoginSb.LoginStartPacket(Name, V764_Last: new(Guid.NewGuid())), token);

            try
            {
                await foreach (var packet in _client.ReadPacketsAsync(token))
                {
                    await HandleAsync(in packet, pv);
                    if (_stopped) break;
                }
            }
            catch (EndOfStreamException)
            {
                DisconnectReason ??= "сервер закрыл соединение";
            }
        }
        finally
        {
            _stopped = true;
            _spawned.TrySetException(
                new InvalidOperationException($"{Name}: {DisconnectReason ?? "соединение закрыто до спавна"}"));
            await _client.DisposeAsync();
        }
    }

    public Task WaitForSpawnAsync(CancellationToken token) => _spawned.Task.WaitAsync(token);

    /// <summary>Отправка: очередь writer'а держит клиент, звать можно из любого потока.</summary>
    public ValueTask SendAsync<T>(T packet, CancellationToken token = default) where T : class, IPacket<T>
        => _client.SendAsync(packet, pv, token.CanBeCanceled ? token : _session);

    // --- логин ---

    protected override ValueTask OnLoginCompress(LoginCb.LoginCompressPacket packet)
    {
        _client.CompressionThreshold = packet.Threshold;
        return default;
    }

    protected override async ValueTask OnLoginSuccess(LoginCb.LoginSuccessPacket packet)
    {
        await SendAsync(new LoginSb.LoginAcknowledgedPacket());
        Phase = PacketPhase.Configuration;

        // обзор 2 чанка: боту мир не нужен, а сервер шлёт каждому в разы меньше чанков
        await SendAsync(new ConfSb.ClientInformationPacket(
            "en_us", 2, 0, true, 0x7F, 1, false, true,
            V768_Last: pv >= 768 ? new(ParticleStatus.All) : null));
    }

    protected override ValueTask OnLoginDisconnect(LoginCb.LoginDisconnectPacket packet)
        => Kicked(packet.Reason);

    // Шифр без Mojang-авторизации: offline-сервер может требовать шифрование канала
    // (ShouldAuthenticate=false). Секрет — 16 случайных байт, RSA по открытому ключу
    // сервера, после отправки ответа обе стороны переходят на AES/CFB8.
    protected override async ValueTask OnEncryptionRequest(LoginCb.EncryptionRequestPacket packet)
    {
        if (packet.V766_Last is { ShouldAuthenticate: true })
        {
            await Kicked("сервер требует Mojang-авторизацию, пример работает только с offline");
            return;
        }

        var secret = RandomNumberGenerator.GetBytes(16);
        using var rsa = RSA.Create();
        rsa.ImportSubjectPublicKeyInfo(packet.PublicKey, out _);
        await SendAsync(new LoginSb.EncryptionResponsePacket(
            rsa.Encrypt(secret, RSAEncryptionPadding.Pkcs1),
            rsa.Encrypt(packet.VerifyToken, RSAEncryptionPadding.Pkcs1)));
        _client.EnableEncryption(secret);
    }

    protected override ValueTask OnLoginPluginRequest(LoginCb.LoginPluginRequestPacket packet)
        => SendAsync(new LoginSb.LoginPluginResponsePacket(packet.MessageId, null)); // канал не понимаем — честно говорим

    // --- configuration ---

    protected override ValueTask OnConfigurationKeepAlive(ConfCb.KeepAlivePacket packet)
        => SendAsync(new ConfSb.KeepAlivePacket(packet.KeepAliveId));

    protected override ValueTask OnConfigurationPing(ConfCb.PingPacket packet)
        => SendAsync(new ConfSb.PongPacket(packet.Id));

    protected override ValueTask OnSelectKnownPacks(ConfCb.SelectKnownPacksPacket packet)
        => SendAsync(new ConfSb.SelectKnownPacksPacket(packet.Packs));

    protected override async ValueTask OnFinishConfiguration(ConfCb.FinishConfigurationPacket packet)
    {
        await SendAsync(new ConfSb.FinishConfigurationPacket());
        Phase = PacketPhase.Play;
    }

    protected override ValueTask OnDisconnect(ConfCb.DisconnectPacket packet)
        => Kicked(packet.V764?.ReasonJson ?? packet.V765_Last?.Reason.ToString());

    // --- play ---

    protected override ValueTask OnKeepAlive(PlayCb.KeepAlivePacket packet)
        => SendAsync(new PlaySb.KeepAlivePacket(packet.KeepAliveId));

    protected override ValueTask OnPing(PlayCb.PingPacket packet)
        => SendAsync(new PlaySb.PongPacket(packet.Id));

    protected override async ValueTask OnStartConfiguration(PlayCb.StartConfigurationPacket packet)
    {
        await SendAsync(new PlaySb.ConfigurationAcknowledgedPacket());
        Phase = PacketPhase.Configuration;
    }

    protected override async ValueTask OnPlayerPosition(PlayCb.PlayerPositionPacket packet)
    {
        var c = Location;
        var f = packet.Flags;
        Location = new BotLocation(
            f.X ? c.X + packet.X : packet.X,
            f.Y ? c.Y + packet.Y : packet.Y,
            f.Z ? c.Z + packet.Z : packet.Z,
            f.Yaw ? c.Yaw + packet.Yaw : packet.Yaw,
            f.Pitch ? c.Pitch + packet.Pitch : packet.Pitch);

        await SendAsync(new PlaySb.TeleportConfirmPacket(packet.TeleportId));
        await SendPositionAsync(Location);
        _spawned.TrySetResult();
    }

    protected override ValueTask OnKickDisconnect(PlayCb.KickDisconnectPacket packet)
        => Kicked(packet.VUntil764?.ReasonJson ?? packet.V765_Last?.Reason.ToString());

    protected override ValueTask OnSystemChat(PlayCb.SystemChatPacket packet)
    {
        var text = packet.V765_Last is { } nbt
            ? CollectStrings(nbt.Content)
            : packet.V760_764?.ContentJson ?? packet.V759?.ContentJson ?? "";
        ChatHeard?.Invoke(this, text);
        return default;
    }

    protected override ValueTask OnPlayerChat(PlayCb.PlayerChatPacket packet)
    {
        ChatHeard?.Invoke(this, packet.V770_Last?.PlainMessage
                                ?? packet.V767_769?.PlainMessage
                                ?? packet.V765_766?.PlainMessage
                                ?? packet.V761_764?.PlainMessage
                                ?? packet.V760?.PlainMessage
                                ?? packet.V759?.SignedChatContent
                                ?? "");
        return default;
    }


    // --- движение ---

    /// <summary>
    /// Идёт к точке; Y не трогаем — её ведёт сервер. Шаг 0.5 раз в 100 мс:
    /// та же скорость, что тиковый шаг 0.25, но пакетов вдвое меньше.
    /// </summary>
    public async Task WalkToAsync(double x, double z, CancellationToken token, double step = 0.5)
    {
        while (!_stopped && Phase == PacketPhase.Play)
        {
            var c = Location;
            var dx = x - c.X;
            var dz = z - c.Z;
            var dist = Math.Sqrt(dx * dx + dz * dz);
            if (dist < 0.2)
            {
                await SendPositionAsync(Location with { X = x, Z = z, Yaw = 0f, Pitch = 0f }, token);
                return;
            }

            var move = Math.Min(step, dist);
            var yaw = (float)(Math.Atan2(-dx, dz) * 180.0 / Math.PI); // yaw по направлению шага
            await SendPositionAsync(
                new BotLocation(c.X + dx / dist * move, c.Y, c.Z + dz / dist * move, yaw, 0f), token);
            await Task.Delay(100, token);
        }
    }

    private ValueTask SendPositionAsync(BotLocation location, CancellationToken token = default)
    {
        Location = location;
        return SendAsync(pv <= 767
            ? new PlaySb.PositionLookPacket(location.X, location.Y, location.Z, location.Yaw, location.Pitch,
                VUntil767: new(true))
            : new PlaySb.PositionLookPacket(location.X, location.Y, location.Z, location.Yaw, location.Pitch,
                V768_Last: new(new MovementFlags(true, false))), token);
    }

    private ValueTask Kicked(string? reason)
    {
        DisconnectReason = reason ?? "кик без причины";
        _stopped = true;
        return default;
    }

    /// <summary>Все строковые листья NBT-компонента одним предложением.</summary>
    private static string CollectStrings(NbtTag tag)
    {
        switch (tag)
        {
            case NbtString s:
                return s.Value;
            case NbtList list:
                return string.Join(' ', list.OfType<NbtTag>().Select(CollectStrings).Where(t => t.Length > 0));
            case NbtCompound compound:
                return string.Join(' ', compound.Tags.Select(CollectStrings).Where(t => t.Length > 0));
            default:
                return "";
        }
    }
}
