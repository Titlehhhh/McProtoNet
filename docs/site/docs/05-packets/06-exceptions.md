# Исключения

Пакетный слой бросает исключение только тогда, когда байты, которые уже
целиком дошли от транспорта, невозможно превратить в пакет: тело оборвалось
раньше времени, несёт битые данные или пакета с таким именем нет на этой
версии протокола. Разрыв соединения, отмену и закрытие описывает соседний
слой - «[Отмена, ошибки, закрытие](../04-transport/06-cancellation.md)»,
здесь речь только про то, что происходит после того, как `IncomingPacket`
уже получен целиком.

## Не исключение: неизвестный номер

Номер пакета, которого реестр не знает в паре (фаза, направление), -
штатное состояние потока, не ошибка. Такой пакет доходит до
`visitor.Unknown` или `OnUnknown` обработчика и не бросает ничего; подробно
об этом - «[Из сырого пакета](03-from-raw-packet.md)».

## Два входа: бросающий и Try

У `PacketIo` и `PacketFlow` по два симметричных входа. `Decode<T>` и
`Dispatch` бросают исключение на битом теле. `TryDecode<T>` и `TryDispatch`
делают тот же разбор, но вместо исключения возвращают `false` и
`DecodeError` - `None`, `UnsupportedVersion`, `TrailingBytes` или
`Malformed`. `ClientboundHandler.HandleAsync` (и `ServerboundHandler`)
устроен только как бросающий вход: у обработчиков нет парного Try-метода.

```csharp
if (!PacketIo.TryDecode<LoginSuccessPacket>(
    raw, pv, out var packet, out var error))
{
    logger.LogWarning("LoginSuccess: {Error}", error);
    return;
}

HandleLoginSuccess(packet);
```

`PacketDecodeException`, которое бросает бросающий вход, несёт `PacketType`
и `Error` - тот же `DecodeError`, что вернул бы Try-путь, - а исходное
исключение (`InvalidDataException`, `NbtFormatException` и так далее) лежит
в `InnerException`.

## Лишние байты

`PacketIo` строг к лишним байтам: если после чтения `T.Read` в буфере
осталось что-то ещё, `Decode<T>` бросает `PacketDecodeException` с
`DecodeError.TrailingBytes`, а `TryDecode<T>` возвращает `false` с той же
причиной. `PacketFlow.Dispatch` / `TryDispatch` и `HandleAsync` ведут себя
иначе: пакет уже дошёл до посетителя или до `On<Имя>`, и лишний хвост -
не повод рвать разбор. Вместо исключения срабатывает событие
`PacketFlow.OnTrailingBytes` с номером пакета, версией протокола и числом
лишних байт. Лишние байты почти всегда значат, что спецификация пакета для
этой версии протокола описана неверно - это находка для отчёта в спеки, а
не сбой соединения.

## Неподдержанная версия

`ProtocolNotSupportException` бросается, когда пакета с этим именем нет на
версии протокола, для которой ведут чтение, запись или типизированную
отправку. Оно несёт `TypeName`, `ActualVersion` и `SupportedRanges` -
диапазоны версий, где пакет всё-таки есть. В Try-путях это сворачивается
в `DecodeError.UnsupportedVersion`.

## Таблица: что произошло → что бросается

| Что произошло | Бросающий путь | Try-путь |
| --- | --- | --- |
| Тело оборвалось раньше времени или несёт битые данные (`VarInt`, NBT) | `InvalidDataException`, `EndOfStreamException`, `NbtFormatException` | `DecodeError.Malformed` |
| Пакета с этим именем нет на данной версии протокола | `ProtocolNotSupportException` | `DecodeError.UnsupportedVersion` |
| Версия-слоистый пакет пишется без нужного слоя | `WrongLayerException` | `DecodeError.Malformed` |
| `PacketIo.Decode`/`TryDecode`: тело разобралось, остались лишние байты | `PacketDecodeException` (`TrailingBytes`) | `DecodeError.TrailingBytes` |
| `Dispatch`/`TryDispatch`/`HandleAsync`: то же, но пакет уже у посетителя | не бросает, событие `OnTrailingBytes` | не бросает, событие `OnTrailingBytes` |
| Номер неизвестен в (фаза, направление) | `visitor.Unknown` / `OnUnknown`, не ошибка | то же, `true` |

`EndOfStreamException` в этой таблице - не то же самое, что чистый конец
потока из «Отмены, ошибок, закрытия»: там оно значит, что сервер закрыл
соединение, здесь - что тело, уже целиком полученное транспортом,
кончилось раньше, чем ожидал `Read`. Различает их место, откуда исключение
поймано: из цикла `ReadPacketsAsync` - разрыв связи, изнутри разбора тела -
битый пакет.

## Что ловить в цикле бота, что чинить в коде

Разрыв связи (`ConnectionAbortedException`, чистый `EndOfStreamException`
из цикла чтения) - обычное дело в цикле бота, его ловят и решают, что
делать дальше: переподключаться или закончить сеанс. Ошибка разбора
пакета - находка о спецификации или о версии протокола в коде приложения,
и глушить её молча не стоит: `PacketDecodeException`, `WrongLayerException`,
`ProtocolNotSupportException` уместно залогировать и разобраться, а не
подавлять постоянно. Try-путь пригоден, чтобы пропустить один плохой пакет
и продолжить сеанс, не разрывая соединение из-за находки в спецификации.

```csharp
try
{
    await foreach (var raw in client.ReadPacketsAsync(token))
        await handler.HandleAsync(raw, pv);
}
catch (EndOfStreamException)
{
    // сервер закрыл поток: штатный конец сессии
}
catch (ConnectionAbortedException ex)
{
    Log(ex.InnerException); // разрыв на уровне транспорта
}
catch (Exception ex) when (ex is InvalidDataException
    or NbtFormatException or ProtocolNotSupportException)
{
    Log(ex); // битый пакет или неподдержанная версия, не разрыв связи
}
```

## Дальше

- [Из сырого пакета](03-from-raw-packet.md) - `PacketRegistry`, `PacketIo`,
  `PacketFlow` и откуда берётся `DecodeError`
- [Обработчики](04-handlers.md) - `ClientboundHandler` и `OnUnknown`
- [Отмена, ошибки, закрытие](../04-transport/06-cancellation.md) - разрыв
  соединения, `Abort`, `CloseReason`
