using McProtoNet.Core.Packets;
using McProtoNet.Utils;

namespace McProtoNet.Protocol340
{
    public sealed class Loginizer340
    {
        private readonly MinecraftPrimitiveReader primitiveReader = new();


        private readonly string sessionId = "";
        private bool IsOnlineMode = false;
        private readonly string username;

        private readonly IPacketProtocol client;
        private readonly IPacketProvider loginPackets;

        public Loginizer340(string username, IPacketProtocol client, IPacketProvider loginPackets)
        {
            this.username = username;
            this.loginPackets = loginPackets;
            this.client = client;
        }
        public Loginizer340(string username, IPacketProtocol client, IPacketProvider loginPackets, string sessionId) : this(username, client, loginPackets)
        {
            IsOnlineMode = true;
            this.sessionId = sessionId;
        }


        public async Task<Guid> Login()
        {
            return await Task.Run<Guid>(() =>
            {
                bool need = true;
                Guid? uuid = null;
                try
                {
                    QueuePacket(new LoginStartPacket(username));

                    while (need)
                    {
                        (int id, MemoryStream data) = client.ReadNextPacket();
                        if (loginPackets.TryGetInputPacket(id, out Packet packet))
                        {
                            primitiveReader.BaseStream = data;

                            packet.Read(primitiveReader);
                            uuid = HandleLoginPacket(packet);
                            if (uuid.HasValue)
                            {
                                need = false;
                            }
                        }
                    }

                }
                catch
                {
                    throw;
                }
                if (uuid.HasValue)
                    return uuid.Value;
                throw new Exception("Login Error");


            });
        }



        private void QueuePacket(Packet packet)
        {
            if (loginPackets.TryGetOutputId(packet.GetType(), out int id))
            {
                client.SendPacket(packet, id);
            }
        }

        private Guid? HandleLoginPacket(Packet packet)
        {
            if (packet is LoginSetCompressionPacket compressionPacket)
            {
                client.SwitchCompression(compressionPacket.Threshold);
            }
            else if (packet is EncryptionRequestPacket encryptionRequestPacket)
            {
                var RSAService = CryptoHandler.DecodeRSAPublicKey(encryptionRequestPacket.PublicKey);
                var privateKey = CryptoHandler.GenerateAESPrivateKey();

                if (IsOnlineMode)
                {
                    throw new Exception("Онлайн режим пока недоступен");
                }
                var key_enc = RSAService.Encrypt(privateKey, false);
                var token_enc = RSAService.Encrypt(encryptionRequestPacket.VerifyToken, false);
                var response = new EncryptionResponsePacket(key_enc, token_enc);
                QueuePacket(response);
                client.SwitchEncryption(privateKey);
            }
            else if (packet is LoginDisconnectPacket disconnectPacket)
            {
                throw new LoginRejectedException(disconnectPacket.Message);
            }
            else if (packet is LoginSuccessPacket successPacket)
            {
                return successPacket.UUID;
            }
            return null;
        }
    }
}
