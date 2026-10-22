# Сеть вокруг соединения

Эти инструменты лежат в клее `McProtoNet` — рядом с `MinecraftClient`, а не в отдельной библиотеке.

## SRV Lookup

`SrvResolver` спрашивает у системного резолвера запись `_minecraft._tcp.<host>` — так же, как это
делает ванильный клиент. Записи нет — это `null`, а не исключение. Исключение только на реальный
отказ DNS.

```C#
using McProtoNet;

SrvResult? record = await SrvResolver.ResolveAsync("mc.example.com");

if (record is { } srv)
    Console.WriteLine($"SRV: {srv.Target}:{srv.Port}");
else
    Console.WriteLine("SRV записи нет — подключаемся по введённому адресу");
```

`MinecraftClient.ConnectAsync` делает этот запрос сам, если порт остался стандартным (25565) и в
`Host` стоит имя, а не IP. Отключается через `MinecraftClientOptions.UseSrv`.

Все записи сразу, если выбор нужен свой, — `SrvResolver.ResolveAllAsync`.

## LanServerDetector

Ищет миры, открытые в локальную сеть: они шлют `[MOTD]…[/MOTD][AD]…[/AD]` на multicast-группу
`224.0.2.60:4445` примерно раз в полторы секунды.
Подробнее по [ссылке](https://minecraft.fandom.com/wiki/Tutorials/Setting_up_a_LAN_world).

Разовый поиск в течение окна, каждый мир один раз:

```C#
using McProtoNet;

IReadOnlyList<LanServer> servers = await LanServerDetector.DiscoverAsync(TimeSpan.FromSeconds(5));

foreach (var server in servers)
    Console.WriteLine($"{server.Motd} — {server.EndPoint}");
```

Непрерывный поток объявлений, как они приходят (повторы не убираются — это сырая лента):

```C#
await using var detector = new LanServerDetector();

await foreach (var server in detector.ListenAsync(cancellationToken))
    Console.WriteLine($"{server.Motd} — {server.EndPoint}");
```

Порт 4445 занимает сам `LanServerDetector`, а не перечислитель: `await using` отдаёт порт всегда —
и по отмене, и по `break`, и если перечислитель бросили с недочитанной датаграммой. Токен обязателен:
слушание бесконечное, и это единственный способ его остановить. Одновременно слушать одним
детектором нельзя — второй `ListenAsync` бросит `InvalidOperationException`.
