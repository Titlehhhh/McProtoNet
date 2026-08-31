# Что из этого видно снаружи

Слоёв четыре, но приложение обычно трогает десяток имён. Вот они.

## Клиент

[`MinecraftClient`](../08-api-reference/McProtoNet/MinecraftClient.md) создаётся
с
[`MinecraftClientOptions`](../08-api-reference/McProtoNet/MinecraftClientOptions.md)
(хост, порт, при необходимости прокси), открывает соединение через
`ConnectAsync` и дальше умеет немного:

- `ReadPacketsAsync(token)` - поток входящих пакетов;
- `ReadPacketAsync(token)` - один пакет, если поток не нужен;
- `SendAsync(packet, protocolVersion)` - отправить типизированный пакет;
- `SendRawAsync(id, body)` - отправить готовые байты;
- `CompressionThreshold` - включить сжатие, когда сервер о нём сообщил;
- `EnableEncryption(secret)` - включить шифр начиная со следующего кадра;
- `DisposeAsync` - закрыть соединение и отпустить буферы.

## Прокси

Сокет можно открыть не самому: в `MinecraftClientOptions` кладётся
`IProxyClient`, реализации - в
[QuickProxyNet](https://github.com/Titlehhhh/QuickProxyNet)
([«Вход на сервер»](../04-transport/01-joining-a-server.md)).

## Пакеты

[`IncomingPacket`](../08-api-reference/McProtoNet/Primitives/IncomingPacket.md)
и
[`OutgoingPacket`](../08-api-reference/McProtoNet/Primitives/OutgoingPacket.md)
- общая валюта всех слоёв. У входящего есть номер и тело; тело - окно в буфер,
которое живёт до следующего чтения
([«Буфер приёма»](../04-transport/03-packet-stream.md)).

Каждый сгенерированный пакет знает свой идентификатор и умеет читать и писать
себя для конкретной версии протокола. Реестр
[`PacketRegistry`](../08-api-reference/McProtoNet/Protocol/PacketRegistry.md)
переводит номер в описание пакета, если это нужно вручную.

## Обработчики

[`ClientboundHandler`](../08-api-reference/McProtoNet/Protocol/ClientboundHandler.md)
(и
[`ServerboundHandler`](../08-api-reference/McProtoNet/Protocol/ServerboundHandler.md)
для серверного направления) - база с методом на каждый пакет. Приложение
наследуется, переопределяет нужное и само переставляет `Phase`: фазы ведёт код
приложения ([«Фаза и направление»](../05-packets/01-phases-and-direction.md)).
Номер, которого реестр не знает для этой версии, фазы и направления, приходит в
`OnUnknown`.

## Мимо клиента

Иногда сокет свой, а разобрать пакеты хочется. Тогда берут
[`PacketStreamReader`](../08-api-reference/McProtoNet/Transport/Framing/PacketStreamReader.md)
и
[`PacketStreamWriter`](../08-api-reference/McProtoNet/Transport/Framing/PacketStreamWriter.md):
они читают и пишут по одному пакету поверх обычного `Stream`, без
[`MinecraftConnection`](../08-api-reference/McProtoNet/Transport/MinecraftConnection.md).

## Вокруг

[`SrvResolver`](../08-api-reference/McProtoNet/SrvResolver.md) находит настоящий
адрес сервера по SRV-записи домена.
[`LanServerDetector`](../08-api-reference/McProtoNet/LanServerDetector.md)
слушает широковещательные объявления и отдаёт серверы, открытые в локальной
сети.
