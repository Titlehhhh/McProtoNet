using System.Buffers;
using System.IO.Compression;
using McProtoNet.Client;
using McProtoNet.MultiVersionProtocol;
using McProtoNet.Net.Zlib;
using ZlibNGSharpMinimal.Inflate;

internal class Program
{
    private static byte[] Compress(byte[] data)
    {
        MemoryStream ms = new MemoryStream();
        using (ZLibStream zLibStream = new ZLibStream(ms, CompressionLevel.SmallestSize, true))
        {
            zLibStream.Write(data);
        }

        return ms.ToArray();
    }

    private static byte[] CreateData(int size)
    {
        byte[] res = new byte[size];
        for (int i = 0; i < res.Length; i++)
            res[i] = (byte)(i % 8);

        return res;
    }

    public static void Main(string[] args)
    {
        Thread thread = new Thread(() =>
        {
            Random r = new Random(27);
            byte[] data = CreateData(500);


            byte[] compressed = Compress(data);

            byte[] outTest = new byte[500];
            var decompressor = LibDeflateCache.RentDecompressor();

            decompressor.Decompress(compressed, outTest, out _);
        });

        
    }

    private static async Task NewMethod()
    {
        MinecraftClient client = new MinecraftClient()
        {
            ConnectTimeout = TimeSpan.FromSeconds(30),
            Host = "127.0.0.1",
            Port = 25565,
            Username = $"TestBot",
            Version = MinecraftVersion.Latest
        };
        await client.Start();
        await Task.Delay(-1);
    }

    private static async Task BowBots()
    {
        List<BowBot> bots = new();
        for (int i = 0; i < 20; i++)
        {
            bots.Add(new BowBot($"TitleBot_{i:D3}"));
        }

        var tasks = bots.Select(x => x.Run());
        await Task.WhenAll(tasks);
        await Task.Delay(-1);
    }

    private static async Task MultipleConnections()
    {
        var list = new List<MinecraftClient>();
        try
        {
            var listProtocols = new List<MultiProtocol>();
            for (int i = 0; i < 1; i++)
            {
                MinecraftClient client = new MinecraftClient()
                {
                    ConnectTimeout = TimeSpan.FromSeconds(30),
                    Host = "94.130.3.102",
                    Port = 25845,
                    Username = $"TTT",
                    Version = MinecraftVersion.Latest
                };
                client.Disconnected += async (sender, eventArgs) =>
                {
                    if (eventArgs.Exception is not null)
                    {
                        Console.WriteLine("Errored: " + eventArgs.Exception.Message);
                        Console.WriteLine(eventArgs.Exception.StackTrace);
                        Console.WriteLine("Restart");
                        try
                        {
                            await client.Start();
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Start: " + e);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Stopped");
                    }
                };
                var protoTest = new MultiProtocol(client);
                listProtocols.Add(protoTest);
                list.Add(client);
            }

            List<Task> tasks = new List<Task>();
            int index = 0;
            foreach (var minecraftClient in list)
            {
                static async Task RunBot(MinecraftClient client, MultiProtocol proto)
                {
                    for (int i = 0; i < 10; i++)
                    {
                        try
                        {
                            await client.Start();
                            //await proto.OnJoinGame.FirstOrDefaultAsync();
                            break;
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                    }
                }

                tasks.Add(RunBot(minecraftClient, listProtocols[index++]));
            }


            await Task.WhenAll(tasks);

            await Task.Delay(1000);
            var sends = listProtocols.Select(async b =>
            {
                try
                {
                    await b.SendChatPacket("Hello from Minecraft Holy Client");
                }
                catch (Exception exception)
                {
                    Console.WriteLine("SendErr: " + exception);
                    // ignored
                }
            });
            await Task.WhenAll(sends);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            // throw;
        }


        await Task.Delay(-1);
    }
}