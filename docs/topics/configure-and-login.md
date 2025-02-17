# Настройка и логинизация бота

В этом разделе мы разберемся с азами подключения бота к серверу Minecraft.

<warning>
Важно! Примеры кода ниже могут иногда некорретно работать на серверах с разными конфигурациями и плагинами,
которые отправляют специфичные пакеты. Этот пример только показывает как подключится к самому обычному ванильному
серверу версии 1.21.4. Внимательно изучите протокол Minecraft, если хотите работать на других версиях и серверах.
</warning>

## Настройка

Сперва нужно создать и настроить класс `MinecraftClient`:

```C#
MinecraftVersion _version = MinecraftVersion.V1_21_4;

_client = new MinecraftClient(new MinecraftClientStartOptions()
{
    ConnectTimeout = TimeSpan.FromSeconds(5),
    Host = host,
    Port = 25565,
    WriteTimeout = TimeSpan.FromSeconds(5),
    ReadTimeout = TimeSpan.FromSeconds(5),
    Version = (int)_version,
    // Proxy = ... можно указать прокси 
});
```

## Рукопожатие

После настройки `MinecraftClient` можно установить соединение с сервером используя метод
`ConnectAsync`.

```C#
await _client.ConnectAsync();
```

После успешного подключения клиент установил TCP-соединение с вашим сервером. Теперь,
согласно протоколу Minecraft, нужно пройти некоторые шаги для того чтобы бот зашел на сервер.

Для удобства сделаем метод расширения под названием `Login`.

```C#
public static async Task Login(
    this IMinecraftClient client, 
    string nickname, 
    string host, 
    ushort port)
```


### Рукопожатие

После установки соединения серверу нужно дать некоторую информацию об текущем соединение,
такую как: версия протокола и дальнейшее действие. За это отвечает пакет рукопожатия.
В McProtoNet за него отвечает класс под названием `SetProtocolPacket`.

```C#
await client.SendPacket(
    new SetProtocolPacket()
    {
        NextState = 2, // 2 - означает Login
        ProtocolVersion = client.ProtocolVersion,
        ServerHost = host,
        ServerPort = port
    });
```

### Авторизация и аутентификация

Исходя из `NextState` равному 2, сервер понимает, что теперь текущее соединение это заход
игрока на сервер для дальнейшей игры. Теперь нужно серверу дать наш никнейм:

```C#
await client.SendPacket(
    new LoginStartPacket
    {
        Username = nickname
    });
```

После отправки пакетов выше сервер нам отправит некоторые важные пакеты.
Давайте их прочитаем:

```C#
await foreach (var serverPacket in 
    client.OnAllPackets(PacketState.Login))
{
    // Обработка
}
```


#### Сжатие

<note>
Ниже для удобства CLogin это алиас 
<code>McProtoNet.Protocol.Packets.Login.Clientbound</code>
</note>

```C#
if (serverPacket is CLogin.CompressPacket compressPacket)
{
    client.SwitchCompression(compressPacket.Threshold);
}
```

#### Шифрование

```C#
if (serverPacket is CLogin.EncryptionBeginPacket encryptBegin)
{
    var RSAService = CryptoHandler.DecodeRSAPublicKey(encryptBegin.PublicKey);
    var secretKey = CryptoHandler.GenerateAESPrivateKey();

    var sharedSecret = RSAService.Encrypt(secretKey, false);
    var verifyToken = RSAService.Encrypt(encryptBegin.VerifyToken, false);

    var response = new EncryptionBeginPacket // это клиентский пакет
    {
        SharedSecret = sharedSecret,
        VerifyToken = verifyToken
    };

    await client.SendPacket(response);

    client.SwitchEncryption(secretKey);
}
```

#### Кик с сервера

```C#
if (serverPacket is CLogin.DisconnectPacket disconnectPacket)
{
    Console.WriteLine("Disconnect: " + disconnectPacket.Reason);
}
```

#### Успешный вход

Если сервер Вас не кикнул и отправил все нужные пакеты, то это означает,
что Вы успешно прошли авторизацию.

### Режим конфигурации

Шаги выше приводят к упешному заходу бота на сервер, но лишь на версиях ниже 764.
Если же Вы хапускаете бота на новых версиях майнкрафт, то будьте готовы
пройти [режим конфигурации](https://minecraft.wiki/w/Java_Edition_protocol#Configuration).

Чтобы войти в режим конфигурации нужно отправить на сервер пакет `LoginAcknowledged`:

```C#
await client.SendPacket(new LoginAcknowledgedPacket());
```

Теперь текущее соединение находится в режиме конфигурации. Отправим пакет с настройками:

```C#
await client.SendPacket(new SConfig.SettingsPacket()
{
    Locale = "ru_ru",
    ViewDistance = 16,
    EnableServerListing = false,
    EnableTextFiltering = false,
    MainHand = 0,
    SkinParts = 127,
    ChatColors = true,
    ChatFlags = 0
});
```

Затем запускаем цикл чтения и конфигурируем:

<code-block lang="C#" 
    collapsible="true"
    collapsed-title="Configuration" 
    src="../code-samples/Configuration.cs"/>


