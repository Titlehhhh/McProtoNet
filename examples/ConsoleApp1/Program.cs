using System.Net.Sockets;
using System.Threading.Channels;
using McProtoNet.Abstractions;
using McProtoNet.Protocol;
using McProtoNet.Serialization;

//Random bytes
byte[] data = [0x00, 0x15, 0x16, 0x01, 0x54];

MinecraftPrimitiveReader reader = new MinecraftPrimitiveReader(new ReadOnlySpan<byte>(data));
reader.ReadVarInt();
Console.WriteLine($"Remaining: {reader.RemainingCount}");


MinecraftPrimitiveWriter writer = new MinecraftPrimitiveWriter();

try
{
    writer.WriteVarInt(111);
    writer.WriteString("Hello");
    var memory = writer.GetWrittenMemory();
    Console.WriteLine(string.Join(", ", memory.Memory));
}
finally
{
    writer.Dispose();
}

