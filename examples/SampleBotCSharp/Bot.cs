using McProtoNet.Abstractions;
using McProtoNet.Client;
using McProtoNet.Protocol;
using McProtoNet.Protocol.Packets.Configuration.Serverbound;
using CPlay = McProtoNet.Protocol.Packets.Play.Clientbound;
using SPlay = McProtoNet.Protocol.Packets.Play.Serverbound;

namespace SampleBotCSharp;

public class Bot
{
    private MinecraftVersion _version;

    //public static ConcurrentDictionary<int, int> bits = new();

    private MinecraftClient _client;
    private string _host;

    public Bot(MinecraftVersion version, string host)
    {
        _version = version;
        _host = host;
    }

    public async Task Start()
    {
        _client = new MinecraftClient(new MinecraftClientStartOptions()
        {
            ConnectTimeout = TimeSpan.FromSeconds(5),
            Host = _host,
            Port = 25565,
            WriteTimeout = TimeSpan.FromSeconds(5),
            ReadTimeout = TimeSpan.FromSeconds(5),
            Version = (int)_version
        });

        await _client.ConnectAsync();
        await _client.Login("TestBot", _host,25565);
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

    private int _entityId;

    private void HandlePlayPacket(IServerPacket packet)
    {
        try
        {
            if (packet is CPlay.KeepAlivePacket keepAlive)
            {
                _client.SendPacket(new SPlay.KeepAlivePacket()
                {
                    KeepAliveId = keepAlive.KeepAliveId
                });
            }
            else if (packet is CPlay.LoginPacket login)
            {
                _entityId = login.EntityId;
            }
            else if (packet is CPlay.EntityStatusPacket statusPacket)
            {
                //Console.WriteLine("Status: " + statusPacket.EntityStatus);
            }
            else if (packet is CPlay.EntityVelocityPacket velocityPacket)
            {
                if (velocityPacket.EntityId == _entityId)
                {
                    Console.WriteLine("Меня ударили!");
                }
            }
            
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}