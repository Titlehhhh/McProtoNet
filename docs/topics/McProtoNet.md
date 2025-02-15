# McProtoNet

В этой библиотеке собраны самые основные классы, которые 
помогают работать с протоколом Minecraft.
Библиотека включает инструменты для чтения и записи пакетов, 
обработки сетевых потоков, а также шифрования и сжатия данных. 

## Работа с пакетами

В пространстве имен `McProtoNet.Net` расположены основные классы, 
которые обеспечивают работу с пакетами. Они читают и записывают
пакеты согласно [протоколу Minecraft](https://minecraft.wiki/w/Minecraft_Wiki:Projects/wiki.vg_merge/Protocol#Packet_format).

1. `MinecraftPacketReader`

Класс `MinecraftPacketReader` предназначен для чтения пакетов из любого потока, в том числе, сетевого.

Для этого есть метод `ReadNextPacketAsync`, который возвращает [](McProtoNet-Abstractions.md#inputpacket).

<code-block lang="C#" src="../code-samples/ReadPacketSample.cs"/>

2. `MinecraftPacketSender`

Этот класс занимается отправкой пакетов. Добиться этого можно с помощью метода `SendPacketAsync`,
который в качестве аргумента принимает `ReadOnlyMemory<byte>`. 

`ReadOnlyMemory<byte>` - это отправленный пакет, у которого первые байты это идентификатор, остальное - данные.

```C#
NetworkStream ns = new NetworkStream(socket);

MinecraftPacketSender sender = new MinecraftPacketSender();
sender.BaseStream = ns;
byte[] data = [0, 3, 4, 5, 6];
await sender.SendPacketAsync(data, cancellationToken: default(CancellationToken));
```

Также имеются методы расширения для [](McProtoNet-Abstractions.md#outputpacket). Это удобно, если
Вы отправляете пакет прямиком из `MinecraftPrimitiveWriter`.

### Сжатие

Оба, вышеупомянутых класса, позволяют включить сжатие.
Это достигается с помощью метода `SwitchCompression`, который принимает
в качестве аргумента число — порог сжатия.

Для повышения производительности при сжатии и 
декомпрессии пакетов используется библиотека [libdeflate](https://github.com/ebiggers/libdeflate).

## Криптография

Пространство имён `McProtoNet.Cryptography` содержит два класса для упрощения
работы с шифрованием:

1. `CryptoHandler`

Утилитарный класс, который предоставляет некоторые методы:

<deflist>
<def title="DecodeRSAPublicKey">
<p>

</p>
</def>
<def title="GenerateAESPrivateKey(string,string, byte[])">
<p>
Генерирует случайный 128-битный приватный ключ.
</p>
<p>
Параметры:
</p>
<p>
<b>serverID</b> - 
</p>
<p>
Возвращает <code>byte[]</code> - ключ.
</p>
</def>
<def title="GetServerHash">
<p>

</p>
</def>
</deflist>

## Класс `MinecraftClient`

Класс `MinecraftClient` — это минималистичная обертка над TCP-соединением, предназначенная 
для передачи пакетов Minecraft-протокола. Он не выполняет обработку пакетов, а лишь отправляет
и принимает их, предоставляя низкоуровневый доступ к сетевому взаимодействию.  

Этот класс одноразовый: после отключения, ошибки или остановки соединения его нельзя 
повторно использовать — необходимо создать новый экземпляр.   

Исходный код: [MinecraftClient.cs](https://github.com/Titlehhhh/McProtoNet/blob/dev/src/McProtoNet/Client/MinecraftClient.cs)  

### Основные методы

#### `ConnectAsync()`  
Устанавливает соединение с сервером Minecraft на основе переданных параметров.  

#### `SendPacket(IPacket packet)`  
Отправляет указанный пакет серверу.  

#### `IAsyncEnumerable<IPacket> ReceivePackets()`  
Возвращает асинхронный поток пакетов, получаемых от сервера.  

#### `Dispose()`  
Закрывает соединение и освобождает ресурсы.  

#### `SwitchEncryption(Span<byte> key)`  
Включает шифрование соединения с использованием переданного ключа.  

#### `SwitchCompression(int threshold)`  
Включает сжатие пакетов, если их размер превышает указанный порог.  

### Основные настройки

<deflist>
<def title="Host">
Хост (IP-адрес или доменное имя) сервера Minecraft для подключения.
</def>
<def title="Port">
Порт, используемый для подключения к серверу Minecraft. 
По умолчанию используется стандартный порт Minecraft — 25565.
</def>
<def title="Username">
Имя пользователя, которое будет использоваться для аутентификации на сервере.
</def>
<def title="Version">
Версия протокола Minecraft, с которой должен работать клиент. 
Этот параметр используется для обеспечения совместимости с различными версиями сервера.
</def>
<def title="Proxy?">
Прокси-сервер, через который будет осуществляться подключение. 
Если не указан, подключение происходит напрямую.
</def>
<def title="ConnectTimeout">
Таймаут подключения к серверу. 
Устанавливает максимальное время ожидания установления соединения.
</def>
<def title="ReadTimeout">
Таймаут чтения данных из сети, измеряется в миллисекундах. 
Этот параметр определяет, сколько времени клиент будет ожидать данные от сервера.
</def>
<def title="WriteTimeout">
Таймаут записи данных в сеть, измеряется в миллисекундах. 
Этот параметр определяет, сколько времени клиент будет ожидать успешную отправку данных на сервер.
</def>
</deflist>

