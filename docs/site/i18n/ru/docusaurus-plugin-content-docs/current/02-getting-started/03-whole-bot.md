# Бот целиком

Всё, что разобрано по кускам в [«Первом боте»](02-first-bot.md), одним файлом:
подключение, обработчик, цикл чтения, переключение фаз. Тот же код лежит в
репозитории как `examples/MinimalBot` и запускается без правок - из корня
клона McProtoNet:

```
dotnet run --project examples/MinimalBot -- 127.0.0.1 25565 McProtoBot
```

Аргументы - адрес, порт и имя бота; если их нет, берутся `127.0.0.1`, `25565`
и `McProtoBot`. Номер протокола задан константой `Pv` в начале листинга.
Сервер нужен с `online-mode=false`: на проверке сессии у Mojang пример
останавливается.

## Что видно в консоли

Вывод сокращён: пропущенных пакетов в play заметно больше. Строки про порог
сжатия в нём нет, потому что на этом сервере сжатие выключено.

```text
MinimalBot: McProtoBot -> 127.0.0.1:25565 (pv 772)
[net] connection open
[login] handshake + login start sent
[login] cipher on: AES/CFB8
[login] success: McProtoBot ccf52582-9c5b-304f-a243-15437255b5eb
[config] in configuration, client information sent
[config] known packs: 1, acknowledged
  .. [Configuration] skipped 0x07 (1611 bytes) - and every later one
[play] configuration finished, in play
  .. [Play] skipped 0x7E (16779 bytes) - and every later one
  .. [Play] skipped 0x45 (8 bytes) - and every later one
[play] position: x=40,6 y=63,0 z=-58,6 - teleport 1 confirmed
[play] spawned
  .. [Play] skipped EntityMetadata (11 bytes) - and every later one
[play] health: 20, food 20
[play] keep-alive #1 - answered and waved
```

Строки с точками - пакеты, до которых у библиотеки нет своего метода: номер
не описан для этой версии, фазы и направления, либо описан, но кода для него
пока не выдано. Пакет, чей `On<Name>` просто не переопределён, разбирается и
тихо отбрасывается - в консоли его не видно. Каждая строка с точками
печатается один раз на фазу, иначе их набегали бы сотни. Имя вместо номера
появляется тогда, когда пакет нашёлся в реестре.

Цикла с `CancellationTokenSource` в листинге нет: пример останавливается на
кике или на конце потока. Токен из раздела «Цикл чтения» добавляется тем,
кому нужна остановка снаружи.

## Листинг

```csharp
// dotnet run [host] [port] [name] - defaults are 127.0.0.1 25565 McProtoBot (pv 772)

using McProtoNet;
using McProtoNet.Primitives;
using McProtoNet.Protocol;
using McProtoNet.Transport.Cryptography;
using ConfCb = McProtoNet.Protocol.Packets.Configuration.Clientbound;
using ConfSb = McProtoNet.Protocol.Packets.Configuration.Serverbound;
using HandshakeSb = McProtoNet.Protocol.Packets.Handshaking.Serverbound;
using LoginCb = McProtoNet.Protocol.Packets.Login.Clientbound;
using LoginSb = McProtoNet.Protocol.Packets.Login.Serverbound;
using PlayCb = McProtoNet.Protocol.Packets.Play.Clientbound;
using PlaySb = McProtoNet.Protocol.Packets.Play.Serverbound;

const int Pv = 772;

var host = args.Length > 0 ? args[0] : "127.0.0.1";
var port = args.Length > 1 && int.TryParse(args[1], out var typed) ? typed : 25565;
var name = args.Length > 2 ? args[2] : "McProtoBot";

Console.WriteLine($"MinimalBot: {name} -> {host}:{port} (pv {Pv})");

await using var client = new MinecraftClient(new MinecraftClientOptions { Host = host, Port = port });
await client.ConnectAsync();
Console.WriteLine("[net] connection open");

await client.SendAsync(new HandshakeSb.SetProtocolPacket(Pv, host, port, 2), Pv);
await client.SendAsync(new LoginSb.LoginStartPacket(name, V764_Last: new(Guid.NewGuid())), Pv);
Console.WriteLine("[login] handshake + login start sent");

var bot = new Bot(client, Pv);

try
{
    await foreach (var packet in client.ReadPacketsAsync())
    {
        await bot.HandleAsync(in packet, Pv);
        if (bot.Stopped) break;
    }
}
catch (EndOfStreamException)
{
    // the server closed the connection - a normal end of session, the reason is logged above
}

Console.WriteLine("Connection closed.");

sealed class Bot(MinecraftClient client, int pv) : ClientboundHandler
{
    private readonly HashSet<(PacketPhase, int)> _unknownSeen = [];
    private int _keepAlives;
    private bool _spawned;

    /// <summary>The session is over: a kick, a login failure or the end of the stream.</summary>
    public bool Stopped { get; private set; }

    protected override ValueTask OnLoginCompress(LoginCb.LoginCompressPacket packet)
    {
        client.CompressionThreshold = packet.Threshold;
        Console.WriteLine($"[login] compression on, threshold {packet.Threshold}");
        return default;
    }

    protected override async ValueTask OnEncryptionRequest(LoginCb.EncryptionRequestPacket packet)
    {
        if (packet.V766_Last is { ShouldAuthenticate: true })
        {
            Console.WriteLine("[login] the server wants Mojang authentication - this example is offline only");
            Stopped = true;
            return;
        }

        using var rsa = EncryptionHelpers.DecodeRSAPublicKey(packet.PublicKey)
                        ?? throw new InvalidOperationException("could not parse the server public key");
        var secret = EncryptionHelpers.GenerateAESPrivateKey();

        await client.SendAsync(new LoginSb.EncryptionResponsePacket(
            rsa.Encrypt(secret, false),
            rsa.Encrypt(packet.VerifyToken, false)), pv);

        client.EnableEncryption(secret);
        Console.WriteLine("[login] cipher on: AES/CFB8");
    }

    protected override ValueTask OnLoginPluginRequest(LoginCb.LoginPluginRequestPacket packet)
        => client.SendAsync(new LoginSb.LoginPluginResponsePacket(packet.MessageId, null), pv);

    protected override async ValueTask OnLoginSuccess(LoginCb.LoginSuccessPacket packet)
    {
        Console.WriteLine($"[login] success: {packet.Username} {packet.Uuid}");
        await client.SendAsync(new LoginSb.LoginAcknowledgedPacket(), pv);
        Phase = PacketPhase.Configuration;

        await client.SendAsync(new ConfSb.ClientInformationPacket(
            "en_us", 2, 0, true, 0x7F, 1, false, true, V768_Last: new(ParticleStatus.All)), pv);
        Console.WriteLine("[config] in configuration, client information sent");
    }

    protected override ValueTask OnLoginDisconnect(LoginCb.LoginDisconnectPacket packet)
        => Kicked("login", packet.Reason);

    protected override ValueTask OnConfigurationKeepAlive(ConfCb.KeepAlivePacket packet)
        => client.SendAsync(new ConfSb.KeepAlivePacket(packet.KeepAliveId), pv);

    protected override ValueTask OnConfigurationPing(ConfCb.PingPacket packet)
        => client.SendAsync(new ConfSb.PongPacket(packet.Id), pv);

    protected override async ValueTask OnSelectKnownPacks(ConfCb.SelectKnownPacksPacket packet)
    {
        await client.SendAsync(new ConfSb.SelectKnownPacksPacket(packet.Packs), pv);
        Console.WriteLine($"[config] known packs: {packet.Packs.Length}, acknowledged");
    }

    protected override async ValueTask OnFinishConfiguration(ConfCb.FinishConfigurationPacket packet)
    {
        await client.SendAsync(new ConfSb.FinishConfigurationPacket(), pv);
        Phase = PacketPhase.Play;
        Console.WriteLine("[play] configuration finished, in play");
    }

    protected override ValueTask OnDisconnect(ConfCb.DisconnectPacket packet)
        => Kicked("config", packet.V764?.ReasonJson ?? packet.V765_Last?.Reason.ToString());

    protected override async ValueTask OnKeepAlive(PlayCb.KeepAlivePacket packet)
    {
        await client.SendAsync(new PlaySb.KeepAlivePacket(packet.KeepAliveId), pv);
        await client.SendAsync(new PlaySb.ArmAnimationPacket(0), pv);
        Console.WriteLine($"[play] keep-alive #{++_keepAlives} - answered and waved");
    }

    protected override ValueTask OnPing(PlayCb.PingPacket packet)
        => client.SendAsync(new PlaySb.PongPacket(packet.Id), pv);

    protected override async ValueTask OnStartConfiguration(PlayCb.StartConfigurationPacket packet)
    {
        await client.SendAsync(new PlaySb.ConfigurationAcknowledgedPacket(), pv);
        Phase = PacketPhase.Configuration;
        Console.WriteLine("[config] the server sent us back to configuration");
    }

    protected override async ValueTask OnPlayerPosition(PlayCb.PlayerPositionPacket packet)
    {
        await client.SendAsync(new PlaySb.TeleportConfirmPacket(packet.TeleportId), pv);
        Console.WriteLine(
            $"[play] position: x={packet.X:F1} y={packet.Y:F1} z={packet.Z:F1} - teleport {packet.TeleportId} confirmed");

        if (!_spawned)
        {
            _spawned = true;
            Console.WriteLine("[play] spawned");
        }
    }

    protected override ValueTask OnUpdateHealth(PlayCb.UpdateHealthPacket packet)
    {
        Console.WriteLine($"[play] health: {packet.Health}, food {packet.Food}");
        return default;
    }

    protected override ValueTask OnKickDisconnect(PlayCb.KickDisconnectPacket packet)
        => Kicked("play", packet.VUntil764?.ReasonJson ?? packet.V765_Last?.Reason.ToString());

    protected override ValueTask OnUnknown(in IncomingPacket raw)
    {
        if (_unknownSeen.Add((Phase, raw.Id)))
        {
            var packetName = PacketRegistry.TryResolve(raw.Id, pv, Phase, Direction, out var desc)
                ? desc.Identity.Name
                : $"0x{raw.Id:X2}";
            Console.WriteLine($"  .. [{Phase}] skipped {packetName} ({raw.Body.Length} bytes) - and every later one");
        }

        return default;
    }

    private ValueTask Kicked(string phase, string? reason)
    {
        Console.WriteLine($"[{phase}] kick: {reason ?? "no reason given"}");
        Stopped = true;
        return default;
    }
}
```

Чат и движение дописываются прямо в класс `Bot`: методы из раздела
[«Жизнь в play»](02-first-bot.md#жизнь-в-play) встают рядом с остальными
обработчиками.

## Дальше

- [Фаза и направление](../05-packets/01-phases-and-direction.md)
- [Обработчики и неизвестные пакеты](../05-packets/04-handlers.md)
- [Отмена, ошибки, закрытие](../04-transport/06-cancellation.md)
