namespace McProtoNet.Cryptography;

internal static class AesKeySchedule
{
    public const int ExpandedKey128Length = 176;

    private static readonly byte[] SBox = BuildSBox();

    private static readonly byte[] Rcon = [0x01, 0x02, 0x04, 0x08, 0x10, 0x20, 0x40, 0x80, 0x1B, 0x36];

    public static void ExpandKey128(ReadOnlySpan<byte> key, Span<byte> expanded)
    {
        key[..16].CopyTo(expanded);

        for (int word = 4; word < 44; word++)
        {
            int previous = (word - 1) * 4;
            byte w0 = expanded[previous];
            byte w1 = expanded[previous + 1];
            byte w2 = expanded[previous + 2];
            byte w3 = expanded[previous + 3];

            if (word % 4 == 0)
            {
                byte rotated = w0;
                w0 = (byte)(SBox[w1] ^ Rcon[word / 4 - 1]);
                w1 = SBox[w2];
                w2 = SBox[w3];
                w3 = SBox[rotated];
            }

            int back = (word - 4) * 4;
            int current = word * 4;
            expanded[current] = (byte)(expanded[back] ^ w0);
            expanded[current + 1] = (byte)(expanded[back + 1] ^ w1);
            expanded[current + 2] = (byte)(expanded[back + 2] ^ w2);
            expanded[current + 3] = (byte)(expanded[back + 3] ^ w3);
        }
    }

    private static byte[] BuildSBox()
    {
        byte[] sbox = new byte[256];
        byte p = 1;
        byte q = 1;

        do
        {
            p = (byte)(p ^ (p << 1) ^ ((p & 0x80) != 0 ? 0x1B : 0));

            q ^= (byte)(q << 1);
            q ^= (byte)(q << 2);
            q ^= (byte)(q << 4);
            if ((q & 0x80) != 0)
            {
                q ^= 0x09;
            }

            sbox[p] = (byte)(q ^ RotateLeft(q, 1) ^ RotateLeft(q, 2) ^ RotateLeft(q, 3) ^ RotateLeft(q, 4) ^ 0x63);
        } while (p != 1);

        sbox[0] = 0x63;
        return sbox;
    }

    private static byte RotateLeft(byte value, int shift)
    {
        return (byte)((value << shift) | (value >> (8 - shift)));
    }
}
