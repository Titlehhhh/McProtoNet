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

<code-blocks src="../code-samples/IPacket.cs" lang="C#" collapsed-title="IPacket.cs" collapsible="true"/>

### IServerPacket

Интерфейс `IServerPacket` описывает серверный пакет. Он наследуется от `IPacket` и добавляет метод `Deserialize`.

<code-blocks src="../code-samples/IServerPacket.cs" lang="C#" collapsed-title="IServerPacket.cs" collapsible="true"/>

### IClientPacket

Интерфейс `IClientPacket` описывает клиентский пакет. Он также наследуется от `IPacket` и добавляет метод `Serialize`.

<code-blocks src="../code-samples/IClientPacket.cs" lang="C#" collapsed-title="IClientPacket.cs" collapsible="true"/>
