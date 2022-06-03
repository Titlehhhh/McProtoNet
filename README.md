
[![NuGet version (McProtoNet)](https://img.shields.io/nuget/v/McProtoNet.svg?style=flat-square)](https://www.nuget.org/packages/McProtoNet/)


# McProtoNet
Ru: Мульти-версионная библиотека для работы с протоколом майнкрафт, написанная на c#.

En: Multi-version library for working with the Minecraft protocol, written in c#.
# Пример кода
```
using McProtoNet.API;
using McProtoNet.API.Packets;
using McProtoNet.API.Protocol;
using McProtoNet.Protocol754;
using System.Diagnostics;
using System.Net.Sockets;

Console.WriteLine("start");
TcpClient tcpClient = new TcpClient("192.168.1.153", 57883);
IPacketReaderWriter client = new PacketReaderWiter(tcpClient.Client);
ISession session = new Session754(client);

client.OnPacketReceived += (s, packet) =>
{
	
};

Console.WriteLine(await session.Login());

```
