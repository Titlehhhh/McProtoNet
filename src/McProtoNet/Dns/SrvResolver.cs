using System.Buffers;
using System.Buffers.Binary;
using System.Globalization;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Security.Cryptography;

namespace McProtoNet;

/// <summary>
/// Provides the <c>_minecraft._tcp</c> SRV lookup that the vanilla client performs before connecting.
/// </summary>
/// <remarks>
/// <para>
/// A host that publishes no record is not a failure: <see cref="ResolveAsync(string, CancellationToken)"/>
/// returns <see langword="null"/>. Only a lookup that no configured DNS server answered, or that the
/// caller canceled, raises an exception.
/// </para>
/// <para>
/// The whole lookup runs on one budget, and each configured server gets an equal slice of it. The query
/// is built and parsed in this library (RFC 1035 and RFC 2782) and sent over UDP, with a TCP retry when
/// the answer does not fit a datagram.
/// </para>
/// </remarks>
public static class SrvResolver
{
    /// <summary>The service label Minecraft servers publish under.</summary>
    public const string ServicePrefix = "_minecraft._tcp.";

    /// <summary>How long the whole lookup may take, across every configured server.</summary>
    internal static readonly TimeSpan DefaultBudget = TimeSpan.FromSeconds(5);

    /// <summary>The smallest slice of the budget any one server gets.</summary>
    internal static readonly TimeSpan MinimumSlice = TimeSpan.FromSeconds(1);

    private const int DnsPort = 53;
    private const int UdpAnswerLimit = 4096;

    // Datagrams that do not answer the question before this server is abandoned for the next one.
    private const int MaxStrayDatagrams = 32;

    private static readonly TimeSpan ServerCacheLifetime = TimeSpan.FromSeconds(30);
    private static readonly object CacheGate = new();
    private static List<IPEndPoint>? _cachedServers;
    private static long _cachedAt;

    /// <summary>
    /// Asynchronously looks up the SRV records of the specified host and selects one of them.
    /// </summary>
    /// <param name="host">The host name, without the service prefix.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is
    /// <see cref="CancellationToken.None"/>.</param>
    /// <returns>A task that represents the asynchronous lookup operation. The result contains the record
    /// to connect to, or <see langword="null"/> if the host publishes no SRV record, if a record says "no
    /// service", or if the machine has no DNS server configured.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="host"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="host"/> is empty or consists only of white-space
    /// characters.</exception>
    /// <exception cref="IOException">No configured DNS server produced an answer.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is
    /// stored into the returned task.</exception>
    /// <remarks>
    /// Selection follows RFC 2782: the lowest priority wins, and records inside that group are drawn by
    /// weight.
    /// </remarks>
    public static async ValueTask<SrvResult?> ResolveAsync(string host, CancellationToken cancellationToken = default)
    {
        var records = await ResolveAllAsync(host, cancellationToken).ConfigureAwait(false);
        return Select(records, Random.Shared.Next);
    }

    /// <summary>
    /// Asynchronously looks up every SRV record published for the specified host.
    /// </summary>
    /// <param name="host">The host name, without the service prefix.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is
    /// <see cref="CancellationToken.None"/>.</param>
    /// <returns>A task that represents the asynchronous lookup operation. The result contains the records
    /// in the order the resolver returned them, and is empty if the host publishes no SRV record.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="host"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="host"/> is empty or consists only of white-space
    /// characters.</exception>
    /// <exception cref="IOException">No configured DNS server produced an answer.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is
    /// stored into the returned task.</exception>
    public static ValueTask<IReadOnlyList<SrvResult>> ResolveAllAsync(string host,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(host);
        return ResolveAllAsync(host, GetSystemDnsServers(), DefaultBudget, cancellationToken);
    }

    /// <summary>Runs the same lookup against the specified DNS servers.</summary>
    internal static async ValueTask<SrvResult?> ResolveAsync(string host, IReadOnlyList<IPEndPoint> servers,
        CancellationToken cancellationToken)
    {
        var records = await ResolveAllAsync(host, servers, DefaultBudget, cancellationToken).ConfigureAwait(false);
        return Select(records, Random.Shared.Next);
    }

    /// <summary>Runs the same full-list lookup against the specified DNS servers and budget.</summary>
    internal static async ValueTask<IReadOnlyList<SrvResult>> ResolveAllAsync(string host,
        IReadOnlyList<IPEndPoint> servers, TimeSpan budget, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(host);
        ArgumentNullException.ThrowIfNull(servers);

        // No resolver to ask is the same answer as no record, on both the public and the internal path.
        if (servers.Count == 0) return Array.Empty<SrvResult>();

        // The question is written once and reused for every server; only the transaction id differs, and
        // these same bytes are what the answer has to echo back.
        var name = ServicePrefix + ToAscii(host.TrimEnd('.'));
        var message = new byte[DnsMessage.MaxQueryLength];
        var messageLength = DnsMessage.WriteQuery(message, 0, name);
        var question = message.AsMemory(DnsMessage.HeaderLength, messageLength - DnsMessage.HeaderLength);

        using var overall = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        overall.CancelAfter(budget);

        var slice = TimeSpan.FromTicks(Math.Max(MinimumSlice.Ticks, budget.Ticks / servers.Count));
        Exception? failure = null;

        foreach (var server in servers)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (overall.IsCancellationRequested) break;

            DnsResponse answer;
            try
            {
                answer = await QueryAsync(server, question, slice, overall.Token).ConfigureAwait(false);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex) when (ex is SocketException or IOException or OperationCanceledException)
            {
                failure = ex;
                continue;
            }

            if (answer.ResponseCode is DnsMessage.CodeNoError or DnsMessage.CodeNameError) return answer.Records;

            failure = new IOException(
                $"DNS server {server} answered '{name}' with response code {answer.ResponseCode.ToString(CultureInfo.InvariantCulture)}.");
        }

        // Losing the race to the caller's own cancellation must not read as a lookup failure.
        cancellationToken.ThrowIfCancellationRequested();
        throw new IOException($"SRV lookup for '{name}' failed: no DNS server answered.", failure);
    }

    /// <summary>Selects one record per RFC 2782: lowest priority first, then a weighted draw inside that
    /// group.</summary>
    internal static SrvResult? Select(IReadOnlyList<SrvResult> records, Func<int, int> roll)
    {
        ArgumentNullException.ThrowIfNull(records);
        ArgumentNullException.ThrowIfNull(roll);
        if (records.Count == 0) return null;

        var best = ushort.MaxValue;
        foreach (var record in records)
            if (record.Priority < best)
                best = record.Priority;

        // RFC 2782 orders weight-0 records first, which is what gives them their small share of the draw.
        var group = new List<SrvResult>(records.Count);
        foreach (var record in records)
            if (record.Priority == best && record.Weight == 0)
                group.Add(record);

        var total = 0;
        foreach (var record in records)
            if (record.Priority == best && record.Weight != 0)
            {
                group.Add(record);
                total += record.Weight;
            }

        if (group.Count == 1) return group[0];
        if (total == 0) return group[roll(group.Count)];

        var point = roll(total + 1);
        var running = 0;
        foreach (var record in group)
        {
            running += record.Weight;
            if (running >= point) return record;
        }

        return group[^1];
    }

    /// <summary>Gets the DNS servers the operating system is configured with, IPv4 first, from a
    /// short-lived cache.</summary>
    internal static List<IPEndPoint> GetSystemDnsServers()
    {
        var now = Environment.TickCount64;
        lock (CacheGate)
        {
            if (_cachedServers is { } cached && now - _cachedAt < (long)ServerCacheLifetime.TotalMilliseconds)
                return cached;

            var servers = ReadSystemDnsServers();
            _cachedServers = servers;
            _cachedAt = now;
            return servers;
        }
    }

    /// <summary>Clears the cached resolver list, so the next lookup reads the interfaces again.</summary>
    internal static void ClearServerCache()
    {
        lock (CacheGate)
        {
            _cachedServers = null;
        }
    }

    private static List<IPEndPoint> ReadSystemDnsServers()
    {
        var v4 = new List<IPAddress>();
        var v6 = new List<IPAddress>();

        foreach (var adapter in NetworkInterface.GetAllNetworkInterfaces())
        {
            if (adapter.OperationalStatus != OperationalStatus.Up) continue;
            if (adapter.NetworkInterfaceType == NetworkInterfaceType.Loopback) continue;

            IPAddressCollection addresses;
            try
            {
                addresses = adapter.GetIPProperties().DnsAddresses;
            }
            catch (NetworkInformationException)
            {
                continue;
            }

            foreach (var address in addresses)
            {
                if (address.AddressFamily == AddressFamily.InterNetwork)
                {
                    if (!v4.Contains(address)) v4.Add(address);
                    continue;
                }

                // fec0::/10 is where Windows parks its placeholder resolvers; nothing answers there.
                if (address.IsIPv6SiteLocal) continue;

                // A link-local resolver without a scope is unreachable: nothing tells us which link.
                if (address.IsIPv6LinkLocal && address.ScopeId == 0) continue;
                if (!v6.Contains(address)) v6.Add(address);
            }
        }

        v4.AddRange(v6);
        return [.. v4.Select(address => new IPEndPoint(address, DnsPort))];
    }

    // Punycode, so a host in a non-ASCII alphabet reaches the wire as the labels DNS carries.
    private static string ToAscii(string host)
    {
        foreach (var c in host)
            if (c > 0x7F)
                return new IdnMapping { AllowUnassigned = true }.GetAscii(host);

        return host;
    }

    // One slice of the budget for the datagram, and a fresh slice for the TCP retry when the answer was
    // truncated, so a truncated answer is not cut short by the time UDP already used.
    private static async ValueTask<DnsResponse> QueryAsync(IPEndPoint server, ReadOnlyMemory<byte> question,
        TimeSpan slice, CancellationToken overallToken)
    {
        // The transaction id is half of what keeps an off-path answer out (the connected socket's random
        // source port is the other half), so it comes from the cryptographic generator, not Random.Shared.
        var id = (ushort)RandomNumberGenerator.GetInt32(0, ushort.MaxValue + 1);

        var query = ArrayPool<byte>.Shared.Rent(DnsMessage.HeaderLength + question.Length);
        try
        {
            BinaryPrimitives.WriteUInt16BigEndian(query, id);
            BinaryPrimitives.WriteUInt16BigEndian(query.AsSpan(2), 0x0100);
            BinaryPrimitives.WriteUInt16BigEndian(query.AsSpan(4), 1);
            query.AsSpan(6, 6).Clear();
            question.Span.CopyTo(query.AsSpan(DnsMessage.HeaderLength));

            var length = DnsMessage.HeaderLength + question.Length;

            DnsResponse answer;
            using (var udpDeadline = CancellationTokenSource.CreateLinkedTokenSource(overallToken))
            {
                udpDeadline.CancelAfter(slice);
                answer = await QueryOverUdpAsync(server, query.AsMemory(0, length), id, question, udpDeadline.Token)
                    .ConfigureAwait(false);
            }

            if (!answer.Truncated) return answer;

            using (var tcpDeadline = CancellationTokenSource.CreateLinkedTokenSource(overallToken))
            {
                tcpDeadline.CancelAfter(slice);
                return await QueryOverTcpAsync(server, query.AsMemory(0, length), id, question, tcpDeadline.Token)
                    .ConfigureAwait(false);
            }
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(query);
        }
    }

    private static async ValueTask<DnsResponse> QueryOverUdpAsync(IPEndPoint server, ReadOnlyMemory<byte> query,
        ushort id, ReadOnlyMemory<byte> question, CancellationToken cancellationToken)
    {
        using var socket = new Socket(server.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
        await socket.ConnectAsync(server, cancellationToken).ConfigureAwait(false);
        await socket.SendAsync(query, SocketFlags.None, cancellationToken).ConfigureAwait(false);

        var buffer = ArrayPool<byte>.Shared.Rent(UdpAnswerLimit);
        try
        {
            // Anything that is not the answer to our question is somebody else's datagram — but a peer
            // that only ever sends those must not hold the slice open, so the patience is finite.
            for (var stray = 0; stray < MaxStrayDatagrams; stray++)
            {
                var read = await socket.ReceiveAsync(buffer, SocketFlags.None, cancellationToken)
                    .ConfigureAwait(false);
                if (DnsMessage.TryRead(buffer.AsSpan(0, read), id, question.Span, out var answer)) return answer;
            }

            throw new IOException(
                $"DNS server {server} sent {MaxStrayDatagrams.ToString(CultureInfo.InvariantCulture)} datagrams that did not answer the question.");
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }
    }

    private static async ValueTask<DnsResponse> QueryOverTcpAsync(IPEndPoint server, ReadOnlyMemory<byte> query,
        ushort id, ReadOnlyMemory<byte> question, CancellationToken cancellationToken)
    {
        using var socket = new Socket(server.AddressFamily, SocketType.Stream, ProtocolType.Tcp) { NoDelay = true };
        await socket.ConnectAsync(server, cancellationToken).ConfigureAwait(false);

        var framed = ArrayPool<byte>.Shared.Rent(query.Length + 2);
        try
        {
            BinaryPrimitives.WriteUInt16BigEndian(framed, (ushort)query.Length);
            query.Span.CopyTo(framed.AsSpan(2));
            await socket.SendAsync(framed.AsMemory(0, query.Length + 2), SocketFlags.None, cancellationToken)
                .ConfigureAwait(false);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(framed);
        }

        var header = new byte[2];
        await ReceiveExactlyAsync(socket, header, cancellationToken).ConfigureAwait(false);

        int length = BinaryPrimitives.ReadUInt16BigEndian(header);
        var buffer = ArrayPool<byte>.Shared.Rent(length);
        try
        {
            await ReceiveExactlyAsync(socket, buffer.AsMemory(0, length), cancellationToken).ConfigureAwait(false);
            if (!DnsMessage.TryRead(buffer.AsSpan(0, length), id, question.Span, out var answer))
                throw new IOException($"DNS server {server} sent a malformed answer over TCP.");
            return answer;
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }
    }

    private static async ValueTask ReceiveExactlyAsync(Socket socket, Memory<byte> buffer,
        CancellationToken cancellationToken)
    {
        while (!buffer.IsEmpty)
        {
            var read = await socket.ReceiveAsync(buffer, SocketFlags.None, cancellationToken).ConfigureAwait(false);
            if (read == 0) throw new EndOfStreamException("DNS server closed the connection mid-answer.");
            buffer = buffer[read..];
        }
    }
}
