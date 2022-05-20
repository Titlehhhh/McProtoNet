

using McProtoNet;
using McProtoNet.IO;
using McProtoNet.Networking;
using McProtoNet.Utils;
using System.Net.Sockets;

class Core
{
    static void Main(string[] args)
    {
        Bot client = new Bot("TestBot");
        client.Connect("localhost", 25565).GetAwaiter().GetResult();
    }
}

class Bot
{
    public static string? name;
    public Bot(string botName)
    {
        name = botName;
    }

    public async Task Connect(string ip, ushort port)
    {
        TcpClient tcpClient = new();
        await tcpClient.ConnectAsync(ip, port);
        IPacketReaderWriter _packetReaderWriter = new PacketReaderWriter(new NetworkMinecraftStream(tcpClient.GetStream()));

        await _packetReaderWriter.SendPacketAsync(new HandShakePacket(HandShakeIntent.LOGIN, 340, port, ip), 0x00);

        await _packetReaderWriter.SendPacketAsync(new LoginStartPacket(name), 0x00);



        bool login = false;
        try
        {
            while (!login)
            {

                (int id, MemoryStream data) = await _packetReaderWriter.ReadNextPacketAsync();


                Packet? packet = null;

                switch (id)
                {
                    case 0x00:
                        packet = new LoginDisconnectPacket();
                        break;
                    case 0x02:
                        packet = new LoginSuccessPacket();
                        break;
                    case 0x03:
                        packet = new LoginSetCompressionPacket();
                        break;
                    case 0x01:
                        packet = new EncryptionRequestPacket();
                        break;

                }
                if (packet is null)
                    continue;

                packet.Read(new MinecraftPrimitiveReader(data));

                login = await HandleLoginPackets(packet, _packetReaderWriter);

            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            login = true;
        }
        await Task.Delay(Timeout.Infinite);
    }

    static async Task<bool> HandleLoginPackets(Packet packet, IPacketReaderWriter packetReaderWriter)
    {

        if (packet is LoginDisconnectPacket)
        {
            var disconnect = packet as LoginDisconnectPacket;
            if(disconnect!=null) Console.WriteLine("Отключен. Причина: " + disconnect.Message);
            throw new Exception("");
        }
        else if (packet is LoginSetCompressionPacket)
        {
            var compress = packet as LoginSetCompressionPacket;

            packetReaderWriter.SwitchCompression(compress.Threshold);
            return false;
        }
        else if (packet is EncryptionRequestPacket)
        {
            var request = packet as EncryptionRequestPacket;
            var RSAService = CryptoHandler.DecodeRSAPublicKey(request.PublicKey);
            byte[] secretKey = CryptoHandler.GenerateAESPrivateKey();

            await packetReaderWriter.SendPacketAsync(new EncryptionResponsePacket(RSAService.Encrypt(secretKey, false), RSAService.Encrypt(request.VerifyToken, false)), 0x01);

            packetReaderWriter.SwitchEncryption(secretKey);
            return false;
        }
        else if (packet is LoginSuccessPacket)
        {
            var succes = packet as LoginSuccessPacket;
            Console.WriteLine("Бот успешно присоединился к серверу");
            return true;
        }
        throw new NotImplementedException("Невалидный пакет: " + packet.GetType().Name);
    }
}
