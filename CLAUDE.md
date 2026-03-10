# Claude Code Instructions — McProtoNet

## Правила генерации пакетов и типов

Читать перед любой работой с пакетами:

- `ai/AGENTS_PACKETS.MD` — правила генерации пакетов
- `ai/AGENTS_TYPES.MD` — правила генерации типов
- `ai/README.md` — общий обзор
- `src/McProtoNet.Protocol/AGENTS_SERIALIZATION.MD` — доступные методы сериализации

## Структура проекта

- `src/McProtoNet.Protocol/Packets/` — сгенерированные пакеты
- `src/McProtoNet.Protocol/Types/` — мультиверсионные типы

## Важно

- Версии: `MinecraftVersion.StartProtocol` … `MinecraftVersion.LatestProtocol`
- Никогда не хардкодить числовые значения этих констант
- Пакеты: `partial`, `sealed`, атрибут `[PacketInfo]`, внутренние `Serialize`/`Deserialize`
