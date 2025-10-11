# Как работает

McProtoNet представляет из себя набор библиотек, предназначенных для разных задач.

## McProtoNet.Abstractions
Базовые интерфейсы и DTO для работы с протоколом Minecraft. Содержит абстракции пакетов, потоков данных и расширяемости.

## McProtoNet.Serialization
Универсальная система сериализации для преобразования объектов в бинарный формат Minecraft-пакетов. Включает кодеки для примитивов и составных типов.

## McProtoNet.NBT
Реализация формата Named Binary Tag (NBT) с поддержкой чтения/записи, LINQ-запросов и преобразования в JSON-подобные структуры.

## McProtoNet
Ядро библиотеки с реализацией Minecraft-клиента. Содержит управление подключением, обработку пакетов, сжатие и шифрование трафика.

## McProtoNet.Protocol
Спецификации протокола для разных версий Minecraft. Включает реестр пакетов, валидацию версий и утилиты для работы с протокольными особенностями.

---

## Граф зависимости

<code-block lang="mermaid">
graph TD
    McProtoNet.NBT
    McProtoNet.Utils
    McProtoNet.Serialization --> McProtoNet.NBT
    McProtoNet.Abstractions --> McProtoNet.Serialization    
    McProtoNet --> McProtoNet.Abstractions
    McProtoNet --> McProtoNet.NBT
    McProtoNet --> McProtoNet.Serialization
    McProtoNet.Protocol --> McProtoNet 
</code-block>