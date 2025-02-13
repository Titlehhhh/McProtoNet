# McProtoNet.Protocol

Эта библиотека включает в себя классы и структуры, которые обеспечивают сериализацию и десериализацию пакетов и типов Minecraft для разных версий игры, добавляя поддержку мультиверсии.

## Интерфейсы

В библиотеке реализованы несколько интерфейсов для обработки пакетов:

### IPacket

`IPacket` — это базовый интерфейс для описания пакетов. Он содержит методы, которые позволяют проверять, какие версии поддерживает пакет, а также свойство типа `PacketIdentifier`, представляющее собой мета-информацию о пакете:

- **Order** — ключ, необходимый для получения идентификатора пакета для конкретной версии протокола.
- **Name** — имя пакета.
- **State** — состояние пакета (например, Handshaking — рукопожатие, Login — логин, Configuration — конфигурация).
- **Direction** — направление пакета (к серверу или клиенту).

<code-block src="../code-samples/IPacket.cs" lang="C#" collapsed-title="IPacket.cs" collapsible="true"/>

### IServerPacket

Интерфейс `IServerPacket` описывает серверный пакет. Он наследуется от `IPacket` и добавляет метод `Deserialize`.

<code-block src="../code-samples/IServerPacket.cs" lang="C#" collapsed-title="IServerPacket.cs" collapsible="true"/>

### IClientPacket

Интерфейс `IClientPacket` описывает клиентский пакет. Он также наследуется от `IPacket` и добавляет метод `Serialize`.

<code-block src="../code-samples/IClientPacket.cs" lang="C#" collapsed-title="IClientPacket.cs" collapsible="true"/>

## PacketIdentifier

Как говорилось выше `PacketIdentifier` это некоторая мета-информация об пакете.
`PacketIdentifier` представлен в виде класса, а значит может создаться впечатление,
что он аллоцируется каждый раз когда вызывается метод `GetPacketId`, но нет.
Каждый класс, описывающий пакет, просто возвращет существующий статический экземпляр
из классов таких как: `ClientConfigurationPacket`, `ClientLoginPacket`, 
`ServerPlayPacket` и другие. Эти классы напоминают перечисления, где каждый вариант -
это статическое readonly поле. Дабы стало понятнее, то вот как это примерно выглядит:

```CSharp
public static class ServerPlayPacket
{
    public static readonly PacketIdentifier Abilities = new(0, nameof(Abilities), PacketState.Play,
        PacketDirection.Clientbound);

    public static readonly PacketIdentifier AcknowledgePlayerDigging =
        new(1, nameof(AcknowledgePlayerDigging), PacketState.Play, PacketDirection.Clientbound);

    public static readonly PacketIdentifier ActionBar = new(2, nameof(ActionBar), PacketState.Play,
        PacketDirection.Clientbound);
    // ...
}
```

```CSharp
public class AbilitiesPacket : IServerPacket
{
    public PacketIdentifier GetPacketId
    {
        return ServerPlayPacket.Abilities;
    }
    // Deserialize...
}
```

С учетом приведенного кода методов из класса `PacketIdHelper`, можно более детально раскрыть процесс получения и использования идентификаторов пакетов в контексте мультиверсии. Следовательно, описание класса `PacketIdHelper` можно дополнить следующим образом:

---

## Получение идентификатора

Протокол Minecraft часто обновляется, и с ним меняются идентификаторы пакетов. Один и тот же пакет может иметь разные идентификаторы на разных версиях протокола.

Для решения этой проблемы используются автоматически генерируемые словари, где:

- **Ключ** — это комбинация версии протокола и `Order` (уникальный номер пакета для конкретной версии).
- **Значение** — это идентификатор пакета (число), который соответствует этому пакету на конкретной версии протокола.

### Работа с идентификаторами пакетов

В классе `PacketIdHelper` реализованы методы, позволяющие получить идентификатор пакета на основе его параметров и версии протокола:

1. **Метод `TryGetPacketIdentifier`**:
   Этот метод пытается найти идентификатор пакета по его `packetId`, версии протокола, состоянию и направлению (к серверу или клиенту).

2. **Метод `GetPacketIdentifier`**:
   Этот метод использует `TryGetPacketIdentifier`, но в случае, если пакет не найден, выбрасывает исключение `KeyNotFoundException`. Этот метод полезен, когда необходимо обязательно получить идентификатор пакета.

3. **Метод `TryGetPacketId`**:
   Метод `TryGetPacketId` работает в обратном направлении: он получает идентификатор пакета и пытается найти его `packetId` для конкретной версии протокола. 

4. **Метод `GetPacketId`**:
   Этот метод использует `TryGetPacketId` для получения `packetId`. Если идентификатор не найден, выбрасывается исключение `KeyNotFoundException`.

### Пример работы с идентификаторами пакетов:

```csharp
// Пример получения идентификатора пакета
int packetId = 30;
int protocolVersion = 340;
PacketState state = PacketState.Play;
PacketDirection direction = PacketDirection.Clientbound;

if (PacketIdHelper.TryGetPacketIdentifier(packetId, protocolVersion, state, direction, out var identifier))
{
    Console.WriteLine($"Packet identifier: {identifier}");
}
else
{
    Console.WriteLine("Packet identifier not found.");
}

// Пример получения packetId из PacketIdentifier
PacketIdentifier packetIdentifier = ServerPlayPacket.KeepAlive;
if (PacketIdHelper.TryGetPacketId(packetIdentifier, protocolVersion, out int foundPacketId))
{
    Console.WriteLine($"Found packet ID: {foundPacketId}");
}
else
{
    Console.WriteLine("Packet ID not found.");
}
```

## Как получить пакеты?

Все классы отвечающие за пакеты хранятся в пространстве имён `McProtoNet.Protocol.Packets.*`.
Серверные пакеты режима конфигурации хранятся в `McProtoNet.Protocol.Packets.Configuration.Clientbound`,
клиентские пакеты режима **Play** в `McProtoNet.Protocol.Packets.Play.Serverbound` и так далее.

```txt
McProtoNet
│
└───Protocol
    │
    └───Packets
        │
        ├───Handshaking
        │   └───Serverbound
        │
        ├───Login
        │   ├───Clientbound
        │   └───Serverbound
        │
        ├───Configuration
        │   ├───Clientbound
        │   └───Serverbound
        │
        └───Play
            ├───Clientbound
            └───Serverbound
```

## Как создаются экземпляры серверных пакетов?

Класс `MinecraftClient` читает "сырые" серверные пакеты, которые нужно преобразовать
в конкретный класс. Этого можно добиться с помощью метода `CreateClientboundPacket`
из статического класс `PacketFactory`. Этот метод принимает в качестве аргументов 
версию протокола, идентификатор прочитанного пакета, а также `PacketState`(пакет какого состояния нужно создать).

### Пример кода

```csharp
MinecraftClient client = //...

await foreach(InputPacket packet = client.ReceivePackets())
{
    IServerPacket createdPacket = 
        PacketFactory.CreateClientboundPacket(client.ProtocolVersion, packet.Id, PacketState.Play);
    
    // Deserialize and handling ...
}
```

Хотя Вы можете так делать, но для упрощения разработки рекомендуется использовать
методы расширения `OnPacket<TPacket>`, `OnAllPackets`.

```csharp
await foreach(IServerPacket packet = client.OnAllPackets(PacketState.Play))
{
    Console.WriteLine($"Received packet: {packet.GetPacketId()}");
}
```

