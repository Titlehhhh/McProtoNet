namespace McProtoNet.Utils
{
    public interface IServerResolver
    {
        Task<(string, ushort)> ResolveAsync(string host);
    }
}