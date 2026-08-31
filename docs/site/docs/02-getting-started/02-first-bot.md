# Первый бот

Бот из этой главы заходит на сервер, доживает до игрового мира и остаётся там:
отвечает на keep-alive, подтверждает телепорты, пишет в консоль, что с ним
происходит. Играть он не умеет, но всё, что нужно для игры, у него уже под
рукой.

Нужен сервер с `online-mode=false`: поход в Mojang за сессией библиотека
не делает. Версия протокола задаётся целым числом: здесь это 772, то есть 1.21.8.
Номер для своей версии - в таблице
[«Версия → протокол»](../07-reference/01-version-to-protocol.md).

Полный текст примера лежит в репозитории: `examples/MinimalBot`.

## Подключение

Клиент открывает TCP-соединение и дальше только гоняет пакеты.

```csharp
const int Pv = 772;

await using var client = new MinecraftClient(new MinecraftClientOptions
{
    Host = "127.0.0.1",
    Port = 25565
});
await client.ConnectAsync();
```

Первые два пакета уходят сразу: рукопожатие с номером протокола и заявка
на вход. Число 2 в конце рукопожатия - это переход в login.

```csharp
await client.SendAsync(
    new HandshakeSb.SetProtocolPacket(Pv, "127.0.0.1", 25565, 2), Pv);
await client.SendAsync(
    new LoginSb.LoginStartPacket("McProtoBot", V764_Last: new(Guid.NewGuid())),
    Pv);
```

## Обработчик

Входящие пакеты разбирает наследник `ClientboundHandler`. Метод на каждый
пакет уже объявлен, переопределяются только нужные.

```csharp
sealed class Bot(MinecraftClient client, int pv) : ClientboundHandler
{
    protected override ValueTask OnLoginCompress(LoginCb.LoginCompressPacket packet)
    {
        client.CompressionThreshold = packet.Threshold;
        return default;
    }

    protected override async ValueTask OnKeepAlive(PlayCb.KeepAlivePacket packet)
    {
        await client.SendAsync(new PlaySb.KeepAlivePacket(packet.KeepAliveId), pv);
    }
}
```

Пакет, для которого метод не переопределён, попадает в `OnUnknown`. Это
нормальное состояние потока, а не ошибка: сервер шлёт много такого, что боту
не нужно.

## Цикл чтения

Пакеты приходят потоком, цикл живёт в коде приложения.

```csharp
var bot = new Bot(client, Pv);

using var cts = new CancellationTokenSource();
Console.CancelKeyPress += (_, e) => { e.Cancel = true; cts.Cancel(); };

try
{
    await foreach (var packet in client.ReadPacketsAsync(cts.Token))
        await bot.HandleAsync(in packet, Pv);
}
catch (OperationCanceledException)
{
    // остановили сами, по Ctrl+C
}
catch (EndOfStreamException)
{
    // сервер закрыл соединение
}
```

Без токена чтение ждёт следующий пакет столько, сколько сервер молчит.
Оборвать цикл можно и с другой стороны - `Abort` или `DisposeAsync` из
соседней задачи, - но токен для этого удобнее всего.

Конец сессии всегда прилетает исключением. Чистый разрыв - это
`EndOfStreamException`; тихо перечисление не заканчивается.

Пакет живёт до следующего чтения: его данные - это окно в буфер, а не
собственная копия. Разбирать надо сразу, тащить через `await` нельзя.

## Фазы переключает бот

Библиотека сама не решает, когда login закончился. Это делает код
в двух местах.

```csharp
protected override async ValueTask OnLoginSuccess(LoginCb.LoginSuccessPacket packet)
{
    await client.SendAsync(new LoginSb.LoginAcknowledgedPacket(), pv);
    Phase = PacketPhase.Configuration;
}

protected override async ValueTask OnFinishConfiguration(ConfCb.FinishConfigurationPacket packet)
{
    await client.SendAsync(new ConfSb.FinishConfigurationPacket(), pv);
    Phase = PacketPhase.Play;
}
```

Сервер может вернуть игрока обратно в configuration прямо из игры: приходит
`StartConfiguration`, бот подтверждает и переставляет `Phase` назад. Подробнее
про весь путь - [«Фазы протокола»](../05-packets/01-phases-and-direction.md).

## Три ответа, без которых бот не доживёт до мира

Сервер ждёт ответа на три пакета, и молчание в любом из них кончается тем,
что бот повисает в configuration или получает кик после спавна.

Сразу после `LoginAcknowledged` уходят настройки клиента - язык, дальность
прорисовки, видимые части скина:

```csharp
await client.SendAsync(new ConfSb.ClientInformationPacket(
    "en_us", 2, 0, true, 0x7F, 1, false, true,
    V768_Last: new(ParticleStatus.All)), pv);
```

Дальше сервер присылает список известных ему наборов данных и ждёт, что
клиент подтвердит тот же список:

```csharp
protected override ValueTask OnSelectKnownPacks(ConfCb.SelectKnownPacksPacket packet)
    => client.SendAsync(new ConfSb.SelectKnownPacksPacket(packet.Packs), pv);
```

А в play каждый телепорт - включая первый, на спавне - нужно подтвердить
его номером, иначе сервер решит, что клиент завис:

```csharp
protected override ValueTask OnPlayerPosition(PlayCb.PlayerPositionPacket packet)
    => client.SendAsync(new PlaySb.TeleportConfirmPacket(packet.TeleportId), pv);
```

## Шифрование

Даже на offline-сервере шифрование включается: приходит
`EncryptionRequestPacket`, бот отвечает своим ключом и включает шифр. С 1.20.5
сервер шифрует поток и без проверки сессии - в протоколе это описано на
странице [Encryption](https://minecraft.wiki/w/Java_Edition_protocol/Encryption),
раздел History.

```csharp
using var rsa = EncryptionHelpers.DecodeRSAPublicKey(packet.PublicKey)!;
var secret = EncryptionHelpers.GenerateAESPrivateKey();

await client.SendAsync(new LoginSb.EncryptionResponsePacket(
    rsa.Encrypt(secret, false),
    rsa.Encrypt(packet.VerifyToken, false)), pv);

client.EnableEncryption(secret);
```

Если сервер требует подтверждения сессии у Mojang, дальше пример не пойдёт.

## Дальше

- [Фазы протокола](../05-packets/01-phases-and-direction.md)
- [Поток пакетов](../04-transport/03-packet-stream.md)
- [Пакет и его идентификатор](../05-packets/03-from-raw-packet.md)
