# MinecraftPrimitiveWriter

[ref-структура](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/ref-struct), предназначенная для записи примитивных 
типов данных в бинарный поток согласно формату Minecraft-протокола.
Она реализована на основе `Span<byte>`, 
что обеспечивает высокую производительность за счёт работы напрямую с памятью, 
исключая лишние копирования.

Исходный код: [MinecraftPrimitiveWriter.cs](https://github.com/Titlehhhh/McProtoNet/blob/dev/src/McProtoNet.Serialization/MinecraftPrimitiveWriter.cs)

## Основные методы

<deflist>
<def title="void WriteBoolean(bool value)">
Writes a single byte representing a boolean value (1 for true, 0 for false).
</def>
<def title="void WriteSignedByte(sbyte value)">
Writes a single signed byte to the buffer.
</def>
<def title="void WriteUnsignedByte(byte value)">
Writes a single unsigned byte to the buffer.
</def>
<def title="void WriteUnsignedShort(ushort value)">
Writes two bytes in BigEndian format representing an unsigned short integer.
</def>
<def title="void WriteSignedShort(short value)">
Writes two bytes in BigEndian format representing a signed short integer.
</def>
<def title="void WriteSignedInt(int value)">
Writes four bytes in BigEndian format representing a signed integer.
</def>
<def title="void WriteUnsignedInt(uint value)">
Writes four bytes in BigEndian format representing an unsigned integer.
</def>
<def title="void WriteSignedLong(long value)">
Writes eight bytes in BigEndian format representing a signed long integer.
</def>
<def title="void WriteUnsignedLong(ulong value)">
Writes eight bytes in BigEndian format representing an unsigned long integer.
</def>
<def title="void WriteFloat(float value)">
Writes four bytes in BigEndian format representing a floating-point number.
</def>
<def title="void WriteDouble(double value)">
Writes eight bytes in BigEndian format representing a double-precision floating-point number.
</def>
<def title="void WriteUUID(Guid value)">
Writes 16 bytes representing a UUID/GUID value.
</def>
<def title="void WriteBuffer(ReadOnlySpan<byte> value)">
Writes a raw byte buffer to the stream.
</def>
<def title="void WriteVarInt(int? value)">
Writes a nullable integer in VarInt format. Throws ArgumentNullException if the value is null.
</def>
<def title="void WriteVarInt(int value)">
Writes an integer in VarInt format (1-5 bytes depending on the value magnitude).
</def>
<def title="void WriteVarLong(long value)">
Writes a long integer in VarLong format (1-10 bytes depending on the value magnitude).
</def>
<def title="void WriteString(string value)">
Writes a string in UTF-8 format, prefixed with its length as a VarInt.
</def>
<def title="void WriteOptionalNbt(NbtTag? value)">
Writes an optional NBT tag. Writes a boolean indicating presence, followed by the tag data if present.
</def>
<def title="void WriteNbt(NbtTag value)">
Writes an NBT tag to the buffer.
</def>
<def title="MemoryOwner<byte> GetWrittenMemory()">
Returns the written memory buffer and marks the writer as disposed.
</def>
<def title="void Dispose()">
Disposes of the writer and releases any allocated resources.
</def>
</deflist>


## Примеры использования

<code-block src="../code-samples/PrimitiveWriterSample.cs" lang="C#"/>

