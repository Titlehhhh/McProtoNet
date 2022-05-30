using McProtoNet.API;
using McProtoNet.API.Packets;
using McProtoNet.API.Protocol;
using System.Collections.Concurrent;
using System.Net.Sockets;

namespace McProtoNet.API
{

    public interface IPacketListener
    {
        void OnPacketReceived(IPacketReaderWriter client,Packet packet);
        void OnPacketSent(IPacketReaderWriter client, Packet packet);
        void OnPacketSend(IPacketReaderWriter client,Packet packet);
        void OnDisconnect(IPacketReaderWriter client,string reason);
        void OnError(IPacketReaderWriter client, Exception exception);
    }
    public delegate void DisconnectedHandler(IPacketReaderWriter client);
    public delegate void ErrorHandler(IPacketReaderWriter client, Exception exception);
    public delegate void PacketReceivedHandler(IPacketReaderWriter client, Packet packet);
    public delegate void PacketSentHandler(IPacketReaderWriter client, Packet packet);
    public delegate void PacketSendHandler(IPacketReaderWriter client, Packet packet);


    
    public interface IPacketReaderWriter : IDisposable
    {
        Socket Client { get;  }

        bool Connected { get; }

        IPacketProvider Packets { get; set; }

        void SetCompressionThreshold(int threshold);
        void SetEncryption(byte[] privateKey);

        bool EncryptionEnabled { get; }
        bool CompressionEnabled { get; }

        

        event ErrorHandler OnError;
        event DisconnectedHandler OnConnectionLost;
        event PacketReceivedHandler OnPacketReceived;
        event PacketSentHandler OnPacketSent;
        event PacketSendHandler OnPacketSend;

        void Start();

        Task SendPacket(Packet packet);
        Task SendPacket(Packet packet, int id);
        void QueuePacket(Packet packet);
        void QueuePacket(Packet packet, int id);

        void Disconnect();
        Task DisconnectAsync();

    }
}
