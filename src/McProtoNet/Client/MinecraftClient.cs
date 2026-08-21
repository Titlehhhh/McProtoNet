using System.Net.Sockets;
using McProtoNet.Transport;

namespace McProtoNet;

/// <summary>
///     The standard client: options plus a TCP connect that yields a
///     <see cref="MinecraftConnection" /> in one-at-a-time mode. Everything above raw packets —
///     handshake, login, phases, the move to streaming — is consumer code (see examples/).
/// </summary>
public sealed class MinecraftClient
{
    public MinecraftClient(MinecraftClientOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        Options = options;
    }

    public MinecraftClientOptions Options { get; }

    /// <summary>Opens the TCP connection and wraps it. No packets are sent.</summary>
    public async ValueTask<MinecraftConnection> ConnectAsync(CancellationToken cancellationToken = default)
    {
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

        return new MinecraftConnection(tcp.GetStream());
    }
}
