namespace McProtoNet.Utils;

/// <summary>
/// Resolves server records asynchronously
/// </summary>
public interface IServerResolver
{
    /// <summary>
    /// Resolves a server record asynchronously
    /// </summary>
    Task<SrvRecord> ResolveAsync(string host, CancellationToken cancellationToken = default);
}