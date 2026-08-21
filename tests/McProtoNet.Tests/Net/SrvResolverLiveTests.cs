namespace McProtoNet.Tests.Net;

/// <summary>
///     The lookup against the machine's real resolver. Wire-format tests prove the parser; only this
///     proves that a hand-written DNS client talks to the DNS server the operating system points at.
///     Every test here skips instead of failing when there is no resolver or no network.
/// </summary>
public class SrvResolverLiveTests
{
    /// <summary>Public Minecraft hosts that publish an SRV record. One answer is enough.</summary>
    private static readonly string[] KnownSrvHosts = ["hypixel.net", "mc.hypixel.net", "wynncraft.com"];

    [Fact]
    public async Task ResolveAsync_FindsARealMinecraftSrvRecord()
    {
        var token = TestContext.Current.CancellationToken;
        SkipWithoutResolver();

        var failures = new List<string>();
        foreach (var host in KnownSrvHosts)
        {
            SrvResult? record;
            try
            {
                record = await SrvResolver.ResolveAsync(host, token);
            }
            catch (IOException e)
            {
                failures.Add($"{host}: {e.Message}");
                continue;
            }

            if (record is not { } srv)
            {
                failures.Add($"{host}: no SRV record");
                continue;
            }

            TestContext.Current.TestOutputHelper?.WriteLine(
                $"{SrvResolver.ServicePrefix}{host} -> {srv.Target}:{srv.Port} priority={srv.Priority} weight={srv.Weight}");

            Assert.NotEmpty(srv.Target);
            Assert.DoesNotContain('\0', srv.Target);
            Assert.False(srv.Target.EndsWith('.'), "The target must come back without the root dot.");
            Assert.Contains('.', srv.Target);
            Assert.InRange(srv.Port, 1, 65535);
            return;
        }

        Assert.Skip($"No public SRV host answered — the network is blocking DNS. Tried: {string.Join("; ", failures)}");
    }

    [Fact]
    public async Task ResolveAsync_HostWithoutARecord_ReturnsNull()
    {
        var token = TestContext.Current.CancellationToken;
        SkipWithoutResolver();

        try
        {
            // example.com exists and publishes no _minecraft._tcp record: an answer, not a failure.
            Assert.Null(await SrvResolver.ResolveAsync("example.com", token));
        }
        catch (IOException e)
        {
            Assert.Skip($"No DNS server answered: {e.Message}");
        }
    }

    [Fact]
    public async Task ResolveAsync_NameThatDoesNotExist_ReturnsNull()
    {
        var token = TestContext.Current.CancellationToken;
        SkipWithoutResolver();

        try
        {
            Assert.Null(await SrvResolver.ResolveAsync("mcprotonet-no-such-host.invalid", token));
        }
        catch (IOException e)
        {
            Assert.Skip($"No DNS server answered: {e.Message}");
        }
    }

    [Fact]
    public async Task ResolveAsync_RejectsAnEmptyHost()
    {
        var token = TestContext.Current.CancellationToken;

        await Assert.ThrowsAsync<ArgumentException>(async () => await SrvResolver.ResolveAsync("  ", token));
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await SrvResolver.ResolveAsync(null!, token));
    }

    private static void SkipWithoutResolver()
    {
        Assert.SkipWhen(SrvResolver.GetSystemDnsServers().Count == 0,
            "This machine has no DNS server configured on any interface.");
    }
}
