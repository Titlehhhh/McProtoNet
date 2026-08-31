# Обработчики и неизвестные пакеты

На предыдущей странице обработчик назван третьим путём к типизированному пакету
- асинхронным, через
[`ClientboundHandler`](../08-api-reference/McProtoNet/Protocol/ClientboundHandler.md)
и
[`ServerboundHandler`](../08-api-reference/McProtoNet/Protocol/ServerboundHandler.md).
Это не альтернатива посетителю на отдельный пакет, а инструмент для всего
направленного потока: прожить им сессию целиком, от login до конца, без ручного
`switch` по номерам.

## Устройство

`HandleAsync` - единственный публичный метод обработчика. Он сам находит номер
пакета в реестре, разбирает тело и вызывает нужный виртуальный метод `On<Имя>`.
Фаза читается один раз, в начале вызова: обработчик, который сам её меняет,
делает это уже после диспетчеризации, а текущий пакет разбирается той фазой, в
которой пришёл.

```csharp
public ValueTask HandleAsync(in IncomingPacket raw, int protocolVersion)
{
    var phase = Phase;
    if (!PacketRegistry.TryGetOrdinal(raw.Id, protocolVersion, phase,
            PacketDirection.Clientbound, out var ordinal))
        return OnUnknown(in raw);
    var reader = new MinecraftPrimitiveReader(raw.Body);
```

Дальше - вложенный `switch`: сперва по фазе, потом по `ordinal` пакета внутри
неё, без рефлексии и без бокса. В каждой ветке одна и та же последовательность:
типизированное чтение тела и вызов `On<Имя>` с готовым пакетом.

## Почему методов много, а переопределяют единицы

На каждый пакет фазы и направления обработчик несёт свой виртуальный метод - в
`ClientboundHandler` их 143, плюс `OnUnknown` на всё остальное. По умолчанию
каждый ничего не делает:

```csharp
protected virtual ValueTask OnLoginCompress(
    Packets.Login.Clientbound.LoginCompressPacket packet) => default;
protected virtual ValueTask OnEncryptionRequest(
    Packets.Login.Clientbound.EncryptionRequestPacket packet) => default;
```

Код приложения переопределяет только то, что ему нужно, остальные пакеты
проходят через no-op без единой строчки кода на своей стороне. `MinimalBot`
наследует `ClientboundHandler` и переопределяет 16 пакетных методов из 143 плюс
`OnUnknown` - login, configuration, keep-alive, телепорт, здоровье.

## Кто выставляет Phase

У `Phase` открытый `get` и `set` только для наследника - это решение владельца:
фазы ведёт код приложения ([«Фаза и направление»](01-phases-and-direction.md)).
Обработчик стартует в фазе, с которой начинается любое соединение: `login` для
`ClientboundHandler`, `handshaking` для `ServerboundHandler`, и дальше сам
двигает `Phase` в ответ на пакеты перехода. Пример перехода и последствия
забытого `Phase` - там же.

## Неизвестный пакет

`OnUnknown` вызывается, когда номер пакета не находится в реестре для текущей
пары фазы и направления. Это не сбой и не повод для исключения - разбирать нужно
не всё подряд, а то, что важно коду приложения, остальное законно остаётся
неизвестным. По умолчанию `OnUnknown` тоже no-op. `MinimalBot` переопределяет
его, чтобы залогировать каждый новый пропущенный пакет фазы один раз, а не на
каждый экземпляр:

```csharp
protected override ValueTask OnUnknown(in IncomingPacket raw)
{
    if (_unknownSeen.Add((Phase, raw.Id)))
    {
        var packetName = PacketRegistry.TryResolve(raw.Id, pv, Phase,
            Direction, out var desc) ? desc.Identity.Name : $"0x{raw.Id:X2}";
        Console.WriteLine($"[{Phase}] пропущен {packetName} " +
            $"({raw.Body.Length} байт)");
    }
    return default;
}
```

`raw` живёт только на время вызова: тело пакета - окно в буфер, которое живёт до
следующего чтения ([«Буфер приёма»](../04-transport/03-packet-stream.md)) - если
байты тела нужны дальше, их нужно скопировать здесь же.

## Лишние байты в конце

Другая ситуация: номер найден, тело разобрано методом `Read`, но после чтения в
буфере остались байты. Обработчик не бросает исключение и не останавливает вызов
`On<Имя>` - тот уже запущен. Вместо этого он поднимает статическое событие
`PacketFlow.OnTrailingBytes`:

```csharp
if (reader.RemainingCount != 0)
    PacketFlow.RaiseTrailingBytes(raw.Id, protocolVersion,
        reader.RemainingCount);
return pending;
```

Подписка на `OnTrailingBytes` (делегат
[`TrailingBytesHook`](../08-api-reference/McProtoNet/Protocol/TrailingBytesHook.md))
- дело кода приложения, и один раз на процесс: событие статическое, общее для
всех обработчиков. Это самостоятельный канал отчёта о подозрительной
спецификации, а не `DecodeError.TrailingBytes`, который возвращают
`PacketIo.TryDecode` и `PacketFlow.TryDispatch`. Там разбор идёт через явный
вызов, и подозрение можно вернуть значением; здесь разбор скрыт внутри
`HandleAsync`, и единственный канал наружу - событие.

## Обработчик или посетитель

`IPacketVisitor.Visit<T>` возвращает `void` - отдать туда `ValueTask`
асинхронного `On<Имя>` некуда, продолжение потеряется молча. Поэтому обработчик
не реализует
[`IPacketVisitor`](../08-api-reference/McProtoNet/Protocol/IPacketVisitor.md) и
не идёт через `PacketFlow.Dispatch`: он делает то же самое - номер, `ordinal`,
чтение, вызов, - но в одном `case`-блоке, без посетителя.

Посетитель нужен там, где обработка синхронна и не завязана на наследование
одного класса.
[`PacketSubscriptions`](../08-api-reference/McProtoNet/Protocol/PacketSubscriptions.md)
- публичная реализация `IPacketVisitor` поверх словаря делегатов: метод
`On<T>(PacketHandler<T> handler)` регистрирует обработчик под тип пакета,
`Visit<T>` находит его по `Identity.Ordinal` и вызывает, а если для типа ничего
не зарегистрировано - молча пропускает. Она годится для набора независимо
собранных подписок с синхронной обработкой. Обработчик - когда на всё соединение
один объект, а часть пакетов требует `await`, как в `MinimalBot`.

## Дальше

- [Из сырого пакета](03-from-raw-packet.md) - три ступени разбора, куда
  обработчик встроен третьим путём
- [Фазы протокола](01-phases-and-direction.md) - откуда берутся фаза и
  направление, и кто их меняет
- [Исключения](06-exceptions.md) - что долетает до кода приложения как
  исключение, а что нет
