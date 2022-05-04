namespace ProtoLib.Services
{
    public interface IServerInfoService
    {
        Task<ServerInfo> GetServerInfoAsync(string host, ushort port);
    }
}