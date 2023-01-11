using McProtoNet.Core;
using McProtoNet.Core.Protocol;
using System.Net;

namespace McProtoNet
{
    public interface IClient<TProto> : IDisposable where TProto : IProtocol, new()
    {
        public event ClientDisconnected<TProto> Disconnected;
        public event ClientPacketReceivedHandler<TProto> PacketReceived;
        public void Disconnect();
        public void Start();
        public EndPoint Address { get; }
        public PacketCategory CurrentCategory { get; set; }
        public void SendPacket(MinecraftPacket<TProto> packet);

        public void SwitchEncryption(byte[] key);
        public void SwitchCompression(int threshold);

    }
}
