# McProtoNet
Ru: Мульти-версионная библиотека для работы с протоколом майнкрафт, написанная на c#.

En: Multi-version library for working with the Minecraft protocol, written in c#.
# Пример кода
```
using McProtoNet;

Console.WriteLine("start");

IClient client = new MinecraftClient754("TestBot", "192.168.1.153", 52029);

client.OnPacketReceived += (s, packet) =>
{
    Console.WriteLine("Received: " + packet.GetType().Name);
};
client.OnDisconnected += (s, reason) =>
{
    Console.WriteLine(reason);
};
client.Start();

Console.ReadLine();
```
