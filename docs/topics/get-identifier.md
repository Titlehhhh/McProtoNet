# Получение идентификатора

Протокол Minecraft часто обновляется, и с ним меняются идентификаторы пакетов. Один и тот же пакет может иметь разные идентификаторы на разных версиях протокола.

Для решения этой проблемы используются автоматически генерируемые словари, где:

- **Ключ** — это комбинация версии протокола и `Order` (уникальный номер пакета для конкретной версии).
- **Значение** — это идентификатор пакета (число), который соответствует этому пакету на конкретной версии протокола.

## Работа с идентификаторами пакетов

В классе `PacketIdHelper` реализованы методы, позволяющие получить идентификатор пакета на основе его параметров и версии протокола:

1. **Метод `TryGetPacketIdentifier`**:
   Этот метод пытается найти идентификатор пакета по его `packetId`, версии протокола, состоянию и направлению (к серверу или клиенту).

2. **Метод `GetPacketIdentifier`**:
   Этот метод использует `TryGetPacketIdentifier`, но в случае, если пакет не найден, выбрасывает исключение `KeyNotFoundException`. Этот метод полезен, когда необходимо обязательно получить идентификатор пакета.

3. **Метод `TryGetPacketId`**:
   Метод `TryGetPacketId` работает в обратном направлении: он получает идентификатор пакета и пытается найти его `packetId` для конкретной версии протокола.

4. **Метод `GetPacketId`**:
   Этот метод использует `TryGetPacketId` для получения `packetId`. Если идентификатор не найден, выбрасывается исключение `KeyNotFoundException`.

## Пример работы с идентификаторами пакетов:

```C#
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