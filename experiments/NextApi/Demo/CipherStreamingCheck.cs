// EXPERIMENT (McProtoNet.Next) — the critic's ACCEPT criterion for the AES/CFB8 migration,
// as a self-test check. Proves AesCfb8Cipher reproduces one continuous CFB8 pass no matter how
// the stream is cut into Transform calls — the property the rejected prototype lacked.
//
// Oracle: a single Aes.EncryptCfb/DecryptCfb over the WHOLE buffer from the IV is, by
// definition, continuous CFB8. The streaming cipher, fed the same bytes in adversarial chunk
// patterns (1, 5, 15, 16, 17, non-16 tails, empty chunks), must match it byte-for-byte in both
// directions — and encrypt→decrypt must round-trip. Any divergence throws.

using System.Security.Cryptography;

namespace McProtoNet.Next.Demo;

internal static class CipherStreamingCheck
{
    public static Task RunAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        byte[] key = Seeded(0x51, 16);
        byte[] iv = key;                     // IV = shared secret, as in the protocol
        byte[] plaintext = Seeded(0x2A, 4096);

        // Oracle: one continuous pass over the whole buffer.
        byte[] oracleCipher = new byte[plaintext.Length];
        using (var oracle = Aes.Create())
        {
            oracle.Key = key;
            oracle.EncryptCfb(plaintext, iv, oracleCipher, PaddingMode.None, feedbackSizeInBits: 8);
        }

        foreach (int[] pattern in ChunkPatterns(plaintext.Length))
        {
            // Encrypt streamed in this pattern must equal the oracle ciphertext.
            byte[] enc = (byte[])plaintext.Clone();
            StreamThrough(new AesCfb8Cipher(key, iv, encrypting: true), enc, pattern);
            RequireEqual(enc, oracleCipher, "encrypt", pattern);

            // Decrypt the oracle ciphertext streamed in this pattern must recover the plaintext.
            byte[] dec = (byte[])oracleCipher.Clone();
            StreamThrough(new AesCfb8Cipher(key, iv, encrypting: false), dec, pattern);
            RequireEqual(dec, plaintext, "decrypt", pattern);
        }

        return Task.CompletedTask;
    }

    // Applies the cipher over the buffer in the given sequence of chunk sizes, in place.
    private static void StreamThrough(PacketCipher cipher, byte[] buffer, int[] chunkSizes)
    {
        using (cipher)
        {
            int offset = 0;
            foreach (int size in chunkSizes)
            {
                cipher.Transform(buffer.AsSpan(offset, size));
                offset += size;
            }

            if (offset != buffer.Length)
                throw new InvalidOperationException(
                    $"Chunk pattern covered {offset} of {buffer.Length} bytes.");
        }
    }

    // Chunk patterns that cover exactly `length` bytes, each stressing a different boundary case.
    private static IEnumerable<int[]> ChunkPatterns(int length)
    {
        yield return [length];                          // one shot — must match the oracle trivially
        yield return Repeat(1, length);                 // every byte its own call (all < 16)
        yield return Repeat(5, length);                 // sub-block chunks, non-16 tail
        yield return Repeat(15, length);                // just below the block
        yield return Repeat(16, length);                // exact blocks
        yield return Repeat(17, length);                // just above the block, drifting tails
        yield return Ascending(length);                 // 0,1,2,3,… lengths incl. empty and < 16
        yield return SeededChunks(length);              // pseudo-random mix of 1..40, many < 16
    }

    private static int[] Repeat(int size, int length)
    {
        var list = new List<int>();
        for (int left = length; left > 0; left -= size)
            list.Add(Math.Min(size, left));
        return list.ToArray();
    }

    private static int[] Ascending(int length)
    {
        var list = new List<int>();
        int size = 0;
        for (int left = length; left > 0;)
        {
            int take = Math.Min(size, left);
            list.Add(take);                             // includes a 0-length Transform (contract allows empty)
            left -= take;
            size = size >= 20 ? 0 : size + 1;
        }
        return list.ToArray();
    }

    private static int[] SeededChunks(int length)
    {
        var rng = new Random(0x1234);
        var list = new List<int>();
        for (int left = length; left > 0;)
        {
            int take = Math.Min(rng.Next(1, 41), left); // 1..40, biased toward < 16
            list.Add(take);
            left -= take;
        }
        return list.ToArray();
    }

    private static void RequireEqual(byte[] actual, byte[] expected, string direction, int[] pattern)
    {
        if (actual.AsSpan().SequenceEqual(expected))
            return;

        int at = 0;
        while (at < actual.Length && actual[at] == expected[at])
            at++;
        throw new InvalidOperationException(
            $"CFB8 {direction} streamed in {DescribePattern(pattern)} diverged from the one-shot oracle at byte {at}.");
    }

    private static string DescribePattern(int[] pattern)
    {
        if (pattern.Length == 1)
            return $"one chunk of {pattern[0]}";
        int distinct = pattern.Distinct().Count();
        return distinct == 1
            ? $"{pattern.Length} chunks of {pattern[0]}"
            : $"{pattern.Length} mixed chunks (first {Math.Min(6, pattern.Length)}: {string.Join(",", pattern.Take(6))}…)";
    }

    private static byte[] Seeded(int seed, int length)
    {
        byte[] data = new byte[length];
        new Random(seed).NextBytes(data);
        return data;
    }
}
