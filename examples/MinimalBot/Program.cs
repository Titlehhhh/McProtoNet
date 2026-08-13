// MinimalBot — исполняемая документация пакетного слоя, теперь на дизайне-симбиозе:
// handler-база ClientboundHandler из генерата, типизированная отправка SendAsync (id из типа),
// имена незнакомых пакетов из PacketRegistry. Ручного свитча по id больше нет.
//
// Использование: dotnet run [host] [port] [name]
// По умолчанию: 127.0.0.1 25566 McProtoBot (paper-1.21.8, pv 772)

using System.Net.Sockets;
using McProtoNet.Client;
using McProtoNet.Net;
using McProtoNet.Protocol;
using McProtoNet.Serialization;
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

Console.WriteLine($"MinimalBot: {name} -> {host}:{port} (pv {Pv}, дизайн-симбиоз)");

using var tcp = new TcpClient();
await tcp.ConnectAsync(host, port);
await using var client = PipelinesMinecraftClient.Create(tcp.GetStream(), Pv);

// handshake: intent=2 (login), дальше сразу login start; id склеивает SendAsync
await client.SendAsync(new HandshakeSb.SetProtocolPacket(Pv, host, port, 2), Pv);
await client.SendAsync(new LoginSb.LoginStartPacket(name, V764_Last: new(Guid.NewGuid())), Pv);
Console.WriteLine("[login] handshake + login start отправлены");

var bot = new Bot(client, Pv);
await foreach (var packet in client.ReadPacketsAsync())
{
    await bot.HandleAsync(in packet, Pv);
    if (bot.Stopped) break;
}

Console.WriteLine("Соединение закрыто.");

// Вся логика — переопределения handler-базы; фазу ведёт бот через слот Phase.
sealed class Bot(PipelinesMinecraftClient client, int pv) : ClientboundHandler
{
    private readonly DateTime _startedAt = DateTime.UtcNow;
    private readonly HashSet<(PacketPhase, int)> _unknownSeen = [];
    private int _playKeepAlives;
    private bool _greeted;

    public bool Stopped { get; private set; }

    // --- login → configuration ---

    protected override ValueTask OnLoginCompress(LoginCb.LoginCompressPacket p)
    {
        client.CompressionThreshold = p.Threshold;
        Console.WriteLine($"[login] компрессия включена, порог {p.Threshold}");
        return default;
    }

    protected override async ValueTask OnLoginSuccess(LoginCb.LoginSuccessPacket p)
    {
        Console.WriteLine($"[login] успех: {p.Username} {p.Uuid}");
        await client.SendAsync(new LoginSb.LoginAcknowledgedPacket(), pv);
        Phase = PacketPhase.Configuration;
        await client.SendAsync(
            new ConfSb.ClientInformationPacket("en_us", 8, 0, true, 0x7F, 1, false, true, V768_Last: new(0)), pv);
        Console.WriteLine("[config] вошли в configuration, client information отправлена");
    }

    protected override ValueTask OnLoginDisconnect(LoginCb.LoginDisconnectPacket p)
    {
        Console.WriteLine($"[login] кик: {p.Reason}");
        Stopped = true;
        return default;
    }

    // --- configuration ---

    protected override ValueTask OnConfigurationKeepAlive(ConfCb.KeepAlivePacket p)
        => client.SendAsync(new ConfSb.KeepAlivePacket(p.KeepAliveId), pv);

    // Play gained its own Ping in the 2026-08-11 spec cycle, so the bare OnPing went to it
    // and the configuration one took the phase prefix — the same shape OnConfigurationKeepAlive
    // above already had.
    protected override ValueTask OnConfigurationPing(ConfCb.PingPacket p)
        => client.SendAsync(new ConfSb.PongPacket(p.Id), pv);

    protected override async ValueTask OnSelectKnownPacks(ConfCb.SelectKnownPacksPacket p)
    {
        await client.SendAsync(new ConfSb.SelectKnownPacksPacket(p.Packs), pv);
        Console.WriteLine($"[config] known packs: {p.Packs.Length} шт, подтвердили");
    }

    protected override async ValueTask OnFinishConfiguration(ConfCb.FinishConfigurationPacket p)
    {
        await client.SendAsync(new ConfSb.FinishConfigurationPacket(), pv);
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
        await client.SendAsync(new PlaySb.KeepAlivePacket(p.KeepAliveId), pv);
        await SendLocalAsync(ArmAnimationPacket.GetPacketId(pv), new ArmAnimationPacket(Hand: 0));
        Console.WriteLine($"[play] keep-alive {p.KeepAliveId} — ответили и махнули рукой");

        if (++_playKeepAlives % 4 == 0) // ~раз в минуту
        {
            var aliveSec = (int)(DateTime.UtcNow - _startedAt).TotalSeconds;
            await SendChatAsync($"живу {aliveSec} сек на handler-базе симбиоза");
        }
    }

    protected override async ValueTask OnPlayerPosition(PlayCb.PlayerPositionPacket p)
    {
        await client.SendAsync(new PlaySb.TeleportConfirmPacket(p.TeleportId), pv);
        Console.WriteLine($"[play] позиция: x={p.X:F1} y={p.Y:F1} z={p.Z:F1} — телепорт {p.TeleportId} подтверждён");

        if (!_greeted) // первый PlayerPosition = спавн
        {
            _greeted = true;
            await SendChatAsync("Привет! Я MinimalBot на дизайне-симбиозе");
            await SendChatAsync("handler-база из генерата, id из типов, незнакомые пакеты зову по имени");
            Console.WriteLine("[play] спавн — поздоровались в чате");
        }
    }

    protected override ValueTask OnUpdateHealth(PlayCb.UpdateHealthPacket p)
    {
        Console.WriteLine($"[play] здоровье: {p.Health}, еда {p.Food}");
        return default;
    }

    // --- незнакомое: имя из реестра, а не «0x2E» из головы ---

    protected override ValueTask OnUnknown(in InputPacket raw)
    {
        if (_unknownSeen.Add((Phase, raw.Id)))
        {
            var packetName = PacketRegistry.TryResolve(raw.Id, pv, Phase, Direction, out var desc)
                ? desc.Identity.Name
                : $"0x{raw.Id:X2}";
            Console.WriteLine($"  .. [{Phase}] пропущен {packetName} ({raw.Data.Length} байт) — и все следующие такие");
        }

        return default;
    }

    // --- локальные пакеты (чата и жеста нет в спеках): прежний ручной путь отправки ---

    private async ValueTask SendChatAsync(string text)
    {
        if (text.Length > 256) text = text[..256];
        await SendLocalAsync(ChatMessagePacket.GetPacketId(pv),
            new ChatMessagePacket(text, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), Salt: 0));
    }

    private async ValueTask SendLocalAsync<T>(int id, T packet) where T : IProtocolType<T>
    {
        var writer = new MinecraftPrimitiveWriter();
        writer.WriteVarInt(id);
        packet.Write(writer, pv);
        using var owner = writer.GetWrittenMemory();
        await client.SendPacketAsync(owner.Memory);
    }
}
