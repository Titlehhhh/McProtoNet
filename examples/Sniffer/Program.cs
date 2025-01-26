// See https://aka.ms/new-console-template for more information

using System.Net;
using System.Net.Sockets;
using System.Text;
using DotNext.IO;
using McProtoNet;
using McProtoNet.Abstractions;
using McProtoNet.Client;
using McProtoNet.Net;
using McProtoNet.Serialization;
using McProtoNet.Utils;
using Extensions = McProtoNet.Extensions;

internal class Program
{
    private static string Host;
    private static ushort Port;

    private static string FeMuiekQFB(string input)
    {
        StringBuilder result = new StringBuilder();

        foreach (var t in input)
        {
            result.Append((char)(t ^ 23640 ^ -30109 ^ 7298 ^ -13844));
        }

        return result.ToString();
    }

    public static async Task Main(string[] args)
    {
        var g = FeMuiekQFB("̶̡̡̧̗̺̖̺̻̻̰̺̅\u0378");
        Console.WriteLine(g);
        return;

        IServerResolver resolver = new ServerResolver();
        try
        {
            var result = await resolver.ResolveAsync("mc.holyworld.ru");


            Host = result.Host;
            Port = result.Port;
        }
        catch (Exception e)
        {
            Host = "mc.holyworld.ru";
            Port = 25565;
        }

        TcpListener listener = new TcpListener(IPAddress.Any, 26636);

        listener.Start();
        Console.WriteLine("Start");
        while (true)
        {
            TcpClient tcpClient = await listener.AcceptTcpClientAsync();
            _ = WorkTcp(tcpClient);
        }
    }

    private static async Task WorkTcp(TcpClient tcpClient)
    {
        using (tcpClient)
        {
            try
            {
                MinecraftPacketSender sender = new MinecraftPacketSender();
                MinecraftPacketReader reader = new MinecraftPacketReader();

                sender.BaseStream = tcpClient.GetStream();
                reader.BaseStream = tcpClient.GetStream();


                using (ToServerClient toServerClient = new ToServerClient(Host, Port))
                {
                    await toServerClient.Connect();


                    using var handshake = await reader.ReadNextPacketAsync();

                    var hand = ParseHandshake(handshake);
                    Console.WriteLine("handshake: " + hand);

                    var responseHandshake = CreateHandshake(hand.NextState);

                    await toServerClient.SendPacket(responseHandshake);

                    if (hand.NextState == 2)
                    {
                        await SkipPacket(reader, toServerClient); //Login Start
                        int threshold = await ReadCompression(toServerClient, sender);

                        reader.SwitchCompression(threshold);
                        sender.SwitchCompression(threshold);

                        toServerClient.SetCompress(threshold);
                        Console.WriteLine("Threshold: " + threshold);

                        await SkipPacketFromServer(toServerClient, sender);


                        Client client = new Client(tcpClient, reader, sender);
                        Task t1 = AnalyzeServerToClient(toServerClient, client);
                        Task t2 = AnalyzeClientToServer(client, toServerClient);
                        await Task.WhenAll(t1, t2);
                    }
                    else
                    {
                        Task t1 = toServerClient.MainStream.CopyToAsync(tcpClient.GetStream());
                        Task t2 = tcpClient.GetStream().CopyToAsync(toServerClient.MainStream);
                        await Task.WhenAll(t1, t2);
                        //Status
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }

    private static async Task AnalyzeServerToClient(ToServerClient server, Client client)
    {
        await server.MainStream.CopyToAsync(client.MainStream);
    }

    private static async Task AnalyzeClientToServer(Client client, ToServerClient server)
    {
        while (true)
        {
            using var packet = await client.ReadNextPacketAsync();

            if (packet.Id == 0x03) // Chat
            {
                string message = ReadChat(packet);
                if (message.Contains("startExp"))
                {
                    string args = message.Replace("startExp", "").Trim();
                    server.StartExploit(args);
                    continue;
                }

                if (message.Contains("stopExp"))
                {
                    server.StopExploit();
                    continue;
                }
            }

            var outP = InputToOut(packet);
            await server.SendPacket(outP);
        }
    }

    private static string ReadChat(InputPacket packet)
    {
        scoped var reader = new MinecraftPrimitiveReader(packet.Data);
        return reader.ReadString();
    }


    private static async Task<int> ReadCompression(ToServerClient server, MinecraftPacketSender sender)
    {
        using var packet = await server.ReadNextPacket();

        if (packet.Id != 0x03)
            throw new Exception("COMPRESSSS");

        int threshold = Extensions.ReadVarInt(packet.Data.Span, out _);

        var outP = InputToOut(packet);
        await sender.SendAndDisposeAsync(outP, default);
        return threshold;
    }

    private static async Task SkipPacketFromServer(ToServerClient server, MinecraftPacketSender sender)
    {
        using var packet = await server.ReadNextPacket();
        var outP = InputToOut(packet);
        await sender.SendAndDisposeAsync(outP, default);
    }

    private static async Task SkipPacket(MinecraftPacketReader reader, ToServerClient client)
    {
        using var packet = await reader.ReadNextPacketAsync();
        var outP = InputToOut(packet);
        await client.SendPacket(outP);
    }

    private static OutputPacket InputToOut(InputPacket packet)
    {
        scoped var writer = new MinecraftPrimitiveWriter();
        try
        {
            writer.WriteVarInt(packet.Id);
            writer.WriteBuffer(packet.Data.Span);
            return new OutputPacket(writer.GetWrittenMemory());
        }
        finally
        {
            writer.Dispose();
        }
    }

    private static OutputPacket CreateHandshake(int state)
    {
        scoped var writer = new MinecraftPrimitiveWriter();
        try
        {
            writer.WriteVarInt(0);
            writer.WriteVarInt(754);
            writer.WriteString(Host);
            writer.WriteUnsignedShort(Port);
            writer.WriteVarInt(state);
            return new OutputPacket(writer.GetWrittenMemory());
        }
        finally
        {
            writer.Dispose();
        }
    }

    private static HandshakePacket ParseHandshake(InputPacket packet)
    {
        scoped var reader = new MinecraftPrimitiveReader(packet.Data);
        return new HandshakePacket()
        {
            ProtocolVersion = reader.ReadVarInt(),
            Address = reader.ReadString(),
            Port = reader.ReadUnsignedShort(),
            NextState = reader.ReadVarInt()
        };
    }
}