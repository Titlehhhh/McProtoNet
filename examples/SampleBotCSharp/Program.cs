using McProtoNet.Protocol.Extensions;
using McProtoNet.Serialization;

namespace SampleBotCSharp;

class Program
{
    public static async Task Main(string[] args)
    {
        byte[] data = new byte[500];
        var reader = new MinecraftPrimitiveReader(data);

        var pos = reader.ReadPosition(50);
    }
}