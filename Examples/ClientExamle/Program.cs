using Examples;
using McProtoNet;
using McProtoNet.Core;
using McProtoNet.Core.Packets.DefaultPackets.Server.Status;
using McProtoNet.Core.Protocol;
using McProtoNet.Protocol754;
using McProtoNet.Protocol754.Packets.Client;
using McProtoNet.Protocol754.Packets.Server;
using McProtoNet.Utils;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.Text;

namespace Example;

public class Pr
{
    public static void Main()
    {
        MinecraftServer754 server754 = new MinecraftServer754(25565);
        server754.Start();
        Console.ReadLine();
    }

    


    private static async void ClientExample()
    {
        string host = "testhost";
        ushort port = 25565;

        // IServerResolver serverResolver = new ServerResolver();

        //  var result = await serverResolver.ResolveAsync(host);
        // host = result.Host;
        // port = result.Port;

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
        if (packet is StatusResponsePacket r)
        {
            File.WriteAllText("test.json", r.JsonResponse);
        }

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

