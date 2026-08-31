# Одна сборка - много версий

На странице «Об проекте» об этом сказано в двух абзацах: один диапазон
протоколов, одна сборка вместо пересборки под каждую игру. Здесь - как
это устроено в пакетном слое: какие атрибуты видны на сгенерированном
пакете, откуда берётся раскладка полей и что происходит, когда просят
версию, которой в коде попросту нет.

## Атрибуты, из которых собран пакет

Каждый пакет несёт три атрибута: `ProtocolSupport` - диапазон версий,
на котором пакет существует; `Packet` - манифестный ключ, фаза
и направление; `PacketField` - поле, а если оно не на всём диапазоне,
то ещё `Group` и границы `From`/`To`.

```csharp
[ProtocolSupport(
    MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
[Packet("play.toClient.open_window",
    PacketPhase.Play, PacketDirection.Clientbound)]
[PacketField("WindowId", "int")]
[PacketField("InventoryType", "int")]
[PacketField("WindowTitleJson", "string", Group = "VUntil764", To = 764)]
[PacketField("WindowTitle", "NbtTag", Group = "V765_Last", From = 765)]
```

`WindowId` и `InventoryType` без `Group` - общие поля, есть в любой
версии из диапазона. `WindowTitleJson` и `WindowTitle` - одно и то же
поле по смыслу, заголовок открываемого окна, но на старых версиях это
строка JSON, а на новых - тег NBT: разные типы, ни одна не входит
в общую часть.

Эти атрибуты читает `VersionRangeGenerator`, единственный генератор
Roslyn в проекте: по `ProtocolSupport` он достраивает
`IsSupportedVersion(int)` и хелпер на бросок исключения. Раскладку
полей сам не строит - она уже собрана в коде пакета.

## Слой вместо отдельного пакета

Библиотека не заводит по классу на версию. У пакета один класс на весь
диапазон, а версии, расходящиеся полями, оседают во вложенных
структурах - слоях:

```csharp
public sealed partial record OpenWindowPacket(
    int WindowId, int InventoryType,
    OpenWindowPacket.VUntil764Layer? VUntil764 = null,
    OpenWindowPacket.V765_LastLayer? V765_Last = null)
    : IPacket<OpenWindowPacket>, IPacket
{
    public readonly record struct VUntil764Layer(string WindowTitleJson);
    public readonly record struct V765_LastLayer(NbtTag WindowTitle);
```

Имя слоя следует диапазону: `VUntil764` - от начала до версии
включительно, `V765_Last` - от неё до конца, `V759_765` (в других
пакетах) - отрезок между двумя версиями. Общие поля лежат в записи
пакета, версийные - в своём слое, и заполнен всегда один слой.

## Разбор идёт по номеру версии, а не по классу

`Read` и `Write` ветвятся по `protocolVersion` и строят или читают
ровно тот слой, что подходит. Вот ветка для версий, где поля
`WindowTitleJson` уже нет и заголовок приходит тегом NBT:

```csharp
if (protocolVersion >= 765)
{
    var windowId = reader.ReadVarInt();
    var inventoryType = reader.ReadVarInt();
    var windowTitle = reader.ReadNbtTag(false)!;
    return new OpenWindowPacket(windowId, inventoryType,
        V765_Last: new V765_LastLayer(windowTitle));
}
```

Ветка для старых версий устроена зеркально: те же `WindowId`
и `InventoryType`, но вместо `ReadNbtTag` - `ReadString`, а слой
собирается как `VUntil764`. Код приложения этой ветки не видит: он
получает готовый `OpenWindowPacket` и смотрит, какой слой заполнен.
Запись чужого слоя не в ту версию `Write` пресекает
`WrongLayerException` - защита от ручной ошибки, не обычный путь.

## Номер пакета меняется чаще, чем сам пакет

Заголовок окна поменялся один раз за весь диапазон, а номер пакета
`open_window` прыгает почти на каждой версии - разные вещи. Номер
знает не библиотека целиком, а конкретный тип, через собственный
`TryGetPacketId`:

```csharp
if (protocolVersion >= 762 && protocolVersion <= 763)
{
    id = 0x30;
    return true;
}

if (protocolVersion >= 764 && protocolVersion <= 765)
{
    id = 0x31;
    return true;
}
```

Таких диапазонов в одном `OpenWindowPacket` больше десятка. Сводку по
всем пакетам держит `PacketRegistry.g.cs` (`IdRange` на диапазон,
`PacketDescriptor` на пакет), а поверх - плоские таблицы
номер-в-ordinal по фазе, направлению и версии; через них код ищет тип
пакета по чужому номеру - устройство адреса на странице
«Фаза и направление».

## Если версии у пакета нет

Отказов два. Версия вне диапазона `ProtocolSupport` целиком - `Read`,
`Write` и типизированная отправка бросают `ProtocolNotSupportException`
с версией и списком поддержанных диапазонов. Версия внутри диапазона,
но без своей ветки в `Read`/`Write` - пробел в раскладке полей, а не
в поддержке версии, - и оба метода бросают `NotSupportedException`
с текстом «has no wire layout for protocol version». А когда сам номер
пакета не находится в чужой версии, до этих исключений дело не
доходит: `PacketRegistry.TryResolve` возвращает отказ, пакет уходит
в `Unknown` - см. «Фаза и направление».

## Что менять в коде приложения при переезде

Обычно - только число: версию протокола, которую код передаёт при
подключении. Диспетчеризация, номера пакетов, выбор слоя
пересчитываются по новому числу сами, без правок в остальном коде.

Правки нужны там, где код приложения трогает версийный слой напрямую -
читает `packet.VUntil764` вместо общих полей. На новой версии свойство
станет `null`, а нужное поле переедет в `V765_Last`, не всегда того же
типа: строка JSON и тег NBT друг в друга не конвертируются. Код,
читающий только общие поля пакета, версийных слоёв не видит вовсе.

## Чего от мультиверсии ждать нельзя

Диапазон - `%min_minecraft_version%` - `%max_minecraft_version%`,
и это диапазон конкретной сборки, а не вся история протокола: версия
за его границами получает `ProtocolNotSupportException`, а не
приблизительный разбор. Мультиверсия здесь - про форму пакета на
проводе, не про игровые данные: числовые идентификаторы блоков
и предметов между версиями библиотекой не выравниваются. И раскладка
по слоям версию не прячет - код, которому нужны версийные поля,
обязан знать, какой слой заполнен.

## Дальше

- [«Из сырого пакета в объект»](03-from-raw-packet.md)
- [«Фаза и направление»](01-phases-and-direction.md)
- [«От версии игры к номеру протокола»](../07-reference/01-version-to-protocol.md)
