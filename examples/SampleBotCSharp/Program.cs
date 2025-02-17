using System.Runtime.Intrinsics;
using McProtoNet.Protocol;
using SampleBotCSharp;

internal class Program
{
    public static async Task Main(string[] args)
    {
        Bot bot = new Bot(MinecraftVersion.V1_21_4, "title-kde");

        await bot.Start();

        await Task.Delay(-1);
    }
}