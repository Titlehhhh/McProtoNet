using McProtoNet.Core.Packets;
using McProtoNet.Utils;
using Starksoft.Net.Proxy;
using System.Net.Sockets;

namespace McProtoNet.Protocol340
{
    public sealed class MinecraftClient340
    {
        #region Fields
        private readonly string sessionId;
        private readonly string nick;
        private string _host;
        private ushort _port;


        private readonly bool proxyUsed = false;
        private readonly string proxyHost;
        private readonly ushort proxyPort;
        private readonly ProxyType proxyType;

        private bool _isClosed = false;
        private IPacketProvider packets;
        private ProtocolMode mode;
        private IPacketProtocol client;


        private TcpClient tcpClient;

        private readonly MinecraftPrimitiveReader reader = new();
        private static readonly IPacketCollection p340 = new PacketCollection340();
        private IPacketRepository packetRepository =
            new PacketRepository(p340.GetAllPackets(PacketSide.Client));
        #endregion

        #region Properties


        public ProtocolMode Mode
        {
            get => mode;
            private set
            {
                switch (value)
                {
                    case ProtocolMode.Handshake:
                        packets = packetRepository.GetPackets(PacketCategory.HandShake);
                        break;
                    case ProtocolMode.Login:
                        packets = packetRepository.GetPackets(PacketCategory.Login);
                        break;
                    case ProtocolMode.Game:
                        packets = packetRepository.GetPackets(PacketCategory.Game);
                        break;
                }

                mode = value;
            }
        }


        public bool IsOnlineMode { get; private set; }
        public Guid LoginUUID { get; private set; }

        #endregion

        #region Ctors

        public MinecraftClient340(string nick, string host, ushort port)
        {
            this._host = host;
            this._port = port;
            this.nick = nick;
            IsOnlineMode = false;
        }

        public MinecraftClient340(string nick, string host, ushort port, string proxyHost, ushort proxyPort, ProxyType proxyType)
        {
            this.nick = nick;
            this._host = host;
            this._port = port;
            this.proxyHost = proxyHost;
            this.proxyPort = proxyPort;
            this.proxyType = proxyType;
            IsOnlineMode = false;
            proxyUsed = true;
        }


        #endregion

        public int TargetProtocolVersion => 340;


        public event OnErrorHandler OnError;
        public event PacketReceivedHandler PacketReceived;
        public event PacketSentingHandler PacketSenting;
        public event PacketSentedHandler PacketSented;

        public event LoginRejectedHandler LoginRejected;


        public void Connect(string serverName = "localhost", ushort port = 25565)
        {


            Task.Run(() =>
            {
                try
                {
                    if (_port == 25565)
                    {
                        try
                        {
                            IServerResolver resolver = new ServerResolver();

                            var response = resolver.ResolveAsync(_host).Result;
                            _host = response.Host;
                            port = response.Port;
                        }
                        catch (SrvNotFoundException e)
                        {

                        }
                    }

                    this.tcpClient = CreateTcp();

                    this.client = new MinecraftProtocol(tcpClient);

                    InternalLogin(serverName, port);
                    Mode = ProtocolMode.Game;
                    StartGameMode();
                }
                catch (LoginRejectedException rej)
                {
                    this.LoginRejected?.Invoke(this, rej.Message);
                }
                catch (Exception e)
                {
                    OnException(e);
                }
            });
        }
        private static ProxyClientFactory factory = new();
        private TcpClient CreateTcp()
        {
            if (proxyUsed)
            {
                return factory.CreateProxyClient(this.proxyType, this.proxyHost, this.proxyPort)
                    .CreateConnection(_host, _port);
            }
            return new TcpClient(_host, _port);
        }

        private void InternalLogin(string serverName, ushort port)
        {
            Mode = ProtocolMode.Handshake;



            QueuePacket(new HandShakePacket(HandShakeIntent.LOGIN, 340, serverName, port));
            Mode = ProtocolMode.Login;

            Loginizer340 loginizer;
            if (IsOnlineMode)
                loginizer = new Loginizer340(nick, client, packets, sessionId);
            else
                loginizer = new Loginizer340(nick, client, packets);
            LoginUUID = loginizer.Login().Result;

        }

        public void Close()
        {
            _isClosed = true;

            client.Dispose();


        }
        public void Dispose()
        {
            client.Dispose();
            GC.SuppressFinalize(this);
        }

        public void QueuePacket(Packet packet)
        {
            try
            {
                if (packets.TryGetOutputId(packet.GetType(), out int id))
                {
                    PacketSenting?.Invoke(this, packet);
                    client.SendPacket(packet, id);
                    PacketSented?.Invoke(this, packet);

                }
            }
            catch (Exception e)
            {
                OnException(e);
            }
        }



        private void StartGameMode()
        {

            new Thread(() =>
            {
                try
                {
                    while (tcpClient.Connected)
                    {

                        (int id, MemoryStream data) = client.ReadNextPacket();

                        if (packets.TryGetInputPacket(id, out Packet packet))
                        {
                            reader.BaseStream = data;
                            packet.Read(reader);
                            data.Dispose();
                            PacketReceived?.Invoke(this, packet);

                        }

                    }

                }
                catch (Exception e)
                {
                    OnException(e);
                }
            })
            { IsBackground = true }
            .Start();
        }




        private void OnException(Exception exception)
        {

            if (_isClosed)
                return;
            _isClosed = true;

            tcpClient?.Close();

            OnError?.Invoke(this, exception);
        }
    }
}
