# McProtoNet
Мульти-версионная библиотека для работы с протоколом майнкрафт, написанная на c#.

# Пример кода
```
using McProtoNet;
using McProtoNet.IO;
using McProtoNet.Networking;
using McProtoNet.Utils;
using System.Net.Sockets;

ushort port = 64463;
TcpClient tcpClient = new();
await tcpClient.ConnectAsync("192.168.1.153", port);
IPacketReaderWriter _packetReaderWriter = new PacketReaderWriter(new NetworkMinecraftStream(tcpClient.GetStream()));

await _packetReaderWriter.SendPacketAsync(new HandShakePacket(HandShakeIntent.LOGIN, 340, port, "192.168.1.153"), 0x00);

await _packetReaderWriter.SendPacketAsync(new LoginStartPacket("TestBot"), 0x00);
```
