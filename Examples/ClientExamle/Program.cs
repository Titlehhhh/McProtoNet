using McProtoNet;
using McProtoNet.IO;
using McProtoNet.Networking;
using McProtoNet.Utils;
using System.Net.Sockets;

Connect();
Console.ReadLine();

static async void Connect()
{
    ushort port = 64463;
    TcpClient tcpClient = new();
    await tcpClient.ConnectAsync("192.168.1.153", port);
    IPacketReaderWriter _packetReaderWriter = new PacketReaderWriter(new NetworkMinecraftStream(tcpClient.GetStream()));

    await _packetReaderWriter.SendPacketAsync(new HandShakePacket(HandShakeIntent.LOGIN, 340, port, "192.168.1.153"), 0x00);

    await _packetReaderWriter.SendPacketAsync(new LoginStartPacket("TestBot"), 0x00);



    bool login = false;
    try
    {
        while (!login)
        {

            (int id, MemoryStream data) = await _packetReaderWriter.ReadNextPacketAsync();


            IPacket packet = null;

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
    Console.WriteLine("end");

    Console.ReadLine();
}

static async Task<bool> HandleLoginPackets(IPacket packet, IPacketReaderWriter packetReaderWriter)
{

    if (packet is LoginDisconnectPacket)
    {
        var disconnect = packet as LoginDisconnectPacket;
        Console.WriteLine("Diss: " + disconnect.Message);
        throw new Exception("");
    }
    else if (packet is LoginSetCompressionPacket)
    {
        var compress = packet as LoginSetCompressionPacket;
        //Session.CompressionThreshold = compress.Threshold;

        packetReaderWriter.SwitchCompression(compress.Threshold);
        return false;
    }
    else if (packet is EncryptionRequestPacket)
    {
        //TODO
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
        Console.WriteLine("succ");
        return true;
    }
    throw new NotImplementedException("Invalid Packet: " + packet.GetType().Name);
}


