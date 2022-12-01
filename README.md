
[![NuGet version (McProtoNet)](https://img.shields.io/nuget/v/McProtoNet?style=flat-square)](https://www.nuget.org/packages/McProtoNet/) 



# McProtoNet

Ru: Мульти-версионная библиотека для работы с протоколом майнкрафт, написанная на c#.

En: Multi-version library for working with the Minecraft protocol, written in c#.


## Поддерживаемые версии
- [x] 1.12 (1.12.2, 340)
- [x] 1.13 (1.13.2, 404) Wait for update 1.7
- [x] 1.14 (1.14.4, 498) Wait for update 1.7
- [x] 1.15 (1.15.2, 578) Wait for update 1.7
- [x] 1.16 (1.16.5, 754) 
- [x] 1.17 (1.17.1, 756) Wait for update 1.7
- [x] 1.18 (1.18.2, 758) Wait for update 1.7
- [x] 1.19 (1.19.2, 760) Wait for update 1.7

# Пример кода
```
using McProtoNet.Core;
using McProtoNet.Protocol754;
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
```
