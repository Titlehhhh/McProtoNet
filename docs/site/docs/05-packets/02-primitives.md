# Чтение и запись примитивов

Тело пакета - это просто байты. Протокол поверх них определяет свои типы
(полный список - на странице
[Data types](https://minecraft.wiki/w/Java_Edition_protocol/Data_types)):
VarInt и VarLong переменной длины, строки с длиной-VarInt перед байтами,
16-байтный UUID, NBT-теги произвольной вложенности. Кодировать и
декодировать их одинаково при чтении и при записи, без лишних аллокаций -
задача отдельного слоя: `MinecraftPrimitiveReader` и
`MinecraftPrimitiveWriter` из `McProtoNet.Primitives`.

## Что читается и пишется

У читателя и писателя симметричные наборы методов: `ReadVarInt`/`WriteVarInt`
и `ReadVarLong`/`WriteVarLong` для переменной длины; `ReadBoolean` - один
байт; знаковые и беззнаковые byte, short, int, long, float и double идут
big-endian, кроме VarInt и VarLong. `ReadString`/`WriteString` кодируют
строку в UTF-8 с длиной в байтах впереди, тем же VarInt, и ограничивают её
длину параметром `maxLength` (по умолчанию `short.MaxValue`).
`ReadUUID`/`WriteUUID` - 16 байт big-endian поверх `Guid`. `ReadNbtTag` и
`WriteNbt`, а также их варианты с байтом-флагом присутствия
`ReadOptionalNbtTag`/`WriteOptionalNbt`, работают с NBT-деревом.
`ReadBuffer`/`ReadRestBuffer`/`WriteBuffer` копируют голые байты без
префикса длины - когда длина известна извне.

`ReadVarInt` показывает форму типичной сигнатуры и то, куда уходит ошибка
при нехватке данных:

```csharp
public int ReadVarInt()
{
    if (!_reader.TryReadVarInt(out int res, out _))
    {
        ThrowHelper.ThrowNotEnoughData();
    }

    return res;
}
```

## `Span<byte>` без лишних копий

`MinecraftPrimitiveReader` - `ref struct` над `SequenceReader<byte>`.
Конструктор оборачивает переданные `ReadOnlyMemory<byte>` или
`ReadOnlySequence<byte>`, ничего не копируя:

```csharp
public ref struct MinecraftPrimitiveReader
{
    private SequenceReader<byte> _reader;

    public MinecraftPrimitiveReader(ReadOnlyMemory<byte> data)
        : this(new ReadOnlySequence<byte>(data))
    {
    }
}
```

Типичный источник этой памяти - `IncomingPacket.Body`: тело пакета - окно
в буфер, которое живёт до следующего чтения; разбирать его нужно сразу,
не через `await`
([«Буфер приёма»](../04-transport/03-packet-stream.md)).
`Read(Span<byte> output)` копирует байты прямо в буфер вызывающего кода
и ничего не выделяет сам.

У `MinecraftPrimitiveWriter` та же экономия в обратную сторону: он держит
`ArrayBufferWriter<byte>`, а `WrittenSpan` и `WrittenMemory` - окна в этот
буфер, которые следующая запись делает невалидными: буфер может переехать
при росте, а старое окно об этом не узнает.

## Кто владеет памятью

`MemoryOwner<T>` - структура поверх массива, арендованного у
`ArrayPool<T>.Shared`. `Allocate` берёт массив нужной длины, `Dispose`
возвращает его в пул:

```csharp
public static MemoryOwner<T> Allocate(int length)
{
    if (length == 0) return default;
    var array = ArrayPool<T>.Shared.Rent(length);
    return new MemoryOwner<T>(array, length);
}

public void Dispose()
{
    var arr = _array;
    if (arr is not null)
    {
        _array = null;
        ArrayPool<T>.Shared.Return(arr);
    }
}
```

`MemoryOwner<T>` - мутируемая структура: копия оправдана только когда она
передаёт владение дальше, иначе оба держателя вернут в пул один массив.

Писатель отдаёт готовые байты через `GetWrittenMemory`, и это уже копия -
не окно в собственный буфер писателя:

```csharp
public MemoryOwner<byte> GetWrittenMemory()
{
    var written = _writer.WrittenSpan;
    var owner = MemoryOwner<byte>.Allocate(written.Length);
    written.CopyTo(owner.Span);
    return owner;
}
```

Копия здесь не лишняя: буфер писателя переиспользуется через
`MinecraftPrimitiveWriterCache` (`Rent`/`Return`, по писателю на поток,
писатели крупнее 64 килобайт отбрасываются), и после `Return` его трогать
нельзя. `OutgoingPacket` берёт готовый `MemoryOwner<byte>` и должен быть
освобождён ровно один раз, чтобы буфер вернулся в пул.

## Ошибки при чтении битых данных

Нехватка данных - самая частая ошибка чтения, и она везде одна:
`InvalidDataException` через внутренний `ThrowHelper.ThrowNotEnoughData`.
VarInt длиннее 5 байт и VarLong длиннее 10 байт - тоже
`InvalidDataException`, с отдельным сообщением про длину. У строк проверок
больше: отрицательная длина-префикс, число байт больше `maxLength * 3`,
итоговая длина строки больше `maxLength` - каждая через
`ThrowHelper.ThrowInvalidData` со своим текстом. Битый NBT всплывает как
`NbtFormatException` изнутри `ReadNbtTag`. У методов чтения VarInt прямо
из `Stream` (`Stream.ReadVarInt`, `ReadVarIntAsync`) - расширений над
потоком, не самого читателя - обрыв данных даёт `EndOfStreamException`.

## Когда это трогает приложение

Обычно напрямую - нет: у сгенерированных пакетов свои `Read`/`Write` на
каждую версию протокола, и они уже написаны поверх этого слоя. Прямой
доступ к `MinecraftPrimitiveReader`/`Writer` нужен в двух случаях: когда
приложение реализует собственный пакет со своими `Read`/`Write`, и когда
тело разбирается вручную - тип пакета заранее не известен или нужны
только первые несколько полей, без декодирования пакета целиком.

## Дальше

- [Из сырого пакета: номер, имя, экземпляр](03-from-raw-packet.md) - где
  тело пакета становится типизированным объектом
- [NBT](../06-nbt/01-nbt.md) - формат тегов, с которым работает этот же
  слой
- [Кадры: где кончается пакет](../04-transport/02-framing.md) - откуда
  берётся тело, которое видит `MinecraftPrimitiveReader`
