using System.Buffers;
using DotNext.Buffers;
using McProtoNet.Serialization;

namespace McProtoNet.Tests.Serialization;

public class VarIntTests
{
    [Fact]
    public void Test1()
    {
        var gg = new SequenceBuilder<byte>();
        byte[] buff = new byte[8192];
        int len1 = 500.GetVarIntLength(buff);
        var asSpan = buff.AsSpan(len1, 500);
        for (int i = 0; i < asSpan.Length; i++)
        {
            asSpan[i] = unchecked((byte)i);
        }

        var orig = asSpan.ToArray();


        gg.Write(buff);
        Array.Clear(buff);
        gg.Write(buff);

        var segments = gg.ToArray();
        var start = gg.Start;
        var seq = gg.Read(ref start, gg.WrittenCount);

        var b = seq.TryReadVarInt(out int result, out var len);
        
        Assert.True(b);

        var startPacket = seq.GetPosition(len);
        var endPacket = seq.GetPosition(result, startPacket);

        var packet = seq.Slice(startPacket, endPacket);

        Assert.Equal(500, packet.Length);

        Assert.Equal(orig, packet.ToArray());
    }
}