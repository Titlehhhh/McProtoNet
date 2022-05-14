using McProtoNet.IO;
using McProtoNet.Networking;
using System.Net.Sockets;
using System.Runtime.Serialization.Json;
using System.Text;

namespace McProtoNet.Utils
{
    public class ServerInfoService : IServerInfoService
    {
        private static readonly DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(ServerInfo));
        public async Task<ServerInfo> GetServerInfoAsync(string host, ushort port)
        {
            throw new Exception();

        }
    }
}