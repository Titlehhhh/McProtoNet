using System.Buffers;
using System.Buffers.Binary;
using System.Globalization;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Security.Cryptography;

namespace McProtoNet;

/// <summary>
///     The <c>_minecraft._tcp</c> SRV lookup, the way vanilla does it: ask the system resolver where the
///     server really is, and take the literal host and port when it has nothing to say.
///     <para>
///     No record is not an error — <see cref="ResolveAsync(string, CancellationToken)" /> returns
///     <see langword="null" />. Only a real failure throws: no DNS server answered, or the caller cancelled.
///     </para>
///     <para>
///     The whole lookup runs on one budget. Each configured server gets an equal slice of it, so a first
///     resolver that has gone quiet cannot swallow the time the second one needed. The DNS query is built
///     and parsed here (RFC 1035 plus RFC 2782) over UDP, with the usual TCP retry when the answer does not
///     fit a datagram. .NET has no in-box SRV lookup on the frameworks this library targets.
///     </para>
/// </summary>
public static class SrvResolver
{
    /// <summary>The service label Minecraft servers publish under.</summary>
    public const string ServicePrefix = "_minecraft._tcp.";

    /// <summary>How long the whole lookup may take, across every configured server.</summary>
    internal static readonly TimeSpan DefaultBudget = TimeSpan.FromSeconds(5);

    /// <summary>No server gets less than this, however many of them are configured.</summary>
    internal static readonly TimeSpan MinimumSlice = TimeSpan.FromSeconds(1);

    private const int DnsPort = 53;
    private const int UdpAnswerLimit = 4096;

    /// <summary>Datagrams that are not our answer before we give up on a server and try the next.</summary>
    private const int MaxStrayDatagrams = 32;

    private static readonly TimeSpan ServerCacheLifetime = TimeSpan.FromSeconds(30);
    private static readonly object CacheGate = new();
    private static List<IPEndPoint>? _cachedServers;
    private static long _cachedAt;

    /// <summary>
    ///     Picks the record a client should connect to, or <see langword="null" /> when the host publishes
    ///     no SRV record, when a record says "no service", or when the machine has no DNS server configured.
    /// </summary>
    /// <param name="host">Host name the user typed, without the service prefix.</param>
    /// <param name="cancellationToken">Cancels the lookup.</param>
    /// <exception cref="IOException">No configured DNS server produced an answer.</exception>
    public static async ValueTask<SrvResult?> ResolveAsync(string host, CancellationToken cancellationToken = default)
    {
        var records = await ResolveAllAsync(host, cancellationToken).ConfigureAwait(false);
        return Select(records, Random.Shared.Next);
    }

    /// <summary>
    ///     Every SRV record published for the host, in the order the resolver returned them, for callers
    ///     that want to walk the list themselves. Empty when there is no record.
    /// </summary>
    /// <exception cref="IOException">No configured DNS server produced an answer.</exception>
    public static ValueTask<IReadOnlyList<SrvResult>> ResolveAllAsync(string host,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(host);
        return ResolveAllAsync(host, GetSystemDnsServers(), DefaultBudget, cancellationToken);
    }

    /// <summary>The same lookup against named servers, so a test can stand one up on the loopback.</summary>
    internal static async ValueTask<SrvResult?> ResolveAsync(string host, IReadOnlyList<IPEndPoint> servers,
        CancellationToken cancellationToken)
    {
        var records = await ResolveAllAsync(host, servers, DefaultBudget, cancellationToken).ConfigureAwait(false);
        return Select(records, Random.Shared.Next);
    }

    /// <inheritdoc cref="ResolveAsync(string, IReadOnlyList{IPEndPoint}, CancellationToken)" />
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

    /// <summary>
    ///     RFC 2782 selection: keep the lowest priority, then draw inside that group by weight.
    ///     <paramref name="roll" /> takes an exclusive upper bound and returns a number below it.
    /// </summary>
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

    /// <summary>
    ///     The resolvers the operating system is configured with, IPv4 first, cached briefly because
    ///     enumerating the interfaces is a blocking call into the OS and DNS settings rarely move.
    /// </summary>
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

    /// <summary>Forgets the cached resolver list, so the next lookup reads the interfaces again.</summary>
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

    /// <summary>Punycode, so a host in a national alphabet reaches the wire as the labels DNS carries.</summary>
    private static string ToAscii(string host)
    {
        foreach (var c in host)
            if (c > 0x7F)
                return new IdnMapping { AllowUnassigned = true }.GetAscii(host);

        return host;
    }

    /// <summary>
    ///     One server, one slice of the budget for the datagram and — if the answer did not fit one — a
    ///     fresh slice for the TCP retry, so a truncated answer is not cut off by the time UDP already used.
    /// </summary>
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
