# Исключения

Мультиверсионные пакеты отправляются и читаются специальными методами расширений. Эти методы
для удобства отладки могут генерировать на данный момент три исключения:

## ProtocolNotSupportedException

Это исключение генерируется, если отправляемый пакет не поддерживается версией протокола, которая указана
в `IMinecraftClient`

## PacketDeserializationException

Возникает когда:

- Ошибка десериализации полученного пакета
- Неожиданный формат данных
- Несоответствие между структурой пакета и полученными данными
- Поврежденные или неполные данные в бинарном потоке

## PacketSerializationException

Возникает когда:

- Ошибка сериализации пакета перед отправкой
- Невалидные данные в пакете (например, строка превышает максимальную длину)
- Проблемы с кодеками сериализации