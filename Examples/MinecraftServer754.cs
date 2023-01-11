using McProtoNet;
using McProtoNet.Core;
using McProtoNet.Core.Protocol;
using McProtoNet.Protocol754;
using McProtoNet.Protocol754.Packets.Client;
using McProtoNet.Protocol754.Packets.Server;
using McProtoNet.Utils;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;

namespace Examples
{
    public class MinecraftServer754
    {
        public Dictionary<string, MinecraftPlayer> Players = new Dictionary<string, MinecraftPlayer>();

        private ServerInfo info = new ServerInfo
        {
            Description = ChatMessage.Simple("НЕГР", ChatColor.Red),
            Icon = Image.Load("ServerIcon.png").ToBase64String(PngFormat.Instance),
            Players = new PlayerInfo
            {
                MaxPlayers = 100,
                OnlinePlayers = 67,
                PlayerList = new GameProfile[]
                {
                    new GameProfile(Guid.NewGuid(), "Title_")
                }

            },
            TargetVersion = new VersionInfo("McProtoFast 1.16.5", 754)
        };


        private IServer<Protocol754> _server;
        public MinecraftServer754(ushort port)
        {
            _server = new Server<Protocol754>(port);

            _server.ClientConnected += _server_ClientConnected;
        }
        public void Start()
        {
            _server.Start();
        }

        private void _server_ClientConnected(IClient<Protocol754> client)
        {
            Console.WriteLine("NewClient: " + client.Address);
            client.Disconnected += (s, e) =>
            {
                Console.WriteLine("error: " + e);
                client.PacketReceived -= Client_PacketReceived;
            };
            client.PacketReceived += Client_PacketReceived;
            client.Start();
        }

        private void Client_PacketReceived(IClient<Protocol754> client, MinecraftPacket<Protocol754> packet)
        {
            Console.WriteLine("RecPAck: " + packet);
            if (client.CurrentCategory == PacketCategory.HandShake)
            {
                if (packet is HandShakePacket handShake)
                {
                    if (handShake.Intent == HandShakeIntent.STATUS)
                    {
                        client.CurrentCategory = PacketCategory.Status;
                    }
                    else
                    {
                        client.CurrentCategory = PacketCategory.Login;

                    }
                }
            }
            else if (client.CurrentCategory == PacketCategory.Status)
            {
                if (packet is StatusQueryPacket)
                {
                    Console.WriteLine("Query");
                    string json = info.ToString();
                    var response = new StatusResponsePacket(json);
                    client.SendPacket(response);
                }
                else if (packet is StatusPingPacket ping)
                {
                    Console.WriteLine("Ping");
                    var pong = new StatusPongPacket(ping.PayLoad);
                    client.SendPacket(pong);
                    client.Disconnect();
                }
            }
            else if (client.CurrentCategory == PacketCategory.Login)
            {
                if (packet is LoginStartPacket startPacket)
                {
                    if (Players.ContainsKey(startPacket.Nickname))
                    {
                        client.SendPacket(
                            new LoginDisconnectPacket(
                                ChatMessage.Simple("Ник смени", ChatColor.Red).ToString()));
                        client.Disconnect();
                    }
                    else
                    {
                        var player = new MinecraftPlayer(client, startPacket.Nickname);
                        Players.Add(startPacket.Nickname, player);
                    }
                }
            }
        }
    }
    public class MinecraftPlayer
    {
        private IClient<Protocol754> _client;
        private string _nick;

        public MinecraftPlayer(IClient<Protocol754> client, string nick)
        {
            _client = client;
            _nick = nick;
            _client.PacketReceived += _client_PacketReceived;

            Init();
            
        }
        private void Init()
        {
            _client.SendPacket(new LoginSetCompressionPacket(256));
            _client.SwitchCompression(256);
            _client.SendPacket(new LoginSuccessPacket(Guid.NewGuid(), ));
        }
        private void _client_PacketReceived(IClient<Protocol754> client, MinecraftPacket<Protocol754> packet)
        {
            if (client.CurrentCategory == PacketCategory.Login)
            {
                HandleLoginPacket(packet);
            }
        }
        private void HandleLoginPacket(MinecraftPacket<Protocol754> packet)
        {
            Console.WriteLine("Login: " + packet);
        }
    }
}
