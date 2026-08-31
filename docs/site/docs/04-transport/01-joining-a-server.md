# Вход на сервер

Соединение открывается через `MinecraftClient` и `MinecraftClientOptions`.
Опции задают адрес, тайминги и, при необходимости, прокси; `ConnectAsync`
резолвит адрес и открывает сокет. Отправка первых пакетов - handshaking
и login - остаётся за вызывающим кодом.

## Опции и ConnectAsync

`MinecraftClientOptions.Host` обязателен, `Port` по умолчанию 25565.
`ConnectTimeout` (30 секунд по умолчанию) ограничивает весь `ConnectAsync`
целиком, включая SRV-поиск и открытие сокета. `LocalEndPoint` привязывает
исходящий сокет к конкретному интерфейсу и порту, `NoDelay` отключает
алгоритм Нейгла (включён по умолчанию) - оба поля действуют только
на прямом TCP-соединении, не через прокси.

```csharp
var options = new MinecraftClientOptions
{
    Host = "play.example.com",
    ConnectTimeout = TimeSpan.FromSeconds(15),
};

await using var client = new MinecraftClient(options);
await client.ConnectAsync(cancellationToken);
```

Токен отмены останавливает попытку раньше `ConnectTimeout`. По завершении
`ConnectAsync` пакеты ещё не отправлены - клиент только открыл соединение.

## Поиск сервера по SRV-записи

Сервер редко слушает порт 25565 на самом домене: обычно
там DNS-запись `_minecraft._tcp.<host>`, которая указывает на настоящие
хост и порт. Ванильный клиент ищет эту запись перед подключением, и
`ConnectAsync` делает то же самое, когда `UseSrv` включён (по умолчанию
да), `Port` остался равен 25565 и `Host` не IP-литерал. Поиск ограничен
`SrvTimeout` (5 секунд по умолчанию, не длиннее `ConnectTimeout`); если
записи нет или тайм-аут истёк, `ConnectAsync` подключается к `Host:Port`,
как они заданы в опциях, - это не ошибка.

Тот же поиск доступен отдельно, через `SrvResolver`:

```csharp
var record = await SrvResolver.ResolveAsync("play.example.com");
if (record is { } srv)
    Console.WriteLine($"{srv.Target}:{srv.Port}");
```

`SrvResult` несёт `Target`, `Port`, `Priority` и `Weight` - четыре поля
из RFC 2782. Когда записей несколько, `ConnectAsync` и `ResolveAsync`
выбирают одну сами: сначала по наименьшему `Priority`, затем взвешенным
выбором по `Weight` внутри этой группы.

## Прокси

Сокет можно открыть не напрямую: `MinecraftClientOptions.Proxy` принимает
`IProxyClient`, и `ConnectAsync` просит у него поток к уже резолвленным
хосту и порту вместо того, чтобы открывать `TcpClient` самому. `NoDelay`
и `LocalEndPoint` в этом случае не действуют - прокси-клиент владеет
своим сокетом и настраивает его сам. Реализации берутся из
[QuickProxyNet](https://github.com/Titlehhhh/QuickProxyNet) - отдельной
библиотеки без зависимостей. Она понимает HTTP CONNECT и SOCKS4/4a/5,
а из современного - VLESS, VMess и Trojan; QUIC и Shadowsocks не умеет.

```csharp
var options = new MinecraftClientOptions
{
    Host = "play.example.com",
    Proxy = proxyClient,
};
```

## Поиск серверов в локальной сети

Открытый мир в локальной сети раз в полторы секунды рассылает
широковещательное объявление на 224.0.2.60:4445 в формате
`[MOTD]…[/MOTD][AD]…[/AD]`. `LanServerDetector` слушает эту группу
и разбирает объявления в `LanServer` - MOTD и адрес, на котором сервер
принимает соединения.

```csharp
var found = await LanServerDetector.DiscoverAsync(TimeSpan.FromSeconds(3));
foreach (var server in found)
    Console.WriteLine($"{server.Motd} -> {server.EndPoint}");
```

`DiscoverAsync` слушает заданное окно времени и убирает повторы одного
мира по адресу. `ListenAsync` отдаёт объявления по мере поступления, без
дедупликации - для списка, который должен обновляться на лету.

## Что уходит на сервер сразу после соединения

Первым уходит handshaking-пакет с протокольной версией, адресом и портом
сервера, вторым - заявка на вход в login. Пример с кодом - в
[«Первом боте»](../02-getting-started/02-first-bot.md).

## Одно соединение на один клиент

`MinecraftClient` одноразовый. Флаг подключения взводится при первом
успешном `ConnectAsync` и снимается только тогда, когда подключиться
не удалось; после разрыва повторный `ConnectAsync` на том же экземпляре
бросает `InvalidOperationException`. Переподключение - это новый клиент,
а вместе с ним и новый обработчик: старый помнит фазу, в которой
оборвалась прошлая сессия.

Дальше сервер отвечает уже в login: сжатием, шифром или сразу успехом.
Порядок пакетов и переходы между фазами - в
[«Фазах протокола»](../05-packets/01-phases-and-direction.md), включение шифра и сжатия -
в [«Сжатии и шифровании»](05-encryption-and-compression.md). Пример
целиком, вместе с ключевым обменом и первым выходом в play, - на
странице [«Первый бот»](../02-getting-started/02-first-bot.md).
