// EXPERIMENT (McProtoNet.Next) — measurement 3: цена AES-128/CFB8 (ключ = IV = 16 Б, как
// в протоколе Minecraft) по трём подходам. Вопрос Д5 плана укрепления
// (docs/design/transport-hardening-plan-2026-08-10.md) — можно ли выкинуть BouncyCastle
// из горячего пути шифрования.
//
// Три подхода:
//   manual  — ручной CFB8 поверх AES-ECB: копия приватного класса AesCfb8Cipher из
//             MinecraftClient.cs (~строки 1144-1177). Сдвиговый регистр 16 Б, один блок
//             AES-ECB на каждый байт входа, XOR первого байта кейстрима.
//   bc      — BouncyCastle, как сегодня собирает шифр AesStream.SwitchEncryption
//             (src/McProtoNet/Cryptography/AesStream.cs, ~строки 79-98):
//             BufferedBlockCipher(CfbBlockCipher(AesEngine, 8)) через ParametersWithIV.
//   builtin — System.Security.Cryptography.Aes, режим CFB8. Перед замером — честная
//             проверка применимости на кусках произвольной длины (см. CheckBuiltIn ниже):
//             ICryptoTransform.TransformBlock через CipherMode.CFB отвергает куски не
//             кратные 16 Б (уже задокументированный факт в MinecraftClient.cs); отдельно
//             проверяется одноразовый EncryptCfb.
//
// Перед замером скорости — проверка корректности: manual и bc обязаны дать байт-в-байт
// одинаковый шифртекст на одном входе (CFB8 детерминирован). Если нет — ручная
// реализация неверна, и её числа НЕ публикуются как валидные.

using System.Diagnostics;
using System.Security.Cryptography;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;

namespace McProtoNet.Next.Measurements;

internal static class CipherCost
{
    public static void Run()
    {
        Console.WriteLine("== Замер 3: цена AES-128/CFB8 — ручной / BouncyCastle / встроенный ==");
        Console.WriteLine("(вопрос Д5 плана укрепления; ключ = IV = 16 Б, как в протоколе)");
        Console.WriteLine();

        // Диагностика: реально ли BouncyCastle взял аппаратный движок (AES-NI), а не
        // управляемый. Если false — цифры bc ниже относятся к управляемому AesEngine, и
        // сравнение с аппаратным .NET нечестное.
        Console.WriteLine($"BouncyCastle AesEngine_X86.IsSupported = {AesEngine_X86.IsSupported} " +
            $"(движок bc: {(AesEngine_X86.IsSupported ? "AesEngine_X86 — аппаратный AES-NI" : "AesEngine — управляемый")})");
        Console.WriteLine($".NET System.Runtime.Intrinsics.X86.Aes.IsSupported = {System.Runtime.Intrinsics.X86.Aes.IsSupported}");
        Console.WriteLine();

        byte[] key = MakeKey();

        bool manualValid = CheckManualMatchesBouncyCastle(key);
        Console.WriteLine();

        bool builtinArbitraryLength = CheckBuiltInArbitraryLength(key);
        Console.WriteLine();

        if (!manualValid)
            Console.WriteLine("ВНИМАНИЕ: ручной CFB8 разошёлся с BouncyCastle — его числа ниже НЕ валидны, не использовать для решения.");

        (int Size, int Iterations)[] plan =
        [
            (16, 500_000),
            (256, 300_000),
            (1500, 60_000),
            (32 * 1024, 4_000),
        ];

        Console.WriteLine($"{"размер",-10}{"подход",-14}{"МБ/с",12}{"нс/байт",10}{"Б аллок./вызов",16}");
        foreach (var (size, iterations) in plan)
            MeasureSize(size, iterations, key, manualValid, builtinArbitraryLength);

        Console.WriteLine();
        Console.WriteLine(builtinArbitraryLength
            ? "встроенный .NET AES CFB8 (EncryptCfb): применим на произвольной длине — см. числа выше."
            : "встроенный .NET AES CFB8: НЕПРИМЕНИМ как есть на произвольной длине — числа для него не приведены.");
    }

    // ------------------------------------------------------------------ correctness

    private static bool CheckManualMatchesBouncyCastle(byte[] key)
    {
        Console.WriteLine("Проверка корректности: manual vs BouncyCastle на одном входе (5000 Б, не кратно 16)");

        byte[] plaintext = MakePlaintext(5000);

        byte[] manualOut = (byte[])plaintext.Clone();
        using (var manual = new ManualCfb8Cipher(key))
            manual.Encrypt(manualOut);

        byte[] bcOut = BouncyCastleOneShot(key, plaintext);

        bool equal = manualOut.AsSpan().SequenceEqual(bcOut);
        Console.WriteLine(equal
            ? "  OK — байт-в-байт совпадение."
            : "  FAIL — шифртексты РАЗЛИЧАЮТСЯ. Ручная реализация неверна.");
        return equal;
    }

    /// <summary>
    /// Честная проверка: работает ли встроенный AES CFB8 на кусках произвольной длины.
    /// Пробует оба пути — потоковый ICryptoTransform.TransformBlock (то, что использует
    /// сам транспорт для manual-обёртки, тут проверяется в режиме CFB напрямую) и
    /// одноразовый Aes.EncryptCfb. Печатает факт, не подгоняет под желаемый ответ.
    /// </summary>
    private static bool CheckBuiltInArbitraryLength(byte[] key)
    {
        Console.WriteLine("Проверка применимости: встроенный AES CFB8 на кусках произвольной длины (1500 Б, 32768 Б)");

        // Путь 1: ICryptoTransform.TransformBlock, CipherMode.CFB, FeedbackSize = 8.
        foreach (int size in new[] { 1500, 32 * 1024 })
        {
            try
            {
                using var aes = Aes.Create();
                aes.Mode = CipherMode.CFB;
                aes.FeedbackSize = 8;
                aes.Padding = PaddingMode.None;
                aes.Key = key;
                using ICryptoTransform enc = aes.CreateEncryptor(key, key);
                byte[] input = MakePlaintext(size);
                byte[] output = new byte[size];
                int written = enc.TransformBlock(input, 0, size, output, 0);
                Console.WriteLine($"  TransformBlock, {size,6} Б: OK ({written} Б записано)");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  TransformBlock, {size,6} Б: ОТКАЗ — {ex.GetType().Name}: {ex.Message}");
            }
        }

        // Путь 2: одноразовый Aes.EncryptCfb. Это и есть тест, который требует задание.
        bool allOk = true;
        byte[]? firstOutput1500 = null;
        foreach (int size in new[] { 1500, 32 * 1024 })
        {
            try
            {
                using var aes = Aes.Create();
                aes.Key = key;
                byte[] input = MakePlaintext(size);
                byte[] output = aes.EncryptCfb(input, key, PaddingMode.None, feedbackSizeInBits: 8);
                Console.WriteLine($"  EncryptCfb,      {size,6} Б: OK ({output.Length} Б записано)");
                if (size == 1500)
                    firstOutput1500 = output;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  EncryptCfb,      {size,6} Б: ОТКАЗ — {ex.GetType().Name}: {ex.Message}");
                allOk = false;
            }
        }

        if (allOk && firstOutput1500 is not null)
        {
            // Бонусная проверка: первые 1500 Б непрерывного потока (manual/BC, тот же
            // ключ=IV) обязаны совпасть с одноразовым EncryptCfb первых 1500 Б того же
            // потока — оба начинают с одного и того же состояния регистра.
            byte[] reference = MakePlaintext(5000).AsSpan(0, 1500).ToArray();
            byte[] refOut = (byte[])reference.Clone();
            using (var manual = new ManualCfb8Cipher(key))
                manual.Encrypt(refOut);

            bool matches = refOut.AsSpan().SequenceEqual(firstOutput1500);
            Console.WriteLine(matches
                ? "  бонус: первые 1500 Б EncryptCfb совпали с manual — OK."
                : "  бонус: первые 1500 Б EncryptCfb РАСХОДЯТСЯ с manual — не использовать как замену без разбора.");
        }

        return allOk;
    }

    // ------------------------------------------------------------------ benchmark

    private static void MeasureSize(int size, int iterations, byte[] key, bool manualValid, bool builtinFeasible)
    {
        byte[] buffer = MakePlaintext(size);

        if (manualValid)
        {
            using var manual = new ManualCfb8Cipher(key);
            RunArm(size, iterations, "manual", buffer, buf => manual.Encrypt(buf));
        }
        else
        {
            Console.WriteLine($"{size,-10}{"manual",-14}{"— пропущено, не прошла проверка корректности",-38}");
        }

        {
            // IBufferedCipher (BouncyCastle) does not implement IDisposable — nothing to release.
            IBufferedCipher bc = CreateBouncyCastleCipher(key, encrypt: true);
            RunArm(size, iterations, "bc", buffer, buf => bc.ProcessBytes(buf, 0, buf.Length, buf, 0));
        }

        if (builtinFeasible)
        {
            // The real shipping cipher (AesCfb8Cipher.cs): Aes.EncryptCfb + a self-kept CFB8
            // register, correct at any chunk length — not the rejected last16-only prototype.
            using var builtin = new AesCfb8Cipher(key, key, encrypting: true);
            RunArm(size, iterations, "builtin", buffer, buf => builtin.Transform(buf));
        }
        else
        {
            Console.WriteLine($"{size,-10}{"builtin",-14}{"— неприменимо, требует кратности 16 (см. проверку выше)",-38}");
        }

        Console.WriteLine();
    }

    private static void RunArm(int size, int iterations, string name, byte[] buffer, Action<byte[]> arm)
    {
        for (int i = 0; i < 200; i++) // прогрев: JIT, аппаратный AES warm-up
            arm(buffer);

        long allocatedBefore = GC.GetAllocatedBytesForCurrentThread();
        var stopwatch = Stopwatch.StartNew();
        for (int i = 0; i < iterations; i++)
            arm(buffer);
        stopwatch.Stop();
        long allocated = GC.GetAllocatedBytesForCurrentThread() - allocatedBefore;

        double totalBytes = (double)size * iterations;
        double mbps = totalBytes / (1024.0 * 1024.0) / stopwatch.Elapsed.TotalSeconds;
        double nsPerByte = stopwatch.Elapsed.TotalNanoseconds / totalBytes;
        double allocPerOp = allocated / (double)iterations;

        Console.WriteLine($"{size,-10}{name,-14}{mbps,12:N1}{nsPerByte,10:N2}{allocPerOp,16:N0}");
    }

    // ------------------------------------------------------------------ ciphers

    /// <summary>
    /// Копия приватного класса AesCfb8Cipher из MinecraftClient.cs (~строки 1144-1177),
    /// зафиксированная под замер: сдвиговый регистр 16 Б, AES-ECB блок на каждый байт,
    /// XOR первого байта кейстрима. Только направление шифрования — расшифровка
    /// симметрична по стоимости и здесь не измеряется.
    /// </summary>
    private sealed class ManualCfb8Cipher : IDisposable
    {
        private readonly Aes _aes;
        private readonly ICryptoTransform _block;
        private readonly byte[] _register = new byte[16];
        private readonly byte[] _keystream = new byte[16];

        public ManualCfb8Cipher(ReadOnlySpan<byte> key)
        {
            _aes = Aes.Create();
            _aes.Mode = CipherMode.ECB;
            _aes.Padding = PaddingMode.None;
            _aes.Key = key.ToArray();
            _block = _aes.CreateEncryptor();
            key.CopyTo(_register); // IV = shared secret, как в протоколе
        }

        public void Encrypt(Span<byte> buffer)
        {
            for (int i = 0; i < buffer.Length; i++)
            {
                _block.TransformBlock(_register, 0, 16, _keystream, 0);
                byte output = (byte)(buffer[i] ^ _keystream[0]);
                buffer[i] = output;

                Buffer.BlockCopy(_register, 1, _register, 0, 15);
                _register[15] = output;
            }
        }

        public void Dispose()
        {
            _block.Dispose();
            _aes.Dispose();
        }
    }

    // The built-in arm now measures the real shipping cipher AesCfb8Cipher (same assembly).
    // The earlier last16-only prototype was removed: the critic pass proved it wrong on chunks
    // below 16 bytes and on the decrypt direction (Demo/CipherStreamingCheck.cs guards this now).

    private static IBufferedCipher CreateBouncyCastleCipher(byte[] key, bool encrypt)
    {
        var cipher = new BufferedBlockCipher(
            new CfbBlockCipher(AesEngine_X86.IsSupported ? new AesEngine_X86() : new AesEngine(), 8));
        cipher.Init(encrypt, new ParametersWithIV(new KeyParameter(key), key, 0, 16));
        return cipher;
    }

    private static byte[] BouncyCastleOneShot(byte[] key, ReadOnlySpan<byte> plaintext)
    {
        IBufferedCipher cipher = CreateBouncyCastleCipher(key, encrypt: true);
        return cipher.DoFinal(plaintext.ToArray());
    }

    // ------------------------------------------------------------------ helpers

    private static byte[] MakeKey()
    {
        byte[] key = new byte[16];
        new Random(1234).NextBytes(key);
        return key;
    }

    private static byte[] MakePlaintext(int length)
    {
        byte[] data = new byte[length];
        new Random(42).NextBytes(data);
        return data;
    }
}
