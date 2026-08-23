# Первый бот за 15 минут

Что получится в конце: консольный бот, который подключается к серверу в offline-режиме, входит, отвечает на keep-alive и печатает своё здоровье. Полная программа — `examples/MinimalBot/Program.cs` в репозитории; здесь она разобрана по шагам.

## 1. Установка

```
dotnet add package McProtoNet --prerelease
```

`McProtoNet` тянет остальные четыре NuGet-пакета (`Transport`, `Protocol`, `Primitives`, `NBT`). Нужен .NET 8 или новее.

## 2. Выбрать версию протокола

Каждая отправка и каждый декод принимают число — **версию протокола** (`pv`). Это формат провода той версии игры, к которой вы подключаетесь; не версия NuGet-пакета.

| Версия игры | Протокол | Константа |
| --- | --- | --- |
| 1.16 | 735 | `MinecraftVersion.V1_16` |
| 1.18.2 | 758 | `MinecraftVersion.V1_18_2` |
| 1.20.2 | 764 | `MinecraftVersion.V1_20_2` |
| 1.21 – 1.21.1 | 767 | `MinecraftVersion.V1_21_To_1_21_1` |
| 1.21.4 | 769 | `MinecraftVersion.V1_21_4` |
| 1.21.7 – 1.21.8 | 772 | `MinecraftVersion.V1_21_7_To_1_21_8` |
| 1.21.11 | 774 | `MinecraftVersion.V1_21_11` |
| 26.1 – 26.1.2 | 775 | `MinecraftVersion.V26_1_To_26_1_2` |
| 26.2 | 776 | `MinecraftVersion.V26_2` |

Полный список (со снапшотами и пререлизами) — `MinecraftVersion` в справке API. Поддержанный диапазон: 735–776. Серверы с ViaVersion принимают старых клиентов, поэтому 772 — безопасное значение для проверки.

## 3. Четыре фазы

Сессия Minecraft проходит четыре **фазы**; один и тот же id пакета в каждой значит своё, поэтому бот сам следит за фазой.

| Фаза | Кто говорит | Что происходит | Вы шлёте |
| --- | --- | --- | --- |
| `handshaking` | клиент | один пакет: версия протокола, хост, порт, «хочу войти» | `SetProtocolPacket` |
| `login` | оба | имя, при необходимости шифрование и сжатие, успех | `LoginStartPacket`, `EncryptionResponsePacket`, `LoginAcknowledgedPacket` |
| `configuration` | оба | настройки клиента, данные реестров, известные паки | `ClientInformationPacket`, `SelectKnownPacksPacket`, `FinishConfigurationPacket` |
| `play` | оба | сама игра: keep-alive, позиции, чат, инвентарь | что делает бот; минимум — `KeepAlivePacket` и `TeleportConfirmPacket` |

Библиотека не крутит эту машину состояний за вас (см. [Чего библиотека не делает](non-goals.md)). Она даёт сгенерированный базовый класс обработчика с одним методом `On<ИмяПакета>` на каждый пакет и свойство `Phase`, которое вы переключаете, когда сервер говорит.

## 4. Программа

```csharp
using McProtoNet;
using McProtoNet.Protocol;
using HandshakeSb = McProtoNet.Protocol.Packets.Handshaking.Serverbound;
using LoginSb = McProtoNet.Protocol.Packets.Login.Serverbound;

const int Pv = 772;                                    // 1.21.8

await using var client = new MinecraftClient(new MinecraftClientOptions { Host = "127.0.0.1", Port = 25565 });
await client.ConnectAsync();

// handshaking → login: два пакета, дальше говорит сервер
await client.SendAsync(new HandshakeSb.SetProtocolPacket(Pv, "127.0.0.1", 25565, 2), Pv);
await client.SendAsync(new LoginSb.LoginStartPacket("McProtoBot", V764_Last: new(Guid.NewGuid())), Pv);

var bot = new Bot(client, Pv);
try
{
    await foreach (var packet in client.ReadPacketsAsync())   // сырые кадры, по одному
    {
        await bot.HandleAsync(in packet, Pv);                  // декод по (фаза, id) → On<Имя>
        if (bot.Stopped) break;
    }
}
catch (EndOfStreamException)
{
    // сервер закрыл сокет — обычный конец сессии, не ошибка
}
```

Обработчик — класс, который переопределяет только нужное. Важные строки — переключение фазы:

```csharp
sealed class Bot(MinecraftClient client, int pv) : ClientboundHandler
{
    public bool Stopped { get; private set; }

    // login
    protected override ValueTask OnLoginCompress(LoginCb.LoginCompressPacket p)
    {
        client.CompressionThreshold = p.Threshold;             // дальше кадры сжаты
        return default;
    }

    protected override async ValueTask OnEncryptionRequest(LoginCb.EncryptionRequestPacket p)
    {
        if (p.V766_Last is { ShouldAuthenticate: true }) { Stopped = true; return; }   // online-сервер: не поддержан
        using var rsa = EncryptionHelpers.DecodeRSAPublicKey(p.PublicKey)!;
        var secret = EncryptionHelpers.GenerateAESPrivateKey();
        await client.SendAsync(new LoginSb.EncryptionResponsePacket(rsa.Encrypt(secret, false), rsa.Encrypt(p.VerifyToken, false)), pv);
        client.EnableEncryption(secret);                        // дальше кадры зашифрованы
    }

    protected override async ValueTask OnLoginSuccess(LoginCb.LoginSuccessPacket p)
    {
        await client.SendAsync(new LoginSb.LoginAcknowledgedPacket(), pv);
        Phase = PacketPhase.Configuration;                      // ← смена фазы
        await client.SendAsync(new ConfSb.ClientInformationPacket("en_us", 2, 0, true, 0x7F, 1, false, true, V768_Last: new(ParticleStatus.All)), pv);
    }

    // configuration
    protected override ValueTask OnSelectKnownPacks(ConfCb.SelectKnownPacksPacket p)
        => client.SendAsync(new ConfSb.SelectKnownPacksPacket(p.Packs), pv);

    protected override async ValueTask OnFinishConfiguration(ConfCb.FinishConfigurationPacket p)
    {
        await client.SendAsync(new ConfSb.FinishConfigurationPacket(), pv);
        Phase = PacketPhase.Play;                               // ← смена фазы
    }

    // play
    protected override ValueTask OnKeepAlive(PlayCb.KeepAlivePacket p)
        => client.SendAsync(new PlaySb.KeepAlivePacket(p.KeepAliveId), pv);

    protected override ValueTask OnPlayerPosition(PlayCb.PlayerPositionPacket p)
        => client.SendAsync(new PlaySb.TeleportConfirmPacket(p.TeleportId), pv);

    protected override ValueTask OnUpdateHealth(PlayCb.UpdateHealthPacket p)
    {
        Console.WriteLine($"здоровье {p.Health}, еда {p.Food}");
        return default;
    }

    protected override ValueTask OnKickDisconnect(PlayCb.KickDisconnectPacket p)
    {
        Stopped = true;
        return default;
    }
}
```

Запустите против локального сервера в offline-режиме (`online-mode=false` в `server.properties`) — и увидите строку со здоровьем, как только окажетесь в мире.

## 5. Когда ломается

| Видите | Значит | Делать |
| --- | --- | --- |
| `EndOfStreamException` из `ReadPacketsAsync` | сервер закрыл соединение — после кика, по простою или при обычном выходе | ловить вокруг цикла; причину искать в последнем `KickDisconnect` / `LoginDisconnect` |
| вызван `OnLoginDisconnect` / `OnKickDisconnect` | сервер вас не пустил; в пакете текст причины | напечатать причину, остановить цикл |
| `EncryptionRequest` с `ShouldAuthenticate = true` | online-сервер, хочет учётку Mojang | библиотека пока не умеет — берите offline-сервер |
| `ProtocolNotSupportException` на `SendAsync` | такого пакета нет на этой версии протокола | сверьтесь с таблицей версий; возьмите пакет, который есть на вашем `pv` |
| `PacketDecodeException` | тело не совпало со спекой этой версии | неверный `pv` или пакет, который генератор ещё не покрывает — баннер сверху про это честно |
| цикл замирает после входа | не ответили на `KeepAlive` или не переключили `Phase` | смотрите строки смены фазы выше |

## Куда дальше

- [Чего библиотека не делает](non-goals.md) — до того, как планировать поиск пути.
- [Словарь](glossary.md) — кадр, фаза, ординал, каталог и другие слова из API.
- [Справка API](xref:McProtoNet) — сначала `MinecraftClient` и `ClientboundHandler`.
