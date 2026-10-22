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
    // Можно указать прокси 
    // Proxy = ... 
});
```

## Запуск

Когда `MinecraftClient` настроен, можно установить соединение с сервером с помощью метода
`ConnectAsync`:

```C#
await _client.ConnectAsync();
```

После успешного вызова этого метода устанавливается TCP-соединение с сервером.


## Авторизация и аутентификация

Теперь, согласно протоколу Minecraft, необходимо выполнить несколько шагов, чтобы бот смог
авторизоваться и войти в игру.

Для удобства сделаем метод расширения под названием `Login`.

```C#
public static async Task Login(
    this IMinecraftClient client, 
    string nickname, 
    string host, 
    ushort port)
```

### Рукопожатие

Серверу необходимо передать информацию о текущем соединении, включая версию протокола и дальнейшие действия. 
Эту задачу выполняет пакет рукопожатия, представленный в McProtoNet классом `SetProtocolPacket`.

```C#
await client.SendPacket(
    new SetProtocolPacket()
    {
        NextState = 2,
        ProtocolVersion = client.ProtocolVersion,
        ServerHost = host,
        ServerPort = port
    });
```

### Режим Login

Когда `NextState` равен 2, сервер воспринимает текущее соединение как вход игрока в игру. На этом этапе необходимо передать серверу наш никнейм:

```C#  
await client.SendPacket(  
    new LoginStartPacket  
    {  
        Username = nickname  
    });  
```

Вслед за этим сервер нам отправит некоторые важные пакеты.
Давайте их прочитаем и обработаем:

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
что бот успешно прошёл авторизацию.

## Режим конфигурации

Шаги выше приводят к успешному заходу бота на сервер, но лишь на версиях ниже 764.
Если же Вы запускаете бота на новых версиях майнкрафт, то будьте готовы
пройти [режим конфигурации](https://minecraft.wiki/w/Java_Edition_protocol#Configuration).

Для этого перед этим нужно отправить на сервер пакет `LoginAcknowledged`:

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

После получения `FinishConfigurationPacket` бот может полноценно переходить в `Play` режим.


