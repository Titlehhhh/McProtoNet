using McProtoNet;
using McProtoNet.Abstractions;
using McProtoNet.Client;
using McProtoNet.Net;
using McProtoNet.Protocol;
using McProtoNet.Protocol.Packets.Handshaking.Serverbound;
using McProtoNet.Protocol.Packets.Login.Serverbound;
using McProtoNet.Serialization;

internal class Program
{
    private static string[] _lines =
        "====================\n$$$$$$$$\\   $$\\ $$\\   \n$$  _____|  $$ \\$$ \\  \n$$ |      $$$$$$$$$$\\ \n$$$$$\\    \\_$$  $$   |\n$$  __|   $$$$$$$$$$\\ \n$$ |      \\_$$  $$  _|\n$$ |        $$ |$$ |  \n\\__|        \\__|\\__|  \n===================="
            .Split('\n');

    private static int linesIndex = 0;

    public static async Task Main(string[] args)
    {
        MinecraftVersion version = MinecraftVersion.V1_21_4;
        MinecraftClient client = new MinecraftClient(new MinecraftClientStartOptions()
        {
            ConnectTimeout = TimeSpan.FromSeconds(5),
            Host = "title-kde",
            Port = 25565,
            WriteTimeout = TimeSpan.FromSeconds(5),
            ReadTimeout = TimeSpan.FromSeconds(5),
            Version = (int)version
        });
        await client.ConnectAsync();
        
        
        
        var hand = new SetProtocolPacket()
        {
            NextState = 2,
            ProtocolVersion = (int)version,
            ServerHost = "title-kde",
            ServerPort = 25565
        };
        var login = new LoginStartPacket.V764_769
        {
            PlayerUUID = Guid.NewGuid(),
            Username = "TestBot"
        };
        
        {
            var writer = new MinecraftPrimitiveWriter();
            writer.WriteVarInt(0x00);
            
            hand.Serialize(ref writer, 769);
            var memory = writer.GetWrittenMemory();
            Console.WriteLine($"Serialized [{string.Join(", ", memory.Memory.ToArray())}]");
            writer.Dispose();
            MemoryStream ms = new();
            MinecraftPacketSender sender = new MinecraftPacketSender
            {
                BaseStream = ms
            };
            await sender.SendPacketAsync(new OutputPacket(memory));
            memory.Dispose();

            Console.WriteLine($"Sent [{string.Join(", ", ms.ToArray())}]");
        }

        {
            var writer = new MinecraftPrimitiveWriter();
            writer.WriteVarInt(0x00);

            

            login.Serialize(ref writer, 769);
            var memory = writer.GetWrittenMemory();
            Console.WriteLine($"Serialized [{string.Join(", ", memory.Memory.ToArray())}]");
            MemoryStream ms = new();
            MinecraftPacketSender sender = new MinecraftPacketSender
            {
                BaseStream = ms
            };
            await sender.SendPacketAsync(new OutputPacket(memory));
            memory.Dispose();

            Console.WriteLine($"Sent [{string.Join(", ", ms.ToArray())}]");

            //writer.Dispose();
        }
        Console.WriteLine();

        await client.SendPacket(hand);
        await client.SendPacket(login);
        await Task.Delay(-1);
    }
}