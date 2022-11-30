using McProtoNet.Protocol340;
using McProtoNet.Protocol340.Packets.Client;
using McProtoNet.Protocol340.Packets.Server;

Console.WriteLine("start");

MinecraftClient340 client = new("TestBot", "192.168.0.3", 53632);


client.PacketSented += (s, p) =>
{

    Console.WriteLine("send:" + p.GetType().Name);
};

client.PacketReceived += (s, p) =>
{
    //   Console.WriteLine(p.GetType().Name);
    if (p is ServerKeepAlivePacket keepAlivePacket)
    {
        client.QueuePacket(new ClientKeepAlivePacket(keepAlivePacket.ID));
    }
    else if (p is ServerBlockChangePacket changePacket)
    {
        Console.WriteLine("change: " + changePacket.Record.Position.ToString());
    }
};
client.OnError += (session, err) =>
{
    Console.WriteLine("err: " + err);
};
Console.WriteLine("asd");
client.Connect("192.168.0.3", 53632);
Console.WriteLine("asd2");

Console.ReadLine();