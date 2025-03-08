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

        await _client.ConnectAsync(default);
        await _client.Login("TestBot", _host, 25565);

        _ = RunReadLoop();
    }

    private async Task RunReadLoop()
    {
        await foreach (var packet in _client.OnAllPackets(PacketState.Play))
        {
            HandlePlayPacket(packet);
        }
    }

    private int _myEntityId;

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
            else if (packet is CPlay.UpdateHealthPacket hp)
            {
                if (hp.Health <= 0)
                {
                    Console.WriteLine("Меня убили!!!");
                    _client.SendPacket(new SPlay.ClientCommandPacket()
                    {
                        ActionId = 0
                    });
                }
            }
            else if (packet is CPlay.LoginPacket login)
            {
                _myEntityId = login.EntityId;
            }
            else if (packet is CPlay.EntityStatusPacket statusPacket)
            {
                //Console.WriteLine("Status: " + statusPacket.EntityStatus);
            }
            else if (packet is CPlay.EntityVelocityPacket velocityPacket)
            {
                if (velocityPacket.EntityId == _myEntityId)
                {
                    _ = SendChat("Ouch!!!");
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private async ValueTask SendChat(string message)
    {
        if (_client.TrySend<SPlay.ChatPacket>(out var sender1))
        {
            sender1.Packet.Message = message;
            await sender1.Send();
        }
        else if (_client.TrySend<SPlay.ChatMessagePacket>(out var sender2))
        {
            sender2.Packet.Message = message;
            await sender2.Send();
        }
    }
}

