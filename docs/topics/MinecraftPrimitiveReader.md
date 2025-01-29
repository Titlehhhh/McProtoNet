# MinecraftPrimitiveReader

Исходный код: [MinecraftPrimitiveReader.cs](https://github.com/Titlehhhh/McProtoNet/blob/dev/src/McProtoNet.Serialization/MinecraftPrimitiveReader.cs)

[ref-структура](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/ref-struct), предназначенная для чтения примитивных типов данных 
из бинарного потока в формате Minecraft-протокола. Она представляет собой
высокопроизводительную оболочку над Span&lt;byte&gt;,
что позволяет работать с данными напрямую из памяти без лишнего копирования.

## Основные методы

<deflist>
<def title="void Advance(int count)">
Перемещает внутренний указатель чтения на указанное количество байт.
</def>
<def title="ReadOnlySpan<byte> Read(int count)">
Считывает указанное количество байт и возвращает их в виде <code>ReadOnlySpan&lt;byte&gt;</code>.
</def>
<def title="int Read(Span<byte> output)">
Считывает данные и записывает их в предоставленный <code>Span&lt;byte&gt;</code>. Возвращает количество фактически прочитанных байт.
</def>
<def title="int ReadVarInt()">
Считывает целое число в формате <code>VarInt</code> из потока. Если число превышает допустимую длину, выбрасывает исключение <code>ArithmeticException</code>.
</def>
<def title="long ReadVarLong()">
Считывает целое число в формате <code>VarLong</code> из потока. Если число превышает допустимую длину, выбрасывает исключение <code>ArithmeticException</code>.
</def>
<def title="bool ReadBoolean()">
Считывает один байт и интерпретирует его как булево значение (<code>true</code> для 1, <code>false</code> для 0).
</def>
<def title="byte ReadUnsignedByte()">
Считывает один байт как беззнаковое целое число.
</def>
<def title="sbyte ReadSignedByte()">
Считывает один байт как знаковое целое число.
</def>
<def title="ushort ReadUnsignedShort()">
Считывает два байта в формате BigEndian и интерпретирует их как беззнаковое короткое целое число.
</def>
<def title="short ReadSignedShort()">
Считывает два байта в формате BigEndian и интерпретирует их как знаковое короткое целое число.
</def>
<def title="int ReadSignedInt()">
Считывает четыре байта в формате BigEndian и интерпретирует их как знаковое целое число.
</def>
<def title="uint ReadUnsignedInt()">
Считывает четыре байта в формате BigEndian и интерпретирует их как беззнаковое целое число.
</def>
<def title="long ReadSignedLong()">
Считывает восемь байт в формате BigEndian и интерпретирует их как знаковое длинное целое число.
</def>
<def title="ulong ReadUnsignedLong()">
Считывает восемь байт в формате BigEndian и интерпретирует их как беззнаковое длинное целое число.
</def>
<def title="float ReadFloat()">
Считывает четыре байта в формате BigEndian и интерпретирует их как значение типа <code>float</code>.
</def>
<def title="double ReadDouble()">
Считывает восемь байт в формате BigEndian и интерпретирует их как значение типа <code>double</code>.
</def>
<def title="string ReadString()">
Считывает строку в формате UTF-8. Сначала считывает длину строки как <code>VarInt</code>, затем соответствующее количество байт.
</def>
<def title="Guid ReadUUID()">
Считывает 16 байт и интерпретирует их как UUID.
</def>
<def title="byte[] ReadRestBuffer()">
Считывает все оставшиеся данные в буфере и возвращает их в виде массива байтов.
</def>
<def title="byte[] ReadBuffer(int length)">
Считывает указанное количество байт и возвращает их в виде массива байтов.
</def>
<def title="NbtTag? ReadOptionalNbtTag(bool readRootTag)">
Считывает опциональный тег NBT. Возвращает <code>null</code>, если тег отсутствует, или объект <code>NbtTag</code>, если он есть.
</def>
<def title="NbtTag ReadNbtTag(bool readRootTag)">
Считывает тег NBT из потока. Поддерживает чтение как корневого, так и вложенного тега.
</def>
</deflist>

## Пример использования

<code-block lang="C#" src="../code-samples/PrimitiveReaderSample.cs"/>