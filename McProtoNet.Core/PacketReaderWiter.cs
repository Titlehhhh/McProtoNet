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

        private readonly CancellationTokenSource cancellationSource = new CancellationTokenSource();

        private Task MainTask;
        private IPacketProvider packets;

        public PacketReaderWiter(Socket socket)
        {
            if (socket is null)
                throw new ArgumentNullException(nameof(socket));

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
            ThrowIfDisposed();
            while (Connected && !cancellationSource.IsCancellationRequested)
            {
                try
                {

                    (int id, MemoryStream data) =
                        await Task.Run(packetProtocol.ReadNextPacket, cancellationSource.Token);

                    if (Packets
                        .TryGetInputPacket(id, out Packet packet))
                    {
                        IMinecraftPrimitiveReader reader = new MinecraftPrimitiveReader(data);
                        packet.Read(reader);

                        OnPacketReceived?.Invoke(this, packet);
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

        }


        public bool Connected => Client.Connected;

        public bool EncryptionEnabled { get; private set; }

        public bool CompressionEnabled { get; private set; }

        public IPacketProvider Packets
        {
            get => packets;
            set
            {
                ThrowIfDisposed();
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
            ThrowIfDisposed();
            cancellationSource.Cancel();
            MainTask.Wait();
        }




        public void SetCompressionThreshold(int threshold)
        {
            ThrowIfDisposed();
            if (CompressionEnabled)
                throw new InvalidOperationException("Сжатие уже включено");
            CompressionEnabled = true;
            packetProtocol.SwitchCompression(threshold);
        }

        public void SetEncryption(byte[] privateKey)
        {
            ThrowIfDisposed();
            if (EncryptionEnabled)
                throw new InvalidOperationException("Шифрование уже включено");
            EncryptionEnabled = true;
            packetProtocol.SwitchEncryption(privateKey);
        }

        public void Start()
        {
            ThrowIfDisposed();
            MainTask = MainLoop();
        }


        public async Task DisconnectAsync()
        {
            ThrowIfDisposed();
            cancellationSource.Cancel();
            await MainTask;
        }

        private async Task SendPacketActionAsync(Packet packet, int id)
        {
            ThrowIfDisposed();
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
            ThrowIfDisposed();
            if (Packets
                .TryGetOutputId(packet.GetType(), out int id))
            {
                await SendPacketActionAsync(packet, id);
            }
        }

        public async Task SendPacket(Packet packet, int id)
        {
            ThrowIfDisposed();
            await SendPacketActionAsync(packet, id);
        }

        public void QueuePacket(Packet packet)
        {
            ThrowIfDisposed();
            if (Packets
                            .TryGetOutputId(packet.GetType(), out int id))
            {
                packetQueue.SendAsync((packet, id));
            }
        }

        public void QueuePacket(Packet packet, int id)
        {
            ThrowIfDisposed();
            packetQueue.SendAsync((packet, id));
        }


        private bool _disposed = false;


        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        ~PacketReaderWiter()
        {
            Dispose(false);
        }
        private void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(PacketReaderWiter));
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                cancellationSource.Dispose();

            }

            packetProtocol.Dispose();
            Client.Dispose();
            _disposed = true;
        }
    }
}
