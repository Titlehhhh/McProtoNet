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
        Console.WriteLine("Connecting");
        await client.ConnectAsync();
        Console.WriteLine("Connected");

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
        var loginRead = client.OnAllPackets(PacketState.Login);
        await client.SendPacket(hand);
        await client.SendPacket(login);

        await foreach (var serverPacket in loginRead.ToAsyncEnumerable())
        {
            Console.WriteLine("Receive: " + serverPacket.GetPacketId());
            if (serverPacket is CompressPacket compressPacket)
            {
                Console.WriteLine("Threshold: " + compressPacket.Threshold);
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
        }

        Console.WriteLine("Success!");

        if (client.ProtocolVersion >= 764)
        {
            Console.WriteLine("Start config");


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

            // await foreach (var pack in client.ReceivePackets.ToAsyncEnumerable())
            // {
            //     Console.WriteLine($"Receive: 0x{pack.Id:X2}");
            // }

            await foreach (var packet in client.OnAllPackets(PacketState.Configuration).ToAsyncEnumerable())
            {
                Console.WriteLine("Receive packet:" + packet.GetPacketId());
                if (packet is CConfig.FinishConfigurationPacket)
                {
                    Console.WriteLine("Finish");
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

            client.OnPacket<CPlay.KeepAlivePacket>().Subscribe(p =>
            {
                client.SendPacket(new SPlay.KeepAlivePacket()
                {
                    KeepAliveId = p.KeepAliveId
                });
            });
            Console.WriteLine("Wait");
            await Task.Delay(3000);
            Console.WriteLine("asdsad");

            await client.SendPacket(new SPlay.ArmAnimationPacket()
            {
                Hand = 0
            });
            
            await Task.Delay(5000);
            
            if (client.TrySend<SPlay.ChatPacket>(out var sender))
            {
                Console.WriteLine("V1");
                sender.Packet.Message = "Helloworld";
                await sender.Send();
            }
            else if (client.TrySend<SPlay.ChatMessagePacket>(out var sender2))
            {
                Console.WriteLine("V2");
                sender2.Packet.Message = "Helloworld";
                sender2.Packet.Timestamp = Random.Shared.NextInt64();
                sender2.Packet.Salt = Random.Shared.NextInt64();
                await sender2.Send();
            }
        }

        await Task.Delay(-1);
    }
}