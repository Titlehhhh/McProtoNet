using System.Net.Sockets;
using McProtoNet.Net;

namespace McProtoNet;

/// <summary>
///     The standard client: a TCP connection with Minecraft packet framing.
///     Connects, sends and receives packets; compression and encryption switches
///     pass through. Everything above raw packets — handshake, login, phases —
///     is consumer code (see examples/).
/// </summary>
public sealed class MinecraftClient : IDisposable, IAsyncDisposable
{
    private MinecraftConnection? _connection;

    public MinecraftClient(MinecraftClientOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        Options = options;
    }

    public MinecraftClientOptions Options { get; }

    /// <summary>The transport underneath — the low-level escape hatch.</summary>
    public MinecraftConnection Connection =>
        _connection ?? throw new InvalidOperationException("Not connected");

    /// <summary>Completes when the connection has shut down. Never faults.</summary>
    public Task Completion => Connection.Completion;

    public bool IsConnected => _connection is not null;

    public int CompressionThreshold
    {
        get => Connection.CompressionThreshold;
        set => Connection.CompressionThreshold = value;
    }

    public bool EncryptionEnabled => Connection.PacketReader.EncryptionEnabled;

    /// <summary>Opens the TCP connection. No packets are sent.</summary>
    public async ValueTask ConnectAsync(CancellationToken cancellationToken = default)
    {
        if (_connection is not null)
            throw new InvalidOperationException("Already connected");

        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeout.CancelAfter(Options.ConnectTimeout);

        var tcp = new TcpClient { NoDelay = true };
        try
        {
            await tcp.ConnectAsync(Options.Host, Options.Port, timeout.Token).ConfigureAwait(false);
        }
        catch
        {
            tcp.Dispose();
            throw;
        }

        _connection = MinecraftConnection.Create(tcp.GetStream());
    }

    /// <summary>Enables AES/CFB8 encryption on both directions from the next frame on.</summary>
    public void EnableEncryption(ReadOnlySpan<byte> sharedSecret)
    {
        var connection = Connection;
        connection.PacketReader.EnableEncryption(sharedSecret);
        connection.PacketWriter.EnableEncryption(sharedSecret);
    }

    public IAsyncEnumerable<InputPacket> ReadPacketsAsync(CancellationToken cancellationToken = default)
        => Connection.ReadPacketsAsync(cancellationToken);

    public ValueTask<InputPacket> ReadPacketAsync(CancellationToken cancellationToken = default)
        => Connection.ReadPacketAsync(cancellationToken);

    /// <summary>Sends one packet: varint id plus body already assembled by the caller.</summary>
    public ValueTask SendPacketAsync(ReadOnlyMemory<byte> packet, CancellationToken cancellationToken = default)
        => Connection.SendPacketAsync(packet, cancellationToken);

    public void Dispose() => _connection?.Dispose();

    public async ValueTask DisposeAsync()
    {
        if (_connection is not null)
        {
            await _connection.DisposeAsync().ConfigureAwait(false);
        }
    }
}
