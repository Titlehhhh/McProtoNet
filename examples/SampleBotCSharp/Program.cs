using System.Runtime.Intrinsics;
using SampleBotCSharp;

internal class Program
{
    public static async Task Main(string[] args)
    {
        sbyte a = (sbyte)0b_0000_0100;

        int c = a & 0b1111;
        int b = a >>> 4;
        
        Console.WriteLine($"{b:b8}");
        Console.WriteLine($"{c:b8}");
        return;


        Bot bot = new Bot();

        await bot.Start();


        await Task.Delay(-1);
    }
}