using McProtoNet;
using McProtoNet._754;
using McProtoNet.Core;
using McProtoNet.Protocol754;
using System.Diagnostics;
using System.Net.Sockets;

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