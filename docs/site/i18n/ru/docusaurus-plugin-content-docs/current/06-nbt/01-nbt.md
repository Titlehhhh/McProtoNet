# NBT

NBT (Named Binary Tag) - бинарный формат Java Edition для вложенных структур:
числа, строки, массивы, списки и compound-теги без единой заранее заданной
схемы. Формат описан на странице
[NBT format](https://minecraft.wiki/w/NBT_format) на minecraft.wiki. Протокол
использует NBT там, где поле пакета несёт данные произвольной формы - предмет с
компонентами, блок-сущность, данные чанка. Такое поле читается и пишется наравне
с остальными примитивами: `MinecraftPrimitiveReader.ReadNbtTag` и
`MinecraftPrimitiveWriter.WriteNbt` (`McProtoNet.Primitives`) стоят в одном ряду
с чтением `VarInt` или строки, разница в том, что за NBT-полем стоит целый
разборщик из `McProtoNet.NBT`.

## Свой разборщик

Библиотека не берёт готовый NBT-парсер и не строит дерево через
generic-десериализацию с рефлексией - формат завязан на детали Java: свой
порядок байт, своя кодировка строк, структура без внешней схемы. Вместо одной
универсальной реализации в `McProtoNet.NBT` их три, каждая под свою форму входа:
[`NbtSpanReader`](../08-api-reference/McProtoNet/NBT/NbtSpanReader.md) читает
непрерывный `ReadOnlySpan<byte>` целиком уместившегося пакета;
[`NbtSequenceReader`](../08-api-reference/McProtoNet/NBT/NbtSequenceReader.md)
читает `SequenceReader<byte>` поверх `ReadOnlySequence<byte>` пакета, разбитого
на сегменты пайпа;
[`NbtReader`](../08-api-reference/McProtoNet/NBT/NbtReader.md) разбирает
`Stream` и идёт по тегам, не строя дерево целиком. Все три говорят об одном
формате: числа big-endian, строки в modified UTF-8, предел вложенности 512
уровней.

## Типы тегов

[`NbtTagType`](../08-api-reference/McProtoNet/NBT/NbtTagType.md) перечисляет 12
типов данных и `End` - маркер конца compound-а и типовой элемент пустого списка:

```csharp
public enum NbtTagType : byte
{
    End = 0x00,
    Byte = 0x01,
    Short = 0x02,
    Int = 0x03,
    Long = 0x04,
    Float = 0x05,
    Double = 0x06,
    ByteArray = 0x07,
    String = 0x08,
    List = 0x09,
    Compound = 0x0a,
    IntArray = 0x0b,
    LongArray = 0x0c
}
```

У `List` все элементы одного типа и без собственных имён; у `Compound` элементы
именованы и перечисляются до тега `End`.

## Из пакета: дерево тегов

Когда поле пакета - NBT, оно читается сразу в дерево объектов
([`NbtTag`](../08-api-reference/McProtoNet/NBT/NbtTag.md) и потомки:
[`NbtCompound`](../08-api-reference/McProtoNet/NBT/NbtCompound.md),
[`NbtList`](../08-api-reference/McProtoNet/NBT/NbtList.md),
[`NbtByte`](../08-api-reference/McProtoNet/NBT/NbtByte.md)).
`MinecraftPrimitiveReader.ReadNbtTag` выбирает читателя по форме буфера, который
остался непрочитанным у текущего пакета:

```csharp
public NbtTag? ReadNbtTag(bool readRootTag)
{
    var unread = _reader.UnreadSequence;
    if (unread.IsSingleSegment)
    {
        // Fast path: parse straight from the contiguous buffer.
        var spanReader = new NbtSpanReader(unread.FirstSpan);
        NbtTag? result = spanReader.ReadAsTag<NbtTag>(readRootTag);
        _reader.Advance(spanReader.ConsumedCount);
        return result;
    }

    // Multi-segment path: parse straight from the sequence.
    return NbtSequenceReader.ReadTag(ref _reader, readRootTag);
}
```

## Курсорный разборщик

`NbtReader` устроен иначе, чем оба читателя пакетного пути: он идёт по тегам
документа один за другим, как `XmlReader` по узлам XML, и не строит дерево - в
его собственном описании в коде он "forward-only" и "non-cached". После каждого
`ReadToFollowing()` текущий тег виден через свойства, а значение читается
отдельно, только если вызывающему коду оно нужно:

```csharp
public NbtTagType TagType { get; private set; }
public string? TagName { get; private set; }
public int Depth { get; private set; }

public bool ReadToFollowing()
```

`ReadAsTag()` у того же `NbtReader` умеет достроить дерево из текущей точки,
если оно понадобилось целиком - курсор и дерево не исключают друг друга, дерево
просто не строится по умолчанию.

## Строки в modified UTF-8

NBT-строки кодируются не обычным UTF-8, а modified UTF-8 - той же кодировкой,
что `DataOutput.writeUTF` в Java. Отличий два: U+0000 записывается как два байта
`C0 80`, а не как нулевой байт обычного UTF-8; символ вне базовой многоязыковой
плоскости кодируется как две трёхбайтовые последовательности, по одной на
суррогат UTF-16, вместо одной четырёхбайтовой.
[`ModifiedUtf8`](../08-api-reference/McProtoNet/NBT/ModifiedUtf8.md) даёт
`GetByteCount`, `GetBytes`, `GetString`; аллоцирует из них только `GetString`.

## Ограничения

`NbtLimits` задаёт два предела на модуль: `MaxDepth = 512` для вложенности
compound-ов и списков и `MaxStringByteLength = ushort.MaxValue` (65535 байт -
предел самого формата, длина строки хранится в двух байтах). Оба предела
проверяются до аллокации: `NbtSpanReader` и `NbtSequenceReader` сверяют
объявленную длину с тем, сколько байт осталось в буфере, и бросают
[`NbtFormatException`](../08-api-reference/McProtoNet/NBT/NbtFormatException.md)
на отрицательной или завышенной длине.

## Запись

Запись зеркалит чтение.
[`NbtBufferWriter`](../08-api-reference/McProtoNet/NBT/NbtBufferWriter.md) пишет
тег в `IBufferWriter<byte>` и стоит за `MinecraftPrimitiveWriter.WriteNbt` в
пакетном пути; [`NbtWriter`](../08-api-reference/McProtoNet/NBT/NbtWriter.md) -
forward-only писатель поверх `Stream`, аналог `NbtReader` на стороне записи.
`WriteTag` идёт по дереву и пишет тип, имя и payload в том порядке, в котором их
читает `NbtSpanReader`; байт `End` для compound-а дописывается сам.

## Частая ошибка: буфер кончается раньше тега

Тело пакета, из которого читается NBT, - окно в буфер, которое живёт до
следующего чтения; разбирать его нужно сразу, не через `await`, как и остальные
поля пакета ([«Буфер приёма»](../04-transport/03-packet-stream.md)).
`ReadNbtTag` читает тот же буфер и должно быть вызвано в том же синхронном
кадре; `NbtSpanReader` закрепляет это на уровне компилятора - как `ref struct`,
его нельзя сохранить в поле или пронести через `await`. Дерево `NbtTag`, которое
`ReadNbtTag` возвращает, от этого свободно: строки и массивы внутри него уже
скопированы (`ModifiedUtf8.GetString` аллоцирует строку, массивы читаются в свою
память), так что оно переживает границу буфера - ограничение касается только
момента разбора, не результата.

## Дальше

- [Чтение и запись примитивов](../05-packets/02-primitives.md)
- [Из сырого пакета: номер, имя, экземпляр](../05-packets/03-from-raw-packet.md)
