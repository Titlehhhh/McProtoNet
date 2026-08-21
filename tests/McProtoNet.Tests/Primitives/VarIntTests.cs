using System.Buffers;
using McProtoNet.Primitives;
namespace McProtoNet.Tests.Primitives;

public class VarIntTests
{
    public static IEnumerable<object[]> GetNumbers() =>
        Enumerable.Range(0, 10_000).Select(x => new object[] { x });

    [Theory]
    [MemberData(nameof(GetNumbers))]
    public void Test1(int value)
    {
        for (int segSize = 1; segSize <= 5; segSize++)
        {
            var seq = CreateSequence(value, segSize);
            var read = seq.TryReadVarInt(out var actual, out var length);
            Assert.True(read);
            Assert.Equal(value, actual);
            Assert.Equal(value.GetVarIntLength(), length);
        }
    }

    private static ReadOnlySequence<byte> CreateSequence(int value, int segmentSize)
    {
        var arr = value.VarIntToArray();
        var segments = arr.Chunk(segmentSize)
                          .Select(x => new ReadOnlyMemory<byte>(x))
                          .ToList();
        return BuildSequence(segments);
    }

    private static ReadOnlySequence<byte> BuildSequence(IList<ReadOnlyMemory<byte>> segments)
    {
        if (segments.Count == 0) return ReadOnlySequence<byte>.Empty;
        if (segments.Count == 1) return new ReadOnlySequence<byte>(segments[0]);

        Seg? first = null, prev = null;
        long pos = 0;
        foreach (var mem in segments)
        {
            var seg = new Seg(mem, pos);
            if (first == null) first = seg;
            prev?.SetNext(seg);
            prev = seg;
            pos += mem.Length;
        }
        return new ReadOnlySequence<byte>(first!, 0, prev!, prev!.Memory.Length);
    }

    private sealed class Seg : ReadOnlySequenceSegment<byte>
    {
        public Seg(ReadOnlyMemory<byte> mem, long runningIndex)
        {
            Memory = mem;
            RunningIndex = runningIndex;
        }
        public void SetNext(Seg next) => Next = next;
    }
}