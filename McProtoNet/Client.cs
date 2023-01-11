using McProtoNet.Core;
using McProtoNet.Core.Protocol;
using System.Net;

namespace McProtoNet
{
    public class Client<TProto> : IClient<TProto> where TProto : IProtocol, new()
    {
        public EndPoint Address { get; private set; }

        public PacketCategory CurrentCategory
        {
            get => _listener.CurrentCategory;
            set => _listener.CurrentCategory = value;
        }

        private PacketListener<TProto> _listener;

        public event ClientDisconnected<TProto> Disconnected;
        public event ClientPacketReceivedHandler<TProto> PacketReceived;

        public Client(EndPoint address, PacketListener<TProto> listener)
        {
            Address = address;
            _listener = listener;
           
            _listener.OnError += Listener_OnError;
            _listener.PacketReceived += _listener_PacketReceived;
           
        }
        public void Start()
        {
            _listener.Start();
        }

        public void SendPacket(MinecraftPacket<TProto> packet)
        {
            try
            {
                _listener.SendPacket(packet);
            }
            catch (Exception e)
            {
                DoDisconnect(e);
            }
        }

        private void Listener_OnError(PacketListener<TProto> sender, Exception exception)
        {
            DoDisconnect(exception);
        }
        private void _listener_PacketReceived(PacketListener<TProto> sender, MinecraftPacket<TProto> packet)
        {
            this.PacketReceived?.Invoke(this, packet);
        }


        private void DoDisconnect(Exception? exception)
        {
            UnsubscribeEvents();
            _listener.Stop();
            this.Disconnected?.Invoke(this, exception);
            Dispose();
        }
        public void Disconnect()
        {
            DoDisconnect(null);
        }
        private void UnsubscribeEvents()
        {
            _listener.OnError -= Listener_OnError;
            _listener.PacketReceived -= _listener_PacketReceived;
        }
        private bool disposed = false;
        public void Dispose()
        {
            if (disposed)
                return;
            disposed = true;
            
            _listener?.Dispose();
            _listener = null;
            Address = null;
            
            GC.SuppressFinalize(this);
        }

        public void SwitchEncryption(byte[] key)
        {
            _listener.SwitchEncryption(key);
        }

        public void SwitchCompression(int threshold)
        {
            _listener.SwitchCompression(threshold);
        }

        
    }
}
