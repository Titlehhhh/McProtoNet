using System.Diagnostics;
using System.Numerics;
using System.Reactive.Linq;
using System.Runtime.Intrinsics.X86;
using System.Security.Cryptography;
using DotNext.Diagnostics;
using McProtoNet;
using McProtoNet.Abstractions;
using McProtoNet.Client;
using McProtoNet.Cryptography;
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
using CLogin = McProtoNet.Protocol.Packets.Login.Clientbound;
using SLogin = McProtoNet.Protocol.Packets.Login.Serverbound;

internal class Program
{
    public static string Figure =
        """
        _###___#__#_
        #___#__#__#_
        #_____######
        #______#__#_
        #______#__#_
        #_____######
        #___#__#__#_
        _###___#__#_
        """;

    private static int linesIndex = 0;

    public static async Task Main(string[] args)
    {
        Console.WriteLine("Start");

        Figure = Figure.Replace("\n", "").Replace("\r", "");

        var count = Figure.Count(x => x == '#');


        List<Task> tasks = new List<Task>(100);
        MinecraftVersion version = MinecraftVersion.V1_21_4;
        for (int i = 0; i < 100; i++)
        {
            Task t = RunBot(version, $"Title_{i:D3}", i);
            tasks.Add(t);
        }

        await Task.WhenAll(tasks);


        await Task.Delay(-1);
    }

    private static async Task RunBot(MinecraftVersion version, string nickname, int id)
    {
        while (true)
        {
            Console.WriteLine($"Start Bot: {nickname}");

            var client = new MinecraftClient(new MinecraftClientStartOptions()
            {
                ConnectTimeout = TimeSpan.FromSeconds(5),
                Host = "localhost",
                Port = 25565,
                WriteTimeout = TimeSpan.FromSeconds(5),
                ReadTimeout = TimeSpan.FromSeconds(5),
                Version = (int)version
            });

            await client.ConnectAsync();
            await Login(client, nickname);
            Vector3 myPos = Vector3.Zero;
            bool first = true;

            _ = Task.Run(async () =>
            {
                try
                {
                    await client.Completion;
                    Console.WriteLine("Success completion");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Completion: {e}");
                }
            });
            try
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
                    else if (packet.TryDeserialize<CPlay.PositionPacket.V768_769>(client.ProtocolVersion,
                                 out var position))
                    {
                        myPos = new Vector3(position.X, position.Y, position.Z);
                        await client.SendPacket(new SPlay.TeleportConfirmPacket()
                        {
                            TeleportId = position.TeleportId
                        });
                        if (first)
                        {
                            first = false;
                            await client.SendPacket(new SPlay.PositionLookPacket.V768_769()
                            {
                                X = position.X,
                                Y = position.Y,
                                Z = position.Z,
                                Yaw = 0,
                                Pitch = 0,
                                Flags = 0x01
                            });
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
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
            //Console.WriteLine($"Read Login: {serverPacket.GetPacketId()}");
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
            else if (serverPacket is CLogin.EncryptionBeginPacket encryptBegin)
            {
                var RSAService = CryptoHandler.DecodeRSAPublicKey(encryptBegin.PublicKey);
                var secretKey = CryptoHandler.GenerateAESPrivateKey();


                var sharedSecret = RSAService.Encrypt(secretKey, false);
                var verifyToken = RSAService.Encrypt(encryptBegin.VerifyToken, false);

                var response = new SLogin.EncryptionBeginPacket.V761_769()
                {
                    SharedSecret = sharedSecret,
                    VerifyToken = verifyToken
                };

                await client.SendPacket(response);

                client.SwitchEncryption(secretKey);
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
                // Console.WriteLine($"Read Configuration: {packet.GetPacketId()}");
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