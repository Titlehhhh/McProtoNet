using McProtoNet.Core;
using McProtoNet.Core.Packets;
using McProtoNet.Core.Protocol;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.Sockets;

namespace McProtoNet
{
    public delegate void PacketReceivedHandler<TPack>(PacketListener<TPack> sender, MinecraftPacket<TPack> packet) where TPack : IProtocol, new();
    public delegate void OnErrorHandler<TPack>(PacketListener<TPack> sender, Exception exception) where TPack : IProtocol, new();
    public class PacketListener<TProto> : IDisposable where TProto : IProtocol, new()
    {
        private IMinecraftProtocol minecraftProtocol;
        private IPacketReaderWriter<TProto> packetReaderWriter;
        public PacketCategory CurrentCategory { get; set; }
        public void SwitchEncryption(byte[] key)
        {
            minecraftProtocol.SwitchEncryption(key);
        }
        public void SwitchCompression(int threshold)
        {
            minecraftProtocol.SwitchCompression(threshold);
        }
        private TcpClient tcpClient;
        public PacketListener(TcpClient tcpClient, PacketSide side)
        {
            this.tcpClient = tcpClient;
            minecraftProtocol = new MinecraftProtocol(tcpClient);
            packetReaderWriter = new PacketReaderWriter<TProto>(side, minecraftProtocol);
        }
        private Thread netThread;
        private CancellationTokenSource CTS = new();
        public void Start()
        {
            netThread = new Thread(Init);
            netThread.Start();
        }
        private void Init()
        {
            try
            {
                while (!CTS.IsCancellationRequested)
                {

                    MinecraftPacket<TProto> packet = packetReaderWriter.ReadNextPacket(CurrentCategory);
                    this.PacketReceived?.Invoke(this, packet);

                }
            }
            catch (Exception e)
            {
                OnError?.Invoke(this, e);
            }
        }
        public void SendPacket(MinecraftPacket<TProto> packet)
        {
            packetReaderWriter.SendPacket(packet, CurrentCategory);
        }
        public void Stop()
        {
            CTS.Cancel();
        }

        public void Dispose()
        {
            minecraftProtocol.Dispose();
            packetReaderWriter.Dispose();
            CTS.Dispose();
        }

        public event PacketReceivedHandler<TProto> PacketReceived;
        public event OnErrorHandler<TProto> OnError;
    }

}
