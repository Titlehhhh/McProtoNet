using McProtoNet.Primitives;
using McProtoNet.Protocol;
namespace McProtoNet.Tests.Protocol;

/// <summary>
///     The generated handler bases decode a raw packet and call the matching <c>On&lt;Name&gt;</c>
///     without a visitor in between. These tests pin the behaviour that route has to keep: the
///     consumer-owned phase decides how an id is read, an unmapped id is a normal condition that
///     reaches <c>OnUnknown</c>, a body the spec did not fully consume raises the trailing-bytes
///     hook rather than throwing, and an exception from consumer code comes out as itself.
/// </summary>
public class HandlerDispatchTests
{
    private const int Pv = 772;

    private static IncomingPacket Encode<T>(T packet, int pv = Pv, byte[]? extra = null) where T : class, IPacket<T>
    {
        Assert.True(T.TryGetPacketId(pv, out var id));

        var writer = new MinecraftPrimitiveWriter();
        packet.Write(writer, pv);
        using var written = writer.GetWrittenMemory();
        var body = written.Memory.ToArray();

        if (extra is { Length: > 0 })
            body = [.. body, .. extra];

        return new IncomingPacket(id, body);
    }

    private sealed class RecordingHandler : ClientboundHandler
    {
        public readonly List<string> Calls = [];
        public long KeepAliveId;
        public int UnknownId = -1;
        public Func<ValueTask>? OnKeepAliveOverride;

        public RecordingHandler(PacketPhase phase) => Phase = phase;

        public void Advance(PacketPhase phase) => Phase = phase;

        protected override ValueTask OnKeepAlive(McProtoNet.Protocol.Packets.Play.Clientbound.KeepAlivePacket packet)
        {
            Calls.Add(nameof(OnKeepAlive));
            KeepAliveId = packet.KeepAliveId;
            return OnKeepAliveOverride?.Invoke() ?? default;
        }

        protected override ValueTask OnRelEntityMove(McProtoNet.Protocol.Packets.Play.Clientbound.RelEntityMovePacket packet)
        {
            Calls.Add(nameof(OnRelEntityMove));
            return default;
        }

        protected override ValueTask OnLoginCompress(McProtoNet.Protocol.Packets.Login.Clientbound.LoginCompressPacket packet)
        {
            Calls.Add(nameof(OnLoginCompress));
            return default;
        }

        protected override ValueTask OnUnknown(in IncomingPacket raw)
        {
            Calls.Add(nameof(OnUnknown));
            UnknownId = raw.Id;
            return default;
        }
    }

    private sealed class RecordingServerHandler : ServerboundHandler
    {
        public readonly List<string> Calls = [];
        public string Host = string.Empty;

        public RecordingServerHandler(PacketPhase phase) => Phase = phase;

        protected override ValueTask OnSetProtocol(McProtoNet.Protocol.Packets.Handshaking.Serverbound.SetProtocolPacket packet)
        {
            Calls.Add(nameof(OnSetProtocol));
            Host = packet.ServerHost;
            return default;
        }

        protected override ValueTask OnUnknown(in IncomingPacket raw)
        {
            Calls.Add(nameof(OnUnknown));
            return default;
        }
    }

    [Fact]
    public async Task HandleAsync_KnownPacket_ReachesTheMatchingHandlerWithTheDecodedValue()
    {
        var handler = new RecordingHandler(PacketPhase.Play);
        var raw = Encode(new McProtoNet.Protocol.Packets.Play.Clientbound.KeepAlivePacket(0x0123456789ABCDEF));

        await handler.HandleAsync(in raw, Pv);

        Assert.Equal(["OnKeepAlive"], handler.Calls);
        Assert.Equal(0x0123456789ABCDEF, handler.KeepAliveId);
    }

    [Fact]
    public async Task HandleAsync_UnmappedId_ReachesOnUnknown()
    {
        var handler = new RecordingHandler(PacketPhase.Play);
        var raw = new IncomingPacket(0x7E, Array.Empty<byte>());

        await handler.HandleAsync(in raw, Pv);

        Assert.Equal(["OnUnknown"], handler.Calls);
        Assert.Equal(0x7E, handler.UnknownId);
    }

    [Fact]
    public async Task HandleAsync_TheConsumerOwnedPhaseDecidesHowAnIdIsRead()
    {
        // The same wire id means different packets in different phases; the handler must read the
        // one its Phase says, not the one the id happens to match in Play.
        var login = new RecordingHandler(PacketPhase.Login);
        var raw = Encode(new McProtoNet.Protocol.Packets.Login.Clientbound.LoginCompressPacket(256));

        await login.HandleAsync(in raw, Pv);

        Assert.Equal(["OnLoginCompress"], login.Calls);

        // The same id in Play is a different packet entirely, so the Play handler reads those
        // bytes as that packet — here it runs off the end of the short body. Either way the one
        // thing that must not happen is the Login handler method being called.
        var play = new RecordingHandler(PacketPhase.Play);
        try
        {
            await play.HandleAsync(in raw, Pv);
        }
        catch (Exception e) when (e is not Xunit.Sdk.XunitException)
        {
        }

        Assert.DoesNotContain("OnLoginCompress", play.Calls);
    }

    [Fact]
    public async Task HandleAsync_PhaseChangedBetweenCalls_UsesTheNewPhase()
    {
        var handler = new RecordingHandler(PacketPhase.Login);
        var compress = Encode(new McProtoNet.Protocol.Packets.Login.Clientbound.LoginCompressPacket(256));
        await handler.HandleAsync(in compress, Pv);

        handler.Advance(PacketPhase.Play);
        var keepAlive = Encode(new McProtoNet.Protocol.Packets.Play.Clientbound.KeepAlivePacket(7));
        await handler.HandleAsync(in keepAlive, Pv);

        Assert.Equal(["OnLoginCompress", "OnKeepAlive"], handler.Calls);
        Assert.Equal(7, handler.KeepAliveId);
    }

    [Fact]
    public async Task HandleAsync_ProtocolVersionOutsideTheKnownRange_ReachesOnUnknown()
    {
        var handler = new RecordingHandler(PacketPhase.Play);
        var raw = Encode(new McProtoNet.Protocol.Packets.Play.Clientbound.KeepAlivePacket(1));

        await handler.HandleAsync(in raw, 1);

        Assert.Equal(["OnUnknown"], handler.Calls);
    }

    [Fact]
    public async Task HandleAsync_BodyWithTrailingBytes_RaisesTheHookAndStillDelivers()
    {
        var handler = new RecordingHandler(PacketPhase.Play);
        var raw = Encode(new McProtoNet.Protocol.Packets.Play.Clientbound.KeepAlivePacket(42), extra: [1, 2, 3]);

        long remaining = 0;
        var seen = false;

        void Hook(int packetId, int protocolVersion, long remainingBytes)
        {
            if (packetId != raw.Id)
                return;

            seen = true;
            remaining = remainingBytes;
        }

        PacketFlow.OnTrailingBytes += Hook;
        try
        {
            await handler.HandleAsync(in raw, Pv);
        }
        finally
        {
            PacketFlow.OnTrailingBytes -= Hook;
        }

        Assert.Equal(["OnKeepAlive"], handler.Calls);
        Assert.Equal(42, handler.KeepAliveId);
        Assert.True(seen, "the trailing-bytes hook did not fire");
        Assert.Equal(3, remaining);
    }

    [Fact]
    public async Task HandleAsync_ExactBody_DoesNotRaiseTheTrailingBytesHook()
    {
        var handler = new RecordingHandler(PacketPhase.Play);
        var raw = Encode(new McProtoNet.Protocol.Packets.Play.Clientbound.KeepAlivePacket(42));

        var seen = false;

        void Hook(int packetId, int protocolVersion, long remainingBytes)
        {
            if (packetId == raw.Id)
                seen = true;
        }

        PacketFlow.OnTrailingBytes += Hook;
        try
        {
            await handler.HandleAsync(in raw, Pv);
        }
        finally
        {
            PacketFlow.OnTrailingBytes -= Hook;
        }

        Assert.False(seen, "the hook fired for a body that was read exactly");
    }

    [Fact]
    public async Task HandleAsync_ConsumerThrows_TheExceptionComesOutAsItself()
    {
        var handler = new RecordingHandler(PacketPhase.Play)
        {
            OnKeepAliveOverride = () => throw new InvalidTimeZoneException("from the consumer")
        };

        var raw = Encode(new McProtoNet.Protocol.Packets.Play.Clientbound.KeepAlivePacket(1));

        var thrown = await Assert.ThrowsAsync<InvalidTimeZoneException>(
            async () => await handler.HandleAsync(in raw, Pv));

        Assert.Equal("from the consumer", thrown.Message);
    }

    [Fact]
    public async Task HandleAsync_MalformedBody_Throws()
    {
        var handler = new RecordingHandler(PacketPhase.Play);
        Assert.True(McProtoNet.Protocol.Packets.Play.Clientbound.KeepAlivePacket.TryGetPacketId(Pv, out var id));
        var raw = new IncomingPacket(id, new byte[2]);

        await Assert.ThrowsAnyAsync<Exception>(async () => await handler.HandleAsync(in raw, Pv));
    }

    [Fact]
    public async Task ServerboundHandler_DispatchesItsOwnDirection()
    {
        var handler = new RecordingServerHandler(PacketPhase.Handshaking);
        var raw = Encode(new McProtoNet.Protocol.Packets.Handshaking.Serverbound.SetProtocolPacket(Pv, "example.org", 25565, 2));

        await handler.HandleAsync(in raw, Pv);

        Assert.Equal(["OnSetProtocol"], handler.Calls);
        Assert.Equal("example.org", handler.Host);
    }

    /// <summary>A handler that never touches Phase, so the assertion is about the generated
    /// default and not about what the constructor was handed.</summary>
    private sealed class DefaultPhaseServerHandler : ServerboundHandler;

    private sealed class DefaultPhaseClientHandler : ClientboundHandler;

    [Fact]
    public void ServerboundHandler_DefaultsToHandshaking()
    {
        Assert.Equal(PacketPhase.Handshaking, new DefaultPhaseServerHandler().Phase);
    }

    [Fact]
    public void ClientboundHandler_DefaultsToLogin()
    {
        Assert.Equal(PacketPhase.Login, new DefaultPhaseClientHandler().Phase);
    }

    [Fact]
    public async Task HandleAsync_HandlerThatAdvancesThePhase_ReadsTheNextPacketWithTheNewPhase()
    {
        // The realistic sequence: a login packet's own handler moves the connection to Play, and
        // the very next packet must already be read as Play. Nothing else touches Phase.
        var handler = new PhaseAdvancingHandler();
        var compress = Encode(new McProtoNet.Protocol.Packets.Login.Clientbound.LoginCompressPacket(256));
        await handler.HandleAsync(in compress, Pv);

        Assert.Equal(PacketPhase.Play, handler.Phase);

        var keepAlive = Encode(new McProtoNet.Protocol.Packets.Play.Clientbound.KeepAlivePacket(99));
        await handler.HandleAsync(in keepAlive, Pv);

        Assert.Equal(["OnLoginCompress", "OnKeepAlive"], handler.Calls);
        Assert.Equal(99, handler.KeepAliveId);
    }

    private sealed class PhaseAdvancingHandler : ClientboundHandler
    {
        public readonly List<string> Calls = [];
        public long KeepAliveId;

        protected override ValueTask OnLoginCompress(
            McProtoNet.Protocol.Packets.Login.Clientbound.LoginCompressPacket packet)
        {
            Calls.Add(nameof(OnLoginCompress));
            Phase = PacketPhase.Play;
            return default;
        }

        protected override ValueTask OnKeepAlive(McProtoNet.Protocol.Packets.Play.Clientbound.KeepAlivePacket packet)
        {
            Calls.Add(nameof(OnKeepAlive));
            KeepAliveId = packet.KeepAliveId;
            return default;
        }

        protected override ValueTask OnUnknown(in IncomingPacket raw)
        {
            Calls.Add(nameof(OnUnknown));
            return default;
        }
    }

    [Theory]
    [InlineData(5)]
    [InlineData(200)]
    public async Task HandleAsync_PhaseOutsideTheEnum_ReachesOnUnknownInsteadOfMisreading(byte phase)
    {
        var handler = new RecordingHandler((PacketPhase)phase);
        var raw = Encode(new McProtoNet.Protocol.Packets.Play.Clientbound.KeepAlivePacket(1));

        await handler.HandleAsync(in raw, Pv);

        Assert.Equal(["OnUnknown"], handler.Calls);
    }
}
