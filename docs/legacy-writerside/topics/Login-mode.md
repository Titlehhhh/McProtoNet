# Режим Login

После отправки пакетов  сервер нам отправит некоторые важные пакеты.
Давайте их прочитаем:

```C#
await foreach (var serverPacket in 
    client.OnAllPackets(PacketState.Login))
{
    // Обработка
}
```