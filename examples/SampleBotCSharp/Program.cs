using System.Reactive.Linq;
using System.Security.Cryptography;
using McProtoNet;
using McProtoNet.Abstractions;
using McProtoNet.Client;
using McProtoNet.Net;
using McProtoNet.Protocol;
using McProtoNet.Protocol.Packets.Configuration.Serverbound;
using McProtoNet.Protocol.Packets.Handshaking.Serverbound;
using McProtoNet.Protocol.Packets.Login.Clientbound;
using McProtoNet.Protocol.Packets.Login.Serverbound;
using McProtoNet.Serialization;
using CConfig = McProtoNet.Protocol.Packets.Configuration.Clientbound;
using SConfig = McProtoNet.Protocol.Packets.Configuration.Serverbound;
using CPlay = McProtoNet.Protocol.Packets.Play.Clientbound;
using SPlay = McProtoNet.Protocol.Packets.Play.Serverbound;

internal class Program
{
    private static string[] _lines =
        "====================\n$$$$$$$$\\   $$\\ $$\\   \n$$  _____|  $$ \\$$ \\  \n$$ |      $$$$$$$$$$\\ \n$$$$$\\    \\_$$  $$   |\n$$  __|   $$$$$$$$$$\\ \n$$ |      \\_$$  $$  _|\n$$ |        $$ |$$ |  \n\\__|        \\__|\\__|  \n===================="
            .Split('\n');

    private static int linesIndex = 0;

    public static async Task Main(string[] args)
    {
        Console.WriteLine("Start");

        List<Task> tasks = new List<Task>(100);
        MinecraftVersion version = MinecraftVersion.V1_21_4;
        for (int i = 1; i <= 30; i++)
        {
            Task t = RunBot(version, $"Title_{i:D3}");
            tasks.Add(t);
        }

        await Task.WhenAll(tasks);


        await Task.Delay(-1);
    }

    private static async Task RunBot(MinecraftVersion version, string nickname)
    {
        while (true)
        {
            Console.WriteLine($"Start Bot: {nickname}");
            try
            {
                MinecraftClient client = new MinecraftClient(new MinecraftClientStartOptions()
                {
                    ConnectTimeout = TimeSpan.FromSeconds(5),
                    Host = "title-kde",
                    Port = 25565,
                    WriteTimeout = TimeSpan.FromSeconds(5),
                    ReadTimeout = TimeSpan.FromSeconds(5),
                    Version = (int)version,
                    SendQueueSize = -1,
                    ReceiveQueueSize = 10
                });

                await client.ConnectAsync();
                await Login(client, nickname);

                Task read = Task.Run(async () =>
                {
                    await foreach (var packet in client.ReceivePackets(default))
                    {
                        if (packet.TryDeserialize<CPlay.KeepAlivePacket>(client.ProtocolVersion, out var p))
                        {
                            //Console.WriteLine("KeepAlive");
                            await client.SendPacket(new SPlay.KeepAlivePacket()
                            {
                                KeepAliveId = p.KeepAliveId
                            });
                        }
                    }
                });
                await Task.Delay(5000);


                await Task.Run(async () =>
                {
                    try
                    {
                        while (true)
                        {


                            if (client.TrySend<SPlay.ChatPacket>(out var sender))
                            {
                                //Console.WriteLine("V1");
                                sender.Packet.Message = "Helloworld";
                                await sender.Send();
                            }
                            else if (client.TrySend<SPlay.ChatMessagePacket>(out var sender2))
                            {
                                // Console.WriteLine("V2");
                                sender2.Packet.Message = "Helloworld";
                                sender2.Packet.Timestamp = Random.Shared.NextInt64();
                                sender2.Packet.Salt = Random.Shared.NextInt64();
                                await sender2.Send();
                            }

                            await Task.Delay(1500);
                        }
                    }
                    catch (Exception e)
                    {

                    }
                });


                await client.Completion;
            }
            catch (Exception e)
            {
            }

            await Task.Delay(1500);
        }
    }

    private static async Task Login(MinecraftClient client, string nickname)
    {
        var hand = new SetProtocolPacket()
        {
            NextState = 2,
            ProtocolVersion = client.ProtocolVersion,
            ServerHost = "title-kde",
            ServerPort = 25565
        };
        var login = new LoginStartPacket.V764_769
        {
            PlayerUUID = Guid.NewGuid(),
            Username = nickname
        };


        await client.SendPacket(hand);
        await client.SendPacket(login);
        
        await foreach (var serverPacket in client.OnAllPackets(PacketState.Login))
        {
            if (serverPacket is CompressPacket compressPacket)
            {
                client.SwitchCompression(compressPacket.Threshold);
            }
            else if (serverPacket is SuccessPacket)
            {
                if (client.ProtocolVersion >= 764)
                {
                    await client.SendPacket(new LoginAcknowledgedPacket());
                }

                break;
            }
            else if (serverPacket is DisconnectPacket disconnectPacket)
            {
                Console.WriteLine("Disconnect: " + disconnectPacket.Reason);
            }
        }


        if (client.ProtocolVersion >= 764)
        {
            await client.SendPacket(new SettingsPacket()
            {
                Locale = "ru_ru",
                ViewDistance = 16,
                EnableServerListing = false,
                EnableTextFiltering = false,
                MainHand = 0,
                SkinParts = 127,
                ChatColors = true,
                ChatFlags = 0
            });


            await foreach (var packet in client.OnAllPackets(PacketState.Configuration))
            {
                if (packet is CConfig.FinishConfigurationPacket)
                {
                    await client.SendPacket(new SConfig.FinishConfigurationPacket());
                    break;
                }

                if (packet is CConfig.KeepAlivePacket.V764_769 keepAlivePacket)
                {
                    await client.SendPacket(new SConfig.KeepAlivePacket()
                    {
                        KeepAliveId = keepAlivePacket.KeepAliveId
                    });
                }
                else if (packet is CConfig.PingPacket.V764_769 pingPacket)
                {
                    await client.SendPacket(new SConfig.PongPacket()
                    {
                        Id = pingPacket.Id
                    });
                }
                else if (packet is CConfig.CustomPayloadPacket.V764_769 payload)
                {
                    await client.SendPacket(new SConfig.CustomPayloadPacket.V764_769()
                    {
                        Data = payload.Data,
                        Channel = payload.Channel
                    });
                }
                else if (packet is CConfig.SelectKnownPacksPacket)
                {
                    await client.SendPacket(new SConfig.SelectKnownPacksPacket());
                }
            }
        }
    }
}