using System;
using System.Buffers.Binary;
using System.Text;

namespace McProtoNet.Benchmark;

public enum ZlibPayload
{
    Text,
    ChunkLike,
    Mixed
}

internal static class ZlibBenchData
{
    public static byte[] Make(ZlibPayload kind, int length, int seed)
    {
        var rng = new Random(seed);
        return kind switch
        {
            ZlibPayload.Text => MakeText(rng, length),
            ZlibPayload.ChunkLike => MakeChunkLike(rng, length),
            ZlibPayload.Mixed => MakeMixed(rng, length),
            _ => throw new ArgumentOutOfRangeException(nameof(kind))
        };
    }

    private static byte[] MakeMixed(Random rng, int length)
    {
        var b = new byte[length];
        for (int i = 0; i < length; i++)
            b[i] = (byte)(i % 7 == 0 ? rng.Next(256) : 97 + i % 13);
        return b;
    }

    private static readonly string[] Names =
    {
        "minecraft:stone", "minecraft:dirt", "minecraft:grass_block", "minecraft:oak_log", "minecraft:water",
        "minecraft:deepslate", "minecraft:iron_ore", "minecraft:gravel", "minecraft:sand", "minecraft:oak_leaves"
    };

    private static byte[] MakeText(Random rng, int length)
    {
        var sb = new StringBuilder(length + 128);
        int id = 0;
        while (sb.Length < length)
        {
            sb.Append("{\"id\":").Append(id++)
                .Append(",\"type\":\"").Append(Names[rng.Next(Names.Length)])
                .Append("\",\"pos\":[").Append(rng.Next(-3000, 3000)).Append(',').Append(rng.Next(-64, 320))
                .Append(',').Append(rng.Next(-3000, 3000))
                .Append("],\"text\":{\"translate\":\"chat.type.text\",\"with\":[\"Player")
                .Append(rng.Next(100)).Append("\",\"hello world ").Append(rng.Next(1000)).Append("\"]}}\n");
        }

        var bytes = Encoding.UTF8.GetBytes(sb.ToString());
        Array.Resize(ref bytes, length);
        return bytes;
    }

    private static byte[] MakeChunkLike(Random rng, int length)
    {
        var b = new byte[length];
        int pos = 0;
        while (pos < length)
        {
            pos = WriteSection(b, pos, rng);
        }

        return b;
    }

    private static int WriteSection(byte[] b, int pos, Random rng)
    {
        int end = b.Length;
        if (pos + 2 > end) return end;
        BinaryPrimitives.WriteInt16BigEndian(b.AsSpan(pos), (short)rng.Next(0, 4096));
        pos += 2;

        int paletteSize = rng.Next(2, 16);
        if (pos + 2 > end) return end;
        b[pos++] = 4;
        b[pos++] = (byte)paletteSize;
        for (int i = 0; i < paletteSize && pos + 3 <= end; i++)
        {
            pos = WriteVarInt(b, pos, rng.Next(1, 30000));
        }

        int longs = 4096 * 4 / 64;
        if (pos + 2 > end) return end;
        BinaryPrimitives.WriteInt16BigEndian(b.AsSpan(pos), (short)longs);
        pos += 2;

        int current = rng.Next(paletteSize);
        int run = 0;
        for (int l = 0; l < longs && pos + 8 <= end; l++)
        {
            ulong packed = 0;
            for (int k = 0; k < 16; k++)
            {
                if (run-- <= 0)
                {
                    if (rng.Next(100) < 85) current = rng.Next(2);
                    else current = rng.Next(paletteSize);
                    run = rng.Next(4, 200);
                }

                packed |= (ulong)(uint)current << (k * 4);
            }

            BinaryPrimitives.WriteUInt64BigEndian(b.AsSpan(pos), packed);
            pos += 8;
        }

        pos = WriteRuns(b, pos, end, 2048, 0xFF, rng);
        pos = WriteRuns(b, pos, end, 2048, 0x00, rng);
        return pos;
    }

    private static int WriteRuns(byte[] b, int pos, int end, int count, byte fill, Random rng)
    {
        int i = 0;
        while (i < count && pos < end)
        {
            int run = Math.Min(count - i, rng.Next(16, 512));
            byte value = rng.Next(100) < 85 ? fill : (byte)rng.Next(256);
            for (int k = 0; k < run && pos < end; k++, i++) b[pos++] = value;
        }

        return pos;
    }

    private static int WriteVarInt(byte[] b, int pos, int value)
    {
        uint v = (uint)value;
        while (v >= 0x80)
        {
            b[pos++] = (byte)(v | 0x80);
            v >>= 7;
        }

        b[pos++] = (byte)v;
        return pos;
    }
}
