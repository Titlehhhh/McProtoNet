using McProtoNet.Core;
using McProtoNet.Core.Protocol;
using System.Net.Sockets;

namespace McProtoNet
{

    public class PacketListener: IDisposable
    {
        
        private IPacketReaderWriter packetReaderWriter;
        public void SwitchEncryption(byte[] key)
        {
            packetReaderWriter.SwitchEncryption(key);
        }
        public void SwitchCompression(int threshold)
        {
            packetReaderWriter.SwitchCompression(threshold);
        }

        public PacketListener(IPacketReaderWriter packetReaderWriter)
        {
            this.packetReaderWriter = packetReaderWriter;
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

                    MinecraftPacket packet = packetReaderWriter.ReadNextPacket();
                    //this.PacketReceived?.Invoke(this, packet);

                }
            }
            catch (Exception e)
            {
                if (!CTS.IsCancellationRequested)
                    //OnError?.Invoke(this, e);
            }
        }
        public void SendPacket(MinecraftPacket packet)
        {
            packetReaderWriter.SendPacket(packet);
        }
        public void Stop()
        {            
            CTS.Cancel();           
            netThread.Join();
            Dispose();
        }
        private bool disposed = false;
        public void Dispose()
        {
            if (disposed)
                return;
            disposed = true;
            tcpClient?.Dispose();
            minecraftProtocol?.Dispose();
            packetReaderWriter?.Dispose();
            CTS?.Dispose();
            netThread = null;
            minecraftProtocol = null;
            packetReaderWriter = null;
            tcpClient = null;
            CTS = null;

            GC.SuppressFinalize(this);
        }

        public event PacketReceivedHandler<TProto> PacketReceived;
        public event OnErrorHandler<TProto> OnError;
    }

}
