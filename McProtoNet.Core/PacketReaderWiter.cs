using McProtoNet.Core.IO;
using McProtoNet.Core.Packets;
using McProtoNet.Core.Protocol;
using System.Net.Sockets;
using System.Threading.Tasks.Dataflow;

namespace McProtoNet.Core
{
    public class PacketReaderWiter : IPacketReaderWriter
    {
        public Socket Client { get; private set; }

        private readonly IPacketProtocol packetProtocol;

        private readonly BufferBlock<(Packet, int)> packetQueue;

        private readonly CancellationTokenSource cancellationSource = new();

        private Task MainTask;
        private IPacketProvider packets;

        public PacketReaderWiter(Socket socket)
        {
            ArgumentNullException.ThrowIfNull(socket, nameof(socket));

            Client = socket;

            packetProtocol = new MinecraftProtocol(socket);

            var blockOptions = new ExecutionDataflowBlockOptions { CancellationToken = cancellationSource.Token, EnsureOrdered = true };
            packetQueue = new BufferBlock<(Packet, int)>(blockOptions);
            var sendPacketBlock = new ActionBlock<(Packet, int)>(item =>
            {
                (Packet packet, int id) = item;
                SendPacketActionAsync(packet, id).Wait();
            },
            blockOptions);

            var linkOptions = new DataflowLinkOptions { PropagateCompletion = true };
            packetQueue.LinkTo(sendPacketBlock, linkOptions);



        }




        private async Task MainLoop()
        {
            try
            {
                while (Connected && !cancellationSource.IsCancellationRequested)
                {

                    (int id, MemoryStream data) =
                     await packetProtocol.ReadNextPacketAsync(cancellationSource.Token);

                    if (Packets
                        .TryGetInputPacket(id, out Packet packet))
                    {
                        IMinecraftPrimitiveReader reader = new MinecraftPrimitiveReader(data);
                        packet.Read(reader);

                        OnPacketReceived?.Invoke(this, packet);
                    }
                }
            }
            catch (OperationCanceledException)
            {

            }
            catch (Exception e)
            {
                OnError?.Invoke(this, e);
            }
        }


        public bool Connected => Client.Connected;

        public bool EncryptionEnabled { get; private set; }

        public bool CompressionEnabled { get; private set; }

        public IPacketProvider Packets
        {
            get => packets;
            set
            {
                if (value is null)
                    throw new ArgumentNullException();
                packets = value;
            }
        }

        public event ErrorHandler OnError;
        public event DisconnectedHandler OnConnectionLost;
        public event PacketReceivedHandler OnPacketReceived;
        public event PacketSentHandler OnPacketSent;
        public event PacketSendHandler OnPacketSend;

        public void Disconnect()
        {
            cancellationSource.Cancel();
            MainTask.Wait();
        }

        public void Dispose()
        {
            cancellationSource.Dispose();
            Client.Dispose();
            GC.SuppressFinalize(this);
        }



        public void SetCompressionThreshold(int threshold)
        {
            if (CompressionEnabled)
                throw new InvalidOperationException("Сжатие уже включено");
            CompressionEnabled = true;
            packetProtocol.SwitchCompression(threshold);
        }

        public void SetEncryption(byte[] privateKey)
        {
            if (EncryptionEnabled)
                throw new InvalidOperationException("Шифрование уже включено");
            EncryptionEnabled = true;
            packetProtocol.SwitchEncryption(privateKey);
        }

        public void Start()
        {
            MainTask = MainLoop();
        }


        public async Task DisconnectAsync()
        {
            cancellationSource.Cancel();
            await MainTask;
        }

        private async Task SendPacketActionAsync(Packet packet, int id)
        {
            if (Client.Connected)
            {
                try
                {
                    OnPacketSend?.Invoke(this, packet);
                    await packetProtocol
                         .SendPacketAsync(packet, id, cancellationSource.Token);
                    OnPacketSent?.Invoke(this, packet);
                }
                catch (OperationCanceledException)
                {

                }
                catch (Exception e)
                {
                    OnError?.Invoke(this, e);
                }
            }
        }

        public async Task SendPacket(Packet packet)
        {
            if (Packets
                .TryGetOutputId(packet.GetType(), out int id))
            {
                await SendPacketActionAsync(packet, id);
            }
        }

        public async Task SendPacket(Packet packet, int id)
        {
            await SendPacketActionAsync(packet, id);
        }

        public void QueuePacket(Packet packet)
        {
            if (Packets
                            .TryGetOutputId(packet.GetType(), out int id))
            {
                packetQueue.SendAsync((packet, id));
            }
        }

        public void QueuePacket(Packet packet, int id)
        {
            packetQueue.SendAsync((packet, id));
        }
    }
}
