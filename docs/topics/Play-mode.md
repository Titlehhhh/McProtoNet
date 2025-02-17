# Игровой режим

Прекрасно! Наш бот умеет подключиться к серверу, проходит аутентификацию и конфигурацию.

```C#
_client = new MinecraftClient(new MinecraftClientStartOptions()
{
    ConnectTimeout = TimeSpan.FromSeconds(5),
    Host = _host,
    Port = 25565,
    WriteTimeout = TimeSpan.FromSeconds(5),
    ReadTimeout = TimeSpan.FromSeconds(5),
    Version = (int)_version
});

await _client.ConnectAsync();
await _client.Login("TestBot", _host,25565);
```

Запустим его.

![runbot.png](runbot.png)

Однако после некоторого простоя бота кикает сервер с ошибкой таймаута.

![botleave.png](botleave.png)


## Чтение игровых пакетов

Дабы бота не кикало, нужно отвечать на пакеты `KeepAlive`.

```C#
private async Task RunReadLoop()
{
    await foreach (var packet in _client.OnAllPackets(PacketState.Play))
    {
        HandlePlayPacket(packet);
    }
}
```

```C#
if (packet is CPlay.KeepAlivePacket keepAlive)
{
    _client.SendPacket(new SPlay.KeepAlivePacket()
    {
        KeepAliveId = keepAlive.KeepAliveId
    });
}
```

Теперь бот может работать вечно.

