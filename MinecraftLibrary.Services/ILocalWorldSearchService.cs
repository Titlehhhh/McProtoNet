using ProtoLib.API;


namespace ProtoLib.Services
{
    public interface ILocalWorldSearchService
    {
        Task<List<ServerInfo>> SearchAsync();
    }
    public class LocalWorldSearchService : ILocalWorldSearchService
    {
        public Task<List<ServerInfo>> SearchAsync()
        {
            throw new NotImplementedException();
        }
    }
}