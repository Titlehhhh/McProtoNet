# McProtoNet.Serialization

**McProtoNet.Serialization** — это библиотека, предназначенная для упрощения сериализации и десериализации данных Minecraft-протокола. Она предоставляет мощный и гибкий API для работы с бинарными данными, характерными для Minecraft, включая:

- Обработку данных в формате BigEndian (старший байт вперёд), что соответствует спецификациям Minecraft-протокола и устраняет необходимость ручного управления порядком байтов.
- Сериализацию и десериализацию специфичных типов данных, таких как [VarInt](https://developers.google.com/protocol-buffers/docs/encoding#varints), [VarLong](https://developers.google.com/protocol-buffers/docs/encoding#varints), [UUID](https://en.wikipedia.org/wiki/Universally_unique_identifier) и [NBT](https://minecraft.fandom.com/wiki/NBT_format) (Named Binary Tag).
- Удобные методы для чтения и записи структур, что упрощает работу с низкоуровневыми данными.


