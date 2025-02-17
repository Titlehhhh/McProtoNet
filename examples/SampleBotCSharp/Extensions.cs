using McProtoNet.Abstractions;
using McProtoNet.Cryptography;
using McProtoNet.Protocol;
using McProtoNet.Protocol.Packets.Handshaking.Serverbound;
using CConfig = McProtoNet.Protocol.Packets.Configuration.Clientbound;
using SConfig = McProtoNet.Protocol.Packets.Configuration.Serverbound;
using CLogin = McProtoNet.Protocol.Packets.Login.Clientbound;
using McProtoNet.Protocol.Packets.Login.Serverbound;

namespace SampleBotCSharp;

public static class Extensions
{
    public static async Task Login(this IMinecraftClient client, string nickname, string host, ushort port)
    {
        await client.SendPacket(new SetProtocolPacket()
        {
            NextState = 2,
            ProtocolVersion = client.ProtocolVersion,
            ServerHost = host,
            ServerPort = port
        });

        await client.SendPacket(
            new LoginStartPacket
            {
                Username = nickname
            });

        await foreach (var serverPacket in client.OnAllPackets(PacketState.Login))
        {
            if (serverPacket is CLogin.CompressPacket compressPacket)
            {
                client.SwitchCompression(compressPacket.Threshold);
            }
            else if (serverPacket is CLogin.SuccessPacket)
            {
                break;
            }
            else if (serverPacket is CLogin.DisconnectPacket disconnectPacket)
            {
                Console.WriteLine("Disconnect: " + disconnectPacket.Reason);
            }
            else if (serverPacket is CLogin.EncryptionBeginPacket encryptBegin)
            {
                var RSAService = CryptoHandler.DecodeRSAPublicKey(encryptBegin.PublicKey);
                var secretKey = CryptoHandler.GenerateAESPrivateKey();


                var sharedSecret = RSAService.Encrypt(secretKey, false);
                var verifyToken = RSAService.Encrypt(encryptBegin.VerifyToken, false);

                var response = new EncryptionBeginPacket
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
            await client.SendPacket(new LoginAcknowledgedPacket());
            
            await client.SendPacket(new SConfig.SettingsPacket()
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
                    // Succes configuration
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