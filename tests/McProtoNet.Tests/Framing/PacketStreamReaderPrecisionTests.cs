using McProtoNet.Tests.Infrastructure;
using McProtoNet.Transport.Framing;

namespace McProtoNet.Tests.Framing;

/// <summary>
///     The one-at-a-time reader must never pull a byte past the frame it returns — that precision is
///     what makes a cipher or threshold switch land between two frames.
/// </summary>
public class PacketStreamReaderPrecisionTests
{
    public static TheoryData<int, bool> Modes => new()
    {
        { -1, false }, { -1, true },
        { 0, false }, { 0, true },
        { 256, false }, { 256, true }
    };

    [Theory]
    [MemberData(nameof(Modes))]
    public async Task StreamPositionStopsExactlyAfterEachFrame(int threshold, bool encrypted)
    {
        var token = TestContext.Current.CancellationToken;
        var packets = Frames.Sample(seed: 3, repeats: 1);
        var wire = Frames.Build(packets, threshold, encrypted);
        var ends = Frames.FrameEnds(packets, threshold);

        var stream = new MemoryStream(wire, writable: false);
        using var cipher = Frames.Decryptor(encrypted);
        using var reader = new PacketStreamReader(stream, leaveOpen: true)
        {
            CompressionThreshold = threshold,
            Cipher = cipher
        };

        for (var i = 0; i < packets.Count; i++)
        {
            var packet = await reader.ReadPacketAsync(token);

            Assert.Equal(packets[i].Id, packet.Id);
            Assert.Equal(packets[i].Body, packet.Body.ToArray());
            Assert.Equal(ends[i], stream.Position);
        }

        Assert.Equal(wire.Length, stream.Position);
    }

    [Fact]
    public async Task SwitchesLandBetweenFrames()
    {
        var token = TestContext.Current.CancellationToken;

        // three frames on one stream: plain, then encrypted, then encrypted and compressed —
        // exactly the shape a login takes
        var plain = new TestPacket(1, "hello"u8.ToArray());
        var afterCipher = new TestPacket(2, new byte[300]);
        var afterThreshold = new TestPacket(3, new byte[900]);
        Random.Shared.NextBytes(afterCipher.Body);
        Random.Shared.NextBytes(afterThreshold.Body);

        var wire = new MemoryStream();
        wire.Write(Frames.Build([plain], -1, false));

        using var encryptor = Crypto.CreateEncryptor();
        var second = Frames.Build([afterCipher], -1, false);
        encryptor.Transform(second);
        wire.Write(second);

        var third = Frames.Build([afterThreshold], 256, false);
        encryptor.Transform(third);
        wire.Write(third);

        wire.Position = 0;
        using var reader = new PacketStreamReader(wire, leaveOpen: true);

        var first = await reader.ReadPacketAsync(token);
        Assert.Equal(plain.Body, first.Body.ToArray());

        using var decryptor = Crypto.CreateDecryptor();
        reader.Cipher = decryptor;
        var next = await reader.ReadPacketAsync(token);
        Assert.Equal(afterCipher.Body, next.Body.ToArray());

        reader.CompressionThreshold = 256;
        var last = await reader.ReadPacketAsync(token);
        Assert.Equal(afterThreshold.Body, last.Body.ToArray());
        Assert.Equal(wire.Length, wire.Position);
    }
}
