namespace McProtoNet.Services
{
    public interface IServerResolver
    {
        Task<(string, ushort)> ResolveAsync(string host);
    }
}