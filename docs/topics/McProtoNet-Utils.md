# McProtoNet.Utils

В этой библиотеке находятся вспомогательные инструменты, которые косвенно необходимы для запуска ботов.

## SRV Lookup

```C#
using McProtoNet.Utils;

IServerResolver resolver = new ServerResolver();

try 
{
    var result = await resolver.ResolveAsync("mc.example.com");
    Console.WriteLine($"SRV найдено {result.Host}:{result.Port}");
}
catch(Exception ex)
{
    Console.WriteLine("Ошибка поиска SRV для mc.example.com");
}
```

## LanServerDetector

<tip>
    Находится в разработке
</tip>

Позволяет искать сервера в локальной сети, которые отправляют UDP broadcast пакеты на `224.0.2.60:4445`.
Подробнее по [ссылке](https://minecraft.fandom.com/wiki/Tutorials/Setting_up_a_LAN_world).

```C#
using McProtNet.Utils;

var detector = new LanServerDetector(ttl: 0);
```

