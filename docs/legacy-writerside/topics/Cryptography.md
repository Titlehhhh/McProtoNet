# Криптография

Пространство имён `McProtoNet.Cryptography` содержит утилиты для работы с криптографией в протоколе Minecraft. Основной класс `CryptoHandler` предоставляет методы для:

- Декодирования RSA-ключей
- Генерации AES-ключей
- Вычисления хэшей для аутентификации

## Класс `CryptoHandler`

```C#
using System.Security.Cryptography;
using System.Text;

namespace McProtoNet.Cryptography;

public static class CryptoHandler
{
    // Основные методы
}
```

### Основные методы

#### DecodeRSAPublicKey(byte[] x509key)

Декодирует RSA публичный ключ из формата X.509 для использования в шифровании.

**Параметры**
- `x509key` (`byte[]`) — сырые байты публичного ключа в кодировке ASN.1

**Возвращает**
- `RSACryptoServiceProvider` — объект для работы с RSA-шифрованием
- `null` — если декодирование не удалось

**Пример использования**
```C#
byte[] serverPublicKey = GetKeyFromServer();
var rsa = CryptoHandler.DecodeRSAPublicKey(serverPublicKey);
```

---

#### GenerateAESPrivateKey()

Генерирует случайный 128-битный ключ для симметричного AES-шифрования

**Возвращает**
- `byte[]` — 16-байтовый AES-ключ

**Особенности**
- Автоматически устанавливает размер ключа 128 бит
- Использует криптографически безопасный генератор

---

#### GetServerHash(string serverID, byte[] PublicKey, byte[] SecretKey)
 
Вычисляет SHA-1 хэш для проверки сессии в online-режиме

**Параметры**
- `serverID` (`string`) — уникальный идентификатор сервера
- `PublicKey` (`byte[]`) — публичный RSA-ключ сервера
- `SecretKey` (`byte[]`) — секретный ключ клиента

**Возвращает**
- `string` — HEX-строка хэша в формате, требуемом протоколом Minecraft

**Алгоритм**
1. Объединяет данные в последовательность: `serverID + SecretKey + PublicKey`
2. Вычисляет SHA-1 хэш
3. Преобразует результат в знаковое целое


### Пример потока шифрования
```C#
// 1. Получаем публичный ключ сервера
byte[] serverKey = GetServerPublicKey();

// 2. Декодируем RSA-ключ
using var rsa = CryptoHandler.DecodeRSAPublicKey(serverKey);

// 3. Генерируем AES-ключ
byte[] aesKey = CryptoHandler.GenerateAESPrivateKey();

// 4. Шифруем AES-ключ с помощью RSA
byte[] encryptedKey = rsa.Encrypt(aesKey, RSAEncryptionPadding.Pkcs1);

// 5. Отправляем зашифрованный ключ серверу
SendEncryptedKey(encryptedKey);
```
