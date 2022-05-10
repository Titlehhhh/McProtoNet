using McProtoNet.PacketRepository754;
using McProtoNet.PacketRepository754.Packets.Client;
using McProtoNet.PacketRepository754.Packets.Server;
using McProtoNet.API;
using McProtoNet.API.IO;
using McProtoNet.API.Networking;
using McProtoNet.API.Protocol;
using McProtoNet.Exceptions;
using McProtoNet.Geometry;
using McProtoNet.Protocol;
using System.ComponentModel;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using McProtoNet.Utils;


namespace McProtoNet
{


    public class MinecraftClient : IDisposable, INotifyPropertyChanged
    {
        public static void Debug(string msg)
        {
            
        }
        private readonly TcpClient tcpClient;
        private NetworkMinecraftStream NetMcStream;
        private PacketReaderWriter packetReaderWriter;



        public MinecraftClient(GameProfile gameProfile, IMinecraftHandler handler, TcpClient tcpClient)
        {
            if (string.IsNullOrEmpty(nickname))
                throw new InvalidOperationException("nick is null");

            if (handler is null)
                throw new ArgumentNullException(nameof(handler));

            if (tcpClient is null)
                throw new ArgumentNullException(nameof(tcpClient));

            this.tcpClient = tcpClient;
            this._handler = handler;

            GameProfile = gameProfile;
        }

        public GameProfile GameProfile { get; private set; }



        #region Игровые свойства
        private ProtocolState subProtocol;
        public ProtocolState SubProtocol
        {
            get { return subProtocol; }
            private set
            {
                switch (value)
                {
                    case ProtocolState.HandShake:
                        RegisterHandShakePackets();
                        break;
                    case ProtocolState.Login:
                        RegisterLoginPackets();
                        break;
                    case ProtocolState.Game:
                        RegisterGamePackets();
                        break;
                }

                subProtocol = value;

            }
        }

        public Guid UUID
        {
            get => uUID;
            private set
            {
                uUID = value;
                OnPropertyChanged();
            }
        }

        public Point3 Location
        {
            get => location;
            private set
            {
                location = value;
                OnPropertyChanged();
            }
        }

        public Rotation Rotation
        {
            get => rotation; private set
            {
                rotation = value;
                OnPropertyChanged();
            }
        }

        public bool IsGround { get; private set; }


        #endregion


        private readonly IMinecraftHandler _handler;



        private static readonly IPacketProvider packetProvider754 = new PacketProvider754();

        public PacketManager PacketManager { get; set; }

        private IPacketProducer PacketFactory => PacketManager;


        private readonly CancellationTokenSource Cancellation = new();

        private Task ReadTask;

        #region Общие методы        
        public async Task LoginAsync()
        {
            try
            {
                this.packetReaderWriter = new PacketReaderWriter(NetMcStream);

                SubProtocol = ProtocolState.HandShake;

                await SendPacketAsync(new API.HandShakePacket(HandShakeIntent.LOGIN, 754, 0, ""));

                SubProtocol = ProtocolState.Login;

                await SendPacketAsync(new LoginStartPacket(GameProfile.Nickname));

                bool login;
                do
                {
                    IPacket packet = await ReadPacketLoginAsync();

                    login = await HandleLoginPackets(packet);

                } while (!login);



                SubProtocol = ProtocolState.Game;

                _handler.OnLoginSucces(UUID);
                ReadTask = ReadLoop();

            }
            catch (LoginRejectException e)
            {
                _handler.OnLoginReject(e.Message);
                throw;
            }
            catch (Exception e)
            {
                _handler.OnDisconnect(e);
                throw;
            }

        }
        private bool Stoping = false;

        public async Task StopAsync()
        {
            Stoping = true;
            Cancellation.Cancel();
            tcpClient.Close();
            await ReadTask;
        }

        private async Task ReadLoop()
        {
            try
            {
                while (tcpClient.Connected)
                {
                    Cancellation.Token.ThrowIfCancellationRequested();
                    (int id, MinecraftStream stream) = await this.packetReaderWriter.ReadNextPacketAsync(Cancellation.Token);
                    Lazy<IPacket> packetLazy = null;
                    if (PacketFactory.TryGetInputPacket(id, out packetLazy))
                    {
                        packetLazy.Value.Read(stream);

                        await HandlePacket(packetLazy.Value);

                    }
                }
            }
            catch (Exception e)
            {
                if (!Stoping)
                    _handler.OnDisconnect(e);
            }
            finally
            {
                tcpClient.Close();
            }
        }


        private async Task<IPacket> ReadPacketLoginAsync()
        {
            (int id, MinecraftStream stream) = await this.packetReaderWriter.ReadNextPacketAsync(Cancellation.Token);
            Lazy<IPacket> packet = null;
            PacketFactory.TryGetInputPacket(id, out packet);
            packet.Value.Read(stream);
            return packet.Value;

        }

        private async Task<bool> HandleLoginPackets(IPacket packet)
        {
            if (packet is LoginDisconnectPacket)
            {
                var disconnect = packet as LoginDisconnectPacket;
                Stoping = true;
                tcpClient.Close();
                Cancellation.Cancel();
                throw new LoginRejectException(disconnect.Message);
            }
            else if (packet is LoginSetCompressionPacket)
            {
                var compress = packet as LoginSetCompressionPacket;
                //Session.CompressionThreshold = compress.Threshold;

                this.packetReaderWriter.SwitchCompression(compress.Threshold);
                return false;
            }
            else if (packet is EncryptionRequestPacket)
            {
                //TODO
                var request = packet as EncryptionRequestPacket;
                var RSAService = CryptoHandler.DecodeRSAPublicKey(request.PublicKey);
                byte[] secretKey = CryptoHandler.GenerateAESPrivateKey();

                await SendPacketAsync(new EncryptionResponsePacket(RSAService.Encrypt(secretKey, false), RSAService.Encrypt(request.VerifyToken, false)));

                NetMcStream.SwitchEncryption(secretKey);
                return false;
            }
            else if (packet is LoginSuccessPacket)
            {
                var succes = packet as LoginSuccessPacket;
                SubProtocol = ProtocolState.Game;
                UUID = succes.UUID;
                return true;
            }
            throw new NotImplementedException("Invalid Packet: " + packet.GetType().Name);
        }



        private async Task SendPacketAsync(IPacket packet)
        {
            try
            {
                int id = 0;
                if (PacketFactory.TryGetOutputId(packet.GetType(), out id))
                {
                    await this.packetReaderWriter.SendPacketAsync(packet, id, Cancellation.Token);
                }
            }
            catch (Exception e)
            {
                tcpClient.Close();

                _handler.OnDisconnect(e);
            }
        }

        private void OnGameDisconnect(string message)
        {
            Stoping = true;
            Cancellation.Cancel();
            tcpClient.Close();

            _handler.OnDisconnect(message);


        }

        private bool EventsSub = false;
        private Guid uUID;
        private Point3 location;
        private Rotation rotation;
        private bool isAuth;
        private string nickname;
        private string password;
        private string host;
        private ushort port = 25565;
        private bool proxyEnabled;
        private string proxyHost;
        private ushort proxyPort;
        private string proxyLogin;
        private string proxyPassword;

        public event PropertyChangedEventHandler? PropertyChanged;




        #endregion

        #region Работа с пакетами


        private async Task HandlePacket(IPacket packet)
        {
            Debug("Пришел пакет: " + packet.GetType().Name);

            _handler.OnPacketReceived(packet);
            //Console.WriteLine(packet.GetType().Name);


            if (packet is ServerJoinGamePacket)
            {
                var join = packet as ServerJoinGamePacket;
                _handler.OnGameJoined();
            }
            else if (packet is ServerDisconnectPacket)
            {
                var disconnect = packet as ServerDisconnectPacket;
                OnGameDisconnect(disconnect.Message);
            }
            else if (packet is ServerKeepAlivePacket)
            {
                var keepalive = packet as ServerKeepAlivePacket;
                await SendPacketAsync(new ClientKeepAlivePacket(keepalive.PingID));
            }
            else if (packet is ServerRespawnPacket)
            {
                var respawn = packet as ServerRespawnPacket;

            }
            else if (packet is ServerChatPacket)
            {
                var message = packet as ServerChatPacket;
                _handler.OnChat(message.Message);
            }
        }



        #endregion


        #region Методы действий
        public void SendLocation(Rotation rotation, bool isGround)
        {

        }

        public void SendLocation(Point3 position, Rotation rotation, bool isGround)
        {

        }
        public void LookHead(Rotation rotation)
        {

        }

        public void LookHead(Point3 targetpos)
        {

        }
        public void SendChat(string msg)
        {

        }

        public void SendLocation(bool isGround)
        {

        }

        public void SendLocation(Point3 position, bool isGround)
        {

        }

        public void SendLocation(Vector3 vector, bool isGround)
        {

        }

        public void SendLocation(Point3 position, float yaw, float pitch, bool isGround)
        {

        }

        public void SendLocation(Vector3 body, Vector3 head, bool isGround)
        {

        }
        #endregion






        private void RegisterHandShakePackets()
        {
            PacketManager.ClearAll();
            PacketManager.LoadOutputPackets(packetProvider754.ClientPackets.HandShakePackets);
        }
        private void RegisterLoginPackets()
        {
            PacketManager.ClearAll();

            PacketManager.LoadOutputPackets(packetProvider754.ClientPackets.LoginPackets);

            PacketManager.LoadInputPackets(packetProvider754.ServerPackets.LoginPackets);
        }
        private void RegisterGamePackets()
        {
            PacketManager.ClearAll();

            PacketManager.LoadOutputPackets(packetProvider754.ClientPackets.GamePackets);

            PacketManager.LoadInputPackets(packetProvider754.ServerPackets.GamePackets);
        }






        public void ClickBlock(Point3_Int pos)
        {

        }

        public void UseItem()
        {

        }

        public void UseItem(byte slot)
        {

        }

        public void UseBlock(Point3_Int pos)
        {

        }

        private void OnPropertyChanged([CallerMemberName] string name = "")
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            tcpClient?.Dispose();
        }
    }
}
