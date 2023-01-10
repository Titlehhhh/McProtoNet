using McProtoNet.Core;
using System.Net;
using System.Net.Sockets;

namespace McProtoNet
{
    public delegate void ClientConnectedHandler<TProto>(IClient<TProto> client) where TProto : IProtocol, new();
    public class Server<TProto> : IDisposable where TProto : IProtocol, new()
    {
        private List<Client<TProto>> Clients = new List<Client<TProto>>();

        private TcpListener server;
        private CancellationTokenSource CTS = new();
        public event ClientConnectedHandler<TProto> ClientConnected;
        public Server(ushort port)
        {
            server = new TcpListener(IPAddress.Any, port);
            CTS.Token.Register(() =>
            {
                server.Stop();
            });
        }
        public async void Start()
        {
            server.Start();
            while (!CTS.IsCancellationRequested)
            {
                TcpClient tcpClient = await server.AcceptTcpClientAsync();
                var addres = tcpClient.Client.RemoteEndPoint;                
                var listener = new PacketListener<TProto>(tcpClient, PacketSide.Server);
                IClient<TProto> newClient = new Client<TProto>(addres, listener);
                this.ClientConnected?.Invoke(newClient);
            }
        }
        public void Dispose()
        {

        }
    }
}
