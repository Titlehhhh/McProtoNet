# McProtoNet.Abstractions

Включает такие классы и структуры, как `InputPacket`, `OutputPacket` и другие, предоставляя единообразный подход к обработке данных Minecraft-протокола.

## InputPacket

Структура, предназначенная для абстракции входящих данных пакета.
Она абстрагирует как сжатые, так и несжатые данные, предоставляя удобный интерфейс для работы с пакетами из [протокола Minecraft](https://minecraft.wiki/w/Minecraft_Wiki:Projects/wiki.vg_merge/Protocol#Packet_format).

Исходный код: [InputPacket.cs](https://github.com/Titlehhhh/McProtoNet/blob/dev/src/McProtoNet.Abstractions/InputPacket.cs)

<deflist>
    <def title="Id: int">
        Поле, представляющее собой идентификатор пакета.
    </def>
    <def title="Data: Memory<byte>">
        Сырые данные.
    </def>
</deflist>

<note>
Эта структура наследуется от интерфейса <code>IDisposable</code>,
но клиенту обычно не нужно вызывать <code>Dispose</code>.
</note>

## OutputPacket

Структура, предназначенная для абстракции исходящих пакетов.

Исходный код: [OutputPacket.cs](https://github.com/Titlehhhh/McProtoNet/blob/dev/src/McProtoNet.Abstractions/OutputPacket.cs)

<warning>
Эта структура наследует <code>IDisposable</code>, поэтому отправленные пакеты всегда нужно очищать.
</warning>

### Свойства

<deflist>
<def title="Memory: ReadOnlyMemory<byte>">
Данные
</def>
<def title="Span: ReadOnlySpany<byte>">
Данные как ReadOnlySpan
</def>
</deflist>