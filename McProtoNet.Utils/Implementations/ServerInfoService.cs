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
            TcpClient tcpClient = new TcpClient(host, port);
            NetworkMinecraftStream networkMinecraftStream = new NetworkMinecraftStream(tcpClient.GetStream());
            IPacketReaderWriter packetReaderWriter = new PacketReaderWriter(networkMinecraftStream);

            await packetReaderWriter.SendPacketAsync(
                new HandShakePacket(HandShakeIntent.STATUS, -1, port, host), 0);
            await packetReaderWriter.SendPacketAsync(
                new StatusQueryPacket(), 0);

            (int id, MinecraftStream stream) = await packetReaderWriter.ReadNextPacketAsync(default);

            string response = stream.ReadString();

            ServerInfo serverInfo = null;
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(response));

            return (ServerInfo)serializer.ReadObject(ms);

        }
    }
}