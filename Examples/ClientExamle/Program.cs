using McProtoNet.Core;
using McProtoNet.Protocol754;
using McProtoNet.Utils;
using System.Net;
using System.Net.Sockets;

LanServerDetector lanServer = new LanServerDetector();

lanServer.ServerFinded += (server) =>
{
    Console.WriteLine($"new Server: {server.Host}:{server.Port}");
    
};
lanServer.StartReceiving();

Console.ReadLine();
Console.WriteLine("start");
TcpClient tcpClient = new TcpClient("192.168.1.153", 57883);
IPacketReaderWriter client = new PacketReaderWiter(tcpClient.Client);
ISession session = new Session754(client);

client.OnPacketReceived += (s, packet) =>
{
    //Trace.WriteLine("packet: " + packet.GetType().Name);

};

Console.WriteLine(await session.Login());

Console.ReadLine();