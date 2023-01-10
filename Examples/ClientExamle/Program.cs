using McProtoNet;
using McProtoNet.Core;
using McProtoNet.Core.Protocol;
using McProtoNet.Protocol754;
using McProtoNet.Protocol754.Packets.Client;
using McProtoNet.Protocol754.Packets.Server;
using McProtoNet.Utils;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using System.Net.Sockets;
using System.Runtime.Serialization.Json;
using System.Text;

namespace Example;

public class Pr
{
    public static void Main()
    {
        Info = "";

        string path = "ServerIcon.png";

        Image img = Image.Load(path);
        
        var icon = img.ToBase64String(PngFormat.Instance);
        

        ServerInfo serverInfo = new ServerInfo()
        {
            Description = ChatMessage.Simple("Сосите хуй"),
            Icon = icon,
            Players = new PlayerInfo
            {
                MaxPlayers = 1000,
                OnlinePlayers = 7,
                PlayerList = new GameProfile[]
                {
                    new GameProfile(Guid.NewGuid(), "Title_")
                },


            },
            TargetVersion = new VersionInfo("1.16.5", 754)
        };

        Info = serverInfo.ToString();

        Server<Protocol754> server = new Server<Protocol754>(25565);
        server.ClientConnected += Server_ClientConnected;
        server.Start();

        Console.ReadLine();
    }

    private static void Server_ClientConnected(IClient<Protocol754> client)
    {
        Console.WriteLine("NewClient: " + client.Address);
        client.Listener.PacketReceived += Listener_PacketReceived;
        client.Listener.OnError += Listener_OnError;
        client.Listener.Start();
    }


    private static void Listener_OnError(PacketListener<Protocol754> sender, Exception exception)
    {
        Console.WriteLine("err: " + exception);
    }
    private static string Info = "";
    private static DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(ServerInfo));
    private static void Listener_PacketReceived(PacketListener<Protocol754> sender, MinecraftPacket<Protocol754> packet)
    {
        Console.WriteLine("RecPack: " + packet.GetType().Name);
        if (sender.CurrentCategory == PacketCategory.HandShake)
        {
            if (packet is HandShakePacket handShake)
            {
                if (handShake.Intent == HandShakeIntent.STATUS)
                {
                    Console.WriteLine("GetStatus");
                    sender.CurrentCategory = PacketCategory.Status;
                }
                else
                {
                    sender.CurrentCategory = PacketCategory.Login;
                }
            }
        }
        else if (sender.CurrentCategory == PacketCategory.Status)
        {
            if (packet is StatusQueryPacket)
            {
                string json = Info;
                var response = new StatusResponsePacket(json);

                Console.WriteLine("Category: " + sender.CurrentCategory);


                sender.SendPacket(response);



            }
            else if (packet is StatusPingPacket ping)
            {
                var pong = new StatusPongPacket(ping.PayLoad);
                sender.SendPacket(pong);
            }
        }
    }


    private static async void ClientExample()
    {
        string host = "testhost";
        ushort port = 25565;

        IServerResolver serverResolver = new ServerResolver();

        var result = await serverResolver.ResolveAsync(host);
        host = result.Host;
        port = result.Port;

        TcpClient tcpClient = new TcpClient(host, port);

        PacketListener<Protocol754> packetListener = new PacketListener<Protocol754>(tcpClient, PacketSide.Client);

        packetListener.CurrentCategory = PacketCategory.HandShake;
        packetListener.PacketReceived += PacketListener_PacketReceived;

        packetListener.OnError += PacketListener_OnError;

        packetListener.SendPacket(new HandShakePacket(HandShakeIntent.STATUS, 754, host, port));
        packetListener.CurrentCategory = PacketCategory.Status;
        packetListener.SendPacket(new StatusQueryPacket());
        packetListener.Start();
    }

    private static void PacketListener_OnError(PacketListener<Protocol754> sender, Exception exception)
    {
        Console.WriteLine("err: " + exception);
    }

    private static void PacketListener_PacketReceived(PacketListener<Protocol754> sender, MinecraftPacket<Protocol754> packet)
    {

        if (packet is EncryptionRequestPacket encryption)
        {
            var RSAService = CryptoHandler.DecodeRSAPublicKey(encryption.PublicKey);
            var privateKey = CryptoHandler.GenerateAESPrivateKey();
            var key_enc = RSAService.Encrypt(privateKey, false);
            var token_enc = RSAService.Encrypt(encryption.VerifyToken, false);
            var response = new EncryptionResponsePacket(key_enc, token_enc);
            sender.SendPacket(response);
            sender.SwitchEncryption(privateKey);

        }
        else if (packet is LoginSuccessPacket)
        {
            sender.CurrentCategory = PacketCategory.Game;
        }
        else if (packet is ServerJoinGamePacket)
        {

        }
        else if (packet is LoginSetCompressionPacket compression)
        {
            sender.SwitchCompression(compression.Threshold);
        }
        else if (packet is ServerKeepAlivePacket keepAlivePacket)
        {
            sender.SendPacket(new ClientKeepAlivePacket(keepAlivePacket.PingID));
        }
        else if (packet is ServerDisconnectPacket dis)
        {
            Console.WriteLine("Dis: " + dis.Message);
        }
    }
}

