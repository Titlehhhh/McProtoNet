# Версия → протокол

Minecraft Java Edition различает версии не по имени, а по числу - номеру
протокола. От него зависит раскладка полей пакета: библиотека держит по
несколько раскладок одного пакета, нужную выбирает номер, пришедший от
вызывающего кода. Он передаётся параметром в `SendAsync`, `PacketIo.TryDecode`,
`PacketFlow.Dispatch`, `PacketRegistry.TryResolve` и в первом пакете
рукопожатия:

```csharp
await client.SendAsync(
    new HandshakeSb.SetProtocolPacket(Pv, host, port, 2), Pv);
```

## Поддержанный диапазон

Библиотека поддерживает диапазон от %min_minecraft_version% до
%max_minecraft_version%. В коде границы - именованные константы:

```csharp
public const int StartProtocol = V1_16_Protocol;   // 735
public const int LatestProtocol = V26_2_Protocol;  // 776
```

## Таблица соответствий

`MinecraftVersion.FromProtocol` сводит номер к версии; снапшоты и пре-релизы
1.16.2 сведены к строке 1.16.2, 1.16.3-rc1 - к 1.16.3. Полная и постоянно
обновляемая таблица версий и номеров протокола - на странице
[Protocol version](https://minecraft.wiki/w/Protocol_version) на minecraft.wiki.

| Версия игры | Протокол |
| --- | --- |
| 1.16 | 735 |
| 1.16.1 | 736 |
| 1.16.2 | 751 |
| 1.16.3 | 753 |
| 1.16.4-1.16.5 | 754 |
| 1.17 | 755 |
| 1.17.1 | 756 |
| 1.18-1.18.1 | 757 |
| 1.18.2 | 758 |
| 1.19 | 759 |
| 1.19.2 | 760 |
| 1.19.3 | 761 |
| 1.19.4 | 762 |
| 1.20-1.20.1 | 763 |
| 1.20.2 | 764 |
| 1.20.3-1.20.4 | 765 |
| 1.20.5-1.20.6 | 766 |
| 1.21-1.21.1 | 767 |
| 1.21.3 | 768 |
| 1.21.4 | 769 |
| 1.21.5 | 770 |
| 1.21.6 | 771 |
| 1.21.7-1.21.8 | 772 |
| 1.21.9-1.21.10 | 773 |
| 1.21.11 | 774 |
| 26.1-26.1.2 | 775 |
| 26.2 | 776 |

## Как получить номер программно

[`MinecraftVersion`](../08-api-reference/McProtoNet/Protocol/MinecraftVersion.md)
несёт именованные константы, обратный поиск и полный список - таблицу
переписывать в код не нужно:

```csharp
int pv = MinecraftVersion.V1_21_11;      // неявно в int, даёт 774
string name = MinecraftVersion.FromProtocol(772).Name;  // "1.21.7–1.21.8"
foreach (var v in MinecraftVersion.AllVersions)
    Console.WriteLine($"{v.Name} -> {v.Protocol}");
```

`FromProtocol` бросает `NotSupportedException` на числе вне таблицы, в том числе
внутри диапазона 735-776, если оно не отвечает версии.

## Номер вне диапазона

Проверки на входе в
[`MinecraftClient`](../08-api-reference/McProtoNet/MinecraftClient.md) нет - она
приходит от самих пакетов. У
[`SetProtocolPacket`](../08-api-reference/McProtoNet/Protocol/Packets/Handshaking/Serverbound/SetProtocolPacket.md)
и любого пакета с раскладками по версиям объявлен диапазон:

```csharp
[ProtocolSupport(MinecraftVersion.StartProtocol, MinecraftVersion.LatestProtocol)]
```

Номер вне диапазона даёт
[`ProtocolNotSupportException`](../08-api-reference/McProtoNet/Protocol/ProtocolNotSupportException.md)
при первой попытке отправить или разобрать пакет, до выхода в сеть. Исключение
несёт имя типа, номер и диапазоны, на которых этот тип существует.

## Дальше

- [Словарь](02-glossary.md)
- [Одна сборка - много версий](../05-packets/05-multiversion.md)
