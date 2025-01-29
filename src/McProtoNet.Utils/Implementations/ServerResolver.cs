using DnsClient;

namespace McProtoNet.Utils;

/// <summary>
/// Resolves server records using DNS queries
/// </summary>
public class ServerResolver : IServerResolver

{
    /// <summary>
    /// Resolves a server record asynchronously
    /// </summary>
    /// <param name="host">The host to resolve</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The resolved server record</returns>
    public async Task<SrvRecord> ResolveAsync(string host, CancellationToken cancellationToken = default)
    {
        LookupClient lookupClient = new();


        var response = await lookupClient.QueryAsync(new DnsQuestion($"_minecraft._tcp.{host}", QueryType.SRV),
            cancellationToken);
        if (response.HasError != true && response.Answers.SrvRecords().Any())
        {
            var result = response.Answers.SrvRecords()
                .OrderBy(record => record.Priority)
                .ThenByDescending(record => record.Weight)
                .ThenBy(record => Guid.NewGuid())
                .First();
            var target = result.Target.Value.Trim('.');
            return new SrvRecord(target, result.Port);
        }


        throw new SrvNotFoundException();
    }
}