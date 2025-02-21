using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using System.Threading.Channels;
using DotNext.Buffers;
using McProtoNet.Protocol;
using McProtoNet.Utils;

class Program
{
    static async Task Main()
    {
        
        
        
        //play.playbesttime.space

        IServerResolver resolver = new ServerResolver();

        string host = "127.0.0.1";
        MinecraftVersion version = MinecraftVersion.V1_16_4_To_1_16_5;
        ushort port = 25565;
        
        
        
        string original = host;
        if(host != "127.0.0.1")
        {
            try
            {
                Console.WriteLine("Resolving...");
                var result = await resolver.ResolveAsync(host);

                host = result.Host;
                port = result.Port;
                Console.WriteLine($"{host}:{port}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        ProxyProvider proxyProvider = null;
        List<Bot> bots = [];
        for (int i = 0; i < 1; i++)
        {
            Bot bot = new Bot(proxyProvider, host, port, version, original);
            bots.Add(bot);
        }

        foreach (var bot in bots)
        {
            _ = bot.Run();
        }

        await Task.Delay(-1);
    }

}
