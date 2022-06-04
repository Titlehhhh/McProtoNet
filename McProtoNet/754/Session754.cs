﻿using McProtoNet.Core;
using McProtoNet.Core.Packets;
using McProtoNet.Core.Protocol;
using McProtoNet.Protocol754;
using McProtoNet.Protocol754.Packets;
using McProtoNet.Protocol754.Packets.Client;
using McProtoNet.Protocol754.Packets.Server;
using McProtoNet.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McProtoNet._754
{
    public class Session754 : IDisposable, ISession
    {



        private static readonly IPacketCollection p754 = new PacketCollection754();
        private IPacketRepository packetRepository;
        private IPacketReaderWriter client;
        private PacketCategory subProtocol;

        public Session754(IPacketReaderWriter client)
        {

            this.packetRepository = CreatePacketRepository();
            this.client = client;
            client.OnPacketReceived += Client_OnPacketReceived;
            client.OnPacketSent += Client_OnPacketSent;
            client.OnPacketSend += Client_OnPacketSend;
            client.OnConnectionLost += Client_OnConnectionLost;
            client.OnError += Client_OnError;
        }



        private static IPacketRepository CreatePacketRepository()
        {
            IPacketProvider handskae =
                new PacketProvider(new Dictionary<Type, int>()
                {
                    {typeof(HandShakePacket),0x00 }
                }, new());
            IPacketProvider login =
                new PacketProvider(p754.GetClientPacketsByCategory(PacketCategory.Login),
                p754.GetServerPacketsByCategory(PacketCategory.Login));
            IPacketProvider game =
                new PacketProvider(p754.GetClientPacketsByCategory(PacketCategory.Game),
                p754.GetServerPacketsByCategory(PacketCategory.Game));

            IPacketRepository repository = new PacketRepository(new Dictionary<PacketCategory, IPacketProvider>()
            {
                {PacketCategory.HandShake, handskae },
                {PacketCategory.Login, login },
                {PacketCategory.Game, game }
            });
            return repository;
        }

        private PacketCategory SubProtocol
        {
            get => subProtocol;
            set
            {
                subProtocol = value;
                client.Packets = packetRepository.GetPackets(subProtocol);
            }
        }

        private void Client_OnError(IPacketReaderWriter client, Exception exception)
        {
            UnRegisterEvents();
            taskCompletion.SetResult(false);
        }

        private void Client_OnConnectionLost(IPacketReaderWriter client)
        {
            UnRegisterEvents();
            taskCompletion.SetResult(false);
        }

        private void Client_OnPacketSend(IPacketReaderWriter client, Packet packet)
        {

        }

        private void Client_OnPacketSent(IPacketReaderWriter client, Packet packet)
        {

        }

        private void Client_OnPacketReceived(IPacketReaderWriter client, Packet packet)
        {

            if (SubProtocol == PacketCategory.Login)
            {
                if (packet is EncryptionRequestPacket requestPacket)
                {
                    var RSAService = CryptoHandler.DecodeRSAPublicKey(requestPacket.PublicKey);
                    var privateKey = CryptoHandler.GenerateAESPrivateKey();

                    var key_enc = RSAService.Encrypt(privateKey, false);
                    var token_enc = RSAService.Encrypt(requestPacket.VerifyToken, false);
                    var response = new EncryptionResponsePacket(key_enc, token_enc);
                    client.SendPacket(response).Wait();


                    client.SetEncryption(privateKey);


                }
                else if (packet is LoginSetCompressionPacket compressionPacket)
                {

                    client.SetCompressionThreshold(compressionPacket.Threshold);

                }
                else if (packet is LoginSuccessPacket successPacket)
                {
                    SubProtocol = PacketCategory.Game;
                    taskCompletion.SetResult(true);
                }
                else if (packet is LoginDisconnectPacket disconnectPacket)
                {
                    client.Disconnect();
                    UnRegisterEvents();
                    taskCompletion.SetResult(false);
                }
            }
            else if (SubProtocol == PacketCategory.Game)
            {
                if (packet is ServerDisconnectPacket disconnectPacket)
                {
                    client.Disconnect();
                    UnRegisterEvents();
                }
                else if (packet is ServerKeepAlivePacket keepAlivePacket)
                {
                    client.QueuePacket(new ClientKeepAlivePacket(keepAlivePacket.PingID));
                }
            }
        }

        private void UnRegisterEvents()
        {
            client.OnPacketReceived -= Client_OnPacketReceived;
            client.OnPacketSent -= Client_OnPacketSent;
            client.OnPacketSend -= Client_OnPacketSend;
            client.OnConnectionLost -= Client_OnConnectionLost;
            client.OnError -= Client_OnError;
        }

        public void Dispose()
        {
            UnRegisterEvents();
            client.Dispose();
            client = null;
            packetRepository.Dispose();
            packetRepository = null;
            GC.SuppressFinalize(this);
        }
        private TaskCompletionSource<bool> taskCompletion;
        public async Task<bool> Login()
        {
            taskCompletion = new();

            SubProtocol = PacketCategory.HandShake;
            try
            {
                client.Start();
                await client.SendPacket(new HandShakePacket(HandShakeIntent.LOGIN, 754, "", 0));
                SubProtocol = PacketCategory.Login;
                await client.SendPacket(new LoginStartPacket("TestBot"));
            }
            catch (Exception e)
            {
                return false;
            }
            return await taskCompletion.Task;
        }

        public void Close()
        {
            throw new NotImplementedException();
        }
    }
}
