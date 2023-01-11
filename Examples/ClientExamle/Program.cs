using Examples;
using McProtoNet;
using McProtoNet.Core;
using McProtoNet.Core.Packets;
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

public class Program
{
    public static void Main()
    {
        ClientExample();

        Console.ReadLine();
    }

    private static void ClientExample()
    {
        MinecraftClient minecraftClient = new MinecraftClient();
        minecraftClient.Start("192.168.0.2", 25565);
    }


}

public class MinecraftClient
{
    private IPacketReaderWriter packetReaderWriter;
    private PacketCategory currentCategory;

    private CancellationTokenSource CTS = new();
    private static readonly IPacketCollection p754 = new PacketCollection754();

    public MinecraftClient()
    {

    }
    public void Start(string host, ushort port)
    {
        Thread thread = new Thread(() => OnStart(host, port))
        {
            IsBackground = true
        };
        thread.Start();
    }
    private void OnStart(string host, ushort port)
    {
        TcpClient tcpClient = new TcpClient();
        using (CTS.Token.Register(tcpClient.Close))
        {
            tcpClient.Connect(host, port);
            using (IMinecraftProtocol mc_proto = new MinecraftProtocol(tcpClient, true))
            {
                try
                {
                    currentCategory = PacketCategory.HandShake;
                    using (IPacketProvider handshakePackets = new PacketProvider(p754.ClientPackets[currentCategory], p754.ServerPackets[currentCategory]))
                    using (IPacketReaderWriter packetReaderWriter = new PacketReaderWriter(mc_proto, handshakePackets, false))
                    {
                        packetReaderWriter.SendPacket(new HandShakePacket(HandShakeIntent.STATUS, 754, "", 2665));
                    }

                    using (IPacketProvider handshakePackets = new PacketProvider(p754.ClientPackets[PacketCategory.Login], p754.ServerPackets[PacketCategory.HandShake]))
                    using (IPacketReaderWriter packetReaderWriter = new PacketReaderWriter(mc_proto, handshakePackets, false))
                    {
                        packetReaderWriter.SendPacket(new LoginStartPacket("Nick"));
                        bool loginSucc = true;
                        while (loginSucc)
                        {
                            MinecraftPacket packet = packetReaderWriter.ReadNextPacket();
                            loginSucc = HandleLogin(packetReaderWriter, packet);
                        }
                    }
                    using (IPacketProvider gamePackets = new PacketProvider(p754.ClientPackets[PacketCategory.Login], p754.ServerPackets[PacketCategory.HandShake]))
                    using (IPacketReaderWriter packetReaderWriter = new PacketReaderWriter(mc_proto, gamePackets, false))
                    {
                        packetReaderWriter.SendPacket(new LoginStartPacket());
                        bool loginSucc = true;
                        while (loginSucc)
                        {
                            MinecraftPacket packet = packetReaderWriter.ReadNextPacket();
                            HandleGame(packetReaderWriter, packet);
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("error: " + e);
                }
            }

        }
    }
    private void HandleGame(IPacketReaderWriter sender, MinecraftPacket packet)
    {
        Console.WriteLine("RecPAck: " + packet.GetType().Name);
    }
    private bool HandleLogin(IPacketReaderWriter sender, MinecraftPacket packet)
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
            return true;
        }
        else if (packet is LoginSetCompressionPacket compression)
        {
            sender.SwitchCompression(compression.Threshold);
        }
        else if (packet is LoginDisconnectPacket disconnect)
        {
            throw new LoginRejectedException(disconnect.Message);
        }
        return false;
    }
}


