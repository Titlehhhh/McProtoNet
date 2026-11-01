using System.Buffers;
using McProtoNet.Transport.Cryptography;
using McProtoNet.Transport.Framing;

namespace McProtoNet.Tests.Infrastructure;

/// <summary>Builds wire bytes the same way the transport does, so readers can be compared to them.</summary>
public static class Frames
{
    /// <summary>Body sizes that hit every varint width, both sides of a 256 threshold, and 70 KiB.</summary>
    private static readonly int[] Sizes =
    [
        0, 1, 2, 3, 5, 15, 63, 126, 127, 128, 129, 254, 255, 256, 257, 300, 511, 512, 1000, 1024,
        4095, 4096, 8192, 16 * 1024, 32 * 1024, 64 * 1024, 65535, 65536, 70 * 1024
    ];

    /// <summary>A cheap sample for byte-at-a-time runs: same varint widths, nothing huge.</summary>
    public static List<TestPacket> SmallSample(int seed)
    {
        var random = new Random(seed);
        int[] sizes = [0, 1, 2, 5, 63, 126, 127, 128, 129, 255, 256, 257, 300, 1000];
        var packets = new List<TestPacket>();
        foreach (var size in sizes)
        {
            var body = new byte[size];
            random.NextBytes(body);
            packets.Add(new TestPacket(random.Next(0, 1 << 20), body));
        }

        return packets;
    }

    public static List<TestPacket> Sample(int seed, int repeats = 3)
    {
        var random = new Random(seed);
        var packets = new List<TestPacket>();

        for (var round = 0; round < repeats; round++)
        {
            foreach (var size in Sizes)
            {
                var body = new byte[size];
                // half random (incompressible), half repetitive (compresses hard) — both paths get exercised
                if (round % 2 == 0) random.NextBytes(body);
                else for (var i = 0; i < body.Length; i++) body[i] = (byte)(i % 7);

                packets.Add(new TestPacket(random.Next(0, 1 << 20), body));
            }
        }

        return packets;
    }

    public static byte[] Build(IReadOnlyList<TestPacket> packets, int compressionThreshold, bool encrypted)
    {
        var writer = new ArrayBufferWriter<byte>(1 << 20);
        foreach (var packet in packets)
            writer.WritePacket(packet.Id, packet.Body, compressionThreshold);

        var wire = writer.WrittenSpan.ToArray();
        if (encrypted)
        {
            using var encryptor = Crypto.CreateEncryptor();
            encryptor.Transform(wire);
        }

        return wire;
    }

    /// <summary>Frame end offsets in the wire, so a reader's stream position can be checked exactly.</summary>
    public static int[] FrameEnds(IReadOnlyList<TestPacket> packets, int compressionThreshold)
    {
        var ends = new int[packets.Count];
        var writer = new ArrayBufferWriter<byte>(1 << 20);
        for (var i = 0; i < packets.Count; i++)
        {
            writer.WritePacket(packets[i].Id, packets[i].Body, compressionThreshold);
            ends[i] = writer.WrittenCount;
        }

        return ends;
    }

    public static PacketCipher? Decryptor(bool encrypted) => encrypted ? Crypto.CreateDecryptor() : null;
}
