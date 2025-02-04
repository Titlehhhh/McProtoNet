using System.Runtime.Intrinsics;
using SampleBotCSharp;

internal class Program
{
    public static async Task Main(string[] args)
    {
        
        Bot bot = new Bot();

        await bot.Start();

     
        await Task.Delay(-1);
    }
}