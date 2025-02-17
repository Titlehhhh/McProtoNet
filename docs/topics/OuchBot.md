# OuchBot

Давайте сделаем простого бота, который пишет в чат `Ouch!!!`, когда его бьют.

Для этого нам нужно узнать идентификатор сущности бота из пакета Login(он еще называется Join Game):

```C#
if (packet is CPlay.LoginPacket login)
{
    _myEntityId = login.EntityId;
}
```

Затем нужно прочитать пакет `EntitiyVelocity`, который передает скорость сущности. Мы должны сравнить 
переданный идентификатор в пакете со своим и затем отправить пакет чата:

```C#
if (packet is CPlay.EntityVelocityPacket velocityPacket)
{
    if (velocityPacket.EntityId == _myEntityId)
    {
        _ = SendChat("Ouch!!!");
    }
}
```

К сожалению из-за автогенерации пакетов пакет чата разделен на два. Поэтому для удобства был создан метод:

```C#
private async ValueTask SendChat(string message)
{
    if (_client.TrySend<SPlay.ChatPacket>(out var sender1))
    {
        sender1.Packet.Message = message;
        await sender1.Send();
    }
    else if (_client.TrySend<SPlay.ChatMessagePacket>(out var sender2))
    {
        sender2.Packet.Message = message;
        await sender2.Send();
    }
}
```

<video src="../videos/ouchbot-preview.mp4"/>