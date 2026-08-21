using System.Text;
using McProtoNet.Tests.Infrastructure;

namespace McProtoNet.Tests.Net;

/// <summary>
///     The DNS wire format the SRV lookup writes and reads, checked against messages built by hand —
///     including the two things a real resolver will hand us and a naive parser will die on: name
///     compression and a truncated answer.
/// </summary>
public class DnsMessageTests
{
    private const ushort Id = 0x1234;
    private const string Question = "_minecraft._tcp.example.com";

    // The question name starts right after the 12-byte header, so "example.com" inside it sits at
    // 12 + 1+10 ("_minecraft") + 1+4 ("_tcp") = 28. Answers point there instead of repeating it.
    private const int SuffixOffset = 28;

    [Fact]
    public void WriteQuery_MatchesTheWire()
    {
        Span<byte> buffer = new byte[DnsMessage.MaxQueryLength];
        var length = DnsMessage.WriteQuery(buffer, Id, Question);

        var expected = new DnsWire()
            .Header(Id, DnsWire.FlagRecursionDesired, 1, 0)
            .Question(Question)
            .ToArray();

        Assert.Equal(expected.Length, length);
        Assert.Equal(expected, buffer[..length].ToArray());

        // Pins the constant the compression tests below lean on.
        Assert.Equal(7, expected[SuffixOffset]);
        Assert.Equal("example", Encoding.ASCII.GetString(expected, SuffixOffset + 1, 7));
    }

    /// <summary>
    ///     Both wire limits are refused before a byte is written, so an over-long name is a bad argument
    ///     and never an index off the end of the buffer.
    /// </summary>
    [Fact]
    public void WriteQuery_RefusesALabelOverSixtyThreeBytes()
    {
        var name = $"{SrvResolver.ServicePrefix}{new string('a', 64)}.com";
        var buffer = new byte[DnsMessage.MaxQueryLength];

        Assert.Throws<ArgumentException>(() => DnsMessage.WriteQuery(buffer, Id, name));
    }

    [Fact]
    public void WriteQuery_RefusesANameOverTwoHundredFiftyFiveBytes()
    {
        // Four legal labels: 4 * 64 plus the 16-byte prefix and the root byte is 273, over the limit.
        var label = new string('a', 63);
        var name = $"{SrvResolver.ServicePrefix}{label}.{label}.{label}.{label}";
        var buffer = new byte[DnsMessage.MaxQueryLength];

        var failure = Assert.Throws<ArgumentException>(() => DnsMessage.WriteQuery(buffer, Id, name));
        Assert.Contains("255", failure.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void WriteQuery_HandlesTheLongestNameThatFits()
    {
        // Labels of 63 plus the service prefix, filled right up to the 255-byte wire limit.
        var name = $"{SrvResolver.ServicePrefix}{new string('a', 63)}.{new string('b', 63)}.{new string('c', 63)}";
        var buffer = new byte[DnsMessage.MaxQueryLength];

        var length = DnsMessage.WriteQuery(buffer, Id, name);

        Assert.True(length <= DnsMessage.MaxQueryLength);
        Assert.True(DnsMessage.TryRead(
            new DnsWire().Header(Id, DnsWire.FlagResponse, 1, 0).Question(name).ToArray(), Id, out _));
    }

    [Fact]
    public void WriteQuery_IgnoresATrailingDot()
    {
        Span<byte> withDot = new byte[DnsMessage.MaxQueryLength];
        Span<byte> without = new byte[DnsMessage.MaxQueryLength];

        var a = DnsMessage.WriteQuery(withDot, Id, Question + ".");
        var b = DnsMessage.WriteQuery(without, Id, Question);

        Assert.Equal(b, a);
        Assert.Equal(without[..b].ToArray(), withDot[..a].ToArray());
    }

    /// <summary>A name may only point backwards; a forward pointer is a malformed message.</summary>
    [Fact]
    public void TryRead_RejectsAForwardPointer()
    {
        var wire = new DnsWire()
            .Header(Id, DnsWire.FlagResponse, 1, 1)
            .Question(Question);

        wire.Record(w => w.Pointer(DnsMessage.HeaderLength), DnsWire.TypeSrv, w =>
        {
            w.U16(0).U16(0).U16(25565);
            w.Pointer(w.Offset + 64);
        });

        Assert.True(DnsMessage.TryRead(wire.ToArray(), Id, out var response));
        Assert.Empty(response.Records);
    }

    /// <summary>A target with no terminator inside its own data must not bleed into the next record.</summary>
    [Fact]
    public void TryRead_KeepsATargetInsideItsOwnData()
    {
        var wire = new DnsWire()
            .Header(Id, DnsWire.FlagResponse, 1, 2)
            .Question(Question);

        // The first record's target runs out of data before its root byte.
        wire.Record(w => w.Pointer(DnsMessage.HeaderLength), DnsWire.TypeSrv,
            w => w.U16(0).U16(0).U16(25565).Labels("runaway"));

        wire.Record(w => w.Pointer(DnsMessage.HeaderLength), DnsWire.TypeSrv,
            w => w.U16(0).U16(0).U16(25566).Name("good.example.com"));

        SrvResult[] expected = [new("good.example.com", 25566, 0, 0)];

        Assert.True(DnsMessage.TryRead(wire.ToArray(), Id, out var response));
        Assert.Equal(expected, response.Records);
    }

    [Fact]
    public void TryRead_FollowsCompressionPointers()
    {
        var wire = new DnsWire()
            .Header(Id, DnsWire.FlagResponse | DnsWire.FlagRecursionAvailable, 1, 2)
            .Question(Question);

        // First answer: the owner name is a pointer, and the target is "mc" + a pointer to the suffix.
        wire.Record(w => w.Pointer(DnsMessage.HeaderLength), DnsWire.TypeSrv,
            w => w.U16(10).U16(5).U16(25566).Labels("mc").Pointer(SuffixOffset));

        // Second answer: the target is nothing but a pointer into the question.
        wire.Record(w => w.Pointer(DnsMessage.HeaderLength), DnsWire.TypeSrv,
            w => w.U16(5).U16(0).U16(25567).Pointer(SuffixOffset));

        Assert.True(DnsMessage.TryRead(wire.ToArray(), Id, out var response));
        Assert.False(response.Truncated);
        Assert.Equal(DnsMessage.CodeNoError, response.ResponseCode);

        SrvResult[] expected =
        [
            new("mc.example.com", 25566, 10, 5),
            new("example.com", 25567, 5, 0)
        ];
        Assert.Equal(expected, response.Records);
    }

    [Fact]
    public void TryRead_SkipsRecordsThatAreNotSrv()
    {
        var wire = new DnsWire()
            .Header(Id, DnsWire.FlagResponse, 1, 2)
            .Question(Question)
            .Record(w => w.Pointer(DnsMessage.HeaderLength), DnsWire.TypeCName,
                w => w.Name("alias.example.com"))
            .Record(w => w.Name("alias.example.com"), DnsWire.TypeSrv,
                w => w.U16(0).U16(0).U16(25565).Name("mc.example.com"));

        SrvResult[] expected = [new("mc.example.com", 25565, 0, 0)];

        Assert.True(DnsMessage.TryRead(wire.ToArray(), Id, out var response));
        Assert.Equal(expected, response.Records);
    }

    [Fact]
    public void TryRead_RootTargetMeansNoService()
    {
        var wire = new DnsWire()
            .Header(Id, DnsWire.FlagResponse, 1, 1)
            .Question(Question)
            .Record(w => w.Pointer(DnsMessage.HeaderLength), DnsWire.TypeSrv,
                w => w.U16(0).U16(0).U16(25565).Byte(0));

        Assert.True(DnsMessage.TryRead(wire.ToArray(), Id, out var response));
        Assert.Empty(response.Records);
    }

    [Fact]
    public void TryRead_ReportsTruncation()
    {
        var wire = new DnsWire()
            .Header(Id, DnsWire.FlagResponse | DnsWire.FlagTruncated, 1, 0)
            .Question(Question);

        Assert.True(DnsMessage.TryRead(wire.ToArray(), Id, out var response));
        Assert.True(response.Truncated);
        Assert.Empty(response.Records);
    }

    [Fact]
    public void TryRead_KeepsTheResponseCode()
    {
        var wire = new DnsWire()
            .Header(Id, (ushort)(DnsWire.FlagResponse | DnsMessage.CodeNameError), 1, 0)
            .Question(Question);

        Assert.True(DnsMessage.TryRead(wire.ToArray(), Id, out var response));
        Assert.Equal(DnsMessage.CodeNameError, response.ResponseCode);
        Assert.Empty(response.Records);
    }

    /// <summary>
    ///     The answer has to echo the question it answers. Names are case-insensitive on the wire, so a
    ///     resolver that echoes a different case is still answering us.
    /// </summary>
    [Fact]
    public void TryRead_AcceptsTheEchoedQuestionWhateverItsCase()
    {
        var asked = QuestionSection(Question);

        foreach (var echoed in new[] { Question, "_MINECRAFT._TCP.EXAMPLE.COM", "_Minecraft._Tcp.Example.Com" })
        {
            var wire = new DnsWire()
                .Header(Id, DnsWire.FlagResponse, 1, 0)
                .Question(echoed)
                .ToArray();

            Assert.True(DnsMessage.TryRead(wire, Id, asked, out _), $"'{echoed}' should be our question.");
        }
    }

    [Fact]
    public void TryRead_RejectsAnAnswerToADifferentQuestion()
    {
        var asked = QuestionSection(Question);

        var wire = new DnsWire()
            .Header(Id, DnsWire.FlagResponse, 1, 1)
            .Question("_minecraft._tcp.attacker.example")
            .Record(w => w.Pointer(DnsMessage.HeaderLength), DnsWire.TypeSrv,
                w => w.U16(0).U16(0).U16(31337).Name("evil.example"))
            .ToArray();

        Assert.False(DnsMessage.TryRead(wire, Id, asked, out _));

        // Without the question to compare against, the same bytes parse: the check is what turns it away.
        Assert.True(DnsMessage.TryRead(wire, Id, out var unchecked_));
        Assert.Single(unchecked_.Records);
    }

    [Fact]
    public void TryRead_RejectsAnAnswerCarryingMoreThanOneQuestion()
    {
        var asked = QuestionSection(Question);

        var wire = new DnsWire()
            .Header(Id, DnsWire.FlagResponse, 2, 0)
            .Question(Question)
            .Question(Question)
            .ToArray();

        Assert.False(DnsMessage.TryRead(wire, Id, asked, out _));
    }

    /// <summary>The question section as the resolver sends it: the query without its 12-byte header.</summary>
    private static byte[] QuestionSection(string name)
    {
        var buffer = new byte[DnsMessage.MaxQueryLength];
        var length = DnsMessage.WriteQuery(buffer, Id, name);
        return buffer[DnsMessage.HeaderLength..length];
    }

    [Fact]
    public void TryRead_RejectsAnAnswerToSomebodyElsesQuestion()
    {
        var wire = new DnsWire()
            .Header(0x4321, DnsWire.FlagResponse, 1, 0)
            .Question(Question);

        Assert.False(DnsMessage.TryRead(wire.ToArray(), Id, out _));
    }

    [Fact]
    public void TryRead_RejectsAQueryPretendingToBeAnAnswer()
    {
        var wire = new DnsWire()
            .Header(Id, DnsWire.FlagRecursionDesired, 1, 0)
            .Question(Question);

        Assert.False(DnsMessage.TryRead(wire.ToArray(), Id, out _));
    }

    [Fact]
    public void TryRead_RejectsAMessageCutShort()
    {
        var wire = new DnsWire()
            .Header(Id, DnsWire.FlagResponse, 1, 1)
            .Question(Question)
            .Record(w => w.Pointer(DnsMessage.HeaderLength), DnsWire.TypeSrv,
                w => w.U16(0).U16(0).U16(25565).Name("mc.example.com"))
            .ToArray();

        Assert.False(DnsMessage.TryRead(wire.AsSpan(0, wire.Length - 4), Id, out _));
        Assert.False(DnsMessage.TryRead(wire.AsSpan(0, DnsMessage.HeaderLength - 1), Id, out _));
    }

    /// <summary>
    ///     A resolver answer is bytes from the network. Whatever they say, the parser returns — it does
    ///     not throw, and it does not walk off the end of the buffer.
    /// </summary>
    [Fact]
    public void TryRead_SurvivesGarbage()
    {
        var random = new Random(20260821);
        var buffer = new byte[512];

        for (var round = 0; round < 20_000; round++)
        {
            random.NextBytes(buffer);

            // Half the rounds keep the header plausible, so the fuzz reaches the record loop.
            var length = random.Next(1, buffer.Length);
            if (round % 2 == 0 && length >= DnsMessage.HeaderLength)
            {
                buffer[0] = Id >> 8;
                buffer[1] = Id & 0xFF;
                buffer[2] = 0x81;
                buffer[3] = 0x80;
                buffer[4] = 0;
                buffer[5] = 1;
                buffer[6] = 0;
                buffer[7] = (byte)random.Next(0, 4);
            }

            if (DnsMessage.TryRead(buffer.AsSpan(0, length), Id, out var response))
                Assert.All(response.Records, record => Assert.InRange(record.Port, 1, 65535));
        }
    }

    /// <summary>A pointer that walks back onto itself is a malformed name, not an endless loop.</summary>
    [Fact]
    public void TryRead_SurvivesAPointerLoop()
    {
        var wire = new DnsWire()
            .Header(Id, DnsWire.FlagResponse, 1, 1)
            .Question(Question);

        // The target of this record is a pointer to the two bytes that make up the pointer.
        wire.Record(w => w.Pointer(DnsMessage.HeaderLength), DnsWire.TypeSrv, w =>
        {
            w.U16(0).U16(0).U16(25565);
            w.Pointer(w.Offset);
        });

        Assert.True(DnsMessage.TryRead(wire.ToArray(), Id, out var response));
        Assert.Empty(response.Records);
    }
}
