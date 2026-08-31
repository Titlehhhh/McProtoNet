# Словарь

Термины, которые уже встречаются в документации. У каждого - краткое значение и
ссылка на страницу, где он разобран подробно.

- **версия протокола** - число, которое клиент присылает в рукопожатии
  (например, 772). Задаёт, какая раскладка полей пакетов и какие номера пакетов
  действуют для сессии. Подробнее -
  [«Из сырого пакета»](../05-packets/03-from-raw-packet.md).

- **гейт отправки** - `SemaphoreSlim(1, 1)` внутри
  [`MinecraftClient`](../08-api-reference/McProtoNet/MinecraftClient.md), через
  который проходит каждый `SendAsync` и `SendRawAsync`. Кадры разных вызовов не
  перемешиваются на одном сокете. Подробнее -
  [«Поток пакетов»](../04-transport/03-packet-stream.md).

- **кадр** - служебная обвязка вокруг пакета: длина, при сжатии - ещё один
  varint, затем тело. Делает видимой границу между пакетами в непрерывном потоке
  TCP. Подробнее - [«Кадры»](../04-transport/02-framing.md).

- **каталог** - список пакетов одной пары (фаза, направление), который отдаёт
  `PacketRegistry.Catalog(phase, dir)`. Подробнее -
  [«Фаза и направление»](../05-packets/01-phases-and-direction.md).

- **чтение пачками** - путь транспорта, на котором кадры читаются и пишутся
  пачками, а не по одному:
  [`StreamingConnection`](../08-api-reference/McProtoNet/Transport/StreamingConnection.md)
  поверх общего буфера. Подробнее -
  [«Соединение без клиента»](../04-transport/04-raw-connection.md).

- **мультиверсия** - способность одной сборки работать со всеми поддержанными
  версиями протокола: раскладки полей и номера пакетов хранятся внутри самого
  пакета, нужную выбирает номер протокола. Подробнее -
  [«Одна сборка - много версий»](../05-packets/05-multiversion.md).

- **направление** -
  [`PacketDirection`](../08-api-reference/McProtoNet/Protocol/PacketDirection.md):
  `Clientbound` для пакетов от сервера клиенту, `Serverbound` - в обратную
  сторону. У каждой фазы свой набор пакетов на каждое направление. Подробнее -
  [«Фаза и направление»](../05-packets/01-phases-and-direction.md).

- **номер пакета** - поле `Id` у
  [`IncomingPacket`](../08-api-reference/McProtoNet/Primitives/IncomingPacket.md),
  число на проводе. Само по себе ничего не значит: тип пакета ищется по номеру
  вместе с фазой, направлением и версией протокола. Подробнее -
  [«Из сырого пакета»](../05-packets/03-from-raw-packet.md).

- **обработчик** -
  [`ClientboundHandler`](../08-api-reference/McProtoNet/Protocol/ClientboundHandler.md)
  и
  [`ServerboundHandler`](../08-api-reference/McProtoNet/Protocol/ServerboundHandler.md):
  базовый класс с методом `On<Имя>` на каждый пакет. Код приложения наследуется
  и переопределяет только нужное. Подробнее -
  [«Первый бот»](../02-getting-started/02-first-bot.md).

- **общий секрет** - 16 байт, которыми стороны обмениваются через
  [`EncryptionRequestPacket`](../08-api-reference/McProtoNet/Protocol/Packets/Login/Clientbound/EncryptionRequestPacket.md)
  и
  [`EncryptionResponsePacket`](../08-api-reference/McProtoNet/Protocol/Packets/Login/Serverbound/EncryptionResponsePacket.md).
  Служит и ключом AES-128, и вектором инициализации шифра. Подробнее -
  [«Сжатие и шифрование»](../04-transport/05-encryption-and-compression.md).

- **окно в буфер** - тело пакета не копия байт, а участок памяти арендованного
  буфера. Живёт только до следующего чтения, поэтому разбирать его нужно сразу,
  не тащить через `await`. Подробнее -
  [«Поток пакетов»](../04-transport/03-packet-stream.md).

- **ordinal** - плотный номер пакета внутри своего каталога, часть
  [`PacketIdentity`](../08-api-reference/McProtoNet/Protocol/PacketIdentity.md).
  В отличие от номера пакета на проводе, ordinal стабилен между сборками и
  версиями протокола. Подробнее -
  [«Фаза и направление»](../05-packets/01-phases-and-direction.md).

- **пакет** - `IncomingPacket` на входе,
  [`OutgoingPacket`](../08-api-reference/McProtoNet/Primitives/OutgoingPacket.md)
  на выходе: общая валюта всех слоёв библиотеки, номер и тело. Подробнее -
  [«Что видно снаружи»](../03-architecture/03-public-surface.md).

- **порог сжатия** - `CompressionThreshold` у `MinecraftClient`. Пакет короче
  порога уходит как есть, не короче - сжимается libdeflate. Подробнее -
  [«Сжатие и шифрование»](../04-transport/05-encryption-and-compression.md).

- **посетитель** -
  [`IPacketVisitor`](../08-api-reference/McProtoNet/Protocol/IPacketVisitor.md)
  с методами `Visit<T>` и `Unknown`, синхронный способ разобрать пакет через
  `PacketFlow.Dispatch`. Для асинхронных методов не годится, поэтому обработчик
  сделан отдельно. Подробнее -
  [«Кто кого знает»](../03-architecture/02-who-knows-whom.md).

- **реестр пакетов** -
  [`PacketRegistry`](../08-api-reference/McProtoNet/Protocol/PacketRegistry.md).
  Переводит номер пакета вместе с фазой, направлением и версией протокола в
  описание пакета или сам типизированный объект. Подробнее -
  [«Из сырого пакета»](../05-packets/03-from-raw-packet.md).

- **сырой пакет** - то, что отдаёт транспорт: номер и кусок байтов, без знания о
  том, что это за пакет и какие у него поля. Подробнее -
  [«Четыре слоя на одном экране»](../03-architecture/01-layers.md).

- **фаза** -
  [`PacketPhase`](../08-api-reference/McProtoNet/Protocol/PacketPhase.md).
  Handshaking, status, login, configuration, play - подряд идущие стадии сессии.
  Библиотека фазу сама не выводит, её переставляет код приложения. Подробнее -
  [«Фаза и направление»](../05-packets/01-phases-and-direction.md).

## Дальше

- [Версия → протокол](01-version-to-protocol.md)
- [Четыре слоя на одном экране](../03-architecture/01-layers.md)
