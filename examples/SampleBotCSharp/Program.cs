using System.Buffers;
using System.Collections.Concurrent;
using System.IO.Compression;
using System.Reactive.Linq;
using McProtoNet.Client;
using McProtoNet.MultiVersionProtocol;
using McProtoNet.Net.Zlib;
using ZlibNGSharpMinimal.Inflate;

internal class Program
{
    private static string[] _lines =
        "====================\n$$$$$$$$\\   $$\\ $$\\   \n$$  _____|  $$ \\$$ \\  \n$$ |      $$$$$$$$$$\\ \n$$$$$\\    \\_$$  $$   |\n$$  __|   $$$$$$$$$$\\ \n$$ |      \\_$$  $$  _|\n$$ |        $$ |$$ |  \n\\__|        \\__|\\__|  \n===================="
            .Split('\n');

    private static int linesIndex = 0;

    public static Task Main(string[] args)
    {
        Console.WriteLine(_lines.Length);
        return MultipleConnections();
        return NewMethod();
    }

    private static async Task NewMethod()
    {
        MinecraftClient client = new MinecraftClient()
        {
            ConnectTimeout = TimeSpan.FromSeconds(30),
            Host = "127.0.0.1",
            Port = 25565,
            Username = $"TestBot",
            Version = (int)MinecraftVersion.Latest
        };
        await client.Start();
        await Task.Delay(-1);
    }

    private static async Task BowBots()
    {
        List<BowBot> bots = new();
        for (int i = 1; i < 30; i++)
        {
            bots.Add(new BowBot($"TitleBot_{i:D3}"));
        }

        var tasks = bots.Select(x => x.Run());
        await Task.WhenAll(tasks);
        await Task.Delay(-1);
    }

    private static async Task MultipleConnections()
    {
        Console.WriteLine("Start");
        var list = new List<MinecraftClient>();
        try
        {
            var listProtocols = new List<(MinecraftClient ,MultiProtocol)>();
            for (int i = 1; i <= 30; i++)
            {
                MinecraftClient client = new MinecraftClient()
                {
                    ConnectTimeout = TimeSpan.FromSeconds(30),
                    Host = "192.168.0.9",
                    Port = 25565,
                    Username = $"BB_{i:D2}",
                    Version = (int)MinecraftVersion.Latest
                };
                client.Disconnected += async (sender, eventArgs) =>
                {
                    if (eventArgs.Exception is not null)
                    {
                        //Console.WriteLine("Errored: " + eventArgs.Exception.Message);
                        //Console.WriteLine(eventArgs.Exception.StackTrace);
                        //Console.WriteLine("Restart");
                        try
                        {
                            await client.Start();
                        }
                        catch (Exception e)
                        {
                            //Console.WriteLine("Start: " + e);
                        }
                    }
                    else
                    {
                        // Console.WriteLine("Stopped");
                    }
                };
                var protoTest = new MultiProtocol(client);
                listProtocols.Add((client,protoTest));
                list.Add(client);
            }

            List<Task> tasks = new List<Task>();
          
            static async Task RunBot(MinecraftClient client, MultiProtocol proto)
            {
                for (int i = 0; i < 10; i++)
                {
                    try
                    {
                        var onJoin = proto.OnLogin.FirstOrDefaultAsync();
                        await client.Start();

                        await onJoin;

                        await Task.Delay(3000);
                        break;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
            }
            
            tasks.AddRange(listProtocols.Select(x=>RunBot(x.Item1,x.Item2)));

            await Task.WhenAll(tasks);

            while (true)
            {
                foreach (var tuple in listProtocols)
                {
                    MinecraftClient client = tuple.Item1;
                    MultiProtocol protocol = tuple.Item2;
                    if (linesIndex >= _lines.Length)
                    {
                        linesIndex = 0;
                        await Task.Delay(300);
                    }

                    string nextMess = _lines[linesIndex++].TrimEnd();

                    //Console.WriteLine($"{client.Username}:\t{nextMess}");
                    await Task.Delay(10);

                    try
                    {
                        await protocol.SendChatPacket(nextMess);
                    }
                    catch(Exception exception)
                    {
                        Console.WriteLine(exception);
                        linesIndex = Math.Max(0, linesIndex - 1);
                    }
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            // throw;
        }


        await Task.Delay(-1);
    }
}