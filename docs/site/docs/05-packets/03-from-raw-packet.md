# Из сырого пакета: номер, имя, экземпляр

Разбор входящего пакета идёт в три ступени: сначала виден только номер,
потом по номеру находится имя пакета, и только затем, если это нужно,
пакет становится типизированным объектом. Каждая следующая ступень дороже
предыдущей и требует больше контекста.

## Номер

`IncomingPacket` - то, что транспорт отдаёт после чтения, до всякого
разбора:

```csharp
public readonly struct IncomingPacket
{
    public readonly int Id;
    public readonly ReadOnlyMemory<byte> Body;
}
```

`Id` - номер пакета на проводе, `Body` - тело без номера. В протоколе это
поле называется Packet ID, формат пакета - на странице
[Packet format](https://minecraft.wiki/w/Java_Edition_protocol/Packets#Without_compression).
`Body` - окно в буфер транспорта, оно живёт только до следующего чтения из
того же читателя; разбирать тело нужно сразу, не через `await`.

Одного `Id` мало. Один и тот же номер в разных фазах и направлениях
означает разные пакеты - `0x00` в login и `0x00` в play не имеют между
собой ничего общего. Больше того, номер одного пакета меняется от версии
протокола к версии. Разобрать пакет по одному номеру нельзя - нужны ещё
фаза, направление и версия протокола (откуда берутся фаза и направление -
[«Фазы протокола»](01-phases-and-direction.md)).

## Имя

`PacketRegistry.TryResolve` переводит номер вместе с фазой, направлением
и версией протокола в `PacketDescriptor`:

```csharp
public static bool TryResolve(int id, int protocolVersion,
    PacketPhase phase, PacketDirection dir,
    [NotNullWhen(true)] out PacketDescriptor? descriptor)
```

`PacketDescriptor` несёт `Identity` - структуру `PacketIdentity` с
человеческим именем `Name` (вроде `Teams`) и ключом манифеста `Key`
(вроде `play.toClient.teams`) - и `Ids`, массив `IdRange`: для каких
версий протокола у пакета какой номер на проводе. Внутри TryResolve
сначала ищет `TryGetOrdinal` - плотный внутренний индекс пакета
в каталоге его (фазы, направления) - и по нему уже берёт нужный
дескриптор из `Catalog`. Наружу этот индекс не нужен, снаружи нужен
только результат.

Этого достаточно, чтобы логировать пакеты по имени, даже когда
декодировать их не нужно:

```csharp
if (PacketRegistry.TryResolve(raw.Id, pv, phase, direction, out var d))
    logger.LogDebug("recv {Name} ({Key})", d.Identity.Name, d.Identity.Key);
else
    logger.LogWarning("recv unmapped id 0x{Id:X2} in {Phase}/{Direction}",
        raw.Id, phase, direction);
```

`PacketRegistry.Catalog(phase, dir)` отдаёт весь список пакетов одной
фазы и направления, `ReadOnlySpan<PacketDescriptor>`. Годится, чтобы
напечатать таблицу пакетов фазы:

```csharp
foreach (var d in PacketRegistry.Catalog(PacketPhase.Play,
    PacketDirection.Clientbound))
    Console.WriteLine($"{d.Identity.Name,-24} {d.Identity.Key}");
```

## Экземпляр

Типизированный объект - самая дорогая ступень, и до неё доходят тремя
разными путями, в зависимости от того, известен ли тип пакета заранее.

Когда тип известен заранее - например, после login ожидается именно
`LoginSuccessPacket`, - берут `PacketIo`:

```csharp
public static bool TryDecode<T>(in IncomingPacket raw, int protocolVersion,
    [NotNullWhen(true)] out T? packet, out DecodeError error)
    where T : class, IPacket<T>
```

`Decode<T>` делает то же самое, но вместо `false` и `error` бросает
`PacketDecodeException`. Обе перегрузки читают тело прямо в `T`, минуя
номер пакета: вызывающий код и так знает, что он ждёт именно этот тип.

Когда тип заранее не известен - идёт произвольный поток пакетов одной
фазы, и его нужно разобрать целиком, - берут `PacketFlow.Dispatch` или
`PacketFlow.TryDispatch` с посетителем:

```csharp
public static void Dispatch<TVisitor>(in IncomingPacket raw,
    int protocolVersion, PacketPhase phase, PacketDirection dir,
    ref TVisitor visitor) where TVisitor : IPacketVisitor
```

`IPacketVisitor` - это `Visit<T>(T packet)`, куда попадает статически
типизированный пакет без бокса, и `Unknown(in IncomingPacket raw)` -
для номера, которого реестр не знает в этой фазе и направлении.
`TryDispatch` - тот же путь диспетчеризации, но вместо исключения на
битом теле отдаёт `false` и `DecodeError`. У `PacketFlow` есть и путь
совсем без посетителя, свой `TryDecode`, который сразу отдаёт `IPacket?`.

Третий путь - асинхронный обработчик, `ClientboundHandler` (или
`ServerboundHandler` для обратного направления): он сам находит номер,
разбирает тело и вызывает нужный виртуальный метод `On<Имя>`. Это самый
частый путь в коде приложения; подробно о нём - на странице
[«Обработчики»](04-handlers.md).

## Ошибки разбора

`DecodeError` - причина, по которой тело не разобралось:

- `UnsupportedVersion` - пакета с таким именем нет на этой версии
  протокола;
- `TrailingBytes` - тело разобралось, но в конце остались лишние байты:
  спецификация пакета для этой версии, видимо, неверна;
- `Malformed` - тело оборвалось раньше времени или несёт битые данные.

Номер, которого реестр не знает в паре (фаза, направление), - это не
ошибка, а штатное состояние потока. Столкнувшись с таким номером,
`Dispatch` вызывает `Unknown` у посетителя и не бросает; `TryDispatch`
и собственный `TryDecode` у `PacketFlow` делают то же самое и вдобавок
возвращают `true`. Настоящая ошибка - только когда номер найден,
а тело не разобралось.

## Дальше

- [Обработчики](04-handlers.md) - асинхронный путь через
  `ClientboundHandler`
- [Фазы протокола](01-phases-and-direction.md) - откуда берутся фаза
  и направление
- [Одна сборка - много версий](05-multiversion.md) - как номер одного
  пакета меняется между версиями протокола
- [Исключения](06-exceptions.md) - когда ошибка разбора долетает
  до кода приложения как исключение
