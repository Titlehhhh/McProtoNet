using System.Buffers;
using System.Diagnostics;
using DotNext.Buffers;
using McProtoNet.Serialization;

namespace McProtoNet.Tests.Serialization;

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

        return arr.Chunk(segmentSize).Select(x =>
            new ReadOnlyMemory<byte>(x)
        ).ToReadOnlySequence();
    }
}