using McProtoNet.API;
using McProtoNet.API.IO;
using McProtoNet.API.Packets;
using McProtoNet.API.Protocol;
using McProtoNet.PacketRepository754.Packets.Server;
using McProtoNet.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace McProtoNet
{
    public class MinecraftClient754 : Client
    {
        public MinecraftClient754(TcpClient tcpClient, SessionToken session, IPacketDictionary packetDictionary, ISessionCheckService sessionCheckService) : base(tcpClient, session, packetDictionary, sessionCheckService)
        {
        }

        private async Task SendPacket(Packet packet, int id)
        {
            if (id < 0)
                throw new ArgumentOutOfRangeException(nameof(id), "меньше нуля");
            PacketSendEvent(packet);
            await packetReaderWriter
                .SendPacketAsync(packet, id, CancellationTokenSource.Token);
            PacketSentEvent(packet);
        }
        /*
         * this.registerOutgoing(0x00, LoginDisconnectPacket.class);
        this.registerOutgoing(0x01, EncryptionRequestPacket.class);
        this.registerOutgoing(0x02, LoginSuccessPacket.class);
        this.registerOutgoing(0x03, LoginSetCompressionPacket.class);
         */
        private Packet GetLoginPacket(int id, Stream data)
        {
            Packet packet = null;
            switch (id)
            {
                case 0x00:
                    packet = new LoginDisconnectPacket();
                    break;
                case 0x01:
                    packet = new EncryptionRequestPacket();
                    break;
                case 0x02:
                    packet = new LoginSuccessPacket();
                    break;
                case 0x03:
                    packet = new LoginSetCompressionPacket();
                    break;
            }
            IMinecraftPrimitiveReader reader = new MinecraftPrimitiveReader(data);
            packet.Read(reader);
            return packet;
        }

        private async Task<bool> HandleLoginPacket(Packet packet)
        {
            if (packet is EncryptionRequestPacket requestPacket)
            {
                var RSAService = CryptoHandler.DecodeRSAPublicKey(requestPacket.PublicKey);
                byte[] secretKey = CryptoHandler.GenerateAESPrivateKey();

                if (!await sessionCheckService.Check(_uuid, _sessionId,
                    CryptoHandler.getServerHash(requestPacket.ServerId, requestPacket.PublicKey, secretKey)))
                {
                    throw new InvalidOperationException("Session failed!");
                }

                byte[] key_enc = RSAService.Encrypt(secretKey, false);
                byte[] token_enc = RSAService.Encrypt(requestPacket.VerifyToken, false);

                await SendPacket(new EncryptionResponsePacket(key_enc, token_enc), 0x01);

                return false;
            }
            else if (packet is LoginSuccessPacket successPacket)
            {
                return true;
            }
            else if (packet is LoginSetCompressionPacket compressionPacket)
            {
                packetReaderWriter.SwitchCompression(compressionPacket.Threshold);
                return false;
            }
            throw new InvalidOperationException("Unkown packet");
        }

        protected override bool CheckPacket(Packet packet)
        {
            if (packet is ServerDisconnectPacket disconnectPacket)
            {
                tcpClient.Close();
                DisconnectedEvent(disconnectPacket.Message);
                return false;
            }
            return true;
        }

        protected override async void MainAction()
        {
            await SendPacket(
                new HandShakePacket(HandShakeIntent.LOGIN, 754, "", 0),
                0x00);
            await SendPacket(new LoginStartPacket(_nick), 0x00);

            bool login = false;

            while (!login)
            {
                (int id, MemoryStream data) = await packetReaderWriter
                    .ReadNextPacketAsync(CancellationTokenSource.Token);
                if (id == 0x00)
                {
                    var disconnect = (LoginDisconnectPacket)GetLoginPacket(id, data);

                    tcpClient.Close();
                    LoginRejectedEvent(disconnect.Message);
                    return;
                }

                login = await HandleLoginPacket(GetLoginPacket(id, data));
            }


            base.MainAction();
        }

    }
}
