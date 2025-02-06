using System.Collections.Concurrent;
using System.Numerics;
using System.Runtime.Intrinsics.Arm;
using System.Runtime.Intrinsics.X86;
using DotNext;
using McProtoNet.Abstractions;
using McProtoNet.Client;
using McProtoNet.Protocol;
using McProtoNet.Protocol.Packets.Configuration.Serverbound;
using CPlay = McProtoNet.Protocol.Packets.Play.Clientbound;
using SPlay = McProtoNet.Protocol.Packets.Play.Serverbound;

namespace SampleBotCSharp;

public class Bot
{
    private MinecraftVersion _version = MinecraftVersion.V1_21_4;

    //public static ConcurrentDictionary<int, int> bits = new();

    private MinecraftClient _client;

    public async Task Start()
    {
        _client = new MinecraftClient(new MinecraftClientStartOptions()
        {
            ConnectTimeout = TimeSpan.FromSeconds(5),
            Host = "title-kde",
            Port = 25565,
            WriteTimeout = TimeSpan.FromSeconds(5),
            ReadTimeout = TimeSpan.FromSeconds(5),
            Version = (int)_version
        });

        await _client.ConnectAsync();
        await _client.Login("TestBot");
        _ = Task.Run(async () =>
        {
            try
            {
                Console.WriteLine("Start Play");
                await foreach (var packet in _client.OnAllPackets(PacketState.Play))
                {
                    HandlePlayPacket(packet);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Read packets: {ex}");
                throw;
            }
            finally
            {
                Console.WriteLine("Stop Play");
            }
        });
    }

    private void HandlePlayPacket(IServerPacket packet)
    {
        try
        {
            if (packet is CPlay.KeepAlivePacket keepAlive)
            {
                Console.WriteLine("KeepAlive");
                _client.SendPacket(new SPlay.KeepAlivePacket()
                {
                    KeepAliveId = keepAlive.KeepAliveId
                });
            }
            else if (packet is CPlay.MapChunkPacket mapChunkPacket)
            {
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

}