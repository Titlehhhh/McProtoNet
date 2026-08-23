# Как создаются экземпляры серверных пакетов?

Класс `MinecraftClient` читает "сырые" серверные пакеты, которые нужно преобразовать
в конкретный класс. Этого можно добиться с помощью метода `CreateClientboundPacket`
из статического класс `PacketFactory`. Этот метод принимает в качестве аргументов
версию протокола, идентификатор прочитанного пакета, а также `PacketState`(пакет какого состояния нужно создать).

## Пример кода

```C#
MinecraftClient client = //...

await foreach(InputPacket packet = client.ReceivePackets())
{
    IServerPacket createdPacket = 
        PacketFactory.CreateClientboundPacket(
            client.ProtocolVersion, 
            packet.Id, 
            PacketState.Play);
    
    // Deserialize and handling ...
}
```

Хотя Вы можете так делать, но для упрощения разработки рекомендуется использовать
методы расширения `OnPacket<TPacket>`, `OnAllPackets`.

```C#
await foreach(IServerPacket packet in 
    client.OnAllPackets(PacketState.Play))
{
    Console.WriteLine($"Received packet: {packet.GetPacketId()}");
}
```