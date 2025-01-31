using System;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using System.Threading.Channels;

class Program
{
    static async Task Main()
    {
        Channel<Item> channel = Channel.CreateBounded<Item>(10);

        await channel.Writer.WriteAsync(new Item());
        await channel.Writer.WriteAsync(new Item());
        await channel.Writer.WriteAsync(new Item());
        channel.Writer.Complete(new Exception("asdasd"));
        while (channel.Reader.TryRead(out var item))
        {
            Console.WriteLine(item.Id);
        }
        
    }
}

class Item
{
    private static int _counter = 0;
    public int Id { get; set; }

    public Item()
    {
        Id = Interlocked.Increment(ref _counter);
    }
}