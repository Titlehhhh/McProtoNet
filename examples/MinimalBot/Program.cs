// MinimalBot — исполняемая документация пакетного слоя на транспорте 2.0:
// login идёт по одному пакету на MinecraftConnection, после login_acknowledged соединение
// переезжает в StreamingConnection и дальше читается пачками. Handler-база ClientboundHandler
// из генерата, типизированная отправка SendAsync (id из типа), имена незнакомых пакетов
// из PacketRegistry. Ручного свитча по id нет.
//
// Использование: dotnet run [host] [port] [name]
// По умолчанию: 127.0.0.1 25566 McProtoBot (paper-1.21.8, pv 772)

using System.Net.Sockets;
using McProtoNet;
using McProtoNet.Primitives;
using McProtoNet.Transport;
using McProtoNet.Protocol;
using MinimalBot.Packets; // локальные пакеты, которых ещё нет в спеках: чат и жест
using ConfCb = McProtoNet.Protocol.Packets.Configuration.Clientbound;
using ConfSb = McProtoNet.Protocol.Packets.Configuration.Serverbound;
using HandshakeSb = McProtoNet.Protocol.Packets.Handshaking.Serverbound;
using LoginCb = McProtoNet.Protocol.Packets.Login.Clientbound;
using LoginSb = McProtoNet.Protocol.Packets.Login.Serverbound;
using PlayCb = McProtoNet.Protocol.Packets.Play.Clientbound;
using PlaySb = McProtoNet.Protocol.Packets.Play.Serverbound;

const int Pv = 772; // 1.21.7/8

var host = args.Length > 0 ? args[0] : "127.0.0.1";
var port = args.Length > 1 ? int.Parse(args[1]) : 25566;
var name = args.Length > 2 ? args[2] : "McProtoBot";

Console.WriteLine($"MinimalBot: {name} -> {host}:{port} (pv {Pv}, транспорт 2.0)");

using var tcp = new TcpClient();
await tcp.ConnectAsync(host, port);
var login = new MinecraftConnection(tcp.GetStream());

// handshake: intent=2 (login), дальше сразу login start; id склеивает SendAsync
await login.SendAsync(new HandshakeSb.SetProtocolPacket(Pv, host, port, 2), Pv);
await login.SendAsync(new LoginSb.LoginStartPacket(name, V764_Last: new(Guid.NewGuid())), Pv);
Console.WriteLine("[login] handshake + login start отправлены");

var bot = new Bot(login, Pv);

// режим «по одному»: ровно один кадр за чтение, здесь включаются сжатие и шифр
while (!bot.Stopped && !bot.LoginDone)
{
    var raw = await login.ReadPacketAsync();
    await bot.HandleAsync(in raw, Pv);
}

if (bot.Stopped)
{
    await login.DisposeAsync();
    Console.WriteLine("Соединение закрыто.");
    return;
}

// переход односторонний: login-соединение умирает, дальше живёт потоковое
await using var game = await bot.EnterStreamingAsync();

await foreach (var packet in game.ReadPacketsAsync())
{
    await bot.HandleAsync(in packet, Pv);
    if (bot.Stopped) break;
}

Console.WriteLine("Соединение закрыто.");

// Вся логика — переопределения handler-базы; фазу ведёт бот через слот Phase.
sealed class Bot(MinecraftConnection login, int pv) : ClientboundHandler
{
    private readonly DateTime _startedAt = DateTime.UtcNow;
    private readonly HashSet<(PacketPhase, int)> _unknownSeen = [];
    private StreamingConnection? _game;
    private int _playKeepAlives;
    private bool _greeted;

    public bool Stopped { get; private set; }

    /// <summary>Login отработан: пора переезжать в потоковый режим.</summary>
    public bool LoginDone { get; private set; }

    public async ValueTask<StreamingConnection> EnterStreamingAsync()
    {
        _game = login.ToStreaming();
        await SendAsync(
            new ConfSb.ClientInformationPacket("en_us", 8, 0, true, 0x7F, 1, false, true, V768_Last: new(0)));
        Console.WriteLine("[config] client information отправлена");
        return _game;
    }

    // --- login → configuration ---

    protected override ValueTask OnLoginCompress(LoginCb.LoginCompressPacket p)
    {
        login.CompressionThreshold = p.Threshold;
        Console.WriteLine($"[login] компрессия включена, порог {p.Threshold}");
        return default;
    }

    protected override async ValueTask OnLoginSuccess(LoginCb.LoginSuccessPacket p)
    {
        Console.WriteLine($"[login] успех: {p.Username} {p.Uuid}");
        await login.SendAsync(new LoginSb.LoginAcknowledgedPacket(), pv);
        Phase = PacketPhase.Configuration;
        LoginDone = true;
        Console.WriteLine("[config] вошли в configuration");
    }

    protected override ValueTask OnLoginDisconnect(LoginCb.LoginDisconnectPacket p)
    {
        Console.WriteLine($"[login] кик: {p.Reason}");
        Stopped = true;
        return default;
    }

    // --- configuration ---

    protected override ValueTask OnConfigurationKeepAlive(ConfCb.KeepAlivePacket p)
        => SendAsync(new ConfSb.KeepAlivePacket(p.KeepAliveId));

    // Play gained its own Ping in the 2026-08-11 spec cycle, so the bare OnPing went to it
    // and the configuration one took the phase prefix — the same shape OnConfigurationKeepAlive
    // above already had.
    protected override ValueTask OnConfigurationPing(ConfCb.PingPacket p)
        => SendAsync(new ConfSb.PongPacket(p.Id));

    protected override async ValueTask OnSelectKnownPacks(ConfCb.SelectKnownPacksPacket p)
    {
        await SendAsync(new ConfSb.SelectKnownPacksPacket(p.Packs));
        Console.WriteLine($"[config] known packs: {p.Packs.Length} шт, подтвердили");
    }

    protected override async ValueTask OnFinishConfiguration(ConfCb.FinishConfigurationPacket p)
    {
        await SendAsync(new ConfSb.FinishConfigurationPacket());
        Phase = PacketPhase.Play;
        Console.WriteLine("[play] configuration завершена, вошли в play");
    }

    protected override ValueTask OnDisconnect(ConfCb.DisconnectPacket p)
    {
        Console.WriteLine($"[config] кик: {p.V764?.ReasonJson ?? p.V765_Last?.Reason.ToString()}");
        Stopped = true;
        return default;
    }

    // --- play ---

    protected override async ValueTask OnKeepAlive(PlayCb.KeepAlivePacket p)
    {
        await SendAsync(new PlaySb.KeepAlivePacket(p.KeepAliveId));
        await SendLocalAsync(ArmAnimationPacket.GetPacketId(pv), new ArmAnimationPacket(Hand: 0));
        Console.WriteLine($"[play] keep-alive {p.KeepAliveId} — ответили и махнули рукой");

        if (++_playKeepAlives % 4 == 0) // ~раз в минуту
        {
            var aliveSec = (int)(DateTime.UtcNow - _startedAt).TotalSeconds;
            await SendChatAsync($"живу {aliveSec} сек на транспорте 2.0");
        }
    }

    protected override async ValueTask OnPlayerPosition(PlayCb.PlayerPositionPacket p)
    {
        await SendAsync(new PlaySb.TeleportConfirmPacket(p.TeleportId));
        Console.WriteLine($"[play] позиция: x={p.X:F1} y={p.Y:F1} z={p.Z:F1} — телепорт {p.TeleportId} подтверждён");

        if (!_greeted) // первый PlayerPosition = спавн
        {
            _greeted = true;
            await SendChatAsync("Привет! Я MinimalBot на транспорте 2.0");
            await SendChatAsync("login по одному, play потоком, id из типов");
            Console.WriteLine("[play] спавн — поздоровались в чате");
        }
    }

    protected override ValueTask OnUpdateHealth(PlayCb.UpdateHealthPacket p)
    {
        Console.WriteLine($"[play] здоровье: {p.Health}, еда {p.Food}");
        return default;
    }

    // --- незнакомое: имя из реестра, а не «0x2E» из головы ---

    protected override ValueTask OnUnknown(in IncomingPacket raw)
    {
        if (_unknownSeen.Add((Phase, raw.Id)))
        {
            var packetName = PacketRegistry.TryResolve(raw.Id, pv, Phase, Direction, out var desc)
                ? desc.Identity.Name
                : $"0x{raw.Id:X2}";
            Console.WriteLine($"  .. [{Phase}] пропущен {packetName} ({raw.Body.Length} байт) — и все следующие такие");
        }

        return default;
    }

    // --- отправка: до перехода через login-соединение, после — через потоковое ---

    private ValueTask SendAsync<T>(T packet) where T : class, IPacket<T>
        => _game is { } game ? game.SendAsync(packet, pv) : login.SendAsync(packet, pv);

    // --- локальные пакеты (чата и жеста нет в спеках): прежний ручной путь отправки ---

    private async ValueTask SendChatAsync(string text)
    {
        if (text.Length > 256) text = text[..256];
        await SendLocalAsync(ChatMessagePacket.GetPacketId(pv),
            new ChatMessagePacket(text, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), Salt: 0));
    }

    private async ValueTask SendLocalAsync<T>(int id, T packet) where T : IProtocolType<T>
    {
        var writer = MinecraftPrimitiveWriterCache.Rent();
        try
        {
            packet.Write(writer, pv);
            if (_game is { } game)
            {
                game.WritePacket(id, writer.WrittenSpan);
                await game.FlushAsync();
            }
            else
            {
                await login.WritePacketAsync(id, writer.WrittenMemory);
            }
        }
        finally
        {
            MinecraftPrimitiveWriterCache.Return(writer);
        }
    }
}
