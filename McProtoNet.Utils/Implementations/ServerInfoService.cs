﻿using System.Runtime.Serialization.Json;

namespace McProtoNet.Utils
{
    public class ServerInfoService : IServerInfoService
    {
        private static readonly DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(ServerInfo));
        public async Task<ServerInfo> GetServerInfoAsync(string host, ushort port)
        {
            throw new NotImplementedException();
        }
    }
}