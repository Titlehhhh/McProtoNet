namespace ProtoLib.Services
{
    public interface IServerResolver
    {
        Task<(string, ushort)> ResolveAsync(string host);
    }
}