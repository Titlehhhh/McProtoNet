using McProtoNet.Core;
using System.Net;
using System.Net.Sockets;

namespace McProtoNet
{
    public class Server<TProto> : IDisposable, IServer<TProto> where TProto : IProtocol, new()
    {
        public event ClientConnectedHandler<TProto> ClientConnected;
        public event ServerOnErrorHandler<TProto> OnError;
        public event ServerStartedHandler<TProto> ServerStarted;
        public event ServerStopedHandler<TProto> ServerStoped;
        private TcpListener server;
        private CancellationTokenSource CTS = new();

        public Server(ushort port)
        {
            server = new TcpListener(IPAddress.Any, port);
        }
        public async void Start()
        {

            server.Start();
            CTS.Token.Register(() =>
            {
                server.Stop();
                this.ServerStoped?.Invoke(this);
            });
            this.ServerStarted?.Invoke(this);
            try
            {
                while (!CTS.IsCancellationRequested)
                {
                    TcpClient tcpClient = await server.AcceptTcpClientAsync();

                    var addres = tcpClient.Client.RemoteEndPoint;
                    var listener = new PacketListener<TProto>(tcpClient, PacketSide.Server);
                    IClient<TProto> newClient = new Client<TProto>(addres, listener);

                    CTS.Token.Register(() =>
                    {
                        newClient.Disconnect();
                        newClient.Dispose();
                    });
                    this.ClientConnected?.Invoke(newClient);
                }
            }
            catch (Exception e)
            {
                if (!CTS.IsCancellationRequested)
                {
                    this.OnError?.Invoke(this, e);
                }
            }
        }

        public void Stop()
        {
            CTS.Cancel();
        }

        public void Dispose()
        {

        }
    }
}
